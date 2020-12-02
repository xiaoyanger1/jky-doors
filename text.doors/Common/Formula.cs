using System;
using System.Collections;
using System.Collections.Generic;
using text.doors.Default;
using text.doors.Model;
using text.doors.Model.DataBase;
using text.doors.Service;

using System.Linq;
namespace text.doors.Common
{
    public class Formula
    {
        #region   y=kx+b
        /// <summary>
        /// 获取标定后值
        /// </summary>
        /// <returns></returns>
        public static double GetValues(PublicEnum.DemarcateType enum_Demarcate, float x)
        {
            List<Calibrating_Dict> dict = GetListByEnum(enum_Demarcate);

            if (dict == null || dict.Count == 0)
            {
                return Math.Round(x, 2);
            }

            if (dict.Find(t => t.y == x) != null)
            {
                return dict.Find(t => t.y == x).x;
            }

            float k = 0, b = 0;

            Compute_KB(dict, x, ref k, ref b);

            if (k == 0 && b == 0)
                return Math.Round(x, 2);
            return Math.Round(k * x + b, 2);
        }


        /// <summary>
        /// 计算斜率k及纵截距b值
        /// </summary>
        /// <param name="x1">坐标点x1</param>
        /// <param name="x2">坐标点x2</param>
        /// <param name="y1">坐标点y1</param>
        /// <param name="y2">坐标点y2</param>
        /// <param name="kvalue">斜率k值</param>
        /// <param name="bvalue">纵截距b值</param>
        private static void Calculate(float x1, float x2, float y1, float y2, ref float kvalue, ref float bvalue)//求方程y=kx+b 系数 k ,b
        {
            float coefficient = 1;//系数值
            try
            {
                if ((x1 == 0) || (x2 == 0) || (x1 == x2)) return; //排除为零的情况以及x1，x2相等时无法运算的情况
                //if (y1 == y2) return; //根据具体情况而定，如何这两个值相等，得到的就是一条直线
                float temp = 0;
                if (x1 >= x2)
                {
                    coefficient = (x1 / x2);
                    temp = y2 * coefficient; //将对应的函数乘以系数
                    bvalue = (temp - y1) / (coefficient - 1);
                    kvalue = (y1 - bvalue) / x1; //求出k值
                }
                else
                {
                    coefficient = x2 / x1;
                    temp = y1 * coefficient;
                    bvalue = (temp - y2) / (coefficient - 1); //求出b值
                    kvalue = (y2 - bvalue) / x2; //求出k值
                }
            }
            catch
            {
                bvalue = 0;
                kvalue = 0;
            }
        }



        /// <summary>
        /// 根据枚举获取字典数据
        /// </summary>
        /// <param name="enum_Demarcate"></param>
        /// <returns></returns>
        private static List<Calibrating_Dict> GetListByEnum(PublicEnum.DemarcateType enum_Demarcate)
        {
            if (enum_Demarcate == PublicEnum.DemarcateType.enum_差压传感器)
            {
                return DAL_Demarcate_Dict.differentialPressureDict;
            }
            if (enum_Demarcate == PublicEnum.DemarcateType.enum_大气压力传感器)
            {
                return DAL_Demarcate_Dict._kPaDict;
            }
            if (enum_Demarcate == PublicEnum.DemarcateType.enum_风速传感器)
            {
                return DAL_Demarcate_Dict._windSpeedDict;
            }
            if (enum_Demarcate == PublicEnum.DemarcateType.enum_温度传感器)
            {
                return DAL_Demarcate_Dict.temperatureDict;
            }
            return new List<Calibrating_Dict>();
        }

        /// <summary>
        /// 获取KB
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="x"></param>
        /// <param name="k"></param>
        /// <param name="b"></param>
        private static void Compute_KB(List<Calibrating_Dict> dictList, float x, ref float k, ref float b)
        {
            // 对数据合计
            for (int i = 0; i < dictList.Count; i++)
            {
                if (dictList.Count < i + 1)
                {
                    break;
                }

                if (dictList[i].x > x && i == 0)
                {
                    break;
                }

                if (dictList[i].y > x && dictList[i - 1].y < x)
                {
                    Calculate(dictList[i - 1].y, dictList[i].y, dictList[i - 1].x, dictList[i].x, ref k, ref b);
                }
            }
        }

        #endregion

        #region  计算流量
        /// <summary>
        /// 计算流量
        /// 公式为 Q = 3.1415*D的平方（配置）/4*v(风速平均值)*3600
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public double MathFlow(double value)
        {
            if (value == 0)
            {
                return 0;
            }
            double _D = DefaultBase._D;

            return Math.Round(3.1415 * _D * _D / 4 * value * 3600, 2);
        }
        #endregion

        #region 获取分级指标缝长和面积
        /// <summary>
        /// 获取分级指标缝长和面积
        /// </summary>
        /// <param name="zd">升压总的</param>
        /// <param name="fj">升压附加</param>
        /// <param name="_zd">降压总的</param>
        /// <param name="_fj">降压附加</param>
        /// 
        ///  <param name="kPa">大气压力</param>
        ///   <param name="tempTemperature">当前温度</param>
        ///    <param name="stitchLength">开启逢长</param>
        ///     <param name="sumArea">总面积</param>
        public static double GetIndexStitchLengthAndArea(double zd, double fj, double _zd, double _fj, bool isFC, double kPa, double tempTemperature, double stitchLength, double sumArea)
        {
            double res = 0;
            //流量数值（正压100升总的 +正压100降总的）/2 -（正压100升附加 +正压100降附加）/2 
            var Q = (zd + _zd) / 2 - (fj + _fj) / 2;

            var qMin = 293 / 101.3 * (kPa / (273 + tempTemperature)) * Q;

            if (isFC)
            {
                res = qMin / stitchLength / 4.65;
            }
            else
            {
                res = qMin / sumArea / 4.65;
            }
            return res;
        }
        #endregion

        #region 等级划分



        /// <summary>
        /// 获取水密等级
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int GetWaterTightLevel(List<Model_dt_sm_Info> waterTight)
        {
            int value = 0;
            if (waterTight == null || waterTight.Count == 0)
                return value;

            if (waterTight.Count == 3)
            {
                List<int> list = new List<int>();
                waterTight.ForEach(t => list.Add(Convert.ToInt32(t.sm_Pa)));
                list.Sort();

                int min = list[0];
                int intermediate = list[1];
                int max = list[2];
                //int minlevel = new QM_Dict.AirtightLevel().GetList().Find(t => t.value == min).level,
                //    intermediatelevel = new QM_Dict.AirtightLevel().GetList().Find(t => t.value == intermediate).level,
                //    maxlevel = new QM_Dict.AirtightLevel().GetList().Find(t => t.value == max).level;
                //todo  update
                int minlevel = DefaultBase.AirtightLevel.ContainsKey(min) ? DefaultBase.AirtightLevel[min] : 0;
                int intermediatelevel = DefaultBase.AirtightLevel.ContainsKey(intermediate) ? DefaultBase.AirtightLevel[intermediate] : 0;
                int maxlevel = DefaultBase.AirtightLevel.ContainsKey(max) ? DefaultBase.AirtightLevel[max] : 0;

                if ((maxlevel - intermediatelevel) > 2)
                {
                    //todo update
                    foreach (var item in DefaultBase.AirtightLevel)
                    {
                        if (item.Value == (intermediatelevel + 2))
                        {
                            max = item.Key; break;
                        }
                    }
                }

                value = (min + intermediate + max) / 3;
            }
            else
            {
                foreach (var item in waterTight)
                    value += int.Parse(item.sm_Pa);

                value = value / waterTight.Count;
            }
            return Formula.GetWaterTightLevel(value);
        }



        /// <summary>
        /// 获取水密压力
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int GetWaterTightPressure(List<Model_dt_sm_Info> list)
        {
            int value = 0;

            if (list == null || list.Count == 0)
                return value;

            if (list.Count == 3)
            {
                List<int> pas = new List<int>();
                list.ForEach(t => pas.Add(int.Parse(t.sm_Pa)));
                list.Sort();

                int min = pas[0];
                int intermediate = pas[1];
                int max = pas[2];

                //int minlevel = new AirtightLevel.AirtightLevel().GetList().Find(t => t.value == min).level,
                //    intermediatelevel = new AirtightLevel.AirtightLevel().GetList().Find(t => t.value == intermediate).level,
                //    maxlevel = new AirtightLevel.AirtightLevel().GetList().Find(t => t.value == max).level;

                //if ((maxlevel - intermediatelevel) > 2)
                //{
                //    max = new AirtightLevel.AirtightLevel().GetList().Find(t => t.level == (intermediatelevel + 2)).value;
                //}
                //todo update
                int minlevel = DefaultBase.AirtightLevel.ContainsKey(min) ? DefaultBase.AirtightLevel[min] : 0;
                int intermediatelevel = DefaultBase.AirtightLevel.ContainsKey(intermediate) ? DefaultBase.AirtightLevel[intermediate] : 0;
                int maxlevel = DefaultBase.AirtightLevel.ContainsKey(max) ? DefaultBase.AirtightLevel[max] : 0;

                if ((maxlevel - intermediatelevel) > 2)
                {
                    foreach (var item in DefaultBase.AirtightLevel)
                    {
                        if (item.Value == (intermediatelevel + 2))
                        {
                            max = item.Key; break;
                        }
                    }
                }
                value = (min + intermediate + max) / 3;
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(list[i].sm_Pa))
                    {
                        value = 0;
                        break;
                    }
                    value += int.Parse(list[i].sm_Pa.ToString());
                }
                value = value / list.Count;
            }


            return value;
        }

        /// <summary>
        /// 气密计算  获取不标准的等级
        /// 范式 气密正负缝长平均值等级 与 气密正负压缝长平均值等级 最大的最次
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int GetAirTightLevel(List<Model_dt_qm_Info> airTight)
        {
            if (airTight == null || airTight.Count == 0)
                return 0;

            double zFc = Math.Round(airTight.Sum(t => double.Parse(t.qm_Z_FC)) / airTight.Count, 2);
            double fFc = Math.Round(airTight.Sum(t => double.Parse(t.qm_F_FC)) / airTight.Count, 2);
            double zMj = Math.Round(airTight.Sum(t => double.Parse(t.qm_Z_MJ)) / airTight.Count, 2);
            double fMj = Math.Round(airTight.Sum(t => double.Parse(t.qm_F_MJ)) / airTight.Count, 2);

            List<int> level = new List<int>();
            level.Add(Formula.GetStitchLengthLevel(zFc));
            level.Add(Formula.GetStitchLengthLevel(fFc));
            level.Add(Formula.GetStitchLengthLevel(zMj));
            level.Add(Formula.GetStitchLengthLevel(fMj));
            level.Sort();

            return level[0];
        }

        /// <summary>
        /// 获取缝长分级
        /// </summary>
        /// <returns></returns>
        private static int GetStitchLengthLevel(double value)
        {
            int res = 0;
            if (4 >= value && value > 3.5)
            {
                res = 1;
            }
            else if (3.5 >= value && value > 3.0)
            {
                res = 2;
            }
            else if (3.0 >= value && value > 2.5)
            {
                res = 3;
            }
            else if (2.5 >= value && value > 2.0)
            {
                res = 4;
            }
            else if (2.0 >= value && value > 1.5)
            {
                res = 5;
            }
            else if (1.5 >= value && value > 1.0)
            {
                res = 6;
            }
            else if (1.0 >= value && value > 0.5)
            {
                res = 7;
            }
            else if (value <= 0.5)
            {
                res = 8;
            }
            return res;
        }

        /// <summary>
        /// 获取面积分级
        /// </summary>
        /// <returns></returns>
        private static int GetAreaLevel(double value)
        {
            int res = 0;
            if (12 >= value && value > 10.5)
            {
                res = 1;
            }
            else if (10.5 >= value && value > 9.0)
            {
                res = 2;
            }
            else if (9.0 >= value && value > 7.5)
            {
                res = 3;
            }
            else if (7.5 >= value && value > 6.0)
            {
                res = 4;
            }
            else if (6.0 >= value && value > 4.5)
            {
                res = 5;
            }
            else if (4.5 >= value && value > 3.0)
            {
                res = 6;
            }
            else if (3.0 >= value && value > 1.5)
            {
                res = 7;
            }
            else if (value <= 1.5)
            {
                res = 8;
            }
            return res;
        }

        /// <summary>
        /// 获取水密分级
        /// </summary>
        /// <returns></returns>
        private static int GetWaterTightLevel(int value)
        {
            int res = 0;
            if (value >= 100 && value < 150)
            {
                res = 1;
            }
            else if (value >= 150 && value < 250)
            {
                res = 2;
            }
            else if (value >= 250 && value < 350)
            {
                res = 3;
            }
            else if (value >= 300 && value < 500)
            {
                res = 4;
            }
            else if (value >= 500 && value < 700)
            {
                res = 5;
            }
            else if (value >= 700)
            {
                res = 6;
            }
            return res;
        }

        #endregion
    }
}
