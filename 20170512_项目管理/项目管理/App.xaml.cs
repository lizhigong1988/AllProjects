using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace 项目管理
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Application.Current.DispatcherUnhandledException += DispatcherUnhandledException;
            base.OnStartup(e);
        }

        #region 处理异常情况
        new void DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var msg = string.Format("应用程序发生错误，具体信息如下：\n{0}", GetInnerException(e.Exception).Message);
            //异常信息提示

            MessageBox.Show(msg, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        Exception GetInnerException(Exception e)
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
