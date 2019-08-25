using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI.Models
{
    public class ShareQRModel
    {
        public class ShareQRReq
        {
            public string UserID { get; set; }
            public string QRCode { get; set; }
            public int SharedQty { get; set; }
            public string Remark { get; set; }
            public string TransTime { get; set; }
            public string TransId { get; set; }
        }

        public class ShareQR_OK
        {
            public string Result = "Success";
            public List<QrDetail> QrDetail { get; set; }
        }

        public class QrDetail
        {
            public string TransId { get; set; }
            public string QRCode { get; set; }
            public int RemainQty { get; set; }
            public string SharedQRCode { get; set; }
            public int SharedQty { get; set; }
            public DateTime ExpiryDate { get; set; }
            public string Remark { get; set; }

        }

        public class ShareQR_Fail
        {
            public string Result = "Fail";
            public string Msg { get; set; }
        }
    }
}
