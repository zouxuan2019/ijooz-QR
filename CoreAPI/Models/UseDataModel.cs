using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI.Models
{
    public class UseDataModel
    {
        public class UseDataReq
        {
            public string QRCode { get; set; }
            public string MachineID { get; set; }
            public string TransId { get; set; }
            public DateTime TransTime { get; set; }
            public int balanceQty { get; set; }
            public DateTime ExpiryDate { get; set; }
            public string Signature { get; set; }
        }

        public class UseData_OK
        {
            public string Result = "Success";            
        }

        public class UseData_Fail
        {
            public string Result = "Fail";
        }
    }
}
