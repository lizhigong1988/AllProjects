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
using 项目管理.Tools;
using System.IO;
using 项目管理.Connect;

namespace 项目管理.Pages
{
    /// <summary>
    /// AddEngineerInfo.xaml 的交互逻辑
    /// </summary>
    public partial class QueryWorkDays : UserControl
    {
        public QueryWorkDays()
        {
            InitializeComponent();

            tbYearMonth.Text = DateTime.Now.ToString("yyyyMM");

            cbSystem.ItemsSource = CommunicationHelper.GetAllSysDic();
            cbSystem.SelectedValuePath = "Key";
            cbSystem.DisplayMemberPath = "Value";
            if (GlobalFuns.LoginSysId != "")
            {
                cbSystem.SelectedValue = GlobalFuns.LoginSysId;
                cbSystem.IsEnabled = false;
            }
            else
            {
                cbSystem.SelectedIndex = 0;
            }
        }

        private string curQuerySys = "";
        private string curQueryWorker = "";
        private string curQueryDate = "";
        private void btnQueryPro_Click(object sender, RoutedEventArgs e)
        {
            if (cbPerson.Text == "")
            {
                return;
            }
            if (tbYearMonth.Text.Length != 6)
            {
                return;
            }
            curQuerySys = cbSystem.SelectedValue.ToString();
            curQueryWorker = cbPerson.Text;
            curQueryDate = tbYearMonth.Text;
            DataTable dt = CommunicationHelper.QueryProDaysInfo(curQuerySys, cbPerson.Text, tbYearMonth.Text);
            dgProInfo.DataContext = dt;
            RefreshMonthDays();
        }

        private void dgProInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView drv = dgProInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                return;
            }
            DataTable dtWorkDays = CommunicationHelper.GetWorkDays(
                curQuerySys, curQueryWorker, drv.Row["DEMAND_ID"].ToString());
            if (dtWorkDays == null)
            {
                return;
            }
            int startMonth = int.Parse(drv.Row["DEMAND_DATE"].ToString().Substring(0, 6));
            string strEndMonth = drv.Row["FINISH_DATE"].ToString();
            if (strEndMonth.Length < 6)
            {
                strEndMonth = DateTime.Now.ToString("yyyyMM");
            }
            strEndMonth = strEndMonth.Substring(0, 6);
            int endMonth = int.Parse(strEndMonth);
            bool hasRow =  false;
            DataTable dtAjust = dtWorkDays.Clone();
            for (int i = startMonth; i < endMonth + 1; i++)
            {
                if (i.ToString().EndsWith("13"))
                {
                    i += 88;
                }
                hasRow = false;
                foreach (DataRow dr in dtWorkDays.Rows)
                {
                    if (dr["MONTH"].ToString() == i.ToString())
                    {
                        dtAjust.Rows.Add(dr.ItemArray);
                        hasRow = true;
                        break;
                    }
                }
                if (!hasRow)
                {
                    dtAjust.Rows.Add(new string[] { drv.Row["DEMAND_ID"].ToString(), curQuerySys, curQueryWorker, i.ToString(), "0" });
                }
            }
            dgWorkDaysInfo.DataContext = dtAjust;
            RefreshAjustable();
        }

        private void dgWorkDaysInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView drv = dgWorkDaysInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                return;
            }
            tbMonthAjust.Text = drv.Row["WORKLOAD"].ToString();
        }

        private void btnAdust_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgWorkDaysInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("请选择需要调整的行");
                return;
            }
            drv.Row["WORKLOAD"] = tbMonthAjust.Text;
            RefreshAjustable();
        }

        private void RefreshAjustable()
        {
            DataRowView drv = dgProInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                return;
            }
            double totalWorkDays = 0;
            DataTable dtAjust = dgWorkDaysInfo.DataContext as DataTable;
            foreach (DataRow dr in dtAjust.Rows)
            {
                totalWorkDays += double.Parse(dr["WORKLOAD"].ToString());
            }
            tbAjustable.Text = (double.Parse(drv.Row["PERSON_DAYS"].ToString()) - totalWorkDays).ToString();
        }

        private void btnSaveAdust_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = dgProInfo.SelectedItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("保存失败！");
                return;
            }
            if (!CommunicationHelper.SaveAdjustWorkDays(drv.Row["DEMAND_ID"].ToString(), curQuerySys, 
                curQueryWorker, dgWorkDaysInfo.DataContext as DataTable))
            {
                MessageBox.Show("保存失败！");
                return;
            }
            MessageBox.Show("保存成功！");
            RefreshMonthDays();
        }

        private void RefreshMonthDays()
        {
            tbTotalDays.Text = CommunicationHelper.GetWorkerMonthDays(curQueryWorker, curQueryDate);
        }

        private void cbSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> listWorkers;
            if (cbSystem.SelectedValue == null)
            {
                listWorkers = CommunicationHelper.GetHisWorkers(GlobalFuns.LoginSysId);
                cbPerson.ItemsSource = listWorkers;
                if (listWorkers.Count != 0)
                {
                    cbPerson.SelectedIndex = 0;
                }
                return;
            }
            string selectSys = cbSystem.SelectedValue.ToString();
            listWorkers = CommunicationHelper.GetHisWorkers(selectSys);
            cbPerson.ItemsSource = listWorkers;
            if (listWorkers.Count != 0)
            {
                cbPerson.SelectedIndex = 0;
            }
        }

    }
}
