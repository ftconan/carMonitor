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
    public partial class frmAddPerson : Form
    {
        public frmAddPerson()
        {
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string personName = this.txtPersonName.Text;
            string sex = this.cboSex.SelectedItem.ToString();
            string phoneNum = this.txtPhoneNum.Text;
            DateTime createTime = DateTime.Now;

            // 添加人员
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string strcmd = String.Format("insert into person (personName,sex,phoneNum,createTime)" + "values('{0}','{1}','{2}','{3}')", personName, sex, phoneNum, createTime);
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            try
            {
                int count = cmd.ExecuteNonQuery();
                if (count > 0)
                {
                    MessageBox.Show("添加人员成功！", "添加成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    // 调用getPerson刷新
                    ((frmPerson)this.Owner).getPerson();
                }
                else
                {
                    MessageBox.Show("添加人员失败！", "添加失败", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void cboSex_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtPersonName_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
