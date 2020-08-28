﻿using AnyStore.BLL;
using AnyStore.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnyStore.UI
{
    public partial class frmUsers : Form
    {
        public frmUsers()
        {
            InitializeComponent();
        }
        userBLL u = new userBLL();
        userDAL dal = new userDAL();

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
           
            u.first_name = txtFirstName.Text;
            u.last_name = txtLastName.Text;
            u.email = txtEmail.Text;
            u.username = txtUsername.Text;
            u.password = txtPassword.Text;
            u.contact = txtContact.Text;
            u.address = txtAddress.Text;
            u.gender = cmbGender.Text;
            u.user_type = cmbUserType.Text;
            u.added_date = DateTime.Now;

            string loggedUser = frmLogin.loggedIn;
            userBLL usr = dal.GetIdFromUsername(loggedUser);
            u.added_by = usr.id;

            bool success = dal.Insert(u);
            DataTable dt = dal.select();
            dgvUsers.DataSource = dt;
            if (success == true)
            {
                clear();
                MessageBox.Show("New User Created Successfully");
            }
            else
            {
                MessageBox.Show("Failed To Create User");
            }
           // DataTable dt = dal.select();
            //dgvUsers.DataSource = dt;

        }
        private void clear()
        {
            txtUserID.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmail.Text = "";
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtContact.Text = "";
            txtAddress.Text = "";
            cmbGender.Text = "";
            cmbUserType.Text = "";

        }

        private void dgvUsers_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int rowIndex = e.RowIndex;
            txtUserID.Text = dgvUsers.Rows[rowIndex].Cells[0].Value.ToString();
            txtFirstName.Text= dgvUsers.Rows[rowIndex].Cells[1].Value.ToString();
            txtLastName.Text= dgvUsers.Rows[rowIndex].Cells[2].Value.ToString();
            txtEmail.Text= dgvUsers.Rows[rowIndex].Cells[3].Value.ToString();
            txtUsername.Text= dgvUsers.Rows[rowIndex].Cells[4].Value.ToString();
            txtPassword.Text= dgvUsers.Rows[rowIndex].Cells[5].Value.ToString();
            txtContact.Text= dgvUsers.Rows[rowIndex].Cells[6].Value.ToString();
            txtAddress.Text= dgvUsers.Rows[rowIndex].Cells[7].Value.ToString();
            cmbGender.Text= dgvUsers.Rows[rowIndex].Cells[8].Value.ToString();
            cmbUserType.Text= dgvUsers.Rows[rowIndex].Cells[9].Value.ToString();

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            u.id = Convert.ToInt32(txtUserID.Text);
            u.first_name = txtFirstName.Text;
            u.last_name = txtLastName.Text;
            u.email = txtEmail.Text;
            u.username = txtUsername.Text;
            u.password = txtPassword.Text;
            u.contact = txtContact.Text;
            u.address = txtAddress.Text;
            u.gender = cmbGender.Text;
            u.user_type = cmbUserType.Text;
            u.added_date = DateTime.Now;
            u.added_by = 1;

            bool success = dal.Update(u);
            DataTable dt = dal.select();
            dgvUsers.DataSource = dt;
            if (success == true)
            {
                clear();
                MessageBox.Show("Update Successfully");
            }
            else
            {
                MessageBox.Show("Failed To Update");
            }
           // DataTable dt = dal.select();
            //dgvUsers.DataSource = dt;
        }

        private void frmUsers_Load(object sender, EventArgs e)
        {
            DataTable dt = dal.select();
            dgvUsers.DataSource = dt;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            u.id = Convert.ToInt32(txtUserID.Text);

            bool success = dal.Delete(u);
            DataTable dt = dal.select();
            dgvUsers.DataSource = dt;

            if (success == true)
            {
                clear();
                MessageBox.Show("Delete Successfully");

            }
            else
            {
                MessageBox.Show("Delete Failed");
            }
            //DataTable dt = dal.select();
            //dgvUsers.DataSource = dt;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keywords = txtSearch.Text;
            if (keywords != null)
            {
                DataTable dt = dal.search(keywords);
                dgvUsers.DataSource = dt;
            }
            else
            {
                DataTable dt = dal.select();
                dgvUsers.DataSource = dt;
            }
        }

        private void dgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
