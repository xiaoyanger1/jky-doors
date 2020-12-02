using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using text.doors.Common;
using text.doors.Default;
using text.doors.Model;
using text.doors.Model.DataBase;
using text.doors.Service;

namespace text.doors.Detection
{
    public partial class WindPressureDetection : Form
    {
        private static Young.Core.Logger.ILog Logger = Young.Core.Logger.LoggerManager.Current();
        private TCPClient _tcpClient;
        //检验编号
        private string _tempCode = "";
        //当前樘号
        private string _tempTong = "";
        public DateTime dtnow { get; set; }

        public WindPressureDetection(TCPClient tcpClient, string tempCode, string tempTong)
        {
            InitializeComponent();
            this._tcpClient = tcpClient;
            this._tempCode = tempCode;
            this._tempTong = tempTong;

            BindData();
            BindSetPressure();
        }
        /// <summary>
        /// 绑定设定压力
        /// </summary>
        private void BindSetPressure()
        {
            lbl_title.Text = string.Format("门窗抗风压性能检测  第{0}号 {1}", this._tempCode, this._tempTong);
        }

        /// <summary>
        /// 风速图标
        /// </summary>
        private void FYchartInit()
        {
            dtnow = DateTime.Now;
            fy_Line.GetVertAxis.SetMinMax(-300, 300);
        }


        private List<string> KFYPa = new List<string>() { "250", "500", "750", "1000", "1250", "1500", "1750", "2000" };

        private void BindData()
        {

            #region 绑定
            dgv_WindPressure.DataSource = GetWindPressureDGV();
            dgv_WindPressure.Height = 215;
            dgv_WindPressure.RowHeadersVisible = false;
            dgv_WindPressure.AllowUserToResizeColumns = false;
            dgv_WindPressure.AllowUserToResizeRows = false;

            dgv_WindPressure.Columns[0].HeaderText = "国际检测";
            dgv_WindPressure.Columns[0].Width = 75;
            dgv_WindPressure.Columns[0].ReadOnly = true;
            dgv_WindPressure.Columns[0].DataPropertyName = "Pa";


            dgv_WindPressure.Columns[1].HeaderText = "位移1";
            dgv_WindPressure.Columns[1].Width = 72;
            dgv_WindPressure.Columns[1].DataPropertyName = "zwy1";


            dgv_WindPressure.Columns[2].HeaderText = "位移2";
            dgv_WindPressure.Columns[2].Width = 72;
            dgv_WindPressure.Columns[2].DataPropertyName = "zwy2";

            dgv_WindPressure.Columns[3].HeaderText = "位移3";
            dgv_WindPressure.Columns[3].Width = 72;
            dgv_WindPressure.Columns[3].DataPropertyName = "zwy3";

            dgv_WindPressure.Columns[4].HeaderText = "挠度";
            dgv_WindPressure.Columns[4].Width = 72;
            dgv_WindPressure.Columns[4].DataPropertyName = "zzd";

            dgv_WindPressure.Columns[5].HeaderText = "I/x";
            dgv_WindPressure.Columns[5].Width = 72;
            dgv_WindPressure.Columns[5].DataPropertyName = "zix";



            dgv_WindPressure.Columns[6].HeaderText = "位移1";
            dgv_WindPressure.Columns[6].Width = 72;
            dgv_WindPressure.Columns[6].DataPropertyName = "fwy1";


            dgv_WindPressure.Columns[7].HeaderText = "位移2";
            dgv_WindPressure.Columns[7].Width = 72;
            dgv_WindPressure.Columns[7].DataPropertyName = "fwy2";

            dgv_WindPressure.Columns[8].HeaderText = "位移3";
            dgv_WindPressure.Columns[8].Width = 72;
            dgv_WindPressure.Columns[8].DataPropertyName = "fwy3";

            dgv_WindPressure.Columns[9].HeaderText = "挠度";
            dgv_WindPressure.Columns[9].Width = 72;
            dgv_WindPressure.Columns[9].DataPropertyName = "fzd";

            dgv_WindPressure.Columns[10].HeaderText = "I/x";
            dgv_WindPressure.Columns[10].Width = 72;
            dgv_WindPressure.Columns[10].DataPropertyName = "fix";


            dgv_WindPressure.Columns["zwy1"].DefaultCellStyle.Format = "N2";
            dgv_WindPressure.Columns["zwy2"].DefaultCellStyle.Format = "N2";
            dgv_WindPressure.Columns["zwy3"].DefaultCellStyle.Format = "N2";
            dgv_WindPressure.Columns["zzd"].DefaultCellStyle.Format = "N2";
            dgv_WindPressure.Columns["zix"].DefaultCellStyle.Format = "N2";

            dgv_WindPressure.Columns["fwy1"].DefaultCellStyle.Format = "N2";
            dgv_WindPressure.Columns["fwy2"].DefaultCellStyle.Format = "N2";
            dgv_WindPressure.Columns["fwy3"].DefaultCellStyle.Format = "N2";
            dgv_WindPressure.Columns["fzd"].DefaultCellStyle.Format = "N2";
            dgv_WindPressure.Columns["fix"].DefaultCellStyle.Format = "N2";
            #endregion

        }


        private List<WindPressureDGV> GetWindPressureDGV()
        {
            List<WindPressureDGV> windPressureDGV = new List<WindPressureDGV>();
            var dt = new DAL_dt_kfy_Info().GetkfyByCodeAndTong(_tempCode, _tempTong);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                foreach (var value in KFYPa)
                {
                    windPressureDGV.Add(new WindPressureDGV()
                    {
                        Pa = value + "Pa",
                        zwy1 = string.IsNullOrWhiteSpace(dr["z_one_" + value].ToString()) ? 0 : double.Parse(dr["z_one_" + value].ToString()),
                        zwy2 = string.IsNullOrWhiteSpace(dr["z_two_" + value].ToString()) ? 0 : double.Parse(dr["z_two_" + value].ToString()),
                        zwy3 = string.IsNullOrWhiteSpace(dr["z_three_" + value].ToString()) ? 0 : double.Parse(dr["z_three_" + value].ToString()),
                        zzd = string.IsNullOrWhiteSpace(dr["z_nd_" + value].ToString()) ? 0 : double.Parse(dr["z_nd_" + value].ToString()),
                        zix = string.IsNullOrWhiteSpace(dr["z_ix_" + value].ToString()) ? 0 : double.Parse(dr["z_ix_" + value].ToString()),

                        fwy1 = string.IsNullOrWhiteSpace(dr["f_one_" + value].ToString()) ? 0 : double.Parse(dr["f_one_" + value].ToString()),
                        fwy2 = string.IsNullOrWhiteSpace(dr["f_two_" + value].ToString()) ? 0 : double.Parse(dr["f_two_" + value].ToString()),
                        fwy3 = string.IsNullOrWhiteSpace(dr["f_three_" + value].ToString()) ? 0 : double.Parse(dr["f_three_" + value].ToString()),
                        fzd = string.IsNullOrWhiteSpace(dr["f_nd_" + value].ToString()) ? 0 : double.Parse(dr["f_nd_" + value].ToString()),
                        fix = string.IsNullOrWhiteSpace(dr["f_ix_" + value].ToString()) ? 0 : double.Parse(dr["f_ix_" + value].ToString()),
                    });
                }
            }
            else
            {
                foreach (var value in KFYPa)
                {
                    windPressureDGV.Add(new WindPressureDGV()
                    {
                        Pa = value + "Pa",
                        zwy1 = 0,
                        zwy2 = 0,
                        zwy3 = 0,
                        zzd = 0,
                        zix = 0,
                        fwy1 = 0,
                        fwy2 = 0,
                        fwy3 = 0,
                        fzd = 0,
                        fix = 0,
                    });
                }
            }
            return windPressureDGV;
        }

        private void AnimateSeries(Steema.TeeChart.TChart chart, int yl)
        {
            this.fy_Line.Add(DateTime.Now, yl);
            this.tChart_fy.Axes.Bottom.SetMinMax(dtnow, DateTime.Now.AddSeconds(20));
        }

        private void tim_PainPic_Tick(object sender, EventArgs e)
        {

            if (_tcpClient.IsTCPLink)
            {
                var IsSeccess = false;
                var c = _tcpClient.GetCYXS(ref IsSeccess);
                int value = int.Parse(c.ToString());
                if (!IsSeccess)
                {
                    MessageBox.Show("获取大气压力异常！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return;
                }
                AnimateSeries(this.tChart_fy, value);
            }
        }

        private void btn_zyyb_Click(object sender, EventArgs e)
        {
            if (_tcpClient.IsTCPLink)
            {
                var res = _tcpClient.SetKFYZYYB();
                if (!res)
                {
                    MessageBox.Show("正压预备异常！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }
            }
        }

        private void btn_zyks_Click(object sender, EventArgs e)
        {
            if (_tcpClient.IsTCPLink)
            {
                var res = _tcpClient.SendKFYZYKS();
                if (!res)
                {
                    MessageBox.Show("正压开始异常！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }
            }
        }

        private void btn_fyyb_Click(object sender, EventArgs e)
        {
            if (_tcpClient.IsTCPLink)
            {
                var res = _tcpClient.SendKFYFYYB();
                if (!res)
                {
                    MessageBox.Show("负压预备异常！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }
            }
        }

        private void btn_fyks_Click(object sender, EventArgs e)
        {
            if (_tcpClient.IsTCPLink)
            {
                var res = _tcpClient.SendKFYFYKS();
                if (!res)
                {
                    MessageBox.Show("负压开始异常！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }
            }
        }

        private void btn_datahandle_Click(object sender, EventArgs e)
        {
            if (AddKfyInfo())
            {
                MessageBox.Show("处理成功！", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
        }

        private bool AddKfyInfo()
        {
            DAL_dt_kfy_Info dal = new DAL_dt_kfy_Info();
            Model_dt_kfy_Info model = new Model_dt_kfy_Info();
            model.dt_Code = _tempCode;
            model.info_DangH = _tempTong;

            for (int i = 0; i < KFYPa.Count; i++)
            {
                #region 获取
                if (i == 0)
                {
                    model.z_one_250 = this.dgv_WindPressure.Rows[i].Cells["zwy1"].Value.ToString();
                    model.z_two_250 = this.dgv_WindPressure.Rows[i].Cells["zwy2"].Value.ToString();
                    model.z_three_250 = this.dgv_WindPressure.Rows[i].Cells["zwy3"].Value.ToString();
                    model.z_nd_250 = this.dgv_WindPressure.Rows[i].Cells["zzd"].Value.ToString();
                    model.z_ix_250 = this.dgv_WindPressure.Rows[i].Cells["zix"].Value.ToString();
                    model.f_one_250 = this.dgv_WindPressure.Rows[i].Cells["fwy1"].Value.ToString();
                    model.f_two_250 = this.dgv_WindPressure.Rows[i].Cells["fwy2"].Value.ToString();
                    model.f_three_250 = this.dgv_WindPressure.Rows[i].Cells["fwy3"].Value.ToString();
                    model.f_nd_250 = this.dgv_WindPressure.Rows[i].Cells["fzd"].Value.ToString();
                    model.f_ix_250 = this.dgv_WindPressure.Rows[i].Cells["fix"].Value.ToString();
                }
                if (i == 1)
                {
                    model.z_one_500 = this.dgv_WindPressure.Rows[i].Cells["zwy1"].Value.ToString();
                    model.z_two_500 = this.dgv_WindPressure.Rows[i].Cells["zwy2"].Value.ToString();
                    model.z_three_500 = this.dgv_WindPressure.Rows[i].Cells["zwy3"].Value.ToString();
                    model.z_nd_500 = this.dgv_WindPressure.Rows[i].Cells["zzd"].Value.ToString();
                    model.z_ix_500 = this.dgv_WindPressure.Rows[i].Cells["zix"].Value.ToString();
                    model.f_one_500 = this.dgv_WindPressure.Rows[i].Cells["fwy1"].Value.ToString();
                    model.f_two_500 = this.dgv_WindPressure.Rows[i].Cells["fwy2"].Value.ToString();
                    model.f_three_500 = this.dgv_WindPressure.Rows[i].Cells["fwy3"].Value.ToString();
                    model.f_nd_500 = this.dgv_WindPressure.Rows[i].Cells["fzd"].Value.ToString();
                    model.f_ix_500 = this.dgv_WindPressure.Rows[i].Cells["fix"].Value.ToString();
                }
                if (i == 2)
                {
                    model.z_one_750 = this.dgv_WindPressure.Rows[i].Cells["zwy1"].Value.ToString();
                    model.z_two_750 = this.dgv_WindPressure.Rows[i].Cells["zwy2"].Value.ToString();
                    model.z_three_750 = this.dgv_WindPressure.Rows[i].Cells["zwy3"].Value.ToString();
                    model.z_nd_750 = this.dgv_WindPressure.Rows[i].Cells["zzd"].Value.ToString();
                    model.z_ix_750 = this.dgv_WindPressure.Rows[i].Cells["zix"].Value.ToString();
                    model.f_one_750 = this.dgv_WindPressure.Rows[i].Cells["fwy1"].Value.ToString();
                    model.f_two_750 = this.dgv_WindPressure.Rows[i].Cells["fwy2"].Value.ToString();
                    model.f_three_750 = this.dgv_WindPressure.Rows[i].Cells["fwy3"].Value.ToString();
                    model.f_nd_750 = this.dgv_WindPressure.Rows[i].Cells["fzd"].Value.ToString();
                    model.f_ix_750 = this.dgv_WindPressure.Rows[i].Cells["fix"].Value.ToString();
                }
                if (i == 3)
                {
                    model.z_one_1000 = this.dgv_WindPressure.Rows[i].Cells["zwy1"].Value.ToString();
                    model.z_two_1000 = this.dgv_WindPressure.Rows[i].Cells["zwy2"].Value.ToString();
                    model.z_three_1000 = this.dgv_WindPressure.Rows[i].Cells["zwy3"].Value.ToString();
                    model.z_nd_1000 = this.dgv_WindPressure.Rows[i].Cells["zzd"].Value.ToString();
                    model.z_ix_1000 = this.dgv_WindPressure.Rows[i].Cells["zix"].Value.ToString();
                    model.f_one_1000 = this.dgv_WindPressure.Rows[i].Cells["fwy1"].Value.ToString();
                    model.f_two_1000 = this.dgv_WindPressure.Rows[i].Cells["fwy2"].Value.ToString();
                    model.f_three_1000 = this.dgv_WindPressure.Rows[i].Cells["fwy3"].Value.ToString();
                    model.f_nd_1000 = this.dgv_WindPressure.Rows[i].Cells["fzd"].Value.ToString();
                    model.f_ix_1000 = this.dgv_WindPressure.Rows[i].Cells["fix"].Value.ToString();
                }
                if (i == 4)
                {
                    model.z_one_1250 = this.dgv_WindPressure.Rows[i].Cells["zwy1"].Value.ToString();
                    model.z_two_1250 = this.dgv_WindPressure.Rows[i].Cells["zwy2"].Value.ToString();
                    model.z_three_1250 = this.dgv_WindPressure.Rows[i].Cells["zwy3"].Value.ToString();
                    model.z_nd_1250 = this.dgv_WindPressure.Rows[i].Cells["zzd"].Value.ToString();
                    model.z_ix_1250 = this.dgv_WindPressure.Rows[i].Cells["zix"].Value.ToString();
                    model.f_one_1250 = this.dgv_WindPressure.Rows[i].Cells["fwy1"].Value.ToString();
                    model.f_two_1250 = this.dgv_WindPressure.Rows[i].Cells["fwy2"].Value.ToString();
                    model.f_three_1250 = this.dgv_WindPressure.Rows[i].Cells["fwy3"].Value.ToString();
                    model.f_nd_1250 = this.dgv_WindPressure.Rows[i].Cells["fzd"].Value.ToString();
                    model.f_ix_1250 = this.dgv_WindPressure.Rows[i].Cells["fix"].Value.ToString();
                }
                if (i == 5)
                {
                    model.z_one_1500 = this.dgv_WindPressure.Rows[i].Cells["zwy1"].Value.ToString();
                    model.z_two_1500 = this.dgv_WindPressure.Rows[i].Cells["zwy2"].Value.ToString();
                    model.z_three_1500 = this.dgv_WindPressure.Rows[i].Cells["zwy3"].Value.ToString();
                    model.z_nd_1500 = this.dgv_WindPressure.Rows[i].Cells["zzd"].Value.ToString();
                    model.z_ix_1500 = this.dgv_WindPressure.Rows[i].Cells["zix"].Value.ToString();
                    model.f_one_1500 = this.dgv_WindPressure.Rows[i].Cells["fwy1"].Value.ToString();
                    model.f_two_1500 = this.dgv_WindPressure.Rows[i].Cells["fwy2"].Value.ToString();
                    model.f_three_1500 = this.dgv_WindPressure.Rows[i].Cells["fwy3"].Value.ToString();
                    model.f_nd_1500 = this.dgv_WindPressure.Rows[i].Cells["fzd"].Value.ToString();
                    model.f_ix_1500 = this.dgv_WindPressure.Rows[i].Cells["fix"].Value.ToString();
                }
                if (i == 6)
                {
                    model.z_one_1750 = this.dgv_WindPressure.Rows[i].Cells["zwy1"].Value.ToString();
                    model.z_two_1750 = this.dgv_WindPressure.Rows[i].Cells["zwy2"].Value.ToString();
                    model.z_three_1750 = this.dgv_WindPressure.Rows[i].Cells["zwy3"].Value.ToString();
                    model.z_nd_1750 = this.dgv_WindPressure.Rows[i].Cells["zzd"].Value.ToString();
                    model.z_ix_1750 = this.dgv_WindPressure.Rows[i].Cells["zix"].Value.ToString();
                    model.f_one_1750 = this.dgv_WindPressure.Rows[i].Cells["fwy1"].Value.ToString();
                    model.f_two_1750 = this.dgv_WindPressure.Rows[i].Cells["fwy2"].Value.ToString();
                    model.f_three_1750 = this.dgv_WindPressure.Rows[i].Cells["fwy3"].Value.ToString();
                    model.f_nd_1750 = this.dgv_WindPressure.Rows[i].Cells["fzd"].Value.ToString();
                    model.f_ix_1750 = this.dgv_WindPressure.Rows[i].Cells["fix"].Value.ToString();
                }
                if (i == 7)
                {
                    model.z_one_2000 = this.dgv_WindPressure.Rows[i].Cells["zwy1"].Value.ToString();
                    model.z_two_2000 = this.dgv_WindPressure.Rows[i].Cells["zwy2"].Value.ToString();
                    model.z_three_2000 = this.dgv_WindPressure.Rows[i].Cells["zwy3"].Value.ToString();
                    model.z_nd_2000 = this.dgv_WindPressure.Rows[i].Cells["zzd"].Value.ToString();
                    model.z_ix_2000 = this.dgv_WindPressure.Rows[i].Cells["zix"].Value.ToString();
                    model.f_one_2000 = this.dgv_WindPressure.Rows[i].Cells["fwy1"].Value.ToString();
                    model.f_two_2000 = this.dgv_WindPressure.Rows[i].Cells["fwy2"].Value.ToString();
                    model.f_three_2000 = this.dgv_WindPressure.Rows[i].Cells["fwy3"].Value.ToString();
                    model.f_nd_2000 = this.dgv_WindPressure.Rows[i].Cells["fzd"].Value.ToString();
                    model.f_ix_2000 = this.dgv_WindPressure.Rows[i].Cells["fix"].Value.ToString();
                }

                #endregion
            }
            return dal.Add_kfy_Info(model);
        }

        private void btn_zff_Click(object sender, EventArgs e)
        {
            if (_tcpClient.IsTCPLink)
            {
                double value = 0;
                var res = _tcpClient.SendZFF(value);
                if (!res)
                {
                    MessageBox.Show("正反复异常！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }
            }
        }

        private void btn_fff_Click(object sender, EventArgs e)
        {
            if (_tcpClient.IsTCPLink)
            {
                double value = 0;
                var res = _tcpClient.SendFFF(value);
                if (!res)
                {
                    MessageBox.Show("负反复异常！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }
            }
        }

        private void btn_zaq_Click(object sender, EventArgs e)
        {
            if (_tcpClient.IsTCPLink)
            {
                double value = 0;
                var res = _tcpClient.SendZAQ(value);
                if (!res)
                {
                    MessageBox.Show("正安全异常！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }
            }
        }

        private void btnfaq_Click(object sender, EventArgs e)
        {
            if (_tcpClient.IsTCPLink)
            {
                double value = 0;
                var res = _tcpClient.SendFAQ(value);
                if (!res)
                {
                    MessageBox.Show("负安全异常！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }
            }
        }

        private void dgv_WindPressure_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

        }
    }
}
