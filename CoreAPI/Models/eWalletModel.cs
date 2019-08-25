using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI.Models
{
    public class eWalletModel
    {
        public class eWalletDeductionReqModel
        {
            public double amount { get; set; }
            public string transactionId { get; set; }
            public string product { get; set; }
            public string company { get; set; }
            public string comment { get; set; }
            public string userId { get; set; }

            public DateTime actionDate = DateTime.Now;

        }
    }
}
