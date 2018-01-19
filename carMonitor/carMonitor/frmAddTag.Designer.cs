namespace carMonitor
{
    partial class frmAddTag
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
            this.cboTagType = new System.Windows.Forms.ComboBox();
            this.txtNum = new System.Windows.Forms.TextBox();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cboTagType
            // 
            this.cboTagType.Font = new System.Drawing.Font("SimSun", 12F);
            this.cboTagType.FormattingEnabled = true;
            this.cboTagType.Items.AddRange(new object[] {
            "人员标签",
            "车辆标签"});
            this.cboTagType.Location = new System.Drawing.Point(252, 205);
            this.cboTagType.Name = "cboTagType";
            this.cboTagType.Size = new System.Drawing.Size(170, 28);
            this.cboTagType.TabIndex = 24;
            // 
            // txtNum
            // 
            this.txtNum.Font = new System.Drawing.Font("SimSun", 12F);
            this.txtNum.Location = new System.Drawing.Point(252, 89);
            this.txtNum.Name = "txtNum";
            this.txtNum.Size = new System.Drawing.Size(170, 30);
            this.txtNum.TabIndex = 21;
            // 
            // btnConfirm
            // 
            this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.Font = new System.Drawing.Font("SimSun", 12F);
            this.btnConfirm.Location = new System.Drawing.Point(65, 338);
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
            this.btnExit.Location = new System.Drawing.Point(252, 338);
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
            this.label1.Location = new System.Drawing.Point(61, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 24);
            this.label1.TabIndex = 17;
            this.label1.Text = "标签卡号";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("SimSun", 14F);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(61, 209);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 24);
            this.label3.TabIndex = 23;
            this.label3.Text = "标签类型";
            // 
            // frmAddTag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 457);
            this.Controls.Add(this.cboTagType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtNum);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.label1);
            this.Name = "frmAddTag";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加标签";
            this.Load += new System.EventHandler(this.frmAddTag_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboTagType;
        private System.Windows.Forms.TextBox txtNum;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
    }
}