using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Young.Core.SQLite;

namespace text.doors.Service
{
    public class DAL_dt_BaseSet
    {
        public  DataTable GetSystemBaseSet()
        {
            string sql = "select * from  dt_BaseSet";
            DataRow dr = SQLiteHelper.ExecuteDataRow(sql);
            if (dr != null)
            {
                return dr.Table;
            }
            return null;
        }
    }
}
