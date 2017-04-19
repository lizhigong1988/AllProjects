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

namespace ClientControls.BasePages
{
    /// <summary>
    /// PageUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class PageUserControl
    {
        /// <summary>
        /// 修整高度
        /// </summary>
        private double ADJUST_HEIGHT = 160;

        /// <summary>
        /// 主表列标题
        /// </summary>
        private enum COLUMNS
        { 
            登录户名,
            USER_PSW,
            用户姓名,
            用户角色,
            手机号码,
            身份证号,
            TOTAL_COUNT
        };

        /// <summary>
        /// 构造函数
        /// </summary>
        public PageUserControl()
        {
            InitializeComponent();
            dgAllUserInfo.Height = Global.WorkAreaHeight - ADJUST_HEIGHT;
        }

        /// <summary>
        /// 增加用户信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            gbAddMod.Header = "增加用户信息";
            tbUserCode.Text = "";
            tbUserCode.IsEnabled = true;
            tbUserName.Text = "";
            tbUserTel.Text = "";
            tbUserID.Text = "";
            tbUserRole.SelectedIndex = 0;
            spDefaultShow.Visibility = Visibility.Collapsed;
            gbAddMod.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModUser_Click(object sender, RoutedEventArgs e)
        {
            DataRow dr = dgAllUserInfo.GetSelectSingleRow();
            if (dr == null)
            {
                ShowMassageBox.JXMassageBox("请选择一行数据！");
                return;
            }

            gbAddMod.Header = "修改用户信息";
            tbUserCode.Text = dr[COLUMNS.登录户名.ToString()].ToString();
            tbUserCode.IsEnabled = false;
            tbUserName.Text = dr[COLUMNS.用户姓名.ToString()].ToString();
            tbUserTel.Text = dr[COLUMNS.手机号码.ToString()].ToString();
            tbUserID.Text = dr[COLUMNS.身份证号.ToString()].ToString();
            tbUserRole.Text = dr[COLUMNS.用户角色.ToString()].ToString();
            spDefaultShow.Visibility = Visibility.Collapsed;
            gbAddMod.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelUser_Click(object sender, RoutedEventArgs e)
        {
            DataRow[] drs = dgAllUserInfo.GetSelectMultiRows();
            if (drs.Length == 0)
            {
                ShowMassageBox.JXMassageBox("请选择所要删除的数据！");
                return;
            }
             if(ShowMassageBox.JXMassageBox("确定要删除所选用户！", ShowMassageBox.SHOW_TYPE.SHOW_QUEST) !=
                 ShowMassageBox.SHOW_RES.SELECT_OK)
             {
                return;
            }
            List<string> listDelUsers = new List<string>();
            foreach (DataRow dr in drs)
            {
                listDelUsers.Add(dr[COLUMNS.登录户名.ToString()].ToString());
            }
            CommonDef.COM_RET ret = CommunicationHelper.DelUserInfo(listDelUsers);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }
            RefreshPage();
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReSetPsw_Click(object sender, RoutedEventArgs e)
        {
            DataRow[] drs = dgAllUserInfo.GetSelectMultiRows();
            if (drs.Length == 0)
            {
                ShowMassageBox.JXMassageBox("请选择所要重置密码的用户！");
                return;
            }
            List<string> listDelUsers = new List<string>();
            foreach (DataRow dr in drs)
            {
                listDelUsers.Add(dr[COLUMNS.登录户名.ToString()].ToString());
            }
            string defaultPsw = "";
            CommonDef.COM_RET ret = CommunicationHelper.ReSetUserPsw(listDelUsers, ref defaultPsw);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }
            ShowMassageBox.JXMassageBox("选择用户的密码已经重置为【" + defaultPsw + "】！");
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
            DataTable tbAllUser = new DataTable();
            CommonDef.COM_RET ret = CommunicationHelper.GetAllUserInfo(ref tbAllUser);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return false;
            }
            for (int i = 0; i < tbAllUser.Columns.Count; i++)
            {
                tbAllUser.Columns[i].ColumnName = ((COLUMNS)i).ToString();
            }
            dgAllUserInfo.SetTable(tbAllUser, true);
            dgAllUserInfo.SetColumnVisible(COLUMNS.USER_PSW.ToString(),  false);
            Dictionary<string, string> dicTansRole = new Dictionary<string,string>()
            {
                { "0", "系统管理员"}
            };
            //角色信息表 角色ID，角色名称，备注
            DataTable dtAllRole = new DataTable();
            ret = CommunicationHelper.GetAllRoleInfo(ref dtAllRole);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return false;
            }
            foreach (DataRow dr in dtAllRole.Rows)
            {
                dicTansRole.Add(dr[0].ToString(), dr[1].ToString());
            }

            dgAllUserInfo.TranslateColumn(COLUMNS.用户角色.ToString(), dicTansRole);
            tbUserRole.ItemsSource = dicTansRole;
            tbUserRole.SelectedValuePath = "Key";
            tbUserRole.DisplayMemberPath = "Value";

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
            if (tbUserCode.Text == "")
            {
                ShowMassageBox.JXMassageBox("请输入登录户名！");
                return;
            }
            if (tbUserName.Text == "")
            {
                ShowMassageBox.JXMassageBox("请输入用户姓名！");
                return;
            }
            if (tbUserTel.Text != "")
            {
                if (!InputTypeControl.IsTelephone(tbUserTel.Text))
                {
                    ShowMassageBox.JXMassageBox("请输入正确的手机号码！");
                    return;
                }
            }
            if (tbUserID.Text != "")
            {
                if (!InputTypeControl.CheckIDCard18(tbUserID.Text))
                {
                    ShowMassageBox.JXMassageBox("请输入正确的身份证号！");
                    return;
                }
            }
            if (gbAddMod.Header.ToString() == "增加用户信息")
            {
                CommonDef.COM_RET ret = CommunicationHelper.AddUserInfo(tbUserCode.Text, tbUserName.Text,
                    tbUserRole.SelectedValue as string, tbUserTel.Text, tbUserID.Text);
                if (ret != CommonDef.COM_RET.RET_OK)
                {
                    ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                    return;
                }
            }
            else
            {
                CommonDef.COM_RET ret = CommunicationHelper.ModUserInfo(tbUserCode.Text, tbUserName.Text,
                    tbUserRole.SelectedValue as string, tbUserTel.Text, tbUserID.Text);
                if (ret != CommonDef.COM_RET.RET_OK)
                {
                    ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                    return;
                }
            }
            RefreshPage();
        }
    }
}
