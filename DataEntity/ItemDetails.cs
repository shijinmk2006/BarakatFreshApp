using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntity
{
    public class ItemDetail
    {
        public int ItemId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Bebefits { get; set; }
        public string Usage { get; set; }
        public string Category { get; set; }
        public string origin { get; set; }
        public string Code { get; set; }
        public string Unit { get; set; }
        public int PriceId { get; set; }
        public int OrganisationId { get; set; }
    }
}
