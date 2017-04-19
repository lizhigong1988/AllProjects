using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;
using System.Data;
using ServerImage;
using System.Windows;
using System.Windows.Interop;
using System.ComponentModel;
using System.Management;
using ClientCommunication.Connects;

namespace ClientCommunication
{
    public class CommunicationHelper
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public static CommonDef.COM_RET InitCommunication()
        {
            if (CommonDef.CLIENT_TYPE == "Single")
            {
                return ServerImageHelper.Init();
            }
            return CommonDef.COM_RET.RET_OK;
        }


        /// <summary>
        /// 解析表格数据
        /// </summary>
        /// <param name="dataHelper"></param>
        /// <returns></returns>
        private static DataTable GetDataTable(DataHelper dataHelper)
        {
            string dtInfo = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.SELECT_TABLE);
            DataTable dt = new DataTable();
            string[] lines = dtInfo.Split('\n');
            if (lines.Length == 0)
            {
                return dt;
            }
            string[] tableNames = lines[0].Split('\t');
            foreach (string name in tableNames)
            {
                dt.Columns.Add(name);
            }
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i] == "")
                {
                    continue;
                }
                dt.Rows.Add(lines[i].Split('\t'));
            }
            return dt;
        }

        /// <summary>
        /// 发送文件，如果是单机版，存本地，如果是网络版，发服务端
        /// </summary>
        /// <param name="msgData"></param>
        /// <returns></returns>
        private static CommonDef.COM_RET SendAndReciveFile(ref string msgData)
        {
            LoadingWorker loading = new LoadingWorker();
            HwndSource winformWindow = (HwndSource.FromDependencyObject(LoadingWorker.MainWind) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(loading)
                {
                    Owner = winformWindow.Handle
                };
            }
            loading.MsgData = msgData;
            loading.ShowDialog();
            msgData = loading.MsgData;
            return CommonDef.COM_RET.RET_OK;
        }

        /// <summary>
        /// 获取总用户数
        /// </summary>
        /// <param name="wind"></param>
        /// <param name="countUser"></param>
        /// <returns></returns>
        public static CommonDef.COM_RET CountUserInfo(ref int countUser)
        {
            string msgData = "";
            DataHelper dataHelper = new DataHelper();
            if (!dataHelper.ClearAndAddConfig(out msgData,
                new Dictionary<DataHelper.CONFIG_KEYS, string>(){
                    {DataHelper.CONFIG_KEYS.FUNC_NO, ((int)DataHelper.FUNC_NAME.COUNT_USER_INFO).ToString()},
                }))
            {
                return CommonDef.COM_RET.CREATE_FILE_ERROR;
            }
            CommonDef.COM_RET ret = SendAndReciveFile(ref msgData);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return ret;
            }
            if (!dataHelper.InitConfigDic(msgData))
            {
                return CommonDef.COM_RET.READ_FILE_ERROR;
            }
            DataTable dt = GetDataTable(dataHelper);
            if (dt.Rows.Count == 0)
            {
                return CommonDef.COM_RET.READ_FILE_ERROR;
            }
            countUser = int.Parse(dt.Rows[0][0].ToString());
            return CommonDef.COM_RET.RET_OK;
        }
        /// <summary>
        /// 获取单个用户全部信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static CommonDef.COM_RET GetUserInfo(string userName, CommonDef.UserInfo userInfo)
        {
            string msgData = "";
            DataHelper dataHelper = new DataHelper();
            if (!dataHelper.ClearAndAddConfig(out msgData,
                new Dictionary<DataHelper.CONFIG_KEYS, string>(){
                    {DataHelper.CONFIG_KEYS.FUNC_NO, ((int)DataHelper.FUNC_NAME.GET_USER_INFO).ToString()},
                    {DataHelper.CONFIG_KEYS.USER_CODE, userName}
                }))
            { 
                return CommonDef.COM_RET.CREATE_FILE_ERROR;
            }
            CommonDef.COM_RET ret = SendAndReciveFile(ref msgData);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return ret;
            }
            if (!dataHelper.InitConfigDic(msgData))
            {
                return CommonDef.COM_RET.READ_FILE_ERROR;
            }
            DataTable dt = GetDataTable(dataHelper);
            if (dt.Rows.Count == 0)
            {
                return CommonDef.COM_RET.NO_USER_INFO;
            }
            userInfo.UserCode = dt.Rows[0][0].ToString();
            userInfo.UserPsw = dt.Rows[0][1].ToString();
            userInfo.UserName = dt.Rows[0][2].ToString();
            userInfo.RoleId = dt.Rows[0][3].ToString();
            userInfo.UserTel = dt.Rows[0][4].ToString();
            userInfo.UserID = dt.Rows[0][5].ToString();
            return CommonDef.COM_RET.RET_OK;
        }

        /// <summary>
        /// 获取全部用户信息
        /// </summary>
        /// <returns></returns>
        public static CommonDef.COM_RET GetAllUserInfo(ref DataTable dtUserInfo)
        {
            string msgData = "";
            DataHelper dataHelper = new DataHelper();
            if (!dataHelper.ClearAndAddConfig(out msgData,
                new Dictionary<DataHelper.CONFIG_KEYS, string>(){
                    {DataHelper.CONFIG_KEYS.FUNC_NO, ((int)DataHelper.FUNC_NAME.GET_ALL_USER_INFO).ToString()},
                }))
            {
                return CommonDef.COM_RET.CREATE_FILE_ERROR;
            }
            CommonDef.COM_RET ret = SendAndReciveFile(ref msgData);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return ret;
            }
            if (!dataHelper.InitConfigDic(msgData))
            {
                return CommonDef.COM_RET.READ_FILE_ERROR;
            }
            dtUserInfo = GetDataTable(dataHelper);
            return CommonDef.COM_RET.RET_OK;
        }
        /// <summary>
        /// 新增用户信息
        /// </summary>
        /// <param name="MainWind"></param>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        /// <param name="p_3"></param>
        /// <param name="p_4"></param>
        /// <param name="p_5"></param>
        /// <returns></returns>
        public static CommonDef.COM_RET AddUserInfo(string code, string name, string rule, 
            string tel, string id)
        {
            string msgData = "";
            DataHelper dataHelper = new DataHelper();
            if (!dataHelper.ClearAndAddConfig(out msgData,
                new Dictionary<DataHelper.CONFIG_KEYS, string>(){
                    {DataHelper.CONFIG_KEYS.FUNC_NO, ((int)DataHelper.FUNC_NAME.ADD_USER_INFO).ToString()},
                    {DataHelper.CONFIG_KEYS.USER_CODE, code},
                    {DataHelper.CONFIG_KEYS.USER_NAME, name},
                    {DataHelper.CONFIG_KEYS.ROLE_ID, rule},
                    {DataHelper.CONFIG_KEYS.USER_TEL, tel},
                    {DataHelper.CONFIG_KEYS.USER_ID, id},
                }))
            {
                return CommonDef.COM_RET.CREATE_FILE_ERROR;
            }
            CommonDef.COM_RET ret = SendAndReciveFile(ref msgData);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return ret;
            }
            if (!dataHelper.InitConfigDic(msgData))
            {
                return CommonDef.COM_RET.READ_FILE_ERROR;
            }
            return (CommonDef.COM_RET)int.Parse(dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ERROR_NO));
        }
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        /// <param name="p_3"></param>
        /// <param name="p_4"></param>
        /// <param name="p_5"></param>
        /// <returns></returns>
        public static CommonDef.COM_RET ModUserInfo(string code, string name, string rule,
            string tel, string id)
        {
            string msgData = "";
            DataHelper dataHelper = new DataHelper();
            if (!dataHelper.ClearAndAddConfig(out msgData,
                new Dictionary<DataHelper.CONFIG_KEYS, string>(){
                    {DataHelper.CONFIG_KEYS.FUNC_NO, ((int)DataHelper.FUNC_NAME.MOD_USER_INFO).ToString()},
                    {DataHelper.CONFIG_KEYS.USER_CODE, code},
                    {DataHelper.CONFIG_KEYS.USER_NAME, name},
                    {DataHelper.CONFIG_KEYS.ROLE_ID, rule},
                    {DataHelper.CONFIG_KEYS.USER_TEL, tel},
                    {DataHelper.CONFIG_KEYS.USER_ID, id},
                }))
            {
                return CommonDef.COM_RET.CREATE_FILE_ERROR;
            }
            CommonDef.COM_RET ret = SendAndReciveFile(ref msgData);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return ret;
            }
            if (!dataHelper.InitConfigDic(msgData))
            {
                return CommonDef.COM_RET.READ_FILE_ERROR;
            }
            return (CommonDef.COM_RET)int.Parse(dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ERROR_NO));
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        /// <returns></returns>
        public static CommonDef.COM_RET ModUserPsw(string userCode, string userPsw)
        {
            string msgData = "";
            DataHelper dataHelper = new DataHelper();
            if (!dataHelper.ClearAndAddConfig(out msgData,
                new Dictionary<DataHelper.CONFIG_KEYS, string>(){
                    {DataHelper.CONFIG_KEYS.FUNC_NO, ((int)DataHelper.FUNC_NAME.MOD_USER_PSW).ToString()},
                    {DataHelper.CONFIG_KEYS.USER_CODE, userCode},
                    {DataHelper.CONFIG_KEYS.USER_PSW, userPsw},
                }))
            {
                return CommonDef.COM_RET.CREATE_FILE_ERROR;
            }
            CommonDef.COM_RET ret = SendAndReciveFile(ref msgData);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return ret;
            }
            if (!dataHelper.InitConfigDic(msgData))
            {
                return CommonDef.COM_RET.READ_FILE_ERROR;
            }
            return (CommonDef.COM_RET)int.Parse(dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ERROR_NO));
        }

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="listDelUsers"></param>
        /// <returns></returns>
        public static CommonDef.COM_RET DelUserInfo(List<string> listDelUsers)
        {
            string users = "";
            foreach (string userCode in listDelUsers)
            {
                users += userCode + "\t";
            }

            string msgData = "";
            DataHelper dataHelper = new DataHelper();
            if (!dataHelper.ClearAndAddConfig(out msgData,
                new Dictionary<DataHelper.CONFIG_KEYS, string>(){
                    {DataHelper.CONFIG_KEYS.FUNC_NO, ((int)DataHelper.FUNC_NAME.DEL_USER_INFO).ToString()},
                    {DataHelper.CONFIG_KEYS.USER_CODE, users},
                }))
            {
                return CommonDef.COM_RET.CREATE_FILE_ERROR;
            }
            CommonDef.COM_RET ret = SendAndReciveFile(ref msgData);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return ret;
            }
            if (!dataHelper.InitConfigDic(msgData))
            {
                return CommonDef.COM_RET.READ_FILE_ERROR;
            }
            return (CommonDef.COM_RET)int.Parse(dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ERROR_NO));
        }
        /// <summary>
        /// 获取所有的角色信息
        /// </summary>
        /// <param name="dtAllRole"></param>
        /// <returns></returns>
        public static CommonDef.COM_RET GetAllRoleInfo(ref DataTable dtAllRole)
        {
            string msgData = "";
            DataHelper dataHelper = new DataHelper();
            if (!dataHelper.ClearAndAddConfig(out msgData,
                new Dictionary<DataHelper.CONFIG_KEYS, string>(){
                    {DataHelper.CONFIG_KEYS.FUNC_NO, ((int)DataHelper.FUNC_NAME.GET_ALL_ROLE_INFO).ToString()},
                }))
            {
                return CommonDef.COM_RET.CREATE_FILE_ERROR;
            }
            CommonDef.COM_RET ret = SendAndReciveFile(ref msgData);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return ret;
            }
            if (!dataHelper.InitConfigDic(msgData))
            {
                return CommonDef.COM_RET.READ_FILE_ERROR;
            }
            dtAllRole = GetDataTable(dataHelper);
            return CommonDef.COM_RET.RET_OK;
        }

        /// <summary>
        /// 获取用户与权限限制对照表
        /// </summary>
        /// <param name="dtAllRole"></param>
        /// <returns></returns>
        public static CommonDef.COM_RET GetRoleFunc(string roleId, ref DataTable dtAllRole)
        {
            string msgData = "";
            DataHelper dataHelper = new DataHelper();
            if (!dataHelper.ClearAndAddConfig(out msgData,
                new Dictionary<DataHelper.CONFIG_KEYS, string>(){
                    {DataHelper.CONFIG_KEYS.FUNC_NO, ((int)DataHelper.FUNC_NAME.GET_ROLE_AUTH_INFO).ToString()},
                    {DataHelper.CONFIG_KEYS.ROLE_ID, roleId},
                }))
            {
                return CommonDef.COM_RET.CREATE_FILE_ERROR;
            }
            CommonDef.COM_RET ret = SendAndReciveFile(ref msgData);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return ret;
            }
            if (!dataHelper.InitConfigDic(msgData))
            {
                return CommonDef.COM_RET.READ_FILE_ERROR;
            }
            dtAllRole = GetDataTable(dataHelper);
            return CommonDef.COM_RET.RET_OK;
        }

        /// <summary>
        /// 增加角色信息
        /// </summary>
        /// <param name="ruleName"></param>
        /// <param name="unAuthRoles"></param>
        /// <param name="p_2"></param>
        /// <returns></returns>
        public static CommonDef.COM_RET AddRoleInfo(string roleName, List<string> unAuthRoles, string remark)
        {
            string auth = "";
            foreach (string ruleAuth in unAuthRoles)
            {
                auth += ruleAuth + "\t";
            }
            string msgData = "";
            DataHelper dataHelper = new DataHelper();
            if (!dataHelper.ClearAndAddConfig(out msgData,
                new Dictionary<DataHelper.CONFIG_KEYS, string>(){
                    {DataHelper.CONFIG_KEYS.FUNC_NO, ((int)DataHelper.FUNC_NAME.ADD_ROLE_INFO).ToString()},
                    {DataHelper.CONFIG_KEYS.ROLE_NAME, roleName},
                    {DataHelper.CONFIG_KEYS.ROLE_AUTH, auth},
                    {DataHelper.CONFIG_KEYS.REMARK, remark},
                }))
            {
                return CommonDef.COM_RET.CREATE_FILE_ERROR;
            }
            CommonDef.COM_RET ret = SendAndReciveFile(ref msgData);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return ret;
            }
            if (!dataHelper.InitConfigDic(msgData))
            {
                return CommonDef.COM_RET.READ_FILE_ERROR;
            }
            return (CommonDef.COM_RET)int.Parse(dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ERROR_NO));
        }

        /// <summary>
        /// 修改角色信息
        /// </summary>
        /// <param name="ruleId"></param>
        /// <param name="ruleName"></param>
        /// <param name="unAuthRoles"></param>
        /// <param name="p_2"></param>
        /// <returns></returns>
        public static CommonDef.COM_RET ModRoleInfo(string roleId, string roleName, List<string> unAuthRoles, string remark)
        {
            string auth = "";
            foreach (string ruleAuth in unAuthRoles)
            {
                auth += ruleAuth + "\t";
            }
            string msgData = "";
            DataHelper dataHelper = new DataHelper();
            if (!dataHelper.ClearAndAddConfig(out msgData,
                new Dictionary<DataHelper.CONFIG_KEYS, string>(){
                    {DataHelper.CONFIG_KEYS.FUNC_NO, ((int)DataHelper.FUNC_NAME.MOD_ROLE_INFO).ToString()},
                    {DataHelper.CONFIG_KEYS.ROLE_ID, roleId},
                    {DataHelper.CONFIG_KEYS.ROLE_NAME, roleName},
                    {DataHelper.CONFIG_KEYS.ROLE_AUTH, auth},
                    {DataHelper.CONFIG_KEYS.REMARK, remark},
                }))
            {
                return CommonDef.COM_RET.CREATE_FILE_ERROR;
            }
            CommonDef.COM_RET ret = SendAndReciveFile(ref msgData);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return ret;
            }
            if (!dataHelper.InitConfigDic(msgData))
            {
                return CommonDef.COM_RET.READ_FILE_ERROR;
            }
            return (CommonDef.COM_RET)int.Parse(dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ERROR_NO));
        }

        /// <summary>
        /// 删除角色信息
        /// </summary>
        /// <param name="listDelRoles"></param>
        /// <returns></returns>
        public static CommonDef.COM_RET DelRoleInfo(List<string> listDelRoles)
        {
            string roles = "";
            foreach (string roleId in listDelRoles)
            {
                roles += roleId + "\t";
            }
            string msgData = "";
            DataHelper dataHelper = new DataHelper();
            if (!dataHelper.ClearAndAddConfig(out msgData,
                new Dictionary<DataHelper.CONFIG_KEYS, string>(){
                    {DataHelper.CONFIG_KEYS.FUNC_NO, ((int)DataHelper.FUNC_NAME.DEL_ROLE_INFO).ToString()},
                    {DataHelper.CONFIG_KEYS.ROLE_ID, roles},
                }))
            {
                return CommonDef.COM_RET.CREATE_FILE_ERROR;
            }
            CommonDef.COM_RET ret = SendAndReciveFile(ref msgData);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return ret;
            }
            if (!dataHelper.InitConfigDic(msgData))
            {
                return CommonDef.COM_RET.READ_FILE_ERROR;
            }
            return (CommonDef.COM_RET)int.Parse(dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ERROR_NO));
        }
        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="listDelUsers"></param>
        /// <returns></returns>
        public static CommonDef.COM_RET ReSetUserPsw(List<string> listDelUsers, ref string defaultPsw)
        {
            string users = "";
            foreach (string userCode in listDelUsers)
            {
                users += userCode + "\t";
            }
            string msgData = "";
            DataHelper dataHelper = new DataHelper();
            if (!dataHelper.ClearAndAddConfig(out msgData,
                new Dictionary<DataHelper.CONFIG_KEYS, string>(){
                    {DataHelper.CONFIG_KEYS.FUNC_NO, ((int)DataHelper.FUNC_NAME.RESET_USER_PSW).ToString()},
                    {DataHelper.CONFIG_KEYS.USER_CODE, users},
                }))
            {
                return CommonDef.COM_RET.CREATE_FILE_ERROR;
            }
            CommonDef.COM_RET ret = SendAndReciveFile(ref msgData);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return ret;
            }
            if (!dataHelper.InitConfigDic(msgData))
            {
                return CommonDef.COM_RET.READ_FILE_ERROR;
            }
            defaultPsw = dataHelper.GetConfig(DataHelper.CONFIG_KEYS.USER_PSW);
            return (CommonDef.COM_RET)int.Parse(dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ERROR_NO));
        }

        /// <summary>
        /// 通用指令类
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="funcNo"></param>
        /// <param name="delClients"></param>
        /// <returns></returns>
        public static CommonDef.COM_RET CommonSend(string nameSpace, int funcNo, string message)
        {
            string msgData = "";
            DataHelper dataHelper = new DataHelper();
            if (!dataHelper.ClearAndAddConfig(out msgData,
                new Dictionary<DataHelper.CONFIG_KEYS, string>(){
                    {DataHelper.CONFIG_KEYS.FUNC_NO, ((int)DataHelper.FUNC_NAME.PLANTFROM_COM).ToString()},
                    {DataHelper.CONFIG_KEYS.PLATFORM_NAME, nameSpace},
                    {DataHelper.CONFIG_KEYS.PLATFORM_FUNCNO, funcNo.ToString()},
                    {DataHelper.CONFIG_KEYS.PLATFORM_MSG, message},
                }))
            {
                return CommonDef.COM_RET.CREATE_FILE_ERROR;
            }
            CommonDef.COM_RET ret = SendAndReciveFile(ref msgData);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return ret;
            }
            if (!dataHelper.InitConfigDic(msgData))
            {
                return CommonDef.COM_RET.READ_FILE_ERROR;
            }
            return (CommonDef.COM_RET)int.Parse(dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ERROR_NO));
        }
        /// <summary>
        /// 通用查询类
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="funcNo"></param>
        /// <param name="tbAllClient"></param>
        /// <returns></returns>
        public static CommonDef.COM_RET CommonRead(string nameSpace, int funcNo, ref DataTable dt, string message = "")
        {
            string msgData = "";
            DataHelper dataHelper = new DataHelper();
            if (!dataHelper.ClearAndAddConfig(out msgData,
                new Dictionary<DataHelper.CONFIG_KEYS, string>(){
                    {DataHelper.CONFIG_KEYS.FUNC_NO, ((int)DataHelper.FUNC_NAME.PLANTFROM_COM).ToString()},
                    {DataHelper.CONFIG_KEYS.PLATFORM_NAME, nameSpace},
                    {DataHelper.CONFIG_KEYS.PLATFORM_FUNCNO, funcNo.ToString()},
                    {DataHelper.CONFIG_KEYS.PLATFORM_MSG, message},
                }))
            {
                return CommonDef.COM_RET.CREATE_FILE_ERROR;
            }
            CommonDef.COM_RET ret = SendAndReciveFile(ref msgData);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return ret;
            }
            if (!dataHelper.InitConfigDic(msgData))
            {
                return CommonDef.COM_RET.READ_FILE_ERROR;
            }
            dt = GetDataTable(dataHelper);
            return (CommonDef.COM_RET)int.Parse(dataHelper.GetConfig(DataHelper.CONFIG_KEYS.ERROR_NO));
        }
    }
}
