using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntity
{
    public class Item
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Origin { get; set; }
        public int Group { get; set; }
        public string Unit { get; set; }
        public string Discount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal PriceNew { get; set; }
        public string WeightInfo { get; set; }
        public decimal Qty { get; set; }
    }
}
