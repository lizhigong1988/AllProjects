using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.IO;

namespace CommonLib
{
    public class DataHelper
    {
        public enum FUNC_NAME
        {
            COUNT_USER_INFO,
            GET_USER_INFO,
            GET_ALL_USER_INFO,
            ADD_USER_INFO,
            MOD_USER_INFO,
            MOD_USER_PSW,
            DEL_USER_INFO,
            RESET_USER_PSW,
            GET_ALL_ROLE_INFO,
            GET_ROLE_AUTH_INFO,
            ADD_ROLE_INFO,
            MOD_ROLE_INFO,
            DEL_ROLE_INFO,
            PLANTFROM_COM,
        }

        /// <summary>
        /// 定义配置项
        /// </summary>
        public enum CONFIG_KEYS
        {
            FUNC_NO,  //功能编号
            ERROR_NO,   //错误码
            USER_CODE,//用户名
            USER_NAME,//用户姓名
            USER_PSW, //用户密码
            USER_TEL, //电话号码
            USER_ID, //身份证号
            ROLE_ID, //角色ID
            ROLE_NAME, //角色名称
            ROLE_AUTH,  //角色权限
            SELECT_TABLE, //查询表格
            REMARK,     //备注
            PLATFORM_NAME,//通用分类
            PLATFORM_FUNCNO,//通用方法代码
            PLATFORM_MSG,//通用消息
            TOTAL_COUNT,
        };

        /// <summary>
        /// KLV中KEY值长度
        /// </summary>
        private static readonly int KEY_LENGH = 3;

        /// <summary>
        /// KLV中L值长度
        /// </summary>
        private static readonly int LEN_LENGH = 9;

        /// <summary>
        /// 全部配置项
        /// </summary>
        private Dictionary<int, string> dicConfig = new Dictionary<int, string>();


        /// <summary>
        /// 初始化配置项
        /// </summary>
        /// <param name="fileInfo"></param>
        public bool InitConfigDic(string MsgInfo)
        {
            string fileInfo = MsgInfo;
            byte[] fileByte = Convert.FromBase64String(fileInfo);
            fileInfo = Encoding.Default.GetString(fileByte);
            dicConfig.Clear();
            string dicInfo = fileInfo;
            while (dicInfo.Length >= KEY_LENGH + LEN_LENGH)
            {
                int keyValue = (int)CONFIG_KEYS.TOTAL_COUNT;
                if (!int.TryParse(dicInfo.Substring(0, KEY_LENGH), out keyValue))
                {
                    break;
                }
                int valueLengh = -1;
                if (!int.TryParse(dicInfo.Substring(KEY_LENGH, LEN_LENGH), out valueLengh))
                {
                    break;
                }
                if (valueLengh < 0)
                {
                    break;    
                }
                if (dicInfo.Length < KEY_LENGH + LEN_LENGH + valueLengh)
                {
                    break;
                }
                string value = "";
                if (valueLengh > 0)
                {
                    value = dicInfo.Substring(KEY_LENGH + LEN_LENGH, valueLengh);
                }
                dicConfig.Add(keyValue, value);
                dicInfo = dicInfo.Substring(KEY_LENGH + LEN_LENGH + valueLengh);
            }
            return true;
        }

        /// <summary>
        /// 增加、替换配置项
        /// </summary>
        /// <param name="cONFIG_KEYS"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool AddConfig(Dictionary<CONFIG_KEYS, string> dicValues)
        {
            foreach (var value in dicValues)
            {
                if (ContainsKey(value.Key))
                {
                    dicConfig[(int)value.Key] = value.Value;
                }
                else
                {
                    dicConfig.Add((int)value.Key, value.Value);
                }
            }
            return true;
        }

        /// <summary>
        /// 增加、替换配置项
        /// </summary>
        /// <param name="cONFIG_KEYS"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool AddConfig(CONFIG_KEYS key, string value)
        {
            if (ContainsKey(key))
            {
                dicConfig[(int)key] = value;
            }
            else
            {
                dicConfig.Add((int)key, value);
            }
            return true;
        }

        /// <summary>
        /// 增加、替换配置项
        /// </summary>
        /// <param name="cONFIG_KEYS"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool ClearAndAddConfig(out string msgData, Dictionary<CONFIG_KEYS, string> dicValues)
        {
            dicConfig.Clear();
            if (!AddConfig(dicValues))
            {
                msgData = "";
                return false;
            }
            return SaveConfig(out msgData);
        }

        /// <summary>
        /// 保存配置项
        /// </summary>
        /// <returns></returns>
        public bool SaveConfig(out string msgData)
        {
            Dictionary<int, string> dicAsc = dicConfig.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            dicConfig = dicAsc;
            string saveFileInfo = "";
            foreach (var dic in dicConfig)
            {
                if (dic.Key.ToString().Length > KEY_LENGH || dic.Value.Length.ToString().Length > LEN_LENGH)
                {
                    continue;
                }
                saveFileInfo += dic.Key.ToString().PadLeft(KEY_LENGH, '0');
                saveFileInfo += dic.Value.Length.ToString().PadLeft(LEN_LENGH, '0');
                saveFileInfo += dic.Value;
            }
            byte[] bytedata = Encoding.Default.GetBytes(saveFileInfo);
            saveFileInfo = Convert.ToBase64String(bytedata, 0, bytedata.Length);
            msgData = saveFileInfo;
            return true;
        }

        /// <summary>
        /// 判断配置项
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool ContainsKey(CONFIG_KEYS key)
        {
            return dicConfig.ContainsKey((int)key);
        }

        /// <summary>
        /// 返回配置值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetConfig(CONFIG_KEYS key)
        {
            if (!ContainsKey(key))
            {
                return "";
            }
            return dicConfig[(int)key];
        }
    }
}
