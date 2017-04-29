using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;
using DataBaseManager.DataTables;
using System.IO;
using System.Reflection;

namespace DataBaseManager
{
    public class DataBaseHelper
    {
        /// <summary>
        /// 解析处理文件
        /// </summary>
        /// <param name="msgData"></param>
        /// <returns></returns>
        public static void ProcessFile(ref string msgData)
        {
            DataHelper dataHelper = new DataHelper();
            try
            {
                if (!dataHelper.InitConfigDic(msgData))
                {
                    SetErrorNo(out msgData, dataHelper, CommonDef.COM_RET.SERVER_READ_FILE_ERROR);
                    return;
                }
                DataHelper.FUNC_NAME func = (DataHelper.FUNC_NAME)int.Parse(
                    dataHelper.GetConfig(DataHelper.CONFIG_KEYS.FUNC_NO));
                CommonDef.COM_RET ret;
                switch (func)
                {
                    case DataHelper.FUNC_NAME.GET_USR_LIST:
                        ret = DataBaseSelectFunc.GetUserList(dataHelper);
                        break;
                    case DataHelper.FUNC_NAME.GET_USER_INFO:
                        ret = DataBaseSelectFunc.GetUserInfo(dataHelper);
                        break;
                    case DataHelper.FUNC_NAME.GET_ALL_USER_INFO:
                        ret = DataBaseSelectFunc.GetAllUserInfo(dataHelper);
                        break;
                    case DataHelper.FUNC_NAME.ADD_USER_INFO:
                        ret = DataBaseSelectFunc.AddUserInfo(dataHelper);
                        break;
                    case DataHelper.FUNC_NAME.MOD_USER_INFO:
                        ret = DataBaseSelectFunc.ModUserInfo(dataHelper);
                        break;
                    case DataHelper.FUNC_NAME.MOD_USER_PSW:
                        ret = DataBaseSelectFunc.ModUserPsw(dataHelper);
                        break;
                    case DataHelper.FUNC_NAME.DEL_USER_INFO:
                        ret = DataBaseSelectFunc.DelUserInfo(dataHelper);
                        break;
                    case DataHelper.FUNC_NAME.RESET_USER_PSW:
                        ret = DataBaseSelectFunc.ResetUserPsw(dataHelper);
                        break;
                    case DataHelper.FUNC_NAME.GET_ALL_ROLE_INFO:
                        ret = DataBaseSelectFunc.GetAllRoleInfo(dataHelper);
                        break;
                    case DataHelper.FUNC_NAME.GET_ROLE_AUTH_INFO:
                        ret = DataBaseSelectFunc.GetRoleAuthInfo(dataHelper);
                        break;
                    case DataHelper.FUNC_NAME.ADD_ROLE_INFO:
                        ret = DataBaseSelectFunc.AddRoleAuthInfo(dataHelper);
                        break;
                    case DataHelper.FUNC_NAME.DEL_ROLE_INFO:
                        ret = DataBaseSelectFunc.DelRoleInfo(dataHelper);
                        break;
                    case DataHelper.FUNC_NAME.MOD_ROLE_INFO:
                        ret = DataBaseSelectFunc.ModRoleInfo(dataHelper);
                        break;
                    case DataHelper.FUNC_NAME.PLANTFROM_COM:
                        ret = DataBaseSelectFunc.PlantfromCom(dataHelper);
                        break;
                    default:
                        SetErrorNo(out msgData, dataHelper, CommonDef.COM_RET.FUNC_NO_ERROR);
                        return;
                }
                if (ret != CommonDef.COM_RET.RET_OK)
                {
                    SetErrorNo(out msgData, dataHelper, ret);
                    return;
                }
                dataHelper.AddConfig(DataHelper.CONFIG_KEYS.ERROR_NO, ((int)ret).ToString());
                dataHelper.SaveConfig(out msgData);
            }
            catch
            {
                SetErrorNo(out msgData, dataHelper, CommonDef.COM_RET.SERVER_ERROR);
                return;
            }
        }

        private static void SetErrorNo(out string msgData, DataHelper dataHelper, CommonDef.COM_RET ComRet)
        {
            dataHelper.ClearAndAddConfig(out msgData,
                new Dictionary<DataHelper.CONFIG_KEYS, string>() 
                        {
                            {DataHelper.CONFIG_KEYS.ERROR_NO, ((int)ComRet).ToString()}
                        });
        }

        public static CommonDef.COM_RET DataBaseInit()
        {
            //初始化数据库连接
            CommonDef.COM_RET ret = DataTableTool.InitDataBase();
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return ret;
            }
            if (!T_USER_INFO.InitTable()) return CommonDef.COM_RET.DATABASE_ERROR;
            if (!T_ROLE_INFO.InitTable()) return CommonDef.COM_RET.DATABASE_ERROR;
            if (!T_ROLE_AUTH_INFO.InitTable()) return CommonDef.COM_RET.DATABASE_ERROR;

            if (!InitExpandDataBase()) return CommonDef.COM_RET.DATABASE_ERROR;
            return CommonDef.COM_RET.RET_OK;
        }

        private static bool InitExpandDataBase()
        {
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
                    string className = file + "." + file + "Server";
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
                    ExpandBaseServer expand = obj as ExpandBaseServer;
                    return expand.InitDataBase();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
    }
}
