namespace carMonitor
{
    partial class frmAddPerson
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cboSex = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPersonName = new System.Windows.Forms.TextBox();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPhoneNum = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cboSex
            // 
            this.cboSex.Font = new System.Drawing.Font("SimSun", 12F);
            this.cboSex.FormattingEnabled = true;
            this.cboSex.Items.AddRange(new object[] {
            "男",
            "女"});
            this.cboSex.Location = new System.Drawing.Point(294, 188);
            this.cboSex.Name = "cboSex";
            this.cboSex.Size = new System.Drawing.Size(170, 28);
            this.cboSex.TabIndex = 24;
            this.cboSex.SelectedIndexChanged += new System.EventHandler(this.cboSex_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("SimSun", 14F);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(104, 188);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 24);
            this.label3.TabIndex = 23;
            this.label3.Text = "性别";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // txtPersonName
            // 
            this.txtPersonName.Font = new System.Drawing.Font("SimSun", 12F);
            this.txtPersonName.Location = new System.Drawing.Point(294, 95);
            this.txtPersonName.Name = "txtPersonName";
            this.txtPersonName.Size = new System.Drawing.Size(170, 30);
            this.txtPersonName.TabIndex = 21;
            this.txtPersonName.TextChanged += new System.EventHandler(this.txtPersonName_TextChanged);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.Font = new System.Drawing.Font("SimSun", 12F);
            this.btnConfirm.Location = new System.Drawing.Point(108, 358);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(110, 46);
            this.btnConfirm.TabIndex = 20;
            this.btnConfirm.Text = "确定";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnExit
            // 
            this.btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExit.Font = new System.Drawing.Font("SimSun", 12F);
            this.btnExit.Location = new System.Drawing.Point(294, 358);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(106, 46);
            this.btnExit.TabIndex = 18;
            this.btnExit.Text = "取消";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("SimSun", 14F);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(104, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 24);
            this.label1.TabIndex = 17;
            this.label1.Text = "人员姓名";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtPhoneNum
            // 
            this.txtPhoneNum.Font = new System.Drawing.Font("SimSun", 12F);
            this.txtPhoneNum.Location = new System.Drawing.Point(294, 281);
            this.txtPhoneNum.Name = "txtPhoneNum";
            this.txtPhoneNum.Size = new System.Drawing.Size(170, 30);
            this.txtPhoneNum.TabIndex = 25;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("SimSun", 14F);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(104, 287);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 24);
            this.label2.TabIndex = 26;
            this.label2.Text = "电话号码";
            // 
            // frmAddPerson
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(566, 496);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPhoneNum);
            this.Controls.Add(this.cboSex);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtPersonName);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.label1);
            this.Name = "frmAddPerson";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加人员";
            this.Load += new System.EventHandler(this.frmAddPerson_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboSex;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPersonName;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPhoneNum;
        private System.Windows.Forms.Label label2;
    }
}