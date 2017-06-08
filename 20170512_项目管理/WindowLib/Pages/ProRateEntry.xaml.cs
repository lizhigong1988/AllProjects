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
using WindowLib.Tools;
using System.IO;
using WindowLib.Connect;

namespace WindowLib.Pages
{
    /// <summary>
    /// AddEngineerInfo.xaml 的交互逻辑
    /// </summary>
    public partial class ProRateEntry : UserControl
    {
        public ProRateEntry()
        {
            InitializeComponent();
            tbEntryDate.Text = DateTime.Now.ToString("yyyyMMdd");
            Refresh();
        }

        private void Refresh()
        {
            DataTable dt = CommunicationHelper.QueryProInfo("全部", "全部未完成", "",
                GlobalFuns.LoginSysId);
            if (dt == null)
            {
                MessageBox.Show("获取项目信息失败");
                return;
            }
            dgProInfo.DataContext = dt;

            cbSystem.ItemsSource = CommunicationHelper.GetAllSysDic();
            if (cbSystem.ItemsSource == null)
            {
                MessageBox.Show("获取系统信息失败");
                return;
            }
            cbSystem.SelectedValuePath = "Key";
            cbSystem.DisplayMemberPath = "Value";
            if (GlobalFuns.LoginSysId == "")
            {
                cbSystem.SelectedIndex = 0;
                cbCurSysOnly.IsChecked = false;
                cbCurSysOnly.Visibility = Visibility.Collapsed;
            }
            else
            {
                cbSystem.SelectedValue = GlobalFuns.LoginSysId;
                cbSystem.IsEnabled = false;
            }
            tbEntryDate.Text = DateTime.Now.ToString("yyyyMMdd");
        }
        DataTable selectProRateDt;
        private void dgProInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView drv = dgProInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                return;
            }
            DataTable newInfo = CommunicationHelper.GetProInfo(drv.Row["DEMAND_ID"].ToString());
            if (newInfo == null)
            {
                MessageBox.Show("获取项目信息失败");
                return;
            }
            DataRow selectDr = newInfo.Rows[0];
            selectProRateDt = CommunicationHelper.GetProRateInfo(drv.Row["DEMAND_ID"].ToString());
            if (selectProRateDt == null)
            {
                MessageBox.Show("获取进度信息失败");
                return;
            }
            foreach (DataRow dr in selectProRateDt.Rows)
            {
                dr["EXPLAIN"] = dr["EXPLAIN"].ToString().Replace("<br/>", "\r\n");
                dr["PROBLEM"] = dr["PROBLEM"].ToString().Replace("<br/>", "\r\n");
            }
            tbProName.Text = drv.Row["DEMAND_NAME"].ToString();
            //计算总体进度
            DataTable dtShowNew = selectProRateDt.Clone();
            //整理最新比例
            foreach (DataRow dr in selectProRateDt.Rows)
            {
                bool has = false;
                foreach (DataRow drNew in dtShowNew.Rows)
                {
                    if (dr["SYS_ID"].ToString() == drNew["SYS_ID"].ToString())
                    {
                        if (int.Parse(dr["DATE"].ToString()) > int.Parse(drNew["DATE"].ToString()))
                        {
                            drNew["DATE"] = dr["DATE"].ToString();
                            drNew["RATE"] = dr["RATE"].ToString();
                            drNew["EXPLAIN"] = dr["EXPLAIN"].ToString();
                            drNew["PROBLEM"] = dr["PROBLEM"].ToString();
                        }
                        has = true;
                        break;
                    }
                }
                if (!has)
                {
                    dtShowNew.Rows.Add(dr.ItemArray);
                }
            }
            DataTable dtProSysInfo = CommunicationHelper.GetProSystemInfo(drv.Row["DEMAND_ID"].ToString());
            if (dtProSysInfo == null)
            {
                MessageBox.Show("获取项目系统信息失败");
                return;
            }
            //总体进度 =（求和） 子单进度 * 子单预计工作量 / 总共工作量
            double totalChildrenRate = 0;
            double totalEstimate = 0;
            tbProgressNote.IsEnabled = false;
            foreach (DataRow dr in dtProSysInfo.Rows)
            {
                if (dr["IS_MAIN"].ToString() == "是")
                {
                    if (dr["SYS_ID"].ToString() == GlobalFuns.LoginSysId)
                    {
                        tbProgressNote.IsEnabled = true;
                    }
                }
                double estimate = 0;
                if (!double.TryParse(dr["ESTIMATE_DAYS"].ToString(), out estimate))
                {
                    continue;
                }
                if (estimate == 0)
                {
                    continue;
                }
                foreach (DataRow drSysRate in dtShowNew.Rows)
                {
                    if (drSysRate["SYS_ID"].ToString() == dr["SYS_ID"].ToString())
                    {
                        totalChildrenRate += double.Parse(drSysRate["RATE"].ToString()) * estimate / 100;
                        break;
                    }
                }
                totalEstimate += estimate;
            }

            if (totalEstimate != 0)
            {
                tbTotalRate.Text = (totalChildrenRate / totalEstimate * 100).ToString("N2") + "%";
            }
            else
            { 
                tbTotalRate.Text = "0%";
            }
            tbProgressNote.Text = selectDr["PRO_NOTE"].ToString();
            if (GlobalFuns.LoginSysId == "")
            {
                tbProgressNote.IsEnabled = true;
            }
            btnRefreshRate_Click(null, null);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drvPro = dgProInfo.SelectedItem as DataRowView;
            if (drvPro == null)
            {
                MessageBox.Show("请选择需求");
                return;
            }
            string sysId = cbSystem.SelectedValue.ToString();
            int rate = 0;
            if (!int.TryParse(tbRate.Text, out rate))
            {
                MessageBox.Show("请输入数字比例");
                return;
            }
            int date = 0;
            if (tbEntryDate.Text.Length != 8 || !int.TryParse(tbEntryDate.Text, out date))
            {
                MessageBox.Show("请输入正确的日期格式");
                return;
            }
            if (drvPro.Row["PRO_KIND"].ToString() != "新项目")
            {
                int lowDate = int.Parse(DateTime.Now.AddDays((int)(DateTime.Now.DayOfWeek) * -1).ToString("yyyyMMdd"));
                int curDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                if (date <= lowDate || date > curDate)
                {
                    MessageBox.Show("日期不正确，请填写本周日期");
                    return;
                }
            }
            DataTable dt = dgProRateInfo.DataContext as DataTable;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["SYS_ID"].ToString() == sysId)
                {
                    int rowDate = int.Parse(dr["DATE"].ToString());
                    if (date < rowDate)
                    {
                        MessageBox.Show("日期小于最新日期");
                        return;
                    }
                    int rowRate = int.Parse(dr["RATE"].ToString());
                    if (rowRate > rate)
                    {
                        MessageBox.Show("进度不能降");
                        return;
                    }
                }
            }
            if (!CommunicationHelper.EntryProRate(drvPro.Row["DEMAND_ID"].ToString(), sysId,
                tbEntryDate.Text, tbProgressNote.Text, tbRate.Text, 
                tbExplain.Text.Replace('\'', '\"').Replace("\r\n", "<br/>").Replace("\n", ""), 
                tbProblem.Text.Replace('\'', '\"').Replace("\r\n", "<br/>").Replace("\n", "")))
            {
                MessageBox.Show("保存进度失败！");
                return;
            }
            MessageBox.Show("保存成功！");
            dgProInfo_SelectionChanged(null, null);
        }

        private void btnRefreshRate_Click(object sender, RoutedEventArgs e)
        {
            if (selectProRateDt == null)
            {
                dgProRateInfo.DataContext = null;
                return;
            }
            bool curSys = (bool)cbCurSysOnly.IsChecked;
            bool newInfo = (bool)cbNewOnly.IsChecked;
            DataTable showDt = selectProRateDt.Copy();
            List<DataRow> deleteRows = new List<DataRow>();
            if (curSys)
            {
                foreach (DataRow dr in showDt.Rows)
                {
                    if (dr["SYS_ID"].ToString() != GlobalFuns.LoginSysId)
                    {
                        deleteRows.Add(dr);
                    }
                }
                foreach(DataRow dr in deleteRows)
                {
                    showDt.Rows.Remove(dr);
                }
            }
            if (newInfo)
            {
                DataTable dtShowNew = showDt.Clone();
                foreach (DataRow dr in showDt.Rows)
                {
                    bool has = false;
                    foreach (DataRow drNew in dtShowNew.Rows)
                    {
                        if (dr["SYS_ID"].ToString() == drNew["SYS_ID"].ToString())
                        {
                            if (int.Parse(dr["DATE"].ToString()) > int.Parse(drNew["DATE"].ToString()))
                            {
                                drNew["DATE"] = dr["DATE"].ToString();
                                drNew["RATE"] = dr["RATE"].ToString();
                                drNew["EXPLAIN"] = dr["EXPLAIN"].ToString();
                                drNew["PROBLEM"] = dr["PROBLEM"].ToString();
                            }
                            has = true;
                            break;
                        }
                    }
                    if (!has)
                    {
                        dtShowNew.Rows.Add(dr.ItemArray);
                    }
                }
                showDt = dtShowNew;
            }
            dgProRateInfo.DataContext = showDt;
        }

        private void dgProRateInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView drv = dgProRateInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                return;
            }
            tbRate.Text = drv.Row["RATE"].ToString();
            tbExplain.Text = drv.Row["EXPLAIN"].ToString();
            tbProblem.Text = drv.Row["PROBLEM"].ToString();
        }

    }
}
