using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysDetection.model
{
    /// <summary>
    /// 设置详情
    /// </summary>
    public class DetectionSet_Info
    {
        
        public string dt_ID { get; set; }//主键
        public string dt_Code { get; set; }//编号
        public string WeiTuoBianHao { get; set; } //委托编号
        public string WeiTuoDanWei { get; set; }  // 委托单位
        public string WeiTuoRen { get; set; }  // 委托人
        public string YangPinMingCheng { get; set; } // 样品名称
        public string CaiYangFangShi { get; set; } //采样方式 
        public string JianYanXiangMu { get; set; }// 检验项目
        public string GuiGeXingHao { get; set; } //规格型号
        public string GuiGeShuLiang { get; set; } //规格数量
        public string JianYanRiQi { get; set; }//检验日期
        public string KaiQiFangShi { get; set; } // 开启方式
        public string DaQiYaLi { get; set; } // 大气压力
        public string BoLiPinZhong { get; set; }// 玻璃品种
        public string DangQianWenDu { get; set; }// 当前温度
        public string BoLiHouDu { get; set; }// 玻璃厚度
        public string ZongMianJi { get; set; } // 总面积
        public string ZuiDaBoLi { get; set; }// 最大玻璃
        public string KaiQiFengChang { get; set; } // 开启缝长
        public string BoLiMiFeng { get; set; }// 玻璃密封
        public string XiangQianFangShi { get; set; } // 镶嵌方式
        public string ShuiMiDengJiSheJiZhi { get; set; } // 水密等级设计值
        public string KuangShanMiFang { get; set; } // 框扇密封
        public string QiMiZhengYaDanWeiFengChangSheJiZhi { get; set; } // 气密正压单位缝长设计值
        public string ZhengYaQiMiDengJiSheJiZhi { get; set; } // 正压气密等级设计值
        public string QiMiFuYaDanWeiFengChangSheJiZhi { get; set; } // 气密负压单位缝长设计值
        public string FuYaQiMiDengJiSheJiZhi { get; set; }// 负压气密等级设计值
        public string ShuiMiSheJiZhi { get; set; } //水密设计值
        public string QiMiZhengYaDanWeiMianJiSheJiZhi { get; set; } // 气密正压单位面积设计值
        public string QiMiFuYaDanWeiMianJiSheJiZhi { get; set; }// 气密负压单位面积设计值
        public string JianYanYiJu { get; set; } //检验依据
        public string GongChengMingCheng { get; set; } // 工程名称
        public string GongChengDiDian { get; set; } //工程地点
        public string ShengChanDanWei { get; set; }// 生产单位
        public string JianLiDanWei { get; set; }// 监理单位
        public string JianZhengRen { get; set; }//见证人
        public string JianZhengHao { get; set; }// 见证号
        public string ShiGongDanWei { get; set; }// 施工单位
        public string WuJinJianZhuangKuang { get; set; }// 五金件状况
        public string SuLiaoChuangChenJinChiCun { get; set; } // 塑料窗衬金尺寸
        public string ShiFouJiaLuoSi { get; set; } // 是否加洛斯
        public string XingCaiGuiGe { get; set; } // 型材规格
        public string XingCaiBiHou { get; set; }// 型材壁厚
        public string XingCaiShengChanChang { get; set; } // 型材生产厂
    }
}
