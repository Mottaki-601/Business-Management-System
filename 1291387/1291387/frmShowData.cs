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
    public partial class frmShowData : Form
    {
        //Disconnected Class
        BindingSource bsS = new BindingSource();
        BindingSource bsC = new BindingSource();
        DataSet ds;
        public frmShowData()
        {
            InitializeComponent();
        }

        private void frmShowData_Load(object sender, EventArgs e)
        {
            LoadDataBindingSource();
        }

        public void LoadDataBindingSource()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter("SELECT s.*, c.cityName FROM clients s\r\nINNER JOIN cities c ON s.cityId=c.id ", con))
                {
                    ds = new DataSet();
                    sda.Fill(ds, "clients");
                    sda.SelectCommand.CommandText = "SELECT * FROM products";
                    sda.Fill(ds, "products");

                    ds.Tables["clients"].Columns.Add(new DataColumn("image", typeof(byte[])));
                    for (int i = 0; i < ds.Tables["clients"].Rows.Count; i++)
                    {
                        ds.Tables["clients"].Rows[i]["image"] = File.ReadAllBytes($@"..\..\Images\{ds.Tables["clients"].Rows[i]["picture"]}");
                    }
                    DataRelation rel = new DataRelation("FK_S_S", ds.Tables["clients"].Columns["clientId"], ds.Tables["products"].Columns["clientId"]);
                    ds.Relations.Add(rel);
                    bsS.DataSource = ds;
                    bsS.DataMember = "clients";

                    bsC.DataSource = bsS;
                    bsC.DataMember = "FK_S_S";
                    dataGridView1.DataSource = bsC;
                    AddDataBindings();
                }
            }
        }

        private void AddDataBindings()
        {
            lblClientId.DataBindings.Clear();
            lblClientId.DataBindings.Add("Text", bsS, "clientId");

            lblCliectName.DataBindings.Clear();
            lblCliectName.DataBindings.Add("Text", bsS, "clientName");

            lblDateOfBirth.DataBindings.Clear();
            lblDateOfBirth.DataBindings.Add("Text", bsS, "dateOfBirth");

            Binding bm = new Binding("Text", bsS, "dateOfBirth", true);
            bm.Format += Bm_Format;
            lblDateOfBirth.DataBindings.Clear();
            lblDateOfBirth.DataBindings.Add(bm);

            pictureBox1.DataBindings.Clear();
            pictureBox1.DataBindings.Add(new Binding("Image", bsS, "image", true));

            chkInDhaka.DataBindings.Clear();
            chkInDhaka.DataBindings.Add("Checked", bsS, "insideDhaka", true);

            lblCity.DataBindings.Clear();
            lblCity.DataBindings.Add("Text", bsS, "cityName");
        }

        private void Bm_Format(object sender, ConvertEventArgs e)
        {
            DateTime d = (DateTime)e.Value;
            e.Value = d.ToString("dd-MM-yyyy");
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            bsS.MoveFirst();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (bsS.Position > 0)
            {
                bsS.MovePrevious();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (bsS.Position < bsS.Count - 1)
            {
                bsS.MoveNext();
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            bsS.MoveLast();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            int v = int.Parse((bsS.Current as DataRowView).Row[0].ToString());
            new frmEdit { TheForm = this, IdToEdit = v }.ShowDialog();
        }
    }
}
