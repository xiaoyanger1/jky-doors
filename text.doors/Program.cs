using text.doors.Detection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using text.doors.Common;

namespace text.doors
{
    static class Program
    {

        private static Young.Core.Logger.ILog Logger = Young.Core.Logger.LoggerManager.Current();
        /// <summary>   
        /// 应用程序的主入口点。
        /// </summary>  
        [STAThread]
        static void Main()
        {

            //处理未捕获的异常  
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //处理UI线程异常  
            Application.ThreadException += Application_ThreadException;
            //处理非UI线程异常  
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;


            //RegDLL.RegClass reg = new RegDLL.RegClass(System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetAssembly(typeof(Login)).Location).ToShortDateString());
            //if (reg.MiStart_Infos() && reg.MiEnd_Infos())
            //{
                Process instance = RunningInstance();
                if (instance == null)
                {
                    //Application.EnableVisualStyles();
                    //Application.SetCompatibleTextRenderingDefault(false);
                    Form Login = new Login();
                    Login.ShowDialog();//显示登陆窗体  
                    if (Login.DialogResult == DialogResult.OK)
                    {
                        Application.Run(new MainForm());//判断登陆成功时主进程显示主窗口
                    }
                    else return;
                }
                else
                {
                    // 已经有一个实例在运行
                    HandleRunningInstance(instance);
                }
            //}
            //else
            //{
            //    Application.Exit();
            //}
        }

        #region 全局错误
        //处理UI线程异常  
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception error = e.Exception as Exception;
            Logger.Error(error);
        }

        //处理非UI线程异常  
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception error = e.ExceptionObject as Exception;
            Logger.Error(error);
        }


        #endregion


        #region  确保程序只运行一个实例

        /// <summary>
        /// 遍历系统进程
        /// </summary>
        /// <returns></returns>
        private static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);

            //遍历与当前进程名称相同的进程列表 
            foreach (Process process in processes)
            {
                //如果实例已经存在则忽略当前进程 
                if (process.Id != current.Id)
                {
                    //保证要打开的进程同已经存在的进程来自同一文件路径
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        //返回已经存在的进程
                        return process;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 已经有了就把它激活，并将其窗口放置最前端
        /// </summary>
        /// <param name="instance"></param>
        private static void HandleRunningInstance(Process instance)
        {
            MessageBox.Show("该程序已经在运行！", "运行提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowWindowAsync(instance.MainWindowHandle, 1);  //调用api函数，正常显示窗口
            SetForegroundWindow(instance.MainWindowHandle); //将窗口放置最前端
        }

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(System.IntPtr hWnd, int cmdShow);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(System.IntPtr hWnd);

        #endregion

    }
}
