using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI.Models
{
    public class ProductList
    {
        public class ProductList_OK
        {
            public string Result = "Success";
            public List<ProductDetail> ProductDetail { get; set; }
        }

        public class ProductDetail
        {
            public string Company { get; set; }
            public string Product { get; set; }
            public string Currency { get; set; }
            public double UnitPrice { get; set; }
        }
    }
}
