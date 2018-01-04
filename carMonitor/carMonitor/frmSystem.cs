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
    public partial class frmSystem : Form
    {
        public frmSystem()
        {
            InitializeComponent();
        }

        private void frmSystem_Load(object sender, EventArgs e)
        {
            getUser();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmAddUser frm = new frmAddUser();
            frm.Show();
        }

        // 删除用户
        private void btnDelete_Click(object sender, EventArgs e)
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            try
            {
                int n = dgvSystem.SelectedRows.Count;//选中的行数
                if(n > 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        string id = dgvSystem.SelectedRows[i].Cells[0].Value.ToString();
                        string strcmd = String.Format("delete from user where id='{0}'", id);
                        MySqlCommand cmd = new MySqlCommand(strcmd, con);
                        int count = cmd.ExecuteNonQuery();
                    }
                    getUser();
                    MessageBox.Show("删除用户成功！", "删除成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("请选中要删除的用户！", "删除失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // 获取用户信息
        public void getUser()
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string strcmd = "select * from user";
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            ada.Fill(ds, "user");                                           //查询结果填充数据集
            DataTable dt = ds.Tables["user"];
            dgvSystem.DataSource = dt;
            dgvSystem.Columns[0].HeaderCell.Value = "序号";
            dgvSystem.Columns[1].HeaderCell.Value = "用户名";
            dgvSystem.Columns[2].HeaderCell.Value = "密码";
            dgvSystem.Columns[3].HeaderCell.Value = "权限等级";
            con.Close();
        }
    }
}
