using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI.Models
{
    public class UpdateDeductionModel
    {
        public class UpdateDeductionReq
        {
            public string TransId { get; set; }
            public string UserID { get; set; }
            public string Company { get; set; }
            public string Product { get; set; }
            public string Currency { get; set; }
            public decimal UnitPrice { get; set; }
            public string QRCode { get; set; }
            public string ReqSN { get; set; }
            public string TransTime { get; set; }
            public string Signature { get; set; }

        }

        public class UpdateDeduction_OK
        {
            public string Result = "Success";
            public List<QrDetail> QrDetail { get; set; }
        }

        public class QrDetail
        {
            public string ReqSN { get; set; }//Preview/Submit
            public string TransId { get; set; }
            public string TransTime { get; set; }

            public string Signature { get; set; }

        }

        public class UpdateDeductionQR_Fail
        {
            public string Result = "Fail";
            public string Msg { get; set; }
        }
    }
}
