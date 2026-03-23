using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1291387
{
    public partial class frmEdit : Form
    {
        List<ProductDetails> details = new List<ProductDetails>();
        string currentFile = "";
        string oldFile = "";
        public frmEdit()
        {
            InitializeComponent();
        }

        public frmShowData TheForm { get; set; }
        public int IdToEdit { get; set; }


        private void frmEdit_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            LoadCityCombo();
            LoadInForm();
            
        }

        private void LoadCityCombo()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                SqlDataAdapter sda = new SqlDataAdapter("SELECT id,cityName FROM cities", con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                cmbCity.DataSource = dt;
                cmbCity.ValueMember = "id";
                cmbCity.DisplayMember = "cityName";
            }
        }

        private void LoadInForm()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM clients WHERE clientId=@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", IdToEdit);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        txtClientName.Text = dr.GetString(1);
                        dptDateOfBirth.Value = dr.GetDateTime(2).Date;
                        chkInDhaka.Checked = dr.GetBoolean(3);
                        pictureBox1.Image = Image.FromFile(@"..\..\Images\" + dr.GetString(4));
                        oldFile = dr.GetString(4);
                        cmbCity.SelectedValue = dr.GetInt32(6);
                    }
                    dr.Close();
                    cmd.CommandText = "SELECT * FROM products WHERE clientId=@i";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@i", IdToEdit);
                    SqlDataReader dr2 = cmd.ExecuteReader();
                    while (dr2.Read())
                    {
                        details.Add(new ProductDetails
                        {
                            productName = dr2.GetString(1),
                            orderDate = dr2.GetDateTime(2).Date,
                            deliveryDate = dr2.GetDateTime(3).Date,
                            productPrice = dr2.GetDecimal(4)
                        });
                    }
                    SetDataSource();
                    con.Close();
                }
            }
        }

        private void SetDataSource()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = details;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                currentFile = openFileDialog1.FileName;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                details.RemoveAt(e.RowIndex);
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = details;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction trx = con.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.Transaction = trx;
                        string f = oldFile;

                        //Image save to folder
                        if (currentFile != "")
                        {
                            string ext = Path.GetExtension(currentFile);
                            f = Path.GetFileNameWithoutExtension(Guid.NewGuid().ToString()) + ext;

                            string savePath = @"..\..\Images\" + f;
                            MemoryStream ms = new MemoryStream(File.ReadAllBytes(currentFile));
                            byte[] bytes = ms.ToArray();
                            FileStream fs = new FileStream(savePath, FileMode.Create);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                        }
                        cmd.CommandText = "UPDATE clients SET clientName=@sn,dateOfBirth=@dob,insideDhaka=@isd,picture=@pic,photo=@photo,cityId=@cityId  WHERE clientId=@id";
                        cmd.Parameters.AddWithValue("@id", IdToEdit);
                        cmd.Parameters.AddWithValue("@sn", txtClientName.Text);
                        cmd.Parameters.AddWithValue("@dob", dptDateOfBirth.Value);
                        cmd.Parameters.AddWithValue("@isd", chkInDhaka.Checked);
                        cmd.Parameters.AddWithValue("@pic", f);

                        //Image save to database
                        MemoryStream ms2 = new MemoryStream();
                        pictureBox1.Image.Save(ms2, pictureBox1.Image.RawFormat);
                        cmd.Parameters.AddWithValue("@photo", ms2.ToArray());
                        cmd.Parameters.AddWithValue("@cityId", cmbCity.SelectedValue);

                        try
                        {
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "DELETE FROM products WHERE clientId=@id";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@id", IdToEdit);
                            cmd.ExecuteNonQuery();
                            foreach (var s in details)
                            {
                                cmd.CommandText = "INSERT INTO products(productName,orderDate,deliveryDate,productPrice,clientId) VALUES(@productName,@orderDate,@deliveryDate,@productPrice,@clientId)";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@productName", s.productName);
                                cmd.Parameters.AddWithValue("@orderDate", s.orderDate);
                                cmd.Parameters.AddWithValue("@deliveryDate", s.deliveryDate);
                                cmd.Parameters.AddWithValue("@productPrice", s.productPrice);
                                cmd.Parameters.AddWithValue("@clientId", IdToEdit);
                                cmd.ExecuteNonQuery();
                            }
                            trx.Commit();
                            TheForm.LoadDataBindingSource();
                            MessageBox.Show("Data updated successfully!!", "Success");
                        }
                        catch
                        {
                            trx.Rollback();
                        }
                    }
                }
                con.Close();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                details.Add(new ProductDetails
            {
                productName = txtProductName.Text,
                orderDate = dptOrderDate.Value,
                deliveryDate = dptDeleveryDate.Value,
                productPrice = Convert.ToDecimal(txtProductPrice.Text)
            });
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = details;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction trx = con.BeginTransaction())
                {

                    string sql = "DELETE FROM products WHERE clientId=@id";
                    using (SqlCommand cmd = new SqlCommand(sql, con, trx))
                    {
                        cmd.Parameters.AddWithValue("@id", IdToEdit);
                        try
                        {
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            cmd.CommandText = "DELETE FROM clients WHERE clientId=@id";
                            cmd.Parameters.AddWithValue("@id", IdToEdit);
                            cmd.ExecuteNonQuery();
                            trx.Commit();
                            MessageBox.Show("Data deleted successfully!!!", "Success");
                            TheForm.LoadDataBindingSource();
                            this.Close();
                        }
                        catch (Exception)
                        {
                            trx.Rollback();
                            MessageBox.Show("Failed to delete", "Error");
                        }
                    }
                    con.Close();
                }
            }
        }
    }
}
