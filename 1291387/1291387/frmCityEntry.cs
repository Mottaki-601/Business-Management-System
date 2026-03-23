using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1291387
{
    public partial class frmCityEntry : Form
    {
        public frmCityEntry()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=.;Database=projectdb;Integrated Security=true;TrustServerCertificate=true");
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "INSERt INTO cities VALUES('" + txtCityName.Text + "')";
            con.Open();
            cmd.ExecuteNonQuery();
            MessageBox.Show("Data inserted successfully!!");
            con.Close();
        }

        private void frmCityEntry_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'projectdbDataSet.cities' table. You can move, or remove it, as needed.
            this.citiesTableAdapter.Fill(this.projectdbDataSet.cities);

        }
    }
}
