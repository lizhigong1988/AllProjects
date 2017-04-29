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
    public partial class OrderPrint : UserControl
    {
        public int PageIndex = 1;

        public OrderPrint()
        {
            InitializeComponent();
        }

        public void DrawGrid()
        {
            if (gridWoodDoor.ColumnDefinitions.Count == 0)
            {
                gridWoodDoor.Visibility = Visibility.Collapsed;
            }
            else
            {
                for (int i = 0; i < gridWoodDoor.ColumnDefinitions.Count; i++)
                {
                    for (int j = 0; j < gridWoodDoor.RowDefinitions.Count; j++)
                    {
                        Border border = new Border();
                        border.BorderThickness = new Thickness(1, 1, 0, 0);
                        border.BorderBrush = Brushes.Black;
                        Grid.SetRow(border, j);
                        Grid.SetColumn(border, i);
                        gridWoodDoor.Children.Add(border);
                    }
                }
            }

            if (gridAlloyDoor.ColumnDefinitions.Count == 0)
            {
                gridAlloyDoor.Visibility = Visibility.Collapsed;
            }
            else
            {
                for (int i = 0; i < gridAlloyDoor.ColumnDefinitions.Count; i++)
                {
                    for (int j = 0; j < gridAlloyDoor.RowDefinitions.Count; j++)
                    {
                        Border border = new Border();
                        border.BorderThickness = new Thickness(1, 1, 0, 0);
                        border.BorderBrush = Brushes.Black;
                        Grid.SetRow(border, j);
                        Grid.SetColumn(border, i);
                        gridAlloyDoor.Children.Add(border);
                    }
                }
            }
            if (gridWoodenWind.ColumnDefinitions.Count == 0)
            {
                gridWoodenWind.Visibility = Visibility.Collapsed;
            }
            else
            {
                for (int i = 0; i < gridWoodenWind.ColumnDefinitions.Count; i++)
                {
                    for (int j = 0; j < gridWoodenWind.RowDefinitions.Count; j++)
                    {
                        Border border = new Border();
                        border.BorderThickness = new Thickness(1, 1, 0, 0);
                        border.BorderBrush = Brushes.Black;
                        Grid.SetRow(border, j);
                        Grid.SetColumn(border, i);
                        gridWoodenWind.Children.Add(border);
                    }
                }
            }
        }

        public static List<UserControl> SetPrintInfo(string name, string tel, string date, string addr, string amt,
            string deposite, DataTable goods)
        {
            List<UserControl> retList = new List<UserControl>();
            DataTable printGoods = goods.Copy();
            int pageCount = 1;
            while (true)
            {
                OrderPrint print = new OrderPrint();
                print.tbClientName.Text = name;
                print.tbClientTel.Text = tel;
                print.tbOrderDate.Text = date;
                print.tbClientAddr.Text = addr;
                print.tbTotalAmt.Text = amt;
                print.tbDepositAmt.Text = deposite;
                int woodDoorIndex = 1;
                int alloyDoorIndex = 1;
                int woodWindIndex = 1;

                List<DataRow> listLoadRows = new List<DataRow>();
                foreach (DataRow dr in printGoods.Rows)
                {
                    switch (dr["KIND"].ToString())
                    {
                        case "木门":
                            print.AddNewTextBlockRow(woodDoorIndex, dr, dr["KIND"].ToString());
                            listLoadRows.Add(dr);
                            woodDoorIndex++;
                            break;
                        case "合金门":
                            print.AddNewTextBlockRow(alloyDoorIndex, dr, dr["KIND"].ToString());
                            listLoadRows.Add(dr);
                            alloyDoorIndex++;
                            break;
                        case "垭口窗套":
                            print.AddNewTextBlockRow(woodWindIndex, dr, dr["KIND"].ToString());
                            listLoadRows.Add(dr);
                            woodWindIndex++;
                            break;
                        default:
                            break;
                    }
                    if (woodDoorIndex + alloyDoorIndex + woodWindIndex - 3 == 20)
                    {
                        break;
                    }
                }
                print.DrawGrid();
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
            for (int i = 0; i < retList.Count; i++ )
            {
                (retList[i] as OrderPrint).tbPageCounts.Text = string.Format("第({0})页 共({1})页", 
                    (i+1).ToString(), pageCount.ToString());
            }
            return retList;
        }

        public List<TextBlock> AddNewTextBlockRow(int rowIndex, DataRow dr, string kind)
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
                    };
            for (int i = 0; i < newRow.Count; i++)
            {
                newRow[i].HorizontalAlignment = HorizontalAlignment.Center;
                newRow[i].VerticalAlignment = VerticalAlignment.Center;
                newRow[i].TextWrapping = TextWrapping.Wrap;
                Grid.SetColumn(newRow[i], i);
                Grid.SetRow(newRow[i], rowIndex);
                switch (kind)
                {
                    case "木门":
                        gridWoodDoor.RowDefinitions.Add(new RowDefinition() {Height = new GridLength(32) });
                        gridWoodDoor.Children.Add(newRow[i]);
                        break;
                    case "合金门":
                        gridAlloyDoor.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(32) });
                        gridAlloyDoor.Children.Add(newRow[i]);
                        break;
                    case "垭口窗套":
                        gridWoodenWind.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(32) });
                        gridWoodenWind.Children.Add(newRow[i]);
                        break;
                    default:
                        break;
                }
            }
            newRow[0].Text = dr["NAME"].ToString();
            newRow[1].Text = dr["MODEL"].ToString();
            newRow[2].Text = dr["SIZE"].ToString();
            newRow[3].Text = dr["COLOR"].ToString();
            newRow[4].Text = dr["STYLE_KIND"].ToString();
            newRow[5].Text = dr["COUNT"].ToString();
            newRow[6].Text = dr["PRICE"].ToString();
            newRow[7].Text = dr["TOTAL_PRICE"].ToString();
            newRow[8].Text = dr["REMARK"].ToString();
            return newRow;
        }
    }
}
