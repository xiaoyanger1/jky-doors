
using text.doors.Common;
using text.doors.Detection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge;
using AForge.Controls;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using Young.Core.Common;
using text.doors.Default;

namespace text.doors
{
    public partial class MainForm : Form
    {
        private static TCPClient tcpClient = new TCPClient();
        public static Young.Core.Logger.ILog Logger = Young.Core.Logger.LoggerManager.Current();


        /// <summary>
        /// 当前温度
        /// </summary>
        private double _temperature = 0;
        /// <summary>
        /// 当前压力
        /// </summary>
        private double _temppressure = 0;

        /// <summary>
        /// 检验编号
        /// </summary>
        private string _tempCode = "";
        /// <summary>
        /// 当前樘号
        /// </summary>
        private string _tempTong = "";

        public MainForm()
        {
            InitializeComponent();

            ExamineLAN();
            OpenTcp();

            //设置高压标零
            var pressureZero = tcpClient.SendGYBD(true);

            DataInit();
            ShowDetectionSet();
        }


        private void ExamineLAN()
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    LAN.ReadLanLink();

                    using (BackgroundWorker bw = new BackgroundWorker())
                    {
                        bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Lan_RunWorkerCompleted);
                        bw.DoWork += new DoWorkEventHandler(Lan_DoWork);
                        bw.RunWorkerAsync();
                    }
                    Thread.Sleep(5000);
                }
            }));
            thread.Start();
        }
        void Lan_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = LAN.IsLanLink ? "网络连接：开启" : "网络连接：断开";
        }

        void Lan_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                this.tcp_type.Text = e.Result.ToString();
            }
            catch (Exception ex)
            {
            }
        }

        private void OpenTcp()
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    using (BackgroundWorker bw = new BackgroundWorker())
                    {
                        bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Tcp_RunWorkerCompleted);
                        bw.DoWork += new DoWorkEventHandler(Tcp_DoWork);
                        bw.RunWorkerAsync();
                    }
                    if (!tcpClient.IsTCPLink)
                    {
                        tcpClient.TcpOpen();
                        Thread.Sleep(500);
                    }
                    else
                        Thread.Sleep(5000);
                }
            }));
            thread.Start();
        }

        void Tcp_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = tcpClient.IsTCPLink ? "服务器连接：成功" : "服务器连接：失败";
        }

        void Tcp_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                this.tsl_tcpclient.Text = e.Result.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }


        private void SelectDangHao(text.doors.Detection.DetectionSet.BottomType bt)
        {
            this.tssl_SetCode.Text = string.Format("{0}  {1}", bt.Code, bt.Tong);
            if (bt.ISOK == true)
            { this.tsl_type.Visible = false; }
            else { this.tsl_type.Visible = true; }

            _tempCode = bt.Code;
            _tempTong = bt.Tong;
            DefaultBase.IsSetTong = bt.ISOK;
            if (bt.ISOK)
            {
                ShowAirtightDetection();
            }
        }

        /// <summary>
        /// 检测监控
        /// </summary>
        //private void ShowRealTimeSurveillance(int type)
        //{
        //    RealTimeSurveillance rts = new RealTimeSurveillance(tcpClient, _tempCode, _tempTong, type);
        //    this.pl_showItem.Controls.Clear();
        //    rts.TopLevel = false;
        //    rts.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        //    rts.Parent = this.pl_showItem;
        //    rts.Show();
        //}

        /// <summary>
        /// 参数设置
        /// </summary>
        private void ShowDetectionSet()
        {
            this.pl_showItem.Controls.Clear();
            DetectionSet ds = new DetectionSet(_temperature, _temppressure, _tempCode, _tempTong);
            ds.deleBottomTypeEvent += new DetectionSet.deleBottomType(SelectDangHao);
            ds.GetDangHaoTrigger();
            ds.TopLevel = false;
            ds.Parent = this.pl_showItem;
            ds.Show();


            //new AirtightDetection().Dispose();
            // new WatertightDetection().Dispose();
            //new WindPressureDetection().Dispose();

            //new AirtightDetection().StopTimer();
            //new WatertightDetection().StopTimer();
            //new WindPressureDetection().StopTimer();
        }

        /// <summary>
        /// 水密监控
        /// </summary>
        private void ShowWatertightDetection()
        {
            WatertightDetection rts = new WatertightDetection(tcpClient, _tempCode, _tempTong);
            this.pl_showItem.Controls.Clear();
            rts.TopLevel = false;
            rts.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            rts.Parent = this.pl_showItem;
            rts.Show();

            //new AirtightDetection().StopTimer();
            //new WatertightDetection().InitTimer();
            //new WindPressureDetection().StopTimer();
        }
        /// <summary>
        /// 气密监控
        /// </summary>
        private void ShowAirtightDetection()
        {
            AirtightDetection rts = new AirtightDetection(tcpClient, _tempCode, _tempTong);
            this.pl_showItem.Controls.Clear();
            rts.TopLevel = false;
            rts.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            rts.Parent = this.pl_showItem;
            rts.Show();


            //new AirtightDetection().InitTimer();
            //new WatertightDetection().StopTimer();
            //new WindPressureDetection().StopTimer();
        }
        /// <summary>
        /// 抗风压
        /// </summary>
        private void ShowWindPressure()
        {
            WindPressureDetection rts = new WindPressureDetection(tcpClient, _tempCode, _tempTong);
            this.pl_showItem.Controls.Clear();
            rts.TopLevel = false;
            rts.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            rts.Parent = this.pl_showItem;
            rts.Show();

            //new AirtightDetection().Dispose();
            //new WatertightDetection().Dispose();
            //new DetectionSet().Dispose();

            //new AirtightDetection().StopTimer();
            //new WatertightDetection().StopTimer();
            //new WindPressureDetection().InitTimer();
        }


        private void DataInit()
        {
            //隐藏打开按钮
            // tsb_open.Visible = false;

            if (tcpClient.IsTCPLink)
            {
                #region 获取面板显示
                var IsSeccess = false;

                var temperature = _temperature = tcpClient.GetWDXS(ref IsSeccess);
                if (!IsSeccess) return;

                var temppressure = _temppressure = tcpClient.GetDQYLXS(ref IsSeccess);
                if (!IsSeccess) return;

                var windSpeed = tcpClient.GetFSXS(ref IsSeccess).ToString();
                if (!IsSeccess) return;

                var diffPress = tcpClient.GetCYXS(ref IsSeccess).ToString();
                if (!IsSeccess) return;

                lbl_wdcgq.Text = temperature.ToString();
                lbl_dqylcgq.Text = temppressure.ToString();
                lbl_fscgq.Text = windSpeed.ToString();
                lbl_cycgq.Text = diffPress.ToString();


                //抗风压
                var displace1 = tcpClient.GetDisplace1(ref IsSeccess).ToString();
                if (!IsSeccess) return;
                var displace2 = tcpClient.GetDisplace2(ref IsSeccess).ToString();
                if (!IsSeccess) return;
                var displace3 = tcpClient.GetDisplace3(ref IsSeccess).ToString();
                if (!IsSeccess) return;

                lbl_Displace1.Text = displace1.ToString();
                lbl_Displace2.Text = displace2.ToString();
                lbl_Displace3.Text = displace3.ToString();

                #endregion

                #region 获取正负压阀状态
                GetPRVs();
                #endregion
            }
        }

        private void GetPRVs()
        {
            //检测正负压阀状态
            bool z = false, f = false;
            var res = tcpClient.GetZFYF(ref z, ref f);
            if (!res)
            {
                return;
                //  MessageBox.Show("压阀状态异常,请确认服务器连接是否成功!", "检测状态", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
            btn_z.BackColor = z ? Color.Green : Color.Transparent;
            btn_f.BackColor = f ? Color.Green : Color.Transparent;
        }

        private void hsb_WindControl_Scroll(object sender, ScrollEventArgs e)
        {
            if (hsb_WindControl.Value == 0)
                txt_hz.Text = "0.00";
            else
                txt_hz.Text = (hsb_WindControl.Value * 0.01).ToString();

            double value = (hsb_WindControl.Value * 0.01) * 80;



            Logger.Info("发送-" + value);
            var res = tcpClient.SendFJKZ(value);

            if (!res)
            {
                MessageBox.Show("风机控制异常,请确认服务器连接是否成功!", "风机", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
        }

        //关闭
        private void tsm_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //收起
        private void tsb_fewer_Click(object sender, EventArgs e)
        {
            this.pl_set.Visible = false;
            this.tsb_fewer.Visible = false;
            this.tsb_open.Visible = true;
        }

        //打开
        private void tsb_open_Click(object sender, EventArgs e)
        {
            this.pl_set.Visible = true;
            this.tsb_open.Visible = false;
            this.tsb_fewer.Visible = true;
        }


        private void tsm_surveillance_Click(object sender, EventArgs e)
        {

        }

        //检测设定
        private void tsb_DetectionSet_Click(object sender, EventArgs e)
        {
            DefaultBase.IsSetTong = false;
            ShowDetectionSet();
        }

        private void tms_DetectionSet_Click(object sender, EventArgs e)
        {
            ShowDetectionSet();
        }

        private void tsm_UpdatePassWord_Click(object sender, EventArgs e)
        {
            UpdatePassWord up = new UpdatePassWord();
            up.Show();
            up.TopMost = true;
        }


        /// <summary>解决关闭按钮bug
        /// 
        /// </summary>
        /// <param name="msg"></param>
        protected override void WndProc(ref Message msg)
        {
            try
            {
                const int WM_SYSCOMMAND = 0x0112;
                const int SC_CLOSE = 0xF060;
                if (msg.Msg == WM_SYSCOMMAND && ((int)msg.WParam == SC_CLOSE))
                {
                    // 点击winform右上关闭按钮 
                    this.Dispose();
                    // 加入想要的逻辑处理
                    System.Environment.Exit(0);
                    return;
                }
                base.WndProc(ref msg);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void tsm_sensorSet_Click(object sender, EventArgs e)
        {
            SensorSet ss = new SensorSet(tcpClient);
            ss.Show();
            ss.TopMost = true;
        }

        private void tsb_生成报告_Click(object sender, EventArgs e)
        {
            if (DefaultBase.IsSetTong)
            {
                ExportReport ep = new ExportReport(_tempCode);
                ep.Show();
                ep.TopMost = true;
            }
            else
                MessageBox.Show("请先检测设定", "检测", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }

        /// <summary>
        /// 高压归零
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btn_gyZero_Click(object sender, EventArgs e)
        {
            if (!tcpClient.SendGYBD())
            {
                MessageBox.Show("高压归零异常,请确认服务器连接是否成功!", "设置", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
        }
        /// <summary>
        /// 风速归零
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_fsgl_Click(object sender, EventArgs e)
        {
            var res = tcpClient.SendFSGL();
            if (!res)
            {
                MessageBox.Show("风速归零异常,请确认服务器连接是否成功!", "设置", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
        }

        private void btn_z_Click(object sender, EventArgs e)
        {
            var res = tcpClient.SendZYF();
            if (!res)
            {
                MessageBox.Show("正压阀异常,请确认服务器连接是否成功!", "设置", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }
            Thread.Sleep(3000);
            GetPRVs();
        }

        private void btn_f_Click(object sender, EventArgs e)
        {
            var res = tcpClient.SendFYF();
            if (!res)
            {
                MessageBox.Show("负压阀异常,请确认服务器连接是否成功!", "设置", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }

            Thread.Sleep(3000);
            GetPRVs();
        }

        private void btn_OkFj_Click(object sender, EventArgs e)
        {
            //0-50HZ滚动条 标示0-4000值
            double value = double.Parse(txt_hz.Text) * 80;
            var res = tcpClient.SendFJKZ(value);

            if (!res)
            {
                MessageBox.Show("风机控制异常,请确认服务器连接是否成功!", "控制", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.Show();
            a.TopMost = true;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (!DefaultBase.IsSetTong)
                MessageBox.Show("请先检测设定", "检测", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);

            ComplexAssessment ca = new ComplexAssessment(_tempCode);
            if (DefaultBase.IsOpenComplexAssessment)
            {
                ca.Show();
                ca.TopMost = true;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!DefaultBase.IsSetTong)
            {
                MessageBox.Show("请先检测设定", "检测", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }
            TakePhotos takePhotos = new TakePhotos(_tempCode);
            takePhotos.Show();
            takePhotos.TopMost = true;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SystemManager sm = new SystemManager();
            sm.Show();
            sm.TopMost = true;
        }

        private void 水密监控ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DefaultBase.IsSetTong)
                ShowWatertightDetection();
            else
                MessageBox.Show("请先检测设定", "检测", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }

        private void 气密监控ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DefaultBase.IsSetTong)
                ShowAirtightDetection();
            else
                MessageBox.Show("请先检测设定", "检测", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }

        private void 抗风压监控ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DefaultBase.IsSetTong)
            {
                ShowWindPressure();
            }
            else
                MessageBox.Show("请先检测设定", "检测", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }

        private void tsb_watertight_Click(object sender, EventArgs e)
        {
            if (DefaultBase.IsSetTong)
                ShowAirtightDetection();
            else
                MessageBox.Show("请先检测设定", "检测", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }

        private void tsbwatertight_Click(object sender, EventArgs e)
        {
            if (DefaultBase.IsSetTong)
                ShowWatertightDetection();
            else
                MessageBox.Show("请先检测设定", "检测", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }

        private void tsb_WindPressure_Click(object sender, EventArgs e)
        {
            if (DefaultBase.IsSetTong)
            {
                ShowWindPressure();
            }
            else
                MessageBox.Show("请先检测设定", "检测", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }


        void hsb_DoWork(object sender, DoWorkEventArgs e)
        {
            var IsSeccess = false;
            var diffPress = tcpClient.ReadFJSD(ref IsSeccess);
            if (!IsSeccess) return;

            //需要计算
            e.Result = diffPress;
        }

        void hsb_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null)
                return;
            if (int.Parse(e.Result.ToString()) == 0)
                return;

            Logger.Info("获取-" + e.Result.ToString());
            var value = int.Parse(e.Result.ToString()) / 80;
            this.hsb_WindControl.Value = value * 100;
            txt_hz.Text = value.ToString();
        }


        private void pID设定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PIDManager p = new PIDManager(tcpClient);
            p.Show();
        }

        private void tim_panelValue_Tick(object sender, EventArgs e)
        {
            if (tcpClient.IsTCPLink)
            {
                DataInit();

                using (BackgroundWorker bw = new BackgroundWorker())
                {
                    bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(hsb_RunWorkerCompleted);
                    bw.DoWork += new DoWorkEventHandler(hsb_DoWork);
                    bw.RunWorkerAsync();
                }

            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
