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
    public partial class frmDevice : Form
    {
        public frmDevice()
        {
            InitializeComponent();
        }

        private void frmDevice_Load(object sender, EventArgs e)
        {
            getDevice();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmAddDevice frm = new frmAddDevice();
            frm.Owner = this;
            frm.ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            try
            {
                int n = dgvDevice.SelectedRows.Count;//选中的行数
                if (n > 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        string id = dgvDevice.SelectedRows[i].Cells[0].Value.ToString();
                        string strcmd = String.Format("delete from device where id='{0}'", id);
                        MySqlCommand cmd = new MySqlCommand(strcmd, con);
                        int count = cmd.ExecuteNonQuery();
                    }
                    getDevice();
                    MessageBox.Show("删除基站成功！", "删除成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("请选中要删除的基站！", "删除失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // 获取设备信息
        public void getDevice()
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string strcmd = "select * from device order by createTime desc";
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            ada.Fill(ds, "device");                                         //查询结果填充数据集
            DataTable dt = ds.Tables["device"];
            dgvDevice.DataSource = dt;
            dgvDevice.Columns[0].HeaderCell.Value = "序号";
            dgvDevice.Columns[1].HeaderCell.Value = "基站名称";
            dgvDevice.Columns[2].HeaderCell.Value = "设备编号";
            dgvDevice.Columns[3].HeaderCell.Value = "创建时间";
            // 显示年月日时分秒
            dgvDevice.Columns[3].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            con.Close();
        }
    }
}
