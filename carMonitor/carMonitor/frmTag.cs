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
    public partial class frmTag : Form
    {
        public frmTag()
        {
            InitializeComponent();
        }

        private void frmTag_Load(object sender, EventArgs e)
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string strcmd = "select * from tag";
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            ada.Fill(ds, "tag");                                           //查询结果填充数据集
            DataTable dt = ds.Tables["tag"];
            dgvTag.DataSource = dt;
            dgvTag.Columns[0].HeaderCell.Value = "序号";
            dgvTag.Columns[1].HeaderCell.Value = "标签卡号";
            dgvTag.Columns[2].HeaderCell.Value = "标签类型";
            dgvTag.Columns[3].HeaderCell.Value = "标签状态";
            dgvTag.Columns[4].HeaderCell.Value = "创建时间";
            con.Close();
        }
    }
}
