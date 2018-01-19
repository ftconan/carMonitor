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
    public partial class frmAddTag : Form
    {
        public frmAddTag()
        {
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string tagNum = this.txtNum.Text;
            string tagType = this.cboTagType.Text;
            string tagState = "未绑定";
            DateTime createTime = DateTime.Now;

            // 用户必填字段判断
            if (String.IsNullOrEmpty(tagNum) || String.IsNullOrEmpty(tagType))
            {
                MessageBox.Show("请把所有字段填写完整！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 添加标签
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string strcmd = String.Format("insert into tag (tagNum,tagType,createTime)" + "values('{0}','{1}','{2}')", tagNum, tagType,createTime);
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            try
            {
                int count = cmd.ExecuteNonQuery();
                if (count > 0)
                {
                    MessageBox.Show("添加标签成功！", "添加成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    // 调用getTag刷新
                    ((frmTag)this.Owner).getTag();
                }
                else
                {
                    MessageBox.Show("添加标签失败！", "添加失败", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void frmAddTag_Load(object sender, EventArgs e)
        {
            this.cboTagType.Text = "人员标签";
        }
    }
}
