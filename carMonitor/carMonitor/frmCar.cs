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

        public string carNum;
        public string bindTag;

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
            // 显示年月日时分秒
            dgvCar.Columns[6].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            con.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmAddCar frm = new frmAddCar();
            frm.Owner = this;
            frm.ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            try
            {
                int n = dgvCar.SelectedRows.Count;//选中的行数
                if (n > 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        // 如果车辆被删除，把已经绑定的标签设成未绑定
                        bindTag = dgvCar.SelectedRows[i].Cells[1].Value.ToString();
                        if (!String.IsNullOrEmpty(bindTag))
                        {
                            string strcmd2 = String.Format("update tag set tagState='未绑定' where tagNum='{0}'", bindTag);
                            MySqlCommand cmd2 = new MySqlCommand(strcmd2, con);
                            cmd2.ExecuteNonQuery();
                        }

                        string id = dgvCar.SelectedRows[i].Cells[0].Value.ToString();
                        string strcmd = String.Format("delete from electriccar where id='{0}'", id);
                        MySqlCommand cmd = new MySqlCommand(strcmd, con);
                        cmd.ExecuteNonQuery();
                    }
                    getCar();
                    MessageBox.Show("删除车辆成功！", "删除成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("请选中要删除的车辆！", "删除失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnBindTag_Click(object sender, EventArgs e)
        {
            int n = dgvCar.SelectedRows.Count;//选中的行数
            if (n > 0)
            {
                carNum = dgvCar.SelectedRows[0].Cells[3].Value.ToString();
                bindTag = dgvCar.SelectedRows[0].Cells[1].Value.ToString();
                frmCarTag frm = new frmCarTag();
                frm.Owner = this;
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("请选中要绑定车主的车辆！", "绑定失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Excel 工作簿（*.xlsx）|*.xlsx|Excel 启动宏的工作簿（*.xlsm）|*.xlsm|Excel 97-2003工作簿（*.xls）|*.xls";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string filePath = dlg.FileName;

                // excel 路径判断
                if (filePath.Length == 0)
                {
                    MessageBox.Show("请选择导入数据的Execl文件");
                }
                else
                {
                    // 添加人员
                    string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
                    MySqlConnection con = new MySqlConnection(str);                 //实例化链接
                    con.Open();                                                     //开启连接

                    // 打开excel,将excel数据导入datatable
                    ExcelHelper excel_helper = new ExcelHelper(filePath);
                    DataTable dt = excel_helper.ExcelToDataTable("", true);

                    // dataTable导入mysql
                    #region
                    try
                    {
                        DataRow dr = null;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dr = dt.Rows[i];
                            string carName = dr["车辆名称"].ToString();
                            string carNum = dr["车牌号"].ToString();
                            string personName = dr["人员姓名"].ToString();
                            string tagId = dr["车辆标签号"].ToString();
                            DateTime createTime = DateTime.Now;

                            // 车辆已添加，不再导入
                            string strcmd = String.Format("select * from electriccar where carNum='{0}'", carNum);
                            MySqlCommand cmd = new MySqlCommand(strcmd, con);
                            cmd.ExecuteNonQuery();
                            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            ada.Fill(ds);
                            DataTable dt1 = ds.Tables[0];
                            if (ds.Tables[0].Rows.Count <= 0)
                            {
                                // 如果标签号不为空，修改标签状态为已绑定
                                // 查询人员标签
                                string strcmd1 = String.Format("select * from person where personName='{0}'", personName);
                                MySqlCommand cmd1 = new MySqlCommand(strcmd1, con);
                                cmd1.ExecuteNonQuery();
                                MySqlDataAdapter ada1 = new MySqlDataAdapter(cmd1);
                                DataSet ds1 = new DataSet();
                                ada1.Fill(ds1);
                                DataTable dt2 = ds1.Tables[0];
                                string personId = dt2.Rows[0]["tagId"].ToString();

                                if (!String.IsNullOrEmpty(tagId))
                                {
                                    string strcmd2 = String.Format("update tag set tagState='已绑定' where tagNum='{0}'", tagId);
                                    MySqlCommand cmd2 = new MySqlCommand(strcmd2, con);
                                    cmd2.ExecuteNonQuery();

                                    string sql = String.Format("insert into electriccar (tagId,carName,carNum,personName,personId,createTime)" + "values('{0}','{1}','{2}','{3}','{4}','{5}')", tagId, carName, carNum, personName, personId, createTime);
                                    MySqlCommand msd = new MySqlCommand(sql, con);
                                    msd.ExecuteNonQuery();
                                }
                                else
                                {
                                    string sql = String.Format("insert into electriccar (carName,carNum,personName,personId,createTime)" + "values('{0}','{1}','{2}','{3}','{4}')", carName, carNum, personName, personId, createTime);
                                    MySqlCommand msd = new MySqlCommand(sql, con);
                                    msd.ExecuteNonQuery();
                                }
                            }
                        }

                        getCar();
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
                    #endregion
                }
            }
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            string fileName = "";
            string saveFileName = "";
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xls|xlsx";
            saveDialog.Filter = "Excel 工作簿（*.xlsx）|*.xlsx|Excel 启动宏的工作簿（*.xlsm）|*.xlsm|Excel 97-2003工作簿（*.xls）|*.xls";
            saveDialog.FileName = fileName;
            saveDialog.ShowDialog();
            saveFileName = saveDialog.FileName;
            // 点击取消
            if (saveFileName.IndexOf(":") < 0)
            {
                return;
            }
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                MessageBox.Show("无法创建Excel对象,您的电脑可能未安装Excel");
                return;
            }
            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];
            // 写入标题
            for (int i = 0; i < dgvCar.ColumnCount; i++)
            {
                worksheet.Cells[1, i + 1] = dgvCar.Columns[i].HeaderText;
            }
            // 写入数值
            for (int r = 0; r < dgvCar.Rows.Count; r++)
            {
                for (int i = 0; i < dgvCar.ColumnCount; i++)
                {
                    worksheet.Cells[r + 2, i + 1] = dgvCar.Rows[r].Cells[i].Value;
                }
                System.Windows.Forms.Application.DoEvents();
            }
            // 列宽自适应
            worksheet.Columns.EntireColumn.AutoFit();
            MessageBox.Show(fileName + "资料保存成功", "提示", MessageBoxButtons.OK);
            if (saveFileName != "")
            {
                try
                {
                    workbook.Saved = true;
                    workbook.SaveCopyAs(saveFileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("导出文件时出错,文件可能正被打开！\n" + ex.Message);
                }
            }
            xlApp.Quit();
            // 强行销毁
            GC.Collect();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string carNum = this.txtCarNum.Text;
            string strcmd;
            // 如果carNum为空，查询所有
            if (String.IsNullOrEmpty(carNum))
            {
                strcmd = "select * from electriccar";
            }
            else
            {
                strcmd = String.Format("select * from electriccar where carNum like '%{0}%'", carNum);
            }
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            try
            {
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
    }
}
