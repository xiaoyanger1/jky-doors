namespace text.doors.Detection
{
    partial class SensorSet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SensorSet));
            this.btn_ok = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbb_type = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_Key = new System.Windows.Forms.TextBox();
            this.txt_ave = new System.Windows.Forms.TextBox();
            this.btn_clear = new System.Windows.Forms.Button();
            this.btn_Compute = new System.Windows.Forms.Button();
            this.btn_collection = new System.Windows.Forms.Button();
            this.lv_cjz = new System.Windows.Forms.ListView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbl_ID = new System.Windows.Forms.Label();
            this.txt_sKey = new System.Windows.Forms.TextBox();
            this.btn_del = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_update = new System.Windows.Forms.Button();
            this.lbl_Key = new System.Windows.Forms.Label();
            this.txt_sValue = new System.Windows.Forms.TextBox();
            this.lv_list = new System.Windows.Forms.ListView();
            this.cbb_cgq = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox7.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_ok
            // 
            this.btn_ok.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_ok.Location = new System.Drawing.Point(34, 185);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(206, 23);
            this.btn_ok.TabIndex = 2;
            this.btn_ok.Text = "应用此值";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label4);
            this.groupBox7.Controls.Add(this.cbb_type);
            this.groupBox7.Controls.Add(this.label2);
            this.groupBox7.Controls.Add(this.btn_ok);
            this.groupBox7.Controls.Add(this.label3);
            this.groupBox7.Controls.Add(this.txt_Key);
            this.groupBox7.Controls.Add(this.txt_ave);
            this.groupBox7.Controls.Add(this.btn_clear);
            this.groupBox7.Controls.Add(this.btn_Compute);
            this.groupBox7.Controls.Add(this.btn_collection);
            this.groupBox7.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox7.Location = new System.Drawing.Point(315, 35);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(270, 249);
            this.groupBox7.TabIndex = 22;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "标定";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 160);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "标准物理量：";
            // 
            // cbb_type
            // 
            this.cbb_type.FormattingEnabled = true;
            this.cbb_type.Items.AddRange(new object[] {
            "风速传感器      米/秒",
            "差压传感器      高 帕",
            "温度传感器      ℃",
            "大气压力传感器 KPa",
            "位移传感器1      mm",
            "位移传感器2      mm",
            "位移传感器3      mm"});
            this.cbb_type.Location = new System.Drawing.Point(118, 23);
            this.cbb_type.Name = "cbb_type";
            this.cbb_type.Size = new System.Drawing.Size(122, 20);
            this.cbb_type.TabIndex = 26;
            this.cbb_type.SelectedIndexChanged += new System.EventHandler(this.cbb_type_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 12);
            this.label2.TabIndex = 25;
            this.label2.Text = "传感器选择";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 129);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "采样平均值：";
            // 
            // txt_Key
            // 
            this.txt_Key.Location = new System.Drawing.Point(165, 154);
            this.txt_Key.Name = "txt_Key";
            this.txt_Key.Size = new System.Drawing.Size(75, 21);
            this.txt_Key.TabIndex = 0;
            // 
            // txt_ave
            // 
            this.txt_ave.Location = new System.Drawing.Point(165, 126);
            this.txt_ave.Name = "txt_ave";
            this.txt_ave.Size = new System.Drawing.Size(75, 21);
            this.txt_ave.TabIndex = 0;
            // 
            // btn_clear
            // 
            this.btn_clear.Location = new System.Drawing.Point(165, 57);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(75, 23);
            this.btn_clear.TabIndex = 2;
            this.btn_clear.Text = "清空";
            this.btn_clear.UseVisualStyleBackColor = true;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // btn_Compute
            // 
            this.btn_Compute.Location = new System.Drawing.Point(33, 91);
            this.btn_Compute.Name = "btn_Compute";
            this.btn_Compute.Size = new System.Drawing.Size(207, 23);
            this.btn_Compute.TabIndex = 2;
            this.btn_Compute.Text = "计算平均值";
            this.btn_Compute.UseVisualStyleBackColor = true;
            this.btn_Compute.Click += new System.EventHandler(this.btn_Compute_Click);
            // 
            // btn_collection
            // 
            this.btn_collection.Location = new System.Drawing.Point(33, 57);
            this.btn_collection.Name = "btn_collection";
            this.btn_collection.Size = new System.Drawing.Size(75, 23);
            this.btn_collection.TabIndex = 2;
            this.btn_collection.Text = "采标准点";
            this.btn_collection.UseVisualStyleBackColor = true;
            this.btn_collection.Click += new System.EventHandler(this.btn_collection_Click);
            // 
            // lv_cjz
            // 
            this.lv_cjz.HideSelection = false;
            this.lv_cjz.Location = new System.Drawing.Point(20, 20);
            this.lv_cjz.Name = "lv_cjz";
            this.lv_cjz.Size = new System.Drawing.Size(260, 217);
            this.lv_cjz.TabIndex = 23;
            this.lv_cjz.UseCompatibleStateImageBehavior = false;
            this.lv_cjz.View = System.Windows.Forms.View.Details;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(615, 346);
            this.tabControl1.TabIndex = 24;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox7);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(607, 320);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "标定";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lv_cjz);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(8, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(301, 249);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "采集";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.lv_list);
            this.tabPage2.Controls.Add(this.cbb_cgq);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(607, 320);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "标定查询";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbl_ID);
            this.groupBox2.Controls.Add(this.txt_sKey);
            this.groupBox2.Controls.Add(this.btn_del);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.btn_update);
            this.groupBox2.Controls.Add(this.lbl_Key);
            this.groupBox2.Controls.Add(this.txt_sValue);
            this.groupBox2.Location = new System.Drawing.Point(316, 64);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(257, 234);
            this.groupBox2.TabIndex = 33;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "选择值";
            // 
            // lbl_ID
            // 
            this.lbl_ID.AutoSize = true;
            this.lbl_ID.Location = new System.Drawing.Point(90, 17);
            this.lbl_ID.Name = "lbl_ID";
            this.lbl_ID.Size = new System.Drawing.Size(0, 12);
            this.lbl_ID.TabIndex = 33;
            this.lbl_ID.Visible = false;
            // 
            // txt_sKey
            // 
            this.txt_sKey.Location = new System.Drawing.Point(92, 42);
            this.txt_sKey.Name = "txt_sKey";
            this.txt_sKey.Size = new System.Drawing.Size(100, 21);
            this.txt_sKey.TabIndex = 31;
            // 
            // btn_del
            // 
            this.btn_del.Location = new System.Drawing.Point(133, 144);
            this.btn_del.Name = "btn_del";
            this.btn_del.Size = new System.Drawing.Size(75, 23);
            this.btn_del.TabIndex = 32;
            this.btn_del.Text = "删除";
            this.btn_del.UseVisualStyleBackColor = true;
            this.btn_del.Click += new System.EventHandler(this.btn_del_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(39, 89);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 30;
            this.label5.Text = "Vlues:";
            // 
            // btn_update
            // 
            this.btn_update.Location = new System.Drawing.Point(39, 144);
            this.btn_update.Name = "btn_update";
            this.btn_update.Size = new System.Drawing.Size(75, 23);
            this.btn_update.TabIndex = 32;
            this.btn_update.Text = "修改";
            this.btn_update.UseVisualStyleBackColor = true;
            this.btn_update.Click += new System.EventHandler(this.btn_update_Click);
            // 
            // lbl_Key
            // 
            this.lbl_Key.AutoSize = true;
            this.lbl_Key.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_Key.Location = new System.Drawing.Point(39, 51);
            this.lbl_Key.Name = "lbl_Key";
            this.lbl_Key.Size = new System.Drawing.Size(47, 12);
            this.lbl_Key.TabIndex = 30;
            this.lbl_Key.Text = "  Key:";
            // 
            // txt_sValue
            // 
            this.txt_sValue.Location = new System.Drawing.Point(92, 86);
            this.txt_sValue.Name = "txt_sValue";
            this.txt_sValue.Size = new System.Drawing.Size(100, 21);
            this.txt_sValue.TabIndex = 31;
            // 
            // lv_list
            // 
            this.lv_list.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lv_list.FullRowSelect = true;
            this.lv_list.HideSelection = false;
            this.lv_list.Location = new System.Drawing.Point(29, 64);
            this.lv_list.Name = "lv_list";
            this.lv_list.Size = new System.Drawing.Size(260, 234);
            this.lv_list.TabIndex = 29;
            this.lv_list.UseCompatibleStateImageBehavior = false;
            this.lv_list.View = System.Windows.Forms.View.Details;
            this.lv_list.SelectedIndexChanged += new System.EventHandler(this.lv_list_SelectedIndexChanged);
            // 
            // cbb_cgq
            // 
            this.cbb_cgq.FormattingEnabled = true;
            this.cbb_cgq.Items.AddRange(new object[] {
            "风速传感器      米/秒",
            "差压传感器      高 帕",
            "温度传感器      ℃",
            "大气压力传感器 KPa",
            "位移传感器1      mm",
            "位移传感器2      mm",
            "位移传感器3      mm"});
            this.cbb_cgq.Location = new System.Drawing.Point(107, 18);
            this.cbb_cgq.Name = "cbb_cgq";
            this.cbb_cgq.Size = new System.Drawing.Size(122, 20);
            this.cbb_cgq.TabIndex = 28;
            this.cbb_cgq.SelectedIndexChanged += new System.EventHandler(this.cbb_cgq_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 27;
            this.label1.Text = "传感器选择";
            // 
            // SensorSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 368);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SensorSet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "传感器标定";
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.Button btn_collection;
        private System.Windows.Forms.ComboBox cbb_type;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_Compute;
        private System.Windows.Forms.TextBox txt_Key;
        private System.Windows.Forms.TextBox txt_ave;
        private System.Windows.Forms.ListView lv_cjz;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbb_cgq;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lv_list;
        private System.Windows.Forms.Label lbl_Key;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_sValue;
        private System.Windows.Forms.TextBox txt_sKey;
        private System.Windows.Forms.Button btn_del;
        private System.Windows.Forms.Button btn_update;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbl_ID;
    }
}