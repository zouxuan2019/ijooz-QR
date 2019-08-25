using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI.Models
{
    public class QrReqModel
    {
        public class QrReq
        {
            public string UserID { get; set; }
            public string QRCode { get; set; }
            public string Company { get; set; }
            public string Product { get; set; }
            public string Currency { get; set; }
            public decimal UnitPrice { get; set; }

        }

        public class QrReq_OK
        {
            public string Result = "Success";
            public List<QrDetail> QrDetail { get; set; }
        }

        public class QrDetail
        {
            public string Company { get; set; }
            public string Product { get; set; }
            public string QRCode { get; set; }
            public int BalanceQty { get; set; }
            public DateTime ExpiredDate { get; set; }

        }

        public class QrReq_Fail
        {
            public string Result = "Fail";
            public string Msg { get; set; }
        }
    }
}
