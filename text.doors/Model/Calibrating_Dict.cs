using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace text.doors.Model
{
    /// <summary>
    /// 获取标定范围字典
    /// </summary>
    public class Calibrating_Dict
    {
        public float x { get; set; }
        public float y { get; set; }
        public string Enum { get; set; }
        public float k { get; set; }
        public float b { get; set; }
    }
}
