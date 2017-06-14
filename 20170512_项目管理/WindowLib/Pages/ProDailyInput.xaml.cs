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
using WindowLib.Tools;
using System.Data;
using WindowLib.Connect;
using WindowLib.PopWindows;

namespace WindowLib.Pages
{
    /// <summary>
    /// ProDailyInput.xaml 的交互逻辑
    /// </summary>
    public partial class ProDailyInput : UserControl
    {
        Dictionary<string, string> dicWorkType = new Dictionary<string, string>() 
        {
            {"1", "开发"},
            {"2", "会议"},
            {"3", "文档整理"},
            {"4", "协助运维"},
            {"Z", "其他"},
        };

        public ProDailyInput()
        {
            InitializeComponent();
            tbStartDate.Text = DateTime.Now.AddMonths(-1).ToString("yyyyMMdd");
            tbEndDate.Text = DateTime.Now.ToString("yyyyMMdd");

            switch (GlobalFuns.LoginRole)
            {
                case "开发人员":
                    tbUserName.Text = GlobalFuns.LoginUser;
                    tbUserName.IsEnabled = false;
                    tbDate.IsEnabled = false;//日期不能改且为服务端查询日期
                    tbSignInTime.IsEnabled = false;
                    tbSignOutTime.IsEnabled = false;
                    btnQuery_Click(null, null);
                    if (!RefreshCurDayInfo())
                    {
                        return;
                    }
                    break;
                default:
                    tbUserName.IsEnabled = true;
                    tbDate.IsEnabled = true;
                    tbSignInTime.IsEnabled = true;
                    tbSignOutTime.IsEnabled = true;
                    //tbDate.Text = DateTime.Now.ToString("yyyyMMdd");
                    break;
            
            }
            cbWorkType.ItemsSource = dicWorkType;
            cbWorkType.DisplayMemberPath = "Value";
            cbWorkType.SelectedValuePath = "Key";

            Dictionary<string, string> proNames = CommunicationHelper.GetCurProNames(GlobalFuns.LoginSysId, false);
            if (proNames == null)
            {
                MessageBox.Show("获取项目信息失败！");
                return;
            }
            cbDemandName.ItemsSource = proNames;
            if (proNames.Count == 0)
            {
                MessageBox.Show("当前无项目！");
                return;
            }
            cbDemandName.SelectedValuePath = "Key";
            cbDemandName.DisplayMemberPath = "Value";

            cbIsNew.ItemsSource = new List<string>() {"新增", "修改" };
            GlobalFuns.OpenFlag = true;
        }

        string curDailyId = "";
        /// <summary>
        /// 查询当天签到情况
        /// </summary>
        private bool RefreshCurDayInfo(string userName = "", string _date = "")
        {
            string date = _date;
            string signIn = "";
            string signOut = "";
            if (userName == "")
            {
                userName = tbUserName.Text;
            }
            DataTable dt = CommunicationHelper.GetCurDailyInfo(userName, ref date, out signIn, out signOut, out curDailyId);
            if (dt == null)
            {
                MessageBox.Show("获取日志失败,请先签到");
                return true;
            }
            dt.Columns.Add("WORK_TYPE_NAME");
            foreach (DataRow dr in dt.Rows)
            {
                if (dicWorkType.ContainsKey(dr["WORK_TYPE"].ToString()))
                {
                    dr["WORK_TYPE_NAME"] = dicWorkType[dr["WORK_TYPE"].ToString()];
                }
            }
            dgCurDailyDetail.DataContext = dt;
            tbDate.Text = date;
            tbSignInTime.Text = signIn;
            if (signIn != "")
            {
                if(GlobalFuns.LoginRole == "开发人员")
                {
                    btnSignIn.IsEnabled = false;
                }
            }
            tbSignOutTime.Text = signOut;
            return true;
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            if (tbUserName.Text == "")
            {
                MessageBox.Show("请输入查询姓名");
                return;
            }
            DataTable dt = CommunicationHelper.QueryDailyInfo(tbUserName.Text, tbStartDate.Text, tbEndDate.Text);
            if (dt == null)
            {
                MessageBox.Show("查询签到信息失败");
                return;
            }
            dgHisDailyInfo.DataContext = dt;
        }


        private void dgHisDailyInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView drv = dgHisDailyInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                return;
            }
            DataTable dt = CommunicationHelper.QueryDailyDetail(drv.Row["DAILY_ID"].ToString());
            if (dt == null)
            {
                MessageBox.Show("查询签到详细信息失败");
                return;
            }
            dt.Columns.Add("WORK_TYPE_NAME");
            foreach (DataRow dr in dt.Rows)
            {
                if (dicWorkType.ContainsKey(dr["WORK_TYPE"].ToString()))
                {
                    dr["WORK_TYPE_NAME"] = dicWorkType[dr["WORK_TYPE"].ToString()]; 
                }
            }
            dgDailyDetail.DataContext = dt;
        }

        private void tbDate_LostFocus(object sender, RoutedEventArgs e)
        {
            RefreshCurDayInfo(tbUserName.Text, tbDate.Text);
        }

        private void dgCurDailyDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tbStartDate_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CalendarPop.ShowCalendarWind(tbStartDate);
        }

        private void tbEndDate_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CalendarPop.ShowCalendarWind(tbEndDate);
        }

        private void tbDate_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CalendarPop.ShowCalendarWind(tbDate);
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            if (tbUserName.Text == "")
            {
                MessageBox.Show("请输入姓名");
                return;
            }
            if (tbDate.Text == "")
            {
                MessageBox.Show("请输入日期");
                return;
            }
            string signInTime = tbSignInTime.Text;
            string signDate = tbDate.Text;
            string harddeskId = "";
            switch (GlobalFuns.LoginRole)
            {
                case "开发人员":
                    signInTime = "";
                    signDate = "";
                    harddeskId = GlobalFuns.GetHardDiskID();
                    break;
                default:
                    break;
            }
            if (!CommunicationHelper.DailySignIn(tbUserName.Text, signDate, signInTime, harddeskId))
            {
                MessageBox.Show("签到失败");
                return;
            }
            btnQuery_Click(null, null);
            RefreshCurDayInfo(tbUserName.Text, tbDate.Text);
        }

        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
            if (tbUserName.Text == "")
            {
                MessageBox.Show("请输入姓名");
                return;
            }
            if (tbDate.Text == "")
            {
                MessageBox.Show("请输入日期");
                return;
            }
            string signOutTime = tbSignOutTime.Text;
            string signDate = tbDate.Text;
            string harddeskId = "";
            switch (GlobalFuns.LoginRole)
            {
                case "开发人员":
                    signOutTime = "";
                    signDate = "";
                    harddeskId = GlobalFuns.GetHardDiskID();
                    break;
                default:
                    break;
            }
            if (!CommunicationHelper.DailySignOut(tbUserName.Text, signDate, signOutTime, harddeskId))
            {
                MessageBox.Show("签退失败");
                return;
            }
            btnQuery_Click(null, null);
            RefreshCurDayInfo(tbUserName.Text, tbDate.Text);
        }

        private void btnAddDetail_Click(object sender, RoutedEventArgs e)
        {
            string selectKey = cbWorkType.SelectedValue.ToString();
            double time = 0;
            if (!double.TryParse(tbUseHours.Text, out time))
            {
                MessageBox.Show("请输入正确的用时时间");
                return;
            }
            if (time == 0)
            {
                MessageBox.Show("请输入正确的用时时间");
                return;
            }
            switch (selectKey)
            {
                case "1"://开发
                    if (cbDemandName.Text == "")
                    {
                        MessageBox.Show("请选择开发项目");
                        break;
                    }
                    if (tbTradeCode.Text == "")
                    {
                        MessageBox.Show("请输入开发交易代码");
                        break;
                    }
                    if (time / 8 + double.Parse(tbUseedDays.Text) > double.Parse(tbSysEstimatedDays.Text))
                    {
                        MessageBox.Show("工作量超过预计，请联系项目经理协调");
                        break;
                    }
                    break;
                case "2"://会议
                case "3"://文档整理
                    if (cbDemandName.Text == "")
                    {
                        MessageBox.Show("请选择开发项目");
                        break;
                    }
                    if (tbRemark.Text == "")
                    {
                        MessageBox.Show("会议请备注会议名称、文档请备注文档名称");
                        break;
                    }
                    if (time / 8 + double.Parse(tbUseedDays.Text) > double.Parse(tbSysEstimatedDays.Text))
                    {
                        MessageBox.Show("工作量超过预计，请联系项目经理");
                        break;
                    }
                    break;
                case "4"://运维
                    if (tbRemark.Text == "")
                    {
                        MessageBox.Show("请备注运维内容");
                        break;
                    }
                    break;
                default://其他
                    if (tbRemark.Text == "")
                    {
                        MessageBox.Show("请备注工作内容");
                        break;
                    }
                    break;
            }
            DataTable dt = dgCurDailyDetail.DataContext as DataTable;
            if (dt == null)
            {
                if (tbDate.Text == "")
                {
                    MessageBox.Show("请输入日期");
                    return;
                }
                if (tbUserName.Text == "")
                {
                    MessageBox.Show("请输入姓名");
                    return;
                }
                RefreshCurDayInfo(tbUserName.Text, tbDate.Text);
                dt = dgCurDailyDetail.DataContext as DataTable;
                if (dt == null)
                {
                    return;
                }
            }
            double todayHours = 0;
            string demandId = cbDemandName.SelectedValue as string;
            string sysId = cbSystem.SelectedValue.ToString();
            string workType = cbWorkType.SelectedValue.ToString();
            foreach(DataRow dr in dt.Rows)
            {
                if (tbTradeCode.Text == dr["TRADE_CODE"].ToString() && demandId == dr["DEMAND_ID"].ToString())
                {
                    MessageBox.Show("不能重复添加交易");
                    return;
                }
                todayHours += double.Parse(dr["USER_HOURS"].ToString());
            }
            if (todayHours + time > 8)
            {
                MessageBox.Show("录入工作量超过8小时，加班请项目经理辅助录入");
                if (GlobalFuns.LoginRole == "开发人员")
                {
                    return;
                }
            }
            dt.Rows.Add(new string[] { curDailyId, demandId, sysId, workType, tbTradeCode.Text, tbUseHours.Text, 
            tbRemark.Text, demandId, sysId, tbTradeCode.Text, tbTradeName.Text, cbIsNew.Text, tbFileName.Text, 
            tbUserName.Text, "", "", workType,
            (cbDemandName.ItemsSource as Dictionary<string, string>)[demandId], 
            (cbSystem.ItemsSource as Dictionary<string, string>)[sysId], 
            dicWorkType[workType]});
        }

        private void cbWorkType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectKey = cbWorkType.SelectedValue.ToString();
            switch (selectKey)
            {
                case "1"://开发
                    spProInfo1.Visibility = Visibility.Visible;
                    spProInfo2.Visibility = Visibility.Visible;
                    spTradesInfo1.Visibility = Visibility.Visible;
                    spTradesInfo2.Visibility = Visibility.Visible;
                    break;
                case "2"://会议
                case "3"://文档整理
                    spProInfo1.Visibility = Visibility.Visible;
                    spProInfo2.Visibility = Visibility.Visible;
                    spTradesInfo1.Visibility = Visibility.Collapsed;
                    spTradesInfo2.Visibility = Visibility.Collapsed;
                    break;
                case "4"://运维
                    spProInfo1.Visibility = Visibility.Collapsed;
                    spProInfo2.Visibility = Visibility.Collapsed;
                    spTradesInfo1.Visibility = Visibility.Collapsed;
                    spTradesInfo2.Visibility = Visibility.Collapsed;
                    break;
                default://其他
                    spProInfo1.Visibility = Visibility.Collapsed;
                    spProInfo2.Visibility = Visibility.Collapsed;
                    spTradesInfo1.Visibility = Visibility.Collapsed;
                    spTradesInfo2.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        DataTable dtChildrenSysInfo;
        private void cbDemandName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string select = cbDemandName.SelectedValue as string;
            dtChildrenSysInfo = CommunicationHelper.GetProSystemInfo(select);
            if (dtChildrenSysInfo == null)
            {
                MessageBox.Show("获取项目子单信息失败！");
                return;
            }
            Dictionary<string, string> dicSysInfo = new Dictionary<string, string>();
            foreach (DataRow dr in dtChildrenSysInfo.Rows)
            {
                dicSysInfo.Add(dr["SYS_ID"].ToString(), dr["SYS_NAME"].ToString());
            }
            cbSystem.ItemsSource = dicSysInfo;
            cbSystem.DisplayMemberPath = "Value";
            cbSystem.SelectedValuePath = "Key";
            if (GlobalFuns.LoginSysId != "")
            {
                cbSystem.IsEnabled = false;
                cbSystem.SelectedValue = GlobalFuns.LoginSysId;
            }
            else
            {
                cbSystem.IsEnabled = true;
                cbSystem.SelectedIndex = 0;
            }
        }

        DataTable dtSysTrades;
        private void cbSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtChildrenSysInfo == null)
            {
                return;
            }
            string select = "";
            if (cbSystem.SelectedValue == null)
            {
                if(GlobalFuns.LoginSysId == "")
                {
                    select = (cbSystem.ItemsSource as Dictionary<string, string>).ElementAt(0).Key;
                }
                else
                {
                    select = GlobalFuns.LoginSysId; 
                }
            }
            foreach (DataRow dr in dtChildrenSysInfo.Rows)
            {
                if (dr["SYS_ID"].ToString() == select)
                {
                    tbSysEstimatedDays.Text = dr["ESTIMATE_DAYS"].ToString();
                    tbUseedDays.Text = dr["USED_DAYS"].ToString();
                    break;
                }
            }
            dtSysTrades = CommunicationHelper.GetTradesInfo(select, GlobalFuns.LoginSysId);
            if (dtSysTrades == null)
            {
                MessageBox.Show("获取开发信息失败！");
                return;
            }
        }

        private void tbTradeCode_LostFocus(object sender, RoutedEventArgs e)
        {
            if (dtSysTrades == null)
            {
                return;
            }
            if(tbTradeCode.Text == "")
            {
                return;
            }
            cbIsNew.Text = "新增";
            foreach (DataRow dr in dtSysTrades.Rows)
            {
                if (dr["TRADE_CODE"].ToString() == tbTradeCode.Text)
                {
                    cbIsNew.Text = dr["IS_NEW"].ToString();
                    tbTradeName.Text = dr["TRADE_NAME"].ToString();
                    tbFileName.Text = dr["FILE_NAME"].ToString();

                    if (dr["WORKER"].ToString() != tbUserName.Text)
                    {
                        MessageBox.Show("此交易上次修改人员是“" + dr["WORKER"].ToString() + "”，添加保存后会更改为当前人员");
                    }
                    break;
                }
            }
        }

        private void btnDelDetail_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgCurDailyDetail.SelectedItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("请选择要删除的工作内容！");
                return;
            }
            drv.Row.Table.Rows.Remove(drv.Row);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = dgCurDailyDetail.DataContext as DataTable;
            if (dt == null)
            {
                MessageBox.Show("请先添加工作内容！");
                return;
            }
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("请先添加工作内容！");
                return;
            }
            if (!CommunicationHelper.SaveDailyDetail(curDailyId, dt))
            {
                MessageBox.Show("保存失败，请检查工作量是否超预期！");
                return;
            }

            MessageBox.Show("保存成功！");
            RefreshCurDayInfo();
        }

    }
}
