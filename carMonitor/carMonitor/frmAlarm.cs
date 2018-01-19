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
    public partial class frmAlarm : Form
    {
        public frmAlarm()
        {
            InitializeComponent();
        }

        //行号，规定其索引初始值为1
        private int num = 1;
        int pagesize = 20;
        int allCount = 0;
        int pagecount = 0;

        private void frmAlarm_Load(object sender, EventArgs e)
        {
            getAlarm();
        }

        // 获取报警信息
        public void getAlarm()
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string strcmd = "select * from alarm order by createTime desc";
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            ada.Fill(ds, "alarm");                                           //查询结果填充数据集
            DataTable dt = ds.Tables["alarm"];
            dgvAlarm.DataSource = dt;
            dgvAlarm.Columns[0].HeaderCell.Value = "序号";
            dgvAlarm.Columns[1].HeaderCell.Value = "车辆标签号";
            dgvAlarm.Columns[2].HeaderCell.Value = "车牌号";
            dgvAlarm.Columns[3].HeaderCell.Value = "报警类型";
            dgvAlarm.Columns[4].HeaderCell.Value = "报警内容";
            dgvAlarm.Columns[5].HeaderCell.Value = "创建时间";
            // 显示年月日时分秒
            dgvAlarm.Columns[5].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            con.Close();

            // 页数计算
            allCount = dt.Rows.Count;
            pagecount = allCount % pagesize;
            // 只能显示第一夜，判断是否整除
            if (pagecount == 0)
            {
                pagecount = allCount / pagesize;
            }
            else
            {
                pagecount = allCount / pagesize + 1;
            }

            this.labAllCount.Text = "共" + pagecount.ToString() + "页";
            // 分页显示函数
            num = 1;
            pageShow(1, 20);
        }

        // 查询
        private void btnSelect_Click(object sender, EventArgs e)
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string carNum = this.txtCardNum.Text;
            string startTime = this.dateTimePicker1.Text;
            string endTime = this.dateTimePicker2.Text;
            string strcmd;
            // 如果carTag为空，查询所有
            if (String.IsNullOrEmpty(carNum))
            {
                strcmd = "select * from alarm order by createTime desc";
            }
            else
            {
                strcmd = String.Format("select * from alarm where carNum like '%{0}%' and createTime between '{1}' and '{2}' order by createTime desc", carNum, startTime, endTime);
            }
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            try
            {
                DataSet ds = new DataSet();
                ada.Fill(ds, "alarm");                                           //查询结果填充数据集
                DataTable dt = ds.Tables["alarm"];
                dgvAlarm.DataSource = dt;
                dgvAlarm.Columns[0].HeaderCell.Value = "序号";
                dgvAlarm.Columns[1].HeaderCell.Value = "车辆标签号";
                dgvAlarm.Columns[2].HeaderCell.Value = "车牌号";
                dgvAlarm.Columns[3].HeaderCell.Value = "报警类型";
                dgvAlarm.Columns[4].HeaderCell.Value = "报警内容";
                dgvAlarm.Columns[5].HeaderCell.Value = "创建时间";
                // 显示年月日时分秒
                dgvAlarm.Columns[5].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";

                // 页数计算
                allCount = dt.Rows.Count;
                pagecount = allCount % pagesize;
                // 只能显示第一夜，判断是否整除
                if (pagecount == 0)
                {
                    pagecount = allCount / pagesize;
                }
                else
                {
                    pagecount = allCount / pagesize + 1;
                }

                this.labAllCount.Text = "共" + pagecount.ToString() + "页";
                // 分页显示函数
                num = 1;
                pageShow(1, 20);
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

        private void btnProtect_Click(object sender, EventArgs e)
        {
            frmProtect frm = new frmProtect();
            frm.Owner = this;
            frm.ShowDialog();
        }

        // 分页显示函数
        private void pageShow(int num, int pagecount)
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接

            // 条件查询和全部查询
            string carNum = this.txtCardNum.Text;
            string startTime = this.dateTimePicker1.Text;
            string endTime = this.dateTimePicker2.Text;
            string strcmd;
            // 如果carTag为空，查询所有
            if (!String.IsNullOrEmpty(carNum))
            {
                strcmd = String.Format("select * from alarm where carNum like '%{0}%' and createTime between '{1}' and '{2}' order by createTime desc limit " + (pagesize * (num - 1)) + ", " + pagesize + "", carNum, startTime, endTime);
            }
            else
            {
                strcmd = "select * from alarm order by createTime desc limit " + (pagesize * (num - 1)) + ", " + pagesize + "";
            }
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            ada.Fill(ds, "alarm");                                           //查询结果填充数据集
            DataTable dt = ds.Tables["alarm"];
            dgvAlarm.DataSource = dt;
            dgvAlarm.Columns[0].HeaderCell.Value = "序号";
            dgvAlarm.Columns[1].HeaderCell.Value = "车辆标签号";
            dgvAlarm.Columns[2].HeaderCell.Value = "车牌号";
            dgvAlarm.Columns[3].HeaderCell.Value = "报警类型";
            dgvAlarm.Columns[4].HeaderCell.Value = "报警内容";
            dgvAlarm.Columns[5].HeaderCell.Value = "创建时间";
            // 显示年月日时分秒
            dgvAlarm.Columns[5].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            con.Close();
            ds = null;

            // 更新页码
            this.labPageSize.Text = "当前第" + num.ToString() + "页";
        }

        // 第一页
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            num = 1;
            pageShow(num, pagesize);
        }

        // 最后一页
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            num = pagecount;
            if (num > 0)
            {
                pageShow(num, pagesize);
            }
            else
            {
                MessageBox.Show("现在最后一页记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // 上一页
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            num--;
            if (num > 0)
            {
                pageShow(num, pagesize);
            }
            else
            {
                MessageBox.Show("现在已经是第一页记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                num = 1;
                return;
            }
        }

        // 下一页
        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            num++;
            if (num <= pagecount)
            {
                pageShow(num, pagesize);
            }
            else
            {
                MessageBox.Show("现在已经是最后一页记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                num = pagecount;
                return;
            }
        }
    }
}
