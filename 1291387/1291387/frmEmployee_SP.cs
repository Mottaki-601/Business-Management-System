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
    public partial class frmEmployee_SP : Form
    {
        string conStr = "Data Source=.;Initial Catalog=projectdb;Integrated Security=true;MultipleActiveResultSets=true; TrustServerCertificate=True;";

        SqlConnection sqlCon;
        SqlCommand sqlCmd;
        string employeeId = "";
        
        public frmEmployee_SP()
        {
            InitializeComponent();
            sqlCon = new SqlConnection(conStr);
            sqlCon.Open();
        }

        private void frmEmployee_SP_Load(object sender, EventArgs e)
        {
            LoadCityCombo();
            LoadDepartmentCombo();
            dataGridView1.DataSource = FetchEmpDetails();
        }

        private void LoadDepartmentCombo()
        {
            SqlDataAdapter sda = new SqlDataAdapter("SELECT id,deptName FROM departments", sqlCon);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmbDept.DataSource = dt;
            cmbDept.ValueMember = "id";
            cmbDept.DisplayMember = "deptName";
        }

        private void LoadCityCombo()
        {
            SqlDataAdapter sda = new SqlDataAdapter("SELECT id,cityName FROM cities", sqlCon);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmbCity.DataSource = dt;
            cmbCity.ValueMember = "id";
            cmbCity.DisplayMember = "cityName";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmpName.Text))
            {
                MessageBox.Show("Enter employee name... 😒 ");
                txtEmpName.Select();
            }
            else if (cmbCity.SelectedIndex <= -1)
            {
                MessageBox.Show("Select city!!!");
                cmbCity.Select();
            }
            else if (cmbDept.SelectedIndex <= -1)
            {
                MessageBox.Show("Select department!!!");
                cmbDept.Select();
            }
            else if (cmbGender.SelectedIndex <= -1)
            {
                MessageBox.Show("Select Gender!!!");
                cmbGender.Select();
            }
            else
            {
                try
                {
                    if (sqlCon.State == ConnectionState.Closed)
                    {
                        sqlCon.Open();
                    }
                    DataTable dt = new DataTable();
                    sqlCmd = new SqlCommand("spEmployee", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@actionType", "SaveData");
                    sqlCmd.Parameters.AddWithValue("@employeeId", employeeId);
                    sqlCmd.Parameters.AddWithValue("@name", txtEmpName.Text);
                    sqlCmd.Parameters.AddWithValue("@cityId", cmbCity.SelectedValue);
                    sqlCmd.Parameters.AddWithValue("@departmentId", cmbDept.SelectedValue);
                    sqlCmd.Parameters.AddWithValue("@gender", cmbGender.Text);
                    int numRes = sqlCmd.ExecuteNonQuery();
                    if (numRes > 0)
                    {
                        MessageBox.Show("Data saved successfully!!!", "Success");
                        dataGridView1.DataSource = FetchEmpDetails();
                        AllClear();
                    }
                    else
                    {
                        MessageBox.Show("Please try again later!!!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error : " + ex.Message);
                }
            }
        }

        private void AllClear()
        {
            txtEmpName.Clear();
            cmbCity.Text = "";
            cmbDept.Text = "";
            cmbGender.SelectedIndex = -1;
            employeeId = "";
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = FetchEmpDetails();
        }

        private object FetchEmpDetails()
        {
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
            }
            DataTable dt = new DataTable();
            sqlCmd = new SqlCommand("spEmployee", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@actionType", "FetchData");
            SqlDataAdapter sda = new SqlDataAdapter(sqlCmd);
            sda.Fill(dt);
            return dt;
        }
        private DataTable FetchEmpRecord(string employeeId)
        {
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
            }
            DataTable dt = new DataTable();
            sqlCmd = new SqlCommand("spEmployee", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@actionType", "FetchRecord");
            sqlCmd.Parameters.AddWithValue("@employeeId", employeeId);
            SqlDataAdapter sda = new SqlDataAdapter(sqlCmd);
            sda.Fill(dt);
            return dt;

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnSave.Text = "Update";
                employeeId = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                DataTable dt = FetchEmpRecord(employeeId);
                if (dt.Rows.Count > 0)
                {
                    employeeId = dt.Rows[0][0].ToString();
                    txtEmpName.Text = dt.Rows[0][1].ToString();
                    cmbCity.Text = dt.Rows[0][5].ToString();
                    cmbDept.Text = dt.Rows[0][6].ToString();
                    cmbGender.Text = dt.Rows[0][2].ToString();
                }
                else
                {
                    AllClear();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(employeeId))
            {
                try
                {
                    if (sqlCon.State == ConnectionState.Closed)
                    {
                        sqlCon.Open();
                    }
                    DataTable dt = new DataTable();
                    sqlCmd = new SqlCommand("spEmployee", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@actionType", "DeleteData");
                    sqlCmd.Parameters.AddWithValue("@employeeId", employeeId);
                    int numRes = sqlCmd.ExecuteNonQuery();
                    if (numRes > 0)
                    {
                        MessageBox.Show("Data delete successfully!!!", "Success");
                        dataGridView1.DataSource = FetchEmpDetails();
                        AllClear();
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            AllClear();
        }
    }
}
