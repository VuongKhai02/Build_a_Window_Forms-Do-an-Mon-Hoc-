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
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace _655332_VuongVanKhai_BTL_Net_QuanLyCuaHangHoaQua
{
    public partial class LoginScreen : Form
    {
        public LoginScreen()
        {
            InitializeComponent();
            txtMatKhau.UseSystemPasswordChar = true;
        }



        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            errpro1.Clear();

            // kiem tra rong
            bool check = false;
            if(txtTaiKhoan.Text.Trim()=="")
            {
                errpro1.SetError(txtTaiKhoan, "Không được bỏ trống!");
                check = true;
            }
            if (txtMatKhau.Text.Trim() == "")
            {
                errpro1.SetError(txtMatKhau, "Không được bỏ trống!");
                check = true;
            }

            if (check)
            {
                MessageBox.Show("Khong duoc bo trong!", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "Select * From Account";
            SqlCommand cmd = new SqlCommand(query, Class.FunctionGeneral.sqlCon);
            SqlDataReader reader = cmd.ExecuteReader();
            

            bool auth = false;
            // 0 = admin
            int role = 0;
            Loading load = new Loading();
            if (radioButton1.Checked) role = 1;
            while (reader.Read())
            {
               if(txtTaiKhoan.Text.Trim() == reader.GetString(0) && txtMatKhau.Text.Trim() == reader.GetString(1) && role == Convert.ToInt32(reader.GetValue(2).ToString()))
                {
                    auth = true;
                    
                    load.Show();
                }
            }

            if (!auth)
            {
                MessageBox.Show("TK hoac mat khau khong dung!");
                txtMatKhau.Text = "";
            reader.Close();
                return;
            }
            this.Hide();
            Home home = new Home();
            home.userName = txtTaiKhoan.Text;
            home.role = role;
            load.Hide();
            home.Show();
            
        }

       

        private void LoginScreen_Load(object sender, EventArgs e)
        {
            Class.FunctionGeneral.MoKetNoi();

            btn_DangKy.FlatStyle = FlatStyle.Flat;
            btn_DangKy.FlatAppearance.BorderSize = 0;

            ss1.Hide();

        }
        //Check account
        private void CheckAccount() 
        {
            string sql;
            sql = "SELECT * FROM Account";
            
        }

    private void ckHienThiMatKhau_Click(object sender, EventArgs e)
        {
            if (!ckHienThiMatKhau.Checked)
            {
                txtMatKhau.UseSystemPasswordChar = true;
            }
            if (ckHienThiMatKhau.Checked)
            {
                txtMatKhau.UseSystemPasswordChar = false;

            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show("Bạn muốn thoát à?","Thật không", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (rs == DialogResult.OK)
            {
                Close();
            }

            
        }

        private void btn_DangKy_Click(object sender, EventArgs e)
        {
            this.Hide();
            SignInScreen signin = new SignInScreen();
            signin.Show();
        }

        private void txtMatKhau_Enter(object sender, EventArgs e)
        {
            ss1.Show();
            tsslbl1.Text = "Check Caps Lock on your keybroad!";
        }

        private void txtTaiKhoan_Enter(object sender, EventArgs e)
        {
            ss1.Show();
            tsslbl1.Text = "Enter your username and choose the Role";
        }

        private void LoginScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
               btnDangNhap.PerformClick();
            }
        }

        private void txtMatKhau_MouseLeave(object sender, EventArgs e)
        {
            tip.Hide(txtMatKhau);
        }

        private void txtMatKhau_MouseHover(object sender, EventArgs e)
        {
            if (Control.IsKeyLocked(Keys.CapsLock))
            {

                tip.ToolTipTitle = "Caps Lock Is On";
                tip.ToolTipIcon = ToolTipIcon.Warning;
                tip.IsBalloon = true;
                tip.SetToolTip(txtMatKhau, "Bật CapsLock có thể khiến mật khẩu của bạn bị sai!\n\nBạn nên tắt CapsLock trước khi nhập mật khẩu.");
                tip.Show("Having Caps Lock on may cause you to enter your password incorrectly.\n\nYou should press Caps Lock to turn it off before entering your password.", txtMatKhau, 5, txtMatKhau.Height - 5);
            }
        }

        private void substringBySpace()
        {
            if (txtTaiKhoan.Text.Contains(" "))
            {
                txtTaiKhoan.Text = txtTaiKhoan.Text.Substring(0, txtTaiKhoan.Text.LastIndexOf(" ") + 1);
                txtTaiKhoan.Focus();
                txtTaiKhoan.SelectionStart = txtTaiKhoan.Text.Length;
            }
            else
            {
                txtTaiKhoan.Text = "";
            }
        }
        private void txtTaiKhoan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift && e.KeyCode == Keys.Back)
            {
                substringBySpace();

                
            }
            

        }

        private void txtTaiKhoan_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
