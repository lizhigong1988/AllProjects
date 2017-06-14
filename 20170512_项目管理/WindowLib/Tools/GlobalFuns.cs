using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Data;
using System.Management;
using System.Windows.Threading;

namespace WindowLib.Tools
{
    public class GlobalFuns
    {

        public static MainWindow MainWind;

        public static void ShowDialogWind(Window handle,Window wind)
        {
            HwndSource winformWindow = (HwndSource.FromDependencyObject(handle) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(wind)
                {
                    Owner = winformWindow.Handle
                };
            }
            wind.ShowDialog();
        }

        public static string ADMIN_USER = "admin";

        public static string ADMIN_PASSWORD = "_admin";
        /// <summary>
        /// 当前登录用户名
        /// </summary>
        public static string LoginUser = "";
        /// <summary>
        /// 当前所操作的系统
        /// </summary>
        public static string LoginSysId = "";
        /// <summary>
        /// 当前操作的系统名称
        /// </summary>
        public static string LoginSysName = "";
        /// <summary>
        /// 当前使用的角色
        /// </summary>
        public static string LoginRole = "";
        /// <summary>
        /// 登录用户的所属系统信息
        /// </summary>
        public static DataTable DtLoginUserSysInfo;

        /// <summary>
        /// 页面刷新功能
        /// </summary>
        /// <returns></returns>
        //public delegate bool PageRefresh();
        //public static event PageRefresh PageRefreshFunc;

        /// <summary>
        /// 页面打开标志
        /// </summary>
        public static bool OpenFlag = false;


        /// <summary>
        /// 获取硬盘ID代码  
        /// </summary>
        /// <returns></returns>
        public static string GetHardDiskID()
        {
            try
            {
                string hdInfo = "";//硬盘序列号  
                ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
                hdInfo = disk.Properties["VolumeSerialNumber"].Value.ToString();
                disk = null;
                return hdInfo.Trim();
            }
            catch
            {
                return "";
            }
        }

        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(delegate(object f)
                {
                    ((DispatcherFrame)f).Continue = false;

                    return null;
                }
                    ), frame);
            Dispatcher.PushFrame(frame);
        } 
    }
}
