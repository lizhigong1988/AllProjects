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
using ClientLibrary;
using System.Data;
using CommonLib;
using ClientCommunication;
using ClientControls;
using System.Reflection;
using System.IO;

namespace JXPlatForm.Client.MainPage
{
    /// <summary>
    /// MainMenu.xaml 的交互逻辑
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public static RoutedEvent MenuItemClickEvent =
            EventManager.RegisterRoutedEvent("MenuItemClick",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(MainMenu));

        public event RoutedEventHandler MenuItemClick
        {
            add { this.AddHandler(MenuItemClickEvent, value); }
            remove { this.RemoveHandler(MenuItemClickEvent, value); }
        }
        public MainMenu()
        {
            InitializeComponent();
        }

        public void MenuInit()
        {
            //获取Menu信息
            List<MenuTree> trees = Global.MenuCopy(Global.RootMenu);
            string[] files = Directory.GetFiles(System.Environment.CurrentDirectory);
            foreach (string filePath in files)
            {
                if ("" == filePath)
                {
                    continue;
                }
                try
                {
                    string dllName = System.IO.Path.GetFileName(filePath);
                    if (!(dllName.StartsWith(CommonDef.EXPAND_DIR) && dllName.EndsWith(".dll")))
                    {
                        continue;
                    }
                    string file = dllName.Remove(dllName.Length - 4);
                    string className = file + "." + file + "Client";
                    Object obj = null;
                    Type tp = null;
                    if (CommonDef.DicFileAss.ContainsKey(file))
                    {
                        tp = CommonDef.DicFileAss[file].GetType(className);
                    }
                    else
                    {
                        byte[] fileData = File.ReadAllBytes(dllName);
                        //Assembly ass = Assembly.LoadFrom(dllName);
                        Assembly ass = Assembly.Load(fileData);
                        tp = ass.GetType(className);
                        CommonDef.DicFileAss.Add(file, ass);
                    }
                    obj = Activator.CreateInstance(tp);
                    ExpandBaseClient expand = obj as ExpandBaseClient;
                    List<MenuTree> listMenu = expand.GetExpandTree();
                    trees.AddRange(listMenu);
                    AddClassFile(listMenu, file);
                }
                catch
                {
                }
            }

            //获取用户权限信息
            //角色信息表 角色ID，功能名称
            DataTable dtRoleFunc = new DataTable();
            CommonDef.COM_RET ret = CommunicationHelper.GetRoleFunc(Global.LoginUser.RoleId,
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
            GetAuthMenu(trees, unAuthFunc);

            //插入Menu信息
            foreach (MenuTree tree in trees)
            {
                TreeViewItem childItem = new TreeViewItem();
                childItem.Header = tree.MenuName;
                if (null != tree.ListChildMenu)
                {
                    InsertToUIMenu(childItem, tree.ListChildMenu);
                }
                tvMenu.Items.Add(childItem);
            }
            //初始化menu事件
            foreach (TreeViewItem item in tvMenu.Items)
            {
                InitMenuEvent(item);
            }
        }

        private void AddClassFile(List<MenuTree> listMenu, string file)
        {
            if (listMenu == null)
            {
                return;
            }
            foreach (MenuTree menu in listMenu)
            {
                if (null != menu.ListChildMenu)
                {
                    AddClassFile(menu.ListChildMenu, file);
                }
                else
                {
                    PageManager.AddDicPageClasses(menu.MenuName, file);
                }
            }
        }

        private void GetAuthMenu(List<MenuTree> trees, List<string> unAuthFunc)
        {
            List<MenuTree> delTree = new List<MenuTree>();
            foreach (MenuTree tree in trees)
            {
                if (unAuthFunc.Contains(tree.MenuName))
                {
                    delTree.Add(tree);
                }
                if (tree.ListChildMenu != null)
                {
                    GetAuthMenu(tree.ListChildMenu, unAuthFunc);
                    if (tree.ListChildMenu.Count == 0)
                    {
                        delTree.Add(tree);
                    }
                }
            }
            foreach (MenuTree tree in delTree)
            {
                trees.Remove(tree);
            }
        }

        private void InsertToUIMenu(TreeViewItem item, List<MenuTree> tree)
        {
            foreach (MenuTree menu in tree)
            {
                TreeViewItem childItem = new TreeViewItem();
                childItem.Header = menu.MenuName;
                if (null != menu.ListChildMenu)
                {
                    InsertToUIMenu(childItem, menu.ListChildMenu);
                }
                item.Items.Add(childItem);
            }
        }

        private void InitMenuEvent(TreeViewItem item)
        {
            if (item.HasItems)
            {
                foreach (TreeViewItem child in item.Items)
                {
                    InitMenuEvent(child);
                }
            }
            else
            {
                item.Tag = item.Header;
                item.MouseDoubleClick += new MouseButtonEventHandler(item_MouseDoubleClick);
            }
        }

        private void item_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            this.RaiseEvent(new RoutedEventArgs { RoutedEvent = MenuItemClickEvent, Source = item.Tag });
        }
    }
}
