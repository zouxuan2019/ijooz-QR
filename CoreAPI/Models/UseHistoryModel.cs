using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI.Models
{
    public class UseHistory
    {
        public class UseHistoryReq
        {
            public string UserID { get; set; }
            public string Range { get; set; }


        }

        public class QueryUse_OK
        {
            public string Result = "Success";
            public List<UseDetail> UseDetail { get; set; }
        }

        public class UseDetail
        {
            public string QRCode { get; set; }
            public string MachineID { get; set; }
            public string TransID { get; set; }
            public string Currency { get; set; }
            public decimal UnitPrice { get; set; }
            public DateTime TransTime { get; set; }
        }

        public class QueryUse_Fail
        {
            public string Result = "Fail";
            public string Msg { get; set; }
        }
    }
}
