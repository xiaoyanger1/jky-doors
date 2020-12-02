using Microsoft.Office.Interop.Word;
//using Microsoft.Office.Interop.Graph;
using text.doors.Common;
using text.doors.dal;
using text.doors.Model.DataBase;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Young.Core.Common;
using text.doors.Default;
using System.Linq;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.IO;

namespace text.doors.Detection
{
    public partial class ExportReport : Form
    {
        private string _tempCode = "";
        private static Young.Core.Logger.ILog Logger = Young.Core.Logger.LoggerManager.Current();

        Formula formula = new Formula();


        public ExportReport(string code)
        {
            InitializeComponent();
            this._tempCode = code;
            cm_Report.SelectedIndex = 0;
        }


        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if (cm_Report.SelectedIndex == 0)
            {
                MessageBox.Show("请选择模板！", "请选择模板！", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                return;
            }

            Eexport(cm_Report.SelectedItem.ToString());
        }


        private void Eexport(string fileName)
        {
            try
            {
                string strResult = string.Empty;
                string strPath = System.Windows.Forms.Application.StartupPath + "\\template";
                string strFile = string.Format(@"{0}\{1}", strPath, fileName);

                FolderBrowserDialog path = new FolderBrowserDialog();
                path.ShowDialog();

                label3.Visible = true;
                if (string.IsNullOrWhiteSpace(path.SelectedPath))
                {
                    return;
                }
                btn_ok.Enabled = false;
                cm_Report.Enabled = false;
                btn_close.Enabled = false;

                string[] name = fileName.Split('.');

                string _name = name[0] + "_" + _tempCode + "." + name[1];

                var saveExcelUrl = path.SelectedPath + "\\" + _name;

                Model_dt_Settings settings = new DAL_dt_Settings().GetInfoByCode(_tempCode);

                if (settings == null)
                {
                    MessageBox.Show("未查询到相关编号!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return;
                }

                var dc = new Dictionary<string, string>();
                if (fileName == "门窗检验报告.doc")
                {
                    dc = GetDWDetectionReport(settings);
                }
                else if (fileName == "实验室记录.doc")
                {
                    dc = GetDetectionReport(settings, saveExcelUrl);
                }

                WordUtility wu = new WordUtility(strFile, saveExcelUrl);
                if (wu.GenerateWordByBookmarks(dc))
                {
                    if (fileName == "门窗检验报告.doc")
                    {
                        if (!string.IsNullOrWhiteSpace(DefaultBase.ImagesName))
                            InsertPtctureToExcel(saveExcelUrl, "图片", DefaultBase.ImagesName);
                    }

                    if (fileName == "实验室记录.doc")
                    {
                        var index = 0;
                        foreach (var item in settings.dt_kfy_Info)
                        {
                            index++;
                            var zitem = new List<double>() { 0 };
                            var fitem = new List<double>() { 0 };
                            zitem.Add(double.Parse(item.z_nd_250));
                            zitem.Add(double.Parse(item.z_nd_500));
                            zitem.Add(double.Parse(item.z_nd_750));
                            zitem.Add(double.Parse(item.z_nd_1000));
                            zitem.Add(double.Parse(item.z_nd_1250));
                            zitem.Add(double.Parse(item.z_nd_1500));
                            zitem.Add(double.Parse(item.z_nd_1750));
                            zitem.Add(double.Parse(item.z_nd_2000));

                            fitem.Add(double.Parse(item.f_nd_250));
                            fitem.Add(double.Parse(item.f_nd_500));
                            fitem.Add(double.Parse(item.f_nd_750));
                            fitem.Add(double.Parse(item.f_nd_1000));
                            fitem.Add(double.Parse(item.f_nd_1250));
                            fitem.Add(double.Parse(item.f_nd_1500));
                            fitem.Add(double.Parse(item.f_nd_1750));
                            fitem.Add(double.Parse(item.f_nd_2000));

                            var file = System.Windows.Forms.Application.StartupPath + ("\\tempImage\\第" + index + "樘" + DateTime.Now.ToString("hhmmdd") + ".jpg");
                            ImageLine(file, item.info_DangH, zitem, fitem);
                            InsertPtctureToExcel(saveExcelUrl, "曲线杆1第" + index + "樘曲线", file);
                        }
                    }

                    label3.Visible = false;

                    MessageBox.Show("导出成功", "导出成功", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                MessageBox.Show("数据出现问题，导出失败!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                this.Close();
            }
        }

        /// <summary>
        /// 导入图片到word
        /// </summary>
        protected void InsertPtctureToExcel(string file, string tag, string imageName)
        {
            object Nothing = System.Reflection.Missing.Value;
            //创建一个名为wordApp的组件对象
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //word文档位置

            object filename = file;

            //定义该插入图片是否为外部链接
            object linkToFile = true;

            //定义插入图片是否随word文档一起保存
            object saveWithDocument = true;

            //打开word文档
            Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Open(ref filename, ref Nothing, ref Nothing, ref Nothing,
               ref Nothing, ref Nothing, ref Nothing, ref Nothing,
               ref Nothing, ref Nothing, ref Nothing, ref Nothing,
               ref Nothing, ref Nothing, ref Nothing, ref Nothing);
            try
            {
                //标签
                object bookMark = tag;
                //图片
                string replacePic = imageName;

                if (doc.Bookmarks.Exists(Convert.ToString(bookMark)) == true)
                {
                    //查找书签
                    doc.Bookmarks.get_Item(ref bookMark).Select();
                    //设置图片位置
                    wordApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;

                    //在书签的位置添加图片
                    InlineShape inlineShape = wordApp.Selection.InlineShapes.AddPicture(replacePic, ref linkToFile, ref saveWithDocument, ref Nothing);
                    inlineShape.ConvertToShape().WrapFormat.Type = WdWrapType.wdWrapFront;
                    //设置图片大小
                    if (tag == "图片")
                    {
                        inlineShape.Width = 500;
                        inlineShape.Height = 300;
                    }
                    else
                    {
                        inlineShape.Width = 250;
                        inlineShape.Height = 215;
                    }
                    doc.Save();
                }
                else
                {
                    doc.Close(ref Nothing, ref Nothing, ref Nothing);
                }
            }
            catch
            {
            }
            finally
            {
                //word文档中不存在该书签，关闭文档
                doc.Close(ref Nothing, ref Nothing, ref Nothing);
            }
        }

        /// <summary>
        /// 获取门窗检测报告文档
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetDWDetectionReport(Model_dt_Settings settings)
        {
            Dictionary<string, string> dc = new Dictionary<string, string>();
            dc.Add("检测条件第0樘型号规格", settings.GuiGeXingHao);
            dc.Add("检测条件第0樘大气压力", settings.DaQiYaLi);
            dc.Add("检测条件第0樘委托单位", settings.WeiTuoDanWei);
            dc.Add("检测条件第0樘委托单位重复1", settings.WeiTuoDanWei);
            dc.Add("检测条件第0樘工程名称", settings.GongChengMingCheng);
            dc.Add("检测条件第0樘开启方式", settings.KaiQiFangShi);
            dc.Add("检测条件第0樘开启缝长", settings.KaiQiFengChang);
            dc.Add("检测条件第0樘当前温度", settings.DangQianWenDu);
            dc.Add("检测条件第0樘总面积", settings.ZongMianJi);
            dc.Add("检测条件第0樘最大玻璃", settings.ZuiDaBoLi);
            dc.Add("检测条件第0樘来样方式", settings.CaiYangFangShi);
            dc.Add("检测条件第0樘来样方式重复1", settings.CaiYangFangShi);
            dc.Add("检测条件第0樘样品名称", settings.YangPinMingCheng);
            dc.Add("检测条件第0樘样品名称重复1", settings.YangPinMingCheng);
            dc.Add("检测条件第0樘框扇密封", settings.KuangShanMiFang);
            dc.Add("检测条件第0樘检验数量", settings.GuiGeShuLiang);
            dc.Add("检测条件第0樘检验日期", settings.JianYanRiQi);
            dc.Add("检测条件第0樘检验日期重复1", settings.JianYanRiQi);
            dc.Add("检测条件第0樘检验日期重复2", settings.JianYanRiQi);
            dc.Add("检测条件第0樘检验编号", settings.dt_Code);
            dc.Add("检测条件第0樘检验编号重复1", settings.dt_Code);
            dc.Add("检测条件第0樘检验编号重复2", settings.dt_Code);
            dc.Add("检测条件第0樘检验编号重复3", settings.dt_Code);
            dc.Add("检测条件第0樘检验项目", settings.JianYanXiangMu);
            dc.Add("检测条件第0樘正压气密等级设计值", settings.ZhengYaQiMiDengJiSheJiZhi);
            dc.Add("检测条件第0樘负压气密等级设计值", settings.FuYaQiMiDengJiSheJiZhi);
            dc.Add("检测条件第0樘抗风压设计值", settings.KangFengYaSheJiZhi);
            dc.Add("检测条件第0樘抗风压等级设计值", settings.KangFengYaDengJiSheJiZhi);
            if (settings.dt_kfy_Info.Count > 0)
            {
                var value = new List<int>();
                foreach (var item in settings.dt_kfy_Info)
                {
                    value.Add(int.Parse(item.p3));
                    value.Add(int.Parse(item._p3));
                }
                var minValue = value.Min(t => t);
                var level = Formula.GetWindPressureLevel(minValue).ToString();
                dc.Add("检测条件第0樘抗风压等级", level);
            }
            else
            {
                dc.Add("检测条件第0樘抗风压等级", "--");
            }
            if (settings.dt_qm_Info.Count > 0)
            {
                var z_qm_level = formula.Get_Z_AirTightLevel(settings.dt_qm_Info);
                dc.Add("检测条件第0樘综合气密正压等级", z_qm_level.ToString());
                var f_qm_level = formula.Get_F_AirTightLevel(settings.dt_qm_Info);
                dc.Add("检测条件第0樘综合气密负压等级", f_qm_level.ToString());
            }
            else
            {
                dc.Add("检测条件第0樘综合气密正压等级", "--");
                dc.Add("检测条件第0樘综合气密负压等级", "--");
            }

            if (settings.dt_sm_Info.Count > 0)
            {
                var sm_level = formula.GetWaterTightLevel(settings.dt_sm_Info);
                var YL = formula.GetWaterTightPressure(settings.dt_sm_Info);

                dc.Add("检测条件第0樘水密等级", sm_level.ToString());
                //  dc.Add("检测条件第0樘水密等级设计值", sm_level.ToString());
                dc.Add("检测条件第0樘水密等级设计值", settings.ShuiMiDengJiSheJiZhi.ToString());
                dc.Add("检测条件第0樘水密保持风压", YL.ToString());
            }
            else
            {
                dc.Add("检测条件第0樘水密等级", "--");
                dc.Add("检测条件第0樘水密等级设计值", "--");
                dc.Add("检测条件第0樘水密保持风压", "--");
            }

            if (settings.dt_kfy_Info.Count > 0)
            {
                var value = new List<int>();
                var _value = new List<int>();

                settings.dt_kfy_Info.ForEach(t => value.Add(int.Parse(t.p1)));
                settings.dt_kfy_Info.ForEach(t => _value.Add(int.Parse(t._p1)));


                dc.Add("检测条件第0樘强度正P1", value.Min(t => t).ToString());
                dc.Add("检测条件第0樘强度负P1", _value.Min(t => t).ToString());

                value = new List<int>();
                _value = new List<int>();
                settings.dt_kfy_Info.ForEach(t => value.Add(int.Parse(t.p2)));
                settings.dt_kfy_Info.ForEach(t => _value.Add(int.Parse(t._p2)));

                dc.Add("检测条件第0樘强度正P2", value.Min(t => t).ToString());
                dc.Add("检测条件第0樘强度负P2", _value.Min(t => t).ToString());


                value = new List<int>();
                _value = new List<int>();
                settings.dt_kfy_Info.ForEach(t => value.Add(int.Parse(t.p3)));
                settings.dt_kfy_Info.ForEach(t => _value.Add(int.Parse(t._p3)));
                dc.Add("检测条件第0樘强度正P3", value.Min(t => t).ToString());
                dc.Add("检测条件第0樘强度负P3", _value.Min(t => t).ToString());

                //dc.Add("检测条件第0樘强度正P4", "正P4");
                //dc.Add("检测条件第0樘强度负P4", "负P4");
            }
            else
            {
                dc.Add("检测条件第0樘强度正P1", "--");
                dc.Add("检测条件第0樘强度负P1", "--");
                dc.Add("检测条件第0樘强度正P2", "--");
                dc.Add("检测条件第0樘强度负P2", "--");
                dc.Add("检测条件第0樘强度正P3", "--");
                dc.Add("检测条件第0樘强度负P3", "--");
                //dc.Add("检测条件第0樘强度正P4", "--");
                //dc.Add("检测条件第0樘强度负P4", "--");
            }


            double zFc = 0, fFc = 0, zMj = 0, fMj = 0;
            if (settings.dt_qm_Info != null && settings.dt_qm_Info.Count > 0)
            {
                zFc = Math.Round(settings.dt_qm_Info.Sum(t => double.Parse(t.qm_Z_FC)) / settings.dt_qm_Info.Count, 2);
                fFc = Math.Round(settings.dt_qm_Info.Sum(t => double.Parse(t.qm_F_FC)) / settings.dt_qm_Info.Count, 2);
                zMj = Math.Round(settings.dt_qm_Info.Sum(t => double.Parse(t.qm_Z_MJ)) / settings.dt_qm_Info.Count, 2);
                fMj = Math.Round(settings.dt_qm_Info.Sum(t => double.Parse(t.qm_F_MJ)) / settings.dt_qm_Info.Count, 2);
            }

            dc.Add("检测条件第0樘正缝长渗透量", zFc.ToString());
            dc.Add("检测条件第0樘负缝长渗透量", fFc.ToString());
            dc.Add("检测条件第0樘正面积渗透量", zMj.ToString());
            dc.Add("检测条件第0樘负面积渗透量", fMj.ToString());
            dc.Add("检测条件第0樘玻璃品种", settings.BoLiPinZhong);
            dc.Add("检测条件第0樘玻璃密封", settings.BoLiMiFeng);
            dc.Add("检测条件第0樘生产单位", settings.ShengChanDanWei);
            dc.Add("检测条件第0樘负责人", settings.WeiTuoRen);
            dc.Add("检测条件第0樘镶嵌方式", settings.XiangQianFangShi);
            return dc;
        }


        #region 获取检测报告文档
        /// <summary>
        /// 获取检测报告文档
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetDetectionReport(Model_dt_Settings settings, string file)
        {
            Dictionary<string, string> dc = new Dictionary<string, string>();

            #region 基础
            dc.Add("检测条件第0樘杆件长度", settings.GanJianChangDu);
            dc.Add("实验室气压", settings.DaQiYaLi);
            dc.Add("实验室温度", settings.DangQianWenDu);
            dc.Add("集流管经", (DefaultBase._D * 1000).ToString());
            dc.Add("检测条件第0樘五金件状况", settings.WuJinJianZhuangKuang);
            dc.Add("检测条件第0樘型号规格", settings.GuiGeXingHao);
            dc.Add("检测条件第0樘大气压力", settings.DaQiYaLi);
            dc.Add("检测条件第0樘委托单位", settings.WeiTuoDanWei);
            dc.Add("检测条件第0樘委托单位重复1", settings.WeiTuoDanWei);
            dc.Add("检测条件第0樘委托单位重复2", settings.WeiTuoDanWei);
            dc.Add("检测条件第0樘委托单位重复3", settings.WeiTuoDanWei);
            dc.Add("检测条件第0樘工程名称", settings.GongChengMingCheng);
            dc.Add("检测条件第0樘工程地点", settings.GongChengDiDian);
            dc.Add("检测条件第0樘开启缝长", settings.KaiQiFengChang);
            dc.Add("检测条件第0樘开启缝长重复1", settings.KaiQiFengChang);
            dc.Add("检测条件第0樘当前温度", settings.DangQianWenDu);
            dc.Add("检测条件第0樘总面积", settings.ZongMianJi);
            dc.Add("检测条件第0樘总面积重复2", settings.ZongMianJi);
            dc.Add("检测条件第0樘最大玻璃", settings.ZuiDaBoLi);
            dc.Add("检测条件第0樘来样方式", settings.CaiYangFangShi);
            dc.Add("检测条件第0樘样品名称", settings.YangPinMingCheng);
            dc.Add("检测条件第0樘样品名称重复1", settings.YangPinMingCheng);
            dc.Add("检测条件第0樘样品名称重复2", settings.YangPinMingCheng);
            dc.Add("检测条件第0樘样品名称重复3", settings.YangPinMingCheng);
            dc.Add("检测条件第0樘框扇密封", settings.KuangShanMiFang);
            dc.Add("检测条件第0樘检验数量", settings.GuiGeShuLiang);
            dc.Add("检测条件第0樘检验编号", settings.dt_Code);
            dc.Add("检测条件第0樘检验编号重复1", settings.dt_Code);
            dc.Add("检测条件第0樘检验编号重复2", settings.dt_Code);
            dc.Add("检测条件第0樘检验编号重复3", settings.dt_Code);
            dc.Add("检测条件第0樘检验编号重复4", settings.dt_Code);
            dc.Add("检测条件第0樘检验编号重复5", settings.dt_Code);
            dc.Add("检测条件第0樘检验日期重复1", settings.JianYanRiQi);
            dc.Add("检测条件第0樘检验日期重复2", settings.JianYanRiQi);
            dc.Add("检测条件第0樘检验日期重复3", settings.JianYanRiQi);
            dc.Add("检测条件第0樘检验日期重复4", settings.JianYanRiQi);
            dc.Add("检测条件第0樘检验日期重复5", settings.JianYanRiQi);
            dc.Add("检测条件第0樘检验日期重复6", settings.JianYanRiQi);
            dc.Add("检测条件第0樘检验日期重复7", settings.JianYanRiQi);
            dc.Add("检测条件第0樘检验项目", settings.JianYanXiangMu);
            dc.Add("检测条件第0樘正压气密等级设计值", settings.ZhengYaQiMiDengJiSheJiZhi);
            dc.Add("检测条件第0樘负压气密等级设计值", settings.FuYaQiMiDengJiSheJiZhi);
            dc.Add("检测条件第0樘水密等级设计值", settings.ShuiMiDengJiSheJiZhi);
            dc.Add("检测条件第0樘玻璃厚度", settings.BoLiHouDu);
            dc.Add("检测条件第0樘玻璃品种", settings.BoLiPinZhong);
            dc.Add("检测条件第0樘玻璃密封", settings.BoLiMiFeng);
            dc.Add("检测条件第0樘抗风压等级设计值", settings.KangFengYaDengJiSheJiZhi);
            dc.Add("检测条件第0樘镶嵌方式", settings.XiangQianFangShi);


            dc.Add("检测条件第0樘单扇单锁点", settings.DanShanDanSuoDian);

            #endregion

            if (settings.dt_qm_Info.Count > 0)
            {
                #region 气密
                //检测条件第0樘综合气密等级
                var z_qm_level = formula.Get_Z_AirTightLevel(settings.dt_qm_Info);
                dc.Add("检测条件第0樘正压气密等级", z_qm_level.ToString());
                var f_qm_level = formula.Get_F_AirTightLevel(settings.dt_qm_Info);
                dc.Add("检测条件第0樘负压气密等级", f_qm_level.ToString());

                if (settings.dt_qm_Info != null && settings.dt_qm_Info.Count > 0)
                {
                    for (int i = 0; i < settings.dt_qm_Info.Count; i++)
                    {
                        if (i == 0)
                        {
                            dc.Add("气密检测第1樘总的渗透正升压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_z_zd100).ToString("#0.00"));
                            dc.Add("气密检测第1樘总的渗透正降压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_j_z_zd100).ToString("#0.00"));
                            dc.Add("气密检测第1樘总的渗透负升压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_f_zd100).ToString("#0.00"));
                            dc.Add("气密检测第1樘总的渗透负降压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_j_f_zd100).ToString("#0.00"));
                            dc.Add("气密检测第1樘附加渗透负升压150帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_f_fj150).ToString("#0.00"));
                            dc.Add("气密检测第1樘总的渗透负升压150帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_f_zd150).ToString("#0.00"));
                            dc.Add("气密检测第1樘总的渗透正升压150帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_z_zd150).ToString("#0.00"));
                            dc.Add("气密检测第1樘附加渗透正升压150帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_z_fj150).ToString("#0.00"));
                            dc.Add("气密检测第1樘附加渗透正升压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_z_fj100).ToString("#0.00"));
                            dc.Add("气密检测第1樘附加渗透正降压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_j_z_fj100).ToString("#0.00"));
                            dc.Add("气密检测第1樘附加渗透负升压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_f_fj100).ToString("#0.00"));
                            dc.Add("气密检测第1樘附加渗透负降压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_j_f_fj100).ToString("#0.00"));
                            dc.Add("流量第一樘升100附加", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_z_fj100)).ToString("#0.00"));
                            dc.Add("流量第一樘升150附加", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_z_fj150)).ToString("#0.00"));
                            dc.Add("流量第一樘负升150附加", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_f_fj150)).ToString("#0.00"));
                            dc.Add("流量第一樘负升100附加", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_f_fj100)).ToString("#0.00"));
                            dc.Add("流量第一樘负升100总的", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_f_zd100)).ToString("#0.00"));
                            dc.Add("流量第一樘升100总的", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_z_zd100)).ToString("#0.00"));
                            dc.Add("流量第一樘升150总的", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_z_zd150)).ToString("#0.00"));
                            dc.Add("流量第一樘负升150总的", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_f_zd150)).ToString("#0.00"));
                            dc.Add("流量第一樘降100总的", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_j_z_zd100)).ToString("#0.00"));
                            dc.Add("流量第一樘降100附加", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_j_z_fj100)).ToString("#0.00"));
                            dc.Add("流量第一樘负降100总的", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_j_f_zd100)).ToString("#0.00"));
                            dc.Add("流量第一樘负降100附加", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_j_f_fj100)).ToString("#0.00"));
                        }
                        if (i == 1)
                        {
                            dc.Add("气密检测第2樘总的渗透正升压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_z_zd100).ToString("#0.00"));
                            dc.Add("气密检测第2樘总的渗透正升压150帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_z_zd150).ToString("#0.00"));
                            dc.Add("气密检测第2樘总的渗透负升压150帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_f_zd150).ToString("#0.00"));
                            dc.Add("气密检测第2樘附加渗透负升压150帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_f_fj150).ToString("#0.00"));
                            dc.Add("气密检测第2樘附加渗透正升压150帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_z_fj150).ToString("#0.00"));
                            dc.Add("气密检测第2樘总的渗透正降压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_j_z_zd100).ToString("#0.00"));
                            dc.Add("气密检测第2樘总的渗透负升压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_f_zd100).ToString("#0.00"));
                            dc.Add("气密检测第2樘总的渗透负降压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_j_f_zd100).ToString("#0.00"));
                            dc.Add("气密检测第2樘附加渗透正升压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_z_fj100).ToString("#0.00"));
                            dc.Add("气密检测第2樘附加渗透正降压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_j_z_fj100).ToString("#0.00"));
                            dc.Add("气密检测第2樘附加渗透负升压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_f_fj100).ToString("#0.00"));
                            dc.Add("气密检测第2樘附加渗透负降压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_j_f_fj100).ToString("#0.00"));

                            //第二樘
                            dc.Add("流量第二樘升100附加", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_z_fj100)).ToString("#0.00"));
                            dc.Add("流量第二樘升150附加", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_z_fj150)).ToString("#0.00"));
                            dc.Add("流量第二樘负升150附加", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_f_fj150)).ToString("#0.00"));
                            dc.Add("流量第二樘负升100附加", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_f_fj100)).ToString("#0.00"));
                            dc.Add("流量第二樘负升100总的", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_f_zd100)).ToString("#0.00"));
                            dc.Add("流量第二樘升100总的", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_z_zd100)).ToString("#0.00"));
                            dc.Add("流量第二樘升150总的", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_z_zd150)).ToString("#0.00"));
                            dc.Add("流量第二樘负升150总的", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_f_zd150)).ToString("#0.00"));
                            dc.Add("流量第二樘降100总的", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_j_z_zd100)).ToString("#0.00"));
                            dc.Add("流量第二樘降100附加", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_j_z_fj100)).ToString("#0.00"));
                            dc.Add("流量第二樘负降100总的", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_j_f_zd100)).ToString("#0.00"));
                            dc.Add("流量第二樘负降100附加", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_j_f_fj100)).ToString("#0.00"));
                        }
                        if (i == 2)
                        {
                            dc.Add("气密检测第3樘总的渗透正升压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_z_zd100).ToString("#0.00"));
                            dc.Add("气密检测第3樘总的渗透正升压150帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_z_zd150).ToString("#0.00"));
                            dc.Add("气密检测第3樘总的渗透负升压150帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_f_zd150).ToString("#0.00"));
                            dc.Add("气密检测第3樘附加渗透负升压150帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_f_fj150).ToString("#0.00"));
                            dc.Add("气密检测第3樘附加渗透正升压150帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_z_fj150).ToString("#0.00"));
                            dc.Add("气密检测第3樘总的渗透正降压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_j_z_zd100).ToString("#0.00"));
                            dc.Add("气密检测第3樘总的渗透负升压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_f_zd100).ToString("#0.00"));
                            dc.Add("气密检测第3樘总的渗透负降压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_j_f_zd100).ToString("#0.00"));
                            dc.Add("气密检测第3樘附加渗透正升压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_z_fj100).ToString("#0.00"));
                            dc.Add("气密检测第3樘附加渗透正降压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_j_z_fj100).ToString("#0.00"));
                            dc.Add("气密检测第3樘附加渗透负升压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_s_f_fj100).ToString("#0.00"));
                            dc.Add("气密检测第3樘附加渗透负降压100帕时风速", double.Parse(settings.dt_qm_Info[i].qm_j_f_fj100).ToString("#0.00"));
                            //流量
                            dc.Add("流量第三樘负升100总的", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_f_zd100)).ToString("#0.00"));
                            dc.Add("流量第三樘升100总的", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_z_zd100)).ToString("#0.00"));
                            dc.Add("流量第三樘负升100附加", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_f_fj100)).ToString("#0.00"));
                            dc.Add("流量第三樘升100附加", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_z_fj100)).ToString("#0.00"));
                            dc.Add("流量第三樘升150总的", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_z_zd150)).ToString("#0.00"));
                            dc.Add("流量第三樘负升150总的", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_f_zd150)).ToString("#0.00"));
                            dc.Add("流量第三樘升150附加", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_z_fj150)).ToString("#0.00"));
                            dc.Add("流量第三樘负升150附加", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_s_f_fj150)).ToString("#0.00"));
                            dc.Add("流量第三樘负降100总的", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_j_f_zd100)).ToString("#0.00"));
                            dc.Add("流量第三樘降100总的", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_j_z_zd100)).ToString("#0.00"));
                            dc.Add("流量第三樘降100附加", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_j_z_fj100)).ToString("#0.00"));
                            dc.Add("流量第三樘负降100附加", formula.MathFlow(double.Parse(settings.dt_qm_Info[i].qm_j_f_fj100)).ToString("#0.00"));
                        }
                    }
                }
                #endregion
            }
            else
            {
                dc.Add("检测条件第0樘综合气密等级", "--");
            }
            if (settings.dt_sm_Info.Count > 0)
            {
                #region 水密
                var sm_level = formula.GetWaterTightLevel(settings.dt_sm_Info);
                dc.Add("检测条件第0樘水密等级", sm_level.ToString());

                for (int i = 0; i < settings.dt_sm_Info.Count; i++)
                {
                    if (i == 0)
                        dc.Add("检测条件第0樘水密检测方法", settings.dt_sm_Info[i].Method);


                    string[] arr = settings.dt_sm_Info[i].sm_PaDesc.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var one = "";
                    var two = "";
                    if (arr.Length == 0)
                        continue;

                    else if (arr.Length == 1)
                        one = arr[0];

                    else if (arr.Length == 2)
                    {
                        one = arr[0];
                        two = arr[1];
                    }

                    if (two.Contains("▲") || two.Contains("●"))
                    {
                        if (i == 0)
                        {
                            //if (settings.dt_sm_Info[i].sm_Pa == "0")
                            //{
                            //    dc.Add("水密检测第1樘压力0帕状态", one);
                            //    dc.Add("水密检测第1樘压力0帕部位", two);
                            //}
                            if (settings.dt_sm_Info[i].sm_Pa == "0")
                            {
                                dc.Add("水密检测第1樘压力100帕状态", one);
                                dc.Add("水密检测第1樘压力100帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "100")
                            {
                                dc.Add("水密检测第1樘压力150帕状态", one);
                                dc.Add("水密检测第1樘压力150帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "150")
                            {
                                dc.Add("水密检测第1樘压力200帕状态", one);
                                dc.Add("水密检测第1樘压力200帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "200")
                            {
                                dc.Add("水密检测第1樘压力250帕状态", one);
                                dc.Add("水密检测第1樘压力250帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "250")
                            {
                                dc.Add("水密检测第1樘压力300帕状态", one);
                                dc.Add("水密检测第1樘压力300帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "300")
                            {
                                dc.Add("水密检测第1樘压力350帕状态", one);
                                dc.Add("水密检测第1樘压力350帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "350")
                            {
                                dc.Add("水密检测第1樘压力400帕状态", one);
                                dc.Add("水密检测第1樘压力400帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "400")
                            {
                                dc.Add("水密检测第1樘压力500帕状态", one);
                                dc.Add("水密检测第1樘压力500帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "500")
                            {
                                dc.Add("水密检测第1樘压力600帕状态", one);
                                dc.Add("水密检测第1樘压力600帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "600")
                            {
                                dc.Add("水密检测第1樘压力700帕状态", one);
                                dc.Add("水密检测第1樘压力700帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "700")
                            {
                                dc.Add("水密检测第1樘压力700帕状态", one);
                                dc.Add("水密检测第1樘压力700帕部位", two);
                            }
                            dc.Add("水密检测第1樘水密实验备注", settings.dt_sm_Info[i].sm_Remark);

                        }
                        if (i == 1)
                        {
                            //if (settings.dt_sm_Info[i].sm_Pa == "0")
                            //{
                            //    dc.Add("水密检测第2樘压力0帕状态", one);
                            //    dc.Add("水密检测第2樘压力0帕部位", two);
                            //}
                            if (settings.dt_sm_Info[i].sm_Pa == "0")
                            {
                                dc.Add("水密检测第2樘压力100帕状态", one);
                                dc.Add("水密检测第2樘压力100帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "100")
                            {
                                dc.Add("水密检测第2樘压力150帕状态", one);
                                dc.Add("水密检测第2樘压力150帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "150")
                            {
                                dc.Add("水密检测第2樘压力200帕状态", one);
                                dc.Add("水密检测第2樘压力200帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "200")
                            {
                                dc.Add("水密检测第2樘压力250帕状态", one);
                                dc.Add("水密检测第2樘压力250帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "250")
                            {
                                dc.Add("水密检测第2樘压力300帕状态", one);
                                dc.Add("水密检测第2樘压力300帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "300")
                            {
                                dc.Add("水密检测第2樘压力350帕状态", one);
                                dc.Add("水密检测第2樘压力350帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "350")
                            {
                                dc.Add("水密检测第2樘压力400帕状态", one);
                                dc.Add("水密检测第2樘压力400帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "400")
                            {
                                dc.Add("水密检测第2樘压力500帕状态", one);
                                dc.Add("水密检测第2樘压力500帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "500")
                            {
                                dc.Add("水密检测第2樘压力600帕状态", one);
                                dc.Add("水密检测第2樘压力600帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "600")
                            {
                                dc.Add("水密检测第2樘压力700帕状态", one);
                                dc.Add("水密检测第2樘压力700帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "700")
                            {
                                dc.Add("水密检测第2樘压力700帕状态", one);
                                dc.Add("水密检测第2樘压力700帕部位", two);
                            }
                            dc.Add("水密检测第2樘水密实验备注", settings.dt_sm_Info[i].sm_Remark);
                        }
                        if (i == 2)
                        {
                            //if (settings.dt_sm_Info[i].sm_Pa == "0")
                            //{
                            //    dc.Add("水密检测第3樘压力0帕状态", one);
                            //    dc.Add("水密检测第3樘压力0帕部位", two);
                            //}
                            if (settings.dt_sm_Info[i].sm_Pa == "0")
                            {
                                dc.Add("水密检测第3樘压力100帕状态", one);
                                dc.Add("水密检测第3樘压力100帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "100")
                            {
                                dc.Add("水密检测第3樘压力150帕状态", one);
                                dc.Add("水密检测第3樘压力150帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "150")
                            {
                                dc.Add("水密检测第3樘压力200帕状态", one);
                                dc.Add("水密检测第3樘压力200帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "200")
                            {
                                dc.Add("水密检测第3樘压力250帕状态", one);
                                dc.Add("水密检测第3樘压力250帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "250")
                            {
                                dc.Add("水密检测第3樘压力300帕状态", one);
                                dc.Add("水密检测第3樘压力300帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "300")
                            {
                                dc.Add("水密检测第3樘压力350帕状态", one);
                                dc.Add("水密检测第3樘压力350帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "350")
                            {
                                dc.Add("水密检测第3樘压力400帕状态", one);
                                dc.Add("水密检测第3樘压力400帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "400")
                            {
                                dc.Add("水密检测第3樘压力500帕状态", one);
                                dc.Add("水密检测第3樘压力500帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "500")
                            {
                                dc.Add("水密检测第3樘压力600帕状态", one);
                                dc.Add("水密检测第3樘压力600帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "600")
                            {
                                dc.Add("水密检测第3樘压力700帕状态", one);
                                dc.Add("水密检测第3樘压力700帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "700")
                            {
                                dc.Add("水密检测第2樘压力700帕状态", one);
                                dc.Add("水密检测第2樘压力700帕部位", two);
                            }
                            dc.Add("水密检测第3樘水密实验备注", settings.dt_sm_Info[i].sm_Remark);
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            if (settings.dt_sm_Info[i].sm_Pa == "0")
                            {
                                dc.Add("水密检测第1樘压力0帕状态", one);
                                dc.Add("水密检测第1樘压力0帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "100")
                            {
                                dc.Add("水密检测第1樘压力100帕状态", one);
                                dc.Add("水密检测第1樘压力100帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "150")
                            {
                                dc.Add("水密检测第1樘压力150帕状态", one);
                                dc.Add("水密检测第1樘压力150帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "200")
                            {
                                dc.Add("水密检测第1樘压力200帕状态", one);
                                dc.Add("水密检测第1樘压力200帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "250")
                            {
                                dc.Add("水密检测第1樘压力250帕状态", one);
                                dc.Add("水密检测第1樘压力250帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "300")
                            {
                                dc.Add("水密检测第1樘压力300帕状态", one);
                                dc.Add("水密检测第1樘压力300帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "350")
                            {
                                dc.Add("水密检测第1樘压力350帕状态", one);
                                dc.Add("水密检测第1樘压力350帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "400")
                            {
                                dc.Add("水密检测第1樘压力400帕状态", one);
                                dc.Add("水密检测第1樘压力400帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "500")
                            {
                                dc.Add("水密检测第1樘压力500帕状态", one);
                                dc.Add("水密检测第1樘压力500帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "600")
                            {
                                dc.Add("水密检测第1樘压力600帕状态", one);
                                dc.Add("水密检测第1樘压力600帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "700")
                            {
                                dc.Add("水密检测第1樘压力700帕状态", one);
                                dc.Add("水密检测第1樘压力700帕部位", two);
                            }
                            dc.Add("水密检测第1樘水密实验备注", settings.dt_sm_Info[i].sm_Remark);

                        }
                        if (i == 1)
                        {
                            if (settings.dt_sm_Info[i].sm_Pa == "0")
                            {
                                dc.Add("水密检测第2樘压力0帕状态", one);
                                dc.Add("水密检测第2樘压力0帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "100")
                            {
                                dc.Add("水密检测第2樘压力100帕状态", one);
                                dc.Add("水密检测第2樘压力100帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "150")
                            {
                                dc.Add("水密检测第2樘压力150帕状态", one);
                                dc.Add("水密检测第2樘压力150帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "200")
                            {
                                dc.Add("水密检测第2樘压力200帕状态", one);
                                dc.Add("水密检测第2樘压力200帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "250")
                            {
                                dc.Add("水密检测第2樘压力250帕状态", one);
                                dc.Add("水密检测第2樘压力250帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "300")
                            {
                                dc.Add("水密检测第2樘压力300帕状态", one);
                                dc.Add("水密检测第2樘压力300帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "350")
                            {
                                dc.Add("水密检测第2樘压力350帕状态", one);
                                dc.Add("水密检测第2樘压力350帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "400")
                            {
                                dc.Add("水密检测第2樘压力400帕状态", one);
                                dc.Add("水密检测第2樘压力400帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "500")
                            {
                                dc.Add("水密检测第2樘压力500帕状态", "36");
                                dc.Add("水密检测第2樘压力500帕部位", "36");
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "600")
                            {
                                dc.Add("水密检测第2樘压力600帕状态", one);
                                dc.Add("水密检测第2樘压力600帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "700")
                            {
                                dc.Add("水密检测第2樘压力700帕状态", one);
                                dc.Add("水密检测第2樘压力700帕部位", two);
                            }
                            dc.Add("水密检测第2樘水密实验备注", settings.dt_sm_Info[i].sm_Remark);
                        }
                        if (i == 2)
                        {
                            if (settings.dt_sm_Info[i].sm_Pa == "0")
                            {
                                dc.Add("水密检测第3樘压力0帕状态", one);
                                dc.Add("水密检测第3樘压力0帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "100")
                            {
                                dc.Add("水密检测第3樘压力100帕状态", one);
                                dc.Add("水密检测第3樘压力100帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "150")
                            {
                                dc.Add("水密检测第3樘压力150帕状态", one);
                                dc.Add("水密检测第3樘压力150帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "200")
                            {
                                dc.Add("水密检测第3樘压力200帕状态", one);
                                dc.Add("水密检测第3樘压力200帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "250")
                            {
                                dc.Add("水密检测第3樘压力250帕状态", one);
                                dc.Add("水密检测第3樘压力250帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "300")
                            {
                                dc.Add("水密检测第3樘压力300帕状态", one);
                                dc.Add("水密检测第3樘压力300帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "350")
                            {
                                dc.Add("水密检测第3樘压力350帕状态", one);
                                dc.Add("水密检测第3樘压力350帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "400")
                            {
                                dc.Add("水密检测第3樘压力400帕状态", one);
                                dc.Add("水密检测第3樘压力400帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "500")
                            {
                                dc.Add("水密检测第3樘压力500帕状态", "36");
                                dc.Add("水密检测第3樘压力500帕部位", "36");
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "600")
                            {
                                dc.Add("水密检测第3樘压力600帕状态", one);
                                dc.Add("水密检测第3樘压力600帕部位", two);
                            }
                            if (settings.dt_sm_Info[i].sm_Pa == "700")
                            {
                                dc.Add("水密检测第3樘压力700帕状态", one);
                                dc.Add("水密检测第3樘压力700帕部位", two);
                            }
                            dc.Add("水密检测第3樘水密实验备注", settings.dt_sm_Info[i].sm_Remark);
                        }
                    }
                }
                #endregion
            }
            else
            {
                dc.Add("检测条件第0樘水密等级", "--");
            }

            #region  缝长计算
            double zFc = 0, fFc = 0, zMj = 0, fMj = 0;
            if (settings.dt_qm_Info != null && settings.dt_qm_Info.Count > 0)
            {
                zFc = Math.Round(settings.dt_qm_Info.Sum(t => double.Parse(t.qm_Z_FC)) / settings.dt_qm_Info.Count, 2);
                fFc = Math.Round(settings.dt_qm_Info.Sum(t => double.Parse(t.qm_F_FC)) / settings.dt_qm_Info.Count, 2);
                zMj = Math.Round(settings.dt_qm_Info.Sum(t => double.Parse(t.qm_Z_MJ)) / settings.dt_qm_Info.Count, 2);
                fMj = Math.Round(settings.dt_qm_Info.Sum(t => double.Parse(t.qm_F_MJ)) / settings.dt_qm_Info.Count, 2);
            }

            dc.Add("检测条件第0樘正缝长渗透量", zFc.ToString());
            dc.Add("检测条件第0樘负缝长渗透量", fFc.ToString());
            dc.Add("检测条件第0樘正面积渗透量", zMj.ToString());
            dc.Add("检测条件第0樘负面积渗透量", fMj.ToString());


            #endregion



            #region 抗风压
            if (settings.dt_kfy_Info.Count > 0)
            {
                var value = new List<int>();

                foreach (var item in settings.dt_kfy_Info)
                {
                    value.Add(int.Parse(item.p3));
                    value.Add(int.Parse(item._p3));
                }
                var minValue = value.Min(t => t);

                dc.Add("检测条件第0樘抗风压等级", Formula.GetWindPressureLevel(minValue).ToString());


                dc.Add("检测条件第0樘单扇单锁点位移选择", settings.dt_kfy_Info[0].CheckLock == 0 ? "--" : settings.dt_kfy_Info[0].CheckLock.ToString());

                for (int i = 0; i < settings.dt_kfy_Info.Count; i++)
                {
                    var kfy = settings.dt_kfy_Info[i];
                    #region 第一樘
                    var index = i + 1;
                    dc.Add($"强度检测第{index}樘正压250帕位移1", double.Parse(kfy.z_one_250).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压250帕位移2", double.Parse(kfy.z_two_250).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压250帕位移3", double.Parse(kfy.z_three_250).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压250帕第一组挠度", double.Parse(kfy.z_nd_250).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压500帕位移1", double.Parse(kfy.z_one_500).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压500帕位移2", double.Parse(kfy.z_two_500).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压500帕位移3", double.Parse(kfy.z_three_500).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压500帕第一组挠度", double.Parse(kfy.z_nd_500).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压750帕位移1", double.Parse(kfy.z_one_750).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压750帕位移2", double.Parse(kfy.z_two_750).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压750帕位移3", double.Parse(kfy.z_three_750).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压750帕第一组挠度", double.Parse(kfy.z_nd_750).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压1000帕位移1", double.Parse(kfy.z_one_1000).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压1000帕位移2", double.Parse(kfy.z_two_1000).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压1000帕位移3", double.Parse(kfy.z_three_1000).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压1000帕第一组挠度", double.Parse(kfy.z_nd_1000).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压1250帕位移1", double.Parse(kfy.z_one_1250).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压1250帕位移2", double.Parse(kfy.z_two_1250).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压1250帕位移3", double.Parse(kfy.z_three_1250).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压1250帕第一组挠度", double.Parse(kfy.z_nd_1250).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压1500帕位移1", double.Parse(kfy.z_one_1500).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压1500帕位移2", double.Parse(kfy.z_two_1500).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压1500帕位移3", double.Parse(kfy.z_three_1500).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压1500帕第一组挠度", double.Parse(kfy.z_nd_1500).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压1750帕位移1", double.Parse(kfy.z_one_1750).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压1750帕位移2", double.Parse(kfy.z_two_1750).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压1750帕位移3", double.Parse(kfy.z_three_1750).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压1750帕第一组挠度", double.Parse(kfy.z_nd_1750).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压2000帕位移1", double.Parse(kfy.z_one_2000).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压2000帕位移2", double.Parse(kfy.z_two_2000).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压2000帕位移3", double.Parse(kfy.z_three_2000).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘正压2000帕第一组挠度", double.Parse(kfy.z_nd_2000).ToString("#0.00"));

                    dc.Add($"强度检测第{index}樘负压250帕位移1", double.Parse(kfy.f_one_250).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压250帕位移2", double.Parse(kfy.f_two_250).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压250帕位移3", double.Parse(kfy.f_three_250).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压250帕第一组挠度", double.Parse(kfy.f_nd_250).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压500帕位移1", double.Parse(kfy.f_one_500).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压500帕位移2", double.Parse(kfy.f_two_500).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压500帕位移3", double.Parse(kfy.f_three_500).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压500帕第一组挠度", double.Parse(kfy.f_nd_500).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压750帕位移1", double.Parse(kfy.f_one_750).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压750帕位移2", double.Parse(kfy.f_two_750).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压750帕位移3", double.Parse(kfy.f_three_750).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压750帕第一组挠度", double.Parse(kfy.f_nd_750).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压1000帕位移1", double.Parse(kfy.f_one_1000).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压1000帕位移2", double.Parse(kfy.f_two_1000).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压1000帕位移3", double.Parse(kfy.f_three_1000).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压1000帕第一组挠度", double.Parse(kfy.f_nd_1000).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压1250帕位移1", double.Parse(kfy.f_one_1250).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压1250帕位移2", double.Parse(kfy.f_two_1250).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压1250帕位移3", double.Parse(kfy.f_three_1250).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压1250帕第一组挠度", double.Parse(kfy.f_nd_1250).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压1500帕位移1", double.Parse(kfy.f_one_1500).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压1500帕位移2", double.Parse(kfy.f_two_1500).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压1500帕位移3", double.Parse(kfy.f_three_1500).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压1500帕第一组挠度", double.Parse(kfy.f_nd_1500).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压1750帕位移1", double.Parse(kfy.f_one_1750).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压1750帕位移2", double.Parse(kfy.f_two_1750).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压1750帕位移3", double.Parse(kfy.f_three_1750).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压1750帕第一组挠度", double.Parse(kfy.f_nd_1750).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压2000帕位移1", double.Parse(kfy.f_one_2000).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压2000帕位移2", double.Parse(kfy.f_two_2000).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压2000帕位移3", double.Parse(kfy.f_three_2000).ToString("#0.00"));
                    dc.Add($"强度检测第{index}樘负压2000帕第一组挠度", double.Parse(kfy.f_nd_2000).ToString("#0.00"));

                    dc.Add($"强度检测第{index}樘正压P1", kfy.p1);
                    dc.Add($"强度检测第{index}樘正压P2", kfy.p2);
                    dc.Add($"强度检测第{index}樘正压P3", kfy.p3);

                    dc.Add($"强度检测第{index}樘负压P1", kfy._p1);
                    dc.Add($"强度检测第{index}樘负压P2", kfy._p2);
                    dc.Add($"强度检测第{index}樘负压P3", kfy._p3);
                    #endregion
                }
            }
            else
            {
                dc.Add("检测条件第0樘抗风压等级", "--");
                //dc.Add("检测条件第0樘单扇单锁点", "--");
                dc.Add("检测条件第0樘单扇单锁点位移选择", "--");

                for (int i = 1; i < 4; i++)
                {
                    dc.Add("强度检测第" + i + "樘正压250帕位移1", "--");
                    dc.Add("强度检测第" + i + "樘正压250帕位移2", "--");
                    dc.Add("强度检测第" + i + "樘正压250帕位移3", "--");
                    dc.Add("强度检测第" + i + "樘正压250帕第一组挠度", "--");
                    dc.Add("强度检测第" + i + "樘正压500帕位移1", "--");
                    dc.Add("强度检测第" + i + "樘正压500帕位移2", "--");
                    dc.Add("强度检测第" + i + "樘正压500帕位移3", "--");
                    dc.Add("强度检测第" + i + "樘正压500帕第一组挠度", "--");
                    dc.Add("强度检测第" + i + "樘正压750帕位移1", "--");
                    dc.Add("强度检测第" + i + "樘正压750帕位移2", "--");
                    dc.Add("强度检测第" + i + "樘正压750帕位移3", "--");
                    dc.Add("强度检测第" + i + "樘正压750帕第一组挠度", "--");
                    dc.Add("强度检测第" + i + "樘正压1000帕位移1", "--");
                    dc.Add("强度检测第" + i + "樘正压1000帕位移2", "--");
                    dc.Add("强度检测第" + i + "樘正压1000帕位移3", "--");
                    dc.Add("强度检测第" + i + "樘正压1000帕第一组挠度", "--");
                    dc.Add("强度检测第" + i + "樘正压1250帕位移1", "--");
                    dc.Add("强度检测第" + i + "樘正压1250帕位移2", "--");
                    dc.Add("强度检测第" + i + "樘正压1250帕位移3", "--");
                    dc.Add("强度检测第" + i + "樘正压1250帕第一组挠度", "--");
                    dc.Add("强度检测第" + i + "樘正压1500帕位移1", "--");
                    dc.Add("强度检测第" + i + "樘正压1500帕位移2", "--");
                    dc.Add("强度检测第" + i + "樘正压1500帕位移3", "--");
                    dc.Add("强度检测第" + i + "樘正压1500帕第一组挠度", "--");
                    dc.Add("强度检测第" + i + "樘正压1750帕位移1", "--");
                    dc.Add("强度检测第" + i + "樘正压1750帕位移2", "--");
                    dc.Add("强度检测第" + i + "樘正压1750帕位移3", "--");
                    dc.Add("强度检测第" + i + "樘正压1750帕第一组挠度", "--");
                    dc.Add("强度检测第" + i + "樘正压2000帕位移1", "--");
                    dc.Add("强度检测第" + i + "樘正压2000帕位移2", "--");
                    dc.Add("强度检测第" + i + "樘正压2000帕位移3", "--");
                    dc.Add("强度检测第" + i + "樘正压2000帕第一组挠度", "--");

                    dc.Add("强度检测第" + i + "樘负压250帕位移1", "--");
                    dc.Add("强度检测第" + i + "樘负压250帕位移2", "--");
                    dc.Add("强度检测第" + i + "樘负压250帕位移3", "--");
                    dc.Add("强度检测第" + i + "樘负压250帕第一组挠度", "--");
                    dc.Add("强度检测第" + i + "樘负压500帕位移1", "--");
                    dc.Add("强度检测第" + i + "樘负压500帕位移2", "--");
                    dc.Add("强度检测第" + i + "樘负压500帕位移3", "--");
                    dc.Add("强度检测第" + i + "樘负压500帕第一组挠度", "--");
                    dc.Add("强度检测第" + i + "樘负压750帕位移1", "--");
                    dc.Add("强度检测第" + i + "樘负压750帕位移2", "--");
                    dc.Add("强度检测第" + i + "樘负压750帕位移3", "--");
                    dc.Add("强度检测第" + i + "樘负压750帕第一组挠度", "--");
                    dc.Add("强度检测第" + i + "樘负压1000帕位移1", "--");
                    dc.Add("强度检测第" + i + "樘负压1000帕位移2", "--");
                    dc.Add("强度检测第" + i + "樘负压1000帕位移3", "--");
                    dc.Add("强度检测第" + i + "樘负压1000帕第一组挠度", "--");
                    dc.Add("强度检测第" + i + "樘负压1250帕位移1", "--");
                    dc.Add("强度检测第" + i + "樘负压1250帕位移2", "--");
                    dc.Add("强度检测第" + i + "樘负压1250帕位移3", "--");
                    dc.Add("强度检测第" + i + "樘负压1250帕第一组挠度", "--");
                    dc.Add("强度检测第" + i + "樘负压1500帕位移1", "--");
                    dc.Add("强度检测第" + i + "樘负压1500帕位移2", "--");
                    dc.Add("强度检测第" + i + "樘负压1500帕位移3", "--");
                    dc.Add("强度检测第" + i + "樘负压1500帕第一组挠度", "--");
                    dc.Add("强度检测第" + i + "樘负压1750帕位移1", "--");
                    dc.Add("强度检测第" + i + "樘负压1750帕位移2", "--");
                    dc.Add("强度检测第" + i + "樘负压1750帕位移3", "--");
                    dc.Add("强度检测第" + i + "樘负压1750帕第一组挠度", "--");
                    dc.Add("强度检测第" + i + "樘负压2000帕位移1", "--");
                    dc.Add("强度检测第" + i + "樘负压2000帕位移2", "--");
                    dc.Add("强度检测第" + i + "樘负压2000帕位移3", "--");
                    dc.Add("强度检测第" + i + "樘负压2000帕第一组挠度", "--");

                    dc.Add("强度检测第" + i + "樘正压P1", "--");
                    dc.Add("强度检测第" + i + "樘正压P2", "--");
                    dc.Add("强度检测第" + i + "樘正压P3", "--");

                    dc.Add("强度检测第" + i + "樘负压P1", "--");
                    dc.Add("强度检测第" + i + "樘负压P2", "--");
                    dc.Add("强度检测第" + i + "樘负压P3", "--");
                }
                //dc.Add ( "强度检测第1樘试验情况记录", "--" );
            }
            #endregion

            return dc;
        }
        #endregion


        private void ImageLine(string file, string name, List<double> zitem, List<double> fitem)
        {
            int height = 350, width = 350;
            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //清空图片背景色
                g.Clear(Color.White);
                System.Drawing.Font font = new System.Drawing.Font("Arial", 9, FontStyle.Regular);
                System.Drawing.Font font1 = new System.Drawing.Font("宋体", 20, FontStyle.Regular);
                System.Drawing.Font font2 = new System.Drawing.Font("Arial", 8, FontStyle.Regular);
                LinearGradientBrush brush = new LinearGradientBrush(
                new System.Drawing.Rectangle(0, 0, image.Width, image.Height), Color.Black, Color.Black, 1.2f, true);
                g.FillRectangle(Brushes.AliceBlue, 0, 0, width, height);
                Brush brush1 = new SolidBrush(Color.Black);
                Brush brush2 = new SolidBrush(Color.SaddleBrown);

                g.DrawString(name, font1, brush1, new PointF(85, 30));
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Black), 0, 0, image.Width - 1, image.Height - 1);

                Pen mypen = new Pen(brush, 1);

                //绘制线条
                //绘制纵向线条
                //int x = 15;
                //for (int i = 0; i < 21; i++)
                //{
                //    g.DrawLine(mypen, x, 80, x, 335);
                //    x = x + 15;
                //}

                int x = 15;
                for (int i = 0; i < 21; i++)
                {
                    g.DrawLine(mypen, x, 196, x, 204);
                    x = x + 15;
                }

                Pen mypen1 = new Pen(Color.Black, 3);
                x = 165;
                g.DrawLine(mypen1, x, 80, x, 335);

                //绘制横向线条
                //int y = 15;
                //for (int i = 0; i < 18; i++)
                //{
                //    if (i == 0)
                //    {
                //        g.DrawLine(mypen, 15, 80, 330, 80);
                //    }
                //    else
                //    {
                //        g.DrawLine(mypen, 15, 80 + y, 330, 80 + y);
                //        y = y + 15;
                //    }
                //}

                int y = 15;
                for (int i = 0; i < 18; i++)
                {
                    if (i == 0)
                    {
                        g.DrawLine(mypen, 167, 80, 173, 80);

                    }
                    else
                    {
                        g.DrawLine(mypen, 167, 80 + y, 173, 80 + y);
                        y = y + 15;
                    }
                }
                y = 200;
                g.DrawLine(mypen1, 15, y, 330, y);

                // x轴
                String[] n = { "-10", "-9", "-8", "-7", "-6", "-5", "-4", "-3", "-2", "-1", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
                x = 11;
                for (int i = 0; i < 21; i++)
                {
                    if (i % 2 == 0)
                    {
                        g.DrawString(n[i].ToString(), font, Brushes.Black, x, 205); //设置文字内容及输出位置
                    }
                    x = x + 15;
                }

                //y轴
                String[] m = { "2000", "1750", "1500", "1250", "1000", "750", "500", "250", "0", "-250", "-500", "-750", "-1000", "-1250", "-1500", "-1750", "-2000" };
                y = 74;
                for (int i = 0; i < 17; i++)
                {
                    if (m[i] == "0")
                    { y = y + 15; continue; }

                    if (Convert.ToInt32(m[i]) > -1)
                    {
                        g.DrawString(m[i].ToString(), font, Brushes.Black, 130, y); //设置文字内容及输出位置
                    }
                    else
                    {
                        g.DrawString(m[i].ToString(), font, Brushes.Black, 173, y); //设置文字内容及输出位置
                    }
                    y = y + 15;
                }

                //double[] z_item1 = new double[9] { 0, 0.55, 1.19, 1.85, 2.40, 3.15, 3.65, 4.01, 4.80 };
                //double[] z_item2 = new double[9] { 0, 0.5, 1.0, 1.48, 2.01, 2.50, 2.99, 3.49, 4.88 };

                //显示折线效果
                System.Drawing.Font font3 = new System.Drawing.Font("Arial", 10, FontStyle.Bold);
                SolidBrush mybrush = new SolidBrush(Color.Red);
                //正压
                System.Drawing.Point[] points1 = new System.Drawing.Point[9];
                Pen mypen2 = new Pen(Color.Black, 1);
                double initialx = 165;
                double initialy = 200;

                for (int i = 0; i < zitem.Count; i++)
                {
                    points1[i].X = Convert.ToInt32(initialx + zitem[i] * 10 * 1.5);
                    points1[i].Y = (int)initialy - i * 15;
                    g.DrawRectangle(mypen2, points1[i].X - 1, points1[i].Y - 1, 2, 2);
                }
                g.DrawLines(mypen2, points1); //绘制折线

                //绘制数字
                for (int i = 1; i < zitem.Count; i++)
                {
                    g.DrawString(fitem[i].ToString(), font3, Brushes.Red, 15, points1[i].Y - 20);
                }

                //负压
                System.Drawing.Point[] points2 = new System.Drawing.Point[9];
                Pen mypen3 = new Pen(Color.Black, 1);

                for (int i = 0; i < 9; i++)
                {
                    points2[i].X = Convert.ToInt32(initialx - fitem[i] * 10 * 1.5);
                    points2[i].Y = (int)initialy + i * 15;
                    g.DrawRectangle(mypen3, points2[i].X - 1, points2[i].Y - 1, 2, 2);
                }
                g.DrawLines(mypen3, points2); //绘制折线

                //绘制数字
                for (int i = 1; i < fitem.Count; i++)
                {
                    g.DrawString("-" + fitem[i].ToString(), font3, Brushes.Red, 15, 205 + 15 * i);
                }

                // System.Windows.Forms.Application.StartupPath + ("\\tempImage\\asd.jpg");
                image.Save(file, System.Drawing.Imaging.ImageFormat.Jpeg);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
    }
}
