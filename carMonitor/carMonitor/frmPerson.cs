using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
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
                    //string str = "Server=localhost;User ID=root;Password=root;Database=car;Charset=utf8";
                    //MySqlConnection con = new MySqlConnection(str);                 //实例化链接
                    //con.Open();                                                     //开启连接
                    //string strcmd = "select * from [Sheet1$]";
                    //MySqlCommand cmd = new MySqlCommand(strcmd, con);
                    //MySqlDataAdapter ada = new MySqlDataAdapter(cmd);

                    ////// excel 数据填充到datagridview
                    //DataSet ds = new DataSet();
                    //ada.Fill(ds, "[Sheet1$]");                                           //查询结果填充数据集
                    //DataTable dt = ds.Tables["[Sheet1$]"];
                    //dgvPerson.DataSource = dt;

                    Console.WriteLine(filePath);
                    ExcelHelper excel_helper = new ExcelHelper(filePath);
                    DataTable dt = excel_helper.ExcelToDataTable("", true);

                    List<string> tableList = GetColumnsByDataTable(dt);
                    for (int i = 0; i < tableList.Count; i++)
                    {
                        foreach (DataRow item in dt.Rows)
                        {
                            try
                            {
                                if (item[0].ToString() != "")
                                {
                                    if (i < tableList.Count - 1 & item[i + 1].ToString() != "")
                                    {
                                        Console.WriteLine(item[0].ToString() + "\t" + item[i + 1].ToString());
                                    }
                                }
                            }
                            catch (Exception ex)
                            { }
                        }
                        Console.WriteLine("");
                    }
                    dgvPerson.DataSource = dt;
                  
                    // dataTable导入mysql
                    //#region
                    //try
                    //{
                    //    if (dgvPerson.Rows.Count > 0)
                    //    {
                    //        DataRow dr = null;
                    //        for (int i = 0; i < dt.Rows.Count; i++)
                    //        {
                    //            dr = dt.Rows[i];
                    //            string tagId = dr["tagId"].ToString();
                    //            string personName = dr["personName"].ToString();
                    //            string sex = dr["sex"].ToString();
                    //            string phoneNum = dr["phoneNum"].ToString();
                    //            string createTime = dr["createTime"].ToString();
                    //            string sql = String.Format("insert into user (userName,password,grade)" + "values('{0}','{1}','{2}','{3}','{4}','{5}')", tagId, personName, sex, phoneNum, createTime);
                    //            MySqlCommand msd = new MySqlCommand(sql, con);
                    //            msd.ExecuteNonQuery();
                    //        }
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show(ex.Message, "操作数据库出错", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //}
                    //finally
                    //{
                    //    con.Close();
                    //    con.Dispose();
                    //}
                    //#endregion
                }
            }
        }

        /// <summary>  
        /// 根据datatable获得列名  
        /// </summary>  
        /// <param name="dt">表对象</param>  
        /// <returns>返回结果的数据列数组</returns>  
        public static List<string> GetColumnsByDataTable(DataTable dt)
        {
            List<string> strColumns = new List<string>();

            if (dt.Columns.Count > 0)
            {
                int columnNum = 0;
                columnNum = dt.Columns.Count; ;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    strColumns.Add(dt.Columns[i].ColumnName);
                }
            }
            return strColumns;
        }
    }
}
