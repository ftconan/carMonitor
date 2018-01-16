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
    public partial class frmCount : Form
    {
        public frmCount()
        {
            InitializeComponent();
        }

        //行号，规定其索引初始值为1
        private int num = 1;  
        int pagesize = 20;
        int allCount = 0;
        int pagecount = 0;

        private void frmCount_Load(object sender, EventArgs e)
        {
            getHistory();
        }

        // 获取历史信息
        public void getHistory()
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string strcmd = "select * from history order by createTime desc";
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            ada.Fill(ds, "history");                                           //查询结果填充数据集
            DataTable dt = ds.Tables["history"];
            dgvCount.DataSource = dt;
            dgvCount.Columns[0].HeaderCell.Value = "序号";
            dgvCount.Columns[1].HeaderCell.Value = "人员标签号";
            dgvCount.Columns[2].HeaderCell.Value = "姓名";
            dgvCount.Columns[3].HeaderCell.Value = "车辆标签号";
            dgvCount.Columns[4].HeaderCell.Value = "车牌号";
            dgvCount.Columns[5].HeaderCell.Value = "创建时间";
            // 显示年月日时分秒
            dgvCount.Columns[5].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            con.Close();

            // 页数计算
            allCount = dt.Rows.Count;
            pagecount = allCount % pagesize;
            // 只能显示第一夜，判断是否整除
            if(pagecount == 0)
            {
                pagecount = allCount / pagesize;
            }
            else
            {
                pagecount = allCount / pagesize + 1;
            }

            this.labAllCount.Text = "共" + pagecount.ToString() + "页";
            // 分页显示函数
            pageShow(1,10);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();
            string carNum = this.txtCount.Text;
            string startTime = this.dateTimePicker1.Text;
            string endTime = this.dateTimePicker2.Text;
            string strcmd;
            if (radioCar.Checked)
            {
                // 如果carTag为空，查询所有
                if (String.IsNullOrEmpty(carNum))
                {
                    strcmd = "select * from history";
                }
                else
                {
                    strcmd = String.Format("select * from history where carNum like '%{0}%' and createTime between '{1}' and '{2}'", carNum, startTime, endTime);
                }
            }
            else
            {
                // 如果carTag为空，查询所有
                if (String.IsNullOrEmpty(carNum))
                {
                    strcmd = "select * from history";
                }
                else
                {
                    strcmd = String.Format("select * from history where personName like '%{0}%' and createTime between '{1}' and '{2}'", carNum, startTime, endTime);
                }
            }
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            try
            {
                DataSet ds = new DataSet();
                ada.Fill(ds, "history");                                           //查询结果填充数据集
                DataTable dt = ds.Tables["history"];
                dgvCount.DataSource = dt;
                dgvCount.Columns[0].HeaderCell.Value = "序号";
                dgvCount.Columns[1].HeaderCell.Value = "人员标签号";
                dgvCount.Columns[2].HeaderCell.Value = "姓名";
                dgvCount.Columns[3].HeaderCell.Value = "车辆标签号";
                dgvCount.Columns[4].HeaderCell.Value = "车牌号";
                dgvCount.Columns[5].HeaderCell.Value = "创建时间";
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

        // 分页显示函数
        private void pageShow(int start, int end)
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string strcmd = "select * from history order by createTime desc limit " + (pagesize * (num - 1))+", "+pagesize+"";
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            ada.Fill(ds, "history");                                           //查询结果填充数据集
            DataTable dt = ds.Tables["history"];
            dgvCount.DataSource = dt;
            dgvCount.Columns[0].HeaderCell.Value = "序号";
            dgvCount.Columns[1].HeaderCell.Value = "人员标签号";
            dgvCount.Columns[2].HeaderCell.Value = "姓名";
            dgvCount.Columns[3].HeaderCell.Value = "车辆标签号";
            dgvCount.Columns[4].HeaderCell.Value = "车牌号";
            dgvCount.Columns[5].HeaderCell.Value = "创建时间";
            // 显示年月日时分秒
            dgvCount.Columns[5].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
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
            pageShow(num - 1, pagesize);
        }

        // 上一页
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            num--;
            if(num > 0)
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
