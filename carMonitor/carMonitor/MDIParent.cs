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
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using Quartz;
using Quartz.Impl;

namespace carMonitor
{
    public partial class MDIParent : Form
    {
        // 字典：键为车辆标签，值为人员数组
        public Dictionary<string, List<string>> carPerson = new Dictionary<string, List<string>>();
        public Dictionary<string, string> tagNames = new Dictionary<string, string>();
        public List<string> timeList = new List<string>();
        // 数据库
        private MySqlConnection con;
        private MySqlCommand cmd;
        private DataTable dt = new DataTable();


        public Thread threadPro;
        public bool stopThread = false;
        Dictionary<string, Form> childForms = new Dictionary<string, Form>();
        Form currentChildForm = null;
        private frmMonitor frm1;
        private frmAlarm frm2;
        private frmCount frm3;
        private frmTag frm4;
        private frmPerson frm5;
        private frmCar frm6;
        private frmDevice frm7;
        private frmSystem frm8;
        // 移除消息时间
        private double removeTime;

        public MDIParent()
        {
            InitializeComponent();
        }

        public int Rate = 0;
        public RfidServer server = new RfidServer();

        private void MDIParent_Load(object sender, EventArgs e)
        {
            this.StateCheck.Enabled = false;

            // 移除tablelayoutpanel中的winform
            frm1 = new frmMonitor(this);           //创建一个子窗体
            frm1.MdiParent = this;                 //子窗体在父窗体中显现
            frm1.Dock = DockStyle.Fill;
            frm1.FormBorderStyle = FormBorderStyle.None;
            childForms.Add("monitor", frm1);
            tableLayoutPanel2.Controls.Add(childForms["monitor"]);
            currentChildForm = childForms["monitor"];
            currentChildForm.Show();

            frm2 = new frmAlarm();               //创建一个子窗体
            frm2.MdiParent = this;                 //子窗体在父窗体中显现
            frm2.Dock = DockStyle.Fill;
            frm2.FormBorderStyle = FormBorderStyle.None;
            frm2.Hide();
            childForms.Add("alarm", frm2);

            frm3 = new frmCount();               //创建一个子窗体
            frm3.MdiParent = this;                 //子窗体在父窗体中显现
            frm3.Dock = DockStyle.Fill;
            frm3.FormBorderStyle = FormBorderStyle.None;
            frm3.Hide();
            childForms.Add("count", frm3);

            frm4 = new frmTag();               //创建一个子窗体
            frm4.MdiParent = this;                 //子窗体在父窗体中显现
            frm4.Dock = DockStyle.Fill;
            frm4.FormBorderStyle = FormBorderStyle.None;
            frm4.Hide();
            childForms.Add("tag", frm4);

            frm5 = new frmPerson();               //创建一个子窗体
            frm5.MdiParent = this;                 //子窗体在父窗体中显现
            frm5.Dock = DockStyle.Fill;
            frm5.FormBorderStyle = FormBorderStyle.None;
            childForms.Add("person", frm5);

            frm6 = new frmCar();               //创建一个子窗体
            frm6.MdiParent = this;                 //子窗体在父窗体中显现
            frm6.Dock = DockStyle.Fill;
            frm6.FormBorderStyle = FormBorderStyle.None;
            childForms.Add("car", frm6);

            frm7 = new frmDevice();               //创建一个子窗体
            frm7.MdiParent = this;                 //子窗体在父窗体中显现
            frm7.Dock = DockStyle.Fill;
            frm7.FormBorderStyle = FormBorderStyle.None;
            childForms.Add("device", frm7);

            frm8 = new frmSystem();        //创建一个子窗体
            frm8.MdiParent = this;                 //子窗体在父窗体中显现
            frm8.Dock = DockStyle.Fill;
            frm8.FormBorderStyle = FormBorderStyle.None;
            childForms.Add("system", frm8);


            // 权限判断
            //string userName = ((frmLogin)this.Owner).userName;
            //string grade = ((frmLogin)this.Owner).grade;

            // 普通用户没有管理功能
            //if (grade == "普通用户")
            //{
            //    this.pictureBox4.Hide();
            //    this.pictureBox5.Hide();
            //    this.pictureBox6.Hide();
            //    this.pictureBox7.Hide();
            //    this.pictureBox8.Hide();
            //    this.labTag.Hide();
            //    this.labPerson.Hide();
            //    this.labCar.Hide();
            //    this.labDevice.Hide();
            //    this.labSystem.Hide();
            //}

            // 删除七天前的数据
            delHistory();

        }

        // 实时监控
        private void labMonitor_Click(object sender, EventArgs e)
        {
            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel2.Controls.Add(childForms["monitor"]);
            currentChildForm = childForms["monitor"];
            currentChildForm.Show();
            frm1.getHistory();
        }

        // 报警管理
        private void labAlarm_Click(object sender, EventArgs e)
        {
            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel2.Controls.Add(childForms["alarm"]);
            childForms["alarm"].Show();
            frm2.getAlarm();
        }

        // 查询统计
        private void labCount_Click(object sender, EventArgs e)
        {
            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel2.Controls.Add(childForms["count"]);
            childForms["count"].Show();
            frm3.getHistory();
        }

        // 标签管理
        private void labTag_Click(object sender, EventArgs e)
        {
            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel2.Controls.Add(childForms["tag"]);
            childForms["tag"].Show();                      //子窗体显现
            frm4.getTag();
        }

        // 人员管理
        private void labPerson_Click(object sender, EventArgs e)
        {
            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel2.Controls.Add(childForms["person"]);
            childForms["person"].Show();
            frm5.getPerson();
        }

        // 车辆管理
        private void labCar_Click(object sender, EventArgs e)
        {
            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel2.Controls.Add(childForms["car"]);
            childForms["car"].Show();
            frm6.getCar();
        }

        // 基站管理
        private void labDevice_Click(object sender, EventArgs e)
        {
            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel2.Controls.Add(childForms["device"]);
            childForms["device"].Show();
            frm7.getDevice();
        }

        // 系统管理
        private void labSystem_Click(object sender, EventArgs e)
        {
            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel2.Controls.Add(childForms["system"]);
            childForms["system"].Show();                      //子窗体显现
            frm8.getUser();
        }

        private void btnMonitor_Click(object sender, EventArgs e)
        {
            if (this.btnMonitor.Text == "开启监控")
            {
                this.btnMonitor.Text = "关闭监控";
                // 判断用户是否设置采集频率
                if (!String.IsNullOrEmpty(this.txtRate.Text))
                {
                    Rate = int.Parse(this.txtRate.Text) * 1000;
                    removeTime = Rate * 0.001 * 1.5;
                    MessageBox.Show("采集频率设置成功！", "设置成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Rate = 6000;
                    removeTime = Rate * 0.001 * 1.5;
                }
                // 监控进程
                stopThread = false;
                LoadFromDB();
                if (threadPro != null)
                {
                    try
                    {
                        threadPro.Abort();
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        threadPro = null;
                    }
                   
                }
                threadPro = new Thread(new ThreadStart(ThreadPro));
                threadPro.Start();
                StateCheck.Interval = Rate;
                StateCheck.Enabled = true;
            }
            else
            {
                StateCheck.Enabled = false;
                Rate = 0;
                stopThread = true;
                Thread.Sleep(200);
                try
                {
                    threadPro.Abort();
                } catch (Exception ex)
                {

                }
                finally
                {
                    threadPro = null;
                }
                if (server.GetState())
                {
                    server.StopServer();
                }
                this.btnMonitor.Text = "开启监控";
            }
        }

        private object listLock = new object();
        private List<TagMsg> mDMsg = new List<TagMsg>();

        // 接收数据
        public void ThreadPro()
        {
            server.StartServer(32500, 1000);
            Thread.Sleep(100);
            if (server.GetState())
            {
                while (true)
                {
                    if (stopThread)
                    {
                        server.StopServer();
                        break;
                    }
                    int nCount = server.GetMsgCount();
                    if (nCount > 0)
                    {
                       TagMsg msg = server.GetMsg();
                        if (msg != null)
                        {
                            lock (listLock)
                            {
                                mDMsg.Add(msg);
                            }
                        }
                        continue;
                    }

                    Thread.Sleep(1000);
                }
            }
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

        /// 从数据库重新加载数据
        public void LoadFromDB()
        {
            // electirccar 查出数据
            carPerson.Clear();
            timeList.Clear();
            DBConnection();
            string strcmd = "select * from electriccar";
            ExeCommand(strcmd);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            ada.Fill(ds, "electriccar");                                           //查询结果填充数据集
            DataTable dt = ds.Tables["electriccar"];

            // 构建carPerson
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string carTag = dt.Rows[i]["tagId"].ToString();
                tagNames[carTag] = dt.Rows[i]["carNum"].ToString();
                string personTag = dt.Rows[i]["personId"].ToString();
                tagNames[personTag] = dt.Rows[i]["personName"].ToString();
              

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
            ExeCommand(strcmd1);
            MySqlDataAdapter ada1 = new MySqlDataAdapter(cmd);
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
        }

        // 新建command对象
        public void ExeCommand(string strcmd)
        {
            if (CheckConnection())
            {
                cmd = new MySqlCommand(strcmd, con);
                if (cmd.ExecuteNonQuery() < 0)
                {

                }
            }
            else
            {
                DBConnection();
                if (CheckConnection())
                {
                    cmd = new MySqlCommand(strcmd, con);
                    if (cmd.ExecuteNonQuery() < 0)
                    {

                    }
                }
                else
                {
                    // 数据库连接不上
                    Console.WriteLine("数据库连接不上");
                }
            }
        }

      
        private void StateCheck_Tick(object sender, EventArgs e)
        {
            lock (listLock)
            {
                // 移除6s前mDMsg
                for (int i = 0; i < mDMsg.Count; i++)
                {
                    TimeSpan ts = DateTime.Now - mDMsg[i].ReciveDt;
                    if (ts.TotalSeconds > removeTime)
                    {
                        mDMsg.RemoveAt(i);
                        i--;
                    }
                }

                // carTags keys
                Dictionary<string, List<string>> carTags = new Dictionary<string, List<string>>();
                for (int i = 0; i < mDMsg.Count; i++)
                {
                    if (carPerson.Keys.Contains(mDMsg[i].TagId.ToString()))
                    {
                        if (!carTags.ContainsKey(mDMsg[i].TagId.ToString()))
                            carTags.Add(mDMsg[i].TagId.ToString(), new List<string>());
                    }
                }

                // 添加人员标签到carTags
                List<string> test = new List<string>(carTags.Keys);
                for (int i = 0; i < test.Count; i++)
                {
                    for (int ii = 0; ii < mDMsg.Count; ii++)
                    {
                        if (carPerson[test[i]].Contains(mDMsg[ii].TagId.ToString()))
                        {
                            if (!carTags[test[i]].Contains(mDMsg[ii].TagId.ToString()))
                            {
                                carTags[test[i]].Add(mDMsg[ii].TagId.ToString());
                            }
                        }
                    }
                }

                    
                for (int i = 0; i < test.Count; i ++)
                {
                    
                   
                    if (carTags[test[i]].Count == 0)
                    {
                        // 判断是否在布防时间内
                        string strNow = DateTime.Now.ToString("T");
                        DateTime now = Convert.ToDateTime(strNow);
                        #region
                        if (timeList.Count == 2)
                        {
                            if (!(now >= Convert.ToDateTime(timeList[0]) && now <= Convert.ToDateTime(timeList[1])))
                            {
                                string strcmd = String.Format("insert into alarm (carTag,carNum,alarmType,alarmContext,createTime)" +
    "values('{0}','{1}','{2}','{3}','{4}')", test[i], tagNames[test[i]], "警告", "电瓶车可能被偷盗", DateTime.Now);
                                ExeCommand(strcmd);

                                SpeechSynthesizer synth = new SpeechSynthesizer();
                                synth.Speak("请注意:" + tagNames[test[i]] + "电瓶车可能被偷盗!");
                            }
                        }
                        else if (timeList.Count == 4)
                        {
                            if (!(((now >= Convert.ToDateTime(timeList[0]) && now <= Convert.ToDateTime(timeList[1])) || (now >= Convert.ToDateTime(timeList[2]) && now <= Convert.ToDateTime(timeList[3])))))
                            {
                                string strcmd = String.Format("insert into alarm (carTag,carNum,alarmType,alarmContext,createTime)" +
    "values('{0}','{1}','{2}','{3}','{4}')", test[i], tagNames[test[i]], "警告", "电瓶车可能被偷盗", DateTime.Now);
                                ExeCommand(strcmd);

                                SpeechSynthesizer synth = new SpeechSynthesizer();
                                synth.Speak("请注意:" + tagNames[test[i]] + "电瓶车可能被偷盗!");
                            }
                        }
                        else
                        {
                            string strcmd = String.Format("insert into alarm (carTag,carNum,alarmType,alarmContext,createTime)" +
    "values('{0}','{1}','{2}','{3}','{4}')", test[i], tagNames[test[i]], "警告", "电瓶车可能被偷盗", DateTime.Now);
                            ExeCommand(strcmd);

                            SpeechSynthesizer synth = new SpeechSynthesizer();
                            synth.Speak("请注意:" + tagNames[test[i]] + "电瓶车可能被偷盗!");
                        }
                        #endregion
                    }
                    else
                    {
                        foreach (var personId in carTags[test[i]])
                        {
                            // che ren duo de 
                            string strcmd = String.Format("insert into history (personTag,personName,carTag,carNum,createTime)" +
                                "values('{0}','{1}','{2}','{3}','{4}')", personId, tagNames[personId], test[i], tagNames[test[i]], DateTime.Now);
                            ExeCommand(strcmd);
                        }
                    }
                }
            }
        }
        protected override void WndProc(ref Message m)
        {
            //Console.WriteLine(m.Msg);  
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;
            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
            {
                //捕捉关闭窗体消息        
                //用户点击关闭窗体控制按钮   注释为最小化窗体     
                //this.WindowState = FormWindowState.Minimized;  

                //窗体隐藏  

                System.Environment.Exit(System.Environment.ExitCode);
            }
            base.WndProc(ref m);
        }

        // 删除作业
        public class JobInterest : IJob
        {
            public Task Execute(IJobExecutionContext context)
            {
                return Task.Run(() =>
                {
                    string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
                    MySqlConnection con = new MySqlConnection(str);                 //实例化链接
                    con.Open();                                     //开启连接
                    string strcmd = "delete from history where to_days(now())-to_days(createTime)>7";
                    MySqlCommand cmd = new MySqlCommand(strcmd, con);
                    int count = cmd.ExecuteNonQuery();
                    Console.WriteLine(count);
                });
            }
        }

        // 定时任务
        public void delHistory()
        {
            var scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();
            //创建触发条件
            ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("InterestTrigger", "Interest")
            .StartNow()
            .WithDailyTimeIntervalSchedule(t =>
            {
                t.OnEveryDay();//每天都执行
                t.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(20, 15));//设置执行的开始时间
                t.WithRepeatCount(0);//设置总共执行次数
            })
            .Build();
            //创建作业内容
            IJobDetail job = JobBuilder.Create(typeof(JobInterest))
           .WithIdentity("InterestJob", "Interest")
           .Build();
            scheduler.ScheduleJob(job, trigger);
            scheduler.Start();
        }
    }
}
