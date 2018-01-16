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
            getTag();
        }

        // 获取标签信息
        public void getTag()
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string strcmd = "select * from tag order by createTime desc";
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
            // 显示年月日时分秒
            dgvTag.Columns[4].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            con.Close();
        }

        // 添加
        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmAddTag frm = new frmAddTag();
            frm.Owner = this;
            frm.ShowDialog();
        }

        // 删除 
        private void btnDelete_Click(object sender, EventArgs e)
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            try
            {
                int n = dgvTag.SelectedRows.Count;//选中的行数
                if (n > 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        string id = dgvTag.SelectedRows[i].Cells[0].Value.ToString();
                        string strcmd = String.Format("delete from tag where id='{0}'", id);
                        MySqlCommand cmd = new MySqlCommand(strcmd, con);
                        int count = cmd.ExecuteNonQuery();
                    }
                    getTag();
                    MessageBox.Show("删除标签成功！", "删除成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("请选中要删除的标签！", "删除失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("请删除已经绑定标签的人员或车辆!", "操作数据库出错", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }

         // 批量导入
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
                            string tagNum = dr["标签卡号"].ToString();
                            string tagType = dr["标签类型"].ToString();
                            string tagState = "未绑定";
                            DateTime createTime = DateTime.Now;

                            // 标签卡号已添加，不再导入
                            string strcmd = String.Format("select * from tag where tagNum='{0}'", tagNum);
                            MySqlCommand cmd = new MySqlCommand(strcmd, con);
                            cmd.ExecuteNonQuery();
                            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            ada.Fill(ds);
                            DataTable dt1 = ds.Tables[0];
                            if (ds.Tables[0].Rows.Count <= 0)
                            {
                                string sql = String.Format("insert into tag (tagNum,tagType,tagState,createTime)" + "values('{0}','{1}','{2}','{3}')", tagNum, tagType, tagState, createTime);
                                MySqlCommand msd = new MySqlCommand(sql, con);
                                msd.ExecuteNonQuery();
                            }
                        }

                        getTag();
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

        // 批量导出
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
            for (int i = 0; i < dgvTag.ColumnCount; i++)
            {
                worksheet.Cells[1, i + 1] = dgvTag.Columns[i].HeaderText;
            }
            // 写入数值
            for (int r = 0; r < dgvTag.Rows.Count; r++)
            {
                for (int i = 0; i < dgvTag.ColumnCount; i++)
                {
                    worksheet.Cells[r + 2, i + 1] = dgvTag.Rows[r].Cells[i].Value;
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

        // 
        private void btnSelect_Click(object sender, EventArgs e)
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string tagNum = this.txtNum.Text;
            string strcmd;
            // 如果carTag为空，查询所有
            if (String.IsNullOrEmpty(tagNum))
            {
                strcmd = "select * from tag";
            }
            else
            {
                strcmd = String.Format("select * from tag where tagNum like '%{0}%'", tagNum);
            }
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            try
            {
                DataSet ds = new DataSet();
                ada.Fill(ds, "tag");                                           //查询结果填充数据集
                DataTable dt = ds.Tables["tag"];
                dgvTag.DataSource = dt;
                dgvTag.Columns[0].HeaderCell.Value = "序号";
                dgvTag.Columns[1].HeaderCell.Value = "标签卡号";
                dgvTag.Columns[2].HeaderCell.Value = "标签类型";
                dgvTag.Columns[3].HeaderCell.Value = "标签状态";
                dgvTag.Columns[4].HeaderCell.Value = "创建时间";
                // 显示年月日时分秒
                dgvTag.Columns[4].DefaultCellStyle.Format = "yyyy-MM-dd hh:mm:ss";
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
