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
    public partial class frmMonitor : Form
    {
        public frmMonitor()
        {
            InitializeComponent();
        }

        private void frmMonitor_Load(object sender, EventArgs e)
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string strcmd = "select * from user";
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            ada.Fill(ds, "user");                                           //查询结果填充数据集
            DataTable dt = ds.Tables["user"];
            dgvMonitor.DataSource = dt;
            dgvMonitor.Columns[0].HeaderCell.Value = "序号";
            dgvMonitor.Columns[1].HeaderCell.Value = "用户名";
            dgvMonitor.Columns[2].HeaderCell.Value = "密码";
            dgvMonitor.Columns[3].HeaderCell.Value = "权限等级";
            con.Close();
        }
    }
}
