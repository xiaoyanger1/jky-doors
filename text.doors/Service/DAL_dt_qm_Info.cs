using text.doors.Common;
using text.doors.Model.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using text.doors.Default;
using Young.Core.SQLite;
using text.doors.Service;

namespace text.doors.dal
{
    public class DAL_dt_qm_Info
    {
        /// <summary>
        /// 添加气密信息
        /// </summary>
        /// <param name="mode"></param>
        public bool Add(Model_dt_qm_Info model)
        {
            //删除结果
            SQLiteHelper.ExecuteNonQuery("delete from dt_qm_Info where dt_Code='" + model.dt_Code + "' and info_DangH = '" + model.info_DangH + "'");

            var sql = string.Format(@"insert into dt_qm_Info (dt_Code,info_DangH,qm_Z_FC,qm_F_FC,qm_Z_MJ,qm_F_MJ,
                qm_s_z_fj100,qm_s_z_fj150,qm_j_z_fj100,qm_s_z_zd100,qm_s_z_zd150,qm_j_z_zd100,qm_s_f_fj100,qm_s_f_fj150,qm_j_f_fj100,qm_s_f_zd100,qm_s_f_zd150,qm_j_f_zd100	) 
                values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                 model.dt_Code, model.info_DangH, model.qm_Z_FC, model.qm_F_FC, model.qm_Z_MJ, model.qm_F_MJ,
                 model.qm_s_z_fj100, model.qm_s_z_fj150, model.qm_j_z_fj100, model.qm_s_z_zd100, model.qm_s_z_zd150, model.qm_j_z_zd100, model.qm_s_f_fj100,
                 model.qm_s_f_fj150, model.qm_j_f_fj100, model.qm_s_f_zd100, model.qm_s_f_zd150, model.qm_j_f_zd100
                 );
            var res = SQLiteHelper.ExecuteNonQuery(sql) > 0 ? true : false;
            if (res)
            {
                new DAL_dt_Info().UpdateTestType(model.dt_Code, model.info_DangH, PublicEnum.SystemItem.Airtight, 1);
            }
            return true;
        }
        
     
        
        public void UpdateResult(Model_dt_Settings settings)
        {
            if (settings.dt_sm_Info != null && settings.dt_sm_Info.Count > 0)
            {
                var sql = "";
                foreach (var item in settings.dt_sm_Info)
                {
                    sql += $"update  dt_sm_Info set sm_PaDesc = '{item.sm_PaDesc}', sm_Pa='{item.sm_Pa}', sm_Remark='{item.sm_Remark}' where dt_Code ='{item.dt_Code}' and info_DangH='{item.info_DangH}';    ";
                }
                SQLiteHelper.ExecuteNonQuery(sql);
            }
            if (settings.dt_qm_Info != null && settings.dt_qm_Info.Count > 0)
            {
                string sql = "";
                foreach (var item in settings.dt_qm_Info)
                {
                    sql += $"update dt_qm_Info  set qm_Z_FC	='{item.qm_Z_FC}',qm_F_FC ='{item.qm_F_FC}',qm_Z_MJ	='{item.qm_Z_MJ}',qm_F_MJ	='{item.qm_F_MJ}' where dt_Code = '{item.dt_Code}' and info_DangH='{item.info_DangH}' ;   ";
                }
                SQLiteHelper.ExecuteNonQuery(sql);
            }
            if (settings.dt_kfy_Info != null && settings.dt_kfy_Info.Count > 0)
            {
                string sql = "";
                foreach (var item in settings.dt_kfy_Info)
                {
                    sql += $"update dt_kfy_Info  set p1	='{item.p1}',p2 ='{item.p2}',p3='{item.p3}',_p1='{item._p1}' ,_p2='{item._p2}',_p3='{item._p3}' where dt_Code = '{item.dt_Code}' and info_DangH='{item.info_DangH}' ;   ";
                }
                SQLiteHelper.ExecuteNonQuery(sql);
            }
        }
    }
}
