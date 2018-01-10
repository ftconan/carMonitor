namespace carMonitor
{
    partial class frmPersonTag
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
            this.cboTag = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labPersonName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cboTag
            // 
            this.cboTag.Font = new System.Drawing.Font("SimSun", 12F);
            this.cboTag.FormattingEnabled = true;
            this.cboTag.Location = new System.Drawing.Point(258, 213);
            this.cboTag.Name = "cboTag";
            this.cboTag.Size = new System.Drawing.Size(170, 28);
            this.cboTag.TabIndex = 30;
            this.cboTag.TextUpdate += new System.EventHandler(this.cboTag_TextUpdate);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("SimSun", 14F);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(68, 213);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 24);
            this.label3.TabIndex = 29;
            this.label3.Text = "标签卡号";
            // 
            // btnConfirm
            // 
            this.btnConfirm.Font = new System.Drawing.Font("SimSun", 12F);
            this.btnConfirm.Location = new System.Drawing.Point(72, 347);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(110, 46);
            this.btnConfirm.TabIndex = 27;
            this.btnConfirm.Text = "确定";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("SimSun", 12F);
            this.btnExit.Location = new System.Drawing.Point(258, 347);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(106, 46);
            this.btnExit.TabIndex = 26;
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
            this.label1.Location = new System.Drawing.Point(68, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 24);
            this.label1.TabIndex = 25;
            this.label1.Text = "人员姓名";
            // 
            // labPersonName
            // 
            this.labPersonName.AutoSize = true;
            this.labPersonName.BackColor = System.Drawing.Color.Transparent;
            this.labPersonName.Font = new System.Drawing.Font("SimSun", 14F);
            this.labPersonName.ForeColor = System.Drawing.Color.Black;
            this.labPersonName.Location = new System.Drawing.Point(254, 90);
            this.labPersonName.Name = "labPersonName";
            this.labPersonName.Size = new System.Drawing.Size(0, 24);
            this.labPersonName.TabIndex = 31;
            // 
            // frmPersonTag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 469);
            this.Controls.Add(this.labPersonName);
            this.Controls.Add(this.cboTag);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.label1);
            this.Name = "frmPersonTag";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "绑定人员标签";
            this.Load += new System.EventHandler(this.frmPersonTag_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboTag;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labPersonName;
    }
}