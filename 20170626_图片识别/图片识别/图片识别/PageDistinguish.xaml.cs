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

        int distingRate = 0;
        private void btnDistinguish_Click(object sender, RoutedEventArgs e)
        {
            if (running)
            {
                System.Windows.MessageBox.Show("正在识别，请稍等");
                return;
            }
            if (!int.TryParse(tbDistingRate.Text, out distingRate))
            {
                System.Windows.MessageBox.Show("请输入正确的精度");
                return;
            }
            distingRate = 100 - distingRate;
            if (distingRate < 0 || distingRate > 100)
            {
                System.Windows.MessageBox.Show("请输入正确的精度");
                return;
            }
            new Thread(DisTinguishThread).Start();
            tbAlert.Text = "识别中。。。";
            //DisTinguishThreadDebug();
        }

        struct SetPoint
        {
            public int x;
            public int y;
            public SetPoint(int X, int Y)
            {
                x = X;
                y = Y;
            }
        };

        bool running = false;
        List<SetPoint> listPoint = new List<SetPoint>();
        private void DisTinguishThread()
        {
            running = true;
            if (!File.Exists("Config"))
            {
                System.Windows.MessageBox.Show("请先导入小图");
                return;
            }
            string config = File.ReadAllText("Config");
            string[] configs = config.Split('\n');
            int imageWidth = int.Parse(configs[0].Split('*')[0]);
            int imageHeight = int.Parse(configs[0].Split('*')[1]);
            string[] points = configs[1].Split(';');
            listPoint.Clear();
            listPoint.Add(new SetPoint(0, 0));
            foreach (string point in points)
            {
                if (point == "1,1")
                {
                    continue;
                }
                listPoint.Add(new SetPoint(int.Parse(point.Split(',')[0]) - 1, int.Parse(point.Split(',')[1]) - 1));
            }

            DataTable dtSource = DataBases.DataBaseManager.GetImages();
            if (dtSource == null)
            {
                System.Windows.MessageBox.Show("获取原图信息失败");
                return;
            }
            Dictionary<string, byte[]> dicSource = new Dictionary<string, byte[]>();
            List<byte[]> listFirstByte = new List<byte[]>();
            foreach (DataRow dr in dtSource.Rows)
            {
                dicSource.Add(dr["IMAGE_NAME"].ToString(), Convert.FromBase64String(dr["IMAGE_TAG"].ToString()));
                listFirstByte.Add(new byte[]{ dicSource[dr["IMAGE_NAME"].ToString()][0] ,
                    dicSource[dr["IMAGE_NAME"].ToString()][1] , dicSource[dr["IMAGE_NAME"].ToString()][2]});
            }
            string message = "";
            List<Rect> listIgnore = new List<Rect>();
            Dictionary<string, string> dicNameResault = new Dictionary<string, string>();
            foreach (var item in lbBigImages.Items)
            {
                listIgnore.Clear();
                string filePath = item.ToString();
                string lable = filePath.Substring(0, filePath.LastIndexOf('.'));
                if (lable.Contains('.'))
                {
                    lable = lable.Substring(0, lable.IndexOf('.'));
                }
                if (!dicNameResault.ContainsKey(lable))
                {
                    dicNameResault.Add(lable, "");
                }
                message += filePath + ":";
                Bitmap bp = new Bitmap(selectPath + "\\" + filePath);
                int width = bp.Width;
                int height = bp.Height;
                for (int j = 0; j <= height - imageHeight; j++)
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
                                j.ToString(), (height - imageHeight).ToString());
                        }));
                    for (int i = 0; i <= width - imageWidth; i++)
                    {//遍历大图的每一个像素点
                        foreach (Rect rect in listIgnore)
                        {//跳过已识别的区域
                            if (i >= rect.X && i <= rect.X + rect.Width &&
                                j >= rect.Y && j <= rect.Y + rect.Height)
                            {
                                i = (int)rect.X + (int)rect.Width + 1;
                                if (i > width - imageWidth)
                                {
                                    i = 0;
                                    j += 1;
                                }
                                break;
                            }
                        }
                        if (j > height - imageHeight)
                        {
                            break;
                        }
                        System.Drawing.Color c = bp.GetPixel(i, j);
                        int listIndex = 0;
                        foreach(byte[] b in listFirstByte)
                        {
                            if (c.R - b[0] <= distingRate && b[0] - c.R <= distingRate &&
                                c.G - b[1] <= distingRate && b[1] - c.G <= distingRate &&
                                c.B - b[2] <= distingRate && b[2] - c.B <= distingRate)
                                //if (b[0] == c.R && b[1] == c.G && b[2] == c.B)
                            {//第一个像素点
                                var dic = dicSource.ElementAt(listIndex);
                                    //foreach (SetPoint point in listPoint)
                                bool has = true;
                                for (int index = 1; index < listPoint.Count; index++)
                                {
                                    SetPoint point = listPoint[index];
                                    System.Drawing.Color col = bp.GetPixel(i + point.x, j + point.y);
                                    if (col.R - dic.Value[index * 3] > distingRate ||
                                        dic.Value[index * 3] - col.R > distingRate)
                                    //if (col.R != dic.Value[index * 3])
                                    {
                                        has = false;
                                        break;
                                    }
                                    if (col.G - dic.Value[index * 3 + 1] > distingRate ||
                                        dic.Value[index * 3 + 1] - col.G > distingRate)
                                    //if (col.G != dic.Value[index * 3 + 1])
                                    {
                                        has = false;
                                        break;
                                    }
                                    if (col.B - dic.Value[index * 3 + 2] > distingRate ||
                                        dic.Value[index * 3 + 2] - col.B > distingRate)
                                    //if (col.B != dic.Value[index * 3 + 2])
                                    {
                                        has = false;
                                        break;
                                    }
                                }
                                if (has)
                                { //匹配成功
                                    message += dic.Key.Substring(0, dic.Key.LastIndexOf('-')) + ",";
                                    dicNameResault[lable] += dic.Key.Substring(0, dic.Key.LastIndexOf('-')) + ",";
                                    listIgnore.Add(new Rect()
                                    {
                                        X = i - imageWidth,
                                        Y = j,
                                        Width = imageWidth * 2,
                                        Height = imageHeight
                                    });
                                    tbResault.Dispatcher.BeginInvoke(
                                        new Action(() =>
                                        {
                                            tbResault.Text = message;
                                        }));
                                    break;
                                }
                            }
                            listIndex++;
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
                    message = "";
                    foreach (var dic in dicNameResault)
                    {
                        message += dic.Key + ":" + dic.Value + "\r\n";
                    }
                    tbResault.Text = message;
                    tbAlert.Text = "";
                }));
        }


        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            running = false;
            tbAlert.Text = "停止中。。。";
        }
    }
}
