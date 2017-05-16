using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectsManageServer.DataBases;
using System.Net;
using ProjectsManageServer.Connect;

namespace ProjectsManageServer
{
    class Program
    {
        static void Main(string[] args)
        {    
            try
            {
                if (!DataBaseManager.InitDataBases())
                {
                    Console.Write("数据库初始化失败！回车退出！");
                    Console.Read();
                    return;
                }
                int portNo = 0;
                IPAddress ip = null;
                while (true)
                {
                    Console.Write("请输入服务端口后回车（默认60000）：");
                    string read = Console.ReadLine();
                    if (read == "")
                    {
                        read = "60000";
                    }
                    if (!int.TryParse(read, out portNo))
                    {
                        if (read.Contains(':'))
                        { 
                            string[] ipAddr = read.Split(':');

                            if (!IPAddress.TryParse(ipAddr[0], out ip))
                            {
                                Console.Write("输入错误，请重新输入。\n");
                                continue;
                            }
                            if (!int.TryParse(ipAddr[1], out portNo))
                            {
                                Console.Write("输入错误，请重新输入。\n");
                                continue;
                            }
                            break;
                        }

                        Console.Write("输入错误，请重新输入。\n");
                        continue;
                    }
                    break;
                }
                if (!Connection.InitConnect(ip, portNo))
                {
                    Console.Write("网络连接初始化失败！回车退出！");
                    Console.Read();
                    return;
                }
            }
            catch(Exception e)
            {
                Console.Write("服务器运行出现错误，请重启！" + e.Message);
                Console.Read();
                return;
            }
        }
    }
}
