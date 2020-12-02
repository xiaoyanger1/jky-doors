
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
    public partial class RealTimeSurveillance : Form
    {
        private static Young.Core.Logger.ILog Logger = Young.Core.Logger.LoggerManager.Current();
        private TCPClient _tcpClient;
        //检验编号
        private string _tempCode = "";
        //当前樘号
        private string _tempTong = "";

        /// <summary>
        /// 气密数据载体
        /// </summary>
        Pressure pressure = new Pressure();
        /// <summary>
        /// 当前选项卡状态
        /// </summary>
        private PublicEnum.SystemItem? systemItem = PublicEnum.SystemItem.Airtight;

        /// <summary>
        /// 气密数据位置
        /// </summary>
        private PublicEnum.AirtightPropertyTest? airtightPropertyTest = null;

        /// <summary>
        /// 水密按钮位置
        /// </summary>
        private PublicEnum.WaterTightPropertyTest? waterTightPropertyTest = null;

        /// <summary>
        /// 分级指标
        /// </summary>
        double zFc = 0, fFc = 0, zMj = 0, fMj = 0;

        /// <summary>
        /// 操作状态
        /// </summary>
        private bool IsSeccess = false;

        private bool IsFirst = true;


        public DateTime dtnow { get; set; }

        public RealTimeSurveillance(TCPClient tcpClient, string tempCode, string tempTong, int type = 1)
        {
            InitializeComponent();
            this._tcpClient = tcpClient;
            this._tempCode = tempCode;
            this._tempTong = tempTong;

            //if (type == 1)
            //{
            //    tc_RealTimeSurveillance.TabPages["page_airtight"].Hide();
            //    tc_RealTimeSurveillance.TabPages["page_watertight"].Show();
            //}
            //else if (type == 2)
            //{
            //    tc_RealTimeSurveillance.TabPages["page_watertight"].Hide();
            //    tc_RealTimeSurveillance.TabPages["page_airtight"].Show();
            //}

            pressure = new Pressure();
            Init();
        }

        private void Init()
        {
            BindWindSpeedBase();
            BindLevelIndex();
            BindFlowBase();
            BindSetPressure();
            QMchartInit();
        }

        /// <summary>
        /// 绑定设定压力
        /// </summary>
        private void BindSetPressure()
        {
            lbl_title.Text = string.Format("门窗气密性能检测  第{0}号 {1}", this._tempCode, this._tempTong);
            lbl_smjc.Text = string.Format("门窗水密性能检测  第{0}号 {1}", this._tempCode, this._tempTong);
        }

        #region 数据绑定

        /// <summary>
        /// 获取流量数据
        /// </summary>
        /// <returns></returns>
        public List<Pressure> GetPressureFlow()
        {
            List<Pressure> pressureList = new List<Pressure>();
            Formula slopeCompute = new Formula();
            for (int i = 0; i < 3; i++)
            {
                Pressure model = new Pressure();
                model.PressurePa = int.Parse(this.dgv_WindSpeed.Rows[i].Cells["PressurePa"].Value.ToString());
                model.Pressure_Z = slopeCompute.MathFlow(double.Parse(this.dgv_WindSpeed.Rows[i].Cells["Pressure_Z"].Value.ToString()));
                model.Pressure_Z_Z = slopeCompute.MathFlow(double.Parse(this.dgv_WindSpeed.Rows[i].Cells["Pressure_Z_Z"].Value.ToString()));
                model.Pressure_F = slopeCompute.MathFlow(double.Parse(this.dgv_WindSpeed.Rows[i].Cells["Pressure_F"].Value.ToString()));
                model.Pressure_F_Z = slopeCompute.MathFlow(double.Parse(this.dgv_WindSpeed.Rows[i].Cells["Pressure_F_Z"].Value.ToString()));
                pressureList.Add(model);
            }
            return pressureList;
        }

        /// <summary>
        /// 绑定流量
        /// </summary>
        private void BindFlowBase()
        {
            dgv_ll.DataSource = GetPressureFlow();

            dgv_ll.Height = 115;
            dgv_ll.RowHeadersVisible = false;
            dgv_ll.AllowUserToResizeColumns = false;
            dgv_ll.AllowUserToResizeRows = false;
            dgv_ll.Columns[0].HeaderText = "压力Pa";
            dgv_ll.Columns[0].Width = 37;
            dgv_ll.Columns[0].ReadOnly = true;
            dgv_ll.Columns[0].DataPropertyName = "PressurePa";
            dgv_ll.Columns[1].HeaderText = "正压附加";
            dgv_ll.Columns[1].Width = 55;
            dgv_ll.Columns[1].DataPropertyName = "Pressure_Z";
            dgv_ll.Columns[2].HeaderText = "正压总的";
            dgv_ll.Columns[2].Width = 55;
            dgv_ll.Columns[2].DataPropertyName = "Pressure_Z_Z";
            dgv_ll.Columns[3].HeaderText = "负压附加";
            dgv_ll.Columns[3].Width = 55;
            dgv_ll.Columns[3].DataPropertyName = "Pressure_F";
            dgv_ll.Columns[4].HeaderText = "负压总的";
            dgv_ll.Columns[4].Width = 55;
            dgv_ll.Columns[4].DataPropertyName = "Pressure_F_Z";

            dgv_ll.Columns["Pressure_Z"].DefaultCellStyle.Format = "N2";
            dgv_ll.Columns["Pressure_Z_Z"].DefaultCellStyle.Format = "N2";
            dgv_ll.Columns["Pressure_F"].DefaultCellStyle.Format = "N2";
            dgv_ll.Columns["Pressure_F_Z"].DefaultCellStyle.Format = "N2";
        }




        /// <summary>
        /// 绑定风速
        /// </summary>
        private void BindWindSpeedBase()
        {
            //todo
            // Model_dt_Settings dt_Settings = new DAL_dt_Settings().Getdt_SettingsResByCode(_tempCode);
            Model_dt_Settings dt_Settings = new DAL_dt_Settings().GetInfoByCode(_tempCode);
            List<Pressure> pressureList = new List<Pressure>();
            if (dt_Settings.dt_qm_Info != null && dt_Settings.dt_qm_Info.Count > 0)
            {
                var qm = dt_Settings.dt_qm_Info.FindAll(t => t.info_DangH == _tempTong && string.IsNullOrWhiteSpace(t.qm_j_f_zd100) == false).OrderBy(t => t.info_DangH);
                //是否首次加载
                if (IsFirst && (qm != null && qm.Count() > 0))
                {
                    gv_list.Enabled = false;
                    foreach (var item in qm)
                    {
                        Pressure model1 = new Pressure();
                        model1.Pressure_F = string.IsNullOrWhiteSpace(item.qm_s_f_fj100) ? 0 : double.Parse(item.qm_s_f_fj100);
                        model1.Pressure_F_Z = string.IsNullOrWhiteSpace(item.qm_s_f_zd100) ? 0 : double.Parse(item.qm_s_f_zd100);

                        model1.Pressure_Z = string.IsNullOrWhiteSpace(item.qm_s_z_fj100) ? 0 : double.Parse(item.qm_s_z_fj100);
                        model1.Pressure_Z_Z = string.IsNullOrWhiteSpace(item.qm_s_z_zd100) ? 0 : double.Parse(item.qm_s_z_zd100);
                        model1.PressurePa = 100;
                        pressureList.Add(model1);

                        Pressure model2 = new Pressure();
                        model2.Pressure_F = string.IsNullOrWhiteSpace(item.qm_s_f_fj150) ? 0 : double.Parse(item.qm_s_f_fj150);
                        model2.Pressure_F_Z = string.IsNullOrWhiteSpace(item.qm_s_f_zd150) ? 0 : double.Parse(item.qm_s_f_zd150);

                        model2.Pressure_Z = string.IsNullOrWhiteSpace(item.qm_s_z_fj150) ? 0 : double.Parse(item.qm_s_z_fj150);
                        model2.Pressure_Z_Z = string.IsNullOrWhiteSpace(item.qm_s_z_zd150) ? 0 : double.Parse(item.qm_s_z_zd150);
                        model2.PressurePa = 150;
                        pressureList.Add(model2);

                        Pressure model3 = new Pressure();
                        model3.Pressure_F = string.IsNullOrWhiteSpace(item.qm_j_f_fj100) ? 0 : double.Parse(item.qm_j_f_fj100);
                        model3.Pressure_F_Z = string.IsNullOrWhiteSpace(item.qm_j_f_zd100) ? 0 : double.Parse(item.qm_j_f_zd100);

                        model3.Pressure_Z = string.IsNullOrWhiteSpace(item.qm_j_z_fj100) ? 0 : double.Parse(item.qm_j_z_fj100);
                        model3.Pressure_Z_Z = string.IsNullOrWhiteSpace(item.qm_j_z_zd100) ? 0 : double.Parse(item.qm_j_z_zd100);
                        model3.PressurePa = 100;
                        pressureList.Add(model3);
                    }
                }
                else
                {
                    pressureList = pressure.GetPressure();
                }
                //水密
                var sm = dt_Settings.dt_sm_Info.FindAll(t => t.info_DangH == _tempTong);
                if (sm != null && sm.Count() > 0)
                {
                    var checkDesc = sm[0].sm_PaDesc;
                    var sm_pa = sm[0].sm_Pa;
                    var remark = sm[0].sm_Remark;

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

                        if (checkDesc.Contains("▲") || checkDesc.Contains("●"))
                        {
                            if (sm_pa == "100")
                                sm_pa = "0";
                            if (sm_pa == "150")
                                sm_pa = "100";
                            if (sm_pa == "200")
                                sm_pa = "150";
                            if (sm_pa == "250")
                                sm_pa = "200";
                            if (sm_pa == "300")
                                sm_pa = "250";
                            if (sm_pa == "350")
                                sm_pa = "300";
                            if (sm_pa == "400")
                                sm_pa = "350";
                            if (sm_pa == "500")
                                sm_pa = "400";
                            if (sm_pa == "600")
                                sm_pa = "500";
                            if (sm_pa == "700")
                                sm_pa = "600";
                        }

                    }
                    txt_zgfy.Text = sm_pa;
                    txt_desc.Text = remark;
                }
            }

            dgv_WindSpeed.DataSource = pressureList;
            dgv_WindSpeed.Height = 115;
            dgv_WindSpeed.RowHeadersVisible = false;
            dgv_WindSpeed.AllowUserToResizeColumns = false;
            dgv_WindSpeed.AllowUserToResizeRows = false;
            dgv_WindSpeed.Columns[0].HeaderText = "压力Pa";
            dgv_WindSpeed.Columns[0].Width = 37;
            dgv_WindSpeed.Columns[0].ReadOnly = true;
            dgv_WindSpeed.Columns[0].DataPropertyName = "PressurePa";
            dgv_WindSpeed.Columns[1].HeaderText = "正压附加";
            dgv_WindSpeed.Columns[1].Width = 55;
            dgv_WindSpeed.Columns[1].DataPropertyName = "Pressure_Z";
            dgv_WindSpeed.Columns[2].HeaderText = "正压总的";
            dgv_WindSpeed.Columns[2].Width = 55;
            dgv_WindSpeed.Columns[2].DataPropertyName = "Pressure_Z_Z";
            dgv_WindSpeed.Columns[3].HeaderText = "负压附加";
            dgv_WindSpeed.Columns[3].Width = 55;
            dgv_WindSpeed.Columns[3].DataPropertyName = "Pressure_F";
            dgv_WindSpeed.Columns[4].HeaderText = "负压总的";
            dgv_WindSpeed.Columns[4].Width = 55;
            dgv_WindSpeed.Columns[4].DataPropertyName = "Pressure_F_Z";


            dgv_WindSpeed.Columns["Pressure_Z"].DefaultCellStyle.Format = "N2";
            dgv_WindSpeed.Columns["Pressure_Z_Z"].DefaultCellStyle.Format = "N2";
            dgv_WindSpeed.Columns["Pressure_F"].DefaultCellStyle.Format = "N2";
            dgv_WindSpeed.Columns["Pressure_F_Z"].DefaultCellStyle.Format = "N2";
        }


        /// <summary>
        /// 绑定分级指标
        /// </summary>
        private void BindLevelIndex()
        {
            GetDatabaseLevelIndex();
            dgv_levelIndex.DataSource = GetLevelIndex();
            dgv_levelIndex.Height = 69;
            dgv_levelIndex.RowHeadersVisible = false;
            dgv_levelIndex.AllowUserToResizeColumns = false;
            dgv_levelIndex.AllowUserToResizeRows = false;
            dgv_levelIndex.Columns[0].HeaderText = "渗透量";
            dgv_levelIndex.Columns[0].Width = 96;
            dgv_levelIndex.Columns[0].ReadOnly = true;
            dgv_levelIndex.Columns[0].DataPropertyName = "Quantity";
            dgv_levelIndex.Columns[1].HeaderText = "正压";
            dgv_levelIndex.Columns[1].Width = 80;
            dgv_levelIndex.Columns[1].DataPropertyName = "PressureZ";
            dgv_levelIndex.Columns[2].HeaderText = "负压";
            dgv_levelIndex.Columns[2].Width = 80;
            dgv_levelIndex.Columns[2].DataPropertyName = "PressureF";

            dgv_levelIndex.Columns["PressureZ"].DefaultCellStyle.Format = "N2";
            dgv_levelIndex.Columns["PressureF"].DefaultCellStyle.Format = "N2";
        }

        /// <summary>
        /// 获取分级指标
        /// </summary>
        /// <returns></returns>
        private List<LevelIndex> GetLevelIndex()
        {
            return new List<LevelIndex>()
            {
                new  LevelIndex(){ Quantity="单位缝长",  PressureZ =Math.Round(zFc,2), PressureF  =Math.Round(fFc,2)},
                new  LevelIndex(){ Quantity="单位面积",  PressureZ =Math.Round(zMj,2), PressureF  =Math.Round(fMj,2)}
            };
        }
        #endregion

        #region 图表控制
        /// <summary>
        /// 风速图标
        /// </summary>
        private void QMchartInit()
        {
            dtnow = DateTime.Now;
            qm_Line.GetVertAxis.SetMinMax(-300, 300);
        }

        /// <summary>
        /// 水密
        /// </summary>
        private void SMchartInit()
        {
            dtnow = DateTime.Now;
            sm_Line.GetVertAxis.SetMinMax(-1000, 1000);
        }

        private void AnimateSeries(Steema.TeeChart.TChart chart, int yl)
        {
            if (systemItem == PublicEnum.SystemItem.Airtight)
            {
                this.qm_Line.Add(DateTime.Now, yl);
                this.tChart_qm.Axes.Bottom.SetMinMax(dtnow, DateTime.Now.AddSeconds(20));
            }
            else if (systemItem == PublicEnum.SystemItem.Watertight)
            {
                this.sm_Line.Add(DateTime.Now, yl);
                this.tChart_sm.Axes.Bottom.SetMinMax(dtnow, DateTime.Now.AddSeconds(20));
            }
        }

        /// <summary>
        /// 确定当前读取的压力状态
        /// </summary>
        private PublicEnum.Kpa_Level? kpa_Level = null;

        /// <summary>
        /// 差压读取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tim_qm_Tick(object sender, EventArgs e)
        {
            if (_tcpClient.IsTCPLink)
            {
                var value = int.Parse(_tcpClient.GetCYXS(ref IsSeccess).ToString());
                if (!IsSeccess)
                {
                    //todo
                    //MessageBox.Show("获取差压异常--气密！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return;
                }
                //气密水密
                lbl_dqyl.Text = value.ToString();
                lbldqyl.Text = value.ToString();

                //读取设定值
                if (airtightPropertyTest == PublicEnum.AirtightPropertyTest.ZStart)
                {
                    double yl = _tcpClient.GetZYYBYLZ(ref IsSeccess, "ZYKS");
                    if (!IsSeccess)
                    {
                        MessageBox.Show("获取正压预备异常！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                        return;
                    }
                    lbl_setYL.Text = yl.ToString();
                }
                else if (airtightPropertyTest == PublicEnum.AirtightPropertyTest.FStart)
                {
                    double yl = _tcpClient.GetZYYBYLZ(ref IsSeccess, "FYKS");
                    if (!IsSeccess)
                    {
                        MessageBox.Show("获取负压开始异常！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                        return;
                    }
                    lbl_setYL.Text = "-" + yl.ToString();
                }
                else if (airtightPropertyTest == PublicEnum.AirtightPropertyTest.Stop)
                {
                    lbl_setYL.Text = "0";
                }

                if (IsStart)
                {
                    if (this.tim_Top10.Enabled == false)
                        SetCurrType(value);
                }
            }

        }

        private void tim_PainPic_Tick(object sender, EventArgs e)
        {
            if (_tcpClient.IsTCPLink)
            {
                var c = _tcpClient.GetCYXS(ref IsSeccess);
                int value = int.Parse(c.ToString());
                if (!IsSeccess)
                {
                    //todo
                    // MessageBox.Show("获取差压异常--画图！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return;
                }

                if (systemItem == PublicEnum.SystemItem.Airtight)
                {
                    AnimateSeries(this.tChart_qm, value);
                }
                else if (systemItem == PublicEnum.SystemItem.Watertight)
                {
                    AnimateSeries(this.tChart_sm, value);
                }
            }
        }

        int index = 0;
        private void tim_Top10_Tick(object sender, EventArgs e)
        {
            gv_list.Enabled = true;

            var cyvalue = _tcpClient.GetCYXS(ref IsSeccess);
            if (!IsSeccess)
            {
                //todo
                //  MessageBox.Show("获取差压异常--取前十！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }

            index++;
            if (index > 8)
            {
                //标记计时结束
                if (kpa_Level == PublicEnum.Kpa_Level.liter100)
                {
                    if (cyvalue > 0)
                        Z_S_100Stop = false;
                    else
                        F_S_100Stop = false;
                }
                else if (kpa_Level == PublicEnum.Kpa_Level.liter150)
                {
                    if (cyvalue > 0)
                        Z_S_150Stop = false;
                    else
                        F_S_150Stop = false;
                }
                else if (kpa_Level == PublicEnum.Kpa_Level.drop100)
                {
                    if (cyvalue > 0)
                        Z_J_100Stop = false;
                    else
                        F_J_100Stop = false;
                }

                this.tim_Top10.Enabled = false;
                index = 0;
                gv_list.Enabled = false;
                return;
            }


            //获取风速
            var fsvalue = _tcpClient.GetFSXS(ref IsSeccess);
            if (!IsSeccess)
            {
                MessageBox.Show("获取风速异常！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }

            if (rdb_fjstl.Checked)
            {
                if (kpa_Level == PublicEnum.Kpa_Level.liter100)
                {
                    if (cyvalue > 0)
                        pressure.AddZYFJ(fsvalue, PublicEnum.Kpa_Level.liter100);
                    else
                        pressure.AddFYFJ(fsvalue, PublicEnum.Kpa_Level.liter100);
                }
                else if (kpa_Level == PublicEnum.Kpa_Level.liter150)
                {
                    if (cyvalue > 0)
                        pressure.AddZYFJ(fsvalue, PublicEnum.Kpa_Level.liter150);
                    else
                        pressure.AddFYFJ(fsvalue, PublicEnum.Kpa_Level.liter150);

                }
                else if (kpa_Level == PublicEnum.Kpa_Level.drop100)
                {
                    if (cyvalue > 0)
                        pressure.AddZYFJ(fsvalue, PublicEnum.Kpa_Level.drop100);
                    else
                        pressure.AddFYFJ(fsvalue, PublicEnum.Kpa_Level.drop100);
                }
            }
            else if (rdb_zdstl.Checked)
            {
                if (kpa_Level == PublicEnum.Kpa_Level.liter100)
                {
                    if (cyvalue > 0)
                        pressure.AddZYZD(fsvalue, PublicEnum.Kpa_Level.liter100);
                    else
                        pressure.AddFYZD(fsvalue, PublicEnum.Kpa_Level.liter100);
                }
                else if (kpa_Level == PublicEnum.Kpa_Level.liter150)
                {
                    if (cyvalue > 0)
                        pressure.AddZYZD(fsvalue, PublicEnum.Kpa_Level.liter150);
                    else
                        pressure.AddFYZD(fsvalue, PublicEnum.Kpa_Level.liter150);

                }
                else if (kpa_Level == PublicEnum.Kpa_Level.drop100)
                {
                    if (cyvalue > 0)
                        pressure.AddZYZD(fsvalue, PublicEnum.Kpa_Level.drop100);
                    else
                        pressure.AddFYZD(fsvalue, PublicEnum.Kpa_Level.drop100);
                }
            }
        }

        /// <summary>
        /// 获取是否已经读取的压力状态
        /// </summary>

        private bool Z_S_100Stop = true;//生正压100 
        private bool Z_S_150Stop = true;//生正压150
        private bool Z_J_100Stop = true;//降正压100
        private bool F_S_100Stop = true;//生负压100s
        private bool F_S_150Stop = true;//生负压150
        private bool F_J_100Stop = true;//降负压100

        /// <summary>
        /// 设置添加数据状态
        /// </summary>
        /// <param name="value"></param>
        private void SetCurrType(int value)
        {

            bool start = _tcpClient.Get_Z_S100TimeStart();

            if (start && Z_S_100Stop)
            {
                kpa_Level = PublicEnum.Kpa_Level.liter100;
                tim_Top10.Enabled = true;
                Z_S_100Stop = false;
            }

            start = _tcpClient.Get_Z_S150PaTimeStart();

            if (start && Z_S_150Stop)
            {
                kpa_Level = PublicEnum.Kpa_Level.liter150;
                tim_Top10.Enabled = true;
                Z_S_150Stop = false;
            }

            start = _tcpClient.Get_Z_J100PaTimeStart();

            if (start && Z_J_100Stop)
            {
                Thread.Sleep(500);
                kpa_Level = PublicEnum.Kpa_Level.drop100;
                tim_Top10.Enabled = true;
                Z_J_100Stop = false;
            }

            //负压
            start = _tcpClient.Get_F_S100PaTimeStart();

            if (start && F_S_100Stop)
            {
                kpa_Level = PublicEnum.Kpa_Level.liter100;
                tim_Top10.Enabled = true;
                F_S_100Stop = false;
            }

            start = _tcpClient.Get_F_S150PaTimeStart();

            if (start && F_S_150Stop)
            {
                kpa_Level = PublicEnum.Kpa_Level.liter150;
                tim_Top10.Enabled = true;
                F_S_150Stop = false;
            }
            start = _tcpClient.Get_F_J100PaTimeStart();

            if (start && F_J_100Stop)
            {
                Thread.Sleep(500);
                kpa_Level = PublicEnum.Kpa_Level.drop100;
                tim_Top10.Enabled = true;
                F_J_100Stop = false;
            }
        }
        #endregion


        #region 气密性能检测按钮事件

        /// <summary>
        /// 判断是否开启正压预备或负压预备
        /// </summary>
        private bool IsYB = false;
        /// <summary>
        /// 判断是否开启正压开始或负压开始
        /// </summary>
        private bool IsStart = false;

        private void btn_zyyb_Click(object sender, EventArgs e)
        {
            if (!_tcpClient.IsTCPLink)
            {
                MessageBox.Show("未连接服务器", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }

            double yl = _tcpClient.GetZYYBYLZ(ref IsSeccess, "ZYYB");
            if (!IsSeccess)
            {
                MessageBox.Show("读取设定值异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }
            lbl_setYL.Text = yl.ToString();

            IsYB = true;
            DisableBtnType();

            var res = _tcpClient.SetZYYB();
            if (!res)
            {
                MessageBox.Show("正压预备异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }

            airtightPropertyTest = PublicEnum.AirtightPropertyTest.ZReady;
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

        /// <summary>
        /// 正压开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_zyks_Click(object sender, EventArgs e)
        {
            IsFirst = false;

            double yl = _tcpClient.GetZYYBYLZ(ref IsSeccess, "ZYKS");
            if (!IsSeccess)
            {
                MessageBox.Show("读取设定值异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }

            IsStart = true;
            DisableBtnType();

            Z_S_100Stop = true;//生正压100 
            Z_S_150Stop = true;//生正压150
            Z_J_100Stop = true;//降正压100
            F_S_100Stop = true;//生负压100s
            F_S_150Stop = true;//生负压150
            F_J_100Stop = true;//降负压1500

            if (rdb_fjstl.Checked)
            {
                new Pressure().ClearZ_F();
            }
            else if (rdb_zdstl.Checked)
            {
                new Pressure().ClearZ_Z();
            }

            _tcpClient.SendZYKS(ref IsSeccess);
            if (!IsSeccess)
            {
                MessageBox.Show("正压开始异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }

            lbl_setYL.Text = yl.ToString();

            airtightPropertyTest = PublicEnum.AirtightPropertyTest.ZStart;
        }

        private void btn_fyyb_Click(object sender, EventArgs e)
        {
            double yl = _tcpClient.GetZYYBYLZ(ref IsSeccess, "FYYB");
            if (!IsSeccess)
            {
                MessageBox.Show("读取设定值异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }
            lbl_setYL.Text = "-" + yl.ToString();


            IsYB = true;
            DisableBtnType();
            var res = _tcpClient.SendFYYB();
            if (!res)
            {
                MessageBox.Show("负压预备异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }

            airtightPropertyTest = PublicEnum.AirtightPropertyTest.FReady;
        }
        /// <summary>
        /// 禁用按钮
        /// </summary>
        private void DisableBtnType()
        {
            this.btn_zyyb.Enabled = false;
            this.btn_fyyb.Enabled = false;
            this.btn_fyks.Enabled = false;
            this.btn_sjcl.Enabled = false;
            this.btn_zyks.Enabled = false;
        }

        /// <summary>
        /// 开启按钮
        /// </summary>
        private void OpenBtnType()
        {
            this.btn_zyyb.Enabled = true;
            this.btn_fyyb.Enabled = true;
            this.btn_fyks.Enabled = true;
            this.btn_sjcl.Enabled = true;
            this.btn_zyks.Enabled = true;
        }

        /// <summary>
        /// 负压开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_fyks_Click(object sender, EventArgs e)
        {

            IsFirst = false;
            double yl = _tcpClient.GetZYYBYLZ(ref IsSeccess, "FYKS");
            if (!IsSeccess)
            {
                MessageBox.Show("读取设定值异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }
            lbl_setYL.Text = "-" + yl.ToString();

            IsStart = true;
            DisableBtnType();

            Z_S_100Stop = true;//生正压100 
            Z_S_150Stop = true;//生正压150
            Z_J_100Stop = true;//降正压100
            F_S_100Stop = true;//生负压100s
            F_S_150Stop = true;//生负压150
            F_J_100Stop = true;//降负压1500

            if (rdb_fjstl.Checked)
            {
                new Pressure().ClearF_F();
            }
            else if (rdb_zdstl.Checked)
            {
                new Pressure().ClearF_Z();
            }
            var res = _tcpClient.SendFYKS();
            if (!res)
            {
                MessageBox.Show("负压开始异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }

            airtightPropertyTest = PublicEnum.AirtightPropertyTest.FStart;
        }


        #endregion

        private void tChart1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.chart_cms_click.Show(MousePosition.X, MousePosition.Y);
            }
        }

        private void 导出图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            this.tChart_qm.Export.ShowExportDialog();
        }

        private string group = Guid.NewGuid().ToString();

        private void btn_sjcl_Click(object sender, EventArgs e)
        {
            BindFlowBase();
            GetDatabaseLevelIndex();
            BindLevelIndex();
            if (AddQMResult())
            {
                MessageBox.Show("处理成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
        }

        /// <summary>
        /// 添加气密结果
        /// </summary>
        /// <returns></returns>
        private bool AddQMResult()
        {
            DAL_dt_qm_Info dal = new DAL_dt_qm_Info();

            Model_dt_qm_Info model = new Model_dt_qm_Info();

            for (int i = 0; i < 2; i++)
            {
                var desc = this.dgv_levelIndex.Rows[i].Cells["Quantity"].Value.ToString();
                model.info_DangH = _tempTong;
                model.dt_Code = _tempCode;
                if (desc == "单位缝长")
                {
                    model.qm_Z_FC = this.dgv_levelIndex.Rows[i].Cells["PressureZ"].Value.ToString();
                    model.qm_F_FC = this.dgv_levelIndex.Rows[i].Cells["PressureF"].Value.ToString();
                }
                else if (desc == "单位面积")
                {
                    model.qm_Z_MJ = this.dgv_levelIndex.Rows[i].Cells["PressureZ"].Value.ToString();
                    model.qm_F_MJ = this.dgv_levelIndex.Rows[i].Cells["PressureF"].Value.ToString();
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if (i == 0)
                {
                    model.qm_s_z_fj100 = this.dgv_WindSpeed.Rows[i].Cells["Pressure_Z"].Value.ToString();
                    model.qm_s_z_zd100 = this.dgv_WindSpeed.Rows[i].Cells["Pressure_Z_Z"].Value.ToString();
                    model.qm_s_f_fj100 = this.dgv_WindSpeed.Rows[i].Cells["Pressure_F"].Value.ToString();
                    model.qm_s_f_zd100 = this.dgv_WindSpeed.Rows[i].Cells["Pressure_F_Z"].Value.ToString();
                }
                else if (i == 1)
                {
                    model.qm_s_z_fj150 = this.dgv_WindSpeed.Rows[i].Cells["Pressure_Z"].Value.ToString();
                    model.qm_s_z_zd150 = this.dgv_WindSpeed.Rows[i].Cells["Pressure_Z_Z"].Value.ToString();
                    model.qm_s_f_fj150 = this.dgv_WindSpeed.Rows[i].Cells["Pressure_F"].Value.ToString();
                    model.qm_s_f_zd150 = this.dgv_WindSpeed.Rows[i].Cells["Pressure_F_Z"].Value.ToString();
                }
                else if (i == 2)
                {
                    model.qm_j_z_fj100 = this.dgv_WindSpeed.Rows[i].Cells["Pressure_Z"].Value.ToString();
                    model.qm_j_z_zd100 = this.dgv_WindSpeed.Rows[i].Cells["Pressure_Z_Z"].Value.ToString();
                    model.qm_j_f_fj100 = this.dgv_WindSpeed.Rows[i].Cells["Pressure_F"].Value.ToString();
                    model.qm_j_f_zd100 = this.dgv_WindSpeed.Rows[i].Cells["Pressure_F_Z"].Value.ToString();
                }
            }
            return dal.Add(model);
        }


        /// <summary>
        /// 获取分级指标
        /// </summary>
        /// <param name="zFc">正压缝长</param>
        /// <param name="fFc">负压缝长</param>
        /// <param name="zMj">正压面积</param>
        /// <param name="fMj">负压面积</param>
        private void GetDatabaseLevelIndex()
        {
            double kPa = 0;
            double tempTemperature = 0;
            double stitchLength = 0;
            double sumArea = 0;

            DataTable dt = new DAL_dt_Settings().Getdt_SettingsByCode(_tempCode);
            if (dt != null && dt.Rows.Count > 0)
            {
                kPa = double.Parse(dt.Rows[0]["DaQiYaLi"].ToString());
                tempTemperature = double.Parse(dt.Rows[0]["DangQianWenDu"].ToString());
                stitchLength = double.Parse(dt.Rows[0]["KaiQiFengChang"].ToString());
                sumArea = double.Parse(dt.Rows[0]["ZongMianJi"].ToString());
            }
            var pressureFlow = GetPressureFlow();
            zFc = Formula.GetIndexStitchLengthAndArea(pressureFlow[0].Pressure_Z_Z, pressureFlow[0].Pressure_Z, pressureFlow[2].Pressure_Z_Z, pressureFlow[2].Pressure_Z, true, kPa, tempTemperature, stitchLength, sumArea);

            fFc = Formula.GetIndexStitchLengthAndArea(pressureFlow[0].Pressure_F_Z, pressureFlow[0].Pressure_F, pressureFlow[2].Pressure_F_Z, pressureFlow[2].Pressure_F, true, kPa, tempTemperature, stitchLength, sumArea);

            zMj = Formula.GetIndexStitchLengthAndArea(pressureFlow[0].Pressure_Z_Z, pressureFlow[0].Pressure_Z, pressureFlow[2].Pressure_Z_Z, pressureFlow[2].Pressure_Z, false, kPa, tempTemperature, stitchLength, sumArea);

            fMj = Formula.GetIndexStitchLengthAndArea(pressureFlow[0].Pressure_F_Z, pressureFlow[0].Pressure_F, pressureFlow[2].Pressure_F_Z, pressureFlow[2].Pressure_F, false, kPa, tempTemperature, stitchLength, sumArea);
        }

        private void btn_tc_Click(object sender, EventArgs e)
        {

            Stop();
            this.btn_zyyb.Enabled = true;
            this.btn_fyyb.Enabled = true;
            this.btn_fyks.Enabled = true;
            this.btn_sjcl.Enabled = true;
            this.btn_zyks.Enabled = true;
            this.tim_Top10.Enabled = false;
            lbl_setYL.Text = "0";
            BindWindSpeedBase();
            BindFlowBase();
            airtightPropertyTest = PublicEnum.AirtightPropertyTest.Stop;
        }

        /// <summary>
        /// 控制气密性能检测按钮显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tim_getType_Tick(object sender, EventArgs e)
        {

            if (_tcpClient.IsTCPLink)
            {
                if (systemItem == PublicEnum.SystemItem.Airtight)
                {
                    if (airtightPropertyTest == null) { return; }

                    if (airtightPropertyTest == PublicEnum.AirtightPropertyTest.ZReady)
                    {
                        int value = _tcpClient.GetZYYBJS(ref IsSeccess);

                        if (!IsSeccess)
                        {
                            MessageBox.Show("正压预备结束状态异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            return;
                        }
                        if (value == 3)
                        {
                            airtightPropertyTest = PublicEnum.AirtightPropertyTest.Stop;
                            lbl_setYL.Text = "0";
                            OpenBtnType();
                        }
                    }
                    if (airtightPropertyTest == PublicEnum.AirtightPropertyTest.ZStart)
                    {
                        double value = _tcpClient.GetZYKSJS(ref IsSeccess);

                        if (!IsSeccess)
                        {
                            MessageBox.Show("正压开始结束状态异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            return;
                        }
                        if (value >= 15)
                        {
                            airtightPropertyTest = PublicEnum.AirtightPropertyTest.Stop;
                            IsStart = false;
                            Thread.Sleep(1000);
                            lbl_setYL.Text = "0";
                            OpenBtnType();
                        }
                    }

                    if (airtightPropertyTest == PublicEnum.AirtightPropertyTest.FReady)
                    {
                        int value = _tcpClient.GetFYYBJS(ref IsSeccess);

                        if (!IsSeccess)
                        {
                            MessageBox.Show("负压预备结束状态异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            return;
                        }
                        if (value == 3)
                        {
                            airtightPropertyTest = PublicEnum.AirtightPropertyTest.Stop;
                            lbl_setYL.Text = "0";
                            OpenBtnType();
                        }
                    }

                    if (airtightPropertyTest == PublicEnum.AirtightPropertyTest.FStart)
                    {
                        double value = _tcpClient.GetFYKSJS(ref IsSeccess);

                        if (!IsSeccess)
                        {
                            MessageBox.Show("负压开始结束状态异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            return;
                        }
                        if (value >= 15)
                        {
                            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Stop;
                            IsStart = false;
                            Thread.Sleep(1000);
                            lbl_setYL.Text = "0";
                            OpenBtnType();
                        }
                    }
                }
                else if (systemItem == PublicEnum.SystemItem.Watertight)
                {
                    if (waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.Ready)
                    {
                        int value = _tcpClient.GetSMYBJS(ref IsSeccess);

                        if (!IsSeccess)
                        {
                            MessageBox.Show("水密预备结束状态异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            return;
                        }
                        if (value == 3)
                        {
                            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Stop;
                            lbl_sdyl.Text = "0";

                            this.btn_yb.Enabled = true;
                            this.btn_ks.Enabled = true;
                            this.btn_xyj.Enabled = true;
                            this.btn_xyj.Enabled = true;
                        }
                    }

                    if (waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.CycleLoading)
                    {
                        int value = _tcpClient.GetSMYBJS(ref IsSeccess);

                        if (!IsSeccess)
                        {
                            MessageBox.Show("依次加压结束状态异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            return;
                        }
                        if (value == 3)
                        {
                            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Stop;
                            lbl_sdyl.Text = "0";

                            this.btn_ycjy.Enabled = true;
                        }
                    }
                }
            }

        }

        private void gv_list_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_tcpClient.IsTCPLink)
                {
                    BindWindSpeedBase();
                    BindFlowBase();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void tc_RealTimeSurveillance_SelectedIndexChanged(object sender, EventArgs e)
        {

            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Stop;

            TabControl tc = (TabControl)sender;
            int index = tc_RealTimeSurveillance.SelectedIndex;

            if (tc.TabPages[index].Text == "气密监控")
            {
                systemItem = PublicEnum.SystemItem.Airtight;

                QMchartInit();
            }
            else if (tc.TabPages[index].Text == "水密监控")
            {
                systemItem = PublicEnum.SystemItem.Watertight;
                SMchartInit();

            }
            Clear();
        }

        private void Clear()
        {
            pressure = new Pressure();
            airtightPropertyTest = null;
            tim_Top10.Enabled = false;
        }



        private void btn_xyj_Click(object sender, EventArgs e)
        {
            if (lbl_sdyl.Text == "700")
            {
                lbl_sdyl.Text = "0";
                Stop();
                this.btn_yb.Enabled = true;
                this.btn_ks.Enabled = true;
                this.btn_xyj.Enabled = true;
                waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Stop;
                return;
            }

            var res = _tcpClient.SendSMXXYJ();
            if (!res)
            {
                MessageBox.Show("设置水密性下一级异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Next;
        }

        private void btn_2tc_Click(object sender, EventArgs e)
        {
            lbl_sdyl.Text = "0";
            Stop();
            this.btn_yb.Enabled = true;
            this.btn_ks.Enabled = true;
            this.btn_xyj.Enabled = true;
            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Stop;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindFlowBase();
        }


        #region 水密性能检测按钮事件
        private void btn_yb_Click(object sender, EventArgs e)
        {
            double yl = _tcpClient.GetSMYBSDYL(ref IsSeccess, "SMYB");
            if (!IsSeccess)
            {
                MessageBox.Show("读取设定值异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }
            lbl_sdyl.Text = yl.ToString();

            this.btn_yb.Enabled = false;
            this.btn_ks.Enabled = false;
            this.btn_xyj.Enabled = false;
            this.btn_xyj.Enabled = false;

            var res = _tcpClient.SetSMYB();
            if (!res)
            {
                MessageBox.Show("水密预备异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }
            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Ready;
        }

        private void btn_ks_Click(object sender, EventArgs e)
        {
            this.btn_ks.Enabled = false;
            this.btn_xyj.Enabled = true;
            tim_upNext.Enabled = true;
            this.btn_yb.Enabled = false;

            //todo:增加波动
            if (this.rdb_bdjy.Checked == true)
            {
                //波动加压
                var res = _tcpClient.SendSMXKS_波动();
                if (!res)
                {
                    MessageBox.Show("波动水密开始异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }
            }
            else
            {
                //稳定加压
                var res = _tcpClient.SendSMXKS();
                if (!res)
                {
                    MessageBox.Show("稳定水密开始异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }
            }
            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Start;
        }
        /// <summary>
        /// 依次加压
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        private bool ycjyType = true;

        private void btn_ycjy_Click(object sender, EventArgs e)
        {
            ycjyType = (ycjyType ? false : true);

            if (!ycjyType)
            {
                btn_ycjy.Text = "停止";
            }
            else
            {
                btn_ycjy.Text = "依次加压";

                lbl_sdyl.Text = "0";
                Stop();
                this.btn_yb.Enabled = true;
                this.btn_ks.Enabled = true;
                this.btn_xyj.Enabled = true;
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
            if (waterTightPropertyTest == PublicEnum.WaterTightPropertyTest.Stop || airtightPropertyTest == PublicEnum.AirtightPropertyTest.Stop)
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
                    MessageBox.Show("读取设定值异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }
                lbl_sdyl.Text = yl.ToString();
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
            if (new DAL_dt_sm_Info().Add(model))
            {
                MessageBox.Show("处理成功！", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
        }
        #endregion


        /// <summary>
        /// 开始波动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ksbd_Click(object sender, EventArgs e)
        {
            int minValue = -1;
            int maxValue = -1;

            int.TryParse(txt_minValue.Text, out minValue);

            int.TryParse(txt_minValue.Text, out maxValue);

            if (minValue == -1 || maxValue == -1)
            {
                MessageBox.Show("上线-下线压力设置异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }

            this.btn_yb.Enabled = false;
            this.btn_ks.Enabled = false;
            this.btn_xyj.Enabled = false;

            tim_upNext.Enabled = false;

            var res = _tcpClient.SendBoDongksjy(maxValue, minValue);
            if (!res)
            {
                MessageBox.Show("水密波动开始异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }

            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Start;
        }

        /// <summary>
        /// 停止波动按钮【波动】
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_tzbd_Click(object sender, EventArgs e)
        {
            lbl_sdyl.Text = "0";
            StopBoDong();
            this.btn_yb.Enabled = true;
            this.btn_ks.Enabled = true;
            this.btn_xyj.Enabled = true;

            waterTightPropertyTest = PublicEnum.WaterTightPropertyTest.Stop;
        }

        private void tChart_sm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.chart_cms_sm_click.Show(MousePosition.X, MousePosition.Y);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.tChart_sm.Export.ShowExportDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stop();
            this.Close();
        }
    }
}
