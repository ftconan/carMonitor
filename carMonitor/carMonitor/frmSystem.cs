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
    public partial class frmSystem : Form
    {
        public frmSystem()
        {
            InitializeComponent();
        }

        private void frmSystem_Load(object sender, EventArgs e)
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
            dgvSystem.DataSource = dt;
            dgvSystem.Columns[0].HeaderCell.Value = "序号";
            dgvSystem.Columns[1].HeaderCell.Value = "用户名";
            dgvSystem.Columns[2].HeaderCell.Value = "密码";
            dgvSystem.Columns[3].HeaderCell.Value = "权限等级";
            con.Close();
        }
    }
}
