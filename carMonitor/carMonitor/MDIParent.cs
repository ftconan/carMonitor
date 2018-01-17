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
using System.Threading;

namespace carMonitor
{
    public partial class MDIParent : Form
    {
        public MDIParent()
        {
            InitializeComponent();
            // 添加皮肤
            //string path = Environment.CurrentDirectory + "\\OneBlue.ssk";
            //this.skinEngine1.SkinFile = path;
            //skinEngine1.DisableTag = 9999;
            //skinEngine1.SkinAllForm = true;
        }

        public static int rate = 0;
        public static RfidServer server = new RfidServer();
        // 字典：键为车辆标签，值为人员数组
        public static Dictionary<string, List<string>> carPerson = new Dictionary<string, List<string>>();
        public static List<string> timeList = new List<string>();
        frmMonitor frm1;

        // 新建监控数据线程
        private Thread monitorThread;

        private void MDIParent_Load(object sender, EventArgs e)
        {
            // 移除tablelayoutpanel中的winform
            tableLayoutPanel2.Controls.Clear();
            frmMonitor frm1 = new frmMonitor();   //创建一个子窗体
            frm1.MdiParent = this;                 //子窗体在父窗体中显现
            frm1.Dock = DockStyle.Fill;
            frm1.FormBorderStyle = FormBorderStyle.None;
            tableLayoutPanel2.Controls.Add(frm1);
            frm1.Show();                           //子窗体显现

            // 权限判断
            string userName = ((frmLogin)this.Owner).userName;
            string grade = ((frmLogin)this.Owner).grade;

            // 普通用户没有管理功能
            if (grade == "普通用户")
            {
                this.pictureBox4.Hide();
                this.pictureBox5.Hide();
                this.pictureBox6.Hide();
                this.pictureBox7.Hide();
                this.pictureBox8.Hide();
                this.labTag.Hide();
                this.labPerson.Hide();
                this.labCar.Hide();
                this.labDevice.Hide();
                this.labSystem.Hide();
            }
        }

        // 实时监控
        private void labMonitor_Click(object sender, EventArgs e)
        {
            // 检查server是否关闭
            if (server.GetState())
            {
                server.StopServer();
            }
            server.StartServer(32500, 1000);

            tableLayoutPanel2.Controls.Clear();
            frm1 = new frmMonitor();               //创建一个子窗体
            frm1.MdiParent = this;                 //子窗体在父窗体中显现
            frm1.Dock = DockStyle.Fill;
            frm1.FormBorderStyle = FormBorderStyle.None;
            tableLayoutPanel2.Controls.Add(frm1);
            frm1.Show();                           //子窗体显现
        }

        // 报警管理
        private void labAlarm_Click(object sender, EventArgs e)
        {
            tableLayoutPanel2.Controls.Clear();
            frmAlarm frm2 = new frmAlarm();   //创建一个子窗体
            frm2.MdiParent = this;                 //子窗体在父窗体中显现
            frm2.Dock = DockStyle.Fill;
            frm2.FormBorderStyle = FormBorderStyle.None;
            tableLayoutPanel2.Controls.Add(frm2);
            frm2.Show();                           //子窗体显现
        }

        // 查询统计
        private void labCount_Click(object sender, EventArgs e)
        {
            tableLayoutPanel2.Controls.Clear();
            frmCount frm3 = new frmCount();   //创建一个子窗体
            frm3.MdiParent = this;            //子窗体在父窗体中显现
            frm3.Dock = DockStyle.Fill;
            frm3.FormBorderStyle = FormBorderStyle.None;
            tableLayoutPanel2.Controls.Add(frm3);
            frm3.Show();                           //子窗体显现
        }

        // 标签管理
        private void labTag_Click(object sender, EventArgs e)
        {
            tableLayoutPanel2.Controls.Clear();
            frmTag frm4 = new frmTag();   //创建一个子窗体
            frm4.MdiParent = this;                 //子窗体在父窗体中显现
            frm4.Dock = DockStyle.Fill;
            frm4.FormBorderStyle = FormBorderStyle.None;
            tableLayoutPanel2.Controls.Add(frm4);
            frm4.Show();                           //子窗体显现
        }

        // 人员管理
        private void labPerson_Click(object sender, EventArgs e)
        {
            tableLayoutPanel2.Controls.Clear();
            frmPerson frm5 = new frmPerson();   //创建一个子窗体
            frm5.MdiParent = this;                 //子窗体在父窗体中显现
            frm5.Dock = DockStyle.Fill;
            frm5.FormBorderStyle = FormBorderStyle.None;
            tableLayoutPanel2.Controls.Add(frm5);
            frm5.Show();                           //子窗体显现
        }

        // 车辆管理
        private void labCar_Click(object sender, EventArgs e)
        {
            tableLayoutPanel2.Controls.Clear();
            frmCar frm6 = new frmCar();   //创建一个子窗体
            frm6.MdiParent = this;                 //子窗体在父窗体中显现
            frm6.Dock = DockStyle.Fill;
            frm6.FormBorderStyle = FormBorderStyle.None;
            tableLayoutPanel2.Controls.Add(frm6);
            frm6.Show();                           //子窗体显现
        }

        // 基站管理
        private void labDevice_Click(object sender, EventArgs e)
        {
            tableLayoutPanel2.Controls.Clear();
            frmDevice frm7 = new frmDevice();   //创建一个子窗体
            frm7.MdiParent = this;                 //子窗体在父窗体中显现
            frm7.Dock = DockStyle.Fill;
            frm7.FormBorderStyle = FormBorderStyle.None;
            tableLayoutPanel2.Controls.Add(frm7);
            frm7.Show();                           //子窗体显现
        }

        // 系统管理
        private void labSystem_Click(object sender, EventArgs e)
        {
            tableLayoutPanel2.Controls.Clear();
            frmSystem frm8 = new frmSystem();   //创建一个子窗体
            frm8.MdiParent = this;                 //子窗体在父窗体中显现
            frm8.Dock = DockStyle.Fill;
            frm8.FormBorderStyle = FormBorderStyle.None;
            tableLayoutPanel2.Controls.Add(frm8);
            frm8.Show();                           //子窗体显现
        }

        private void btnMonitor_Click(object sender, EventArgs e)
        {
            if (this.btnMonitor.Text == "开启监控")
            {
                // 监控进程
                monitorThread = new Thread(startMonitor);
                monitorThread.Start();
                this.btnMonitor.Text = "关闭监控";
                // 判断用户是否设置采集频率
                if (!String.IsNullOrEmpty(this.txtRate.Text))
                {
                    rate = int.Parse(this.txtRate.Text) * 1000;
                    MessageBox.Show("采集频率设置成功！", "设置成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    rate = 6000;
                }
                tableLayoutPanel2.Controls.Clear();
                frm1 = new frmMonitor();               //创建一个子窗体
                frm1.MdiParent = this;                 //子窗体在父窗体中显现
                frm1.Dock = DockStyle.Fill;
                frm1.FormBorderStyle = FormBorderStyle.None;
                tableLayoutPanel2.Controls.Add(frm1);
                frm1.Show();                         //子窗体显现
            }
            else
            {
                rate = 0;
                //server.StopServer();
                monitorThread.Abort();
                monitorThread.Join();
                // 清空carPerson
                carPerson.Clear();
                timeList.Clear();

                this.btnMonitor.Text = "开启监控";

                tableLayoutPanel2.Controls.Clear();
                frm1 = new frmMonitor();               //创建一个子窗体
                frm1.MdiParent = this;                 //子窗体在父窗体中显现
                frm1.Dock = DockStyle.Fill;
                frm1.FormBorderStyle = FormBorderStyle.None;
                tableLayoutPanel2.Controls.Add(frm1);
                frm1.Show();                           //子窗体显现
            }
        }

        // 开启数据采集服务器
        public static void startMonitor()
        {
            server.StartServer(32500, 1000);
            if (server.GetState())
            {
                // electirccar 查出数据
                string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
                MySqlConnection con = new MySqlConnection(str);                 //实例化链接
                con.Open();                                                     //开启连接
                string strcmd = "select * from electriccar";
                MySqlCommand cmd = new MySqlCommand(strcmd, con);
                MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                ada.Fill(ds, "electriccar");                                           //查询结果填充数据集
                DataTable dt = ds.Tables["electriccar"];

                // 构建carPerson
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string carTag = dt.Rows[i]["tagId"].ToString();
                    string personTag = dt.Rows[i]["personId"].ToString();

                    // 车辆存在，追加personId
                    if (carPerson.ContainsKey(carTag))
                    {
                        carPerson[carTag].Add(personTag);
                    }
                    else
                    {
                        List<string> personList = new List<string>();
                        personList.Add(personTag);
                        carPerson[carTag] = personList;
                    }
                }

                // 查询布防时间
                string strcmd1 = "select * from protection order by createTime desc limit 2";
                MySqlCommand cmd1 = new MySqlCommand(strcmd1, con);
                MySqlDataAdapter ada1 = new MySqlDataAdapter(cmd1);
                DataSet ds1 = new DataSet();
                ada1.Fill(ds1, "protection");                                           //查询结果填充数据集
                DataTable dt1 = ds1.Tables["protection"];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    string startTime = dt1.Rows[i]["startTime"].ToString();
                    string endTime = dt1.Rows[i]["endTime"].ToString();

                    timeList.Add(startTime);
                    timeList.Add(endTime);
                }
                con.Close();
            }
        }
    }
}
