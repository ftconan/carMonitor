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
    public partial class frmCar : Form
    {
        public frmCar()
        {
            InitializeComponent();
        }

        private void frmCar_Load(object sender, EventArgs e)
        {
            getCar();
        }

        // 获取车辆信息
        public void getCar()
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string strcmd = "select * from electriccar";
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            ada.Fill(ds, "electriccar");                                           //查询结果填充数据集
            DataTable dt = ds.Tables["electriccar"];
            dgvCar.DataSource = dt;
            dgvCar.Columns[0].HeaderCell.Value = "序号";
            dgvCar.Columns[1].HeaderCell.Value = "车辆标签号";
            dgvCar.Columns[2].HeaderCell.Value = "车辆名称";
            dgvCar.Columns[3].HeaderCell.Value = "车牌号";
            dgvCar.Columns[4].HeaderCell.Value = "人员姓名";
            dgvCar.Columns[5].HeaderCell.Value = "人员标签号";
            dgvCar.Columns[6].HeaderCell.Value = "创建时间";
            con.Close();
        }
    }
}
