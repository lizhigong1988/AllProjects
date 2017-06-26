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
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Drawing;
using 图片识别.DataBases;

namespace 图片识别
{
    /// <summary>
    /// PageImport.xaml 的交互逻辑
    /// </summary>
    public partial class PageImport : System.Windows.Controls.UserControl
    {
        public PageImport()
        {
            InitializeComponent();
            Refresh();
        }

        private string FILE_PATH = "IMAGES";

        private void btnFilePath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string[] programFiles = Directory.GetFiles(dialog.SelectedPath);
            DataTable dtImportTable = dgImportList.DataContext as DataTable;
            if (dtImportTable == null)
            {
                dtImportTable = new DataTable();
                dtImportTable.Columns.Add("IMAGE_NAME");
                dtImportTable.Columns.Add("ImageSource", typeof(ImageSource));
                dtImportTable.Columns.Add("IMAGE_TAG");
                dtImportTable.Columns.Add("REMARK");
                dgImportList.DataContext = dtImportTable;
            }
            foreach (string file in programFiles)
            {
                if (!file.ToUpper().EndsWith(".PNG") &&
                   !file.ToUpper().EndsWith(".JPG"))
                {
                    continue;
                }
                string imageName = System.IO.Path.GetFileName(file);
                imageName = imageName.Substring(0, imageName.IndexOf('.'));
                imageName = imageName.Replace(",", "");
                imageName = imageName.Replace("\'", "");
                try
                {
                    //string destPath = System.Environment.CurrentDirectory + "\\" + FILE_PATH + "\\" + imageName + "_bak";
                    if (!Directory.Exists(FILE_PATH))
                    {
                        Directory.CreateDirectory(FILE_PATH);
                    }
                    string destPath = FILE_PATH + "/" + imageName + "_bak";
                    File.Copy(file, destPath, true);
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.StreamSource = new MemoryStream(File.ReadAllBytes(file));
                    bmp.EndInit();

                    Bitmap bp = new Bitmap(file);

                    List<byte> imageTag = new List<byte>();
                    if (bp.Width < CommonDef.IMAGE_DEFAULT_SIZE || bp.Height < CommonDef.IMAGE_DEFAULT_SIZE )
                    {
                        System.Windows.MessageBox.Show(string.Format("（{0}）图片太小，请选择大于60个像素的图片", imageName));
                        return;
                    }
                    string remark = "";
                    int mid_X = CommonDef.IMAGE_DEFAULT_SIZE / CommonDef.IMAGE_TAG_X_LENGTH;
                    int mid_Y = CommonDef.IMAGE_DEFAULT_SIZE / CommonDef.IMAGE_TAG_Y_LENGTH;
                    for (int i = 0; i < CommonDef.IMAGE_TAG_X_LENGTH; i++)
                    {
                        for (int j = 0; j < CommonDef.IMAGE_TAG_Y_LENGTH; j++)
                        {
                            System.Drawing.Color col = bp.GetPixel(i * mid_X, j * mid_Y);
                            /*
                             * 1 ~ 4 ~ 7 ~
                             * 2 ~ 5 ~ 8 ~
                             * 3 ~ 6 ~ 9 ~
                             * ~ ~ ~ ~ ~ ~
                             */
                            imageTag.Add(col.R);
                            imageTag.Add(col.G);
                            imageTag.Add(col.B);
                            remark += string.Format("[{0}_{1}_{2}])", col.R.ToString(), col.G.ToString(), col.B.ToString());
                        }
                    }
                    string strImgTag = Convert.ToBase64String(imageTag.ToArray());
                    object[] newRow = new object[] 
                    {
                        imageName, bmp, strImgTag, remark
                    };
                    bool has = false;
                    foreach (DataRow dr in dtImportTable.Rows)
                    {
                        if (dr["IMAGE_NAME"].ToString() == imageName)
                        {
                            has = true;
                            dr["ImageSource"] = bmp;
                            dr["IMAGE_TAG"] = strImgTag;
                            dr["REMARK"] = remark;
                            break;
                        }
                    }
                    if (!has)
                    {
                        dtImportTable.Rows.Add(newRow);
                    }
                }
                catch
                {
                    continue;
                }
            }

        }

        private void btnDelImport_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgImportList.SelectedItem as DataRowView;
            if (drv == null)
            {
                System.Windows.MessageBox.Show("选择所要删除的行");
                return;
            }
            drv.Row.Table.Rows.Remove(drv.Row);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DataTable dtImport = dgImportList.DataContext as DataTable;
            if (dtImport == null)
            {
                System.Windows.MessageBox.Show("请先导入图片");
                return;
            }
            if (!DataBaseManager.ImportImages(dtImport))
            {
                System.Windows.MessageBox.Show("导入失败");
                return;
            }
            string[] programFiles = Directory.GetFiles(FILE_PATH);
            foreach (string file in programFiles)
            {
                if (file.EndsWith("_bak"))
                {
                    string rename = file.Substring(0, file.Length - 4);
                    if(File.Exists(rename))
                    {
                        File.Delete(rename);
                    }
                    File.Move(file, rename);
                }
            }
            Refresh();
        }

        private void Refresh()
        {
            DataTable dt = DataBaseManager.GetImages();
            if (dt == null)
            {
                return;
            }
            dt.Columns.Add("ImageSource", typeof(ImageSource));
            dgCurList.DataContext = dt;
            foreach (DataRow dr in dt.Rows)
            {
                if (File.Exists(FILE_PATH + "/" + dr["IMAGE_NAME"].ToString()))
                {
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.StreamSource = new MemoryStream(File.ReadAllBytes(FILE_PATH + "/" + dr["IMAGE_NAME"].ToString()));
                    bmp.EndInit();
                    dr["ImageSource"] = bmp;
                }
            }
            dgImportList.DataContext = null;
        }

        private void btnDelImage_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgCurList.SelectedItem as DataRowView;
            if (drv == null)
            {
                System.Windows.MessageBox.Show("选择所要删除的行");
                return;
            }
            if (!DataBaseManager.DelImages(drv.Row["IMAGE_NAME"].ToString()))
            {
                System.Windows.MessageBox.Show("删除失败");
                return;
            }
            drv.Row.Table.Rows.Remove(drv.Row);
        }
    }
}
