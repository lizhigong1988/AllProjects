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
using ClientCommunication;
using System.Data;
using CommonLib;
using ClientLibrary;
using ClientControls;
using ExpandDemo.DataBase;

namespace ExpandDemo.Pages
{
    /// <summary>
    /// PageUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ClientInfoManage
    {
        /// <summary>
        /// 修整高度
        /// </summary>
        private double ADJUST_HEIGHT = 160;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public ClientInfoManage()
        {
            InitializeComponent();
            dgAllClientInfo.Height = Global.WorkAreaHeight - ADJUST_HEIGHT;
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <returns></returns>
        public override bool RefreshPage()
        {
            DataTable tbAllClient = null;
            CommonDef.COM_RET ret =CommunicationHelper.CommonRead("ExpandDemo",
                (int)ExpandDemoCommon.FUNC_NO.GET_CLIENT_INFO, ref tbAllClient);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return false;
            }
            Dictionary<string, string> dicColumns = new Dictionary<string, string>() 
            {
                {"客户姓名", DEMO_CLIENT_INFO.TABLE_COLUMS.CLIENT_NAME.ToString()},
                {"电话号码", DEMO_CLIENT_INFO.TABLE_COLUMS.CLIENT_TEL.ToString()},
                {"地址", DEMO_CLIENT_INFO.TABLE_COLUMS.CLIENT_ADDR.ToString()},
                {"备注", DEMO_CLIENT_INFO.TABLE_COLUMS.CLIENT_REMARK.ToString()},
            };
            dgAllClientInfo.InitDataBox(dicColumns, tbAllClient, true);
            spDefaultShow.Visibility = Visibility.Visible;
            gbAddMod.Visibility = Visibility.Collapsed;
            
            return true;
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (tbClientName.Text == "")
            {
                ShowMassageBox.JXMassageBox("请输入客户姓名！");
                return;
            }
            if (tbClientTel.Text != "")
            {
                if (!InputTypeControl.IsTelephone(tbClientTel.Text))
                {
                    ShowMassageBox.JXMassageBox("请输入正确的手机号码！");
                    return;
                }
            }
            string message = tbClientName.Text.Trim() + "\n" + tbClientTel.Text.Trim() + "\n";
            message += tbAddr.Text.Trim() + "\n" + tbRemark.Text.Trim() + "\n";
            if (gbAddMod.Header.ToString() == "增加客户信息")
            {
                CommonDef.COM_RET ret = CommunicationHelper.CommonSend("ExpandDemo",
                (int)ExpandDemoCommon.FUNC_NO.ADD_CLIENT_INFO, message);
                if (ret != CommonDef.COM_RET.RET_OK)
                {
                    ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                    return;
                }
            }
            else
            {
                message = modClinentIndex + "\n" + message;
                CommonDef.COM_RET ret = CommunicationHelper.CommonSend("ExpandDemo",
                (int)ExpandDemoCommon.FUNC_NO.MOD_CLIENT_INFO, message);
                if (ret != CommonDef.COM_RET.RET_OK)
                {
                    ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                    return;
                }
            }
            RefreshPage();
        }

        private void btnAddClient_Click(object sender, RoutedEventArgs e)
        {
            gbAddMod.Header = "增加客户信息";
            tbClientName.Text = "";
            tbClientTel.Text = "";
            tbAddr.Text = "";
            tbRemark.Text = "";
            spDefaultShow.Visibility = Visibility.Collapsed;
            gbAddMod.Visibility = Visibility.Visible;
        }

        private void btnModClient_Click(object sender, RoutedEventArgs e)
        {
            DataRow dr = dgAllClientInfo.GetSelectSingleRow();
            if (dr == null)
            {
                ShowMassageBox.JXMassageBox("请选择一行数据！");
                return;
            }

            gbAddMod.Header = "修改客户信息";
            tbClientName.Text = dr[DEMO_CLIENT_INFO.TABLE_COLUMS.CLIENT_NAME.ToString()].ToString();
            tbClientTel.Text = dr[DEMO_CLIENT_INFO.TABLE_COLUMS.CLIENT_TEL.ToString()].ToString();
            modClinentIndex = dr[DEMO_CLIENT_INFO.TABLE_COLUMS.CLIENT_ID.ToString()].ToString();
            tbAddr.Text = dr[DEMO_CLIENT_INFO.TABLE_COLUMS.CLIENT_ADDR.ToString()].ToString();
            tbRemark.Text = dr[DEMO_CLIENT_INFO.TABLE_COLUMS.CLIENT_REMARK.ToString()].ToString();
            spDefaultShow.Visibility = Visibility.Collapsed;
            gbAddMod.Visibility = Visibility.Visible;
        }

        private string modClinentIndex = "0";

        private void btnDelClient_Click(object sender, RoutedEventArgs e)
        {
            DataRow[] drs = dgAllClientInfo.GetSelectMultiRows();
            if (drs.Length == 0)
            {
                ShowMassageBox.JXMassageBox("请选择所要删除的数据！");
                return;
            }
            if (ShowMassageBox.JXMassageBox("已有订单的客户不能被删除，确定要删除所选客户！", ShowMassageBox.SHOW_TYPE.SHOW_QUEST) !=
                ShowMassageBox.SHOW_RES.SELECT_OK)
            {
                return;
            }
            string delClients = "";
            foreach (DataRow dr in drs)
            {
                delClients += dr[DEMO_CLIENT_INFO.TABLE_COLUMS.CLIENT_ID.ToString()] + "\n";
            }
            CommonDef.COM_RET ret = CommunicationHelper.CommonSend("ExpandDemo", 
                (int)ExpandDemoCommon.FUNC_NO.DEL_CLIENT, delClients);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }
            RefreshPage();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }
    }
}
