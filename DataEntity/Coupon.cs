using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntity
{
    public class Coupon
    {
        public string couponCode { get; set; }
        public DateTime deliveryDate { get; set; }
       
    }

    public class Voucher
    {
        public string voucherCode { get; set; }
        public DateTime deliveryDate { get; set; }

    }

}
