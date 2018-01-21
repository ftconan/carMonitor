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
using RfidNet;


namespace carMonitor
{
    public partial class frmMonitor : Form
    {

        private static System.Windows.Forms.Timer timerRate = new System.Windows.Forms.Timer();
        // 数据库
        private MySqlConnection con;
        private MySqlCommand cmd;

        public MDIParent frm;
        public frmMonitor(MDIParent frm)
        {
            this.frm = frm;
            InitializeComponent();
        }

        private void frmMonitor_Load(object sender, EventArgs e)
        {
            getHistory();
            timerRate.Interval = 1000;
            timerRate.Tick += new EventHandler(timerRate_Tick);
            timerRate.Enabled = true;
        }

        private void timerRate_Tick(object sender, EventArgs e)
        {
           
            // 刷新列表
            if (frm.Rate == 0)
            {
                return;
            }
            else
            {
                if (timerRate.Interval != frm.Rate)
                {
                    if (frm.Rate >= 1000)
                    {
                        timerRate.Interval = frm.Rate;
                        timerRate.Enabled = false;
                        timerRate.Enabled = true;
                    }
                }
            }
            getHistory();
        }

        // 获取最新的50条历史信息，即实时数据
        public void getHistory()
        {
            DBConnection();
            string strcmd = "select * from history order by createTime desc limit 50";
            ExeCommand(strcmd);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            ada.Fill(ds, "history");                                           //查询结果填充数据集
            DataTable dt = ds.Tables["history"];
            dgvMonitor.DataSource = dt;
            dgvMonitor.Columns[0].HeaderCell.Value = "序号";
            dgvMonitor.Columns[1].HeaderCell.Value = "人员标签号";
            dgvMonitor.Columns[2].HeaderCell.Value = "姓名";
            dgvMonitor.Columns[3].HeaderCell.Value = "车辆标签号";
            dgvMonitor.Columns[4].HeaderCell.Value = "车牌号";
            dgvMonitor.Columns[5].HeaderCell.Value = "创建时间";
            // 显示年月日时分秒
            dgvMonitor.Columns[5].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
        }

        /// <summary>
        /// 初始化数据连接
        /// </summary>
        public void DBConnection()
        {
            CloseDB();
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                     //开启连接
        }

        // 关闭连接
        public void CloseDB()
        {
            if (con == null)
                return;
            try
            {
                con.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con = null;
            }
        }

        // 检查数据库连接
        public bool CheckConnection()
        {
            if (con == null)
            {
                return false;
            }

            if (con.State == ConnectionState.Open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // 新建command对象
        public void ExeCommand(string strcmd)
        {
            if (CheckConnection())
            {
                cmd = new MySqlCommand(strcmd, con);
            }
            else
            {
                DBConnection();
                if (CheckConnection())
                {
                    cmd = new MySqlCommand(strcmd, con);
                }
                else
                {
                    // 数据库连接不上
                    Console.WriteLine("数据库连接不上");
                }
            }
        }

    }
}
