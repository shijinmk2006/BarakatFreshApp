using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntity
{
    public class OrderItem
    {
        public int OrderId { get; set; }

        public  int ItemId { get; set; }

        public  decimal Price { get; set; }

        public decimal Qty { get; set; }

        public int PriceId { get; set; }

        public string ItemName { get; set; }

    }

    public class PurchaseList
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public int ItemId { get; set; }
        public string OrderRef { get; set; }
        public string ItemImage { get; set; }
        public decimal CartTotal { get; set; }
        public string OrderDate { get; set; }

        public string DeliveryDate { get; set; }
        public int OrderStatus { get; set; }
        public decimal ItemQty { get; set; }
        public decimal Total { get; set; }
        public decimal SubTotal { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalVat { get; set; }
        public string ItemTitle { get; set; }
        public int CustomerId { get; set; }
        public string PaymentType { get; set; }
        public string DeliveryAddress { get; set; }


    }
}
