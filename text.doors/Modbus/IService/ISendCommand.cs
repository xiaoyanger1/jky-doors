using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace text.doors.Modbus.IService
{
    interface ISendCommand
    {   /// <summary>
        /// 设置高压标0
        /// </summary>
        bool SendGYBD(bool logon = false);
        /// <summary>
        /// 设置风速归零
        /// </summary>
        void SendFSGL(ref bool IsSuccess, bool logon = false);

        /// <summary>
        /// 设置风速归零
        /// </summary>
        void SendFSGL(ref bool IsSuccess, bool logon = false);
        /// <summary>
        /// 获取温度显示
        /// </summary>
        double GetWDXS(ref bool IsSuccess);
        /// <summary>
        /// 获取大气压力显示
        /// </summary>
        double GetDQYLXS(ref bool IsSuccess);
        /// <summary>
        /// 获取风速显示
        /// </summary>
        double GetFSXS(ref bool IsSuccess);

        /// <summary>
        /// 读取差压显示
        /// </summary>
        /// <param name="IsSuccess"></param>
        /// <returns></returns>
        int GetCYXS(ref bool IsSuccess);
        /// <summary>
        /// 设置风机控制
        /// </summary>
        void SendFJKZ(double value, ref bool IsSuccess);
        /// <summary>
        /// 设置正压阀
        /// </summary>
        /// <param name="IsSuccess"></param>
        void SendZYF(ref bool IsSuccess);

        /// <summary>
        /// 设置负压阀
        /// </summary>
        /// <param name="IsSuccess"></param>
        void SendFYF(ref bool IsSuccess);
        /// <summary>
        /// 读取正负压阀
        /// </summary>
        void GetZFYF(ref bool IsSuccess, ref bool z, ref bool f);
        /*图标页面*/

        /// <summary>
        /// 设置正压预备
        /// </summary>
        /// <param name="IsSuccess"></param>
        void SetZYYB(ref bool IsSuccess);
        /// <summary>
        /// 读取正压预备结束
        /// </summary>
        /// <param name="IsSuccess"></param>
        int GetZYYBJS(ref bool IsSuccess);
        /// <summary>
        /// 发送正压开始
        /// </summary>
        void SendZYKS(ref bool IsSuccess);
        /// <summary>
        /// 读取正压开始结束
        /// </summary>
        /// <param name="IsSuccess"></param>
        double GetZYKSJS(ref bool IsSuccess);
        /// <summary>
        /// 发送负压预备
        /// </summary>
        /// <param name="IsSuccess"></param>
        void SendFYYB(ref bool IsSuccess);
        /// <summary>
        /// 发送负压开始
        /// </summary>
        void SendFYKS(ref bool IsSuccess);
        /// <summary>
        /// 读取正压预备结束
        /// </summary>
        /// <param name="IsSuccess"></param>
        int GetFYYBJS(ref bool IsSuccess);
        /// <summary>
        /// 读取负压开始结束
        /// </summary>
        /// <param name="IsSuccess"></param>
        double GetFYKSJS(ref bool IsSuccess);
        /// <summary>
        /// 获取正压预备时，设定压力值
        /// </summary>
        double GetZYYBYLZ(ref bool IsSuccess, string type);
        /// <summary>
        /// 获取正压100Pa是否开始计时
        /// </summary>
        bool Get_Z_S100TimeStart(ref bool IsSuccess);
        /// <summary>
        /// 获取正压150Pa是否开始计时
        /// </summary>
        bool Get_Z_S150PaTimeStart(ref bool IsSuccess);

        /// <summary>
        /// 获取降压100Pa是否开始计时
        /// </summary>
        bool Get_Z_J100PaTimeStart(ref bool IsSuccess);
        /// <summary>
        /// 获取负压100Pa是否开始计时
        /// </summary>
        bool Get_F_S100PaTimeStart(ref bool IsSuccess);
        /// <summary>
        /// 获取负压150Pa是否开始计时
        /// </summary>
        bool Get_F_S150PaTimeStart(ref bool IsSuccess);
        /// <summary>
        /// 获取负压降100Pa是否开始计时
        /// </summary>
        bool Get_F_J100PaTimeStart(ref bool IsSuccess);

        /*水密*/

        /// <summary>
        /// 设置水密预备
        /// </summary>
        /// <param name="IsSuccess"></param>
        void SetSMYB(ref bool IsSuccess);
        /// <summary>
        /// 读取水密预备结束
        /// </summary>
        /// <param name="IsSuccess"></param>
        int GetSMYBJS(ref bool IsSuccess);
        /// <summary>
        /// 读取水密预备设定压力
        /// </summary>
        /// <param name="IsSuccess"></param>
        int GetSMYBSDYL(ref bool IsSuccess, string type);
        /// <summary>
        /// 发送水密开始
        /// </summary>
        void SendSMXKS(ref bool IsSuccess);
        /// <summary>
        /// 发送水密性下一级
        /// </summary>
        void SendSMXXYJ(ref bool IsSuccess);

        /// <summary>
        /// 设置水密依次加压
        /// </summary>
        void SendSMYCJY(double value, ref bool IsSuccess);

        /// <summary>
        /// 急停
        /// </summary>
        void Stop(ref bool IsSuccess);
        /// <summary>
        /// 写入PID
        /// </summary>
        /// <param name="IsSuccess"></param>
        void SendPid(ref bool IsSuccess, string type, double value);
        /// <summary>
        /// 读取PID
        /// </summary>
        /// <param name="IsSuccess"></param>
        int GetPID(ref bool IsSuccess, string type);
    }
}
