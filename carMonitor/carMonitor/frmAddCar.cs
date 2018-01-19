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
    public partial class frmAddCar : Form
    {
        public frmAddCar()
        {
            InitializeComponent();
        }

        List<string> person = new List<string>();
        List<string> newPerson = new List<string>();

        private void frmAddCar_Load(object sender, EventArgs e)
        {
            // 动态添加人员姓名
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            // 查出没有和车辆绑定的人员
            string strcmd = "select personName from person where personName not in (select personName from electriccar)";
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            try
            {
                DataSet ds = new DataSet();
                ada.Fill(ds, "person");                                           //查询结果填充数据集
                DataTable dt = ds.Tables["person"];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    person.Add(dt.Rows[i]["personName"].ToString());
                }
                // 添加人员姓名到组合框
                this.cboPersonName.Items.AddRange(person.ToArray());
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

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string carName = this.txtCarName.Text;
            string cardNum = this.txtCarNum.Text;
            string personName = this.cboPersonName.Text;
            DateTime createTime = DateTime.Now;

            // 用户必填字段判断
            if(String.IsNullOrEmpty(carName) || String.IsNullOrEmpty(cardNum) || String.IsNullOrEmpty(personName))
            {
                MessageBox.Show("请把所有字段填写完整！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 添加人员
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            try
            {
                // 查询人员标签
                string strcmd1 = String.Format("select tagId from person where personName='{0}'", personName);
                MySqlCommand cmd1 = new MySqlCommand(strcmd1, con);
                cmd1.ExecuteNonQuery();
                MySqlDataAdapter ada = new MySqlDataAdapter(cmd1);
                DataSet ds = new DataSet();
                ada.Fill(ds);
                DataTable dt = ds.Tables[0];
                if(ds.Tables[0].Rows.Count > 0){
                    string personId = dt.Rows[0]["tagId"].ToString();
                    string strcmd = String.Format("insert into electriccar (carName,carNum,personName,personId,createTime)" + "values('{0}','{1}','{2}','{3}','{4}')", carName, cardNum, personName, personId, createTime);
                    MySqlCommand cmd = new MySqlCommand(strcmd, con);
                    int count = cmd.ExecuteNonQuery();
                    if (count > 0)
                    {
                        MessageBox.Show("添加车辆成功！", "添加成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                        // 调用getCar刷新
                        ((frmCar)this.Owner).getCar();
                    }
                    else
                    {
                        MessageBox.Show("添加车辆失败！", "添加失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // combobox模糊查询
        private void cboPersonName_TextUpdate(object sender, EventArgs e)
        {
            // 清空combobox
            this.cboPersonName.Items.Clear();
            // 清空newCardNum
            newPerson.Clear();
            foreach (var item in person)
            {
                if (item.Contains(this.cboPersonName.Text))
                {
                    newPerson.Add(item);
                }
            }
            // combobox添加已经查到的关键词
            this.cboPersonName.Items.AddRange(newPerson.ToArray());
            // 设置光标位置，否则光标位置始终保持在第一列，造成输入关键词的倒序排列
            this.cboPersonName.SelectionStart = this.cboPersonName.Text.Length;
            // 保持鼠标指针形状，有时候鼠标指针会被覆盖，所以要进行一次设置
            Cursor = Cursors.Default;
            // 自动弹出下拉框
            this.cboPersonName.DroppedDown = true;
        }
    }
}
