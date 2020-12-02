using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace text.doors.Default
{
    /// <summary>
    /// 寄存器命令常量
    /// </summary>
    public static class BFMCommand
    {
        //public const string 风速0标定 = "M200";
        //public const string 风速1 = "M201";
        //public const string 风速3 = "M202";
        //public const string 风速5 = "M203";
        //public const string 风速8 = "M204";
        //public const string 风速10 = "M205";
        //public const string 风速13 = "M206";
        //public const string 风速15 = "M207";
        //public const string 风速18 = "M208";
        //public const string 风速20 = "M209";
        //public const string 差压0 = "M210";
        //public const string 差压100 = "M211";
        //public const string 差压200 = "M212";
        //public const string 差压500 = "M213";
        //public const string 差压800 = "M214";
        //public const string 差压1000 = "M215";
        //public const string 差压_100 = "M216";
        //public const string 差压_200 = "M217";
        //public const string 差压_500 = "M218";
        //public const string 差压_800 = "M219";
        //public const string 差压_1000 = "M220";

        //public const string 温度_40 = "M221";

        //public const string 温度0 = "M222";
        //  public const string 温度80 = "M223";
        //  public const string 大气压力80 = "M224";
        // public const string 大气压力103_1 = "M225";
        // public const string 大气压力110 = "M226";
        // public const string 标定全部清除 = "M60";
        //public const string 正极限 = "X0";
        //public const string 负极限 = "X1";
        //public const string 正压阀 = "Y0";
        //public const string 负压阀 = "Y1";


        public const string 高压标0_交替型按钮 = "M66";
        public const string 风速标0_交替型按钮 = "M66";
        public const string 正压阀 = "M62";
        public const string 负压阀 = "M63";
        public const string 正压预备 = "M50";
        public const string 正压开始 = "M51";
        public const string 负压预备 = "M52";
        public const string 负压开始 = "M53";
        public const string 水密性预备加压 = "M54";
        public const string 水密性开始 = "M45";
        public const string 依次加压数值 = "D550";
        public const string 依次加压 = "M46";
        //public const string 结束 = "M40";
        public const string 下一级 = "M42";
        public const string 急停 = "M120";
        public const string 差压显示 = "D130";
        public const string 温度显示 = "D132";
        public const string 大气压力显示 = "D134";
        public const string 风速显示 = "D136";
        //public const string 差压标定后值 = "D130";
        //public const string 温度标定值 = "D132";
        //public const string 大气压力标定值 = "D134";
        // public const string 风速标定值 = "D136";
        // public const string 正负负压开始结束 = "T20";


        public const string 正压预备结束 = "D30";
        public const string 负压预备结束 = "D31";
        public const string 正压开始结束 = "T20";
        public const string 负压开始结束 = "T10";


        public const string 风压正压预备结束 = "D33";
        public const string 风压负压预备结束 = "D34";

        public const string 风压正压开始结束 = "T24";
        public const string 风压负压开始结束 = "T27";

        //五次
        public const string 正反复结束 = "D35";
        public const string 负反复结束 = "D36";


        public const string 正安全结束 = "T33";
        public const string 负安全结束 = "T35";


        public const string 风机控制 = "D40";
        public const string 风机设定值 = "D40";
        public const string 正压预备_设定值 = "D2000";
        public const string 正压开始_设定值 = "D2001";
        public const string 负压预备_设定值 = "D2002";
        public const string 负压开始_设定值 = "D2005";

        public const string 正压100TimeStart = "T2";
        public const string 正压150TimeStart = "T3";
        public const string 正压_100TimeStart = "T4";
        public const string 负压100TimeStart = "T7";
        public const string 负压150TimeStart = "T8";
        public const string 负压_100TimeStart = "T9";


        //public const string 水密设定压力 = "D2009";
        //public const string 水密开始结束 = "T12";


        public const string 水密预备结束 = "D32";
        public const string 水密预备_设定值 = "D2008";
        public const string 水密开始_设定值 = "D2009";
        public const string 水密依次加压_设定值 = "D550";



        #region  波动加压

        public const string 国标检测波动加压开始 = "M47";
        public const string 上限压力设定 = "D350";
        public const string 下限压力设定 = "D354";
        public const string 工程检测水密性波动开始 = "M48";
        public const string 工程检测水密性停止加压 = "M49";


        public const string 读取设定波动加压Min = "D16";
        public const string 读取设定波动加压Max = "D14";
        public const string 水密开始波动结束 = "T17";

        public const string 水密开始波动逐级结束 = "D46";

        #endregion



        //气密
        public const string P = "D2101";
        public const string I = "D2102";
        public const string D = "D2103";
        //水密
        public const string _P = "D2151";
        public const string _I = "D2152";
        public const string _D = "D2153";

        //波动
        public const string B_P = "D2251";
        public const string B_I = "D2252";
        public const string B_D = "D2253";

        #region 风压

        //public const string 位移1 = "D410";
        //public const string 位移2 = "D420";
        //public const string 位移3 = "D430";


        public const string 位移1 = "D440";
        public const string 位移2 = "D442";
        public const string 位移3 = "D444";

        public const string 风压正压预备 = "M55";
        public const string 风压正压开始 = "M56";
        public const string 风压负压预备 = "M57";
        public const string 风压负压开始 = "M58";
        public const string 位移1标零 = "M67";
        public const string 位移2标零 = "M68";
        public const string 位移3标零 = "M69";

        public const string 风压_正压是否计时 = "T23";
        public const string 风压_负压是否计时 = "T26";


        public const string 风压_正压预备_设定值 = "D2200";
        public const string 风压_正压开始_设定值 = "D2202";
        public const string 风压_负压预备_设定值 = "D2204";
        public const string 风压_负压开始_设定值 = "D2206";

        #region 未实现


        //public const string 风压差压标定后值 = "D130";

        #endregion

        public const string 正反复数值 = "D2208";
        public const string 负反复数值 = "D2210";
        public const string 正安全数值 = "D560";
        public const string 负安全数值 = "D566";

        public const string 正反复 = "M80";
        public const string 负反复 = "M81";
        public const string 正安全 = "M82";
        public const string 负安全 = "M83";
        #endregion









        private static int m_10_Min = 0;//M命令十进制最小值
        private static int m_16_Min = 2048;//M命令十六进制最小值
        private static int d_10_Min = 0;//D命令十进制最小值
        private static int d_16_Min = 4096;//D命令十六进制最小值
        private static int x_10_Min = 0;//D命令十进制最小值
        private static int x_16_Min = 1024;//D命令十六进制最小值
        private static int y_10_Min = 0;//D命令十进制最小值
        private static int y_16_Min = 1280;//D命令十六进制最小值
        private static int t_10_Min = 0;//t命令十进制最小值
        private static int t_16_Min = 1536;//t命令十六进制最小值


        /// <summary>
        /// 获取命令对应关系
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static ushort GetCommandDict(string command)
        {
            ushort res = 0;
            if (string.IsNullOrWhiteSpace(command))
            {
                return res;
            }

            string name = command.Substring(0, 1);
            int con = int.Parse(command.Substring(1, command.Length - 1));

            switch (name)
            {
                case "D":
                    res = (ushort)(con - d_10_Min + d_16_Min);
                    break;
                case "M":
                    res = (ushort)(con - m_10_Min + m_16_Min);
                    break;
                case "X":
                    res = (ushort)(con - x_10_Min + x_16_Min);
                    break;
                case "Y":
                    res = (ushort)(con - y_10_Min + y_16_Min);
                    break;
                case "T":
                    res = (ushort)(con - t_10_Min + t_16_Min);
                    break;
                default:
                    break;
            }
            return res;
        }
    }
}
