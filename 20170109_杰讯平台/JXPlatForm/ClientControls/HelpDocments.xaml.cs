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
using System.Windows.Navigation;
using System.IO;
using System.IO.Packaging;
using System.Windows.Xps.Packaging;
using System.Runtime.InteropServices;

using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Word;

namespace ClientControls
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HelpDocments 
    {
        private string HELP_DOC_PATH = "HelpDoc";

        public HelpDocments()
        {
            InitializeComponent();

            DirectoryInfo TheFolder = new DirectoryInfo(HELP_DOC_PATH);
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                if (NextFile.Name.ToUpper().EndsWith("DOC") || NextFile.Name.ToUpper().EndsWith("DOCX"))
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = NextFile.Name;
                    item.Tag = NextFile.FullName;
                    this.lbnav.Items.Add(item);
                }
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            lbnav.SelectedIndex = 0;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            ListBoxItem lbi = lb.SelectedItem as ListBoxItem;
            string filePath = lbi.Tag.ToString();
            docViewer.Document = ConvertWordToXPS(filePath).GetFixedDocumentSequence();
            if (null != docViewer.Document)
            {
                docViewer.FitToWidth();
            }
        }

        private XpsDocument ConvertWordToXPS(string wordDocName)
        {
            FileInfo fi=new FileInfo(wordDocName);
            XpsDocument result = null;
            string xpsDocName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache), fi.Name);
            xpsDocName = xpsDocName.Replace(".docx", ".xps").Replace(".doc", ".xps");
            Microsoft.Office.Interop.Word.Application wordApplication = new Microsoft.Office.Interop.Word.Application();
            try
            {
                if (!File.Exists(xpsDocName))
                {
                    wordApplication.Documents.Add(wordDocName);
                    Document doc = wordApplication.ActiveDocument;
                    doc.ExportAsFixedFormat(xpsDocName, WdExportFormat.wdExportFormatXPS, false, WdExportOptimizeFor.wdExportOptimizeForPrint, WdExportRange.wdExportAllDocument, 0, 0, WdExportItem.wdExportDocumentContent, true, true, WdExportCreateBookmarks.wdExportCreateHeadingBookmarks, true, true, false, Type.Missing);
                    result = new XpsDocument(xpsDocName, System.IO.FileAccess.Read);
                }

                if (File.Exists(xpsDocName))
                {
                    result = new XpsDocument(xpsDocName, FileAccess.Read);                    
                }

            }
            catch 
            {
                MessageBox.Show("读取【" + fi.Name + "】文件失败");
            }

            return result;
        }
    }
}
