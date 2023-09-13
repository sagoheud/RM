using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RM.Model
{
    public partial class frmProductAdd : SampleAdd
    {
        public frmProductAdd()
        {
            InitializeComponent();
        }

        public int id = 0;
        public int cID = 0;

        private void frmProductAdd_Load(object sender, EventArgs e)
        {
            // 체크박스 칸이 차면
            string qry = "select catID 'id', catName 'name' from category";

            MainClass.CBFill(qry, cbCategory);

            if (cID >0) // 업데이트되면
            { 
                cbCategory.SelectedValue = cID;
            }

            if (id > 0)
            {
                ForUpdateLoadData();
            }
        }

        string filePath;
        Byte[] imgaeByteArray;

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Images(.jpg, .png)| * .png; * .jpg;";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filePath = ofd.FileName;
                txtImage.Image = new Bitmap(filePath);
            }
        }

        public override void btnSave_Click(object sender, EventArgs e)
        {
            string qry = "";

            if (id == 0) // 입력
            {
                qry = "Insert into product Values(@Name, @Price, @category, @Image)";
            }
            else // 업데이트
            {
                qry = "update product set pName = @Name, pPrice = @Price, categoryID = @category, pImage = @Image where pID = @id";
            }

            // 이미지
            Image temp = new Bitmap(txtImage.Image);
            MemoryStream ms = new MemoryStream();
            temp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            imgaeByteArray = ms.ToArray();

            Hashtable ht = new Hashtable();
            ht.Add("@id", id);
            ht.Add("@Name", txtName.Text);
            ht.Add("@Price", txtPrice.Text);
            ht.Add("@category", Convert.ToInt32(cbCategory.SelectedValue));
            ht.Add("@Image", imgaeByteArray);

            if (MainClass.SQl(qry, ht) > 0)
            {
                guna2MessageDialog1.Show("저장 완료하였습니다.");
                id = 0;
                cID = 0;
                txtName.Text = "";
                txtPrice.Text = "";
                cbCategory.SelectedIndex = 0;
                cbCategory.SelectedIndex = -1;
                txtImage.Image = RM.Properties.Resources.productPic;
                txtName.Focus();
            }
            this.Close();
        }

        private void ForUpdateLoadData()
        {
            string qry = @"select * from product where pID =" + id + "";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            { 
                txtName.Text = dt.Rows[0]["pName"].ToString();
                txtPrice.Text = dt.Rows[0]["pPrice"].ToString();

                Byte[] imageArray = (byte[])(dt.Rows[0]["pImage"]);
                byte[] imageByteArray = imageArray;
                txtImage.Image = Image.FromStream(new MemoryStream(imageArray));
            }
        }
    }
}
