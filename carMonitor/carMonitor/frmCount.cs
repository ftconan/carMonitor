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
            string strcmd = "select * from history";
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
    }
}
