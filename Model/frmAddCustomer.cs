using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RM.Model
{
    public partial class frmAddCustomer : Form
    {
        public frmAddCustomer()
        {
            InitializeComponent();
        }

        public string orderType = null;
        public int driverID = 0;
        public string customerName = "";
        
        public int MainID = 0;

        private void frmAddCustomer_Load(object sender, EventArgs e)
        {
            if (orderType == "Take Away") 
            {
                lblDriver.Visible = false;
                cbDriver.Visible = false;
            }

            string qry = "Select stfID 'id', stfName 'name' from staff where stfRole = '배달원'";
            MainClass.CBFill(qry, cbDriver);

            if (MainID > 0)
            {
                cbDriver.SelectedValue = driverID;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbDriver_SelectedIndexChanged(object sender, EventArgs e)
        {
            driverID = Convert.ToInt32(cbDriver.SelectedValue);
        }
    }
}
