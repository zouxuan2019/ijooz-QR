using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI.Models
{
    public class TopUpQRModel
    {
        public class TopUpQRReq
        {
            public string ReqType { get; set; }//Preview/Submit
            public string UserID { get; set; }
            public string QRCode { get; set; }
            public int TopUpQty { get; set; }
            public string PromotionCode { get; set; }
            public string Remark { get; set; }
            public string TransTime { get; set; }
            public string TransId { get; set; }
        }

        public class TopUpQR_OK
        {
            public string Result = "Success";
            public List<TopUpQrDetail> QrDetail { get; set; }
        }

        public class TopUpQrDetail
        {
            public string TransId { get; set; }
            public string ReqType { get; set; }//Preview/Submit
            public string Company { get; set; }
            public string Product { get; set; }
            public string QRCode { get; set; }
            public int TopUpQty { get; set; }
            public int TtlQty { get; set; }
            public string Currency { get; set; }
            public double Amt { get; set; }
            public DateTime ExpiryDate { get; set; }

        }

        public class TopUpQR_Fail
        {
            public string Result = "Fail";
            public string Msg { get; set; }
        }
    }
}
