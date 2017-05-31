﻿using System;
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

namespace ProjectsManageServer
{
    class TimerThread
    {
        /// <summary>
        /// 定时器
        /// </summary>
        static Timer timer = new Timer();

        public static void InitThread()
        {
            timer.Elapsed += new ElapsedEventHandler(TimerRefresh);
            //1分钟
            timer.Interval = 1000 * 60;
            timer.Start();
        }

        public static void TimerRefresh(object source, ElapsedEventArgs e)
        {
            DataTable dt = DataBases.DataBaseManager.GetSysConfig();
            string sendName = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_NAME);
            string sendEmail = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_ADDR);
            string sendPsw = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_PASSWORD);
            string sendHost = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_EMAIL_HOST);
            string sendPMTime = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_PM_TIME);
            string sendALLTime = GetConfig(dt, CommonDef.CONFIG_KEYS.SEND_ALL_TIME);
            string lastSendPmDate = GetConfig(dt, CommonDef.CONFIG_KEYS.LAST_SEND_PM);
            string lastSendAllDate = GetConfig(dt, CommonDef.CONFIG_KEYS.LAST_SEND_ALL);
            string date = DateTime.Now.ToString("yyyyMMdd");
            string time = DateTime.Now.ToString("HHmmss");
            string ERRORLOG = MiddleService.LOG_PATH + "\\" + "ERROR_LOG";
            if (sendPMTime != "")
            {
                if (time.CompareTo(sendPMTime) > 0)//超过发送项目经理的时间点
                {
                    if (lastSendPmDate != "")
                    {
                        if (date.CompareTo(lastSendPmDate) > 0)//超过最后一次发送的日期
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
                            if(dtSystems == null)
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
                                    string proId = drPro["DEMAND_ID"].ToString();
                                    msg += HtmAddRow("项目名称：" + drPro["DEMAND_NAME"].ToString());
                                    msg += HtmAddRow("项目性质：" + drPro["PRO_KIND"].ToString());
                                    msg += HtmAddRow("项目阶段：" + drPro["PRO_STAGE"].ToString());
                                    msg += HtmAddRow("进度信息：");

                                    DataTable dtProSysInfo = DataBaseManager.GetProSystemInfo(proId);
                                    if (dtProSysInfo == null)
                                    {
                                        //获取系统信息失败，终止任务
                                        File.AppendAllText(ERRORLOG, "SEND_PM_EMAIL:读取项目系统信息失败\r\n");
                                        return;
                                    }
                                    DataTable dtRateInfo = DataBaseManager.GetProRateInfo(proId);
                                    if (dtRateInfo == null)
                                    {
                                        //获取系统信息失败，终止任务
                                        File.AppendAllText(ERRORLOG, "SEND_PM_EMAIL:读取项目进度信息失败\r\n");
                                        return;
                                    }
                                    DataTable dtProSysRateInfo = new DataTable();
                                    dtProSysRateInfo.Columns.Add("SYS_ID");
                                    dtProSysRateInfo.Columns.Add("SYS_NAME");
                                    dtProSysRateInfo.Columns.Add("ESTIMATE_DAYS");
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
                                                    drProSys["DATE"] = drRate["DATE"].ToString();
                                                    drProSys["EXPLAIN"] = drRate["EXPLAIN"].ToString();
                                                    drProSys["PROBLEM"] = drRate["PROBLEM"].ToString(); 
                                                }
                                                else if (drProSys["DATE"].ToString().CompareTo(drRate["DATE"].ToString()) < 0)
                                                {
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
                                        {"DATE","最新录入日期"},
                                        {"EXPLAIN","进度说明"},
                                        {"PROBLEM","面临问题"},
                                    });
                                    msg += HtmAddRow("");
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
                                if (recvUsers == "")
                                {
                                    continue;
                                }
                                string sendTitle = drSys["SYS_NAME"].ToString() + "项目日报";
                                if (!SendEmail(recvUsers, sendEmail, sendName, sendEmail, sendPsw, sendHost, sendTitle, msg, true))
                                {
                                    File.AppendAllText(ERRORLOG, "SEND_PM_EMAIL:发送项目系统邮件失败\r\n");
                                    continue;
                                }
                                #endregion
                            }
                            //3、更新lastSendPmDate
                            DataBaseManager.SaveSysConfig(dt, CommonDef.CONFIG_KEYS.LAST_SEND_PM, date);
                        }
                    }
                }
            }
            if (sendALLTime != "")
            {
                if (time.CompareTo(sendALLTime) > 0)//超过发送汇总信息的时间点
                {
                    if (lastSendAllDate != "")
                    {
                        if (date.CompareTo(lastSendAllDate) > 0)//超过最后一次发送的日期
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
                            }
                            string msg = "";
                            msg += HtmAddRow("信息技术部全部在建需求数：" + totalCount.ToString() + "个，其中:");
                            msg += HtmAddRow("新建项目：" + newCount.ToString() + "个，系统升级类需求:" + updateCount.ToString() + "个。");
                            //2、按照部门汇总需求总数、新需求数目、维护升级数目
                            //3、汇总新需求（重点项目）详情
                            /*
                             * 项目名称
                             * 系统名称、预估工作量、系统开发进度、进度说明
                             */
                            //4、汇总维护需求详情
                            /*
                             * 项目名称
                             * 系统名称、预估工作量、系统开发进度、进度说明
                             */
                            //5、发送日报
                            //6、更新lastSendAllDate
                        }
                    }
                }
            }
        }

        private static bool SendEmail(string recv, string send, string sendName, string cc, 
            string sendPsw, string server, string title, string content, bool htm)
        {
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            msg.To.Add(recv);//收件人邮箱   
            msg.From = new MailAddress(send, send, System.Text.Encoding.UTF8);
            /* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
            msg.Subject = title;//邮件标题    
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码    
            msg.Body = content;//邮件内容    
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码    
            msg.IsBodyHtml = htm;//是否是HTML邮件    
            //msg.Priority = MailPriority.High;//邮件优先级    
            msg.CC.Add(cc);//李志功<lizhigong@hkcts.com>
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(send, sendPsw);
            client.Host = server;//smtp.hkcts.com
            object userState = msg;
            try
            {
                client.SendAsync(msg, userState);
                //简单一点儿可以client.Send(msg);    
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
                tableInfo += "<td>" + dictionary[dc.ColumnName] + "</td>";
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
                    tableInfo += "<td>" + dr[dc.ColumnName].ToString() + "</td>";
                }
                tableInfo += "</tr>";
            }
            tableInfo += "</tbody>";
            tableInfo += "</table>";
            return tableInfo;
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
