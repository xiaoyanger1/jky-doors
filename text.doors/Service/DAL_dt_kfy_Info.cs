using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using text.doors.dal;
using text.doors.Default;
using text.doors.Model.DataBase;
using Young.Core.SQLite;

namespace text.doors.Service
{
    public class DAL_dt_kfy_Info
    {
        /// <summary>
        /// 添加抗风压信息
        /// </summary>
        /// <param name="mode"></param>
        public bool Add_kfy_Info(Model_dt_kfy_Info model)
        {
            SQLiteHelper.ExecuteNonQuery("delete from dt_kfy_Info where dt_Code = '" + model.dt_Code + "' and info_DangH = '" + model.info_DangH + "'");

            #region 拼接

            var sql = string.Format(
  @"insert into dt_kfy_Info (
dt_Code ,
info_DangH ,
z_one_250 ,
z_one_500 ,
z_one_750 ,
z_one_1000 ,
z_one_1250 ,
z_one_1500 ,
z_one_1750 ,
z_one_2000 ,
z_two_250 ,
z_two_500 ,
z_two_750 ,
z_two_1000 ,
z_two_1250 ,
z_two_1500 ,
z_two_1750 ,
z_two_2000 ,
z_three_250 ,
z_three_500 ,
z_three_750 ,
z_three_1000 ,
z_three_1250 ,
z_three_1500 ,
z_three_1750 ,
z_three_2000 ,
z_nd_250 ,
z_nd_500 ,
z_nd_750 ,
z_nd_1000 ,
z_nd_1250 ,
z_nd_1500 ,
z_nd_1750 ,
z_nd_2000 ,
z_ix_250 ,
z_ix_500 ,
z_ix_750 ,
z_ix_1000 ,
z_ix_1250 ,
z_ix_1500 ,
z_ix_1750 ,
z_ix_2000 ,
f_one_250 ,
f_one_500 ,
f_one_750 ,
f_one_1000 ,
f_one_1250 ,
f_one_1500 ,
f_one_1750 ,
f_one_2000 ,
f_two_250 ,
f_two_500 ,
f_two_750 ,
f_two_1000 ,
f_two_1250 ,
f_two_1500 ,
f_two_1750 ,
f_two_2000 ,
f_three_250 ,
f_three_500 ,
f_three_750 ,
f_three_1000 ,
f_three_1250 ,
f_three_1500 ,
f_three_1750 ,
f_three_2000 ,
f_nd_250 ,
f_nd_500 ,
f_nd_750 ,
f_nd_1000 ,
f_nd_1250 ,
f_nd_1500 ,
f_nd_1750 ,
f_nd_2000 ,
f_ix_250 ,
f_ix_500 ,
f_ix_750 ,
f_ix_1000 ,
f_ix_1250 ,
f_ix_1500 ,
f_ix_1750 ,
f_ix_2000,
desc,
p1,
p2,
p3,
_p1,
_p2,
_p3,
CheckLock
) 
values(
'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}',
'{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}',
'{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}','{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}','{60}',
'{61}','{62}','{63}','{64}','{65}','{66}','{67}','{68}','{69}','{70}','{71}','{72}','{73}','{74}','{75}','{76}','{77}','{78}','{79}','{80}',
'{81}','{82}','{83}','{84}','{85}','{86}','{87}','{88}',{89})",
  model.dt_Code,
  model.info_DangH,
  model.z_one_250,
  model.z_one_500,
  model.z_one_750,
  model.z_one_1000,
  model.z_one_1250,
  model.z_one_1500,
  model.z_one_1750,
  model.z_one_2000,
  model.z_two_250,
  model.z_two_500,
  model.z_two_750,
  model.z_two_1000,
  model.z_two_1250,
  model.z_two_1500,
  model.z_two_1750,
  model.z_two_2000,
  model.z_three_250,
  model.z_three_500,
  model.z_three_750,
  model.z_three_1000,
  model.z_three_1250,
  model.z_three_1500,
  model.z_three_1750,
  model.z_three_2000,
  model.z_nd_250,
  model.z_nd_500,
  model.z_nd_750,
  model.z_nd_1000,
  model.z_nd_1250,
  model.z_nd_1500,
  model.z_nd_1750,
  model.z_nd_2000,
  model.z_ix_250,
  model.z_ix_500,
  model.z_ix_750,
  model.z_ix_1000,
  model.z_ix_1250,
  model.z_ix_1500,
  model.z_ix_1750,
  model.z_ix_2000,
  model.f_one_250,
  model.f_one_500,
  model.f_one_750,
  model.f_one_1000,
  model.f_one_1250,
  model.f_one_1500,
  model.f_one_1750,
  model.f_one_2000,
  model.f_two_250,
  model.f_two_500,
  model.f_two_750,
  model.f_two_1000,
  model.f_two_1250,
  model.f_two_1500,
  model.f_two_1750,
  model.f_two_2000,
  model.f_three_250,
  model.f_three_500,
  model.f_three_750,
  model.f_three_1000,
  model.f_three_1250,
  model.f_three_1500,
  model.f_three_1750,
  model.f_three_2000,
  model.f_nd_250,
  model.f_nd_500,
  model.f_nd_750,
  model.f_nd_1000,
  model.f_nd_1250,
  model.f_nd_1500,
  model.f_nd_1750,
  model.f_nd_2000,
  model.f_ix_250,
  model.f_ix_500,
  model.f_ix_750,
  model.f_ix_1000,
  model.f_ix_1250,
  model.f_ix_1500,
  model.f_ix_1750,
  model.f_ix_2000,
  model.desc,
  model.p1,
  model.p2,
  model.p3,
  model._p1,
  model._p2,
  model._p3,
  model.CheckLock
  );
            #endregion
            var res = SQLiteHelper.ExecuteNonQuery(sql) > 0 ? true : false;
            if (res)
            {
                return new DAL_dt_Info().UpdateTestType(model.dt_Code, model.info_DangH, PublicEnum.SystemItem.AirPressure, 1);
            }
            return true;
        }

        /// <summary>
        ///更新抗风压描述
        /// </summary>
        /// <param name="mode"></param>
        public bool Update_kfy_Info(Model_dt_kfy_Info model)
        {
            string sql = string.Format(@"update dt_kfy_Info  set desc ='{0}' where dt_Code = '{1}' and info_DangH='{2}'
                ", model.desc, model.dt_Code, model.info_DangH);
            return SQLiteHelper.ExecuteNonQuery(sql) > 0 ? true : false;
        }

        /// <summary>
        /// 获取单当数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="tong"></param>
        /// <returns></returns>
        public DataTable GetkfyByCodeAndTong(string code, string tong = null)
        {
            string sql = "select * from  dt_kfy_Info  where dt_Code='" + code + "'";

            if (!string.IsNullOrWhiteSpace(tong))
            {
                sql += " and info_DangH = '" + tong + "'";
            }
            var res = SQLiteHelper.ExecuteDataRow(sql);
            if (res == null)
            {
                return null;
            }
            return res.Table;
        }
    }
}
