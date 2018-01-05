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
    public partial class frmCount : Form
    {
        public frmCount()
        {
            InitializeComponent();
        }

        private void frmCount_Load(object sender, EventArgs e)
        {
            getHistory();
        }

        // 获取历史信息
        public void getHistory()
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string strcmd = "select * from history";
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            ada.Fill(ds, "history");                                           //查询结果填充数据集
            DataTable dt = ds.Tables["history"];
            dgvCount.DataSource = dt;
            dgvCount.Columns[0].HeaderCell.Value = "序号";
            dgvCount.Columns[1].HeaderCell.Value = "人员标签号";
            dgvCount.Columns[2].HeaderCell.Value = "姓名";
            dgvCount.Columns[3].HeaderCell.Value = "车辆标签号";
            dgvCount.Columns[4].HeaderCell.Value = "车牌号";
            dgvCount.Columns[5].HeaderCell.Value = "创建时间";
            con.Close();
        }
    }
}
