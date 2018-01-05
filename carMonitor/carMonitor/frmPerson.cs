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
    public partial class frmPerson : Form
    {
        public frmPerson()
        {
            InitializeComponent();
        }

        private void frmPerson_Load(object sender, EventArgs e)
        {
            getPerson();
        }

        // 获取人员信息
        public void getPerson()
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string strcmd = "select * from person";
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            ada.Fill(ds, "person");                                           //查询结果填充数据集
            DataTable dt = ds.Tables["person"];
            dgvPerson.DataSource = dt;
            dgvPerson.Columns[0].HeaderCell.Value = "序号";
            dgvPerson.Columns[1].HeaderCell.Value = "人员标签号";
            dgvPerson.Columns[2].HeaderCell.Value = "姓名";
            dgvPerson.Columns[3].HeaderCell.Value = "性别";
            dgvPerson.Columns[4].HeaderCell.Value = "电话号码";
            dgvPerson.Columns[5].HeaderCell.Value = "创建时间";
            con.Close();
        }
    }
}
