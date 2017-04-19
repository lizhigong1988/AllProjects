using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CommonLib
{
    public class CommonDef
    {
        /// <summary>
        /// 定义默认密码
        /// </summary>
        public static string DEFAUTL_PSW = "111111";

        /// <summary>
        /// 定义扩展包命名前缀
        /// </summary>
        public static string EXPAND_DIR = "Expand";

        /// <summary>
        /// 客户端类型 <!--Single 单机版， Web 网络版-->
        /// </summary>
        public static string CLIENT_TYPE = "Single";
        /// <summary>
        /// 开发模式
        /// </summary>
        public static bool IS_DEBUG = true;

        /// <summary>
        /// 保存扩展文件
        /// </summary>
        public static Dictionary<string, Assembly> DicFileAss = new Dictionary<string, Assembly>();

        /// <summary>
        /// 全局底层返回定义
        /// </summary>
        public enum COM_RET
        {
            RET_OK,
            CREATE_FILE_ERROR,  //通信报文打包错误
            READ_FILE_ERROR,  //通信报文解包错误
            CONNECT_ERROR,      //连接服务错误
            SERVER_READ_FILE_ERROR,   //服务端解包错误
            FUNC_NO_ERROR,   //未知服务代码
            SERVER_ERROR,   //服务器处理异常
            DATABASE_ERROR,   //数据库错误
            NO_USER_INFO,   //没有用户信息
            HAS_USER_INFO,   //用户信息重复
            HAS_ROLE_NAME,  //角色名称重复
            HAS_USER_ROLE, //该角色中存在用户
            NO_ROLE_INFO, //没有角色信息
            DATA_DUP, // 数据重复
            DATA_RELY, // 数据依赖
        }

        private static Dictionary<COM_RET, string> dicErrorInfo = new Dictionary<COM_RET, string>()
        {
            {COM_RET.RET_OK, "操作成功！"},
            {COM_RET.CREATE_FILE_ERROR,  "通信报文打包错误！"}, //通信报文打包错误
            {COM_RET.READ_FILE_ERROR, "通信报文解包错误！"},  //通信报文解包错误
            {COM_RET.CONNECT_ERROR,   "连接服务错误！"},    //连接服务错误
            {COM_RET.SERVER_READ_FILE_ERROR, "服务端解包错误！"},   //服务端解包错误
            {COM_RET.FUNC_NO_ERROR,   "未知服务代码！"}, //未知服务代码
            {COM_RET.SERVER_ERROR,   "服务器处理异常！"}, //服务器处理异常
            {COM_RET.DATABASE_ERROR,  "数据库错误！"},  //数据库错误
            {COM_RET.NO_USER_INFO,  "没有用户信息！"},  //没有用户信息
            {COM_RET.HAS_USER_INFO,  "用户信息重复！"},  //没有用户信息
            {COM_RET.HAS_ROLE_NAME,  "角色名称重复！"},  //角色名称重复
            {COM_RET.HAS_USER_ROLE,  "该角色中存在用户！"},  //该角色中存在用户
            {COM_RET.NO_ROLE_INFO,  "没有角色信息！"},  //没有角色信息
            {COM_RET.DATA_DUP,  "数据重复！"},  
            {COM_RET.DATA_RELY,  "数据存在依赖！"},  
        };

        /// <summary>
        /// 定义用户类
        /// </summary>
        public class UserInfo
        {
            public string UserCode;
            public string UserPsw;
            public string UserName;
            public string RoleId;
            public string UserTel;
            public string UserID;

            public void SetDefalt()
            {
                UserCode = "";
                UserPsw = "";
                UserName = "管理员";
                RoleId = "0";
                UserTel = "";
                UserID = "";
            }
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <param name="ret"></param>
        /// <returns></returns>
        public static string GetErrorInfo(COM_RET ret)
        {
            if (!dicErrorInfo.ContainsKey(ret))
            {
                return "未知错误！";
            }
            return dicErrorInfo[ret];
        }
    }
}
