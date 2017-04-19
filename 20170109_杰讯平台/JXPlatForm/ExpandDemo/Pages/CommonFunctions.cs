using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CommonLib;
using ClientCommunication;

namespace ExpandDemo.Pages
{
    class CommonFunctions
    {
        private static DataTable dtGoodsPrices;

        internal static decimal GetGoodsPrice(string kind, string name, string model)
        {
            if (dtGoodsPrices == null)
            {
                dtGoodsPrices = GetAllGoodsPrice();
            }
            if (dtGoodsPrices == null)
            {
                return 0;
            }
            decimal retPrice = 0;
            foreach (DataRow dr in dtGoodsPrices.Rows)
            {
                if (dr["KIND"].ToString() == kind)
                {
                    if (dr["NAME"].ToString() == name)
                    {
                        if (dr["MODEL"].ToString() == model)
                        {
                            retPrice = decimal.Parse(dr["PRICE"].ToString());
                            return retPrice;
                        }
                        if (dr["MODEL"].ToString() == "")
                        {
                            retPrice = decimal.Parse(dr["PRICE"].ToString());
                        }
                    }
                    if (dr["NAME"].ToString() == "")
                    {
                        retPrice = decimal.Parse(dr["PRICE"].ToString());
                    }
                }
            }
                return retPrice;
        }

        internal static DataTable GetAllGoodsPrice()
        {
            CommonDef.COM_RET ret = CommunicationHelper.CommonRead("ExpandDemo",
                (int)ExpandDemoCommon.FUNC_NO.GET_SALE_PRICE_INFO, ref dtGoodsPrices);
            if (ret != CommonDef.COM_RET.RET_OK)
            {
                return null;
            }
            return dtGoodsPrices;
        }
    }
}
