
using Modbus.Device;
using Steema.TeeChart;
using Steema.TeeChart.Styles;
using text.doors.Common;
using text.doors.dal;
using text.doors.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Young.Core.Common;
using text.doors.Model.DataBase;
using text.doors.Default;
using text.doors.Service;

namespace text.doors.Detection
{
    public partial class WatertightDetection : Form
    {
        private static Young.Core.Logger.ILog Logger = Young.Core.Logger.LoggerManager.Current();
        private TCPClient _tcpClient;
        //检验编号
        private string _tempCode = "";
        //当前樘号
        private string _tempTong = "";


        /// <summary>
        /// 水密按钮位置
        /// </summary>
        private PublicEnum.WaterTightPropertyTest? waterTightPropertyTest = null;

        /// <summary>
        /// 操作状态
        /// </summary>
        private bool IsSeccess = false;


        public DateTime dtnow { get; set; }


        public WatertightDetection(TCPClient tcpClient, string tempCode, string tempTong)
        {
            InitializeComponent();
            this._tcpClient = tcpClient;
            this._tempCode = tempCode;
            this._tempTong = tempTong;
            Init();
            // InitTimer();

        }

        public void StopTimer()
        {
            this.tim_PainPic.Enabled = false;
            this.tim_sm.Enabled = false;
            this.tim_getType.Enabled = false;
        }
        public void InitTimer()
        {
            this.tim_PainPic.Enabled = true;
            this.tim_sm.Enabled = true;
            this.tim_getType.Enabled = true;
        }
        private void Init()
        {
            Initial();
            Title();
            SMchartInit();
        }
        #region 数据绑定

        /// <summary>
        /// 绑定水密初始值
        /// </summary>
        private void Initial()
        {
            Model_dt_Settings dt_Settings = new DAL_dt_Settings().GetInfoByCode(_tempCode);
            List<Pressure> pressureList = new List<Pressure>();
            if (dt_Settings.dt_sm_Info != null && dt_Settings.dt_sm_Info.Count > 0)
            {
                var sm = dt_Settings.dt_sm_Info.FindAll(t => t.info_DangH == _tempTong);
                if (sm != null && sm.Count() > 0)
                {
                    #region 绑定
                    var checkDesc = sm[0].sm_PaDesc;
                    var sm_pa = sm[0].sm_Pa;
                    var remark = sm[0].sm_Remark;
                    var method = sm[0].Method;

                    if (method == "稳定加压") {
                        this.rdb_wdjy.Checked = true;
                    }
                    else if (method == "波动加压") {
                        this.rdb_bdjy.Checked = true;
                    }
                    else
                    {
                        this.rdb_wdjy.Checked = true;
                    }
                    var flish = "";
                    var two = "";
                    string[] temp = null;
                    if (!string.IsNullOrWhiteSpace(checkDesc))
                    {
                        if (checkDesc.Contains("〇"))
                        {
                            temp = checkDesc.Split(new char[] { '〇' }, StringSplitOptions.RemoveEmptyEntries);
                            flish = temp[0];
                            two = "〇" + temp[1];
                        }
                        else if (checkDesc.Contains("□"))
                        {
                            temp = checkDesc.Split(new char[] { '□' }, StringSplitOptions.RemoveEmptyEntries);
                            flish = temp[0];
                            two = "□" + temp[1];
                        }
                        else if (checkDesc.Contains("△"))
                        {
                            temp = checkDesc.Split(new char[] { '△' }, StringSplitOptions.RemoveEmptyEntries);
                            flish = temp[0];
                            two = "△" + temp[1];
                        }
                        else if (checkDesc.Contains("▲"))
                        {
                            temp = checkDesc.Split(new char[] { '▲' }, StringSplitOptions.RemoveEmptyEntries);
                            flish = temp[0];
                            two = "▲" + temp[1];
                        }
                        else if (checkDesc.Contains("●"))
                        {
                            temp = checkDesc.Split(new char[] { '●' }, StringSplitOptions.RemoveEmptyEntries);
                            flish = temp[0];
                            two = "●" + temp[1];
                        }
                        else
                        {
                            temp = checkDesc.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            flish = temp[0];
                            two = temp[1];
                        }



                        if (checkDesc.Contains("●") || checkDesc.Contains("▲"))
                        {
                            //if (sm_pa == "0")
                            //{
                            //    cbb_1_0Pa.Text = flish;
                            //    cbb_2_0Pa.Text = two;
                            //}
                            if (sm_pa == "0")
                            {
                                cbb_1_100Pa.Text = flish;
                                cbb_2_100Pa.Text = two;
                            }
                            if (sm_pa == "100")
                            {
                                cbb_1_150Pa.Text = flish;
                                cbb_2_150Pa.Text = two;
                            }
                            if (sm_pa == "150")
                            {
                                cbb_1_200Pa.Text = flish;
                                cbb_2_200Pa.Text = two;
                            }
                            if (sm_pa == "200")
                            {
                                cbb_1_250Pa.Text = flish;
                                cbb_2_250Pa.Text = two;
                            }
                            if (sm_pa == "250")
                            {
                                cbb_1_300Pa.Text = flish;
                                cbb_2_300Pa.Text = two;
                            }
                            if (sm_pa == "300")
                            {
                                cbb_1_350Pa.Text = flish;
                                cbb_2_350Pa.Text = two;
                            }
                            if (sm_pa == "350")
                            {
                                cbb_1_400Pa.Text = flish;
                                cbb_2_400Pa.Text = two;
                            }
                            if (sm_pa == "400")
                            {
                                cbb_1_500Pa.Text = flish;
                                cbb_2_500Pa.Text = two;
                            }
                            if (sm_pa == "500")
                            {
                                cbb_1_600Pa.Text = flish;
                                cbb_2_600Pa.Text = two;
                            }
                            if (sm_pa == "600")
                            {
                                cbb_1_700Pa.Text = flish;
                                cbb_2_700Pa.Text = two;
                            }
                            if (sm_pa == "700")
                            {
                                cbb_1_700Pa.Text = flish;
                                cbb_2_700Pa.Text = two;
                            }
                        }
                        else
                        {
                            if (sm_pa == "0")
                            {
                                cbb_1_0Pa.Text = flish;
                                cbb_2_0Pa.Text = two;
                            }
                            if (sm_pa == "100")
                            {
                                cbb_1_100Pa.Text = flish;
                                cbb_2_100Pa.Text = two;
                            }
                            if (sm_pa == "150")
                            {
                                cbb_1_150Pa.Text = flish;
                                cbb_2_150Pa.Text = two;
                            }
                            if (sm_pa == "200")
                            {
                                cbb_1_200Pa.Text = flish;
                                cbb_2_200Pa.Text = two;
                            }
                            if (sm_pa == "250")
                            {
                                cbb_1_250Pa.Text = flish;
                                cbb_2_250Pa.Text = two;
                            }
                            if (sm_pa == "300")
                            {
                                cbb_1_300Pa.Text = flish;
                                cbb_2_300Pa.Text = two;
                            }
                            if (sm_pa == "350")
                            {
                                cbb_1_350Pa.Text = flish;
                                cbb_2_350Pa.Text = two;
                            }
                            if (sm_pa == "400")
                            {
                                cbb_1_400Pa.Text = flish;
                                cbb_2_400Pa.Text = two;
                            }
                            if (sm_pa == "500")
                            {
                                cbb_1_500Pa.Text = flish;
                                cbb_2_500Pa.Text = two;
                            }
                            if (sm_pa == "600")
                            {
                                cbb_1_600Pa.Text = flish;
                                cbb_2_600Pa.Text = two;
                            }
                            if (sm_pa == "700")
                            {
                                cbb_1_700Pa.Text = flish;
                                cbb_2_700Pa.Text = two;
                            }
                        }


                        //if (checkDesc.Contains("▲") || checkDesc.Contains("●"))
                        //{
                        //    if (sm_pa == "100")
                        //        sm_pa = "0";
                        //    if (sm_pa == "150")
                        //        sm_pa = "100";
                        //    if (sm_pa == "200")
                        //        sm_pa = "150";
                        //    if (sm_pa == "250")
                        //        sm_pa = "200";
                        //    if (sm_pa == "300")
                        //        sm_pa = "250";
                        //    if (sm_pa == "350")
                        //        sm_pa = "300";
                        //    if (sm_pa == "400")
                        //        sm_pa = "350";
                        //    if (sm_pa == "500")
                        //        sm_pa = "400";
                        //    if (sm_pa == "600")
                        //        sm_pa = "500";
                        //    if (sm_pa == "700")
                        //        sm_pa = "600";
                        //}

                    }
                    txt_zgfy.Text = sm_pa;
                    txt_desc.Text = remark;
                    #endregion
                }
            }
        }
        /// <summary>
        /// 绑定设定压力
        /// </summary>
        private void Title()
        {
            lbl_smjc.Text = string.Format("门窗水密性能检测  第{0}号 {1}", this._tempCode, this._tempTong);

            btn_ksbd.Enabled = false;
            btn_tzbd.Enabled = false;
            _tcpClient.qiehuanTab(false);
        }

        #endregion

        #region 图表控制

        /// <summary>
        /// 水密
        /// </summary>
        private void SMchartInit()
        {
            dtnow = DateTime.Now;
            sm_Line.GetVertAxis.SetMinMax(-1100, 1100);
        }

        private void AnimateSeries(Steema.TeeChart.TChart chart, int yl)
        {

            this.sm_Line.Add(DateTime.Now, yl);
            this.tChart_sm.Axes.Bottom.SetMinMax(dtnow, DateTime.Now.AddSeconds(20));
        }

        private void tim_PainPic_Tick(object sender, EventArgs e)
        {
            if (_tcpClient.IsTCPLink)
            {
                var c = _tcpClient.GetCYXS(ref IsSeccess);
                int value = int.Parse(c.ToString());
                if (!IsSeccess)
                {
                    //   MessageBox.Show("获取大气压力异常！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return;
                }
                AnimateSeries(this.tChart_sm, value);
            }
        }
        #endregion


        private void tChart1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.chart_cms_sm_click.Show(MousePosition.X, MousePosition.Y);
            }
        }

        private void 导出图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //todo
            //this.tChart_qm.Export.ShowExportDialog();
        }

        /// <summary>
        /// 急停
        /// </summary>
        private void Stop()
        {
            var res = _tcpClient.Stop();
            if (!res)
                MessageBox.Show("急停异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }

        /// <summary>
        /// 停止波动
        /// </summary>
        private void StopBoDong()
        {
            var res = _tcpClient.StopBoDong();
            if (!res)
                MessageBox.Show("急停异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }


        private void btn_next_Click(object sender, EventArgs e)
        {

            if (this.rdb_bdjy.Checked == true)
            {
                if (lbl_sdyl.Text == "350")
                {
                    lbl_sdyl.Text = "0";
                    Stop();
                    this.btn_ready.Enabled = true;
                    this.btn_start.Enabled = true;
                    this.btn_next.Enabled = true;
                    waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Stop;
                    return;
                }
            }
            else
            {
                if (lbl_sdyl.Text == "700")
                {
                    lbl_sdyl.Text = "0";
                    Stop();
                    this.btn_ready.Enabled = true;
                    this.btn_start.Enabled = true;
                    this.btn_next.Enabled = true;
                    waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Stop;
                    return;
                }
            }


            var res = _tcpClient.SendSMXXYJ();
            if (!res)
            {
                MessageBox.Show("设置水密性下一级异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Next;
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            lbl_sdyl.Text = "0";
            // StopBoDong();
            Stop();

            this.btn_ready.Enabled = true;
            this.btn_start.Enabled = true;
            this.btn_next.Enabled = true;
            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Stop;
        }


        #region 水密性能检测按钮事件
        private void btn_ready_Click(object sender, EventArgs e)
        {
            double yl = _tcpClient.GetSMYBSDYL(ref IsSeccess, "SMYB");
            if (!IsSeccess)
            {
                //MessageBox.Show("读取设定值异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }
            lbl_sdyl.Text = yl.ToString();

            this.btn_ready.Enabled = false;
            this.btn_start.Enabled = false;
            this.btn_next.Enabled = false;
            this.btn_next.Enabled = false;

            var res = _tcpClient.SetSMYB();
            if (!res)
            {
                MessageBox.Show("水密预备异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }

            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Ready;

        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            this.btn_start.Enabled = false;
            this.btn_next.Enabled = true;
            this.tim_upNext.Enabled = true;
            this.btn_ready.Enabled = false;
            var res = _tcpClient.SendSMXKS();
            if (!res)
            {
                MessageBox.Show("水密开始异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }

            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Start;
        }
        /// <summary>
        /// 依次加压
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        private bool ycjyType = true;

        private void btn_upKpa_Click(object sender, EventArgs e)
        {
            ycjyType = (ycjyType ? false : true);

            if (!ycjyType)
            {
                btn_upKpa.Text = "停止";
            }
            else
            {
                btn_upKpa.Text = "依次加压";

                //lbl_sdyl.Text = "0";
                lbl_sdyl.Text = txt_ycjy.Text;

                Stop();
                this.btn_ready.Enabled = true;
                this.btn_start.Enabled = true;
                this.btn_next.Enabled = true;
                waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Stop;
                return;
            }

            tim_upNext.Enabled = false;

            var value = int.Parse(txt_ycjy.Text);
            var res = _tcpClient.SendSMYCJY(value);
            if (!res)
            {
                MessageBox.Show("设置水密依次加压异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
            lbl_sdyl.Text = value.ToString();
            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.CycleLoading;

        }
        #endregion

        private void tim_upNext_Tick(object sender, EventArgs e)
        {
            if (waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.Stop)
            {
                return;
            }

            if (_tcpClient.IsTCPLink)
            {
                string TEMP = "";
                if (waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.Ready)
                    TEMP = "SMYB";
                if (waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.CycleLoading)
                    TEMP = "SMKS";
                if (waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.Start)
                    TEMP = "SMKS";
                if (waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.Next)
                    TEMP = "XYJ";

                double yl = _tcpClient.GetSMYBSDYL(ref IsSeccess, TEMP);

                if (!IsSeccess)
                {
                    // MessageBox.Show("读取设定值异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return;
                }
                lbl_sdyl.Text = yl.ToString();


                if (waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.Next ||
                 waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.Start ||
                 waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.Next ||
                 waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.SrartBD)
                {
                    if (this.rdb_bdjy.Checked == true)
                    {
                        lbl_max.Visible = true;

                        var minVal = 0;
                        var maxVal = 0;

                        _tcpClient.GetCYXS_BODONG(ref IsSeccess, ref minVal, ref maxVal);

                        lbl_sdyl.Text = minVal.ToString();
                        lbl_max.Text = maxVal.ToString();
                    }
                }

            }
        }

        #region -- 水密选择

        /// <summary>
        /// 位置
        /// </summary>
        private string CheckPosition = "";

        /// <summary>
        /// 问题
        /// </summary>
        private string CheckProblem = "";

        /// <summary>
        /// 数值
        /// </summary>
        private int CheckValue = 0;

        private void cbb_2_0Pa_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(cbb_1_0Pa.Text))
            {
                MessageBox.Show("请选择位置", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                cbb_2_0Pa.Text = "";
                CheckProblem = "";
                CheckPosition = "";
                CheckValue = 0;
                return;
            }

            CheckPosition = cbb_1_0Pa.Text;
            CheckProblem = cbb_2_0Pa.Text;
            CheckValue = 0;

            txt_zgfy.Text = CheckValue.ToString();
        }

        private void cbb_2_100Pa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbb_1_100Pa.Text))
            {
                MessageBox.Show("请选择位置", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                cbb_2_100Pa.Text = "";
                CheckProblem = "";
                CheckPosition = "";
                CheckValue = 0;
                return;
            }
            CheckPosition = cbb_1_100Pa.Text;
            CheckProblem = cbb_2_100Pa.Text;
            CheckValue = 100;

            if (cbb_2_100Pa.Text.Contains("▲") || cbb_2_100Pa.Text.Contains("●"))
            {
                CheckValue = 0;
            }

            txt_zgfy.Text = CheckValue.ToString();
        }

        private void cbb_2_150Pa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbb_1_150Pa.Text))
            {
                MessageBox.Show("请选择位置", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                cbb_2_150Pa.Text = "";
                CheckProblem = "";
                CheckPosition = "";
                CheckValue = 0;
                return;
            }
            CheckPosition = cbb_1_150Pa.Text;
            CheckProblem = cbb_2_150Pa.Text;
            CheckValue = 150;

            if (cbb_2_150Pa.Text.Contains("▲") || cbb_2_150Pa.Text.Contains("●"))
            {
                CheckValue = 100;
            }
            txt_zgfy.Text = CheckValue.ToString();
        }

        private void cbb_2_200Pa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbb_1_200Pa.Text))
            {
                MessageBox.Show("请选择位置", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                cbb_2_200Pa.Text = "";
                CheckProblem = "";
                CheckPosition = "";
                CheckValue = 0;
                return;
            }
            CheckPosition = cbb_1_200Pa.Text;
            CheckProblem = cbb_2_200Pa.Text;
            CheckValue = 200;

            if (cbb_2_200Pa.Text.Contains("▲") || cbb_2_200Pa.Text.Contains("●"))
            {
                CheckValue = 150;
            }
            txt_zgfy.Text = CheckValue.ToString();
        }

        private void cbb_2_250Pa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbb_1_250Pa.Text))
            {
                MessageBox.Show("请选择位置", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                cbb_2_250Pa.Text = "";
                CheckProblem = "";
                CheckPosition = "";
                CheckValue = 0;
                return;
            }
            CheckPosition = cbb_1_250Pa.Text;
            CheckProblem = cbb_2_250Pa.Text;
            CheckValue = 250;

            if (cbb_2_250Pa.Text.Contains("▲") || cbb_2_250Pa.Text.Contains("●"))
            {
                CheckValue = 200;
            }
            txt_zgfy.Text = CheckValue.ToString();
        }

        private void cbb_2_300Pa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbb_1_300Pa.Text))
            {
                MessageBox.Show("请选择位置", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                cbb_2_300Pa.Text = "";
                CheckProblem = "";
                CheckPosition = "";
                CheckValue = 0;
                return;
            }
            CheckPosition = cbb_1_300Pa.Text;
            CheckProblem = cbb_2_300Pa.Text;
            CheckValue = 300;

            if (cbb_2_300Pa.Text.Contains("▲") || cbb_2_300Pa.Text.Contains("●"))
            {
                CheckValue = 250;
            }
            txt_zgfy.Text = CheckValue.ToString();
        }

        private void cbb_2_350Pa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbb_1_350Pa.Text))
            {
                MessageBox.Show("请选择位置", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                cbb_2_350Pa.Text = "";
                CheckProblem = "";
                CheckPosition = "";
                CheckValue = 0;
                return;
            }
            CheckPosition = cbb_1_350Pa.Text;
            CheckProblem = cbb_2_350Pa.Text;
            CheckValue = 350;

            if (cbb_2_350Pa.Text.Contains("▲") || cbb_2_350Pa.Text.Contains("●"))
            {
                CheckValue = 300;
            }
            txt_zgfy.Text = CheckValue.ToString();
        }

        private void cbb_2_400Pa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbb_1_400Pa.Text))
            {
                MessageBox.Show("请选择位置", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                cbb_2_400Pa.Text = "";
                CheckProblem = "";
                CheckPosition = "";
                CheckValue = 0;
                return;
            }
            CheckPosition = cbb_1_400Pa.Text;
            CheckProblem = cbb_2_400Pa.Text;
            CheckValue = 400;

            if (cbb_2_400Pa.Text.Contains("▲") || cbb_2_400Pa.Text.Contains("●"))
            {
                CheckValue = 350;
            }
            txt_zgfy.Text = CheckValue.ToString();
        }

        private void cbb_2_500Pa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbb_1_500Pa.Text))
            {
                MessageBox.Show("请选择位置", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                cbb_2_500Pa.Text = "";
                CheckProblem = "";
                CheckPosition = "";
                CheckValue = 0;
                return;
            }
            CheckPosition = cbb_1_500Pa.Text;
            CheckProblem = cbb_2_500Pa.Text;
            CheckValue = 500;

            if (cbb_2_500Pa.Text.Contains("▲") || cbb_2_500Pa.Text.Contains("●"))
            {
                CheckValue = 400;
            }
            txt_zgfy.Text = CheckValue.ToString();
        }

        private void cbb_2_600Pa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbb_1_600Pa.Text))
            {
                MessageBox.Show("请选择位置", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                cbb_2_600Pa.Text = "";
                CheckProblem = "";
                CheckPosition = "";
                CheckValue = 0;
                return;
            }
            CheckPosition = cbb_1_600Pa.Text;
            CheckProblem = cbb_2_600Pa.Text;
            CheckValue = 600;

            if (cbb_2_600Pa.Text.Contains("▲") || cbb_2_600Pa.Text.Contains("●"))
            {
                CheckValue = 500;
            }
            txt_zgfy.Text = CheckValue.ToString();
        }

        private void cbb_2_700Pa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbb_1_700Pa.Text))
            {
                MessageBox.Show("请选择位置", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                cbb_2_700Pa.Text = "";
                CheckProblem = "";
                CheckPosition = "";
                CheckValue = 0;
                return;
            }
            CheckPosition = cbb_1_700Pa.Text;
            CheckProblem = cbb_2_700Pa.Text;
            CheckValue = 700;

            if (cbb_2_700Pa.Text.Contains("▲") || cbb_2_700Pa.Text.Contains("●"))
            {
                CheckValue = 600;
            }
            txt_zgfy.Text = CheckValue.ToString();
        }

        private void txt_zgfy_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                int zgfy = int.Parse(txt_zgfy.Text);
                CheckValue = zgfy;
            }
            catch
            {
                MessageBox.Show("请输入数字", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
        }

        /// <summary>
        /// 水密数据处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_2sjcl_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CheckPosition) || string.IsNullOrWhiteSpace(CheckProblem))
            {
                MessageBox.Show("选择失去焦点，请重新选择检测记录！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }
            Model_dt_sm_Info model = new Model_dt_sm_Info();
            model.dt_Code = _tempCode;
            model.info_DangH = _tempTong;
            model.sm_Pa = CheckValue.ToString();
            model.sm_PaDesc = CheckPosition + "," + CheckProblem;
            model.sm_Remark = txt_desc.Text;

            if (this.rdb_bdjy.Checked == true)
            {
                model.Method = "波动加压";
            }
            else
            {
                model.Method = "稳定加压";
            }


            if (new DAL_dt_sm_Info().Add(model))
            {
                MessageBox.Show("处理成功！", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
        }
        #endregion

        private void tChart_sm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.chart_cms_sm_click.Show(MousePosition.X, MousePosition.Y);
            }
        }

        private void export_image_sm_Click(object sender, EventArgs e)
        {
            this.tChart_sm.Export.ShowExportDialog();
        }

        private void tim_sm_Tick(object sender, EventArgs e)
        {
            if (_tcpClient.IsTCPLink)
            {
                lbl_max.Visible = false;
                var value = int.Parse(_tcpClient.GetCYXS(ref IsSeccess).ToString());
                if (!IsSeccess)
                {
                    //todo
                    // MessageBox.Show("获取差压异常--水密！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return;
                }
                lbldqyl.Text = value.ToString();

                if (waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.Next ||
                    waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.Start ||
                    waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.Next ||
                    waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.SrartBD)
                {
                    if (this.rdb_bdjy.Checked == true)
                    {
                        lbl_max.Visible = true;

                        var minVal = 0;
                        var maxVal = 0;

                        _tcpClient.GetCYXS_BODONG(ref IsSeccess, ref minVal, ref maxVal);

                        lbl_sdyl.Text = minVal.ToString();
                        lbl_max.Text = maxVal.ToString();
                    }
                }
            }
        }

        private void btn_ksbd_Click(object sender, EventArgs e)
        {

            int minValue = -1;
            int maxValue = -1;

            int.TryParse(txt_minValue.Text, out maxValue);

            int.TryParse(txt_maxValue.Text, out minValue);

            if (minValue == 0 || maxValue == 0)
            {
                MessageBox.Show("上线-下线压力请设置大于零数字", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }

            this.btn_ksbd.Enabled = false;
            this.btn_tzbd.Enabled = true;
            this.btn_ready.Enabled = false;
            this.btn_start.Enabled = false;
            this.btn_next.Enabled = false;

            tim_upNext.Enabled = false;

            var res = _tcpClient.SendBoDongksjy(maxValue, minValue);
            if (!res)
            {
                MessageBox.Show("水密波动开始异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }

            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.SrartBD;
        }

        private void btn_tzbd_Click(object sender, EventArgs e)
        {
            lbl_sdyl.Text = "0";
            StopBoDong();
            // Stop();
            this.btn_tzbd.Enabled = false;

            Thread.Sleep(5000);

            this.btn_ksbd.Enabled = true;
            this.btn_ready.Enabled = true;
            this.btn_start.Enabled = true;
            this.btn_next.Enabled = true;
            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.StopBD;
        }

        private void tim_getType_Tick(object sender, EventArgs e)
        {
            if (_tcpClient.IsTCPLink)
            {
                //水密预备
                if (waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.Ready)
                {
                    int value = _tcpClient.GetSMYBJS(ref IsSeccess);

                    if (!IsSeccess)
                    {
                        return;
                    }
                    if (value == 3)
                    {
                        waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Stop;
                        lbl_sdyl.Text = "0";

                        this.btn_ready.Enabled = true;
                        this.btn_start.Enabled = true;
                        this.btn_next.Enabled = true;
                    }
                }

                //if (this.rdb_bdjy.Checked == true)
                //{
                //    if (waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.Start || waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.CycleLoading)
                //    {
                //        int value = _tcpClient.GetSMXYJJS_BD(ref IsSeccess);
                //        if (!IsSeccess)
                //        {
                //            return;
                //        }
                //        if (value == 9)
                //        {
                //            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Stop;
                //            lbl_sdyl.Text = "0";

                //            this.btn_upKpa.Enabled = true;
                //            this.btn_ready.Enabled = true;
                //            this.btn_start.Enabled = true;
                //            this.btn_next.Enabled = true;
                //        }
                //    }
                //}
                //else
                //{
                //    //开始- 依次加压
                //    if (waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.Start || waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.CycleLoading)
                //    {
                //        int value = _tcpClient.GetSMYBJS(ref IsSeccess);
                //        if (!IsSeccess)
                //        {
                //            return;
                //        }
                //        if (value == 3)
                //        {
                //            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Stop;
                //            lbl_sdyl.Text = "0";

                //            this.btn_upKpa.Enabled = true;
                //            this.btn_ready.Enabled = true;
                //            this.btn_start.Enabled = true;
                //            this.btn_next.Enabled = true;
                //        }
                //    }
                //}

                ////波动加压
                //if (waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.SrartBD)
                //{
                //    int value = _tcpClient.GetSMJS_BD(ref IsSeccess);

                //    if (!IsSeccess)
                //    {
                //        return;
                //    }
                //    if (value >= 8995)
                //    {
                //        waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Stop;
                //        lbl_sdyl.Text = "0";
                //        lbl_max.Text = "0";
                //        this.btn_ready.Enabled = true;
                //        this.btn_start.Enabled = true;
                //        this.btn_next.Enabled = true;
                //    }
                //}
            }
        }

        private void rdb_bdjy_CheckedChanged(object sender, EventArgs e)
        {
            btn_upKpa.Enabled = false;
            btn_ksbd.Enabled = true;
            btn_tzbd.Enabled = true;

            if (this.rdb_bdjy.Checked == true)
            {
                _tcpClient.qiehuanTab(true);
            }
            else
            {
                _tcpClient.qiehuanTab(false);
            }
        }

        private void rdb_wdjy_CheckedChanged(object sender, EventArgs e)
        {
            btn_upKpa.Enabled = true;
            btn_ksbd.Enabled = false;
            btn_tzbd.Enabled = false;
            if (this.rdb_bdjy.Checked == true)
            {
                _tcpClient.qiehuanTab(true);
            }
            else
            {
                _tcpClient.qiehuanTab(false);
            }

        }
    }
}
