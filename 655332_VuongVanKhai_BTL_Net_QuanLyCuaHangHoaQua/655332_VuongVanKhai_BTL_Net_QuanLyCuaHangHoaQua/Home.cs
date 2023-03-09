using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Collections;
using _655332_VuongVanKhai_BTL_Net_QuanLyCuaHangHoaQua.Class;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using COMExcel = Microsoft.Office.Interop.Excel;
using System.Reflection;


using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace _655332_VuongVanKhai_BTL_Net_QuanLyCuaHangHoaQua
{
    public partial class Home : Form
    {

        public string userName = "";
        public int role;
        public Home()
        {
            InitializeComponent();
        }

        DataTable tblCUS; //Chứa dữ liệu bảng Customer
        DataTable tblSTA; //Chứa dữ liệu bảng Staff
        DataTable tblBIL; //Chứa dữ liệu bảng Bill
        DataTable tblPRO; //Chứa dữ liệu bảng Fruit
        DataTable tblCTHDB; //Chứa dữ liệu bảng Bill_Detail

        SqlDataAdapter adapter = null;
        DataSet ds = null;
        SqlConnection sqlCon = null;
        SqlCommandBuilder builder = null;
        string str = @"Data Source=ACERASPIRE-KHAI\SQLEXPRESS;Initial Catalog=QuanLyBanHoaQua;Integrated Security=True";

        private void Home_Load(object sender, EventArgs e)
        {
            //Khai báo biến Role
            string roleString = "";
            //Ẩn các panel
            panHangHoa.Hide();
            panStaff.Hide();
            panCustomer.Hide(); 
            panBill.Hide(); 
            dgvStaff.Hide();
            dgvBill.Hide();
            dgvCustomer.Hide();
            dgvProduct.Hide();

            panelSearch_Sta.Hide();
            panelSearch_Bill.Hide();
            panelSearch_Cus.Hide();
            panelSearch_Pro.Hide();

            btnCloseSearch_Sta.Hide();
            btnUpdate_Sale.Enabled = false;
            btnDelete_Sale.Enabled = false;
            btnPrint_Sale.Enabled = false;

            Class.FunctionGeneral.MoKetNoi(); //Mở kết nối
            LoadDataCustomerGridView(); //Lấy data từ bảng Customer
            LoadDataStaffGridView();    //Lấy data từ bảng Staff
            LoadDataBillGridView();    //Lấy data từ bảng Bill
            LoadDataProductGridView();//Lấy data từ bảng Fruit

            //Phân quyền
            if (role == 1)
            {
                roleString = " Nhân viên";
                btnBill.Enabled = false;
                btnCustomer.Enabled = false;    
                btnProduct.Enabled = false;     
                btnStaff.Enabled = false;
                tableLayoutPanelStaff.Enabled = false;
            }
            else if(role == 0)
            {
                roleString = " Quản lý";
            }

            tsslbl1.Text = "  Quản lý" ;
            tsstbl1_1.Text = "Account: " + userName + "|| Role: " + roleString;
            txtXinChao.Text = userName;
            txtQuyen.Text = roleString;
            txtThoiGianDangNhap.Text = DateTime.Now.ToString();

            //Đẩy dữ liệu từ CSDL vào comboBox
            FunctionGeneral.FillCombo("SELECT STA_ID from Staff", cbSta_id, "STA_ID", "STA_ID");
            //Mặc định combox chưa đc chọn.
            cbSta_id.SelectedIndex = -1;
            FunctionGeneral.FillCombo("SELECT FRU_ID FROM Fruit", cbPro_id, "FRU_ID", "FRU_ID");
            cbPro_id.SelectedIndex = -1;
            FunctionGeneral.FillCombo("SELECT CUS_ID from Customer", cbCus_id, "CUS_ID", "CUS_ID");
            cbCus_id.SelectedIndex = -1;


            //Load Info Hóa đơn khi khởi tạo chương trình
            if (txtBil_id.Text != "")
            {
                LoadInfoHoaDon();
            }

            //Ẩn chức năng đổi mật khẩu khi khởi tạo
            panDoiMatKhau.Hide();

            textBox23.Hide();
            button8.Hide();
            button1.Hide();
        }

        //========================================== CUSTOMER ==============================================
        private void LoadDataCustomerGridView()
        {
            string sql;
            sql = "SELECT * FROM Customer";
            tblCUS = Class.FunctionGeneral.GetDataToTable(sql); //Đọc dữ liệu từ bảng
            dgvCustomer.DataSource = tblCUS; //Nguồn dữ liệu            
            dgvCustomer.AllowUserToAddRows = false; //Không cho người dùng thêm dữ liệu trực tiếp
            dgvCustomer.EditMode = DataGridViewEditMode.EditProgrammatically; //Không cho sửa dữ liệu trực tiếp
            dgvCustomer.CurrentCell.Selected = false;
        }
        private void btnCustomer_Click(object sender, EventArgs e)
        {
            LoadDataCustomerGridView(); //Nạp lại DataGridView
            //Set menuStrip
            tsslbl1.Text = "  Quản lý >> Khách hàng";

            panCustomer.Show();
            dgvCustomer.Show();
            tableLayoutPanelCustomer.Show();    
            panStaff.Hide();
            dgvStaff.Hide();
            tableLayoutPanelStaff.Hide();
            panHangHoa.Hide();
            dgvProduct.Hide();
            tableLayoutPanelProduct.Hide();
            panBill.Hide();
            dgvBill.Hide();
            tableLayoutPanelBill.Hide();
            //Ẩn nút sửa và xóa, textbox tìm kiếm
            btnUpdate_Cus.Enabled = false;
            btnDelete_Cus.Enabled = false;
            btnCloseSearch_Cus.Hide();
            //Thông báo khi ở chế độ xem
            MessageBox.Show("Bạn đang ở chế độ xem!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            tsslbl1.Text = "  Quản lý >> Khách hàng >> Xem"  ;
        }

        //Ấn vào Cell trong Dgv
        private void dgvCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //Câu lệnh truy vấn
            string sqlQuery = "Select * From Customer";
            sqlCon = new SqlConnection(str);
            adapter = new SqlDataAdapter(sqlQuery, sqlCon);
            builder = new SqlCommandBuilder(adapter);
            ds = new DataSet();
            adapter.Fill(ds, "Customer");
            dgvCustomer.DataSource = ds.Tables["Customer"];
                vt = e.RowIndex;
                DataRow row = ds.Tables["Customer"].Rows[vt];
                textBox1.Text = row["CUS_ID"].ToString().Trim();
                textBox2.Text = row["CUS_Name"].ToString().Trim();
                textBox3.Text = row["CUS_Address"].ToString().Trim();
                textBox4.Text = row["CUS_Phone"].ToString().Trim();
            tsslbl1.Text = "  Quản lý >> Khách hàng >> Sửa";
            //Hiện nút Sửa và Xóa
            btnUpdate_Cus.Enabled = true;
            btnDelete_Cus.Enabled = true;

            textBox1.Cursor = Cursors.No;
        }

        //Xóa dữ liệu textbox
        private void XoaDuLieuCUSForm()
        {
            textBox1.Text = textBox2.Text = textBox4.Text = textBox3.Text = "";
        }

        //Khởi tạo cellClick ban đầu = -1
        int vt = -1;

        //Thông báo là đang ở chế độ thêm khi click vào text box.
        private void textBox1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Bạn đang ở chế độ 'Thêm'", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            tsslbl1.Text = "  Quản lý >> Khách hàng >> Thêm";
        }

        //Sự kiện THÊM.
        private void btnCreate_Cus_Click(object sender, EventArgs e)
        {
            string sql; //Lưu lệnh sql
            if (textBox1.Text.Trim().Length == 0) //Nếu chưa nhập mã KH
            {
                MessageBox.Show("Bạn phải nhập mã khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox1.Focus();
                return;
            }
            if (textBox2.Text.Trim().Length == 0) //Nếu chưa nhập tên KH
            {
                MessageBox.Show("Bạn phải nhập tên khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox2.Focus();
                return;
            }

            if (textBox3.Text.Trim().Length == 0) //Nếu chưa nhập SDT
            {
                MessageBox.Show("Bạn phải nhập sdt khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox3.Focus();
                return;
            }
            if (textBox4.Text.Trim().Length == 0) //Nếu chưa nhập DC
            {
                MessageBox.Show("Bạn phải nhập địa chỉ khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox4.Focus();
                return;
            }

            //Kiểm tra xem mã khách hàng vừa nhập đã tồn tại chưa
            sql = "Select CUS_ID From Customer where CUS_ID=N'" + textBox1.Text.Trim() + "'";
            if (Class.FunctionGeneral.CheckKey(sql))
            {
                MessageBox.Show("Mã KH này đã có, bạn phải nhập mã khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                XoaDuLieuCUSForm();
                textBox1.Focus();
                return;
            }
           //Câu lệnh SQL thêm khách hàng.
            sql = "INSERT INTO Customer VALUES(N'" +
                textBox1.Text + "',N'" + textBox2.Text + "',N'" + textBox3.Text + "',N'" + textBox4.Text + "')";
            Class.FunctionGeneral.RunSQL(sql); //Thực hiện câu lệnh sql
            LoadDataCustomerGridView(); //Nạp lại DataGridView
            XoaDuLieuCUSForm();
        }

        //Sự kiện SỬA.
        private void btnUpdate_Cus_Click(object sender, EventArgs e)
        {
            string sql; //Lưu câu lệnh sql
            if (tblCUS.Rows.Count == 0)
            {
                MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (vt == -1)
            {
                MessageBox.Show("Bạn chưa chọn dữ liệu để sửa!");
                return;
            }
            
            if (textBox2.Text.Trim().Length == 0) //nếu chưa nhập tên KH
            {
                MessageBox.Show("Bạn chưa nhập tên KH", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //Câu lệnh SQL sửa dữ liệu khách hàng. 
            sql = "UPDATE Customer SET CUS_Name=N'" +
                textBox2.Text.ToString() + "',CUS_Address=N'" +
                textBox3.Text.ToString() + "',CUS_Phone=N'" +
                textBox4.Text.ToString() + 
                "' WHERE CUS_ID=N'" + textBox1.Text + "'";
            Class.FunctionGeneral.RunSQL(sql);
            LoadDataCustomerGridView();
            XoaDuLieuCUSForm();
        }

        //Sự kiện XÓA.
        private void btnDelete_Cus_Click(object sender, EventArgs e)
        {
            string sql;
            if (tblCUS.Rows.Count == 0)
            {
                MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (vt == -1)
            {
                MessageBox.Show("Bạn chưa chọn dữ liệu để sửa!","Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Bạn có muốn xoá không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                sql = "DELETE Customer WHERE CUS_ID=N'" + textBox1.Text + "'";
                Class.FunctionGeneral.RunSqlDel(sql);
                LoadDataCustomerGridView();
                XoaDuLieuCUSForm();
            }
        }

        //Sự kiện BỎ QUA.
        private void btnSkip_Cus_Click(object sender, EventArgs e)
        {
            XoaDuLieuCUSForm();
            //Chuyển trạng thái ô vừa đc chọn thành Unselected
            dgvCustomer.CurrentCell.Selected = false;
            textBox1.Cursor = Cursors.IBeam;
        }

        //Sự kiện TÌM KIẾM.
        private void btnSearch_Cus_Click(object sender, EventArgs e)
        {
            panelSearch_Cus.Show();
            btnCloseSearch_Cus.Show();
        }
        private void btnCloseSearch_Cus_Click(object sender, EventArgs e)
        {
            panelSearch_Cus.Hide();
        }
        //Thực thi tìm kiếm
        private void btnClose_Click(object sender, EventArgs e)
        {
            if(txtSearch_Cus.Text == "")
            {
                MessageBox.Show("Chưa nhập dữ liệu tìm kiếm");
            }
            else
            {
                //Câu lệnh thực thi tìm kiếm
            string query = "select * from Customer where CUS_Name like N'%" + txtSearch_Cus.Text + "%'";
            dgvCustomer.DataSource = Class.FunctionGeneral.GetDataToTable(query);

            }
        }

        //Sự kiện placholder
        private void txtSearch_Cus_Enter(object sender, EventArgs e)
        {
            if (txtSearch_Cus.Text == "dd/mm/yyyy")
            {
                txtSearch_Cus.Text = "";
                txtSearch_Cus.ForeColor = Color.Black;
            }
        }

        //========================================== STAFF ==============================================

        //Xóa dữ liệu textbox
        private void dgvStaff_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string sqlQuery = "Select * From Staff";
            adapter = null;
            sqlCon = new SqlConnection(str);
            adapter = new SqlDataAdapter(sqlQuery, sqlCon);
            builder = new SqlCommandBuilder(adapter);
            ds = new DataSet();
            adapter.Fill(ds, "Staff");
            dgvStaff.DataSource = ds.Tables["Staff"];

            vt = e.RowIndex;
            DataRow row = ds.Tables["Staff"].Rows[vt];
            textBox8.Text = row["STA_ID"].ToString().Trim();
            textBox7.Text = row["STA_Name"].ToString().Trim();
            textBox6.Text = row["STA_Address"].ToString().Trim();
            textBox5.Text = row["STA_Phone"].ToString().Trim();
            textBox9.Text = row["STA_Gender"].ToString().Trim();
            cbGender_Sta.Text = row["STA_Gender"].ToString().Trim();
            //string a = row["STA_Gender"].ToString().Trim();

            //DateTime b = Convert.ToDateTime(a);
            //textBox10.Text = b.ToString("MM/dd/yyyy");
            //textBox10.Text = row["STA_DateOfBirth"].ToString().Trim();
            string a = row["STA_DateOfBirth"].ToString().Trim();
            DateTime b = Convert.ToDateTime(a);
            textBox10.Text = b.ToString("MM/dd/yyyy");

            tsslbl1.Text = "  Quản lý >> Nhân viên >> Sửa";
            btnUpdate_Sta.Enabled = true;
            btnDelete_Sta.Enabled = true;

            textBox8.Cursor = Cursors.No;
        }
        private void LoadDataStaffGridView()
        {
            string sql;
            sql = "SELECT * FROM Staff";
            tblSTA = Class.FunctionGeneral.GetDataToTable(sql); //Đọc dữ liệu từ bảng
            dgvStaff.DataSource = tblSTA; //Nguồn dữ liệu            
            dgvStaff.AllowUserToAddRows = false; //Không cho người dùng thêm dữ liệu trực tiếp
            dgvStaff.EditMode = DataGridViewEditMode.EditProgrammatically; //Không cho sửa dữ liệu trực tiếp
            dgvStaff.CurrentCell.Selected = false;

        }
        private void XoaDuLieuSTAForm()
        {
            textBox5.Text = textBox6.Text = textBox7.Text = textBox9.Text = textBox10.Text = textBox8.Text = "";
        }
        private void btnStaff_Click(object sender, EventArgs e)
        {
            tsslbl1.Text = "  Quản lý >> Nhân viên";
            if (textBox1.Text.Trim() != "")
            {
                DialogResult rs = MessageBox.Show("Những sự thay đổi chưa được lưu, bạn thực sự muốn rời đi?", "Rời đi", MessageBoxButtons.OKCancel);
                if (rs == DialogResult.OK)
                {
                    panStaff.Show();
                    dgvStaff.Show();
                    tableLayoutPanelStaff.Show();
                    XoaDuLieuCUSForm();
                }
            }
            else
            {
                panStaff.Show();
                dgvStaff.Show();
                tableLayoutPanelStaff.Show();
                //panHangHoa.Hide();
            }
            btnUpdate_Sta.Enabled = false;
            btnDelete_Sta.Enabled = false;

            btnCloseSearch_Sta.Hide();

            MessageBox.Show("Bạn đang ở chế độ xem!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            tsslbl1.Text = "  Quản lý >> Nhân viên >> Xem";
        }
        //Sự kiện TÌM KIẾM.
        private void btnSearch_Sta_Click(object sender, EventArgs e)
        {
            panelSearch_Sta.Show();
            btnCloseSearch_Sta.Show();
        }
        private void btnCloseSearch_Sta_Click(object sender, EventArgs e)
        {
            panelSearch_Sta.Hide();
        }

        //Sự kiện THÊM.
        private void btnCreate_Sta_Click(object sender, EventArgs e)
        {
            string sql; //Lưu lệnh sql
            if (textBox8.Text.Trim().Length == 0) //Nếu chưa nhập mã KH
            {
                MessageBox.Show("Bạn phải nhập mã nhân viên", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox8.Focus();
                return;
            }
            if (textBox7.Text.Trim().Length == 0) //Nếu chưa nhập tên KH
            {
                MessageBox.Show("Bạn phải nhập tên nhân viên", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox7.Focus();
                return;
            }

            if (textBox6.Text.Trim().Length == 0) //Nếu chưa nhập DC
            {
                MessageBox.Show("Bạn phải nhập địa chỉ nhân viên", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox6.Focus();
                return;
            }
            if (textBox5.Text.Trim().Length == 0) //Nếu chưa nhập DC
            {
                MessageBox.Show("Bạn phải nhập sđt nhân viên", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox5.Focus();
                return;
            }
            if (textBox9.Text.Trim().Length == 0) //Nếu chưa nhập DC
            {
                MessageBox.Show("Bạn phải nhập giới tính nhân viên", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox9.Focus();
                return;
            }
            
            if (!FunctionGeneral.IsDate(textBox10.Text))
            {
                MessageBox.Show("Bạn phải nhập lại ngày sinh", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox10.Focus();
                return;
            }

            //Chekc ma da co chua làm
            sql = "Select STA_ID From Staff where STA_ID=N'" + textBox8.Text.Trim() + "'";
            if (Class.FunctionGeneral.CheckKey(sql))
            {
                MessageBox.Show("Mã KH này đã có, bạn phải nhập mã khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                XoaDuLieuCUSForm();
                textBox8.Focus();
                return;
            }

            sql = "INSERT INTO Staff VALUES(N'" +
                textBox8.Text + "',N'" + textBox7.Text + "',N'" + textBox6.Text + "',N'" + textBox5.Text + "',N'" +
                textBox9.Text + "',N'" + FunctionGeneral.ConvertDateTime(textBox10.Text) + "')";
            Class.FunctionGeneral.RunSQL(sql); //Thực hiện câu lệnh sql
            LoadDataStaffGridView(); //Nạp lại DataGridView
            XoaDuLieuSTAForm();
        }

        //Sự kiện SỬA.
        private void btnUpdate_Sta_Click(object sender, EventArgs e)
        {
            string sql; //Lưu câu lệnh sql
            if (tblCUS.Rows.Count == 0)
            {
                MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (vt == -1)
            {
                MessageBox.Show("Bạn chưa chọn dữ liệu để sửa!");
                return;
            }

            sql = "UPDATE Staff SET STA_Name=N'" +
                textBox7.Text.ToString() + "',STA_Address=N'" +
                textBox6.Text.ToString() + "',STA_Phone=N'" +
                textBox5.Text.ToString() + "',STA_Gender=N'" +
                textBox9.Text.ToString() + "',STA_DateOfBirth=N'" +
                textBox10.Text.ToString() +
                "' WHERE STA_ID=N'" + textBox8.Text + "'";
            Class.FunctionGeneral.RunSQL(sql);
            LoadDataStaffGridView();
            XoaDuLieuSTAForm();
        }

        //Sự kiện XÓA.
        private void btnDelete_Sta_Click(object sender, EventArgs e)
        {
            string sql;
            if (tblCUS.Rows.Count == 0)
            {
                MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (vt == -1)
            {
                MessageBox.Show("Bạn chưa chọn dữ liệu để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Bạn có muốn xoá không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                sql = "DELETE Staff WHERE STA_ID=N'" + textBox8.Text + "'";
                Class.FunctionGeneral.RunSqlDel(sql);
                LoadDataStaffGridView();
                XoaDuLieuSTAForm();
            }
        }

        //Sự kiện BỎ QUA.
        private void btnSkip_Sta_Click(object sender, EventArgs e)
        {
            XoaDuLieuSTAForm();
            dgvStaff.CurrentCell.Selected = false;
            textBox8.Cursor = Cursors.IBeam;
        }
        //Thực thi tìm kiếm
        private void txtSearch_Sta_Enter(object sender, EventArgs e)
        {
            if (txtSearch_Sta.Text == "Enter Name")
            {
                txtSearch_Sta.Text = "";
                txtSearch_Sta.ForeColor = Color.Black;
            }
        }

        private void btn_rs_sta_Click(object sender, EventArgs e)
        {
            if (txtSearch_Sta.Text == "")
            {
                MessageBox.Show("Chưa nhập dữ liệu tìm kiếm");
            }

            else
            {
                string query = "select * from Staff where STA_Name like N'%" + txtSearch_Sta.Text + "%'";
                dgvStaff.DataSource = Class.FunctionGeneral.GetDataToTable(query);
            }
        }
        private void cbGender_Sta_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox9.Text = cbGender_Sta.Text;
        }

        private void textBox10_Enter(object sender, EventArgs e)
        {
            if (textBox10.Text == "dd/mm/yyyy")
            {
                textBox10.Text = "";
                textBox10.ForeColor = Color.Black;
            }
        }

        //========================================== PRODUCT ==============================================
        private void LoadDataProductGridView()
        {
            string sql;
            sql = "SELECT * FROM Fruit";
            tblPRO = Class.FunctionGeneral.GetDataToTable(sql); //Đọc dữ liệu từ bảng
            dgvProduct.DataSource = tblPRO; //Nguồn dữ liệu            
            dgvProduct.AllowUserToAddRows = false; //Không cho người dùng thêm dữ liệu trực tiếp
            dgvProduct.EditMode = DataGridViewEditMode.EditProgrammatically; //Không cho sửa dữ liệu trực tiếp
            dgvProduct.CurrentCell.Selected = false;

        }
        private void dgvProduct_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string sqlQuery = "Select * From Fruit";
            adapter = null;
            sqlCon = new SqlConnection(str);
            adapter = new SqlDataAdapter(sqlQuery, sqlCon);
            builder = new SqlCommandBuilder(adapter);
            ds = new DataSet();
            adapter.Fill(ds, "Fruit");
            dgvProduct.DataSource = ds.Tables["Fruit"];

            vt = e.RowIndex;
            DataRow row = ds.Tables["Fruit"].Rows[vt];
            textBox16.Text = row["FRU_ID"].ToString().Trim();
            textBox15.Text = row["FRS_ID"].ToString().Trim();
            textBox12.Text = row["FRU_Name"].ToString().Trim();
            textBox13.Text = row["FRU_Amount"].ToString().Trim();
            textBox14.Text = row["FRU_Unit"].ToString().Trim();
            textBox11.Text = row["FRU_Price"].ToString().Trim();
            tsslbl1.Text = "  Quản lý >> Hàng hóa >> Sửa";

            btnUpdate_Pro.Enabled = true;
            btnDelete_Pro.Enabled = true;

            textBox16.Cursor = Cursors.No;
        }
        private void btnProduct_Click(object sender, EventArgs e)
        {
            tsslbl1.Text = "  Quản lý >> Sản phẩm";
            if ((textBox20.Text.Trim()) != "")
            {
                DialogResult rs = MessageBox.Show("Những sự thay đổi chưa được lưu, bạn thực sự muốn rời đi?", "Rời đi", MessageBoxButtons.OKCancel);
                if (rs == DialogResult.OK)
                {
                    panHangHoa.Show();
                    dgvProduct.Show();
                    tableLayoutPanelProduct.Show();
                    panCustomer.Hide();
                    dgvCustomer.Hide();
                    tableLayoutPanelCustomer.Hide();

                    panStaff.Hide();
                    dgvStaff.Hide();
                    tableLayoutPanelStaff.Hide();

                    panBill.Hide();
                    dgvBill.Hide();
                    tableLayoutPanelBill.Hide();
                    XoaDuLieuCUSForm();
                }
            }
            else
            {
                panHangHoa.Show();
                dgvProduct.Show();
                tableLayoutPanelProduct.Show();
                panCustomer.Hide();
                dgvCustomer.Hide();
                tableLayoutPanelCustomer.Hide();

                panStaff.Hide();
                dgvStaff.Hide();
                tableLayoutPanelStaff.Hide();

                panBill.Hide();
                dgvBill.Hide();
                tableLayoutPanelBill.Hide();
            }
            btnUpdate_Pro.Enabled = false;
            btnDelete_Pro.Enabled = false;

            btnCloseSearch_Pro.Hide();

            MessageBox.Show("Bạn đang ở chế độ xem!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            tsslbl1.Text = "  Quản lý >> Hàng hóa >> Xem";
        }

        //Xóa dữ liệu textbox
        private void XoaDuLieuPROForm()
        {
            textBox16.Text = textBox15.Text = textBox12.Text = textBox13.Text = textBox14.Text = textBox11.Text = "";
        }

        //Chức năng TÌM KIẾM
        private void btnSearch_Pro_Click(object sender, EventArgs e)
        {
            panelSearch_Pro.Show();
            btnCloseSearch_Pro.Show();
        }
        private void btnCloseSearch_Pro_Click(object sender, EventArgs e)
        {
            panelSearch_Pro.Hide();
        }

        //Sự kiện THÊM.
        private void btnCreate_Pro_Click(object sender, EventArgs e)
        {
            string sql; //Lưu lệnh sql
            if (textBox16.Text.Trim().Length == 0) //Nếu chưa nhập mã KH
            {
                MessageBox.Show("Bạn phải nhập mã hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox16.Focus();
                return;
            }
            if (textBox15.Text.Trim().Length == 0) //Nếu chưa nhập tên KH
            {
                MessageBox.Show("Bạn phải nhập mã Nguồn hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox15.Focus();
                return;
            }

            if (textBox12.Text.Trim().Length == 0) //Nếu chưa nhập DC
            {
                MessageBox.Show("Bạn phải nhập tên hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox12.Focus();
                return;
            }
            if (textBox13.Text.Trim().Length == 0) //Nếu chưa nhập DC
            {
                MessageBox.Show("Bạn phải nhập số lượng hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox13.Focus();
                return;
            }
            if (textBox14.Text.Trim().Length == 0) //Nếu chưa nhập DC
            {
                MessageBox.Show("Bạn phải nhập đơn vị tính của mặt hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox14.Focus();
                return;
            }
            if (textBox11.Text.Trim().Length == 0) //Nếu chưa nhập DC
            {
                MessageBox.Show("Bạn phải nhập giá mặt hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox11.Focus();
                return;
            }

            sql = "Select FRU_ID From Fruit where FRU_ID=N'" + textBox16.Text.Trim() + "'";
            if (Class.FunctionGeneral.CheckKey(sql))
            {
                MessageBox.Show("Mã Hàng này đã có, bạn phải nhập mã khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                XoaDuLieuPROForm();
                textBox16.Focus();
                return;
            }

            sql = "INSERT INTO Fruit VALUES(N'" +
                textBox16.Text + "',N'" + textBox15.Text + "',N'" + textBox12.Text + "',N'" + textBox13.Text + "',N'" +
                textBox14.Text + "',N'" + textBox11.Text + "',' ',' ')";
            Class.FunctionGeneral.RunSQL(sql); //Thực hiện câu lệnh sql
            LoadDataProductGridView(); //Nạp lại DataGridView
            XoaDuLieuPROForm();
        }

        //Sự kiện SỬA.
        private void btnUpdate_Pro_Click(object sender, EventArgs e)
        {
            string sql; //Lưu câu lệnh sql
            if (tblCUS.Rows.Count == 0)
            {
                MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (vt == -1)
            {
                MessageBox.Show("Bạn chưa chọn dữ liệu để sửa!");
                return;
            }

            sql = "UPDATE Fruit SET FRS_ID=N'" +
                textBox15.Text.ToString() + "',FRU_Name=N'" +
                textBox12.Text.ToString() + "',FRU_Amount=N'" +
                textBox13.Text.ToString() + "',FRU_Unit=N'" +
                textBox14.Text.ToString() + "',FRU_Price=N'" +
                textBox11.Text.ToString() +
                "' WHERE FRU_ID=N'" + textBox16.Text + "'";
            Class.FunctionGeneral.RunSQL(sql);
            LoadDataProductGridView();
            XoaDuLieuPROForm();
        }

        //Sự kiện XÓA.
        private void btnDelete_Pro_Click(object sender, EventArgs e)
        {
            string sql;
            if (tblCUS.Rows.Count == 0)
            {
                MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (vt == -1)
            {
                MessageBox.Show("Bạn chưa chọn dữ liệu để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Bạn có muốn xoá không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                sql = "DELETE Fruit WHERE FRU_ID=N'" + textBox16.Text + "'";
                Class.FunctionGeneral.RunSqlDel(sql);
                LoadDataProductGridView();
                XoaDuLieuPROForm();
            }
        }

        //Sự kiện BỎ QUA.
        private void btnSkip_Pro_Click(object sender, EventArgs e)
        {
            XoaDuLieuPROForm();
            dgvBill.CurrentCell.Selected = false;
            textBox16.Cursor = Cursors.IBeam;
        }
        //thực thi tìm Kiếm
        private void btn_rs_pro_Click(object sender, EventArgs e)
        {
            if (txtSearch_Pro.Text == "")
            {
                MessageBox.Show("Chưa nhập dữ liệu tìm kiếm");
            }
            else
            {
                string query = "select * from Fruit where FRS_ID like N'%" + txtSearch_Pro.Text + "%'";
                dgvProduct.DataSource = Class.FunctionGeneral.GetDataToTable(query);

            }
        }

        private void txtSearch_Pro_Enter(object sender, EventArgs e)
        {
            if (txtSearch_Pro.Text == "Enter FruitSource_ID")
            {
                txtSearch_Pro.Text = "";
                txtSearch_Pro.ForeColor = Color.Black;
            }
        }

        //=================== BILL =======================
        private void dgvBill_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string sqlQuery = "Select * From Bill";
            adapter = null;

            sqlCon = new SqlConnection(str);
            adapter = new SqlDataAdapter(sqlQuery, sqlCon);
            builder = new SqlCommandBuilder(adapter);
            ds = new DataSet();
            adapter.Fill(ds, "Bill");
            dgvBill.DataSource = ds.Tables["Bill"];
            
            vt = e.RowIndex;
            DataRow row = ds.Tables["Bill"].Rows[vt];
            textBox20.Text = row["BIL_IDAuto"].ToString().Trim();
            textBox19.Text = row["STA_ID"].ToString().Trim();
            textBox17.Text = row["CUS_ID"].ToString().Trim();
            //textBox18.Text = row["BIL_Date"].ToString().Trim();
            string a = row["BIL_Date"].ToString().Trim();

            DateTime b = Convert.ToDateTime(a);
            textBox18.Text = b.ToString("MM/dd/yyyy");
            //textBox10.Text = row["STA_DateOfBirth"].ToString().Trim();
            textBox22.Text = row["BIL_Discount"].ToString().Trim();
            textBox21.Text = row["BIL_Total"].ToString().Trim();

            btnUpdate_Bil.Enabled = true;
            btnDelete_Bil.Enabled = true;
            tsslbl1.Text = "  Quản lý >> Hóa đơn >> Sửa";

            textBox20.Cursor = Cursors.No;
        }
        private void LoadDataBillGridView()
        {
            string sql;
            sql = "SELECT * FROM Bill";
            tblBIL = Class.FunctionGeneral.GetDataToTable(sql); //Đọc dữ liệu từ bảng
            dgvBill.DataSource = tblBIL; //Nguồn dữ liệu            
            /*
            dgvCustomer.Columns[0].HeaderText = "Mã chất liệu";
            dgvCustomer.Columns[1].HeaderText = "Mã chất liệu";
            dgvCustomer.Columns[0].Width = 100;
            dgvCustomer.Columns[1].Width = 300;
            */
            dgvBill.AllowUserToAddRows = false; //Không cho người dùng thêm dữ liệu trực tiếp
            dgvBill.EditMode = DataGridViewEditMode.EditProgrammatically; //Không cho sửa dữ liệu trực tiếp
            dgvBill.CurrentCell.Selected = false;

        }

        //Xóa dữ liệu textbox
        private void XoaDuLieuBILForm()
        {
            textBox17.Text = textBox18.Text = textBox19.Text = textBox20.Text = textBox21.Text = textBox22.Text = "";
        }
        private void btnBill_Click(object sender, EventArgs e)
        {
            tsslbl1.Text = "  Quản lý >> Hóa đơn";
            if (textBox16.Text.Trim() != "")
            {
                DialogResult rs = MessageBox.Show("Những sự thay đổi chưa được lưu, bạn thực sự muốn rời đi?", "Rời đi", MessageBoxButtons.OKCancel);
                if (rs == DialogResult.OK)
                {
                    panBill.Show();
                    dgvBill.Show();
                    tableLayoutPanelBill.Show();
                    XoaDuLieuCUSForm();
                }
            }
            else
            {
                panBill.Show();
                dgvBill.Show();
                tableLayoutPanelBill.Show();
            }

            btnUpdate_Bil.Enabled = false;
            btnDelete_Bil.Enabled = false;

            btnCloseSearch_Bill.Hide();
            MessageBox.Show("Bạn đang ở chế độ xem!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            tsslbl1.Text = "  Quản lý >> Hóa đơn >> Xem";

            textBox20.Cursor = Cursors.No;
        }

        //CHỨC NĂNG TÌM KIẾM
        private void btnSearch_Bil_Click(object sender, EventArgs e)
        {
            panelSearch_Bill.Show();
            btnCloseSearch_Bill.Show();
        }
        private void btnCloseSearch_Bill_Click(object sender, EventArgs e)
        {
            panelSearch_Bill.Hide();
        }

        //Sự kiện THÊM.
        

        private void btnCreate_Bil_Click(object sender, EventArgs e)
        {
                string sql; //Lưu lệnh sql
                
                if (textBox19.Text.Trim().Length == 0) //Nếu chưa nhập tên KH
                {
                    MessageBox.Show("Bạn phải nhập mã mã nhân viên", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox19.Focus();
                    return;
                }

                if (textBox17.Text.Trim().Length == 0) //Nếu chưa nhập DC
                {
                    MessageBox.Show("Bạn phải nhập mã khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox17.Focus();
                    return;
                }
            /* Date
            if (textBox18.Text.Trim().Length == 0) //Nếu chưa nhập DC
            {
                MessageBox.Show("Bạn phải nhập ngày xuất", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox18.Focus();
                return;
            }
            *//* Discount
            if (textBox22.Text.Trim().Length == 0) //Nếu chưa nhập DC
            {
                MessageBox.Show("Bạn phải nhập giá mặt hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox22.Focus();
                return;
            }*/
            /* Thành tiền
        if (textBox21.Text.Trim().Length == 0) //Nếu chưa nhập DC
            {
                MessageBox.Show("Bạn phải nhập giá mặt hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox21.Focus();
                return;
            }
            */

            /*sql = "Select BIL_ID From Bill where BIL_ID=N'" + textBox20.Text.Trim() + "'";
            if (Class.FunctionGeneral.CheckKey(sql))
            {
                MessageBox.Show("Mã ID này đã có, bạn phải nhập mã khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                XoaDuLieuPROForm();
                textBox16.Focus();
                return;
            }*/

            if (textBox18.Text.Trim() == "" )
            {
                    sql = "INSERT INTO Bill(STA_ID,CUS_ID, BIL_Discount, BIL_Total) VALUES(N'" + textBox19.Text + "',N'"  + textBox17.Text + "',N'" +
                    textBox22.Text + "',N'" + textBox21.Text + "')";
                    Class.FunctionGeneral.RunSQL(sql);
                    LoadDataBillGridView();
                    XoaDuLieuBILForm();
            }
            else
            {
                sql = "INSERT INTO Bill VALUES(N'" + textBox19.Text + "',N'" + textBox17.Text + "',N'" + FunctionGeneral.ConvertDateTime(textBox18.Text) + "',N'" +
                textBox22.Text + "',N'" + textBox21.Text + "')";

                Class.FunctionGeneral.RunSQL(sql);

                LoadDataBillGridView();
                    XoaDuLieuBILForm();
            }    


        }

        //Sự kiện SỬA.
        private void btnUpdate_Bil_Click(object sender, EventArgs e)
        {
                string sql; //Lưu câu lệnh sql
                if (tblCUS.Rows.Count == 0)
                {
                    MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (vt == -1)
                {
                    MessageBox.Show("Bạn chưa chọn dữ liệu để sửa!");
                    return;
                }
                sql = "UPDATE Bill SET STA_ID=N'" +
                    textBox19.Text.ToString() + "',CUS_ID=N'" +
                    textBox17.Text.ToString() + "',BIL_Date=N'" +
                    textBox18.Text.ToString() + "',BIL_Discount=N'" +
                    textBox22.Text.ToString() + "',BIL_Total=N'" +
                    textBox21.Text.ToString() +
                    "' WHERE BIL_IDAuto=N'" + textBox20.Text + "'";
                Class.FunctionGeneral.RunSQL(sql);
                LoadDataBillGridView();
                XoaDuLieuBILForm();
        }

        //Sự kiện XÓA.
        private void btnDelete_Bil_Click(object sender, EventArgs e)
        {
                string sql;
                if (tblCUS.Rows.Count == 0)
                {
                    MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (vt == -1)
                {
                    MessageBox.Show("Bạn chưa chọn dữ liệu để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (MessageBox.Show("Bạn có muốn xoá không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    sql = "DELETE Bill WHERE BIL_IDAuto=N'" + textBox20.Text + "'";
                    Class.FunctionGeneral.RunSQL(sql);
                    sql = "DELETE BillDetail WHERE BIL_IDAuto=N'" + textBox20.Text + "'";
                    Class.FunctionGeneral.RunSQL(sql);

                LoadDataBillGridView();
                    XoaDuLieuBILForm();
                }
        }

        //Sự kiện BỎ QUA.
        private void btnSkip_Bil_Click(object sender, EventArgs e)
        {
            XoaDuLieuBILForm();
            dgvBill.CurrentCell.Selected = false;
            textBox20.Cursor = Cursors.IBeam;
        }
        //Thực thi Tìm kiếm
        private void txtSearch_Bil_Enter(object sender, EventArgs e)
        {
            if (txtSearch_Bil.Text == "Enter Staff_ID")
            {
                txtSearch_Bil.Text = "";
                txtSearch_Bil.ForeColor = Color.Black;
            }
        }

        private void btn_rs_bil_Click(object sender, EventArgs e)
        {
            if (txtSearch_Bil.Text == "")
            {
                MessageBox.Show("Chưa nhập dữ liệu tìm kiếm");
            }
            else
            {
                string query = "select * from Bill where STA_ID like N'%" + txtSearch_Bil.Text + "%'";
                dgvBill.DataSource = Class.FunctionGeneral.GetDataToTable(query);

            }
        }
        //Các thông báo khi bắt đầu nhập mã, sẽ ở chế độ thêm.

        private void textBox19_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Bạn đang ở chế độ 'Thêm'", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            tsslbl1.Text = "  Quản lý >> Hóa đơn >> Thêm";
        }

        private void textBox8_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Bạn đang ở chế độ 'Thêm'", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            tsslbl1.Text = "  Quản lý >> Nhân viên >> Thêm";
        }
        private void textBox16_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Bạn đang ở chế độ 'Thêm'", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            tsslbl1.Text = "  Quản lý >> Hàng hóa >> Thêm";
        }
        //Ấn nhầm, sự kiên Rỗng
        private void tc1_DrawItem(object sender, DrawItemEventArgs e)
        {
        }
        //Sự kiện ấn enter khi tìm kiếm sẽ nhảy sang nút tìm kiếm
        private void txtSearch_Bil_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void txtSearch_Sta_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void txtSearch_Pro_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void txtSearch_Cus_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void tabGeneral_Click(object sender, EventArgs e)
        {
            label32.Hide();
            button5.FlatStyle = FlatStyle.Standard;
            button5.FlatAppearance.BorderSize = 0;
        }
        //Tab báo cáo
        private void richTextBox1_Enter(object sender, EventArgs e)
        {
            if (richTextBox1.Text == "Typing.....")
            {
                richTextBox1.Text = "";
                richTextBox1.ForeColor = Color.Black;
            }
        }
        //btn tải lên file.
        private void button9_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }
        //Sự kiện đóng chương trình
        private void Home_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult rs = MessageBox.Show("Bạn muốn thoát à?", "Thật không", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (rs == DialogResult.OK)
            {
            }
            else
            {
                e.Cancel = true;
            }
                
        }
        //Khi click vào ngày của hóa đơn
        private void textBox18_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Kiểm tra", "Dữ liệu ngày của hóa đơn sẽ tự động được cập nhật cho hôm nay, Bạn thực sự muốn thay đổi?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
        //Báo cáo ,trợ giúp
        private void textBox32_Enter_1(object sender, EventArgs e)
        {
            if (textBox32.Text == "  Mô tả vấn đề của bạn")
            {
                textBox32.Text = "  ";
                textBox32.ForeColor = Color.Black;
            }
        }

        private void tbSell_MouseHover(object sender, EventArgs e)
        {
        }
        //==================================================================================
        //==================================================================================
        //==================================================================================
        //==================================================================================
        //==================================================================================
        //===== tab Bán Hàng
        private void LoadDataSaleGridView()
        {
            string sql;
            sql = "SELECT a.BIL_IDAuto, b.FRU_Name, a.BD_Amount, b.FRU_Price,a.BD_Sum FROM BillDetail AS a, Fruit AS b WHERE a.BIL_IDAuto = N'" + txtBil_id.Text + "' AND a.FRU_ID=b.FRU_ID";
            tblCTHDB = FunctionGeneral.GetDataToTable(sql);
            dgvSale.DataSource = tblCTHDB;
            dgvSale.AllowUserToAddRows = false;
            dgvSale.EditMode = DataGridViewEditMode.EditProgrammatically;
        }
        private void LoadInfoHoaDon()
        {
            string str;
            str = "SELECT BIL_Date FROM Bill WHERE BIL_IDAuto = N'" + txtBil_id.Text + "'";
            txtBil_date.Text = FunctionGeneral.ConvertDateTime(FunctionGeneral.GetFieldValues(str));
            str = "SELECT STA_ID FROM Staff WHERE BIL_IDAuto = N'" + txtBil_id.Text + "'";
            cbGender_Sta.Text = FunctionGeneral.GetFieldValues(str);
            //str = "SELECT CUS_ID FROM Customer WHERE BIL_ID = N'" + txtBil_id.Text + "'";
            //cboMaKhach.Text = FunctionGeneral.GetFieldValues(str);
            str = "SELECT BIL_Total FROM Bill WHERE BIL_IDAuto = N'" + txtBil_id.Text + "'";
            txtTongTien.Text = FunctionGeneral.GetFieldValues(str);
            //lblBangChu.Text = "Bằng chữ: " + FunctionGeneral.ChuyenSoSangChu(txtTongTien.Text);
        }
        //Sự kiện khi chọn mã nhân viên từ combox box.
        private void cbSta_id_TextChanged(object sender, EventArgs e)
        {
            string str = "SELECT STA_Name FROM Staff WHERE STA_ID =N'" + cbSta_id.SelectedValue + "'";
            txtSta_Name.Text = FunctionGeneral.GetFieldValues(str);
        }
        //Sự kiện khi chọn mã hàng hóa từ combox box.
        private void cbPro_id_TextChanged(object sender, EventArgs e)
        {
            string str = "SELECT FRU_Name FROM Fruit WHERE FRU_ID =N'" + cbPro_id.SelectedValue + "'";
            txtPro_Name.Text = FunctionGeneral.GetFieldValues(str);

            string str1 = "SELECT FRU_Price FROM Fruit WHERE FRU_ID =N'" + cbPro_id.SelectedValue + "'";
            txtDongia.Text = FunctionGeneral.GetFieldValues(str1);

            string str2 = "SELECT FRU_Unit FROM Fruit WHERE FRU_ID =N'" + cbPro_id.SelectedValue + "'";
            txtDonViTinh.Text = FunctionGeneral.GetFieldValues(str2);

            txtSoluong.Text = "";
        }
        //SK khi chọn mã khách từ cb

        //Sự kiện khi ấn nút thêm hóa đơn
        private void btnCreate_Sale_Click(object sender, EventArgs e)
        {
            //Ẩn  3 ntus Xóa Update In
            btnUpdate_Sale.Enabled = true;
            btnDelete_Sale.Enabled = true;
            btnPrint_Sale.Enabled = true;

            //Tự động gen ra ngày
            txtBil_date.Text = DateTime.Now.ToString("MM/dd/yyyy");

            //Khai báo mã hóa đơn là mã lớn nhất + 1 từ csdl hóa đơn
            int maxBIL_IDAuto;

            string sql = "select Max(BIL_IDAuto) as Max from Bill";
            SqlCommand cmd = new SqlCommand(sql, Class.FunctionGeneral.sqlCon);
            SqlDataReader reader = cmd.ExecuteReader();
            //FunctionGeneral.RunSQL(sql);    
            reader.Read();
            maxBIL_IDAuto = Convert.ToInt32(reader.GetValue(0)) + 1;
            txtBil_id.Text = maxBIL_IDAuto.ToString();
            reader.Close();

            LoadDataSaleGridView();
            txtTongTien.Text = "";
            txtGiamgia.Text = "";
            cbSta_id.SelectedIndex = 0;
            txtSta_Name.Text = "";
            cbCus_id.SelectedIndex = 0;
            txtCus_Name.Text = "";
            txtCus_Address.Text = "";
            txtCus_Phone.Text = "";
            cbPro_id.SelectedIndex = 0;
            txtPro_Name.Text = "";
            txtDongia.Text = "";
            txtDonViTinh.Text = "";
        }

        private void btnUpdate_Sale_Click(object sender, EventArgs e)
        {
            string sql;
            double sl, SLcon, tong, Tongmoi;
            //Kiểm tra Mã hóa đơn đã có chưa, nếu chưa có thì bắt đầu nhập
            sql = "SELECT BIL_IDAuto FROM Bill WHERE BIL_IDAuto=N'" + txtBil_id.Text + "'";
            if (!FunctionGeneral.CheckKey(sql))
            {
                if (txtBil_date.Text.Length == 0)
                {
                    MessageBox.Show("Bạn phải nhập ngày bán", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtBil_date.Focus();
                    return;
                }
                if (cbSta_id.Text.Length == 0)
                {
                    MessageBox.Show("Bạn phải nhập nhân viên", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (cbPro_id.Text.Length == 0)
                {
                    MessageBox.Show("Bạn phải nhập khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cbPro_id.Focus();
                    return;
                }

                //Kiểm tra khách hàng  cũ hay mới
                //Nếu là KH mới thì phải cập nhật thông tin KH này vào bảng Customer
                sql = "select CUS_ID from Customer where CUS_ID = N'" + txtCus_id.Text + "'";
                if (FunctionGeneral.CheckKey(sql))
                {
                }
                else
                {
                    //Cập nhật vào bảng Customer
                    sql = "INSERT INTO Customer VALUES(N'" +
                    txtCus_id.Text + "',N'" + txtCus_Name.Text + "',N'" + txtCus_Phone.Text + "',N'" + txtCus_Address.Text + "')";
                    Class.FunctionGeneral.RunSQL(sql); //Thực hiện câu lệnh sql
                }

                //Kiem tra xem ma Hàng da co trong hoa don đang tạo chưa?
                sql = "SELECT FRU_ID FROM BillDetail WHERE FRU_ID=N'" + cbPro_id.SelectedValue + "' AND BIL_IDAuto = N'" + txtBil_id.Text.Trim() + "'";
                if (FunctionGeneral.CheckKey(sql))
                {
                    MessageBox.Show("Mã hàng này đã có, bạn phải nhập mã khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cbPro_id.Focus();
                    return;
                }

                //Nếu chưa có thì tiến hành chèn mặt hàng đó vào hóa đơn
                sql = "INSERT INTO Bill(STA_ID, CUS_ID, BIL_Date,BIL_Discount, BIL_Total) VALUES (N'" +
                         cbSta_id.SelectedValue + "',N'" + txtCus_id.Text + "',N'" +
                        txtBil_date.Text.Trim() + "',N'" + txtGiamgia.Text + "',N'" + txtTongTien.Text + "')";
                Class.FunctionGeneral.RunSQL(sql);
                LoadDataSaleGridView();
            }
            //Kiểm tra các ô textbox đã nhập chưa
            if (cbPro_id.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập mã hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if ((txtSoluong.Text.Trim().Length == 0) || (txtSoluong.Text == "0"))
            {
                MessageBox.Show("Bạn phải nhập số lượng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSoluong.Text = "";
                txtSoluong.Focus();
                return;
            }
            
            //Kiểm tra mã hàng đã có chưa
            sql = "SELECT FRU_ID FROM BillDetail WHERE FRU_ID=N'" + cbPro_id.SelectedValue + "' AND BIL_IDAuto = N'" + txtBil_id.Text.Trim() + "'";
            if (FunctionGeneral.CheckKey(sql))
            {
                MessageBox.Show("Mã hàng này đã có, bạn phải nhập mã khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cbPro_id.Focus();
                return;
            }
            // Kiểm tra xem số lượng hàng trong kho còn đủ để cung cấp không?
            sl = Convert.ToDouble(FunctionGeneral.GetFieldValues("SELECT FRU_Amount FROM Fruit WHERE FRU_ID = N'" + cbPro_id.SelectedValue + "'"));
            if (Convert.ToDouble(txtSoluong.Text) > sl)
            {
                MessageBox.Show("Số lượng mặt hàng này chỉ còn " + sl, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSoluong.Text = "";
                txtSoluong.Focus();
                return;
            }
            //Nếu số lượng đủ, các dl khác ok thì tiến hàng thêm vào hóa đơn
            else { 
            sql = "INSERT INTO BillDetail(BIL_IDAuto,FRU_ID,BD_Amount,BD_Sum) VALUES(N'" + txtBil_id.Text.Trim() + "',N'" + cbPro_id.SelectedValue + "',N'" + txtSoluong.Text + "',N'" +txtThanhtien.Text + "')";
            Class.FunctionGeneral.RunSQL(sql);
            LoadDataSaleGridView();}

            // Cập nhật lại số lượng của mặt hàng vào bảng tblHang
            SLcon = sl - Convert.ToDouble(txtSoluong.Text);
            sql = "UPDATE Fruit SET FRU_Amount =" + SLcon + " WHERE FRU_ID= N'" + cbPro_id.SelectedValue + "'";
            Class.FunctionGeneral.RunSQL(sql);
            // Cập nhật lại tổng tiền cho hóa đơn bán
            tong = Convert.ToDouble(FunctionGeneral.GetFieldValues("SELECT BIL_Total FROM Bill WHERE BIL_IDAuto = N'" + txtBil_id.Text.Trim() + "'"));
            Tongmoi = tong + Convert.ToDouble(txtThanhtien.Text);
            sql = "UPDATE Bill SET BIL_Total =" + Tongmoi + " WHERE BIL_IDAuto = N'" + txtBil_id.Text.Trim() + "'";
            FunctionGeneral.RunSQL(sql);
            txtTongTien.Text = Tongmoi.ToString();
        }

        private void txtSoluong_TextChanged(object sender, EventArgs e)
        {
            if(txtSoluong.Text.Trim()== "-")
            {
                MessageBox.Show("Không đc thêm số lượng -");
                txtSoluong.Text =      "";
            }
            //Khi thay đổi số lượng thì thực hiện tính lại thành tiền
            double tt, sl, dg, gg;
            if (txtSoluong.Text == "")
                sl = 0;
            else
                sl = Convert.ToDouble(txtSoluong.Text);
            if (txtGiamgia.Text == "")
                gg = 0;
            else
                gg = Convert.ToDouble(txtGiamgia.Text);
            if (txtDongia.Text == "")
                dg = 0;
            else
                dg = Convert.ToDouble(txtDongia.Text);
            tt = sl * dg - sl * dg * gg / 100;
            txtThanhtien.Text = tt.ToString();
        }

        private void txtGiamgia_TextChanged(object sender, EventArgs e)
        {
            //Khi thay đổi giảm giá thì tính lại thành tiền
            double tt, gg, sum, mustPay;

            if (txtGiamgia.Text == "")
                gg = 0;
            else
                gg = Convert.ToDouble(txtGiamgia.Text.Trim());

            tt = Convert.ToDouble(txtTongTien.Text);
            sum = (tt * gg)/ 100;
            mustPay = tt - sum; 
            
            txtTongTien.Text = mustPay.ToString();
        }

        private void btnPrint_Sale_Click(object sender, EventArgs e)
        {
            // Khởi động chương trình Excel
            COMExcel.Application exApp = new COMExcel.Application();
            exApp.Visible = true;
            COMExcel.Workbook exBook; //Trong 1 chương trình Excel có nhiều Workbook
            COMExcel.Worksheet exSheet; //Trong 1 Workbook có nhiều Worksheet
            COMExcel.Range exRange;
            string sql;
            int hang = 0, cot = 0;
            DataTable tblThongtinHD, tblThongtinHang;
            exBook = exApp.Workbooks.Add(COMExcel.XlWBATemplate.xlWBATWorksheet);
            exSheet = exBook.Worksheets[1];
            // Định dạng chung
            exRange = exSheet.Cells[1, 1];
            exRange.Range["A1:Z300"].Font.Name = "Times new roman"; //Font chữ
            exRange.Range["A1:B5"].Font.Size = 10;
            exRange.Range["A1:B5"].Font.Bold = true;
            exRange.Range["A1:B5"].Font.ColorIndex = 3; //Màu xanh da trời
            exRange.Range["A1:A1"].ColumnWidth = 7;
            exRange.Range["B1:B1"].ColumnWidth = 15;

            exRange.Range["C2:E2"].Font.Bold = true;

            exRange.Range["A1:B1"].Font.Size = 14;
            exRange.Range["A1:B1"].MergeCells = true;
            exRange.Range["A1:B1"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["A1:B1"].Value = "KHAI'S STORE";


            exRange.Range["A2:B2"].MergeCells = true;
            exRange.Range["A2:B2"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["A2:B2"].Value = "CS1: Chợ Long Biên - Hà Nội";

            exRange.Range["A3:B3"].MergeCells = true;
            exRange.Range["A3:B3"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["A3:B3"].Value = "CS2: Chợ Đông Tảo - Hà Nội";

            exRange.Range["A4:B4"].MergeCells = true;
            exRange.Range["A4:B4"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["A4:B4"].Value = "SĐT: 0347063600";




            exRange.Range["B6:E9"].Borders.LineStyle = COMExcel.XlLineStyle.xlContinuous;
            exRange.Range["A11:F15"].Borders.LineStyle = COMExcel.XlLineStyle.xlContinuous;

            exRange.Range["F1:E1"].Font.Size = 14;
            exRange.Range["F1:E1"].Font.Bold = true;
            exRange.Range["F1:E1"].Font.ColorIndex = 3; //Màu đỏ
            exRange.Range["F1:E1"].MergeCells = true;
            exRange.Range["F1:E1"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignLeft;
            exRange.Range["F1:E1"].Value = "HÓA ĐƠN BÁN";

            //
            exRange.Range["F2:E2"].Font.Size = 12;
            exRange.Range["F2:E2"].Font.ColorIndex = 3; //Màu đỏ
            exRange.Range["F2:E2"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;

            exRange.Range["E2:E2"].Value = "Chuyên:";

            exRange.Range["F3:E3"].Font.Size = 12;
            exRange.Range["F3:E3"].Font.ColorIndex = 3; //Màu đỏ
            exRange.Range["F3:E3"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignLeft;
            exRange.Range["F3:E3"].MergeCells = true;
            exRange.Range["F3:E3"].Font.Bold = true;


            exRange.Range["F3:E3"].Value = "- Hoa quả nội & ngoại";

            exRange.Range["F4:E4"].Font.Size = 12;
            exRange.Range["F4:E4"].Font.ColorIndex = 3; //Màu đỏ
            exRange.Range["F4:E4"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignLeft;
            exRange.Range["F4:E4"].MergeCells = true;
            exRange.Range["F4:E4"].Font.Bold = true;
            

            exRange.Range["F4:E4"].Value = "- Cung cấp sỉ - lẻ";

            // Biểu Fiễn thông tin chung của hóa đơn bán
            sql = "SELECT a.BIL_IDAuto, a.BIL_Date, a.BIL_Total, b.CUS_Name, b.CUS_Address, b.CUS_Phone, c.STA_Name, a.BIL_Discount FROM Bill AS a, Customer AS b, Staff AS c WHERE a.BIL_IDAuto = N'" + txtBil_id.Text + "' AND a.CUS_ID = b.CUS_ID AND a.STA_ID = c.STA_ID";
            tblThongtinHD = Class.FunctionGeneral.GetDataToTable(sql);
            exRange.Range["B6:C9"].Font.Size = 12;
            exRange.Range["B6:B6"].Value = "Mã hóa đơn:";
            exRange.Range["C6:E6"].MergeCells = true;
            exRange.Range["C6:E6"].Value = tblThongtinHD.Rows[0][0].ToString();
            exRange.Range["C6:E6"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignRight;

            exRange.Range["B7:B7"].Value = "Khách hàng:";
            exRange.Range["C7:E7"].MergeCells = true;
            exRange.Range["C7:E7"].Value = tblThongtinHD.Rows[0][3].ToString();
            exRange.Range["C7:E7"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignRight;

            exRange.Range["B8:B8"].Value = "Địa chỉ:";
            exRange.Range["C8:E8"].MergeCells = true;
            exRange.Range["C8:E8"].Value = tblThongtinHD.Rows[0][4].ToString();
            exRange.Range["C8:E8"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignRight;

            exRange.Range["B9:B9"].Value = "Điện thoại:";
            exRange.Range["C9:E9"].MergeCells = true;
            exRange.Range["C9:E9"].Value = tblThongtinHD.Rows[0][5].ToString(); 
            exRange.Range["C9:E9"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignRight;

            //Lấy thông tin các mặt hàng
            sql = "SELECT b.FRU_Name, a.BD_Amount, b.FRU_Price, a.BD_Amount, a.BD_Sum " +
                  "FROM BillDetail AS a , Fruit AS b WHERE a.BIL_IDAuto = N'" +
                  txtBil_id.Text + "' AND a.FRU_ID = b.FRU_ID";
            tblThongtinHang = FunctionGeneral.GetDataToTable(sql);
            //Tạo dòng tiêu đề bảng
            exRange.Range["A11:F11"].Font.Bold = true;
            exRange.Range["A11:F11"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["C11:F11"].ColumnWidth = 12;
            exRange.Range["A11:A11"].Value = "STT";
            exRange.Range["B11:B11"].Value = "Tên hàng";
            exRange.Range["C11:C11"].Value = "Số lượng";
            exRange.Range["D11:D11"].Value = "Đơn giá";
            exRange.Range["E11:E11"].Value = "Giảm giá";
            exRange.Range["F11:F11"].Value = "Thành tiền";
            for (hang = 0; hang < tblThongtinHang.Rows.Count; hang++)
            {
                //Điền số thứ tự vào cột 1 từ dòng 12
                exSheet.Cells[1][hang + 12] = hang + 1;
                for (cot = 0; cot < tblThongtinHang.Columns.Count; cot++)
                //Điền thông tin hàng từ cột thứ 2, dòng 12
                {
                    exSheet.Cells[cot + 2][hang + 12] = tblThongtinHang.Rows[hang][cot].ToString();
                    //if (cot == 3) exSheet.Cells[cot + 2][hang + 12] = tblThongtinHang.Rows[hang][cot].ToString() + "%";
                    if (cot == 3) exSheet.Cells[cot + 2][hang + 12] = tblThongtinHD.Rows[0][7].ToString() + "%";
                }
            }
            exRange = exSheet.Cells[cot][hang + 14];
            exRange.Font.Bold = true;
            exRange.Value2 = "Tổng tiền:";
            exRange = exSheet.Cells[cot + 1][hang + 14];
            exRange.Font.Bold = true;
            exRange.Value2 = tblThongtinHD.Rows[0][2].ToString();
            exRange = exSheet.Cells[1][hang + 15]; //Ô A1 
            exRange.Range["A1:F1"].MergeCells = true;
            exRange.Range["A1:F1"].Font.Bold = true;
            exRange.Range["A1:F1"].Font.Italic = true;
            exRange.Range["A1:F1"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignRight;
            //exRange.Range["A1:F1"].Value = "Bằng chữ: " + FunctionGeneral.ChuyenSoSangChu(tblThongtinHD.Rows[0][2].ToString());
            exRange = exSheet.Cells[4][hang + 17]; //Ô A1 
            exRange.Range["A1:C1"].MergeCells = true;
            exRange.Range["A1:C1"].Font.Italic = true;
            exRange.Range["A1:C1"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            DateTime d = Convert.ToDateTime(tblThongtinHD.Rows[0][1]);
            exRange.Range["A1:C1"].Value = "Hà Nội, ngày " + d.Day + " tháng " + d.Month + " năm " + d.Year;
            exRange.Range["A2:C2"].MergeCells = true;
            exRange.Range["A2:C2"].Font.Italic = true;
            exRange.Range["A2:C2"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["A2:C2"].Value = "Nhân viên bán hàng";
            exRange.Range["A4:C4"].MergeCells = true;
            exRange.Range["A4:C4"].Font.Italic = true;
            exRange.Range["A4:C4"].HorizontalAlignment = COMExcel.XlHAlign.xlHAlignCenter;
            exRange.Range["A4:C4"].Value = tblThongtinHD.Rows[0][6];
            exSheet.Name = "Hóa đơn nhập";
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////
        // HỆ THỐNG
        private void btnDoiMatKhau_Click(object sender, EventArgs e)
        {
            panDoiMatKhau.Show();
        }

        private void txtXacNhanMatKhau_TextChanged(object sender, EventArgs e)
        {
            if (txtXacNhanMatKhau.Text.Trim() != txtMatKhauMoi.Text.Trim())
            {
                errorProvider1.SetError(txtXacNhanMatKhau, "Không trùng");
            }
            else
            {
                errorProvider1.Clear();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (txtMatKhauMoi.Text.Trim() == "" || txtXacNhanMatKhau.Text.Trim() == "")
            {
                MessageBox.Show("Bạn phải nhập đầy đủ thông tin");
            }
            else
            {
                string sql = "select ACC_Password from Account where ACC_Password =N'" + txtMkHienTai.Text.Trim() + "'";
                if (!FunctionGeneral.CheckKey(sql))
                {
                    MessageBox.Show("Mật khẩu hiện tại không chính xác");
                    txtMatKhauMoi.Text = txtXacNhanMatKhau.Text = txtMkHienTai.Text = "";

                }
                else
                {
                    if (txtMatKhauMoi.Text.Trim() != txtXacNhanMatKhau.Text.Trim())
                    {
                        MessageBox.Show("Xác nhận mật khẩu không trùng khớp");
                        txtMatKhauMoi.Text = txtXacNhanMatKhau.Text = txtMkHienTai.Text = "";
                    }

                    else
                    {
                        string query = "update Account set ACC_Password = N'" + txtMatKhauMoi.Text
                   + "' where ACC_Username= '" + userName + "'";
                        //Class.FunctionGeneral.RunSQL(query); //Thực hiện câu lệnh sql
                        SqlCommand cmd = new SqlCommand(query, Class.FunctionGeneral.sqlCon);
                        int kq = cmd.ExecuteNonQuery();
                        if (kq > 0)
                        {
                            DialogResult rs = MessageBox.Show("Đổi mật khẩu thành công! Bạn có muốn đăng xuất khỏi thiết bị?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (rs == DialogResult.Yes)
                            {
                                this.Hide();
                                LoginScreen login = new LoginScreen();
                                login.Show();
                            }
                            else if (rs == DialogResult.No)
                            {
                                txtMatKhauMoi.Text = txtXacNhanMatKhau.Text = txtMkHienTai.Text = "";
                            }
                        }
                    }
                }

            }
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show("Bạn có muốn đăng xuất?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (rs == DialogResult.Yes)
            {
                this.Hide();
                LoginScreen login = new LoginScreen();
                login.Show();
            }
            else if (rs == DialogResult.No)
            {

            }
        }

        private void btnDelete_Sale_Click(object sender, EventArgs e)
        {
            txtBil_id.Text = txtSta_Name.Text = txtBil_date.Text = txtCus_Address.Text = txtCus_id.Text = txtCus_Phone.Text = txtCus_Name.Text = txtPro_Name.Text = txtSoluong.Text = txtDongia.Text = txtDonViTinh.Text = "";
            cbPro_id.SelectedIndex = -1;
            cbSta_id.SelectedIndex = -1;
            txtTongTien.Text = "";
            a = 0;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox23.Show();
            button1.Show();
            button8.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dgvSale.DataSource = tblCTHDB;
            dgvSale.AllowUserToAddRows = false;
            dgvSale.EditMode = DataGridViewEditMode.EditProgrammatically;
            //string sql = "Select * from BillDetail where BIL_IDAuto = '" + textBox23.Text + "'";
            string sql = "SELECT a.BIL_IDAuto, b.FRU_Name, a.BD_Amount, b.FRU_Price,a.BD_Sum FROM BillDetail AS a, Fruit AS b WHERE a.BIL_IDAuto = N'" + textBox23.Text + "' AND a.FRU_ID=b.FRU_ID";

            dgvSale.DataSource = Class.FunctionGeneral.GetDataToTable(sql);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox23.Hide();
            button1.Hide();
        }

        private void txtCus_id_Click(object sender, EventArgs e)
        {
            
        }

        private void cbCus_id_TextChanged(object sender, EventArgs e)
        {
            txtCus_id.Text = cbCus_id.Text;
                string str = "SELECT CUS_Name FROM Customer WHERE CUS_ID =N'" + cbCus_id.SelectedValue + "'";
                txtCus_Name.Text = FunctionGeneral.GetFieldValues(str);

                string str1 = "SELECT CUS_Address FROM Customer WHERE CUS_ID =N'" + cbCus_id.SelectedValue + "'";
            txtCus_Address.Text = FunctionGeneral.GetFieldValues(str1);

                string str2 = "SELECT CUS_Phone FROM Customer WHERE CUS_ID =N'" + cbCus_id.SelectedValue + "'";
            txtCus_Phone.Text = FunctionGeneral.GetFieldValues(str2);

                txtSoluong.Text = "";
        }
        int a = 0; // khMoi
        //a = 1: kh cu
        private void cbCus_id_Click(object sender, EventArgs e)
        {
            if(a == 0)
            {
                DialogResult rs = MessageBox.Show("Khách hàng mới?", "Question?", MessageBoxButtons.YesNo);
                if (rs == DialogResult.Yes)
                {
                    cbCus_id.Hide();
                    txtCus_id.Show();
                }
                else
                {
                    a = 1;
                    cbCus_id.Show();
                    txtCus_id.Hide();
                }
            }
            else if (a == 1)
            {

                cbCus_id.Show();
                txtCus_id.Hide();
            }
            
        }

    }
}
