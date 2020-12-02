using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace text.doors.Model
{
    public class LevelIndex
    {
        //渗透量
        public string Quantity { get; set; }
        //正压
        public double PressureZ { get; set; }
        //负压
        public double PressureF { get; set; }
    }
}
