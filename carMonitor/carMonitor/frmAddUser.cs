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
    public partial class frmAddUser : Form
    {
        public frmAddUser()
        {
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string userName = this.txtName.Text;
            string password = this.txtPassword.Text;
            string grade = this.cboGrade.SelectedItem.ToString();

            // 添加用户
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string strcmd = String.Format("insert into user (userName,password,grade)" + "values('{0}','{1}','{2}')", userName, password, grade);
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            try
            {
                int count = cmd.ExecuteNonQuery();
                if (count > 0)
                {
                    MessageBox.Show("添加用户成功！", "添加成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    // 调用getUser刷新
                    ((frmSystem)this.Owner).getUser();
                }
                else
                {
                    MessageBox.Show("添加用户失败！", "添加失败", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "操作数据库出错", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
