using text.doors.Common;
using text.doors.dal;
using text.doors.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using text.doors.Model.DataBase;
using text.doors.Default;
using System.Linq;
namespace text.doors.Detection
{
    public partial class ComplexAssessment : Form
    {
        private static Young.Core.Logger.ILog Logger = Young.Core.Logger.LoggerManager.Current();
        public string _code = "";

        private Model_dt_Settings _settings = new Model_dt_Settings();

        public ComplexAssessment(string code)
        {
            InitializeComponent();
            this._code = code;

            if (!DefaultBase.IsSetTong)
            {
                MessageBox.Show("请先检测设定", "检测", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                this.Hide();
                DefaultBase.IsOpenComplexAssessment = false;
                return;
            }

            this._settings = new DAL_dt_Settings().GetInfoByCode(_code);

            InitResult();
        }


        /// <summary>
        /// 绑定检测结果
        /// </summary>
        private void InitResult()
        {
            try
            {
                string error = "";
                if (!IsTestFinish(ref error))
                {
                    MessageBox.Show(error, "检测", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    this.Hide();
                    DefaultBase.IsOpenComplexAssessment = false;
                    return;
                }

                for (int i = 0; i < _settings.dt_qm_Info.Count; i++)
                {
                    if (i == 0)
                    {
                        txt_1zfc.Text = _settings.dt_qm_Info[i].qm_Z_FC;
                        txt_1ffc.Text = _settings.dt_qm_Info[i].qm_F_FC;
                        txt_1zmj.Text = _settings.dt_qm_Info[i].qm_Z_MJ;
                        txt_1fmj.Text = _settings.dt_qm_Info[i].qm_F_MJ;
                    }
                    else if (i == 1)
                    {
                        txt_2zfc.Text = _settings.dt_qm_Info[i].qm_Z_FC;
                        txt_2ffc.Text = _settings.dt_qm_Info[i].qm_F_FC;
                        txt_2zmj.Text = _settings.dt_qm_Info[i].qm_Z_MJ;
                        txt_2fmj.Text = _settings.dt_qm_Info[i].qm_F_MJ;
                    }
                    else if (i == 2)
                    {
                        txt_3zfc.Text = _settings.dt_qm_Info[i].qm_Z_FC;
                        txt_3ffc.Text = _settings.dt_qm_Info[i].qm_F_FC;
                        txt_3zmj.Text = _settings.dt_qm_Info[i].qm_Z_MJ;
                        txt_3fmj.Text = _settings.dt_qm_Info[i].qm_F_MJ;
                    }
                }

                for (int i = 0; i < _settings.dt_sm_Info.Count; i++)
                {
                    if (i == 0)
                    {
                        lbl_1desc.Text = _settings.dt_sm_Info[i].sm_Remark;
                        lbl_1resdesc.Text = _settings.dt_sm_Info[i].sm_PaDesc;
                        txt_1fy.Text = _settings.dt_sm_Info[i].sm_Pa;
                    }
                    else if (i == 1)
                    {
                        lbl_2desc.Text = _settings.dt_sm_Info[i].sm_Remark;
                        lbl_2resdesc.Text = _settings.dt_sm_Info[i].sm_PaDesc;
                        txt_2fy.Text = _settings.dt_sm_Info[i].sm_Pa;
                    }
                    else if (i == 2)
                    {
                        lbl_3desc.Text = _settings.dt_sm_Info[i].sm_Remark;
                        lbl_3resdesc.Text = _settings.dt_sm_Info[i].sm_PaDesc;
                        txt_3fy.Text = _settings.dt_sm_Info[i].sm_Pa;
                    }
                }
                for (int i = 0; i < _settings.dt_kfy_Info.Count; i++)
                {
                    //todo：抗风压绑定
                }

                #region old


                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    if (i == 0)
                //    {
                //        groupBox1.Text = dt.Rows[i]["info_DangH"].ToString();
                //        if (DetectionItemEnum == PublicEnum.DetectionItem.enum_气密性能检测)
                //        {
                //            txt_1zfc.Text = dt.Rows[i]["qm_Z_FC"].ToString();
                //            txt_1ffc.Text = dt.Rows[i]["qm_F_FC"].ToString();
                //            txt_1zmj.Text = dt.Rows[i]["qm_Z_MJ"].ToString();
                //            txt_1fmj.Text = dt.Rows[i]["qm_F_MJ"].ToString();
                //        }
                //        else if (DetectionItemEnum == PublicEnum.DetectionItem.enum_水密性能检测)
                //        {
                //            lbl_1desc.Text = dt.Rows[i]["sm_Remark"].ToString();
                //            lbl_1resdesc.Text = dt.Rows[i]["sm_PaDesc"].ToString();
                //            txt_1fy.Text = dt.Rows[i]["sm_Pa"].ToString();
                //        }
                //        else if (DetectionItemEnum == PublicEnum.DetectionItem.enum_气密性能及水密性能检测)
                //        {
                //            lbl_1desc.Text = dt.Rows[i]["sm_Remark"].ToString();
                //            lbl_1resdesc.Text = dt.Rows[i]["sm_PaDesc"].ToString();
                //            txt_1fy.Text = dt.Rows[i]["sm_Pa"].ToString();
                //            txt_1zfc.Text = dt.Rows[i]["qm_Z_FC"].ToString();
                //            txt_1ffc.Text = dt.Rows[i]["qm_F_FC"].ToString();
                //            txt_1zmj.Text = dt.Rows[i]["qm_Z_MJ"].ToString();
                //            txt_1fmj.Text = dt.Rows[i]["qm_F_MJ"].ToString();
                //        }
                //    }
                //    if (i == 1)
                //    {
                //        groupBox2.Text = dt.Rows[i]["info_DangH"].ToString();
                //        if (DetectionItemEnum == PublicEnum.DetectionItem.enum_气密性能检测)
                //        {
                //            txt_2zfc.Text = dt.Rows[i]["qm_Z_FC"].ToString();
                //            txt_2ffc.Text = dt.Rows[i]["qm_F_FC"].ToString();
                //            txt_2zmj.Text = dt.Rows[i]["qm_Z_MJ"].ToString();
                //            txt_2fmj.Text = dt.Rows[i]["qm_F_MJ"].ToString();
                //        }
                //        else if (DetectionItemEnum == PublicEnum.DetectionItem.enum_水密性能检测)
                //        {
                //            lbl_2desc.Text = dt.Rows[i]["sm_Remark"].ToString();
                //            lbl_2resdesc.Text = dt.Rows[i]["sm_PaDesc"].ToString();
                //            txt_2fy.Text = dt.Rows[i]["sm_Pa"].ToString();
                //        }
                //        else if (DetectionItemEnum == PublicEnum.DetectionItem.enum_气密性能及水密性能检测)
                //        {
                //            lbl_2desc.Text = dt.Rows[i]["sm_Remark"].ToString();
                //            lbl_2resdesc.Text = dt.Rows[i]["sm_PaDesc"].ToString();
                //            txt_2fy.Text = dt.Rows[i]["sm_Pa"].ToString();
                //            txt_2zfc.Text = dt.Rows[i]["qm_Z_FC"].ToString();
                //            txt_2ffc.Text = dt.Rows[i]["qm_F_FC"].ToString();
                //            txt_2zmj.Text = dt.Rows[i]["qm_Z_MJ"].ToString();
                //            txt_2fmj.Text = dt.Rows[i]["qm_F_MJ"].ToString();
                //        }
                //    }
                //    if (i == 2)
                //    {
                //        groupBox3.Text = dt.Rows[i]["info_DangH"].ToString();
                //        if (DetectionItemEnum == PublicEnum.DetectionItem.enum_气密性能检测)
                //        {
                //            txt_3zfc.Text = dt.Rows[i]["qm_Z_FC"].ToString();
                //            txt_3ffc.Text = dt.Rows[i]["qm_F_FC"].ToString();
                //            txt_3zmj.Text = dt.Rows[i]["qm_Z_MJ"].ToString();
                //            txt_3fmj.Text = dt.Rows[i]["qm_F_MJ"].ToString();
                //        }
                //        else if (DetectionItemEnum == PublicEnum.DetectionItem.enum_水密性能检测)
                //        {
                //            lbl_3desc.Text = dt.Rows[i]["sm_Remark"].ToString();
                //            lbl_3resdesc.Text = dt.Rows[i]["sm_PaDesc"].ToString();
                //            txt_3fy.Text = dt.Rows[i]["sm_Pa"].ToString();
                //        }
                //        else if (DetectionItemEnum == PublicEnum.DetectionItem.enum_气密性能及水密性能检测)
                //        {
                //            lbl_3desc.Text = dt.Rows[i]["sm_Remark"].ToString();
                //            lbl_3resdesc.Text = dt.Rows[i]["sm_PaDesc"].ToString();
                //            txt_3fy.Text = dt.Rows[i]["sm_Pa"].ToString();
                //            txt_3zfc.Text = dt.Rows[i]["qm_Z_FC"].ToString();
                //            txt_3ffc.Text = dt.Rows[i]["qm_F_FC"].ToString();
                //            txt_3zmj.Text = dt.Rows[i]["qm_Z_MJ"].ToString();
                //            txt_3fmj.Text = dt.Rows[i]["qm_F_MJ"].ToString();
                //        }
                //    }
                //}
                #endregion

                DefaultBase.IsOpenComplexAssessment = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Logger.Error(ex);
            }
        }

        //是否测试完成
        private bool IsTestFinish(ref string error)
        {
            var testItem = DefaultBase._TestItem;
            var specCount = DefaultBase.base_SpecCount;
            if (specCount != _settings.dt_InfoList.Count)
            {
                error = "设置规格为" + specCount + "樘,当前完成" + _settings.dt_InfoList.Count + "樘";
                return false;
            }

            if (PublicEnum.DetectionItem.enum_抗风压性能检测 == testItem)
            {
                foreach (var item in _settings.dt_InfoList)
                {
                    if (item.WindPressure == 0)
                    {
                        error = item.info_DangH + "未完成抗风压性能检测";
                        return false;
                    }
                }
            }

            if (PublicEnum.DetectionItem.enum_气密性能及抗风压性能检测 == testItem)
            {
                foreach (var item in _settings.dt_InfoList)
                {
                    if (item.WindPressure == 0 || item.Airtight == 0)
                    {
                        if (item.WindPressure == 0)
                            error = item.info_DangH + "未完成抗风压性能检测";
                        if (item.Airtight == 0)
                            error = item.info_DangH + "未完成气密性能检测";

                        return false;
                    }
                }
            }
            if (PublicEnum.DetectionItem.enum_气密性能及水密性能检测 == testItem)
            {
                foreach (var item in _settings.dt_InfoList)
                {
                    if (item.Watertight == 0 || item.Airtight == 0)
                    {
                        if (item.Watertight == 0)
                            error = item.info_DangH + "未完成气密性能检测";
                        if (item.Airtight == 0)
                            error = item.info_DangH + "未完成气密性能检测";

                        return false;
                    }
                }
            }
            if (PublicEnum.DetectionItem.enum_气密性能检测 == testItem)
            {
                foreach (var item in _settings.dt_InfoList)
                {
                    if (item.Watertight == 0)
                    {
                        if (item.Watertight == 0)
                            error = item.info_DangH + "未完成气密性能检测";
                        return false;
                    }
                }
            }

            if (PublicEnum.DetectionItem.enum_气密水密抗风压性能检测 == testItem)
            {
                foreach (var item in _settings.dt_InfoList)
                {
                    if (item.WindPressure == 0 || item.Airtight == 0 || item.Watertight == 0)
                    {
                        if (item.Watertight == 0)
                            error = item.info_DangH + "未完成水密性能检测";
                        if (item.Airtight == 0)
                            error = item.info_DangH + "未完成气密性能检测";
                        if (item.WindPressure == 0)
                            error = item.info_DangH + "未完成抗风压性能检测";

                        return false;
                    }
                }
            }
            if (PublicEnum.DetectionItem.enum_水密性能及抗风压性能检测 == testItem)
            {
                foreach (var item in _settings.dt_InfoList)
                {
                    if (item.WindPressure == 0 || item.Watertight == 0)
                    {
                        if (item.Watertight == 0)
                            error = item.info_DangH + "未完成水密性能检测";
                        if (item.WindPressure == 0)
                            error = item.info_DangH + "未完成抗风压性能检测";

                        return false;
                    }
                }
            }
            if (PublicEnum.DetectionItem.enum_水密性能检测 == testItem)
            {

                foreach (var item in _settings.dt_InfoList)
                {
                    if (item.Watertight == 0)
                    {
                        if (item.Watertight == 0)
                            error = item.info_DangH + "未完成水密性能检测";

                        return false;
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// 水密属性
        /// </summary>
        public int sm_value = 999;


        private void btn_audit_Click(object sender, EventArgs e)
        {
            try
            {

                #region     修改数据
                if (_settings.dt_qm_Info != null && _settings.dt_qm_Info.Count > 0)
                {
                    for (int i = 0; i < _settings.dt_qm_Info.Count; i++)
                    {
                        var setting = _settings.dt_qm_Info[i];
                        setting.dt_Code = _code;
                        setting.info_DangH = groupBox1.Text;
                        if (i == 0)
                        {
                            setting.qm_Z_FC = txt_1zfc.Text;
                            setting.qm_F_FC = txt_1ffc.Text;
                            setting.qm_Z_MJ = txt_1zmj.Text;
                            setting.qm_F_MJ = txt_1fmj.Text;
                        }
                        if (i == 1)
                        {
                            setting.info_DangH = groupBox2.Text;
                            setting.qm_Z_FC = txt_2zfc.Text;
                            setting.qm_F_FC = txt_2ffc.Text;
                            setting.qm_Z_MJ = txt_2zmj.Text;
                            setting.qm_F_MJ = txt_2fmj.Text;
                        }
                        if (i == 2)
                        {
                            setting.info_DangH = groupBox3.Text;
                            setting.qm_Z_FC = txt_3zfc.Text;
                            setting.qm_F_FC = txt_3ffc.Text;
                            setting.qm_Z_MJ = txt_3zmj.Text;
                            setting.qm_F_MJ = txt_3fmj.Text;
                        }
                    }
                }

                if (_settings.dt_sm_Info != null && _settings.dt_sm_Info.Count > 0)
                {
                    for (int i = 0; i < _settings.dt_sm_Info.Count; i++)
                    {
                        var setting = _settings.dt_sm_Info[i];
                        setting.info_DangH = groupBox1.Text;
                        if (i == 0)
                        {
                            setting.sm_Pa = txt_1fy.Text;
                            setting.sm_PaDesc = lbl_1resdesc.Text;
                            setting.sm_Remark = lbl_1desc.Text;
                        }
                        if (i == 1)
                        {
                            setting.sm_Pa = txt_2fy.Text;
                            setting.sm_PaDesc = lbl_2resdesc.Text;
                            setting.sm_Remark = lbl_2desc.Text;
                        }
                        if (i == 2)
                        {
                            setting.sm_Pa = txt_3fy.Text;
                            setting.sm_PaDesc = lbl_3resdesc.Text;
                            setting.sm_Remark = lbl_3desc.Text;
                        }
                    }
                    new DAL_dt_qm_Info().UpdateResult(_settings);

                    #endregion

                    #region 获取设置后的樘号信息 --   判定

                    InitResult();


                    Formula formula = new Formula();
                    DataTable settings = new DAL_dt_Settings().Getdt_SettingsByCode(_code);
                    if (settings != null && settings.Rows.Count > 0)
                    {
                        txt_sjz1.Text = settings.Rows[0]["ShuiMiSheJiZhi"].ToString();
                        txt_sjz2.Text = settings.Rows[0]["QiMiZhengYaDanWeiFengChangSheJiZhi"].ToString();
                        txt_sjz3.Text = settings.Rows[0]["QiMiFuYaDanWeiFengChangSheJiZhi"].ToString();
                        txt_sjz4.Text = settings.Rows[0]["QiMiZhengYaDanWeiMianJiSheJiZhi"].ToString();
                        txt_sjz5.Text = settings.Rows[0]["QiMiFuYaDanWeiMianJiSheJiZhi"].ToString();
                    }
                    if (_settings.dt_qm_Info != null && _settings.dt_qm_Info.Count > 0)
                    {
                        var airTight = _settings.dt_qm_Info;
                        txt_dj1.Text = formula.GetAirTightLevel(airTight).ToString();


                        double zFc = Math.Round(airTight.Sum(t => double.Parse(t.qm_Z_FC)) / airTight.Count, 2);
                        double fFc = Math.Round(airTight.Sum(t => double.Parse(t.qm_F_FC)) / airTight.Count, 2);
                        double zMj = Math.Round(airTight.Sum(t => double.Parse(t.qm_Z_MJ)) / airTight.Count, 2);
                        double fMj = Math.Round(airTight.Sum(t => double.Parse(t.qm_F_MJ)) / airTight.Count, 2);


                        if (zFc >= double.Parse(txt_sjz2.Text))
                            txt_jg2.Text = "合格";
                        else
                            txt_jg2.Text = "不合格";

                        if (fFc >= double.Parse(txt_sjz3.Text))
                            txt_jg3.Text = "合格";
                        else
                            txt_jg3.Text = "不合格";

                        if (zMj >= double.Parse(txt_sjz4.Text))
                            txt_jg4.Text = "合格";
                        else
                            txt_jg4.Text = "不合格";

                        if (fMj <= double.Parse(txt_sjz4.Text))
                            txt_jg5.Text = "合格";
                        else
                            txt_jg5.Text = "不合格";
                    }


                    if (_settings.dt_sm_Info != null && _settings.dt_sm_Info.Count > 0)
                    {
                        txt_dj2.Text = formula.GetWaterTightLevel(_settings.dt_sm_Info).ToString();

                        if (sm_value >= int.Parse(txt_sjz1.Text))
                            txt_jg1.Text = "合格";
                        else
                            txt_jg1.Text = "不合格";
                    }

                    if (_settings.dt_kfy_Info != null && _settings.dt_kfy_Info.Count > 0)
                    {
                        //todo:抗风压
                    }

                    #endregion
                    MessageBox.Show("生成成功！", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Logger.Error(ex);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ExportReport er = new ExportReport(_code);
            er.Show();
        }
    }
}
