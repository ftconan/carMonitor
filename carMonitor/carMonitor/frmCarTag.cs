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
    public partial class frmCarTag : Form
    {
        public frmCarTag()
        {
            InitializeComponent();
        }

        List<string> cardNum = new List<string>();
        List<string> newCardNum = new List<string>();

        private void frmCarTag_Load(object sender, EventArgs e)
        {
            string carNum = ((frmCar)this.Owner).carNum;
            this.labelCardNum.Text = carNum;

            // 动态添加标签卡号
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string strcmd = "select tagNum from tag where tagType='车辆标签' and tagState='未绑定'";
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            try
            {
                DataSet ds = new DataSet();
                ada.Fill(ds, "tag");                                           //查询结果填充数据集
                DataTable dt = ds.Tables["tag"];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cardNum.Add(dt.Rows[i]["tagNum"].ToString());
                }
                // 添加标签卡号到组合框
                this.cboTag.Items.AddRange(cardNum.ToArray());
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
            string carNum = this.labelCardNum.Text;
            string tagId;
            // 判断cboTag是否为空
            if (!String.IsNullOrEmpty(this.cboTag.Text))
            {
                tagId = this.cboTag.Text;
            }
            else
            {
                tagId = this.cboTag.SelectedItem.ToString();
            }

            // 绑定标签
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接

            string strcmd = String.Format("update electriccar set tagId='{0}' where carNum='{1}'", tagId, carNum);
            MySqlCommand cmd = new MySqlCommand(strcmd, con);

            // 修改标签状态，改成已绑定
            string strcmd1 = String.Format("update tag set tagState='已绑定' where tagNum='{0}'", tagId);
            MySqlCommand cmd1 = new MySqlCommand(strcmd1, con);
            try
            {
                int count = cmd.ExecuteNonQuery();
                if (count > 0)
                {
                    MessageBox.Show("绑定标签成功！", "绑定成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    // 调用getCar刷新
                    ((frmCar)this.Owner).getCar();
                }
                else
                {
                    MessageBox.Show("绑定标签失败！", "绑定失败", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                cmd1.ExecuteNonQuery();

                // 如果车辆已经绑定过标签，把已经绑定的标签设成未绑定
                string bindTag = ((frmCar)this.Owner).bindTag;
                if (!String.IsNullOrEmpty(bindTag))
                {
                    string strcmd2 = String.Format("update tag set tagState='未绑定' where tagNum='{0}'", bindTag);
                    MySqlCommand cmd2 = new MySqlCommand(strcmd2, con);
                    cmd2.ExecuteNonQuery();
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

        // comboBox模糊查询
        private void cboTag_TextUpdate(object sender, EventArgs e)
        {
            // 清空combobox
            this.cboTag.Items.Clear();
            // 清空newCardNum
            newCardNum.Clear();
            foreach (var item in cardNum)
            {
                if (item.Contains(this.cboTag.Text))
                {
                    newCardNum.Add(item);
                }
            }
            // combobox添加已经查到的关键词
            this.cboTag.Items.AddRange(newCardNum.ToArray());
            // 设置光标位置，否则光标位置始终保持在第一列，造成输入关键词的倒序排列
            this.cboTag.SelectionStart = this.cboTag.Text.Length;
            // 保持鼠标指针形状，有时候鼠标指针会被覆盖，所以要进行一次设置
            Cursor = Cursors.Default;
            // 自动弹出下拉框
            this.cboTag.DroppedDown = true;
        }
    }
}
