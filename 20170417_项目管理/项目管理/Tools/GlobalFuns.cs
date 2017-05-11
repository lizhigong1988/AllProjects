using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace 项目管理.Tools
{
    public class GlobalFuns
    {

        public static Window MainWind;

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
    }
}
