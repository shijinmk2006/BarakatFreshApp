using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntity
{
    public class RecommendedItems
    {
        public int ItemId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public string ImagePath { get; set; }
        public string Origin { get; set; }
        public string Description { get; set; }
        public string NoOfVisit { get; set; }


    }
}
