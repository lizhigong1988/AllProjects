using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Windows.Interop;
using ClientCommunication;

namespace ClientControls.Dialogs
{
    /// <summary>
    /// SelectDataBoxDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SelectDataBoxDialog : Window
    {
        public DataRow SelectDataRow = null;

        public SelectDataBoxDialog()
        {
            InitializeComponent();
            dbTable.SetExportInUse(false);
            dbTable.DataBoxDoubleClicks += dbTableDoubleClick;
        }

        private void dbTableDoubleClick(DataRow dr)
        {
            SelectDataRow = dr;
            this.Close();
        }

        public static DataRow PopSelectTable(string header, Dictionary<string, string> dicColumns, DataTable bindData, bool mutiSelect = false)
        {
            SelectDataBoxDialog dialog = new SelectDataBoxDialog();
            HwndSource winformWindow = (HwndSource.FromDependencyObject(LoadingWorker.MainWind) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(dialog)
                {
                    Owner = winformWindow.Handle
                };
            }
            dialog.gbMainGroup.Header = header;
            dialog.dbTable.InitDataBox(dicColumns, bindData, mutiSelect);
            dialog.ShowDialog();
            return dialog.SelectDataRow;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bder_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
