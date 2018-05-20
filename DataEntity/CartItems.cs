using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntity
{
    public class CartInsertItems
    {
        public string CustomerId { get; set; }
        public string ItemId { get; set; }
        public string ItemTypeId { get; set; }
        public string ItemTitle { get; set; }
        public decimal ItemPrice { get; set; }
        public string ItemImage { get; set; }
        public string ItemUnit { get; set; }
        public decimal ItemQty { get; set; }
        public decimal Total { get; set; }
    }
    public class ItemCart
    {
        public int CartId { get; set; }
        public string ItemId { get; set; }
        public string ItemTypeId { get; set; }
        public string ItemTitle { get; set; }
        public decimal ItemPrice { get; set; }
        public string ItemImage { get; set; }
        public string ItemUnit { get; set; }
        public decimal ItemQty { get; set; }
        public decimal Total { get; set; }
    }
}
