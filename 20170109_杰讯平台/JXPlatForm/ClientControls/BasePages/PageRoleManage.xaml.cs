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
    public partial class PageRoleManage
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
            ROLE_ID,
            角色名称,
            备注,
            TOTAL_COUNT
        };

        /// <summary>
        /// 所修改的角色ID
        /// </summary>
        private string modRoleId = "";

        /// <summary>
        /// 构造函数
        /// </summary>
        public PageRoleManage()
        {
            InitializeComponent();
            dgAllRoleInfo.Height = Global.WorkAreaHeight - ADJUST_HEIGHT;
        }

        /// <summary>
        /// 刷新页面
        /// </summary>
        /// <returns></returns>
        public override bool RefreshPage()
        {
            //展示表
            DataTable setDataRole = new DataTable();
            for (int i = 0; i < (int)COLUMNS.TOTAL_COUNT; i++)
            {
                setDataRole.Columns.Add(((COLUMNS)i).ToString());
            }
            setDataRole.Rows.Add(new string[]{"0", "系统管理员",  "系统自带，最高权限，不可更改"});
            //角色信息表 角色ID，角色名称，备注
            DataTable dtAllRole = new DataTable();
            CommonDef.COM_RET ret = CommunicationHelper.GetAllRoleInfo(ref dtAllRole);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return false;
            }
            foreach (DataRow dr in dtAllRole.Rows)
            {
                List<object> newRow = new List<object>(dr.ItemArray);
                setDataRole.Rows.Add(newRow.ToArray());
            }
            dgAllRoleInfo.SetTable(setDataRole, true);
            dgAllRoleInfo.SetColumnVisible(COLUMNS.ROLE_ID.ToString(), false);
            spDefaultShow.Visibility = Visibility.Visible;
            gbAddMod.Visibility = Visibility.Collapsed;

            return true;
        }
        
        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddRole_Click(object sender, RoutedEventArgs e)
        {
            gbAddMod.Header = "增加角色信息";
            tbRoleName.Text = "";
            InitRoleAuthBox();
            tbRemark.Text = "";

            dgRoleAuth.IsEnabled = true;
            tbRoleName.IsEnabled = true;
            tbRemark.IsEnabled = true;
            btnOK.Visibility = Visibility.Visible;

            spDefaultShow.Visibility = Visibility.Collapsed;
            gbAddMod.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 修改角色信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModRole_Click(object sender, RoutedEventArgs e)
        {
            DataRow dr = dgAllRoleInfo.GetSelectSingleRow();
            if (dr == null)
            {
                ShowMassageBox.JXMassageBox("请选择一行数据！");
                return;
            }
            if(dr[2].ToString() == "系统管理员")
            {
                ShowMassageBox.JXMassageBox("该角色不能修改！");
                return;
            }
            //角色信息表 角色ID，功能名称
            DataTable dtRoleFunc = new DataTable();
            CommonDef.COM_RET ret = CommunicationHelper.GetRoleFunc(dr[COLUMNS.ROLE_ID.ToString()].ToString(),
                ref dtRoleFunc);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }
            List<string> unAuthFunc = new List<string>();
            foreach (DataRow authRow in dtRoleFunc.Rows)
            {
                unAuthFunc.Add(authRow[1].ToString());
            }

            gbAddMod.Header = "修改角色信息";
            InitRoleAuthBox(unAuthFunc);
            modRoleId = dr[COLUMNS.ROLE_ID.ToString()].ToString();
            tbRoleName.Text = dr[COLUMNS.角色名称.ToString()].ToString();
            tbRemark.Text = dr[COLUMNS.备注.ToString()].ToString();

            dgRoleAuth.IsEnabled = true;
            tbRoleName.IsEnabled = true;
            tbRemark.IsEnabled = true;
            btnOK.Visibility = Visibility.Visible;

            spDefaultShow.Visibility = Visibility.Collapsed;
            gbAddMod.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 查询权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRoleAuth_Click(object sender, RoutedEventArgs e)
        {
            DataRow dr = dgAllRoleInfo.GetSelectSingleRow();
            if (dr == null)
            {
                ShowMassageBox.JXMassageBox("请选择一行数据！");
                return;
            }
            //角色信息表 角色ID，功能名称
            DataTable dtRoleFunc = new DataTable();
            CommonDef.COM_RET ret = CommunicationHelper.GetRoleFunc(dr[COLUMNS.ROLE_ID.ToString()].ToString(), 
                ref dtRoleFunc);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }
            List<string> unAuthFunc = new List<string>();
            foreach (DataRow authRow in dtRoleFunc.Rows)
            {
                unAuthFunc.Add(authRow[1].ToString());
            }

            gbAddMod.Header = "查询角色信息";
            InitRoleAuthBox(unAuthFunc);
            tbRoleName.Text = dr[COLUMNS.角色名称.ToString()].ToString();
            tbRemark.Text = dr[COLUMNS.备注.ToString()].ToString();

            dgRoleAuth.IsEnabled = false;
            tbRoleName.IsEnabled = false;
            tbRemark.IsEnabled = false;
            btnOK.Visibility = Visibility.Hidden;

            spDefaultShow.Visibility = Visibility.Collapsed;
            gbAddMod.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 初始化权限表
        /// </summary>
        /// <param name="unAuthFunc"></param>
        private void InitRoleAuthBox(List<string> unAuthFunc = null)
        {
            DataTable setDt = new DataTable();
            setDt.Columns.Add(new DataColumn("选择", typeof(bool)) { ReadOnly = false });
            setDt.Columns.Add(new DataColumn("所在目录", typeof(string)) { ReadOnly = true });
            setDt.Columns.Add(new DataColumn("功能名称", typeof(string)) { ReadOnly = true });
            List<List<object>> rows = GetAuthRows("", Global.RootMenu, unAuthFunc);
            foreach (List<object> row in rows)
            {
                setDt.Rows.Add(row.ToArray());
            }
            foreach (var dic in CommonDef.DicFileAss)
            {
                string className = dic.Key + "." + dic.Key + "Client";
                Type tp = dic.Value.GetType(className);
                Object obj = Activator.CreateInstance(tp);
                ExpandBaseClient expand = obj as ExpandBaseClient;
                List<MenuTree> listMenu = expand.GetExpandTree();
                rows = GetAuthRows("", listMenu, unAuthFunc);
                foreach (List<object> row in rows)
                {
                    setDt.Rows.Add(row.ToArray());
                }
            }
            dgRoleAuth.SetTable(setDt, true);
        }

        /// <summary>
        /// 获取权限表行信息
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="listMenuTree"></param>
        /// <param name="unAuthFunc"></param>
        /// <returns></returns>
        private List<List<object>> GetAuthRows(string rootPath, List<MenuTree> listMenuTree, List<string> unAuthFunc)
        {
            List<List<object>> retList = new List<List<object>>();
            string curRoot = rootPath;
            foreach (MenuTree menu in listMenuTree)
            {
                if (null == menu.ListChildMenu)
                {
                    List<object> newRow = new List<object>();
                    if (unAuthFunc == null)
                    {
                        newRow.Add(true);
                    }
                    else
                    {
                        if (unAuthFunc.Contains(menu.MenuName))
                        {
                            newRow.Add(false);
                        }
                        else
                        {
                            newRow.Add(true);
                        }
                    }
                    newRow.Add(rootPath);
                    newRow.Add(menu.MenuName);
                    retList.Add(newRow);
                }
                else
                {
                    string newRoot = "";
                    if (curRoot != "")
                    {
                        newRoot = curRoot + "/";
                    }
                    newRoot += menu.MenuName;
                    retList.AddRange(GetAuthRows(newRoot, menu.ListChildMenu, unAuthFunc));
                }
            }
            return retList;
        }

        /// <summary>
        /// 删除角色信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelRole_Click(object sender, RoutedEventArgs e)
        {
            DataRow[] drs = dgAllRoleInfo.GetSelectMultiRows();
            if (drs.Length == 0)
            {
                ShowMassageBox.JXMassageBox("请勾选索要删除的数据！");
                return;
            }
            List<string> listDelRoles = new List<string>();
            foreach (DataRow dr in drs)
            {
                listDelRoles.Add(dr[COLUMNS.ROLE_ID.ToString()].ToString());
            }
            if (listDelRoles.Contains("0"))
            {
                ShowMassageBox.JXMassageBox("系统管理员角色不能被删除！");
                return;
            }
            if (ShowMassageBox.JXMassageBox("确认删除所选择的数据！", ShowMassageBox.SHOW_TYPE.SHOW_QUEST) !=
                ShowMassageBox.SHOW_RES.SELECT_OK)
            {
                return;
            }
            CommonDef.COM_RET ret = CommunicationHelper.DelRoleInfo(listDelRoles);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                return;
            }
            RefreshPage();
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
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (tbRoleName.Text == "")
            {
                ShowMassageBox.JXMassageBox("请输入角色名称！");
                return;
            }
            List<List<object>> listAuthRole = GetAuthRows("", Global.RootMenu, null);
            foreach (var dic in CommonDef.DicFileAss)
            {
                string className = dic.Key + "." + dic.Key + "Client";
                Type tp = dic.Value.GetType(className);
                Object obj = Activator.CreateInstance(tp);
                ExpandBaseClient expand = obj as ExpandBaseClient;
                List<MenuTree> listMenu = expand.GetExpandTree();
                listAuthRole.AddRange(GetAuthRows("", listMenu, null));
            }

            DataRow[] selectRows = dgRoleAuth.GetSelectMultiRows();
            List<string> selectRoleAuth = new List<string>();
            List<string> unAuthRoles = new List<string>();
            foreach(DataRow dr in selectRows)
            {
                selectRoleAuth.Add(dr["功能名称"].ToString());
            }
            foreach (List<object> row in listAuthRole)
            {
                if (!selectRoleAuth.Contains(row[2].ToString()))
                {
                    unAuthRoles.Add(row[2].ToString());
                }
            }
            if (gbAddMod.Header.ToString() == "增加角色信息")
            {
                CommonDef.COM_RET ret = CommunicationHelper.AddRoleInfo(tbRoleName.Text, unAuthRoles,
                    tbRemark.Text);
                if (ret != CommonDef.COM_RET.RET_OK)
                {
                    ShowMassageBox.JXMassageBox(CommonDef.GetErrorInfo(ret));
                    return;
                }
            }
            else
            {
                CommonDef.COM_RET ret = CommunicationHelper.ModRoleInfo(modRoleId, tbRoleName.Text, unAuthRoles,
                    tbRemark.Text);
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
