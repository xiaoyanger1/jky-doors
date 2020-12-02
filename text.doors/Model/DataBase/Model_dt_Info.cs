using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace text.doors.Model.DataBase
{
    public class Model_dt_Info
    {
        /// <summary>
        /// 主表外键
        /// </summary>
        public string dt_Code { get; set; }
        /// <summary>
        /// 当前樘号
        /// </summary>
        public string info_DangH { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string info_Create { get; set; }

        public int Airtight { get; set; }

        public int Watertight { get; set; }
        public int WindPressure { get; set; }
    }
}
