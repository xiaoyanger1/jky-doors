namespace text.doors.Detection
{
    partial class AirtightDetection
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
            this.components = new System.ComponentModel.Container();
            this.tc_RealTimeSurveillance = new System.Windows.Forms.TabControl();
            this.page_airtight = new System.Windows.Forms.TabPage();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.tChart_qm = new Steema.TeeChart.TChart();
            this.chart_cms_qm_click = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.export_image_qm = new System.Windows.Forms.ToolStripMenuItem();
            this.qm_Line = new Steema.TeeChart.Styles.FastLine();
            this.btn_exit = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbl_dqyl = new System.Windows.Forms.Label();
            this.lbl_setYL = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbl_title = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.风速原始数据 = new System.Windows.Forms.TabPage();
            this.dgv_WindSpeed = new System.Windows.Forms.DataGridView();
            this.流量原始数据 = new System.Windows.Forms.TabPage();
            this.dgv_ll = new System.Windows.Forms.DataGridView();
            this.btn_datadispose = new System.Windows.Forms.Button();
            this.btn_stop = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rdb_zdstl = new System.Windows.Forms.RadioButton();
            this.btn_losestart = new System.Windows.Forms.Button();
            this.btn_juststart = new System.Windows.Forms.Button();
            this.btn_loseready = new System.Windows.Forms.Button();
            this.btn_justready = new System.Windows.Forms.Button();
            this.rdb_fjstl = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgv_levelIndex = new System.Windows.Forms.DataGridView();
            this.tim_qm = new System.Windows.Forms.Timer(this.components);
            this.tim_getType = new System.Windows.Forms.Timer(this.components);
            this.tim_Top10 = new System.Windows.Forms.Timer(this.components);
            this.gv_list = new System.Windows.Forms.Timer(this.components);
            this.tim_PainPic = new System.Windows.Forms.Timer(this.components);
            this.tc_RealTimeSurveillance.SuspendLayout();
            this.page_airtight.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.chart_cms_qm_click.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.风速原始数据.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_WindSpeed)).BeginInit();
            this.流量原始数据.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ll)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_levelIndex)).BeginInit();
            this.SuspendLayout();
            // 
            // tc_RealTimeSurveillance
            // 
            this.tc_RealTimeSurveillance.Controls.Add(this.page_airtight);
            this.tc_RealTimeSurveillance.ItemSize = new System.Drawing.Size(120, 26);
            this.tc_RealTimeSurveillance.Location = new System.Drawing.Point(0, 0);
            this.tc_RealTimeSurveillance.Name = "tc_RealTimeSurveillance";
            this.tc_RealTimeSurveillance.SelectedIndex = 0;
            this.tc_RealTimeSurveillance.Size = new System.Drawing.Size(1151, 621);
            this.tc_RealTimeSurveillance.TabIndex = 0;
            // 
            // page_airtight
            // 
            this.page_airtight.BackColor = System.Drawing.Color.White;
            this.page_airtight.Controls.Add(this.groupBox9);
            this.page_airtight.Controls.Add(this.btn_exit);
            this.page_airtight.Controls.Add(this.groupBox1);
            this.page_airtight.Controls.Add(this.tabControl1);
            this.page_airtight.Controls.Add(this.btn_datadispose);
            this.page_airtight.Controls.Add(this.btn_stop);
            this.page_airtight.Controls.Add(this.groupBox3);
            this.page_airtight.Controls.Add(this.groupBox2);
            this.page_airtight.Location = new System.Drawing.Point(4, 30);
            this.page_airtight.Name = "page_airtight";
            this.page_airtight.Padding = new System.Windows.Forms.Padding(3);
            this.page_airtight.Size = new System.Drawing.Size(1143, 587);
            this.page_airtight.TabIndex = 0;
            this.page_airtight.Text = "气密监控";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.tChart_qm);
            this.groupBox9.Location = new System.Drawing.Point(9, 122);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(853, 437);
            this.groupBox9.TabIndex = 20;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "检测";
            // 
            // tChart_qm
            // 
            // 
            // 
            // 
            this.tChart_qm.Aspect.ZOffset = 0D;
            this.tChart_qm.BackColor = System.Drawing.Color.White;
            this.tChart_qm.ContextMenuStrip = this.chart_cms_qm_click;
            this.tChart_qm.Cursor = System.Windows.Forms.Cursors.Default;
            // 
            // 
            // 
            this.tChart_qm.Header.Lines = new string[] {
        "气密检测"};
            this.tChart_qm.Location = new System.Drawing.Point(6, 50);
            this.tChart_qm.Name = "tChart_qm";
            // 
            // 
            // 
            // 
            // 
            // 
            this.tChart_qm.Panel.Brush.Color = System.Drawing.Color.White;
            // 
            // 
            // 
            this.tChart_qm.Panel.Brush.Gradient.Visible = false;
            this.tChart_qm.Panel.MarginLeft = 0D;
            this.tChart_qm.Panel.MarginRight = 2D;
            this.tChart_qm.Panel.MarginTop = 0D;
            this.tChart_qm.Series.Add(this.qm_Line);
            this.tChart_qm.Size = new System.Drawing.Size(832, 294);
            this.tChart_qm.TabIndex = 18;
            this.tChart_qm.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tChart1_MouseDown);
            // 
            // chart_cms_qm_click
            // 
            this.chart_cms_qm_click.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.export_image_qm});
            this.chart_cms_qm_click.Name = "contextMenuStrip1";
            this.chart_cms_qm_click.Size = new System.Drawing.Size(125, 26);
            // 
            // export_image_qm
            // 
            this.export_image_qm.Name = "export_image_qm";
            this.export_image_qm.Size = new System.Drawing.Size(124, 22);
            this.export_image_qm.Text = "导出图片";
            this.export_image_qm.Click += new System.EventHandler(this.export_image_qm_Click);
            // 
            // qm_Line
            // 
            this.qm_Line.Color = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(102)))), ((int)(((byte)(163)))));
            this.qm_Line.ColorEach = false;
            this.qm_Line.Cursor = System.Windows.Forms.Cursors.Cross;
            // 
            // 
            // 
            this.qm_Line.LinePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(102)))), ((int)(((byte)(163)))));
            // 
            // 
            // 
            // 
            // 
            // 
            this.qm_Line.Marks.Callout.ArrowHead = Steema.TeeChart.Styles.ArrowHeadStyles.None;
            this.qm_Line.Marks.Callout.ArrowHeadSize = 8;
            // 
            // 
            // 
            this.qm_Line.Marks.Callout.Brush.Color = System.Drawing.Color.Black;
            this.qm_Line.Marks.Callout.Distance = 0;
            this.qm_Line.Marks.Callout.Draw3D = false;
            this.qm_Line.Marks.Callout.Length = 10;
            this.qm_Line.Marks.Callout.Style = Steema.TeeChart.Styles.PointerStyles.Rectangle;
            this.qm_Line.ShowInLegend = false;
            this.qm_Line.Title = "fastLine1";
            this.qm_Line.TreatNulls = Steema.TeeChart.Styles.TreatNullsStyle.Ignore;
            // 
            // 
            // 
            this.qm_Line.XValues.DataMember = "X";
            this.qm_Line.XValues.DateTime = true;
            this.qm_Line.XValues.Order = Steema.TeeChart.Styles.ValueListOrder.Ascending;
            // 
            // 
            // 
            this.qm_Line.YValues.DataMember = "Y";
            // 
            // btn_exit
            // 
            this.btn_exit.Location = new System.Drawing.Point(1052, 526);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Size = new System.Drawing.Size(75, 33);
            this.btn_exit.TabIndex = 19;
            this.btn_exit.Text = "退出";
            this.btn_exit.UseVisualStyleBackColor = true;
            this.btn_exit.Click += new System.EventHandler(this.btn_exit_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbl_dqyl);
            this.groupBox1.Controls.Add(this.lbl_setYL);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.lbl_title);
            this.groupBox1.Location = new System.Drawing.Point(9, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(853, 82);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "状态";
            // 
            // lbl_dqyl
            // 
            this.lbl_dqyl.AutoSize = true;
            this.lbl_dqyl.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_dqyl.Location = new System.Drawing.Point(673, 46);
            this.lbl_dqyl.Name = "lbl_dqyl";
            this.lbl_dqyl.Size = new System.Drawing.Size(17, 16);
            this.lbl_dqyl.TabIndex = 24;
            this.lbl_dqyl.Text = "0";
            // 
            // lbl_setYL
            // 
            this.lbl_setYL.AutoSize = true;
            this.lbl_setYL.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_setYL.Location = new System.Drawing.Point(672, 15);
            this.lbl_setYL.Name = "lbl_setYL";
            this.lbl_setYL.Size = new System.Drawing.Size(17, 16);
            this.lbl_setYL.TabIndex = 23;
            this.lbl_setYL.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(497, 46);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(144, 16);
            this.label7.TabIndex = 22;
            this.label7.Text = "当前压力（帕）：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(497, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(144, 16);
            this.label6.TabIndex = 21;
            this.label6.Text = "设定压力（帕）：";
            // 
            // lbl_title
            // 
            this.lbl_title.AutoSize = true;
            this.lbl_title.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_title.Location = new System.Drawing.Point(77, 36);
            this.lbl_title.Name = "lbl_title";
            this.lbl_title.Size = new System.Drawing.Size(257, 16);
            this.lbl_title.TabIndex = 20;
            this.lbl_title.Text = "门窗气密性能检测  第0号 第0樘";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.风速原始数据);
            this.tabControl1.Controls.Add(this.流量原始数据);
            this.tabControl1.Location = new System.Drawing.Point(868, 7);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(273, 202);
            this.tabControl1.TabIndex = 16;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // 风速原始数据
            // 
            this.风速原始数据.Controls.Add(this.dgv_WindSpeed);
            this.风速原始数据.Location = new System.Drawing.Point(4, 22);
            this.风速原始数据.Name = "风速原始数据";
            this.风速原始数据.Padding = new System.Windows.Forms.Padding(3);
            this.风速原始数据.Size = new System.Drawing.Size(265, 176);
            this.风速原始数据.TabIndex = 0;
            this.风速原始数据.Text = "风速原始数据(m/s)";
            this.风速原始数据.UseVisualStyleBackColor = true;
            // 
            // dgv_WindSpeed
            // 
            this.dgv_WindSpeed.AllowUserToResizeColumns = false;
            this.dgv_WindSpeed.AllowUserToResizeRows = false;
            this.dgv_WindSpeed.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_WindSpeed.Location = new System.Drawing.Point(3, 5);
            this.dgv_WindSpeed.Name = "dgv_WindSpeed";
            this.dgv_WindSpeed.RowTemplate.Height = 23;
            this.dgv_WindSpeed.Size = new System.Drawing.Size(260, 165);
            this.dgv_WindSpeed.TabIndex = 0;
            // 
            // 流量原始数据
            // 
            this.流量原始数据.Controls.Add(this.dgv_ll);
            this.流量原始数据.Location = new System.Drawing.Point(4, 22);
            this.流量原始数据.Name = "流量原始数据";
            this.流量原始数据.Padding = new System.Windows.Forms.Padding(3);
            this.流量原始数据.Size = new System.Drawing.Size(265, 176);
            this.流量原始数据.TabIndex = 1;
            this.流量原始数据.Text = "流量原始数据(m3/h)";
            this.流量原始数据.UseVisualStyleBackColor = true;
            // 
            // dgv_ll
            // 
            this.dgv_ll.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_ll.Location = new System.Drawing.Point(4, 5);
            this.dgv_ll.Name = "dgv_ll";
            this.dgv_ll.RowTemplate.Height = 23;
            this.dgv_ll.Size = new System.Drawing.Size(260, 165);
            this.dgv_ll.TabIndex = 0;
            // 
            // btn_datadispose
            // 
            this.btn_datadispose.Location = new System.Drawing.Point(865, 526);
            this.btn_datadispose.Name = "btn_datadispose";
            this.btn_datadispose.Size = new System.Drawing.Size(82, 33);
            this.btn_datadispose.TabIndex = 14;
            this.btn_datadispose.Text = "数据处理";
            this.btn_datadispose.UseVisualStyleBackColor = true;
            this.btn_datadispose.Click += new System.EventHandler(this.btn_datadispose_Click);
            // 
            // btn_stop
            // 
            this.btn_stop.Location = new System.Drawing.Point(961, 526);
            this.btn_stop.Name = "btn_stop";
            this.btn_stop.Size = new System.Drawing.Size(83, 33);
            this.btn_stop.TabIndex = 12;
            this.btn_stop.Text = "停止";
            this.btn_stop.UseVisualStyleBackColor = true;
            this.btn_stop.Click += new System.EventHandler(this.btn_stop_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rdb_zdstl);
            this.groupBox3.Controls.Add(this.btn_losestart);
            this.groupBox3.Controls.Add(this.btn_juststart);
            this.groupBox3.Controls.Add(this.btn_loseready);
            this.groupBox3.Controls.Add(this.btn_justready);
            this.groupBox3.Controls.Add(this.rdb_fjstl);
            this.groupBox3.Location = new System.Drawing.Point(865, 352);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(272, 142);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "监控";
            // 
            // rdb_zdstl
            // 
            this.rdb_zdstl.AutoSize = true;
            this.rdb_zdstl.Checked = true;
            this.rdb_zdstl.Location = new System.Drawing.Point(160, 20);
            this.rdb_zdstl.Name = "rdb_zdstl";
            this.rdb_zdstl.Size = new System.Drawing.Size(83, 16);
            this.rdb_zdstl.TabIndex = 0;
            this.rdb_zdstl.TabStop = true;
            this.rdb_zdstl.Tag = "stl";
            this.rdb_zdstl.Text = "总的渗透量";
            this.rdb_zdstl.UseVisualStyleBackColor = true;
            // 
            // btn_losestart
            // 
            this.btn_losestart.Location = new System.Drawing.Point(148, 88);
            this.btn_losestart.Name = "btn_losestart";
            this.btn_losestart.Size = new System.Drawing.Size(95, 41);
            this.btn_losestart.TabIndex = 4;
            this.btn_losestart.Text = "负压开始";
            this.btn_losestart.UseVisualStyleBackColor = true;
            this.btn_losestart.Click += new System.EventHandler(this.btn_losestart_Click);
            // 
            // btn_juststart
            // 
            this.btn_juststart.Location = new System.Drawing.Point(38, 89);
            this.btn_juststart.Name = "btn_juststart";
            this.btn_juststart.Size = new System.Drawing.Size(95, 41);
            this.btn_juststart.TabIndex = 4;
            this.btn_juststart.Text = "正压开始";
            this.btn_juststart.UseVisualStyleBackColor = true;
            this.btn_juststart.Click += new System.EventHandler(this.btn_juststart_Click);
            // 
            // btn_loseready
            // 
            this.btn_loseready.Location = new System.Drawing.Point(148, 42);
            this.btn_loseready.Name = "btn_loseready";
            this.btn_loseready.Size = new System.Drawing.Size(95, 41);
            this.btn_loseready.TabIndex = 4;
            this.btn_loseready.Text = "负压预备 150Pa";
            this.btn_loseready.UseVisualStyleBackColor = true;
            this.btn_loseready.Click += new System.EventHandler(this.btn_loseready_Click);
            // 
            // btn_justready
            // 
            this.btn_justready.Location = new System.Drawing.Point(38, 43);
            this.btn_justready.Name = "btn_justready";
            this.btn_justready.Size = new System.Drawing.Size(95, 41);
            this.btn_justready.TabIndex = 4;
            this.btn_justready.Text = "正压预备 150Pa";
            this.btn_justready.UseVisualStyleBackColor = true;
            this.btn_justready.Click += new System.EventHandler(this.btn_justready_Click);
            // 
            // rdb_fjstl
            // 
            this.rdb_fjstl.AutoSize = true;
            this.rdb_fjstl.Location = new System.Drawing.Point(38, 21);
            this.rdb_fjstl.Name = "rdb_fjstl";
            this.rdb_fjstl.Size = new System.Drawing.Size(83, 16);
            this.rdb_fjstl.TabIndex = 0;
            this.rdb_fjstl.Tag = "stl";
            this.rdb_fjstl.Text = "附加渗透量";
            this.rdb_fjstl.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgv_levelIndex);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(865, 222);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(275, 112);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "分级指标";
            // 
            // dgv_levelIndex
            // 
            this.dgv_levelIndex.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_levelIndex.Location = new System.Drawing.Point(10, 20);
            this.dgv_levelIndex.Name = "dgv_levelIndex";
            this.dgv_levelIndex.RowTemplate.Height = 23;
            this.dgv_levelIndex.Size = new System.Drawing.Size(259, 86);
            this.dgv_levelIndex.TabIndex = 0;
            // 
            // tim_qm
            // 
            this.tim_qm.Enabled = true;
            this.tim_qm.Interval = 2000;
            this.tim_qm.Tick += new System.EventHandler(this.tim_qm_Tick);
            // 
            // tim_getType
            // 
            this.tim_getType.Enabled = true;
            this.tim_getType.Tick += new System.EventHandler(this.tim_getType_Tick);
            // 
            // tim_Top10
            // 
            this.tim_Top10.Interval = 800;
            this.tim_Top10.Tick += new System.EventHandler(this.tim_Top10_Tick);
            // 
            // gv_list
            // 
            this.gv_list.Interval = 1000;
            this.gv_list.Tick += new System.EventHandler(this.gv_list_Tick);
            // 
            // tim_PainPic
            // 
            this.tim_PainPic.Enabled = true;
            this.tim_PainPic.Interval = 800;
            this.tim_PainPic.Tick += new System.EventHandler(this.tim_PainPic_Tick);
            // 
            // AirtightDetection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1153, 628);
            this.Controls.Add(this.tc_RealTimeSurveillance);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AirtightDetection";
            this.Text = "RealTimeSurveillance";
            this.tc_RealTimeSurveillance.ResumeLayout(false);
            this.page_airtight.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.chart_cms_qm_click.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.风速原始数据.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_WindSpeed)).EndInit();
            this.流量原始数据.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ll)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_levelIndex)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tc_RealTimeSurveillance;
        private System.Windows.Forms.TabPage page_airtight;
        private System.Windows.Forms.Button btn_stop;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rdb_zdstl;
        private System.Windows.Forms.Button btn_losestart;
        private System.Windows.Forms.Button btn_juststart;
        private System.Windows.Forms.Button btn_loseready;
        private System.Windows.Forms.Button btn_justready;
        private System.Windows.Forms.RadioButton rdb_fjstl;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_datadispose;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage 风速原始数据;
        private System.Windows.Forms.TabPage 流量原始数据;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbl_title;
        private Steema.TeeChart.TChart tChart_qm;
        private System.Windows.Forms.DataGridView dgv_WindSpeed;
        private System.Windows.Forms.DataGridView dgv_levelIndex;
        private System.Windows.Forms.DataGridView dgv_ll;
        private Steema.TeeChart.Styles.FastLine qm_Line;
        private System.Windows.Forms.ContextMenuStrip chart_cms_qm_click;
        private System.Windows.Forms.ToolStripMenuItem export_image_qm;
        private System.Windows.Forms.Label lbl_setYL;
        private System.Windows.Forms.Label lbl_dqyl;
        private System.Windows.Forms.Timer tim_Top10;
        private System.Windows.Forms.Timer gv_list;
        private System.Windows.Forms.Button btn_exit;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Timer tim_qm;
        private System.Windows.Forms.Timer tim_getType;
        private System.Windows.Forms.Timer tim_PainPic;
    }
}