using Modbus.Device;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Windows.Forms;
using text.doors.Default;
using Young.Core.Common;

namespace text.doors.Common
{
    public class TCPClient
    {

        public static Young.Core.Logger.ILog Logger = Young.Core.Logger.LoggerManager.Current();

        public TcpClient tcpClient;
        public ModbusIpMaster _MASTER;
        /// <summary>
        /// 是否打开
        /// </summary>
        public bool IsTCPLink = false;

        public void TcpOpen()
        {
            IsTCPLink = false;
            if (_MASTER != null)
                _MASTER.Dispose();
            if (tcpClient != null)
                tcpClient.Close();
            if (LAN.IsLanLink)
            {
                try
                {
                    tcpClient = new TcpClient();
                    //开始一个对远程主机连接的异步请求
                    IAsyncResult asyncResult = tcpClient.BeginConnect(DefaultBase.IPAddress, DefaultBase.TCPPort, null, null);
                    asyncResult.AsyncWaitHandle.WaitOne(500, true);
                    if (!asyncResult.IsCompleted)
                    {
                        tcpClient.Close();
                        IsTCPLink = false;
                        Logger.Info("连接服务器失败!:IP" + DefaultBase.IPAddress + ",port:" + DefaultBase.TCPPort);
                        return;
                    }
                    //由TCP客户端创建Modbus TCP的主
                    _MASTER = ModbusIpMaster.CreateIp(tcpClient);
                    _MASTER.Transport.Retries = 0;   //不必调试
                    _MASTER.Transport.ReadTimeout = 1500;//读取超时
                    IsTCPLink = true;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    IsTCPLink = false;
                    tcpClient.Close();
                }
            }
        }


        private ushort _StartAddress = 0;
        private ushort _NumOfPoints = 1;
        private byte _SlaveID = 1;


        /// <summary>
        /// 设置高压标0
        /// </summary>
        public bool SendGYBD(bool logon = false)
        {
            if (!IsTCPLink)
                return false;
            try
            {
                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.高压标0_交替型按钮);
                bool[] readCoils = _MASTER.ReadCoils(_SlaveID, _StartAddress, _NumOfPoints);
                if (readCoils[0])
                    _MASTER.WriteSingleCoil(_StartAddress, false);
                else
                {
                    if (logon == false)
                        _MASTER.WriteSingleCoil(_StartAddress, true);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 设置风速归零
        /// </summary>
        public bool SendFSGL(bool logon = false)
        {
            if (!IsTCPLink)
                return false;
            try
            {
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.风速标0_交替型按钮);
                    bool[] readCoils = _MASTER.ReadCoils(_SlaveID, _StartAddress, _NumOfPoints);
                    if (readCoils[0])
                        _MASTER.WriteSingleCoil(_StartAddress, false);
                    else
                    {
                        if (logon == false)
                        {
                            _MASTER.WriteSingleCoil(_StartAddress, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取温度显示
        /// </summary>
        public double GetWDXS(ref bool IsSuccess)
        {
            double res = 0;
            try
            {
                if (!IsTCPLink)
                {
                    IsSuccess = false;
                    return res;
                }

                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.温度显示);

                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = double.Parse((double.Parse(holding_register[0].ToString()) / 10).ToString());
                    res = Formula.GetValues(PublicEnum.DemarcateType.enum_温度传感器, float.Parse(res.ToString()));
                    IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                IsSuccess = false;
            }
            return res;
        }

        /// <summary>
        /// 获取大气压力显示
        /// </summary>
        public double GetDQYLXS(ref bool IsSuccess)
        {
            double res = 0;
            try
            {
                if (!IsTCPLink)
                {
                    IsSuccess = false;
                    return res;
                }
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.大气压力显示);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = double.Parse((double.Parse(holding_register[0].ToString()) / 10).ToString());
                    res = Formula.GetValues(PublicEnum.DemarcateType.enum_大气压力传感器, float.Parse(res.ToString()));
                }
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                IsSuccess = false;
            }

            return res;
        }

        /// <summary>
        /// 获取风速显示
        /// </summary>
        public double GetFSXS(ref bool IsSuccess)
        {
            double res = 0;

            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }
            try
            {
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.风速显示);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    var f = double.Parse((double.Parse(holding_register[0].ToString()) / 100).ToString());
                    res = Formula.GetValues(PublicEnum.DemarcateType.enum_风速传感器, float.Parse(f.ToString()));
                    IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Logger.Error(ex);
            }
            return res;
        }

        /// <summary>
        /// 读取差压显示
        /// </summary>
        /// <param name="IsSuccess"></param>
        /// <returns></returns>
        public int GetCYXS(ref bool IsSuccess)
        {
            double res = 0;
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return 0;
            }
            try
            {
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.差压显示);

                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    var f = double.Parse(holding_register[0].ToString()) / 100;

                    if (int.Parse(holding_register[0].ToString()) > 1100)
                        f = -(65535 - int.Parse(holding_register[0].ToString()));
                    else
                        f = int.Parse(holding_register[0].ToString());

                    res = Formula.GetValues(PublicEnum.DemarcateType.enum_差压传感器, float.Parse(f.ToString()));
                    IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Logger.Error(ex);
            }
            return int.Parse(Math.Round(res, 0).ToString());
        }

        /// <summary>
        /// 设置风机控制
        /// </summary>
        public bool SendFJKZ(double value)
        {
            if (!IsTCPLink)
                return false;
            try
            {
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.风机控制);
                    _MASTER.WriteSingleRegister(_SlaveID, _StartAddress, (ushort)(value));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            return true;
        }


        /// <summary>
        /// 设置正压阀
        /// </summary>
        public bool SendZYF()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压阀);
                    _MASTER.WriteSingleCoil(_StartAddress, false);
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压阀);
                    _MASTER.WriteSingleCoil(_StartAddress, true);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 设置负压阀
        /// </summary>
        public bool SendFYF()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压阀);
                    _MASTER.WriteSingleCoil(_StartAddress, false);
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压阀);
                    _MASTER.WriteSingleCoil(_StartAddress, true);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 读取正负压阀
        /// </summary>
        public bool GetZFYF(ref bool z, ref bool f)
        {
            if (!IsTCPLink)
                return false;
            try
            {
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压阀);
                    bool[] readCoils_z = _MASTER.ReadCoils(_StartAddress, _NumOfPoints);
                    z = bool.Parse(readCoils_z[0].ToString());

                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压阀);
                    bool[] readCoils_f = _MASTER.ReadCoils(_StartAddress, _NumOfPoints);
                    f = bool.Parse(readCoils_f[0].ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            return true;
        }

        /*图标页面*/

        /// <summary>
        /// 设置正压预备
        /// </summary>
        /// <param name="IsSuccess"></param>
        public bool SetZYYB()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                var res = SendZYF();
                if (!res)
                    return false;

                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压预备);
                _MASTER.WriteSingleCoil(_StartAddress, false);
                _MASTER.WriteSingleCoil(_StartAddress, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 读取正压预备结束
        /// </summary>
        /// <param name="IsSuccess"></param>
        public int GetZYYBJS(ref bool IsSuccess)
        {
            int res = 0;
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }
            try
            {
                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压预备结束);
                ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                res = int.Parse(holding_register[0].ToString());
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Logger.Error(ex);
            }

            return res;
        }

        /// <summary>
        /// 发送正压开始
        /// </summary>
        public bool SendZYKS(ref bool IsSuccess)
        {
            if (!IsTCPLink)
                return false;
            try
            {
                var res = SendZYF();
                if (!res)
                {
                    return false;
                }
                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压开始);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, false);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            return true;
        }


        /// <summary>
        /// 读取正压开始结束
        /// </summary>
        /// <param name="IsSuccess"></param>
        public double GetZYKSJS(ref bool IsSuccess)
        {
            double res = 0;
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }
            try
            {
                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压开始结束);
                ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                res = double.Parse(holding_register[0].ToString());
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Logger.Error(ex);
            }
            return res;
        }

        /// <summary>
        /// 发送负压预备
        /// </summary>
        /// <param name="IsSuccess"></param>
        public bool SendFYYB()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                var res = SendFYF();
                if (!res)
                {
                    return false;
                }

                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压预备);
                _MASTER.WriteSingleCoil(_StartAddress, false);
                _MASTER.WriteSingleCoil(_StartAddress, true);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 发送负压开始
        /// </summary>
        public bool SendFYKS()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                var res = SendFYF();
                if (!res)
                {
                    return false;
                }
                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压开始);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, false);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            return true;
        }


        /// <summary>
        /// 读取正压预备结束
        /// </summary>
        /// <param name="IsSuccess"></param>
        public int GetFYYBJS(ref bool IsSuccess)
        {
            int res = 0;
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }

            try
            {
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压预备结束);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = int.Parse(holding_register[0].ToString());
                    IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Logger.Error(ex);
            }
            return res;
        }

        /// <summary>
        /// 读取负压开始结束
        /// </summary>
        /// <param name="IsSuccess"></param>
        public double GetFYKSJS(ref bool IsSuccess)
        {
            double res = 0;
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }
            try
            {
                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压开始结束);
                ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                res = double.Parse(holding_register[0].ToString());
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Logger.Error(ex);
            }

            return res;
        }



        /// <summary>
        /// 获取正压预备时，设定压力值
        /// </summary>
        public double GetZYYBYLZ(ref bool IsSuccess, string type)
        {
            double res = 0;
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }

            try
            {
                lock (_MASTER)
                {
                    if (type == "ZYYB")
                    {
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压预备_设定值);
                    }
                    else if (type == "ZYKS")
                    {
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压开始_设定值);
                    }
                    else if (type == "FYYB")
                    {
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压预备_设定值);
                    }
                    else if (type == "FYKS")
                    {
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压开始_设定值);
                    }

                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = double.Parse((double.Parse(holding_register[0].ToString())).ToString());
                    IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Logger.Error(ex);
            }
            return res;
        }



        /// <summary>
        /// 获取正压100Pa是否开始计时
        /// </summary>
        public bool Get_Z_S100TimeStart()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压100TimeStart);
                    ushort[] t = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (Convert.ToInt32(t[0]) > 20)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }

        }

        /// <summary>
        /// 获取正压150Pa是否开始计时
        /// </summary>
        public bool Get_Z_S150PaTimeStart()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压150TimeStart);
                    ushort[] t = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (Convert.ToInt32(t[0]) > 20)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 获取降压100Pa是否开始计时
        /// </summary>
        public bool Get_Z_J100PaTimeStart()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压_100TimeStart);
                    ushort[] t = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (Convert.ToInt32(t[0]) > 20)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 获取负压100Pa是否开始计时
        /// </summary>
        public bool Get_F_S100PaTimeStart()
        {
            if (!IsTCPLink)
                return false;

            try
            {
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压100TimeStart);
                    ushort[] t = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (Convert.ToInt32(t[0]) > 20)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 获取负压150Pa是否开始计时
        /// </summary>
        public bool Get_F_S150PaTimeStart()
        {
            if (!IsTCPLink)
                return false;

            try
            {
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压150TimeStart);
                    ushort[] t = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (Convert.ToInt32(t[0]) > 20)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {

                Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 获取负压降100Pa是否开始计时
        /// </summary>
        public bool Get_F_J100PaTimeStart()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压_100TimeStart);
                    ushort[] t = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (Convert.ToInt32(t[0]) > 20)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }
        /*水密*/

        /// <summary>
        /// 设置水密预备
        /// </summary>
        /// <param name="IsSuccess"></param>
        public bool SetSMYB()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                var res = SendZYF();
                if (!res)
                    return false;

                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.水密性预备加压);
                _MASTER.WriteSingleCoil(_StartAddress, false);
                _MASTER.WriteSingleCoil(_StartAddress, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            return true;
        }


        /// <summary>
        /// 读取水密预备结束
        /// </summary>
        /// <param name="IsSuccess"></param>
        public int GetSMYBJS(ref bool IsSuccess)
        {
            int res = 0;
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }
            try
            {
                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.水密预备结束);
                ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                res = int.Parse(holding_register[0].ToString());
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Logger.Error(ex);
            }
            return res;
        }


        /// <summary>
        /// 读取水密预备设定压力
        /// </summary>
        /// <param name="IsSuccess"></param>
        public int GetSMYBSDYL(ref bool IsSuccess, string type)
        {
            int res = 0;
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }

            try
            {
                if (type == "SMYB")
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.水密预备_设定值);
                }
                else if (type == "SMKS")
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.水密开始_设定值);
                }
                else if (type == "YCJY")
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.水密依次加压_设定值);
                }
                else if (type == "XYJ")
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.水密开始_设定值);
                }

                ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                res = int.Parse(holding_register[0].ToString());
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Logger.Error(ex);
            }
            return res;
        }

        /// <summary>
        /// 发送水密开始
        /// </summary>
        public bool SendSMXKS()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                var res = SendZYF();
                if (!res)
                    return false;

                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.水密性开始);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, false);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, true);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 发送水密性下一级
        /// </summary>
        public bool SendSMXXYJ()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.下一级);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, false);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, true);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }



        /// <summary>
        /// 设置水密依次加压
        /// </summary>
        public bool SendSMYCJY(double value)
        {
            if (!IsTCPLink)
                return false;
            try
            {
                var res = SendZYF();
                if (!res)
                    return false;

                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.依次加压数值);
                _MASTER.WriteSingleRegister(_SlaveID, _StartAddress, (ushort)(value));

                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.依次加压);
                _MASTER.WriteSingleCoil(_StartAddress, false);
                _MASTER.WriteSingleCoil(_StartAddress, true);
                return true;

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }

        }


        /// <summary>
        /// 急停
        /// </summary>
        public bool Stop()
        {
            if (!IsTCPLink)
                return false;

            try
            {
                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.急停);
                _MASTER.WriteSingleCoil(_StartAddress, false);
                _MASTER.WriteSingleCoil(_StartAddress, true);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
                // System.Environment.Exit(0);
            }
        }

        /// <summary>
        /// 写入PID
        /// </summary>
        /// <param name="IsSuccess"></param>
        public bool SendPid(string type, double value)
        {
            if (!IsTCPLink)
                return false;
            try
            {
                if (type == "P")
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.P);
                else if (type == "I")
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.I);
                else if (type == "D")
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.D);

                _MASTER.WriteSingleRegister(_SlaveID, _StartAddress, (ushort)value);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 读取PID
        /// </summary>
        /// <param name="IsSuccess"></param>
        public int GetPID(string type, ref bool IsSuccess)
        {
            int res = 0;
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }
            try
            {
                if (type == "P")
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.P);
                else if (type == "I")
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.I);
                else if (type == "D")
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.D);

                ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                res = int.Parse(holding_register[0].ToString());
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Logger.Error(ex);
            }
            return res;
        }

        /// <summary>
        /// 获取位移传感器1
        /// </summary>
        public double GetDisplace1(ref bool IsSuccess)
        {
            double res = 0;
            try
            {
                if (!IsTCPLink)
                {
                    IsSuccess = false;
                    return res;
                }
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.位移1);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = double.Parse((double.Parse(holding_register[0].ToString()) / 10).ToString());
                    //res = Formula.GetValues(PublicEnum.DemarcateType.enum_大气压力传感器, float.Parse(res.ToString()));
                    //todo:位移标定
                }
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                IsSuccess = false;
            }

            return res;
        }

        /// <summary>
        /// 获取位移传感器2
        /// </summary>
        public double GetDisplace2(ref bool IsSuccess)
        {
            double res = 0;
            try
            {
                if (!IsTCPLink)
                {
                    IsSuccess = false;
                    return res;
                }
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.位移2);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = double.Parse((double.Parse(holding_register[0].ToString()) / 10).ToString());
                    //res = Formula.GetValues(PublicEnum.DemarcateType.enum_大气压力传感器, float.Parse(res.ToString()));
                    //todo:位移标定
                }
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                IsSuccess = false;
            }

            return res;
        }

        #region 风压

        /// <summary>
        /// 获取位移传感器3
        /// </summary>
        public double GetDisplace3(ref bool IsSuccess)
        {
            double res = 0;
            try
            {
                if (!IsTCPLink)
                {
                    IsSuccess = false;
                    return res;
                }
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.位移3);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = double.Parse((double.Parse(holding_register[0].ToString()) / 10).ToString());
                    //todo:位移标定
                    //res = Formula.GetValues(PublicEnum.DemarcateType.enum_大气压力传感器, float.Parse(res.ToString()));
                }
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                IsSuccess = false;
            }

            return res;
        }

        /// <summary>
        /// 设置位移归零
        /// </summary>
        public bool SendWYGL(bool logon = false)
        {
            if (!IsTCPLink)
                return false;
            try
            {
                lock (_MASTER)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.位移置零);
                    bool[] readCoils = _MASTER.ReadCoils(_SlaveID, _StartAddress, _NumOfPoints);
                    if (readCoils[0])
                        _MASTER.WriteSingleCoil(_StartAddress, false);
                    else
                    {
                        if (logon == false)
                        {
                            _MASTER.WriteSingleCoil(_StartAddress, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 设置抗风压正压预备
        /// </summary>
        /// <param name="IsSuccess"></param>
        public bool SetKFYZYYB()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                var res = SendZYF();
                if (!res)
                    return false;

                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.风压正压预备);
                _MASTER.WriteSingleCoil(_StartAddress, false);
                _MASTER.WriteSingleCoil(_StartAddress, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 发送正压开始
        /// </summary>
        public bool SendKFYZYKS()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                var res = SendZYF();
                if (!res)
                {
                    return false;
                }
                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.风压正压开始);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, false);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 发送抗风压负压预备
        /// </summary>
        /// <param name="IsSuccess"></param>
        public bool SendKFYFYYB()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                var res = SendFYF();
                if (!res)
                {
                    return false;
                }

                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.风压负压预备);
                _MASTER.WriteSingleCoil(_StartAddress, false);
                _MASTER.WriteSingleCoil(_StartAddress, true);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 发送抗风压负压开始
        /// </summary>
        public bool SendKFYFYKS()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                var res = SendFYF();
                if (!res)
                {
                    return false;
                }
                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.风压负压开始);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, false);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            return true;
        }


        /// <summary>
        /// 设置正反复
        /// </summary>
        public bool SendZFF(double value)
        {
            if (!IsTCPLink)
                return false;
            try
            {
                var res = SendZYF();
                if (!res)
                    return false;

                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.反复数值);
                _MASTER.WriteSingleRegister(_SlaveID, _StartAddress, (ushort)(value));

                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正反复);
                _MASTER.WriteSingleCoil(_StartAddress, false);
                _MASTER.WriteSingleCoil(_StartAddress, true);
                return true;

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }
        /// <summary>
        /// 设置负反复
        /// </summary>
        public bool SendFFF(double value)
        {
            if (!IsTCPLink)
                return false;
            try
            {
                var res = SendZYF();
                if (!res)
                    return false;

                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.反复数值);
                _MASTER.WriteSingleRegister(_SlaveID, _StartAddress, (ushort)(value));

                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负反复);
                _MASTER.WriteSingleCoil(_StartAddress, false);
                _MASTER.WriteSingleCoil(_StartAddress, true);
                return true;

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }

        }


        /// <summary>
        /// 设置正安全
        /// </summary>
        public bool SendZAQ(double value)
        {
            if (!IsTCPLink)
                return false;
            try
            {
                var res = SendZYF();
                if (!res)
                    return false;

                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.安全数值);
                _MASTER.WriteSingleRegister(_SlaveID, _StartAddress, (ushort)(value));

                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正安全);
                _MASTER.WriteSingleCoil(_StartAddress, false);
                _MASTER.WriteSingleCoil(_StartAddress, true);
                return true;

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }
        /// <summary>
        /// 设置负安全
        /// </summary>
        public bool SendFAQ(double value)
        {
            if (!IsTCPLink)
                return false;
            try
            {
                var res = SendZYF();
                if (!res)
                    return false;

                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.安全数值);
                _MASTER.WriteSingleRegister(_SlaveID, _StartAddress, (ushort)(value));

                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负安全);
                _MASTER.WriteSingleCoil(_StartAddress, false);
                _MASTER.WriteSingleCoil(_StartAddress, true);
                return true;

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }

        }



        #endregion





    }

}
