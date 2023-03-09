using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace _655332_VuongVanKhai_BTL_Net_QuanLyCuaHangHoaQua
{
    public partial class SignInScreen : Form
    {
        public SignInScreen()
        {
            InitializeComponent();
            txtMatKhau.UseSystemPasswordChar = true;
            textBox1.UseSystemPasswordChar = true;
            errpro1.Clear();
        }


        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            bool check = true;
            errpro1.Clear();
            if(txtMatKhau.Text.Trim() == "" )
            {
                errpro1.SetError(txtMatKhau, "Không được bỏ trống!");
                check = false;
            }

            if (txtTaiKhoan.Text.Trim() == "" )
            {
                errpro1.SetError(txtTaiKhoan, "Không được bỏ trống!");
                check = false;

            }

            if (textBox1.Text.Trim() == "" )
            {
                errpro1.SetError(textBox1, "Không được bỏ trống!");
                check = false;
            }

            if (txtMatKhau.Text.Trim() != textBox1.Text.Trim())
            {
                MessageBox.Show("Xác nhận mật khẩu không trùng khớp");
                check = false;
                txtMatKhau.Text = txtTaiKhoan.Text = textBox1.Text = "";
            }

            if (check == false)
            {
                MessageBox.Show("Vui lòng nhập lại thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }



            if (check)
            {
                int auth = 0;
                if (radSta.Checked)
                {
                    auth = 1;
                }

                if (radAdm.Checked)
                {
                    auth = 0;
                }

                string query = "INSERT INTO Account VALUES(N'" +
                txtTaiKhoan.Text + "',N'" + txtMatKhau.Text + "',N'" + Convert.ToString(auth) + "')";
                //Class.FunctionGeneral.RunSQL(query); //Thực hiện câu lệnh sql
                SqlCommand cmd = new SqlCommand(query, Class.FunctionGeneral.sqlCon);
                int kq = cmd.ExecuteNonQuery(); 
                if(kq > 0)
                {
                    MessageBox.Show("Tạo tài khoản thành công!","Thông báo",MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtMatKhau.Text = txtTaiKhoan.Text = textBox1.Text = "";
                }

            }
            

            //string query = "Select * From Account";
            //SqlCommand cmd = new SqlCommand(sql, Class.FunctionGeneral.sqlCon);

        }

        private void SignInScreen_Load(object sender, EventArgs e)
        {
            Class.FunctionGeneral.MoKetNoi();

        }

        private void btn_DangKy_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginScreen login = new LoginScreen();
            login.Show();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            txtMatKhau.Text = txtTaiKhoan.Text = textBox1.Text = "";

        }

        private void SignInScreen_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void SignInScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_DangKy.PerformClick();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() != txtMatKhau.Text.Trim())
            {
                errpro1.SetError(textBox1, "Không trùng");
            }
            else
            {
                errpro1.Clear();
            }
        }
    }
}
