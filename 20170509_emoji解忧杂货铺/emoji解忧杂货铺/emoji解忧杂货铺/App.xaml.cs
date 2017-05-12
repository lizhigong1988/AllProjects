using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace emoji解忧杂货铺
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (!CommonInit(20170512, 1))
            {
                return;
            }
            base.OnStartup(e);
        }
        /// <summary>
        /// 初始化，检测测试版，添加异常处理
        /// </summary>
        /// <param name="testDate">0表示正式版</param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static bool CommonInit(int testDate = 0, int count = 0)
        {
            Application.Current.DispatcherUnhandledException += DispatcherUnhandledException;
            return true;
        }
        #region 处理异常情况
        private static void DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var msg = string.Format("应用程序发生错误，具体信息如下：\n{0}", GetInnerException(e.Exception).Message);
            //异常信息提示

            MessageBox.Show(msg, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static Exception GetInnerException(Exception e)
        {
            if (e == null) return null;
            if (e.InnerException != null)
                return GetInnerException(e.InnerException);
            else
                return e;
        }
        #endregion
    }
}
