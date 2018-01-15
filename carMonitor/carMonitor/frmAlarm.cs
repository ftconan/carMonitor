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
            string strcmd = "select * from alarm";
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
                strcmd = "select * from alarm";
            }
            else
            {
                strcmd = String.Format("select * from alarm where carNum like '%{0}%' and createTime between '{1}' and '{2}'", carNum, startTime, endTime);
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
    }
}
