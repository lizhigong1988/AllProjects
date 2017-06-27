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
            if (File.Exists("Config"))
            {
                string[] configs = File.ReadAllText("Config").Split('\n');
                tbImageSize.Text = configs[0];
                tbImageTagSet.Text = configs[1];
            }
            else
            {
                tbImageTagSet.Text = "1,1;90,1;45,45;1,90;90,90";
            }
            Refresh();
        }

        private string FILE_PATH = "IMAGES";

        private void btnFilePath_Click(object sender, RoutedEventArgs e)
        {
            int imageWidth = 0;
            int imageHeight = 0;
            string[] sizes = tbImageSize.Text.Split('*');
            if (sizes.Length < 1)
            {
                System.Windows.MessageBox.Show("请输入正确格式的尺寸（宽*高）");
                return;
            }
            if (!int.TryParse(sizes[0], out imageWidth))
            {
                System.Windows.MessageBox.Show("请输入正确格式的尺寸（宽*高）");
                return;
            }
            if (!int.TryParse(sizes[1], out imageHeight))
            {
                System.Windows.MessageBox.Show("请输入正确格式的尺寸（宽*高）");
                return;
            }
            if(tbImageTagSet.Text == "")
            {
                System.Windows.MessageBox.Show("请输入特征点设置");
                return;
            }
            List<string> listPoint = new List<string>();
            listPoint.Add("0,0");
            string[] points = tbImageTagSet.Text.Split(';');
            int x = 0;
            int y = 0; 
            foreach (string point in points)
            {
                if (point == "")
                {
                    continue;
                }
                string[] xys = point.Split(',');
                if (xys.Length < 1)
                {
                    System.Windows.MessageBox.Show("请输入正确格式的特征点设置（x,y;x1,y1;x2,y2）");
                    return;
                }
                if (!int.TryParse(xys[0], out x))
                {
                    System.Windows.MessageBox.Show("请输入正确格式的特征点设置（x,y;x1,y1;x2,y2）");
                    return;
                }
                if (!int.TryParse(xys[1], out y))
                {
                    System.Windows.MessageBox.Show("请输入正确格式的特征点设置（x,y;x1,y1;x2,y2）");
                    return;
                }
                if (x > imageWidth)
                {
                    System.Windows.MessageBox.Show("特征点x不能超过图像宽度");
                    return;
                }
                if (y > imageHeight)
                {
                    System.Windows.MessageBox.Show("特征点y不能超过图像高度");
                    return;
                }
                if (x == 1 && y == 1)
                {
                }
                else
                {
                    listPoint.Add((x - 1).ToString() + "," + (y - 1).ToString());
                }
            }


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
                imageName = imageName.Replace(",", "-");
                imageName = imageName.Replace("\'", "");
                try
                {
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
                    if (bp.Width < imageWidth || bp.Height < imageHeight )
                    {
                        System.Windows.MessageBox.Show(string.Format("（{0}）图片不能小于设定像素", imageName));
                        return;
                    }
                    foreach (string pointInfo in listPoint)
                    {
                        string[] items = pointInfo.Split(',');
                        System.Drawing.Color col = bp.GetPixel(int.Parse(items[0]), int.Parse(items[1]));
                        imageTag.Add(col.R);
                        imageTag.Add(col.G);
                        imageTag.Add(col.B);
                    }
                    string strImgTag = Convert.ToBase64String(imageTag.ToArray());
                    object[] newRow = new object[] 
                    {
                        imageName, bmp, strImgTag
                    };
                    bool has = false;
                    foreach (DataRow dr in dtImportTable.Rows)
                    {
                        if (dr["IMAGE_NAME"].ToString() == imageName)
                        {
                            has = true;
                            dr["ImageSource"] = bmp;
                            dr["IMAGE_TAG"] = strImgTag;
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
            File.WriteAllText("Config", tbImageSize.Text + "\n" + tbImageTagSet.Text);
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

        private void btnClearImage_Click(object sender, RoutedEventArgs e)
        {
            if (!DataBaseManager.DelImages())
            {
                System.Windows.MessageBox.Show("删除失败");
                return;
            }
            dgCurList.DataContext = null;
        }

        private void btnClearAddImage_Click(object sender, RoutedEventArgs e)
        {
            dgImportList.DataContext = null;
        }
    }
}
