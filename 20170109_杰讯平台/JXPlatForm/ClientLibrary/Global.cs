using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;
using System.Reflection;

namespace ClientLibrary
{

    public class MenuTree
    {
        public string MenuName {get; set;}
        public List<MenuTree> ListChildMenu { get; set; }
    };


    /// <summary>
    /// 定义全局变量
    /// </summary>
    public class Global
    {

        /// <summary>
        /// 定义登录用户信息
        /// </summary>
        public static CommonDef.UserInfo LoginUser = new CommonDef.UserInfo();

        /// <summary>
        /// 工作区高度
        /// </summary>
        public static double WorkAreaHeight = 0;

        /// <summary>
        /// 总菜单
        /// </summary>
        public static List<MenuTree> RootMenu = new List<MenuTree>()
        {//初始值
             new MenuTree(){
                 MenuName = "基础管理",
                 ListChildMenu = new List<MenuTree>()
                 {
                    new MenuTree(){ MenuName = "用户管理" },//用户信息的增、删、改、查
                    new MenuTree(){ MenuName = "角色管理" },//用户信息角色、权限管理
                    new MenuTree(){ MenuName = "修改密码" },//修改用户登录密码
                    new MenuTree(){ MenuName = "系统设置" },//设置背景图片、logo
                 }
             }
        };

        public static List<MenuTree> MenuCopy(List<MenuTree> rootMenu)
        {
            if (null == rootMenu)
            {
                return null;
            }
            List<MenuTree> copy = new List<MenuTree>();
            foreach (MenuTree menu in rootMenu)
            {
                MenuTree tree = new MenuTree();
                tree.MenuName = menu.MenuName;
                tree.ListChildMenu = MenuCopy(menu.ListChildMenu);
                copy.Add(tree);
            }
            return copy;
        }
    }
}
