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
    public partial class frmMasterDetails : Form
    {
        List<ProductDetails> details = new List<ProductDetails>();
        string currentFile = "";
        public Form1 TheForm { get; set; }
        public frmMasterDetails()
        {
            InitializeComponent();
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

                MessageBox.Show("Please fill all requirment, " + ex.Message);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                currentFile = openFileDialog1.FileName;
            }
        }

        private void frmMasterDetails_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            LoadCityCombo();
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction trx = con.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.Transaction = trx;
                        string ext = Path.GetExtension(currentFile);
                        string f = Path.GetFileNameWithoutExtension(Guid.NewGuid().ToString()) + ext;

                        string savePath = @"..\..\Images\" + f;
                        MemoryStream ms = new MemoryStream(File.ReadAllBytes(currentFile));
                        byte[] bytes = ms.ToArray();
                        FileStream fs = new FileStream(savePath, FileMode.Create);
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Close();

                        cmd.CommandText = "INSERT INTO clients (clientName,dateOfBirth,insideDhaka,picture,photo,cityId) VALUES(@clientName,@dateOfBirth,@insideDhaka,@picture,@photo,@cityId); SELECT SCOPE_IDENTITY();";
                        cmd.Parameters.AddWithValue("@clientName", txtClientName.Text);
                        cmd.Parameters.AddWithValue("@dateOfBirth", dptDateOfBirth.Value);
                        cmd.Parameters.AddWithValue("@insideDhaka", chkInDhaka.Checked);
                        cmd.Parameters.AddWithValue("@picture", f);

                            //Image save to database
                            MemoryStream ms2 = new MemoryStream();
                            pictureBox1.Image.Save(ms2, pictureBox1.Image.RawFormat);
                            cmd.Parameters.AddWithValue("@photo", ms2.ToArray());
                            cmd.Parameters.AddWithValue("@cityId", cmbCity.SelectedValue);

                            //for Product Table
                            try
                        {
                            var cId = cmd.ExecuteScalar();
                            foreach (var c in details)
                            {
                                cmd.CommandText = "INSERT INTO products(productName,orderDate,deliveryDate,productPrice,clientId) VALUES(@productName,@orderDate,@deliveryDate,@productPrice,@clientId)";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@productName", c.productName);
                                cmd.Parameters.AddWithValue("@orderDate", c.orderDate);
                                cmd.Parameters.AddWithValue("@deliveryDate", c.deliveryDate);
                                cmd.Parameters.AddWithValue("@productPrice", c.productPrice);
                                cmd.Parameters.AddWithValue("@clientId", cId);
                                cmd.ExecuteNonQuery();
                                
                            }
                            trx.Commit();
                            MessageBox.Show("Data inserted successfully...!!!", "Success");
                            
                            
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
            catch (Exception)
            {

                MessageBox.Show("Product Details are not fill, Please fill and try again", "🔏");
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
    }

    public class ProductDetails
    {
        public string productName { get; set; }
        public DateTime orderDate { get; set; }
        public DateTime deliveryDate { get; set; }
        public decimal productPrice { get; set; }
    }
}
