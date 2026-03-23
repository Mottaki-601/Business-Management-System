using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1291387
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.IsMdiContainer = true;
        }

        private void masterDetailsToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            frmMasterDetails frmMasterDetails = new frmMasterDetails();
            frmMasterDetails.Show();
            frmMasterDetails.MdiParent = this;
        }

        private void exposeDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShowData frmShowData = new frmShowData();
            frmShowData.Show();
            frmShowData.MdiParent = this;
        }

        private void employeeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEmployee_SP es = new frmEmployee_SP();
            es.Show();
            es.MdiParent = this;
        }

        private void clientInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmClientReport sr = new frmClientReport();
            sr.Show();
            sr.MdiParent = this;
        }

        private void employeeInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEmployeeReport er = new frmEmployeeReport();
            er.Show();
            er.MdiParent = this;
        }

        private void cityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCityEntry frmCityEntry = new frmCityEntry();
            frmCityEntry.Show();
            frmCityEntry.MdiParent = this;
        }

        private void clientBasicInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmClientBasicInformationReport sr = new frmClientBasicInformationReport();
            sr.Show();
            sr.MdiParent = this;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
