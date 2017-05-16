using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CommonLib;

namespace ProjectsManageServer.Connect
{
    class ConnectService
    {
        /// <summary>
        /// 记录未发完的数据
        /// </summary>
        static Dictionary<string, string> dicUnFinishData = new Dictionary<string, string>();

        /// <summary>
        /// 服务器服务
        /// </summary>
        /// <param name="RecievedData"></param>
        /// <param name="SendData"></param>
        /// RecievedData
        /// 第一行 MSGID
        /// 第二行 MSGLENGTH
        /// 第三行 FUNNO
        /// 第四行以后 ELEMNS
        /// SendData
        /// 第一行 MSGID
        /// 第二行 MSGLENGTH
        /// 第三行 STAT
        /// 第四行以后 ELEMNS
        static Dictionary<string, string> dicReciveData = new Dictionary<string, string>();
        internal static void Services(string connectFlag, byte[] RecievedData, ref byte[] SendData)
        {
            try
            {
                #region 完整性传输
                //检查包是否完整
                if (dicReciveData.ContainsKey(connectFlag))
                {
                    dicReciveData[connectFlag] += Encoding.Default.GetString(RecievedData);
                }
                else
                {
                    dicReciveData.Add(connectFlag, Encoding.Default.GetString(RecievedData));
                }
                string reciveData = dicReciveData[connectFlag];
                if (reciveData.StartsWith("[BIGEN]") && reciveData.EndsWith("[END]"))
                {
                    //已收到完整的包
                    reciveData = reciveData.Substring(7);
                    reciveData = reciveData.Remove(reciveData.Length - 5);//去掉bigen end
                    dicReciveData.Remove(connectFlag);
                }
                else
                {
                    //不是完整的包
                    if (!reciveData.StartsWith("[BIGEN]"))
                    {//丢掉有问题的包
                        dicReciveData.Remove(connectFlag);
                    }
                    return;
                }
                #endregion
                #region 长报文多次传输完整性检查
                //解析包
                string[] recvElem = reciveData.Split('\n');
                int headerLength = recvElem[0].Length + recvElem[1].Length + 2;//报文ID + 报文长度
                int recvLength = RecievedData.Length - headerLength; //实际接收长度
                string msgData = "";
                //是否为未完成的传输报文
                if (dicUnFinishData.ContainsKey(recvElem[0]))//已知长报文
                {
                    if (int.Parse(recvElem[1]) < 0)//<0发送长报文 >=0接收长报文
                    { 
                        //未完成的发送报文
                        List<byte> unsendData = Encoding.Default.GetBytes(dicUnFinishData[recvElem[0]]).ToList();
                        int unsendLength = unsendData.Count;
                        if (unsendData.Count > CommonDef.MAX_MSG_LENGTH)//剩余部分仍然为长报文需要继续发
                        {
                            dicUnFinishData[recvElem[0]] = Encoding.Default.GetString(unsendData.GetRange(CommonDef.MAX_MSG_LENGTH, unsendData.Count - CommonDef.MAX_MSG_LENGTH).ToArray());
                            unsendData.RemoveRange(CommonDef.MAX_MSG_LENGTH, unsendData.Count - CommonDef.MAX_MSG_LENGTH);
                        }
                        else
                        {
                            dicUnFinishData.Remove(recvElem[0]);
                        }
                        unsendData.InsertRange(0, Encoding.Default.GetBytes(recvElem[0] + "\n" + unsendLength.ToString() + "\n"));
                        SendData = unsendData.ToArray();
                        return;
                    }
                    else
                    { 
                        //未完成的接收报文
                        int recvedLen = Encoding.Default.GetBytes(dicUnFinishData[recvElem[0]]).Length;
                        if (recvedLen + recvLength > int.Parse(recvElem[1]))
                        {
                            //报文长度已够
                            msgData = dicUnFinishData[recvElem[0]] + reciveData.Substring(headerLength);
                            dicUnFinishData.Remove(recvElem[0]);
                        }
                        else
                        {
                            //报文长度不够
                            dicUnFinishData[recvElem[0]] += reciveData.Substring(headerLength);
                            SendData = Encoding.Default.GetBytes(recvElem[0] + "\n1\n0");
                            return;
                        }
                    }
                }
                else
                {
                    //新报文
                    if (recvLength >= int.Parse(recvElem[1]))//完整的短报文
                    {
                        msgData = reciveData.Substring(headerLength);
                    }
                    else//长报文，没有接收完整
                    {
                        dicUnFinishData.Add(recvElem[0], reciveData.Substring(headerLength));
                        SendData = Encoding.Default.GetBytes(recvElem[0] + "\n1\n0");
                        return;
                    }
                }
                #endregion
                #region 解析报文并发送
                List<byte> sendList = MiddleService.AnalysisFile(msgData).ToList();
                int totalLength = sendList.Count;
                if (sendList.Count > CommonDef.MAX_MSG_LENGTH)//回复长报文，分多次传输
                {
                    dicUnFinishData.Add(recvElem[0], Encoding.Default.GetString(sendList.GetRange(CommonDef.MAX_MSG_LENGTH, sendList.Count - CommonDef.MAX_MSG_LENGTH).ToArray()));
                    sendList.RemoveRange(CommonDef.MAX_MSG_LENGTH, sendList.Count - CommonDef.MAX_MSG_LENGTH);
                }
                sendList.InsertRange(0, Encoding.Default.GetBytes(recvElem[0] + "\n" + totalLength.ToString() + "\n"));
                SendData = sendList.ToArray();
                #endregion
            }
            catch (Exception ex)
            {
                Console.Write("\n异常：" + ex.Message);
                string send = CommonDef.ERROR.ToString();
                SendData = Encoding.Default.GetBytes("0\n" + send.Length.ToString() + "\n" + send);
            }
        }

    }
}
