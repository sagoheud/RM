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

namespace RM
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            // 데이터베이스에서 사용자 정보 테이블을 작성해야함
            if (MainClass.IsValidUser(txtUsername.Text, txtPassword.Text) == false)
            {
                guna2MessageDialog1.Show("유저명 또는 비밀번호가 일치하지 않습니다");
                return;
            }
            else { 
                this.Hide();
                frmMain frm = new frmMain();
                frm.Show();
            }

            //우선 유저 입력
        }      
    }
}
