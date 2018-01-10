namespace carMonitor
{
    partial class frmAddCar
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
            this.cboPersonName = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCarName = new System.Windows.Forms.TextBox();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCarNum = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cboPersonName
            // 
            this.cboPersonName.Font = new System.Drawing.Font("SimSun", 12F);
            this.cboPersonName.FormattingEnabled = true;
            this.cboPersonName.Location = new System.Drawing.Point(285, 238);
            this.cboPersonName.Name = "cboPersonName";
            this.cboPersonName.Size = new System.Drawing.Size(170, 28);
            this.cboPersonName.TabIndex = 30;
            this.cboPersonName.TextUpdate += new System.EventHandler(this.cboPersonName_TextUpdate);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("SimSun", 14F);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(94, 242);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 24);
            this.label3.TabIndex = 29;
            this.label3.Text = "车主";
            // 
            // txtCarName
            // 
            this.txtCarName.Font = new System.Drawing.Font("SimSun", 12F);
            this.txtCarName.Location = new System.Drawing.Point(285, 70);
            this.txtCarName.Name = "txtCarName";
            this.txtCarName.Size = new System.Drawing.Size(170, 30);
            this.txtCarName.TabIndex = 28;
            // 
            // btnConfirm
            // 
            this.btnConfirm.Font = new System.Drawing.Font("SimSun", 12F);
            this.btnConfirm.Location = new System.Drawing.Point(98, 338);
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
            this.btnExit.Location = new System.Drawing.Point(285, 338);
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
            this.label1.Location = new System.Drawing.Point(94, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 24);
            this.label1.TabIndex = 25;
            this.label1.Text = "车辆名称";
            // 
            // txtCarNum
            // 
            this.txtCarNum.Font = new System.Drawing.Font("SimSun", 12F);
            this.txtCarNum.Location = new System.Drawing.Point(285, 153);
            this.txtCarNum.Name = "txtCarNum";
            this.txtCarNum.Size = new System.Drawing.Size(170, 30);
            this.txtCarNum.TabIndex = 32;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("SimSun", 14F);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(94, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 24);
            this.label2.TabIndex = 31;
            this.label2.Text = "车牌号";
            // 
            // frmAddCar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 473);
            this.Controls.Add(this.txtCarNum);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboPersonName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtCarName);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.label1);
            this.Name = "frmAddCar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加电瓶车";
            this.Load += new System.EventHandler(this.frmAddCar_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboPersonName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCarName;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCarNum;
        private System.Windows.Forms.Label label2;
    }
}