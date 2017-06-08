using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectsManageServer.DataBases;
using System.Net;
using ProjectsManageServer.Connect;
using System.Runtime.InteropServices;

namespace ProjectsManageServer
{
    class Program
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        extern static IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);
        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        extern static IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        /// <summary>  
        /// 关闭时的事件  
        /// </summary>  
        /// <param name="sender">对象</param>  
        /// <param name="e">参数</param>  
        protected static void CloseConsole(object sender, ConsoleCancelEventArgs e)
        {
            Environment.Exit(0);
            //return;
        }

        static void Main(string[] args)
        {
            Console.Title = "项目管理服务端";
            IntPtr windowHandle = FindWindow(null, "项目管理服务端");
            IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
            uint SC_CLOSE = 0xF060;
            RemoveMenu(closeMenu, SC_CLOSE, 0x0);
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CloseConsole);
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
                if (args.Length == 0)
                {
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
                }
                else
                {
                    portNo = int.Parse(args[0]);
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
