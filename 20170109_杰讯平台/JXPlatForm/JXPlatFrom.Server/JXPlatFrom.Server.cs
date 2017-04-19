using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;
using DataBaseManager;

namespace JXPlatFrom.Server
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            CommonDef.COM_RET ret = DataBaseHelper.DataBaseInit();
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return;
            }


        }
    }
}
