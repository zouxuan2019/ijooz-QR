using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI.Models
{
    public class NewQRModel
    {
        public class NewQRReq
        {
            public string ReqType { get; set; }//Preview/Submit
            public string UserID { get; set; }
            public string Company { get; set; }
            public string Product { get; set; }
            public int NewQty { get; set; }
            public string PromotionCode { get; set; }
            public string Remark { get; set; }
            public string TransTime { get; set; }
            public string TransId { get; set; }
        }

        public class NewQR_OK
        {
            public string Result = "Success";
            public List<NewQRDetail> QrDetail { get; set; }
        }

        public class NewQRDetail
        {
            public string TransId { get; set; }
            public string ReqType { get; set; }//Preview/Submit
            public string Company { get; set; }
            public string Product { get; set; }
            public string QRCode { get; set; }
            public int TtlQty { get; set; }
            public string Currency { get; set; }
            public double Amt { get; set; }
            public DateTime ExpiredDate { get; set; }

        }

        public class NewQR_Fail
        {
            public string Result = "Fail";
            public string Msg { get; set; }
        }
    }
}
