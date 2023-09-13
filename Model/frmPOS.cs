using Guna.UI2.WinForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace RM.Model
{
    public partial class frmPOS : Form
    {
        public frmPOS()
        {
            InitializeComponent();
        }


        public int MainID = 0;
        public string OrderType = null;
        public int driverID = 0;
        public string customerName = "";
        public string customerPhone = "";

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmPOS_Load(object sender, EventArgs e)
        {
            guna2DataGridView1.BorderStyle = BorderStyle.FixedSingle;
            AddCategory();

            ProductPanel.Controls.Clear();
            LoadProducts();
        }

        private void AddCategory()
        {
            string qry = "select * from category";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            CategoryPanel.Controls.Clear();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Guna.UI2.WinForms.Guna2Button b = new Guna.UI2.WinForms.Guna2Button();
                    b.FillColor = Color.FromArgb(55, 50, 120);
                    b.Size = new Size(134, 45);
                    b.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
                    b.Text = row["catName"].ToString();

                    // 클릭이벤트
                    b.Click += new EventHandler(b_click);
                    CategoryPanel.Controls.Add(b);
                }
;            }
        }

        private void b_click(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button b = (Guna.UI2.WinForms.Guna2Button)sender;
            if (b.Text == "All")
            {
                txtSearch.Text = "1";
                txtSearch.Text = "";
                return;
            }

            foreach (var item in ProductPanel.Controls)
            {
                var pro = (ucProduct)item;
                pro.Visible = pro.pcategory.ToLower().Contains(b.Text.Trim().ToLower());
            }
        }

        private void AddItems(string id, string proID, string name, string cat, string price, Image pimage)
        {
            var w = new ucProduct()
            {
                pName = name,
                pPrice = price,
                pcategory = cat,
                pImage = pimage,
                id = Convert.ToInt32(proID)
            };

            ProductPanel.Controls.Add(w);

            w.onSelect += (ss, ee) =>
            {
                var wdg = (ucProduct)ss;

                foreach (DataGridViewRow item in guna2DataGridView1.Rows)
                {
                    // 이 줄은 이미 1개 이상 있는 상품이나 업데이트를 체크할 시
                    if (Convert.ToInt32(item.Cells["dgvproID"].Value) == wdg.id)
                    {
                        item.Cells["dgvQty"].Value = int.Parse(item.Cells["dgvQty"].Value.ToString()) + 1;
                        item.Cells["dgvAmount"].Value = int.Parse(item.Cells["dgvQty"].Value.ToString()) *
                                                        double.Parse(item.Cells["dgvPrice"].Value.ToString());
                        GetTotal();
                        return;
                    }
                }

                // 포스에서 클릭시 새 상품이 추가
                guna2DataGridView1.Rows.Add(new object[] { 0, 0, wdg.id, wdg.pName, 1, wdg.pPrice, wdg.pPrice });
                GetTotal();
            };
        }

        // 데이터베이스에서 상품 가져옴
        private void LoadProducts()
        {
            string qry = "Select * from product inner join category on catID = categoryID";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow item in dt.Rows)
            {
                Byte[] imagearray = (byte[])item["pImage"];
                byte[] immagebytearray = imagearray;

                AddItems("0", item["pID"].ToString(), item["pName"].ToString(), item["catName"].ToString(), 
                    item["pPrice"].ToString(), Image.FromStream(new MemoryStream(imagearray)));
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            foreach (var item in CategoryPanel.Controls)
            {
                if (item is Guna.UI2.WinForms.Guna2Button)
                {
                    Guna.UI2.WinForms.Guna2Button b = (Guna.UI2.WinForms.Guna2Button)item;
                    b.Checked = false;
                }
            }
                

            foreach (var item in ProductPanel.Controls)
            {
                var pro = (ucProduct)item;
                pro.Visible = pro.pName.ToLower().Contains(txtSearch.Text.Trim().ToLower());
            }
        }

        private void guna2DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // 불연속일때

            int count = 0;
            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                count++;
                row.Cells[0].Value = count;
            }
        }

        private void GetTotal()
        { 
            double total = 0;
            lblTotal.Text = "";
            foreach (DataGridViewRow item in guna2DataGridView1.Rows)
            {
                total += double.Parse(item.Cells["dgvAmount"].Value.ToString());
            }
            lblTotal.Text = total.ToString("N2");
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            MainID = 0;
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblDriverName.Text = "";
            OrderType = null;
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            lblDriverName.Visible = false;
            btnTake.Checked = false;
            btnDelivery.Checked = false;
            btnDin.Checked = false;
            guna2DataGridView1.Rows.Clear();
            lblTotal.Text = "0.00";
        }

        private void btnDelivery_Click(object sender, EventArgs e)
        {
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            OrderType = "Delivery";

            frmAddCustomer frm = new frmAddCustomer();
            frm.MainID = MainID;
            frm.orderType = OrderType;
            MainClass.BlurBackground(frm);

            if (frm.txtName.Text != "" & frm.txtPhone.Text != "" & frm.cbDriver.Text != "") // 포장에서 고객 정보가 있을때
            {
                driverID = frm.driverID;
                lblDriverName.Text = "고객: " + frm.txtName.Text + " 전화번호: " + frm.txtPhone.Text + " 배달원: " + frm.cbDriver.Text;
                lblDriverName.Visible = true;
                customerName = frm.txtName.Text;
                customerPhone = frm.txtPhone.Text;
            }
            else 
            {
                guna2MessageDialog1.Show("정확히 입력해주세요.");
                OrderType = null;
                btnDelivery.Checked = false;
                lblDriverName.Text = "";
                return;
            }
        }

        private void btnTake_Click(object sender, EventArgs e)
        {
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            OrderType = "Take Away";

            frmAddCustomer frm = new frmAddCustomer();
            frm.MainID = MainID;
            frm.orderType = OrderType;
            MainClass.BlurBackground(frm);

            if (frm.txtName.Text != "" & frm.txtPhone.Text != "") // 포장에서 고객 정보가 있을때
            { 
                driverID = frm.driverID;
                lblDriverName.Text = "고객: " + frm.txtName.Text + " 전화번호: " + frm.txtPhone.Text;
                lblDriverName.Visible = true;
                customerName = frm.txtName.Text;
                customerPhone = frm.txtPhone.Text;
            }
            else
            {
                guna2MessageDialog1.Show("정확히 입력해주세요.");
                OrderType = null;
                btnTake.Checked = false;
                lblDriverName.Text = "";
                return;
            }
        }

        private void btnDin_Click(object sender, EventArgs e)
        {
            OrderType = "Din In";
            lblDriverName.Text = "";
            lblDriverName.Visible = false;

            // 테이블 선택과 웨이터 선택 폼 생성 필요
            frmTableSelect ts = new frmTableSelect();
            MainClass.BlurBackground(ts);
            if (ts.TableName != "")
            {
                lblTable.Text = ts.TableName;
                lblTable.Visible = true;
            }
            else
            {
                lblTable.Text = "";
                lblTable.Visible = false;
            }

            frmWaiterSelect ws = new frmWaiterSelect();
            MainClass.BlurBackground(ws);
            if (ws.WaiterName != "")
            {
                lblWaiter.Text = ws.WaiterName;
                lblWaiter.Visible = true;
            }
            else
            {
                lblWaiter.Text = "";
                lblWaiter.Visible = false;
            }
        }

        private void btnKot_Click(object sender, EventArgs e)
        {
            // 데이터베이스에 저장
            // 테이블 생성
            // 추가적인 정보 저장옹 태이블 필드 필요
            string qry1 = ""; // 메인 db
            string qry2 = ""; // 상세 db

            int detailID = 0;

            if (OrderType == null)
            {
                guna2MessageDialog1.Show("주문 방식을 선택해주세요.");
                return;
            }
            if (lblTotal.Text == "0.00")
            {
                guna2MessageDialog1.Show("메뉴를 선택해주세요.");
                return;
            }

            if (MainID == 0) // 입력
            {
                qry1 = @"Insert into tblMain values(@aDate, @aTime, @TableName, @WaiterName,
                        @status, @orderType, @total, @received, @change, @driverID, @cusName, @cusPhone);
                        Select SCOPE_IDENTITY()";
                // 이 줄은 id값을 받아 얻어온다
            }
            else // 업데이트
            {
                qry1 = @"Update tblMain Set status = @status, total = @total,
                        received = @received, change = @change where MainID = @ID";
            }

            SqlCommand cmd1 = new SqlCommand(qry1, MainClass.con);
            cmd1.Parameters.AddWithValue("@ID", MainID);
            cmd1.Parameters.AddWithValue("@aDate",Convert.ToDateTime(DateTime.Now.Date));
            cmd1.Parameters.AddWithValue("@aTime", DateTime.Now.ToShortTimeString());
            cmd1.Parameters.AddWithValue("@TableName",lblTable.Text);
            cmd1.Parameters.AddWithValue("@WaiterName",lblWaiter.Text);
            cmd1.Parameters.AddWithValue("@status","Pending");
            cmd1.Parameters.AddWithValue("@orderType", OrderType);
            cmd1.Parameters.AddWithValue("@total",Convert.ToDouble(lblTotal.Text)); // 주문 가격에 대한 데이터만 저장하므로 결제가 완료되면 업데이트
            cmd1.Parameters.AddWithValue("@received", Convert.ToDouble(0));
            cmd1.Parameters.AddWithValue("@change", Convert.ToDouble(0));
            cmd1.Parameters.AddWithValue("@driverID", driverID);
            cmd1.Parameters.AddWithValue("@cusName", customerName);
            cmd1.Parameters.AddWithValue("@cusPhone", customerPhone);

            if (MainClass.con.State == ConnectionState.Closed) { MainClass.con.Open(); }
            if (MainID == 0) { MainID = Convert.ToInt32(cmd1.ExecuteScalar()); } else { cmd1.ExecuteNonQuery(); }
            if (MainClass.con.State == ConnectionState.Open) { MainClass.con.Close(); }

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                detailID = Convert.ToInt32(row.Cells["dgvid"].Value);

                if (detailID == 0) // 입력
                {
                    qry2 = @"Insert into tblDetails values(@MainID, @proID, @qty, @price, @amount)";
                }
                else // 업데이트
                {
                    qry2 = @"Update tblDetails Set proID = @proID, qty = @qty, price = @price, amount = @amount
                            where DetailID = @ID";
                }

                SqlCommand cmd2 = new SqlCommand(qry2, MainClass.con);
                cmd2.Parameters.AddWithValue("@ID", detailID);
                cmd2.Parameters.AddWithValue("@MainID", MainID);
                cmd2.Parameters.AddWithValue("@proID",Convert.ToInt32(row.Cells["dgvproID"].Value));
                cmd2.Parameters.AddWithValue("@qty", Convert.ToInt32(row.Cells["dgvQty"].Value));
                cmd2.Parameters.AddWithValue("@price", Convert.ToDouble(row.Cells["dgvPrice"].Value));
                cmd2.Parameters.AddWithValue("@amount", Convert.ToDouble(row.Cells["dgvAmount"].Value));

                if (MainClass.con.State == ConnectionState.Closed) { MainClass.con.Open(); }
                cmd2.ExecuteNonQuery();
                if (MainClass.con.State == ConnectionState.Open) { MainClass.con.Close(); }
            }

            guna2MessageDialog1.Show("저장 완료하였습니다.");
            MainID = 0;
            guna2DataGridView1.Rows.Clear();
            OrderType = null;
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblDriverName.Text = "";
            lblDriverName.Visible = false;
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            btnTake.Checked = false;
            btnDelivery.Checked = false;
            btnDin.Checked = false;
            lblTotal.Text = "0.00";
        }

        public int id = 0;
        private void btnBill_Click(object sender, EventArgs e)
        {
            frmBillList frm = new frmBillList();
            MainClass.BlurBackground(frm);

            if (frm.MainID > 0)
            {
                id = frm.MainID;
                MainID = frm.MainID;
                LoadEntries();
            }
        }

        private void LoadEntries()
        {
            string qry = @"Select * from tblMain m
                                inner join tblDetails d on m.MainID = d.MainID
                                inner join product p on p.pID = d.proID 
                                where m.MainID = " + id + "";

            SqlCommand cmd2 = new SqlCommand(qry, MainClass.con);
            DataTable dt2 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            da2.Fill(dt2);

            if (dt2.Rows[0]["orderType"].ToString() == "Delivery")
            {
                btnDelivery.Checked = true;
                lblWaiter.Visible = false;
                lblTable.Visible = false;
            }
            else if (dt2.Rows[0]["orderType"].ToString() == "Take Away")
            {
                btnTake.Checked = true;
                lblWaiter.Visible = false;
                lblTable.Visible = false;
            }
            else
            {
                btnDin.Checked = true;
                lblWaiter.Visible = true;
                lblTable.Visible = true;
            }

                guna2DataGridView1.Rows.Clear();

            foreach (DataRow item in dt2.Rows)
            {
                lblTable.Text = item["TableName"].ToString();
                lblWaiter.Text = item["WaiterName"].ToString();

                string DetailID = item["DetailID"].ToString();
                string proID = item["proID"].ToString();
                string pName = item["pName"].ToString();
                string qty = item["qty"].ToString();
                string price = item["price"].ToString();
                string amount = item["amount"].ToString();

                object[] obj = {0, DetailID, proID, pName, qty, price, amount};
                guna2DataGridView1.Rows.Add(obj);
            }
            GetTotal();
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            frmCheckout frm = new frmCheckout();
            frm.MainID = id;
            frm.amt = Convert.ToDouble(lblTotal.Text);
            MainClass.BlurBackground(frm);

            MainID = 0;
            guna2DataGridView1.Rows.Clear();
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblDriverName.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            lblDriverName.Visible = false;
            lblTotal.Text = "0.00";
        }

        private void btnHold_Click(object sender, EventArgs e)
        {
            // kot와 비슷하다
            string qry1 = ""; // 메인 db
            string qry2 = ""; // 상세 db

            int detailID = 0;

            if (OrderType == null)
            {
                guna2MessageDialog1.Show("주문 방식을 선택해주세요.");
                return;
            }
            if (lblTotal.Text == "0.00")
            {
                guna2MessageDialog1.Show("메뉴를 선택해주세요.");
                return;
            }

            if (MainID == 0) // 입력
            {
                qry1 = @"Insert into tblMain values(@aDate, @aTime, @TableName, @WaiterName,
                        @status, @orderType, @total, @received, @change, @driverID, @cusName, @cusPhone);
                        Select SCOPE_IDENTITY()";
                // 이 줄은 id값을 받아 얻어온다
            }
            else // 업데이트
            {
                qry1 = @"Update tblMain Set status = @status, total = @total,
                        received = @received, change = @change where MainID = @ID";
            }

            SqlCommand cmd1 = new SqlCommand(qry1, MainClass.con);
            cmd1.Parameters.AddWithValue("@ID", MainID);
            cmd1.Parameters.AddWithValue("@aDate", Convert.ToDateTime(DateTime.Now.Date));
            cmd1.Parameters.AddWithValue("@aTime", DateTime.Now.ToShortTimeString());
            cmd1.Parameters.AddWithValue("@TableName", lblTable.Text);
            cmd1.Parameters.AddWithValue("@WaiterName", lblWaiter.Text);
            cmd1.Parameters.AddWithValue("@status", "Hold");
            cmd1.Parameters.AddWithValue("@orderType", OrderType);
            cmd1.Parameters.AddWithValue("@total", Convert.ToDouble(lblTotal.Text)); // 주문 가격에 대한 데이터만 저장하므로 결제가 완료되면 업데이트
            cmd1.Parameters.AddWithValue("@received", Convert.ToDouble(0));
            cmd1.Parameters.AddWithValue("@change", Convert.ToDouble(0));
            cmd1.Parameters.AddWithValue("@driverID", driverID);
            cmd1.Parameters.AddWithValue("@cusName", customerName);
            cmd1.Parameters.AddWithValue("@cusPhone", customerPhone);

            if (MainClass.con.State == ConnectionState.Closed) { MainClass.con.Open(); }
            if (MainID == 0) { MainID = Convert.ToInt32(cmd1.ExecuteScalar()); } else { cmd1.ExecuteNonQuery(); }
            if (MainClass.con.State == ConnectionState.Open) { MainClass.con.Close(); }

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                detailID = Convert.ToInt32(row.Cells["dgvid"].Value);

                if (detailID == 0) // 입력
                {
                    qry2 = @"Insert into tblDetails values(@MainID, @proID, @qty, @price, @amount)";
                }
                else // 업데이트
                {
                    qry2 = @"Update tblDetails Set proID = @proID, qty = @qty, price = @price, amount = @amount
                            where DetailID = @ID";
                }

                SqlCommand cmd2 = new SqlCommand(qry2, MainClass.con);
                cmd2.Parameters.AddWithValue("@ID", detailID);
                cmd2.Parameters.AddWithValue("@MainID", MainID);
                cmd2.Parameters.AddWithValue("@proID", Convert.ToInt32(row.Cells["dgvproID"].Value));
                cmd2.Parameters.AddWithValue("@qty", Convert.ToInt32(row.Cells["dgvQty"].Value));
                cmd2.Parameters.AddWithValue("@price", Convert.ToDouble(row.Cells["dgvPrice"].Value));
                cmd2.Parameters.AddWithValue("@amount", Convert.ToDouble(row.Cells["dgvAmount"].Value));

                if (MainClass.con.State == ConnectionState.Closed) { MainClass.con.Open(); }
                cmd2.ExecuteNonQuery();
                if (MainClass.con.State == ConnectionState.Open) { MainClass.con.Close(); }
            }

            guna2MessageDialog1.Show("저장 완료하였습니다.");
            MainID = 0;
            guna2DataGridView1.Rows.Clear();
            OrderType = null;
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblDriverName.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            lblDriverName.Visible = false;
            btnTake.Checked = false;
            btnDelivery.Checked = false;
            btnDin.Checked = false;
            lblTotal.Text = "0.00";
        }
    }
}
