using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClothesstoreProductsAPI.Models
{
    public class ProductDetail
    {
        public string DetailId { get; set; }

        public string Description { get; set; }

        public string ImagesD { get; set; }

        public Seller Seller { get; set; }

        public City City { get; set; }

        public string Brand { get; set; }

        public string Thumbnail { get; set; }

        public string Currency { get; set; }

        public float Rating { get; set; }


    }
}
