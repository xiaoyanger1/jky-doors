namespace text.doors.Detection
{
    partial class PIDManager
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnhd = new System.Windows.Forms.Button();
            this.btnhi = new System.Windows.Forms.Button();
            this.btnhp = new System.Windows.Forms.Button();
            this.txthd = new System.Windows.Forms.TextBox();
            this.txthi = new System.Windows.Forms.TextBox();
            this.txthp = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnhd);
            this.groupBox1.Controls.Add(this.btnhi);
            this.groupBox1.Controls.Add(this.btnhp);
            this.groupBox1.Controls.Add(this.txthd);
            this.groupBox1.Controls.Add(this.txthi);
            this.groupBox1.Controls.Add(this.txthp);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(23, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(238, 115);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "设定";
            // 
            // btnhd
            // 
            this.btnhd.Location = new System.Drawing.Point(143, 79);
            this.btnhd.Name = "btnhd";
            this.btnhd.Size = new System.Drawing.Size(75, 23);
            this.btnhd.TabIndex = 17;
            this.btnhd.Text = "写入";
            this.btnhd.UseVisualStyleBackColor = true;
            this.btnhd.Click += new System.EventHandler(this.btnhd_Click);
            // 
            // btnhi
            // 
            this.btnhi.Location = new System.Drawing.Point(143, 48);
            this.btnhi.Name = "btnhi";
            this.btnhi.Size = new System.Drawing.Size(75, 23);
            this.btnhi.TabIndex = 16;
            this.btnhi.Text = "写入";
            this.btnhi.UseVisualStyleBackColor = true;
            this.btnhi.Click += new System.EventHandler(this.btnhi_Click);
            // 
            // btnhp
            // 
            this.btnhp.Location = new System.Drawing.Point(143, 18);
            this.btnhp.Name = "btnhp";
            this.btnhp.Size = new System.Drawing.Size(75, 23);
            this.btnhp.TabIndex = 15;
            this.btnhp.Text = "写入";
            this.btnhp.UseVisualStyleBackColor = true;
            this.btnhp.Click += new System.EventHandler(this.btnhp_Click);
            // 
            // txthd
            // 
            this.txthd.Location = new System.Drawing.Point(48, 81);
            this.txthd.Name = "txthd";
            this.txthd.Size = new System.Drawing.Size(80, 21);
            this.txthd.TabIndex = 8;
            // 
            // txthi
            // 
            this.txthi.Location = new System.Drawing.Point(48, 50);
            this.txthi.Name = "txthi";
            this.txthi.Size = new System.Drawing.Size(80, 21);
            this.txthi.TabIndex = 6;
            // 
            // txthp
            // 
            this.txthp.Location = new System.Drawing.Point(48, 20);
            this.txthp.Name = "txthp";
            this.txthp.Size = new System.Drawing.Size(80, 21);
            this.txthp.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "D：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "I：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "P：";
            // 
            // PIDManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 213);
            this.Controls.Add(this.groupBox1);
            this.Name = "PIDManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PID设置";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnhd;
        private System.Windows.Forms.Button btnhi;
        private System.Windows.Forms.Button btnhp;
        private System.Windows.Forms.TextBox txthd;
        private System.Windows.Forms.TextBox txthi;
        private System.Windows.Forms.TextBox txthp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}