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
using System.Windows.Threading;
using System.Threading;

namespace 图片识别
{
    /// <summary>
    /// PageDistinguish.xaml 的交互逻辑
    /// </summary>
    public partial class PageDistinguish : System.Windows.Controls.UserControl
    {
        public PageDistinguish()
        {
            InitializeComponent();
        }

        private void lbBigImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectPath == "")
            {
                return;
            }
            string select = lbBigImages.SelectedItem.ToString();
            string file = selectPath + "\\" + select;
            image.Source = new BitmapImage(new Uri(file));
        }

        private string selectPath = "";

        private void btnSelectPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string[] programFiles = Directory.GetFiles(dialog.SelectedPath);
            selectPath = dialog.SelectedPath;
            List<string> fileNames = new List<string>();
            foreach (string file in programFiles)
            {
                if (!file.ToUpper().EndsWith(".PNG") &&
                   !file.ToUpper().EndsWith(".JPG"))
                {
                    continue;
                }
                string imageName = System.IO.Path.GetFileName(file);
                fileNames.Add(imageName);
            }
            lbBigImages.ItemsSource = fileNames;
        }

        private void btnDelImage_Click(object sender, RoutedEventArgs e)
        {
            if (lbBigImages.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("请选择要删除的图片");
                return;
            }
            lbBigImages.Items.Remove(lbBigImages.SelectedItem);
        }

        private void btnDistinguish_Click(object sender, RoutedEventArgs e)
        {
            if (running)
            {
                System.Windows.MessageBox.Show("正在识别，请稍等");
                return;
            }
            new Thread(DisTinguishThread).Start();
            tbAlert.Text = "识别中。。。";
            //DisTinguishThreadDebug();
        }

        bool running = false;
        int middle_X = 0;
        int middle_Y = 0;
        private void DisTinguishThread()
        {
            running = true;
            DataTable dtSource = DataBases.DataBaseManager.GetImages();
            if (dtSource == null)
            {
                System.Windows.MessageBox.Show("获取原图信息失败");
                return;
            }
            Dictionary<string, byte[]> dicSource = new Dictionary<string, byte[]>();
            List<int> listFirstByte = new List<int>();
            foreach (DataRow dr in dtSource.Rows)
            {
                dicSource.Add(dr["IMAGE_NAME"].ToString(), Convert.FromBase64String(dr["IMAGE_TAG"].ToString()));
                listFirstByte.Add(dicSource[dr["IMAGE_NAME"].ToString()][0] * 
                    dicSource[dr["IMAGE_NAME"].ToString()][1] * dicSource[dr["IMAGE_NAME"].ToString()][2]);
            }
            string message = "";
            List<Rect> listIgnore = new List<Rect>();
            middle_X = CommonDef.IMAGE_DEFAULT_SIZE / 2;
            middle_Y = CommonDef.IMAGE_DEFAULT_SIZE / 2;
            foreach (var item in lbBigImages.Items)
            {
                listIgnore.Clear();
                string filePath = item.ToString();
                message += filePath + ":";
                Bitmap bp = new Bitmap(selectPath + "\\" + filePath);
                int width = bp.Width;
                int height = bp.Height;
                for (int j = middle_Y; j < height - middle_Y; j++)
                {
                    if (!running)
                    {
                        tbAlert.Dispatcher.BeginInvoke(
                            new Action(() =>
                            {
                                tbResault.Text = message;
                                tbAlert.Text = "";
                            }));
                        return;
                    }
                    tbAlert.Dispatcher.BeginInvoke(
                        new Action(() =>
                        {
                            tbAlert.Text = string.Format("({0})识别中。{1}/{2}", filePath,
                                (j - middle_Y).ToString(), (height - CommonDef.IMAGE_DEFAULT_SIZE).ToString());
                        }));
                    for (int i = middle_X; i < width - middle_X; i++)
                    {//遍历大图的每一个像素点
                        foreach (Rect rect in listIgnore)
                        {//跳过已识别的区域
                            if (i >= rect.X && i <= rect.X + rect.Width &&
                                j >= rect.Y && j <= rect.Y + rect.Height)
                            {
                                i = (int)rect.X + (int)rect.Width + 1;
                                if (i >= width - CommonDef.IMAGE_DEFAULT_SIZE)
                                {
                                    i = 0;
                                    j += 1;
                                }
                                break;
                            }
                        }
                        if (j >= height - middle_Y)
                        {
                            break;
                        }
                        System.Drawing.Color c = bp.GetPixel(i, j);
                        foreach(int b in listFirstByte)
                        {
                            if (b == c.R * c.G * c.B)
                            {
                                if (ImageCompare(bp, i, j, dicSource, ref message, filePath))
                                {
                                    listIgnore.Add(new Rect()
                                    {
                                        X = i - middle_X,
                                        Y = j - middle_Y,
                                        Width = CommonDef.IMAGE_DEFAULT_SIZE,
                                        Height = CommonDef.IMAGE_DEFAULT_SIZE
                                    });
                                    tbResault.Dispatcher.BeginInvoke(
                                        new Action(() =>
                                        {
                                            tbResault.Text = message;
                                        }));
                                    break;
                                }
                            }
                        }

                    }
                }

                message += "\r\n";
                tbResault.Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        tbResault.Text = message;
                    }));
            }
            running = false;
            tbAlert.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    tbResault.Text = message;
                    tbAlert.Text = "";
                }));
        }

        private bool ImageCompare(Bitmap bp, int x, int y, Dictionary<string, byte[]> dicSource,
            ref string message, string filePath)
        {
            bool has = false;
            foreach (var dic in dicSource)
            {
                has = true;
                for (int i = 0; i < CommonDef.IMAGE_TAG_X_LENGTH; i++)
                {
                    if (!running)
                    {
                        return false;
                    }
                    System.Drawing.Color col = bp.GetPixel(x + i, y);
                    if (col.R != dic.Value[i * 3])
                    {
                        has = false;
                        break;
                    }
                    if (col.G != dic.Value[i * 3 + 1])
                    {
                        has = false;
                        break;
                    }
                    if (col.B != dic.Value[i * 3 + 2])
                    {
                        has = false;
                        break;
                    }
                }
                if (has)
                { //匹配成功
                    //校验结果
                    System.Drawing.Color col = bp.GetPixel(x + middle_X, y + middle_Y);
                    if (col.R == dic.Value[CommonDef.IMAGE_TAG_X_LENGTH * 3] &&
                        col.G == dic.Value[CommonDef.IMAGE_TAG_X_LENGTH * 3 + 1] &&
                        col.B == dic.Value[CommonDef.IMAGE_TAG_X_LENGTH * 3 + 2]
                        )
                    {
                        //测试用
                        //for (int k = 0; k < 5; k++)
                        //{
                        //    for (int l = 0; l < 5; l++)
                        //    {
                        //        bp.SetPixel(x + k, y + l, System.Drawing.Color.White);
                        //    }
                        //}
                        //bp.Save(filePath + "_" + x.ToString() + "_" + y.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                        message += dic.Key.Substring(0, dic.Key.LastIndexOf('-')) + ",";
                        return true;
                    }
                }
            }
            return false;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            running = false;
            tbAlert.Text = "停止中。。。";
        }
    }
}
