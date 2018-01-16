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
    public partial class frmProtect : Form
    {
        public frmProtect()
        {
            InitializeComponent();
        }

        private void frmProtect_Load(object sender, EventArgs e)
        {
            getProtect();
        }

        // 布防
        private void btnProtect_Click(object sender, EventArgs e)
        {
            string protectName = this.txtRate.Text;
            string startTime = this.dateTimePicker1.Text;
            string endTime = this.dateTimePicker2.Text;
            DateTime createTime = DateTime.Now;

            // 添加布防
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string strcmd = String.Format("insert into protection (protectName,startTime,endTime,createTime)" + "values('{0}','{1}','{2}','{3}')", protectName, startTime, endTime, createTime);
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            try
            {
                int count = cmd.ExecuteNonQuery();
                if (count > 0)
                {
                    MessageBox.Show("添加布防成功！", "添加成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // 调用getProtect刷新
                    getProtect();
                }
                else
                {
                    MessageBox.Show("添加布防失败！", "添加失败", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        // 撤防
        private void btnUnProtect_Click(object sender, EventArgs e)
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            try
            {
                int n = dgvProtect.SelectedRows.Count;//选中的行数
                if (n > 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        string id = dgvProtect.SelectedRows[i].Cells[0].Value.ToString();
                        string strcmd = String.Format("delete from protection where id='{0}'", id);
                        MySqlCommand cmd = new MySqlCommand(strcmd, con);
                        int count = cmd.ExecuteNonQuery();
                    }
                    getProtect();
                    MessageBox.Show("删除布防成功！", "删除成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("请选中要删除的布防！", "删除失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // 获取布防信息
        public void getProtect()
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string strcmd = "select * from protection order by createTime desc";
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            ada.Fill(ds, "protection");                                           //查询结果填充数据集
            DataTable dt = ds.Tables["protection"];
            dgvProtect.DataSource = dt;
            dgvProtect.Columns[0].HeaderCell.Value = "序号";
            dgvProtect.Columns[1].HeaderCell.Value = "布防名称";
            dgvProtect.Columns[2].HeaderCell.Value = "开始时间";
            dgvProtect.Columns[3].HeaderCell.Value = "结束时间";
            dgvProtect.Columns[4].HeaderCell.Value = "创建时间";
            // 显示年月日时分秒
            dgvProtect.Columns[4].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            con.Close();
        }
    }
}
