using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClothesstoreProductsAPI.Models
{
    public class Product
    {

        public string product_id { get; set; }

        public string name { get; set; }

        public string brand { get; set; }

        public string thumbnail { get; set; }

        public string pictures { get; set; }

        public string description { get; set; }

        public float price { get; set; }

        public float discountPrice => (float)(price - ((price * discountPrice) / 100));

        public float discountPercent { get; set; }

        public string city_code { get; set; }

        public string seller_id { get; set; }

        public string currency { get; set; }

        public float rating { get; set; }

    }
}
