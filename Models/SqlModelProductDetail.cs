using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClothesstoreProductsAPI.Models
{
    public class SqlModelProductDetail
    {

        public string Detail_Id { get; set; }

        public string Seller_Id { get; set; }

        public string City_Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public float Price { get; set; }

        public float Discountprice { get; set; }

        public string Images { get; set; }

        public string Brand { get; set; }

        public string Thumbnail { get; set; }

        public string Currency { get; set; }

        public float Rating { get; set; }

    }
}
