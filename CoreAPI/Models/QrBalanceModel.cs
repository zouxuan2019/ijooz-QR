using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI.Models
{
    public class QrBalanceModel
    {
        public class QrBalanceReq
        {
            public string UserID { get; set; }

        }

        public class QrBalance_OK
        {
            public string Result = "Success";
            public List<QrBalance> QrBalance { get; set; }
        }

        public class QrBalance
        {
            public string UserID { get; set; }
            public string QRCode { get; set; }
            public string Company { get; set; }
            public string Product { get; set; }
            public string Currency { get; set; }
            public double UnitPrice { get; set; }
            public DateTime ExpiryDate { get; set; }
            public int Qty { get; set; }
            public string Remark { get; set; }
            public DateTime LastUpdateTime { get; set; }

        }

        public class QrBalance_Fail
        {
            public string Result = "Fail";
            public string Msg { get; set; }
        }
    }
}
