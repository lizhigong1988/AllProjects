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
using System.Data;

namespace ExpandDemo.Pages.Prints
{
    /// <summary>
    /// OrderPrint.xaml 的交互逻辑
    /// </summary>
    public partial class AlloyStockPrint : UserControl
    {
        public int PageIndex = 1;

        public AlloyStockPrint()
        {
            InitializeComponent();
            for (int i = 0; i < gridMain.ColumnDefinitions.Count; i++)
            {
                for (int j = 0; j < gridMain.RowDefinitions.Count; j++)
                {
                    Border border = new Border();
                    border.BorderThickness = new Thickness(1, 1, 0, 0);
                    border.BorderBrush = Brushes.Black;
                    Grid.SetRow(border, j);
                    Grid.SetColumn(border, i);
                    gridMain.Children.Add(border);
                }
            }
            for (int i = 0; i < gridRemark.ColumnDefinitions.Count; i++)
            {
                Border border = new Border();
                border.BorderThickness = new Thickness(1, 1, 0, 0);
                border.BorderBrush = Brushes.Black;
                Grid.SetRow(border, 0);
                Grid.SetColumn(border, i);
                gridRemark.Children.Add(border);
            }
        }

        public List<TextBlock> AddNewTextBlockRow(int rowIndex, DataRow dr)
        {
            List<TextBlock> newRow = new List<TextBlock>() 
                    {
                        new TextBlock(),
                        new TextBlock(),
                        new TextBlock(),
                        new TextBlock(),
                        new TextBlock(),

                        new TextBlock(),
                        new TextBlock(),
                        new TextBlock(),
                        new TextBlock(),
                        new TextBlock(),

                        new TextBlock(),
                    };
            for (int i = 0; i < newRow.Count; i++)
            {
                newRow[i].HorizontalAlignment = HorizontalAlignment.Center;
                newRow[i].VerticalAlignment = VerticalAlignment.Center;
                newRow[i].TextWrapping = TextWrapping.Wrap;
                Grid.SetColumn(newRow[i], i);
                Grid.SetRow(newRow[i], rowIndex);
                gridMain.Children.Add(newRow[i]);
            }
            newRow[0].Text = dr["COUNT_INDEX"].ToString();
            newRow[1].Text = dr["TEXTURE"].ToString();
            newRow[2].Text = dr["COLOR"].ToString();
            newRow[3].Text = dr["NAME"].ToString();
            newRow[4].Text = dr["SIZE"].ToString();

            newRow[5].Text = dr["COUNT"].ToString();
            newRow[6].Text = dr["DOOR_COUNT"].ToString();
            newRow[7].Text = dr["SHUTTER"].ToString();
            newRow[8].Text = dr["SUSPEND"].ToString();
            newRow[9].Text = dr["GLASS_MODEL"].ToString();

            newRow[10].Text = dr["FORWORD"].ToString();
            return newRow;
        }

        internal static List<UserControl> SetPrintInfo(DataRow modDr, DataTable dataTable, string date, string remark)
        {
            List<UserControl> retList = new List<UserControl>();
            DataTable printGoods = dataTable.Copy();
            int pageCount = 1;

            DateTime orderDay;
            if (!DateTime.TryParseExact(date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out orderDay))
            {
                return retList;
            }
            while (true)
            {
                AlloyStockPrint print = new AlloyStockPrint();
                print.tbStockDate.Text = orderDay.ToLongDateString();
                print.tbStockUnit.Text = modDr["CLIENT_NAME"].ToString();
                print.tbTel.Text = modDr["CLIENT_TEL"].ToString();
                print.tbRemark.Text = remark;
                int woodDoorIndex = 1;

                List<DataRow> listLoadRows = new List<DataRow>();
                foreach (DataRow dr in printGoods.Rows)
                {
                    if (woodDoorIndex < 7)
                    {
                        print.AddNewTextBlockRow(woodDoorIndex, dr);
                        listLoadRows.Add(dr);
                        woodDoorIndex++;
                    }
                    else
                    {
                        break;
                    }
                }
                retList.Add(print);
                foreach (DataRow dr in listLoadRows)
                {
                    printGoods.Rows.Remove(dr);
                }
                if (printGoods.Rows.Count == 0)
                {
                    break;
                }
                pageCount++;
            }
            return retList;
        }
    }
}
