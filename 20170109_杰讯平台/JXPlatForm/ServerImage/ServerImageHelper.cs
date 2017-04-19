using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;
using DataBaseManager;

namespace ServerImage
{
    /// <summary>
    /// 服务端镜像，用于单机版
    /// </summary>
    public class ServerImageHelper
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public static CommonDef.COM_RET Init()
        {
            //初始化数据库连接以及内置表初始化
            CommonDef.COM_RET ret = DataBaseHelper.DataBaseInit();
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return ret;
            }

            //根据配置文件初始化serverexpand
            return CommonDef.COM_RET.RET_OK;
        }

        /// <summary>
        /// 文件解析
        /// </summary>
        /// <param name="fileName"></param>
        public static void ProcessFile(ref string msgData)
        {
            DataBaseHelper.ProcessFile(ref msgData);
        }
    }
}
