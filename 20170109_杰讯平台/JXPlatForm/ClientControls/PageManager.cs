using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using ClientControls.BasePages;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using ClientLibrary;
using CommonLib;

namespace ClientControls
{
    public class PageBase : UserControl
    {
        public PageBase()
        {
            this.Background = Brushes.Gray;
        }

        //enter键拥有tab键的功能
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                // MoveFocus takes a TraveralReqest as its argument.
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                // Gets the element with keyboard focus.
                UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;
                // Change keyboard focus. 
                if (elementWithFocus != null)
                {
                    elementWithFocus.MoveFocus(request);
                }
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// 刷新初始化页面信息元素
        /// </summary>
        virtual public bool RefreshPage()
        { 
            return true;
        }

        /// <summary>
        /// 关闭页面
        /// </summary>
        /// <returns></returns>
        virtual public bool PageClose()
        {
            return true;
        }
    }

    public class ExpandBaseClient
    {
        /// <summary>
        /// 获取扩展目录
        /// </summary>
        /// <returns></returns>
        virtual public List<MenuTree> GetExpandTree()
        {
            return null;
        }

        /// <summary>
        /// 获取目录和页面类名对应关系
        /// </summary>
        /// <returns></returns>
        virtual public string GetPageFile(string menuName)
        {
            return null;
        }
    }

    public class PageManager
    {
        private static Dictionary<string, string> dicClassesFile = new Dictionary<string, string>();
        public static void AddDicPageClasses(string key, string value)
        {
            if (!dicClassesFile.ContainsKey(key))
            {
                dicClassesFile.Add(key, value);
            }
        }

        public static PageBase GetNewPage(string pageName)
        {
            PageBase page = null;
            switch (pageName)
            {
                case "用户管理":
                    page = new PageUserControl();
                    break;
                case "角色管理":
                    page = new PageRoleManage();
                    break;
                case "修改密码":
                    page = new PageModUserPsw();
                    break;
                case "系统设置":
                    page = new PageSysSetting();
                    break;
                default:
                    page = GetExpandPage(pageName);
                    break;
            }
            if (page == null)
            {
                return null;
            }

            page.Background = Brushes.Transparent;
            object obj = page.Content;
            Grid grid = new Grid();
            page.Content = grid;

            ScrollViewer scv = new ScrollViewer();
            scv.Content = obj;
            scv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            Border bd = new Border();
            bd.BorderThickness = new Thickness(0, 2, 0, 2);
            bd.BorderBrush = Brushes.DarkOrange;
            bd.Background = Brushes.Gray;
            bd.Opacity = 0.3;

            grid.Children.Add(bd);
            grid.Children.Add(scv);

            return page;
        }

        private static PageBase GetExpandPage(string pageName)
        {
            PageBase page = null;
            try
            {
                string file = dicClassesFile[pageName];
                string className = file + "." + file + "Client";
                Type tp = CommonDef.DicFileAss[file].GetType(className);
                Object obj = Activator.CreateInstance(tp);
                ExpandBaseClient expand = obj as ExpandBaseClient;
                string pageClassName = expand.GetPageFile(pageName);
                tp = CommonDef.DicFileAss[dicClassesFile[pageName]].GetType(pageClassName);
                obj = Activator.CreateInstance(tp);
                page = obj as PageBase;
            }
            catch
            {
            }
            return page;
        }
    }
}
