using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using text.doors.Default;
using Young.Core.Common;

namespace text.doors.Modbus.IService
{
    public class SendCommand : ISendCommand
    {

        private ushort _NumOfPoints = 1;
        private byte _SlaveID = 1;

        /// <summary>
        /// 设置高压标0
        /// </summary>
        public bool SendGYBD(bool logon = false)
        {
            try
            {
                ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.高压标0_交替型按钮);
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
                return true;
            }
            catch (Exception ex)
            {
                IsTCPLink = false;
                return false;
            }
        }

        /// <summary>
        /// 设置风速归零
        /// </summary>
        public void SendFSGL(ref bool IsSuccess, bool logon = false)
        {
            try
            {
                ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.风速标0_交替型按钮);
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
            catch (Exception ex)
            {
                IsTCPLink = false;
                IsSuccess = false;
            }
            IsSuccess = true;
        }

        /// <summary>
        /// 获取温度显示
        /// </summary>
        public double GetWDXS(ref bool IsSuccess)
        {
            double res = 0;
            lock (_MASTER)
            {
                try
                {
                    ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.温度显示);

                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = double.Parse((double.Parse(holding_register[0].ToString()) / 10).ToString());
                    res = Formula.GetValues(PublicEnum.DemarcateType.enum_温度传感器, float.Parse(res.ToString()));
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    IsTCPLink = false;
                    // Log.Error("ExportReport.Eexport", "message:获取温度显示" + ex.Message);
                }

                IsSuccess = true;
            }
            return res;
        }

        /// <summary>
        /// 获取大气压力显示
        /// </summary>
        public double GetDQYLXS(ref bool IsSuccess)
        {
            double res = 0; lock (_MASTER)
            {
                try
                {
                    ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.大气压力显示);

                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = double.Parse((double.Parse(holding_register[0].ToString()) / 10).ToString());
                    res = Formula.GetValues(PublicEnum.DemarcateType.enum_大气压力传感器, float.Parse(res.ToString()));
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    IsTCPLink = false;
                    //  Log.Error("ExportReport.Eexport", "message:获取大气压力" + ex.Message);
                }

                IsSuccess = true;
            }
            return res;
        }

        /// <summary>
        /// 获取风速显示
        /// </summary>
        public double GetFSXS(ref bool IsSuccess)
        {

            double res = 0;
            lock (_MASTER)
            {
                try
                {
                    ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.风速显示);

                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = double.Parse((double.Parse(holding_register[0].ToString()) / 100).ToString());
                    res = Formula.GetValues(PublicEnum.DemarcateType.enum_风速传感器, float.Parse(res.ToString()));
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    IsTCPLink = false;
                    Log.Error("ExportReport.Eexport", "message:获取风速显示" + ex.Message);

                    MessageBox.Show("获取风速异常", "获取风速异常",
                                           MessageBoxButtons.OKCancel,
                                           MessageBoxIcon.Information,
                                           MessageBoxDefaultButton.Button1,
                                          MessageBoxOptions.ServiceNotification
                                          );
                    System.Environment.Exit(0);

                }

                IsSuccess = true;
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
            lock (_MASTER)
            {
                ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.差压显示);

                ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                res = double.Parse(holding_register[0].ToString()) / 100;

                if (int.Parse(holding_register[0].ToString()) > 1100)
                    res = -(65535 - int.Parse(holding_register[0].ToString()));
                else
                    res = int.Parse(holding_register[0].ToString());

                res = Formula.GetValues(PublicEnum.DemarcateType.enum_差压传感器, float.Parse(res.ToString()));
                IsSuccess = true;
            }
            return int.Parse(Math.Round(res, 0).ToString());
        }

        /// <summary>
        /// 设置风机控制
        /// </summary>
        public void SendFJKZ(double value, ref bool IsSuccess)
        {
            try
            {
                ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.风机控制);
                _MASTER.WriteSingleRegister(_SlaveID, _StartAddress, (ushort)(value));
            }
            catch (Exception ex)
            {
                IsSuccess = false; IsTCPLink = false;
                Log.Error("ExportReport.Eexport", "message:设置风机控制" + ex.Message);
            }
            IsSuccess = true;
        }


        /// <summary>
        /// 设置正压阀
        /// </summary>
        /// <param name="IsSuccess"></param>
        public void SendZYF(ref bool IsSuccess)
        {
            lock (_MASTER)
            {
                try
                {
                    ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压阀);
                    _MASTER.WriteSingleCoil(_StartAddress, false);
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压阀);
                    _MASTER.WriteSingleCoil(_StartAddress, true);
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    IsTCPLink = false;
                    Log.Error("ExportReport.Eexport", "message:设置正压阀" + ex.Message);
                }
            }
            IsSuccess = true;
        }

        /// <summary>
        /// 设置负压阀
        /// </summary>
        /// <param name="IsSuccess"></param>
        public void SendFYF(ref bool IsSuccess)
        {
            lock (_MASTER)
            {
                try
                {
                    ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压阀);
                    _MASTER.WriteSingleCoil(_StartAddress, false);
                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压阀);
                    _MASTER.WriteSingleCoil(_StartAddress, true);
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    IsTCPLink = false;
                    Log.Error("ExportReport.Eexport", "message:设置负压阀" + ex.Message);
                }
            }
            IsSuccess = true;
        }


        /// <summary>
        /// 读取正负压阀
        /// </summary>
        public void GetZFYF(ref bool IsSuccess, ref bool z, ref bool f)
        {
            lock (_MASTER)
            {
                try
                {
                    ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压阀);
                    bool[] readCoils_z = _MASTER.ReadCoils(_StartAddress, _NumOfPoints);
                    z = bool.Parse(readCoils_z[0].ToString());

                    _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压阀);
                    bool[] readCoils_f = _MASTER.ReadCoils(_StartAddress, _NumOfPoints);
                    f = bool.Parse(readCoils_f[0].ToString());
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    IsTCPLink = false;
                    Log.Error("ExportReport.Eexport", "message:读取正负压阀" + ex.Message);
                }
            }
            IsSuccess = true;
        }



        /*图标页面*/

        /// <summary>
        /// 设置正压预备
        /// </summary>
        /// <param name="IsSuccess"></param>
        public void SetZYYB(ref bool IsSuccess)
        {
            try
            {
                SendZYF(ref IsSuccess);
                if (!IsSuccess)
                {
                    return;
                }

                ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压预备);
                _MASTER.WriteSingleCoil(_StartAddress, false);
                _MASTER.WriteSingleCoil(_StartAddress, true);

            }
            catch (Exception ex)
            {
                IsSuccess = false;
                IsTCPLink = false;
                Log.Error("ExportReport.Eexport", "message:设置正压预备" + ex.Message);
            }
            IsSuccess = true;
        }

        /// <summary>
        /// 读取正压预备结束
        /// </summary>
        /// <param name="IsSuccess"></param>
        public int GetZYYBJS(ref bool IsSuccess)
        {
            int res = 0;
            try
            {
                ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压预备结束);
                ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                res = int.Parse(holding_register[0].ToString());
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                IsTCPLink = false;
                Log.Error("ExportReport.Eexport", "message:读取正压预备结束" + ex.Message);
            }
            IsSuccess = true;
            return res;
        }

        /// <summary>
        /// 发送正压开始
        /// </summary>
        public void SendZYKS(ref bool IsSuccess)
        {
            try
            {
                SendZYF(ref IsSuccess);
                if (!IsSuccess)
                {
                    return;
                }
                ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压开始);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, false);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, true);
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                IsTCPLink = false;
                Log.Error("ExportReport.Eexport", "message:发送正压开始" + ex.Message);
            }
            IsSuccess = true;
        }


        /// <summary>
        /// 读取正压开始结束
        /// </summary>
        /// <param name="IsSuccess"></param>
        public double GetZYKSJS(ref bool IsSuccess)
        {
            double res = 0;
            try
            {
                ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压开始结束);
                ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                res = double.Parse(holding_register[0].ToString());
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                IsTCPLink = false;
                Log.Error("ExportReport.Eexport", "message:读取正压开始结束" + ex.Message);
            }
            IsSuccess = true;
            return res;
        }

        /// <summary>
        /// 发送负压预备
        /// </summary>
        /// <param name="IsSuccess"></param>
        public void SendFYYB(ref bool IsSuccess)
        {
            try
            {
                SendFYF(ref IsSuccess);
                if (!IsSuccess)
                {
                    return;
                }

                ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压预备);
                _MASTER.WriteSingleCoil(_StartAddress, false);
                _MASTER.WriteSingleCoil(_StartAddress, true);

            }
            catch (Exception ex)
            {
                IsSuccess = false;
                IsTCPLink = false;
                Log.Error("ExportReport.Eexport", "message:发送负压预备" + ex.Message);
            }
            IsSuccess = true;
        }
        /// <summary>
        /// 发送负压开始
        /// </summary>
        public void SendFYKS(ref bool IsSuccess)
        {
            try
            {
                SendFYF(ref IsSuccess);
                if (!IsSuccess)
                {
                    return;
                }
                ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压开始);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, false);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, true);
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                IsTCPLink = false;
                Log.Error("ExportReport.Eexport", "message:发送负压开始" + ex.Message);
            }
            IsSuccess = true;
        }


        /// <summary>
        /// 读取正压预备结束
        /// </summary>
        /// <param name="IsSuccess"></param>
        public int GetFYYBJS(ref bool IsSuccess)
        {
            int res = 0;
            lock (_MASTER)
            {
                try
                {
                    ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压预备结束);
                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = int.Parse(holding_register[0].ToString());
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    IsTCPLink = false;
                    Log.Error("ExportReport.Eexport", "message:读取正压预备结束" + ex.Message);
                }
                IsSuccess = true;
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
            try
            {
                ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压开始结束);
                ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                res = double.Parse(holding_register[0].ToString());
                Log.Info("", res.ToString());
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                IsTCPLink = false;
                Log.Error("ExportReport.Eexport", "message:读取正压开始结束" + ex.Message);
            }
            IsSuccess = true;
            return res;
        }



        /// <summary>
        /// 获取正压预备时，设定压力值
        /// </summary>
        public double GetZYYBYLZ(ref bool IsSuccess, string type)
        {
            double res = 0;
            lock (_MASTER)
            {
                try
                {
                    ushort _StartAddress = 0;
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

                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    IsTCPLink = false;
                    Log.Error("ExportReport.Eexport", "message:获取正压预备时，设定压力值" + ex.Message);
                }

                IsSuccess = true;
            }
            return res;
        }



        /// <summary>
        /// 获取正压100Pa是否开始计时
        /// </summary>
        public bool Get_Z_S100TimeStart(ref bool IsSuccess)
        {
            bool res = false;
            lock (_MASTER)
            {
                try
                {
                    ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压100TimeStart);
                    ushort[] t = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    //bool[] readCoils_z = _MASTER.ReadCoils(_StartAddress, _NumOfPoints);
                    //res = bool.Parse(readCoils_z[0].ToString());

                    if (Convert.ToInt32(t[0]) > 20)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    IsTCPLink = false;
                    Log.Error("ExportReport.Eexport", "message:获取正压100Pa是否开始计时" + ex.Message);
                }
            }
            return res;
        }

        /// <summary>
        /// 获取正压150Pa是否开始计时
        /// </summary>
        public bool Get_Z_S150PaTimeStart(ref bool IsSuccess)
        {
            bool res = false;
            lock (_MASTER)
            {
                try
                {
                    ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压150TimeStart);

                    ushort[] t = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (Convert.ToInt32(t[0]) > 20)
                        return true;
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    IsTCPLink = false;
                    Log.Error("ExportReport.Eexport", "message:获取正压150Pa是否开始计时" + ex.Message);
                }
            }
            return res;
        }

        /// <summary>
        /// 获取降压100Pa是否开始计时
        /// </summary>
        public bool Get_Z_J100PaTimeStart(ref bool IsSuccess)
        {
            bool res = false;
            lock (_MASTER)
            {
                try
                {
                    ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.正压_100TimeStart);
                    //bool[] readCoils_z = _MASTER.ReadCoils(_StartAddress, _NumOfPoints);
                    //res = bool.Parse(readCoils_z[0].ToString());
                    ushort[] t = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (Convert.ToInt32(t[0]) > 20)
                        return true;
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    IsTCPLink = false;
                    Log.Error("ExportReport.Eexport", "message:获取降压100Pa是否开始计时" + ex.Message);
                }
            }
            return res;
        }

        /// <summary>
        /// 获取负压100Pa是否开始计时
        /// </summary>
        public bool Get_F_S100PaTimeStart(ref bool IsSuccess)
        {
            bool res = false;
            lock (_MASTER)
            {
                try
                {
                    ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压100TimeStart);
                    ushort[] t = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (Convert.ToInt32(t[0]) > 20)
                        return true;
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    IsTCPLink = false;
                    Log.Error("ExportReport.Eexport", "message:获取负压100Pa是否开始计时" + ex.Message);
                }
            }
            return res;
        }

        /// <summary>
        /// 获取负压150Pa是否开始计时
        /// </summary>
        public bool Get_F_S150PaTimeStart(ref bool IsSuccess)
        {
            bool res = false;
            lock (_MASTER)
            {
                try
                {
                    ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压150TimeStart);
                    ushort[] t = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (Convert.ToInt32(t[0]) > 20)
                        return true;
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    IsTCPLink = false;
                    IsSuccess = false;
                    Log.Error("ExportReport.Eexport", "message:获取负压150Pa是否开始计时" + ex.Message);

                }
            }
            return res;
        }

        /// <summary>
        /// 获取负压降100Pa是否开始计时
        /// </summary>
        public bool Get_F_J100PaTimeStart(ref bool IsSuccess)
        {
            bool res = false;
            lock (_MASTER)
            {
                try
                {
                    ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.负压_100TimeStart);
                    ushort[] t = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    if (Convert.ToInt32(t[0]) > 20)
                        return true;
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    IsTCPLink = false;
                    Log.Error("ExportReport.Eexport", "message:获取负压降100Pa是否开始计时" + ex.Message);
                }
            }
            return res;
        }
        /*水密*/

        /// <summary>
        /// 设置水密预备
        /// </summary>
        /// <param name="IsSuccess"></param>
        public void SetSMYB(ref bool IsSuccess)
        {
            try
            {
                SendZYF(ref IsSuccess);
                if (!IsSuccess)
                {
                    return;
                }

                ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.水密性预备加压);
                _MASTER.WriteSingleCoil(_StartAddress, false);
                _MASTER.WriteSingleCoil(_StartAddress, true);

            }
            catch (Exception ex)
            {
                IsSuccess = false;
                IsTCPLink = false;
                Log.Error("ExportReport.Eexport", "message:水密性预备加压" + ex.Message);

            }
            IsSuccess = true;
        }


        /// <summary>
        /// 读取水密预备结束
        /// </summary>
        /// <param name="IsSuccess"></param>
        public int GetSMYBJS(ref bool IsSuccess)
        {
            int res = 0;
            try
            {
                ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.水密预备结束);
                ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                res = int.Parse(holding_register[0].ToString());
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                IsTCPLink = false;
                Log.Error("ExportReport.Eexport", "message:水密预备结束" + ex.Message);
            }
            IsSuccess = true;
            return res;
        }


        /// <summary>
        /// 读取水密预备设定压力
        /// </summary>
        /// <param name="IsSuccess"></param>
        public int GetSMYBSDYL(ref bool IsSuccess, string type)
        {
            int res = 0;
            try
            {
                ushort _StartAddress = 0;
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
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                IsTCPLink = false;
                Log.Error("ExportReport.Eexport", "message:读取水密预备设定压力" + ex.Message);
            }
            IsSuccess = true;
            return res;
        }

        /// <summary>
        /// 发送水密开始
        /// </summary>
        public void SendSMXKS(ref bool IsSuccess)
        {
            try
            {
                SendZYF(ref IsSuccess);
                if (!IsSuccess)
                {
                    return;
                }

                ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.水密性开始);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, false);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, true);
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                IsTCPLink = false;
                Log.Error("ExportReport.Eexport", "message:水密性开始" + ex.Message);
            }
            IsSuccess = true;
        }

        /// <summary>
        /// 发送水密性下一级
        /// </summary>
        public void SendSMXXYJ(ref bool IsSuccess)
        {
            try
            {
                ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.下一级);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, false);
                _MASTER.WriteSingleCoil(_SlaveID, _StartAddress, true);
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                IsTCPLink = false;
                Log.Error("ExportReport.Eexport", "message:下一级" + ex.Message);
            }
            IsSuccess = true;
        }



        /// <summary>
        /// 设置水密依次加压
        /// </summary>
        public void SendSMYCJY(double value, ref bool IsSuccess)
        {
            try
            {
                SendZYF(ref IsSuccess);

                ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.以此加压数值);
                _MASTER.WriteSingleRegister(_SlaveID, _StartAddress, (ushort)(value));

                _StartAddress = BFMCommand.GetCommandDict(BFMCommand.以此加压);
                _MASTER.WriteSingleCoil(_StartAddress, false);
                _MASTER.WriteSingleCoil(_StartAddress, true);

            }
            catch (Exception ex)
            {
                IsSuccess = false;
                IsTCPLink = false;
                Log.Error("ExportReport.Eexport", "message:设置水密依次加压" + ex.Message);
            }
            IsSuccess = true;
        }


        /// <summary>
        /// 急停
        /// </summary>
        public void Stop(ref bool IsSuccess)
        {
            try
            {
                ushort _StartAddress = BFMCommand.GetCommandDict(BFMCommand.急停);
                _MASTER.WriteSingleCoil(_StartAddress, false);
                _MASTER.WriteSingleCoil(_StartAddress, true);
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                IsTCPLink = false;
                Log.Error("ExportReport.Eexport", "message:急停" + ex.Message);
                MessageBox.Show("急停异常", "急停",
                                         MessageBoxButtons.OKCancel,
                                         MessageBoxIcon.Information,
                                         MessageBoxDefaultButton.Button1,
                                        MessageBoxOptions.ServiceNotification
                                        );
                System.Environment.Exit(0);
            }
            IsSuccess = true;
        }

        /// <summary>
        /// 写入PID
        /// </summary>
        /// <param name="IsSuccess"></param>
        public void SendPid(ref bool IsSuccess, string type, double value)
        {
            try
            {
                if (IsTCPLink == false)
                {
                    TcpOpen();
                }
                else
                {
                    ushort _StartAddress = 0;
                    if (type == "P")
                    {
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.P);
                    }
                    else if (type == "I")
                    {
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.I);
                    }
                    else if (type == "D")
                    {
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.D);
                    }

                    _MASTER.WriteSingleRegister(_SlaveID, _StartAddress, (ushort)value);
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                IsTCPLink = false;
                Log.Error("ExportReport.Eexport", "message:设置Pid" + ex.Message + "\r\nsource:" + ex.Source + "\r\nStackTrace:" + ex.StackTrace);
            }

        }

        /// <summary>
        /// 读取PID
        /// </summary>
        /// <param name="IsSuccess"></param>
        public int GetPID(ref bool IsSuccess, string type)
        {
            int res = 0;
            try
            {
                if (IsTCPLink == false)
                {
                    TcpOpen();
                }
                else
                {
                    ushort _StartAddress = 0;

                    if (type == "P")
                    {
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.P);
                    }
                    else if (type == "I")
                    {
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.I);
                    }
                    else if (type == "D")
                    {
                        _StartAddress = BFMCommand.GetCommandDict(BFMCommand.D);
                    }

                    ushort[] holding_register = _MASTER.ReadHoldingRegisters(_SlaveID, _StartAddress, _NumOfPoints);
                    res = int.Parse(holding_register[0].ToString());
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                IsTCPLink = false;
                Log.Error("ExportReport.Eexport", "message:读取Pid" + ex.Message + "\r\nsource:" + ex.Source + "\r\nStackTrace:" + ex.StackTrace);
            }
            IsSuccess = true;
            return res;
        }
    }
}
