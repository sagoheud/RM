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

namespace RM.Model
{
    public partial class frmWaiterSelect : Form
    {
        public frmWaiterSelect()
        {
            InitializeComponent();
        }

        public string WaiterName;

        private void frmWaiterSelect_Load(object sender, EventArgs e)
        {
            string qry = "Select * from staff where stfRole Like '접객원'";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                Guna.UI2.WinForms.Guna2Button b = new Guna.UI2.WinForms.Guna2Button();
                b.Text = row["stfName"].ToString();
                b.Width = 150;
                b.Height = 50;
                b.FillColor = Color.FromArgb(128, 255, 128);
                b.HoverState.FillColor = Color.FromArgb(55, 50, 120);

                // 클릭이벤트
                b.Click += new EventHandler(b_click);
                flowLayoutPanel1.Controls.Add(b);
            }
        }

        private void b_click(object sender, EventArgs e)
        {
            WaiterName = (sender as Guna.UI2.WinForms.Guna2Button).Text.ToString();
            this.Close();
        }
    }
}
