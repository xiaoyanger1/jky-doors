using Modbus.Device;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using text.doors.Default;
using Young.Core.Common;

namespace text.doors.Common
{
    public class TCPClient
    {

        public static Young.Core.Logger.ILog Logger = Young.Core.Logger.LoggerManager.Current();
        private ushort _StartAddress = 0;
        private ushort _NumOfPoints = 1;
        private byte _SlaveID = 1;
        public TcpClient tcpClient;
        public ModbusIpMaster _MASTER;
        private static Object syncLock = new Object();
        /// <summary>
        /// 是否打开
        /// </summary>
        public bool IsTCPLink = false;

        #region TCP

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

        #endregion

        #region 气密

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
                lock (syncLock)
                {
                    var res = SendZYF();
                    if (!res)
                        return false;

                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压预备);
                    _MASTER.WriteSingleCoil(_StartAddress, false);
                    _MASTER.WriteSingleCoil(_StartAddress, true);
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压预备结束);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = int.Parse(holding_register[0].ToString());
                    IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
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
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压开始结束);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = double.Parse(holding_register[0].ToString());
                }
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
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

            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
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
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压预备结束);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = int.Parse(holding_register[0].ToString());
                    IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压开始结束);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = double.Parse(holding_register[0].ToString());
                    IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
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
                IsTCPLink = false;
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
                lock (syncLock)
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
                IsTCPLink = false;
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
                lock (syncLock)
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
                IsTCPLink = false;
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
                lock (syncLock)
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
                IsTCPLink = false;
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
                lock (syncLock)
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
                IsTCPLink = false;
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
                lock (syncLock)
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
                IsTCPLink = false;
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
                lock (syncLock)
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
                IsTCPLink = false;
                Logger.Error(ex);
                return false;
            }
        }

        #endregion

        #region 水密

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
                lock (syncLock)
                {
                    var res = SendZYF();
                    if (!res)
                        return false;

                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.水密性预备加压);
                    _MASTER.WriteSingleCoil(_StartAddress, false);
                    _MASTER.WriteSingleCoil(_StartAddress, true);
                 
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.水密预备结束);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = int.Parse(holding_register[0].ToString());
                    IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                IsSuccess = false;
                Logger.Error(ex);
            }
            return res;
        }

        /// <summary>
        /// 读取水密预备结束——波动
        /// </summary>
        /// <param name="IsSuccess"></param>
        public int GetSMXYJJS_BD(ref bool IsSuccess)
        {
            int res = 0;
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }
            try
            {
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.水密预备结束);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = int.Parse(holding_register[0].ToString());
                    IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                IsSuccess = false;
                Logger.Error(ex);
            }
            return res;
        }

        /// <summary>
        /// 读取水密波动加压结束
        /// </summary>
        /// <param name="IsSuccess"></param>
        public int GetSMJS_BD(ref bool IsSuccess)
        {
            int res = 0;
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }
            try
            {
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.水密开始波动结束);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = int.Parse(holding_register[0].ToString());
                    IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                IsSuccess = false;
                Logger.Error(ex);
            }
            return res;
        }


        /// <summary>
        /// 读取水密预备设定压力-稳定加压
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
                lock (syncLock)
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
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                IsSuccess = false;
                Logger.Error(ex);
            }
            return res;
        }

        /// <summary>
        /// 发送水密开始-稳定加压
        /// </summary>
        public bool SendSMXKS()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                lock (syncLock)
                {
                    var res = SendZYF();
                    if (!res)
                        return false;

                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.水密性开始);
                    _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, false);
                    _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 发送水密性下一级 -稳定加压
        /// </summary>
        public bool SendSMXXYJ()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.下一级);
                    _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, false);
                    _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
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

            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.急停);
                    _MASTER.WriteSingleCoil(_StartAddress, true);
                    _MASTER.WriteSingleCoil(_StartAddress, false);
                    return true;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                Logger.Error(ex);
                return false;
                // System.Environment.Exit(0);
            }
        }
        #endregion


        #region 波动加压

        public bool SendSMXKS_波动()
        {
            if (!IsTCPLink)
                return false;
            try
            {
                lock (syncLock)
                {
                    var res = SendZYF();
                    if (!res)
                        return false;

                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.国标检测波动加压开始);
                    _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, false);
                    _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 停止波动
        /// </summary>
        public bool StopBoDong()
        {
            if (!IsTCPLink)
                return false;

            try
            {
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.工程检测水密性停止加压);
                     _MASTER.WriteSingleCoil(_StartAddress, true);
                    _MASTER.WriteSingleCoil(_StartAddress, false);
                    return true;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 设置水密-工程检测波动开始波动
        /// </summary>
        public bool SendBoDongksjy(double maxValue, double minValue)
        {
            if (!IsTCPLink)
                return false;
            try
            {
                lock (syncLock)
                {
                    var res = SendZYF();
                    if (!res)
                        return false;

                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.上限压力设定);
                    _MASTER.WriteSingleRegister(_SlaveID, _StartAddress, (ushort)(maxValue));

                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.下限压力设定);
                    _MASTER.WriteSingleRegister(_SlaveID, _StartAddress, (ushort)(minValue));


                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.工程检测水密性波动开始);
                    _MASTER.WriteSingleCoil(_StartAddress, true);
                    _MASTER.WriteSingleCoil(_StartAddress, false);
                    return true;
                }

            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 切换波动
        /// </summary>
        /// <param name="IsSuccess"></param>
        public bool qiehuanTab(bool type)
        {
            if (!IsTCPLink)
            {
                return false;
            }
            try
            {
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.国标检测波动加压开始);
                    _MASTER.WriteSingleCoil(_StartAddress, type);
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                Logger.Error(ex);
                return false;
            }
            return true;
        }


        #endregion


        #region 位移

        /// <summary>
        /// 获取位移传感器1
        /// </summary>
        public double GetDisplace1(ref bool IsSuccess)
        {
            double res = 0;
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }
            try
            {
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.位移1);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (holding_register.Length > 0)
                    {
                        res = double.Parse((double.Parse(holding_register[0].ToString()) / 1000).ToString());
                        res = Formula.GetValues(PublicEnum.DemarcateType.位移传感器1, float.Parse(res.ToString()));
                        IsSuccess = true;
                    }
                }

            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }
            try
            {
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.位移2);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (holding_register.Length > 0)
                    {
                        res = double.Parse((double.Parse(holding_register[0].ToString()) / 1000).ToString());
                        res = Formula.GetValues(PublicEnum.DemarcateType.位移传感器2, float.Parse(res.ToString()));
                        IsSuccess = true;
                    }
                }

            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                Logger.Error(ex);
                IsSuccess = false;
            }

            return res;
        }


        /// <summary>
        /// 获取位移传感器3
        /// </summary>
        public double GetDisplace3(ref bool IsSuccess)
        {
            double res = 0;
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }
            try
            {
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.位移3);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (holding_register.Length > 0)
                    {
                        res = double.Parse((double.Parse(holding_register[0].ToString()) / 1000).ToString());
                        res = Formula.GetValues(PublicEnum.DemarcateType.位移传感器3, float.Parse(res.ToString()));
                        IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.位移1标零);
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

                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.位移2标零);
                    bool[] readCoils2 = _MASTER.ReadCoils(_SlaveID, _StartAddress, _NumOfPoints);
                    if (readCoils2[0])
                        _MASTER.WriteSingleCoil(_StartAddress, false);
                    else
                    {
                        if (logon == false)
                        {
                            _MASTER.WriteSingleCoil(_StartAddress, true);
                        }
                    }

                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.位移3标零);
                    bool[] readCoils3 = _MASTER.ReadCoils(_SlaveID, _StartAddress, _NumOfPoints);
                    if (readCoils3[0])
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
                IsTCPLink = false;
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
                lock (syncLock)
                {
                    var res = SendZYF();
                    if (!res)
                        return false;

                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.风压正压预备);
                    _MASTER.WriteSingleCoil(_StartAddress, false);
                    _MASTER.WriteSingleCoil(_StartAddress, true);
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
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
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
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

            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
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
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                Logger.Error(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 设置风压安全反复数值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set_FY_Value(string commandValue, string commandStr, double value, bool isZ = true)
        {
            if (!IsTCPLink)
                return false;
            try
            {
                lock (syncLock)
                {
                    var res = isZ ? SendZYF() : SendFYF();
                    if (!res)
                        return false;

                    _StartAddress = BFMCommand.GetCommandDict(commandValue);
                    _MASTER.WriteSingleRegister(_SlaveID, _StartAddress, (ushort)(value));

                    _StartAddress = BFMCommand.GetCommandDict(commandStr);
                    _MASTER.WriteSingleCoil(_StartAddress, false);
                    _MASTER.WriteSingleCoil(_StartAddress, true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                Logger.Error(ex);
                return false;
            }
        }




        /// <summary>
        /// 获取风压是否开始计时
        /// </summary>
        public bool Read_FY_Static_IsStart(string commandStr)
        {
            if (!IsTCPLink)
                return false;
            try
            {
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(commandStr);
                    ushort[] t = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (Convert.ToInt32(t[0]) >= 40)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                Logger.Error(ex);
                return false;
            }

        }


        /// <summary>
        /// 读取风压按钮状态
        /// </summary>
        /// <param name="IsSuccess"></param>
        public int Read_FY_BtnType(string commandStr, ref bool IsSuccess)
        {
            int res = 0;
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }
            try
            {
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(commandStr);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = int.Parse(holding_register[0].ToString());
                    IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                IsSuccess = false;
                Logger.Error(ex);
            }
            return res;
        }



        /// <summary>
        /// 获取正压预备时，设定压力值
        /// </summary>
        public double Read_FY_Btn_SetValue(ref bool IsSuccess, string type)
        {
            double res = 0;
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }

            try
            {
                lock (syncLock)
                {
                    if (type == "ZYYB")
                    {
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.风压_正压预备_设定值);
                    }
                    else if (type == "ZYKS")
                    {
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.风压_正压开始_设定值);
                    }
                    else if (type == "FYYB")
                    {
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.风压_负压预备_设定值);
                    }
                    else if (type == "FYKS")
                    {
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.风压_负压开始_设定值);
                    }

                    else if (type == "ZAQ")
                    {
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正安全数值);
                    }
                    else if (type == "FAQ")
                    {
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负安全数值);
                    }
                    else if (type == "ZFF")
                    {
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正反复数值);
                    }
                    else if (type == "FFF")
                    {
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负反复数值);
                    }

                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = double.Parse((double.Parse(holding_register[0].ToString())).ToString());
                    IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                IsSuccess = false;
                Logger.Error(ex);
            }
            return res;
        }


        /// <summary>
        /// 发送风压按钮
        /// </summary>
        /// <param name="IsSuccess"></param>
        /// <param name="isZ">是否正压</param>
        public bool Send_FY_Btn(string commandStr, bool isZ = true)
        {
            if (!IsTCPLink)
                return false;
            try
            {
                lock (syncLock)
                {
                    var res = isZ ? SendZYF() : SendFYF();
                    if (!res)
                        return false;

                    _StartAddress = BFMCommand.GetCommandDict(commandStr);
                    _MASTER.WriteSingleCoil(_StartAddress, false);
                    _MASTER.WriteSingleCoil(_StartAddress, true);
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                Logger.Error(ex);
                return false;
            }
            return true;
        }

        #endregion

        #region 首页


        /// <summary>
        /// 设置高压标0
        /// </summary>
        public bool SendGYBD(bool logon = false)
        {
            if (!IsTCPLink)
                return false;
            try
            {
                lock (syncLock)
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
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
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
                IsTCPLink = false;
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
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }
            try
            {
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.温度显示);

                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (holding_register.Length > 0)
                    {
                        res = double.Parse((double.Parse(holding_register[0].ToString()) / 10).ToString());
                        res = Formula.GetValues(PublicEnum.DemarcateType.温度传感器, float.Parse(res.ToString()));
                    }
                    IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }

            try
            {
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.大气压力显示);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (holding_register.Length > 0)
                    {
                        res = double.Parse((double.Parse(holding_register[0].ToString()) / 10).ToString());
                        res = Formula.GetValues(PublicEnum.DemarcateType.大气压力传感器, float.Parse(res.ToString()));
                    }
                }
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.风速显示);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (holding_register.Length > 0)
                    {
                        var f = double.Parse((double.Parse(holding_register[0].ToString()) / 100).ToString());
                        res = Formula.GetValues(PublicEnum.DemarcateType.风速传感器, float.Parse(f.ToString()));

                        IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.差压显示);

                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (holding_register.Length > 0)
                    {
                        var f = double.Parse(holding_register[0].ToString());// / 100;

                        if (int.Parse(holding_register[0].ToString()) > 10000)
                            f = -(65535 - int.Parse(holding_register[0].ToString()));
                        else
                            f = int.Parse(holding_register[0].ToString());

                        //if (int.Parse(holding_register[0].ToString()) > 1100)
                        //    f = -(65535 - int.Parse(holding_register[0].ToString()));
                        //else
                        //    f = int.Parse(holding_register[0].ToString());

                        res = Formula.GetValues(PublicEnum.DemarcateType.差压传感器, float.Parse(f.ToString()));
                        IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                IsSuccess = false;
                Logger.Error(ex);
            }
            return int.Parse(Math.Round(res, 0).ToString());
        }


        /// <summary>
        /// 读取波动差压显示（min ,max ）
        /// </summary>
        /// <param name="IsSuccess"></param>
        /// <returns></returns>
        public void GetCYXS_BODONG(ref bool IsSuccess, ref int minVal, ref int maxVal)
        {
            // double res = 0;
            if (!IsTCPLink)
            {
                IsSuccess = false;
            }
            try
            {
                lock (syncLock)
                {
                    //最小
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.读取设定波动加压Min);

                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (holding_register.Length > 0)
                    {
                        var f = double.Parse(holding_register[0].ToString());

                        if (int.Parse(holding_register[0].ToString()) > 10000)
                            f = -(65535 - int.Parse(holding_register[0].ToString()));
                        else
                            f = int.Parse(holding_register[0].ToString());

                        var res = Formula.GetValues(PublicEnum.DemarcateType.差压传感器, float.Parse(f.ToString()));
                        minVal = int.Parse(Math.Round(res, 0).ToString());
                    }

                    //最大
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.读取设定波动加压Max);

                    ushort[] holding_register1 = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (holding_register1.Length > 0)
                    {
                        var f = double.Parse(holding_register1[0].ToString());

                        if (int.Parse(holding_register1[0].ToString()) > 10000)
                            f = -(65535 - int.Parse(holding_register1[0].ToString()));
                        else
                            f = int.Parse(holding_register1[0].ToString());

                        var res = Formula.GetValues(PublicEnum.DemarcateType.差压传感器, float.Parse(f.ToString()));
                        maxVal = int.Parse(Math.Round(res, 0).ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                IsSuccess = false;
                Logger.Error(ex);
            }
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
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.风机控制);
                    _MASTER.WriteSingleRegister(_SlaveID, _StartAddress, (ushort)(value));
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                Logger.Error(ex);
                return false;
            }
            return true;
        }



        /// <summary>
        /// 获取风机显示
        /// </summary>
        public double ReadFJSD(ref bool IsSuccess)
        {
            double res = 0;
            if (!IsTCPLink)
            {
                IsSuccess = false;
                return res;
            }
            try
            {
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.风机设定值);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = int.Parse(holding_register[0].ToString());
                    IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                IsSuccess = false;
                Logger.Error(ex);
            }
            return res;
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
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压阀);
                    _MASTER.WriteSingleCoil(_StartAddress, false);
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压阀);
                    _MASTER.WriteSingleCoil(_StartAddress, true);
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
                {
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压阀);
                    _MASTER.WriteSingleCoil(_StartAddress, false);
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压阀);
                    _MASTER.WriteSingleCoil(_StartAddress, true);
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
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
                IsTCPLink = false;
                Logger.Error(ex);
                // return false;
            }
            return true;
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
                lock (syncLock)
                {
                    if (type == "P")
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.P);
                    else if (type == "I")
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.I);
                    else if (type == "D")
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.D);
                    else if (type == "_P")
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand._P);
                    else if (type == "_I")
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand._I);
                    else if (type == "_D")
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand._D);
                    else if (type == "B_P")
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.B_P);
                    else if (type == "B_I")
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.B_I);
                    else if (type == "B_D")
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.B_D);

                    _MASTER.WriteSingleRegister(_SlaveID, _StartAddress, (ushort)value);
                    return true;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
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
                lock (syncLock)
                {
                    if (type == "P")
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.P);
                    else if (type == "I")
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.I);
                    else if (type == "D")
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.D);

                    else if (type == "_P")
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand._P);
                    else if (type == "_I")
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand._I);
                    else if (type == "_D")
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand._D);

                    else if (type == "B_P")
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.B_P);
                    else if (type == "B_I")
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.B_I);
                    else if (type == "B_D")
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.B_D);


                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = int.Parse(holding_register[0].ToString());
                    IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                IsSuccess = false;
                Logger.Error(ex);
            }
            return res;
        }


        #endregion

    }
}
