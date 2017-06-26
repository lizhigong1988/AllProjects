using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace 图片识别
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (!CommonInit(20170626, 1))
            {
                return;
            }
            if (!DataBases.DataBaseManager.InitDataBases())
            {
                MessageBox.Show("数据库初始化失败");
                Environment.Exit(0);
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
            #region 不提供源码
            if (testDate != 0)
            {
                DateTime now = GetNowTime();
                DateTime testDay = DateTime.ParseExact(testDate.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                DateTime endDay = testDay.AddDays(count);
                if (now >= endDay || now < testDay)
                {
                    MessageBox.Show("试用版已过期");
                    DeleteThisExe();
                    return false;
                }
            }
            #endregion
            Application.Current.DispatcherUnhandledException += DispatcherUnhandledException;
            return true;
        }
        #region 不提供源码
        private static DateTime GetNowTime()
        {
            DateTime dt = DateTime.Now;
            System.Net.WebRequest wrt = null;
            System.Net.WebResponse wrp = null;
            try
            {
                wrt = System.Net.WebRequest.Create("http://www.hko.gov.hk/cgi-bin/gts/time5a.pr");
                wrp = wrt.GetResponse();

                string html = string.Empty;
                using (System.IO.Stream stream = wrp.GetResponseStream())
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8))
                    {
                        html = sr.ReadToEnd();
                    }
                }
                int seconds = int.Parse(html.Substring(2, 10));
                DateTime zeroDate = new DateTime(1970, 1, 1, 8, 0, 0);
                dt = zeroDate.AddSeconds(seconds);
            }
            catch
            {
            }
            finally
            {
                if (wrp != null)
                    wrp.Close();
                if (wrt != null)
                    wrt.Abort();
            }
            return dt;
        }

        private static void DeleteThisExe()
        {
            string fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "remove.bat");
            System.IO.StreamWriter bat = new System.IO.StreamWriter(fileName, false, System.Text.Encoding.Default);
            bat.WriteLine(string.Format("del \"{0}\" /q", System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
            bat.WriteLine(string.Format("del \"{0}\" /q", fileName));
            bat.Close();
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(fileName);
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            System.Diagnostics.Process.Start(info);
            Environment.Exit(0);
        }
        #endregion
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
