using text.doors.Common;
using text.doors.dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using text.doors.Default;

namespace text.doors.Model
{
    public class Pressure
    {
        public Pressure()
        {
            ZYFJ_100 = new List<double>();
            ZYFJ150 = new List<double>();
            ZYFJ100 = new List<double>();
            ZYZD_100 = new List<double>();
            ZYZD150 = new List<double>();
            ZYZD100 = new List<double>();

            FYFJ_100 = new List<double>();
            FYFJ150 = new List<double>();
            FYFJ100 = new List<double>();
            FYZD_100 = new List<double>();
            FYZD150 = new List<double>();
            FYZD100 = new List<double>();

            PressurePa = 0;
            Pressure_Z = 0.00;
            Pressure_Z_Z = 0.00;
            Pressure_F = 0.00;
            Pressure_F_Z = 0.00;
        }

        public List<Pressure> ClearZ_F()
        {
            ZYFJ_100 = new List<double>();
            ZYFJ150 = new List<double>();
            ZYFJ100 = new List<double>();

            Pressure_Z = 0;
            return GetPressure();
        }


        public List<Pressure> ClearZ_Z()
        {
            ZYZD_100 = new List<double>();
            ZYZD150 = new List<double>();
            ZYZD100 = new List<double>();
            Pressure_Z_Z = 0.00;
            return GetPressure();
        }

        public List<Pressure> ClearF_Z()
        {
            FYZD_100 = new List<double>();
            FYZD150 = new List<double>();
            FYZD100 = new List<double>();
            Pressure_F_Z = 0.00;
            return GetPressure();
        }

        public List<Pressure> ClearF_F()
        {
            FYFJ_100 = new List<double>();
            FYFJ150 = new List<double>();
            FYFJ100 = new List<double>();

            Pressure_F = 0.00;
            return GetPressure();
        }

        //压力pa
        public int PressurePa { get; set; }
        //正压附加
        public double Pressure_Z { get; set; }
        //正压总的
        public double Pressure_Z_Z { get; set; }
        //负压附加
        public double Pressure_F { get; set; }
        //负压总的
        public double Pressure_F_Z { get; set; }

        /// <summary>
        /// 正压附加
        /// </summary>
        private List<double> ZYFJ_100 = new List<double>();

        private List<double> ZYFJ150 = new List<double>();

        private List<double> ZYFJ100 = new List<double>();

        /// <summary>
        /// 正压总的
        /// </summary>
        private List<double> ZYZD_100 = new List<double>();

        private List<double> ZYZD150 = new List<double>();

        private List<double> ZYZD100 = new List<double>();

        /// <summary>
        /// 负压附加
        /// </summary>
        private List<double> FYFJ_100 = new List<double>();

        private List<double> FYFJ150 = new List<double>();

        private List<double> FYFJ100 = new List<double>();

        /// <summary>
        /// 负压总的
        /// </summary>
        private List<double> FYZD_100 = new List<double>();

        private List<double> FYZD150 = new List<double>();

        private List<double> FYZD100 = new List<double>();


        /// <summary>
        /// 获取风速数据
        /// </summary>
        /// <returns></returns>
        public List<Pressure> GetPressure()
        {
            List<Pressure> list = new List<Pressure>();
            AddYL_100(list);
            AddYL150(list);
            AddYL100(list);
            return list;
        }




        /// <summary>
        /// 增加风速数据(正压附加)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fs">风速枚举</param>
        public void AddZYFJ(double data, PublicEnum.Kpa_Level fs)
        {
            if (fs == PublicEnum.Kpa_Level.liter100)
                ZYFJ_100.Add(data);
            if (fs == PublicEnum.Kpa_Level.liter150)
                ZYFJ150.Add(data);
            if (fs == PublicEnum.Kpa_Level.drop100)
                ZYFJ100.Add(data);
        }

        /// <summary>
        /// 增加风速数据(正压总的)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fs">风速枚举</param>
        public void AddZYZD(double data, PublicEnum.Kpa_Level fs)
        {
            if (fs == PublicEnum.Kpa_Level.liter100)
                ZYZD_100.Add(data);
            if (fs == PublicEnum.Kpa_Level.liter150)
                ZYZD150.Add(data);
            if (fs == PublicEnum.Kpa_Level.drop100)
                ZYZD100.Add(data);
        }

        /// <summary>
        /// 增加风速数据(负压附加)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fs">风速枚举</param>
        public void AddFYFJ(double data, PublicEnum.Kpa_Level fs)
        {
            if (fs == PublicEnum.Kpa_Level.liter100)
                FYFJ_100.Add(data);
            if (fs == PublicEnum.Kpa_Level.liter150)
                FYFJ150.Add(data);
            if (fs == PublicEnum.Kpa_Level.drop100)
                FYFJ100.Add(data);
        }

        /// <summary>
        /// 增加风速数据(负压总的)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fs">风速枚举</param>
        public void AddFYZD(double data, PublicEnum.Kpa_Level fs)
        {
            if (fs == PublicEnum.Kpa_Level.liter100)
                FYZD_100.Add(data);
            if (fs == PublicEnum.Kpa_Level.liter150)
                FYZD150.Add(data);
            if (fs == PublicEnum.Kpa_Level.drop100)
                FYZD100.Add(data);
        }

        /// <summary>
        /// 增加一级100压力
        /// </summary>
        /// <param name="list"></param>
        private void AddYL_100(List<Pressure> list)
        {
            Pressure model = new Pressure();
            model.PressurePa = 100;
            model.Pressure_Z = ZYFJ_100.Count() == 0 ? 0 : Math.Round(ZYFJ_100.Sum(t => t) / ZYFJ_100.Count(), 2);
            model.Pressure_Z_Z = ZYZD_100.Count() == 0 ? 0 : Math.Round(ZYZD_100.Sum(t => t) / ZYZD_100.Count(), 2);
            model.Pressure_F = FYFJ_100.Count() == 0 ? 0 : Math.Round(FYFJ_100.Sum(t => t) / FYFJ_100.Count(), 2);
            model.Pressure_F_Z = FYZD_100.Count() == 0 ? 0 : Math.Round(FYZD_100.Sum(t => t) / FYZD_100.Count(), 2);
            list.Add(model);
        }

        /// <summary>
        /// 获取150压力
        /// </summary>
        /// <param name="list"></param>
        private void AddYL150(List<Pressure> list)
        {
            Pressure model = new Pressure();
            model.PressurePa = 150;
            model.Pressure_Z = ZYFJ150.Count() == 0 ? 0 : Math.Round(ZYFJ150.Sum(t => t) / ZYFJ150.Count(), 2);
            model.Pressure_Z_Z = ZYZD150.Count() == 0 ? 0 : Math.Round(ZYZD150.Sum(t => t) / ZYZD150.Count(), 2);
            model.Pressure_F = FYFJ150.Count() == 0 ? 0 : Math.Round(FYFJ150.Sum(t => t) / FYFJ150.Count(), 2);
            model.Pressure_F_Z = FYZD150.Count() == 0 ? 0 : Math.Round(FYZD150.Sum(t => t) / FYZD150.Count(), 2);
            list.Add(model);
        }

        /// <summary>
        /// 增加二级100压力
        /// </summary>
        /// <param name="list"></param>
        private void AddYL100(List<Pressure> list)
        {
            Pressure model = new Pressure();
            model.PressurePa = 100;
            model.Pressure_Z = ZYFJ100.Count() == 0 ? 0 : Math.Round(ZYFJ100.Sum(t => t) / ZYFJ100.Count(), 2);
            model.Pressure_Z_Z = ZYZD100.Count() == 0 ? 0 : Math.Round(ZYZD100.Sum(t => t) / ZYZD100.Count(), 2);
            model.Pressure_F = FYFJ100.Count() == 0 ? 0 : Math.Round(FYFJ100.Sum(t => t) / FYFJ100.Count(), 2);
            model.Pressure_F_Z = FYZD100.Count() == 0 ? 0 : Math.Round(FYZD100.Sum(t => t) / FYZD100.Count(), 2);
            list.Add(model);
        }
    }
}
