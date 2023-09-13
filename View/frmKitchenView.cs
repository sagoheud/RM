using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RM.View
{
    public partial class frmKitchenView : Form
    {
        public frmKitchenView()
        {
            InitializeComponent();
        }

        private void frmKitchenView_Load(object sender, EventArgs e)
        {
            GetOrders();
        }

        private void GetOrders()
        {
            flowLayoutPanel1.Controls.Clear();
            string qry1 = @"Select * from tblMain where status = 'Pending' ";
            SqlCommand cmd1 = new SqlCommand(qry1, MainClass.con);
            DataTable dt1 = new DataTable();
            SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
            da1.Fill(dt1);

            FlowLayoutPanel p1;

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                p1 = new FlowLayoutPanel();
                p1.AutoSize = true;
                p1.Width = 230;
                p1.Height = 350;
                p1.FlowDirection = FlowDirection.TopDown;
                p1.BorderStyle = BorderStyle.FixedSingle;
                p1.Margin = new Padding(10);

                FlowLayoutPanel p2 = new FlowLayoutPanel();
                p2 = new FlowLayoutPanel();
                p2.BackColor = Color.FromArgb(55, 50, 120);
                p2.AutoSize = true;
                p2.Width = 230;
                p2.Height = 125;
                p2.FlowDirection = FlowDirection.TopDown;               
                p2.Margin = new Padding(0);

                Label lb1 = new Label();
                lb1.ForeColor = Color.White;
                lb1.Margin = new Padding(10,10,3,0);
                lb1.AutoSize = true;

                Label lb2 = new Label();
                lb2.ForeColor = Color.White;
                lb2.Margin = new Padding(10, 5, 3, 0);
                lb2.AutoSize = true;

                Label lb3 = new Label();
                lb3.ForeColor = Color.White;
                lb3.Margin = new Padding(10, 5, 3, 0);
                lb3.AutoSize = true;

                Label lb4 = new Label();
                lb4.ForeColor = Color.White;
                lb4.Margin = new Padding(10, 5, 3, 0);
                lb4.AutoSize = true;

                Label lb5 = new Label();
                lb5.ForeColor = Color.White;
                lb5.Margin = new Padding(10, 5, 3, 10);
                lb5.AutoSize = true;

                lb1.Text = "테이블 : " + dt1.Rows[i]["TableName"].ToString();
                lb2.Text = "담당직원 : " + dt1.Rows[i]["WaiterName"].ToString();
                lb3.Text = "주문날짜 : " + DateTime.Now.Date.ToString("yyyy-MM-dd");
                lb4.Text = "주문시간 : " + dt1.Rows[i]["aTime"].ToString();
                lb5.Text = "주문방법 : " + dt1.Rows[i]["orderType"].ToString();

                p2.Controls.Add(lb1);
                p2.Controls.Add(lb2);
                p2.Controls.Add(lb3);
                p2.Controls.Add(lb4);
                p2.Controls.Add(lb5);

                p1.Controls.Add(p2);

                // 이제 상품을 추가
                int mid = 0;
                mid = Convert.ToInt32(dt1.Rows[i]["MainID"].ToString());

                string qry2 = @"Select * from tblMain m
                                inner join tblDetails d on m.MainID = d.MainID
                                inner join product p on p.pID = d.proID 
                                where m.MainID = "+mid+"";

                SqlCommand cmd2 = new SqlCommand(qry2, MainClass.con);
                DataTable dt2 = new DataTable();
                SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                da2.Fill(dt2);

                for (int j = 0; j < dt2.Rows.Count; j++)
                {
                    Label lb6 = new Label();
                    lb6.ForeColor = Color.White;
                    lb6.Margin = new Padding(10, 5, 3, 5);
                    lb6.AutoSize = true;

                    int no = j + 1;
                    lb6.Text = "no" + no + " " + dt2.Rows[j]["pName"].ToString() + " " + dt2.Rows[j]["qty"].ToString();

                    p1.Controls.Add(lb6);
                }

                // 주문 사항 변경 버튼 추가

                Guna.UI2.WinForms.Guna2Button b = new Guna.UI2.WinForms.Guna2Button();
                b.AutoRoundedCorners = true;
                b.Size = new Size(100,35);
                b.FillColor = Color.FromArgb(128, 255, 128);
                b.Margin = new Padding(20,5,3,10);
                b.Text = "완료";
                b.Tag = dt1.Rows[i]["MainID"].ToString(); // id 저장

                b.Click += new EventHandler(b_click);
                p1.Controls.Add(b);

                flowLayoutPanel1.Controls.Add(p1);
            }


        }

        private void b_click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32((sender as Guna.UI2.WinForms.Guna2Button).Tag.ToString());

            guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question;
            guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;
            if (guna2MessageDialog1.Show("정말 삭제하시겠습니까?") == DialogResult.Yes)
            {
                string qry1 = @"Update tblMain set status = 'Complete' where MainID = @ID";
                Hashtable ht = new Hashtable();
                ht.Add("@ID", id);

                if ( MainClass.SQl(qry1,ht)>0)
                {
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                    guna2MessageDialog1.Show("저장 완료하였습니다.");
                }
                GetOrders();              
            }
        }
    }
}
