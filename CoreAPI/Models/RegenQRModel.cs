using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI.Models
{
    public class RegenQRModel
    {
        public class RegenQRReq
        {

            public string UserID { get; set; }
            public string QRCode { get; set; }
            public string TransId { get; set; }
        }

        public class RegenQR_OK
        {
            public string Result = "Success";
            public List<QrDetail> QrDetail { get; set; }
        }

        public class QrDetail
        {
            public string TransId { get; set; }
            public string NewQRCode { get; set; }
            public int TtlQty { get; set; }
            public DateTime ExpiryDate { get; set; }

        }

        public class RegenQR_Fail
        {
            public string Result = "Fail";
            public string Msg { get; set; }
        }
    }
}
