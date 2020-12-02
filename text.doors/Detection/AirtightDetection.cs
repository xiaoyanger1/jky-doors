
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

namespace text.doors.Detection
{
    public partial class AirtightDetection : Form
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
        /// 气密数据位置
        /// </summary>
        private PublicEnum.AirtightPropertyTest? airtightPropertyTest = null;

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
        public AirtightDetection() { 
        
        }

        public AirtightDetection(TCPClient tcpClient, string tempCode, string tempTong)
        {
            InitializeComponent();
            this._tcpClient = tcpClient;
            this._tempCode = tempCode;
            this._tempTong = tempTong;

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

            Clear();
        }

        public void StopTimer()
        {
            this.tim_PainPic.Enabled = false;
            this.tim_qm.Enabled = false;
            this.tim_getType.Enabled = false;
        }
        public void InitTimer()
        {
            this.tim_PainPic.Enabled = true;
            this.tim_qm.Enabled = true;
            this.tim_getType.Enabled = true;
        }

        /// <summary>
        /// 绑定设定压力
        /// </summary>
        private void BindSetPressure()
        {
            lbl_title.Text = string.Format("门窗气密性能检测  第{0}号 {1}", this._tempCode, this._tempTong);
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
            }
            else
            {
                pressureList = pressure.GetPressure();
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



        private void AnimateSeries(Steema.TeeChart.TChart chart, int yl)
        {
            this.qm_Line.Add(DateTime.Now, yl);
            this.tChart_qm.Axes.Bottom.SetMinMax(dtnow, DateTime.Now.AddSeconds(20));
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
                lbl_dqyl.Text = value.ToString();

                //读取设定值
                if (airtightPropertyTest == PublicEnum.AirtightPropertyTest.ZStart)
                {
                    double yl = _tcpClient.GetZYYBYLZ(ref IsSeccess, "ZYKS");
                    if (!IsSeccess)
                    {
                      //  MessageBox.Show("获取正压预备异常！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                        return;
                    }
                    lbl_setYL.Text = yl.ToString();
                }
                else if (airtightPropertyTest == PublicEnum.AirtightPropertyTest.FStart)
                {
                    double yl = _tcpClient.GetZYYBYLZ(ref IsSeccess, "FYKS");
                    if (!IsSeccess)
                    {
                       // MessageBox.Show("获取负压开始异常！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
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
                   // MessageBox.Show("获取差压压力异常！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return;
                }

                AnimateSeries(this.tChart_qm, value);

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
              //  MessageBox.Show("获取差压异常--气密前十！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
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
                //MessageBox.Show("获取风速异常！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
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

        private void btn_justready_Click(object sender, EventArgs e)
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
        /// 正压开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_juststart_Click(object sender, EventArgs e)
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

        private void btn_loseready_Click(object sender, EventArgs e)
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
            this.btn_justready.Enabled = false;
            this.btn_loseready.Enabled = false;
            this.btn_losestart.Enabled = false;
            this.btn_datadispose.Enabled = false;
            this.btn_juststart.Enabled = false;
        }

        /// <summary>
        /// 开启按钮
        /// </summary>
        private void OpenBtnType()
        {
            this.btn_justready.Enabled = true;
            this.btn_loseready.Enabled = true;
            this.btn_losestart.Enabled = true;
            this.btn_datadispose.Enabled = true;
            this.btn_juststart.Enabled = true;
        }

        /// <summary>
        /// 负压开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_losestart_Click(object sender, EventArgs e)
        {

            IsFirst = false;
            double yl = _tcpClient.GetZYYBYLZ(ref IsSeccess, "FYKS");
            if (!IsSeccess)
            {
               // MessageBox.Show("读取设定值异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
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
                this.chart_cms_qm_click.Show(MousePosition.X, MousePosition.Y);
            }
        }

        private void export_image_qm_Click(object sender, EventArgs e)
        {

            this.tChart_qm.Export.ShowExportDialog();
        }

        private string group = Guid.NewGuid().ToString();

        private void btn_datadispose_Click(object sender, EventArgs e)
        {
            BindFlowBase();
            GetDatabaseLevelIndex();
            BindLevelIndex();
            if (AddQMResult())
            {
                MessageBox.Show("处理完成！", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
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

        private void btn_stop_Click(object sender, EventArgs e)
        {
            Stop();
            this.btn_justready.Enabled = true;
            this.btn_loseready.Enabled = true;
            this.btn_losestart.Enabled = true;
            this.btn_datadispose.Enabled = true;
            this.btn_juststart.Enabled = true;
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
                if (airtightPropertyTest == null) { return; }

                if (airtightPropertyTest == PublicEnum.AirtightPropertyTest.ZReady)
                {
                    int value = _tcpClient.GetZYYBJS(ref IsSeccess);

                    if (!IsSeccess)
                    {
                        //MessageBox.Show("正压预备结束状态异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
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
                       // MessageBox.Show("正压开始结束状态异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
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
                     //   MessageBox.Show("负压预备结束状态异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
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
                        IsStart = false;
                        Thread.Sleep(1000);
                        lbl_setYL.Text = "0";
                        OpenBtnType();
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

        private void Clear()
        {
            pressure = new Pressure();
            airtightPropertyTest = null;
            tim_Top10.Enabled = false;
        }



        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindFlowBase();
        }

        private void tChart_sm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.chart_cms_qm_click.Show(MousePosition.X, MousePosition.Y);
            }
        }


        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.tChart_qm.Export.ShowExportDialog();
        }


        private void btn_exit_Click(object sender, EventArgs e)
        {
            Stop();
            this.Close();
        }

    }
}
