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

namespace carMonitor
{
    public partial class frmMonitor : Form
    {
        private DataTable dt = new DataTable();
        private Dictionary<int, Temp> mDMsg = new Dictionary<int, Temp>();
        private List<string> carTagList = new List<string>();
        private List<string> personTagList = new List<string>();
        private static System.Windows.Forms.Timer timerRate = new System.Windows.Forms.Timer();

        private int rate = MDIParent.rate;
        private RfidServer server = MDIParent.server;
        // 字典：键为车辆标签，值为人员数组
        private Dictionary<string, List<string>> carPerson = MDIParent.carPerson;
        private List<string> timeList = MDIParent.timeList;
        private bool alarmState;

        // 引入user32.dll
        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        public const int WM_CLOSE = 0x10;

        public frmMonitor()
        {
            InitializeComponent();
        }

        private void frmMonitor_Load(object sender, EventArgs e)
        {
            getHistory();
            // 判断rate是否为0，设置采集频率
            if (rate != 0)
            {
                timerRate.Interval = rate;
                timerRate.Tick += new EventHandler(timerRate_Tick);
                timerRate.Enabled = true;
            }
            else
            {
                timerRate.Enabled = false;

                // 清空mDMsg
                mDMsg.Clear();
                carTagList.Clear();
                personTagList.Clear();
                server.StopServer();
            }
        }

        private void timerRate_Tick(object sender, EventArgs e)
        {
            // 清空汽车、人员列表
            mDMsg.Clear();
            carTagList.Clear();
            personTagList.Clear();

            #region
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
            #endregion

            //#region
            foreach (var item in mDMsg)
            {
                Temp temp = item.Value;
                TagMsg msg = temp.Tagmsg;
                string tagId = msg.TagId.ToString();

                // 判断是否是车辆标签
                if (carPerson.Keys.Contains(tagId))
                {
                    carTagList.Add(tagId);
                }
                else
                {
                    personTagList.Add(tagId);
                }
            }
            //#endregion


            // 判断是否有车辆标签
            #region
            if (carTagList.Count > 0)
            {
                foreach (string key in carPerson.Keys)
                {
                    List<string> personList = carPerson[key];
                    alarmState = true;

                    // 遍历人员列表
                    #region
                    for (int i = 0; i < personList.Count; i++)
                    {
                        // 判断personTagList是否存在
                        #region
                        if (personTagList.Contains(personList[i]))
                        {
                            // alarmState
                            alarmState = false;

                            // 添加数据到历史记录
                            DateTime createTime = DateTime.Now;

                            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
                            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
                            con.Open();                                                     //开启连接
                            try
                            {
                                // 查询车辆信息
                                string strcmd1 = String.Format("select * from electriccar where tagId='{0}' and personId='{1}'", key, personList[i]);
                                MySqlCommand cmd1 = new MySqlCommand(strcmd1, con);
                                cmd1.ExecuteNonQuery();
                                MySqlDataAdapter ada = new MySqlDataAdapter(cmd1);
                                DataSet ds = new DataSet();
                                ada.Fill(ds);
                                DataTable dt = ds.Tables[0];
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    string carNum = dt.Rows[0]["carNum"].ToString();
                                    string personName = dt.Rows[0]["personName"].ToString();
                                    string strcmd = String.Format("insert into history (personTag,personName,carTag,carNum,createTime)" + "values('{0}','{1}','{2}','{3}','{4}')", personList[i], personName, key, carNum, createTime);
                                    MySqlCommand cmd = new MySqlCommand(strcmd, con);
                                    int count = cmd.ExecuteNonQuery();
                                    if (count > 0)
                                    {
                                        //MessageBox.Show("添加实时数据成功！", "添加成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        getHistory();
                                    }
                                    else
                                    {
                                        MessageBox.Show("添加实时数据失败！", "添加失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
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
                        #endregion
                    }
                    #endregion

                    // 报警判断，只要对应车辆有一个车主在，不报警
                    if (alarmState)
                    {
                        // 判断是否在布防时间内
                        string strNow = DateTime.Now.ToString("T");
                        DateTime now = Convert.ToDateTime(strNow);
                        if (timeList.Count == 2)
                        {
                            if (!(now >= Convert.ToDateTime(timeList[0]) && now <= Convert.ToDateTime(timeList[1])))
                            {
                                addAlarm(key);
                            }
                        }
                        else if (timeList.Count == 4)
                        {
                            if (!(((now >= Convert.ToDateTime(timeList[0]) && now <= Convert.ToDateTime(timeList[1])) || (now >= Convert.ToDateTime(timeList[2]) && now <= Convert.ToDateTime(timeList[3])))))
                            {
                                addAlarm(key);
                            }
                        }
                        else
                        {
                            addAlarm(key);
                        }
                    }
                }
            }
            #endregion

            // 刷新列表
            getHistory();
        }


        // 获取最新的50条历史信息，即实时数据
        public void getHistory()
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string strcmd = "select * from history order by createTime desc limit 50";
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
            // 显示年月日时分秒
            dgvMonitor.Columns[5].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            con.Close();
        }

        public void addAlarm(string tagId)
        {
            string alarmType = "警告";
            string alarmContext = "电瓶车可能被偷盗";
            DateTime createTime = DateTime.Now;

            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            try
            {
                // 查询车牌号
                string strcmd1 = String.Format("select * from electriccar where tagId='{0}'", tagId);
                MySqlCommand cmd1 = new MySqlCommand(strcmd1, con);
                cmd1.ExecuteNonQuery();
                MySqlDataAdapter ada = new MySqlDataAdapter(cmd1);
                DataSet ds = new DataSet();
                ada.Fill(ds);
                DataTable dt = ds.Tables[0];
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string carNum = dt.Rows[0]["carNum"].ToString();
                    string strcmd = String.Format("insert into alarm (carTag,carNum,alarmType,alarmContext,createTime)" + "values('{0}','{1}','{2}','{3}','{4}')", tagId, carNum, alarmType, alarmContext, createTime);
                    MySqlCommand cmd = new MySqlCommand(strcmd, con);
                    int count = cmd.ExecuteNonQuery();
                    if (count > 0)
                    {
                        // 2秒自动关闭
                        StartKiller();
                        MessageBox.Show(carNum + "电瓶车可能被偷盗!", "报警提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SpeechSynthesizer synth = new SpeechSynthesizer();
                        synth.Speak("请注意:" + carNum + "电瓶车可能被偷盗!");
                    }
                    else
                    {
                        MessageBox.Show("添加车辆报警失败！", "报警提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
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

        private void StartKiller()
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 2000; //2秒启动 
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            KillMessageBox();
            //停止Timer 
            ((System.Windows.Forms.Timer)sender).Stop();
        }
        private void KillMessageBox()
        {
            //按照MessageBox的标题，找到MessageBox的窗口 
            IntPtr ptr = FindWindow(null, "报警提示");
            if (ptr != IntPtr.Zero)
            {
                //找到则关闭MessageBox窗口 
                PostMessage(ptr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
        }
    }
}
