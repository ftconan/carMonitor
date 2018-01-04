using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace carMonitor
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        public string userName;
        public string grade;

        private void txtExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car";
            MySqlConnection con = new MySqlConnection(str); //实例化链接
            con.Open();   //开启连接
            string strcmd = string.Format("select * from user where userName='{0}'", txtName.Text);
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            ada.Fill(ds); //查询结果填充数据集

            try
            {
                DataTable dt = ds.Tables[0];

                if (ds.Tables[0].Rows.Count > 0)
                {
                    userName = dt.Rows[0]["userName"].ToString();
                    string password = dt.Rows[0]["password"].ToString();
                    grade = dt.Rows[0]["grade"].ToString();
                    if (txtName.Text.Equals(userName) && txtPassword.Text.Equals(password))
                    {
                        MessageBox.Show("欢迎进入!", "登录成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MDIParent mdi = new MDIParent();
                        this.Hide();
                        // 传递grade
                        mdi.Owner = this;
                        mdi.Show();
                    }
                    else if (!txtPassword.Text.Equals(password))
                    {
                        MessageBox.Show("密码错误，请重新输入!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtPassword.Focus();
                        txtPassword.SelectAll();
                    }
                    else
                    {
                        MessageBox.Show("账号或密码错误，请重新输入!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("账号错误，请重新输入!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtName.Focus();
                    txtName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());

            }
            finally
            {
                con.Close();
                con.Dispose();
                ada.Dispose();
            }
        }
    }
}
