using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace text.doors.Default
{
    public class PublicEnum
    {
        /// <summary>
        /// 压力级别枚举
        /// </summary>
        public enum Kpa_Level
        {
            liter100,//升100 
            liter150,//升150
            drop100//降100
        }


        /// <summary>
        /// 压力级别枚举
        /// </summary>
        public enum FY_Kpa_Static_Level
        {
            S250,
            S500,
            S750,
            S1000,
            S1250,
            S1500,
            S1750,
            S2000,
            J250,
            J500,
            J750,
            J1000,
            J1250,
            J1500,
            J1750,
            J2000,
        }


        /// <summary>
        /// 系统项
        /// </summary>
        public enum SystemItem
        {
            /// <summary>
            /// 水密
            /// </summary>
            Watertight,
            /// <summary>
            /// 气密
            /// </summary>
            Airtight,
            /// <summary>
            /// 风压
            /// </summary>
            AirPressure
        }

        /// <summary>
        /// 气密性能检测
        /// </summary>
        public enum AirtightPropertyTest
        {
            ZReady,//正压预备
            ZStart,//正压开始
            FReady,//负压预备
            FStart,//负压开始
            Stop//停止
        }


        /// <summary>
        /// 水密性能检测
        /// </summary>
        public enum WaterTightPropertyTest
        {
            Ready,//预备
            Start,//开始
            Next,//下一级
            CycleLoading,//依次加压
            Stop, //停止
            SrartBD, //开始波动
            StopBD //结束波动
        }


        /// <summary>
        /// 风压性能检测
        /// </summary>
        public enum WindPressureTest
        {
            /// <summary>
            /// 正压预备
            /// </summary>
            ZReady,
            /// <summary>
            /// 正压开始
            /// </summary>
            ZStart,
            /// <summary>
            /// 负压预备
            /// </summary>
            FReady,
            /// <summary>
            /// 负压开始
            /// </summary>
            FStart,
            /// <summary>
            /// 正反复
            /// </summary>
            ZRepeatedly,
            /// <summary>   
            /// 负反复
            /// </summary>
            FRepeatedly,
            /// <summary>
            /// 正安全
            /// </summary>
            ZSafety,
            /// <summary>
            /// 负安全
            /// </summary>
            FSafety,
            /// <summary>
            /// 结束
            /// </summary>
            Stop
        }

        /// <summary>
        /// 标定
        /// </summary>
        public enum DemarcateType
        {
            风速传感器,
            差压传感器,
            温度传感器,
            大气压力传感器,
            位移传感器1,
            位移传感器2,
            位移传感器3
        }

        /// <summary>
        /// 检测项
        /// </summary>
        public enum DetectionItem
        {
            气密水密抗风压性能检测,
            气密性能检测,
            水密性能检测,
            抗风压性能检测,
            气密性能及水密性能检测,
            气密性能及抗风压性能检测,
            水密性能及抗风压性能检测
        }

    }
}