using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Data;
using CommonLib;
using ProjectsManageServer.Connect;
using System.IO;
using ProjectsManageServer.DataBases;
using System.Net.Mail;

namespace ProjectsManageServer.Connect
{
    class TimerThread
    {
        static string ERRORLOG = MiddleService.LOG_PATH + "\\" + "ERROR_LOG";
        static string sendName = "";
        static string sendEmail = "";
        static string sendPsw = "";
        static string sendHost = "";
        static string sendPMTime = "";
        static string sendALLTime = "";
        static string lastSendPmDate = "";
        static string lastSendAllDate = "";
        static string date = "";
        static string time = "";

        static bool TestFlag = false;
        internal static bool TestEmail(string senderName, string senderEmail,
            string senderPsw, string senderServer)
        {
            TestFlag = true;
            sendName = senderName;
            sendEmail = senderEmail;
            sendPsw = senderPsw;
            sendHost = senderServer;
            try
            {
                SendPmDaily();
                SendAllDaily();
            }
            catch
            {
                TestFlag = false;
                return false;
            }
            TestFlag = false;
            return true;
        }

        public static void TimerRefresh()
        {
            #region 获取系统配置
            DataTable dt = DataBases.DataBaseManager.GetSysConfig();
            sendName = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_NAME);
            sendEmail = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_ADDR);
            sendPsw = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_PASSWORD);
            sendHost = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_HOST);
            sendPMTime = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_PM_TIME);
            sendALLTime = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_ALL_TIME);
            lastSendPmDate = GetConfig(dt, CommonDef.CONFIG_KEYS.LAST_SEND_PM);
            lastSendAllDate = GetConfig(dt, CommonDef.CONFIG_KEYS.LAST_SEND_ALL);
            string sendFlag = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_FLAG);
            if (sendFlag != "1" || sendName == "" || sendEmail == "" || 
                sendPsw == "" || sendHost == "")//1允许发邮件
            {
                return;
            }
            date = DateTime.Now.ToString("yyyyMMdd");
            time = DateTime.Now.ToString("HHmmss");
            #endregion
            #region 发送各系统项目信息
            if (sendPMTime != "")
            {
                if (time.CompareTo(sendPMTime) > 0)//超过发送项目经理的时间点
                {
                    if (lastSendPmDate == "")
                    {
                        lastSendPmDate = "00000000";
                    }
                    if (date.CompareTo(lastSendPmDate) > 0)//超过最后一次发送的日期
                    {
                        SendPmDaily();
                        //3、更新lastSendPmDate
                        DataBaseManager.SaveSysConfig(dt, CommonDef.CONFIG_KEYS.LAST_SEND_PM, date);
                    }
                }
            }
            #endregion
            #region 发送全部项目汇总信息
            if (sendALLTime != "")
            {
                if (time.CompareTo(sendALLTime) > 0)//超过发送汇总信息的时间点
                {
                    if (lastSendAllDate == "")
                    {
                        lastSendAllDate = "00000000";
                    }
                    if (date.CompareTo(lastSendAllDate) > 0)//超过最后一次发送的日期
                    {
                        SendAllDaily();
                        //6、更新lastSendAllDate
                        DataBaseManager.SaveSysConfig(dt, CommonDef.CONFIG_KEYS.LAST_SEND_ALL, date);

                        //7、备份系统数据
                        if (!File.Exists(DataBaseTool_SQLite3.DATABASE_FILE + date))
                        {
                            File.Copy(DataBaseTool_SQLite3.DATABASE_FILE, DataBaseTool_SQLite3.DATABASE_FILE + date);
                        }
                    }
                }
            }
            #endregion
        }

        private static bool AddProMsgInfo(DataRow drPro, ref string msg)
        {
            string proId = drPro["DEMAND_ID"].ToString();
            msg += HtmAddRow("项目名称：" + drPro["DEMAND_NAME"].ToString());
            msg += HtmAddRow("需求部门：" + drPro["DEMAND_DEPART"].ToString());
            msg += HtmAddRow("项目性质：" + drPro["PRO_KIND"].ToString());
            msg += HtmAddRow("项目阶段：" + drPro["PRO_STAGE"].ToString());
            msg += HtmAddRow("进度信息：");

            DataTable dtProSysInfo = DataBaseManager.GetProSystemInfo(proId);
            if (dtProSysInfo == null)
            {
                //获取系统信息失败，终止任务
                File.AppendAllText(ERRORLOG, "SEND_PM_EMAIL:读取项目系统信息失败\r\n");
                return false;
            }
            DataTable dtRateInfo = DataBaseManager.GetProRateInfo(proId);
            if (dtRateInfo == null)
            {
                //获取系统信息失败，终止任务
                File.AppendAllText(ERRORLOG, "SEND_PM_EMAIL:读取项目进度信息失败\r\n");
                return false;
            }
            DataTable dtProSysRateInfo = new DataTable();
            dtProSysRateInfo.Columns.Add("SYS_ID");
            dtProSysRateInfo.Columns.Add("SYS_NAME");
            dtProSysRateInfo.Columns.Add("ESTIMATE_DAYS");
            dtProSysRateInfo.Columns.Add("RATE");
            dtProSysRateInfo.Columns.Add("DATE");
            dtProSysRateInfo.Columns.Add("EXPLAIN");
            dtProSysRateInfo.Columns.Add("PROBLEM");
            foreach (DataRow drProSys in dtProSysInfo.Rows)
            {
                dtProSysRateInfo.Rows.Add(new string[]
                                            {
                                                drProSys["SYS_ID"].ToString(),
                                                drProSys["SYS_NAME"].ToString(),
                                                drProSys["ESTIMATE_DAYS"].ToString(),
                                                "",
                                                "",
                                                "",
                                                "",
                                            });
            }
            foreach (DataRow drRate in dtRateInfo.Rows)
            {
                foreach (DataRow drProSys in dtProSysRateInfo.Rows)
                {
                    if (drRate["SYS_ID"].ToString() == drProSys["SYS_ID"].ToString())
                    {
                        if (drProSys["DATE"].ToString() == "")
                        {
                            drProSys["RATE"] = drRate["RATE"].ToString() + "%";
                            drProSys["DATE"] = drRate["DATE"].ToString();
                            drProSys["EXPLAIN"] = drRate["EXPLAIN"].ToString();
                            drProSys["PROBLEM"] = drRate["PROBLEM"].ToString();
                        }
                        else if (drProSys["DATE"].ToString().CompareTo(drRate["DATE"].ToString()) < 0)
                        {
                            drProSys["RATE"] = drRate["RATE"].ToString() + "%";
                            drProSys["DATE"] = drRate["DATE"].ToString();
                            drProSys["EXPLAIN"] = drRate["EXPLAIN"].ToString();
                            drProSys["PROBLEM"] = drRate["PROBLEM"].ToString();
                        }
                        break;
                    }
                }
            }
            msg += HtmAddTable(dtProSysRateInfo, new Dictionary<string, string>() 
                                    {
                                        {"SYS_NAME","录入系统"},
                                        {"ESTIMATE_DAYS", "预计工作量"},
                                        {"RATE","最新录入进度"},
                                        {"DATE","最新录入日期"},
                                        {"EXPLAIN","进度说明"},
                                        {"PROBLEM","面临问题"},
                                    });
            msg += HtmAddRow("");
            return true;
        }

        private static void SendAllDaily()
        {
            //发送项目汇总日报
            //1、汇总需求总数、新需求数目、维护升级数目
            DataTable dtProInfo = DataBaseManager.QueryProInfo("全部", "全部未完成", "", "");
            if (dtProInfo == null)
            {
                //获取系统信息失败，终止任务
                File.AppendAllText(ERRORLOG, "SEND_ALL_EMAIL:读取项目信息失败\r\n");
                return;
            }
            int totalCount = dtProInfo.Rows.Count;
            int newCount = 0;
            int updateCount = 0;

            DataTable dtDepartProInfo = new DataTable();//按部门统计表
            dtDepartProInfo.Columns.Add("DEMAND_DEPART");
            dtDepartProInfo.Columns.Add("TOTAL_COUNT");
            dtDepartProInfo.Columns.Add("NEW_PRO");
            dtDepartProInfo.Columns.Add("UPDATE_PRO");

            foreach (DataRow dr in dtProInfo.Rows)
            {
                if (dr["PRO_KIND"].ToString() == "新项目")
                {
                    newCount++;
                }
                else
                {
                    updateCount++;
                }
                bool has = false;
                foreach (DataRow drDepart in dtDepartProInfo.Rows)
                {
                    if (drDepart["DEMAND_DEPART"].ToString() == dr["DEMAND_DEPART"].ToString())
                    {
                        has = true;
                        drDepart["TOTAL_COUNT"] = (int.Parse(drDepart["TOTAL_COUNT"].ToString()) + 1).ToString();
                        if (dr["PRO_KIND"].ToString() == "新项目")
                        {
                            drDepart["NEW_PRO"] = (int.Parse(drDepart["NEW_PRO"].ToString()) + 1).ToString();
                        }
                        else
                        {
                            drDepart["UPDATE_PRO"] = (int.Parse(drDepart["UPDATE_PRO"].ToString()) + 1).ToString();
                        }
                        break;
                    }
                }
                if (!has)
                {
                    if (dr["PRO_KIND"].ToString() == "新项目")
                    {
                        dtDepartProInfo.Rows.Add(new string[] { dr["DEMAND_DEPART"].ToString(), "1", "1", "0" });
                    }
                    else
                    {
                        dtDepartProInfo.Rows.Add(new string[] { dr["DEMAND_DEPART"].ToString(), "1", "0", "1" });
                    }
                }
            }
            string msg = "";
            msg += HtmAddRow("一、信息技术部全部在建需求数：" + totalCount.ToString() + "个，其中:");
            msg += HtmAddRow("    新建项目：" + newCount.ToString() + "个，系统升级类需求:" + updateCount.ToString() + "个。");
            msg += HtmAddRow("");
            //2、按照部门汇总需求总数、新需求数目、维护升级数目
            msg += HtmAddRow("二、各业务部门在建需求汇总：");
            msg += HtmAddTable(dtDepartProInfo, new Dictionary<string, string>() 
                                    {
                                        {"DEMAND_DEPART","部门名称"},
                                        {"TOTAL_COUNT", "需求总数"},
                                        {"NEW_PRO","新需求个数"},
                                        {"UPDATE_PRO","升级需求个数"},
                                    });
            msg += HtmAddRow("");
            //3、汇总新需求（重点项目）详情
            /*
             * 项目名称
             * 系统名称、预估工作量、系统开发进度、进度说明
             */
            msg += HtmAddRow("三、汇总新需求（重点项目）详情：");

            foreach (DataRow drPro in dtProInfo.Rows)
            {
                if (drPro["PRO_KIND"].ToString() == "新项目")
                {
                    if (!AddProMsgInfo(drPro, ref msg))
                    {
                        return;
                    }
                }
            }
            //4、汇总维护需求详情
            /*
             * 项目名称
             * 系统名称、预估工作量、系统开发进度、进度说明
             */
            msg += HtmAddRow("四、汇总升级维护需求详情：");
            foreach (DataRow drPro in dtProInfo.Rows)
            {
                if (drPro["PRO_KIND"].ToString() != "新项目")
                {
                    if (!AddProMsgInfo(drPro, ref msg))
                    {
                        return;
                    }
                }
            }
            //5、发送日报
            #region 发送
            //给个系统项目经理发送项目日报
            DataTable dtUserInfo = DataBaseManager.GetUserInfo();
            if (dtUserInfo == null)
            {
                //获取系统信息失败，终止任务
                File.AppendAllText(ERRORLOG, "SEND_PM_EMAIL:读取用户信息失败\r\n");
                return;
            }
            string recvUsers = "";
            string ccUsers = "";
            foreach (DataRow drUser in dtUserInfo.Rows)
            {
                if (drUser["EMAIL"].ToString() == "")
                {
                    continue;
                }
                if (drUser["USER_ROLE"].ToString() == "项目经理")
                {
                    ccUsers += drUser["USER_NAME"].ToString() + "<" + drUser["EMAIL"].ToString() + ">;";
                    continue;
                }
                if (drUser["USER_ROLE"].ToString() == "PMO")
                {
                    ccUsers += drUser["USER_NAME"].ToString() + "<" + drUser["EMAIL"].ToString() + ">;";
                    continue;
                }
                if (drUser["USER_ROLE"].ToString() == "部门领导")
                {
                    recvUsers += drUser["USER_NAME"].ToString() + "<" + drUser["EMAIL"].ToString() + ">;";
                    continue;
                }
            }
            if (recvUsers != "")
            {
                string sendTitle = "信息技术部项目日报";
                if (!SendEmail(recvUsers, sendEmail, sendName, ccUsers, sendPsw, sendHost, sendTitle, msg, true))
                {
                    File.AppendAllText(ERRORLOG, "SEND_PM_EMAIL:发送项目汇总邮件失败\r\n");
                }
            }
            #endregion
        }

        private static void SendPmDaily()
        {
            //给个系统项目经理发送项目日报
            DataTable dtUserInfo = DataBaseManager.GetUserInfo();
            if (dtUserInfo == null)
            {
                //获取系统信息失败，终止任务
                File.AppendAllText(ERRORLOG, "SEND_PM_EMAIL:读取用户信息失败\r\n");
                return;
            }
            DataTable dtUserSysInfo = DataBaseManager.GetUserSysInfo();
            if (dtUserSysInfo == null)
            {
                //获取系统信息失败，终止任务
                File.AppendAllText(ERRORLOG, "SEND_PM_EMAIL:读取用户信息失败\r\n");
                return;
            }

            //1、查出全部系统
            DataTable dtSystems = DataBaseManager.GetSystemInfo();
            if (dtSystems == null)
            {
                //获取系统信息失败，终止任务
                File.AppendAllText(ERRORLOG, "SEND_PM_EMAIL:读取系统信息失败\r\n");
                return;
            }
            //2、逐个系统发送项目进度信息
            /*
             * 项目名称、性质
             * 系统名称、预估工作量、系统开发进度、进度说明
             */
            foreach (DataRow drSys in dtSystems.Rows)
            {
                //组项目邮件信息 html格式
                string msg = "";
                string sysId = drSys["SYS_ID"].ToString();
                DataTable dtProInfo = DataBaseManager.QueryProInfo("全部", "全部未完成", "",
                    sysId);
                if (dtProInfo == null)
                {
                    //获取系统信息失败，终止任务
                    File.AppendAllText(ERRORLOG, "SEND_PM_EMAIL:读取项目信息失败\r\n");
                    return;
                }
                #region 汇总当前系统关联的每个项目
                foreach (DataRow drPro in dtProInfo.Rows)
                {
                    if (!AddProMsgInfo(drPro, ref msg))
                    {
                        return;
                    }
                }
                #endregion
                #region 发送
                string recvUsers = "";
                foreach (DataRow drUser in dtUserInfo.Rows)
                {
                    if (drUser["EMAIL"].ToString() == "")
                    {
                        continue;
                    }
                    if (drUser["USER_ROLE"].ToString() != "项目经理")
                    {
                        continue;
                    }
                    foreach (DataRow drUserSys in dtUserSysInfo.Rows)
                    {
                        if (drUser["USER_NAME"].ToString() == drUserSys["USER_NAME"].ToString())
                        {
                            if (drUserSys["SYS_ID"].ToString() == sysId)
                            {
                                recvUsers += drUser["USER_NAME"].ToString() + "<" + drUser["EMAIL"].ToString() + ">;";
                                break;
                            }
                        }
                    }
                }
                if (recvUsers == "" || msg == "")
                {
                    continue;
                }
                string sendTitle = drSys["SYS_NAME"].ToString() + "项目日报";
                if (!SendEmail(recvUsers, sendEmail, sendName, "", sendPsw, sendHost, sendTitle, msg, true))
                {
                    File.AppendAllText(ERRORLOG, "SEND_PM_EMAIL:发送项目系统邮件失败\r\n");
                    continue;
                }
                #endregion
            }
        }

        private static bool SendEmail(string recv, string send, string sendName, string cc, 
            string sendPsw, string server, string title, string content, bool htm)
        {
            string sendLog = "";
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            if (TestFlag)
            {
                msg.To.Add(send);//收件人邮箱   
                msg.CC.Add(send);//李志功<lizhigong@hkcts.com>
            }
            else
            {
                string[] recvUsers = recv.Split(';');
                foreach (string user in recvUsers)
                {
                    if (user == "")
                    {
                        continue;
                    }
                    msg.To.Add(user);//收件人邮箱   
                }
                string[] ccUsers = cc.Split(';');
                foreach (string user in ccUsers)
                {
                    if (user == "")
                    {
                        continue;
                    }
                    msg.CC.Add(user);//李志功<lizhigong@hkcts.com>
                }
            }
            msg.From = new MailAddress(send, sendName, System.Text.Encoding.UTF8);
            /* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
            msg.Subject = title;//邮件标题    
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码    
            msg.Body = content;//邮件内容    
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码    
            msg.IsBodyHtml = htm;//是否是HTML邮件   
            //msg.Priority = MailPriority.High;//邮件优先级    
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(send, sendPsw);
            client.Host = server;//smtp.hkcts.com
            object userState = msg;
            try
            {
                client.SendAsync(msg, userState);
                //简单一点儿可以client.Send(msg); 
                sendLog += "发件人：" + sendName + "<" + sendEmail + ">\r\n";
                sendLog += "收件人：" + recv + "\r\n";
                sendLog += "抄送：" + cc + "\r\n";
                sendLog += "内容：\r\n" + content + "\r\n";
                File.AppendAllText("SendLog" + DateTime.Now.ToString("yyyyMMdd") + ".txt", sendLog);
                return true;
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                return false;
            }
        }

        private static string HtmAddTable(DataTable dtRateInfo, Dictionary<string, string> dictionary)
        {
            string tableInfo = "";
            tableInfo += "<table style=\"width:100%;\" cellpadding=\"2\" cellspacing=\"0\" border=\"1\" bordercolor=\"#000000\">";
            tableInfo += "<tbody>";
            tableInfo += "<tr>";
            foreach(DataColumn dc in dtRateInfo.Columns)
            {
                if(!dictionary.ContainsKey(dc.ColumnName))
                {
                    continue;
                }
                tableInfo += "<td>" + ChangeHtmStr(dictionary[dc.ColumnName]) + "</td>";
            }
            tableInfo += "</tr>";
            foreach(DataRow dr in dtRateInfo.Rows)
            {
                tableInfo += "<tr>";
                foreach(DataColumn dc in dtRateInfo.Columns)
                {
                    if(!dictionary.ContainsKey(dc.ColumnName))
                    {
                        continue;
                    }
                    tableInfo += "<td>" + ChangeHtmStr(dr[dc.ColumnName].ToString()) + "</td>";
                }
                tableInfo += "</tr>";
            }
            tableInfo += "</tbody>";
            tableInfo += "</table>";
            return tableInfo;
        }

        //private static Dictionary<string, string> dicChangeHtm = new Dictionary<string, string>()
        //{
        //    {"<br/>","\r\n"},
        //};

        private static string ChangeHtmStr(string str)
        {
            //foreach (var dic in dicChangeHtm)
            //{
            //    str = str.Replace(dic.Key, dic.Value);
            //}
            return str;
        }

        private static string HtmAddRow(string row)
        {
            return "<p>" + row + "</p>";
        }

        private static string GetConfig(DataTable dt, CommonDef.CONFIG_KEYS key)
        {
            string keyIndex = ((int)key).ToString();
            foreach (DataRow dr in dt.Rows)
            {
                if (keyIndex == dr["KEY"].ToString())
                {
                    return dr["VALUE"].ToString();
                }
            }
            return "";
        }
    }
}
