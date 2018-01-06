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
        private RfidServer server = new RfidServer();
        private DataTable dt = new DataTable();
        private Dictionary<int, Temp> mDMsg = new Dictionary<int, Temp>();

        public frmMonitor()
        {
            InitializeComponent();
        }

        private void frmMonitor_Load(object sender, EventArgs e)
        {
            //getHistory();
            // 初始化dataGridView
            DataColumn col1 = new DataColumn("设备ID", typeof(string));
            DataColumn col2 = new DataColumn("激活ID", typeof(string));
            DataColumn col3 = new DataColumn("标签ID", typeof(string));
            DataColumn col4 = new DataColumn("激活Rssi", typeof(string));
            DataColumn col5 = new DataColumn("标签Rssi", typeof(string));
            DataColumn col6 = new DataColumn("状态", typeof(string));
            DataColumn col7 = new DataColumn("时间", typeof(string));
            DataColumn col8 = new DataColumn("次数", typeof(string));
            dt.Columns.Add(col1);
            dt.Columns.Add(col2);
            dt.Columns.Add(col3);
            dt.Columns.Add(col4);
            dt.Columns.Add(col5);
            dt.Columns.Add(col6);
            dt.Columns.Add(col7);
            dt.Columns.Add(col8);
            dgvMonitor.DataSource = dt.DefaultView;
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
            dgvMonitor.DataSource = dt;
            dgvMonitor.Columns[0].HeaderCell.Value = "序号";
            dgvMonitor.Columns[1].HeaderCell.Value = "人员标签号";
            dgvMonitor.Columns[2].HeaderCell.Value = "姓名";
            dgvMonitor.Columns[3].HeaderCell.Value = "车辆标签号";
            dgvMonitor.Columns[4].HeaderCell.Value = "车牌号";
            dgvMonitor.Columns[5].HeaderCell.Value = "创建时间";
            con.Close();
        }

        private void btnMonitor_Click(object sender, EventArgs e)
        {
            if (this.btnMonitor.Text == "开启监控")
            {
                server.StartServer(32500, 1000);
                if (server.GetState())
                {
                    this.btnMonitor.Text = "关闭监控";
                    this.timer1.Enabled = true;
                }
            }
            else
            {
                this.timer1.Enabled = false;
                server.StopServer();
                this.btnMonitor.Text = "开启监控";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //getHistory();

            // 处理TagMsg
            int nCount = server.GetMsgCount();
            while (nCount > 0)
            {
                TagMsg msg = server.GetMsg();
                if (mDMsg.ContainsKey(msg.TagId))
                {
                    mDMsg[msg.TagId].Tagmsg = msg;
                    mDMsg[msg.TagId].Count += 1;
                }

                else
                {
                    mDMsg[msg.TagId] = new Temp(msg);
                }
                nCount--;
            }
            this.Invoke((EventHandler)(delegate
            {

                dt.Rows.Clear();
                int COUNT = 0;

                foreach (var item in mDMsg)
                {
                    Temp temp = item.Value;
                    TagMsg msg = temp.Tagmsg;
                    DataRow row = dt.NewRow();
                    row[0] = msg.DeviceId;
                    row[1] = msg.ActiveId;
                    row[2] = msg.TagId;
                    row[3] = msg.TagRssi;
                    row[4] = msg.DeviceRssi;
                    row[5] = msg.State.ToString("X2");
                    //row[6] = msg.ReciveDt.ToString("HH:mm:ss");
                    //row[7] = temp.Count.ToString();
                    dt.Rows.Add(row);
                    COUNT++;
                }

                this.dgvMonitor.DataSource = dt.DefaultView;
            }));
        }
    }
}
