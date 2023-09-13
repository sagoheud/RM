﻿using System;
using System.Collections;
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
    public partial class frmStaffAdd : SampleAdd
    {
        public frmStaffAdd()
        {
            InitializeComponent();
        }

        public int id = 0;

        private void frmStaffAdd_Load(object sender, EventArgs e)
        {

        }

        public override void btnSave_Click(object sender, EventArgs e)
        {
            string qry = "";

            if (id == 0) // 입력
            {
                qry = "Insert into staff Values(@Name, @Phone, @Role)";
            }
            else // 업데이트
            {
                qry = "update staff set stfName = @Name, stfPhone = @Phone, stfRole = @Role where stfID = @id";
            }

            Hashtable ht = new Hashtable();
            ht.Add("@id", id);
            ht.Add("@Name", txtName.Text);
            ht.Add("@Phone", txtPhone.Text);
            ht.Add("@Role", cbRole.Text);

            if (MainClass.SQl(qry, ht) > 0)
            {
                guna2MessageDialog1.Show("저장 완료하였습니다.");
                id = 0;
                txtName.Text = "";
                txtPhone.Text = "";
                cbRole.SelectedIndex = -1;
                txtName.Focus();
            }
            this.Close();
        }
    }
}
