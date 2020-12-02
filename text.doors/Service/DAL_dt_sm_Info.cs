using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using text.doors.dal;
using text.doors.Default;
using text.doors.Model.DataBase;
using Young.Core.SQLite;

namespace text.doors.Service
{
    public class DAL_dt_sm_Info
    {
        /// <summary>
        /// 添加水密信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Add(Model_dt_sm_Info model)
        {
            //删除
            SQLiteHelper.ExecuteNonQuery("delete from dt_sm_Info where dt_Code='" + model.dt_Code + "' and info_DangH = '" + model.info_DangH + "'");

            var sql = string.Format("insert into dt_sm_Info (dt_Code,info_DangH,sm_PaDesc,sm_Pa,sm_Remark,Method) values('{0}','{1}','{2}','{3}','{4}','{5}')", model.dt_Code, model.info_DangH, model.sm_PaDesc, model.sm_Pa, model.sm_Remark,model.Method);
            var res = SQLiteHelper.ExecuteNonQuery(sql) > 0 ? true : false;
            if (res)
            {
                return new DAL_dt_Info().UpdateTestType(model.dt_Code, model.info_DangH, PublicEnum.SystemItem.Watertight, 1);
            }
            return true;
        }

    }
}
