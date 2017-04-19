using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace DataBaseManager
{
    public class ExpandBaseServer
    {
        public virtual bool InitDataBase()
        {
            return true;
        }

        public virtual CommonDef.COM_RET HandleData(DataHelper dataHelper)
        {
            return CommonDef.COM_RET.RET_OK;
        }
    }
}
