using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClothesstoreProductsAPI.Models
{
    public class Product
    {

        public string ProductId { get; set; }

        public string Name { get; set; }


        public string Images { get; set; }

        public float Price { get; set; }

        public float DiscountPrice => (float)(Price - ((Price * DiscountPercent) / 100));

        public float DiscountPercent { get; set; }

        public ProductDetail ProductDetail { get; set; }


    }
}
