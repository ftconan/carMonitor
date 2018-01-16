using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

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

        public string personName;
        public string bindTag;

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
            // 显示年月日时分秒
            dgvPerson.Columns[5].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            con.Close();
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
            if(xlApp == null)
            {
                MessageBox.Show("无法创建Excel对象,您的电脑可能未安装Excel");
                return;
            }
            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];
            // 写入标题
            for (int i=0; i<dgvPerson.ColumnCount; i++)
            {
                worksheet.Cells[1, i + 1] = dgvPerson.Columns[i].HeaderText;
            }
            // 写入数值
            for(int r=0; r<dgvPerson.Rows.Count; r++)
            {
                for(int i=0; i < dgvPerson.ColumnCount; i++)
                {
                    worksheet.Cells[r+2,i+1] = dgvPerson.Rows[r].Cells[i].Value;
                }
                System.Windows.Forms.Application.DoEvents();
            }
            // 列宽自适应
            worksheet.Columns.EntireColumn.AutoFit();
            MessageBox.Show(fileName+"资料保存成功","提示",MessageBoxButtons.OK);
            if(saveFileName != "")
            {
                try
                {
                    workbook.Saved = true;
                    workbook.SaveCopyAs(saveFileName);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("导出文件时出错,文件可能正被打开！\n" + ex.Message);
                }
            }
            xlApp.Quit();
            // 强行销毁
            GC.Collect();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Excel 工作簿（*.xlsx）|*.xlsx|Excel 启动宏的工作簿（*.xlsm）|*.xlsm|Excel 97-2003工作簿（*.xls）|*.xls";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string filePath = dlg.FileName;
                
                // excel 路径判断
                if(filePath.Length == 0)
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
                            string personName = dr["姓名"].ToString();
                            string sex = dr["性别"].ToString();
                            string phoneNum = dr["电话号码"].ToString();
                            string tagId = dr["人员标签号"].ToString();
                            DateTime createTime = DateTime.Now;

                            // 人员姓名已添加，不再导入
                            string strcmd = String.Format("select * from person where personName='{0}'", personName);
                            MySqlCommand cmd = new MySqlCommand(strcmd, con);
                            cmd.ExecuteNonQuery();
                            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            ada.Fill(ds);
                            DataTable dt1 = ds.Tables[0];
                            if (ds.Tables[0].Rows.Count <= 0)
                            {
                                // 如果标签号不为空，修改标签状态为已绑定
                                if (!String.IsNullOrEmpty(tagId))
                                {
                                    string strcmd2 = String.Format("update tag set tagState='已绑定' where tagNum='{0}'", tagId);
                                    MySqlCommand cmd2 = new MySqlCommand(strcmd2, con);
                                    cmd2.ExecuteNonQuery();

                                    string sql = String.Format("insert into person (tagId,personName,sex,phoneNum,createTime)" + "values('{0}','{1}','{2}','{3}','{4}')", tagId, personName, sex, phoneNum, createTime);
                                    MySqlCommand msd = new MySqlCommand(sql, con);
                                    msd.ExecuteNonQuery();
                                }
                                else
                                {
                                    string sql = String.Format("insert into person (personName,sex,phoneNum,createTime)" + "values('{0}','{1}','{2}','{3}')",personName, sex, phoneNum, createTime);
                                    MySqlCommand msd = new MySqlCommand(sql, con);
                                    msd.ExecuteNonQuery();
                                }
                            }
                        }

                        getPerson();
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmAddPerson frm = new frmAddPerson();
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
                int n = dgvPerson.SelectedRows.Count;//选中的行数
                if (n > 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        // 如果用户被删除，把已经绑定的标签设成未绑定
                        bindTag = dgvPerson.SelectedRows[i].Cells[1].Value.ToString();
                        if (!String.IsNullOrEmpty(bindTag))
                        {
                            string strcmd2 = String.Format("update tag set tagState='未绑定' where tagNum='{0}'", bindTag);
                            MySqlCommand cmd2 = new MySqlCommand(strcmd2, con);
                            cmd2.ExecuteNonQuery();
                        }

                        string id = dgvPerson.SelectedRows[i].Cells[0].Value.ToString();
                        string strcmd = String.Format("delete from person where id='{0}'", id);
                        MySqlCommand cmd = new MySqlCommand(strcmd, con);
                        cmd.ExecuteNonQuery();
                    }
                    getPerson();
                    MessageBox.Show("删除人员成功！", "删除成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("请选中要删除的人员！", "删除失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            int n = dgvPerson.SelectedRows.Count;//选中的行数
            if (n > 0)
            {
                personName = dgvPerson.SelectedRows[0].Cells[2].Value.ToString();
                bindTag = dgvPerson.SelectedRows[0].Cells[1].Value.ToString();
                frmPersonTag frm = new frmPersonTag();
                frm.Owner = this;
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("请选中要绑定标签的人员！", "绑定失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
            MySqlConnection con = new MySqlConnection(str);                 //实例化链接
            con.Open();                                                     //开启连接
            string personName = this.txtPersonName.Text;
            string strcmd;
            // 如果personName为空，查询所有
            if (String.IsNullOrEmpty(personName))
            {
                strcmd = "select * from person";
            }
            else
            {
                strcmd = String.Format("select * from person where personName like '%{0}%'", personName);
            }
            MySqlCommand cmd = new MySqlCommand(strcmd, con);
            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            try
            {
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
                // 显示年月日时分秒
                dgvPerson.Columns[5].DefaultCellStyle.Format = "yyyy-MM-dd hh:mm:ss";
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
