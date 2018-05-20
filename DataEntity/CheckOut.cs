using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntity
{
    public class CheckOut
    {
        [Required(ErrorMessage = "Delivery Time is required!")]
        public int DeliveryTime { get; set; }
        [Required(ErrorMessage = "Delivery Date is required!")]
        public string DeliveryDate { get; set; }

        [Required(ErrorMessage = "Delivery Area is required!")]
        public int DeliveryArea { get; set; }
        public string DeliveryNote { get; set; }
        //[Required(ErrorMessage = "Accept the Terms & Conditions")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Please Accept Terms & Conditions")]
        public bool TermsandConditions { get; set; }
        [Required(ErrorMessage = "Name is required!")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Mobile number is required!")]
        [Display(Name = "Mobile Number")]
        [RegularExpression(@"^(50|52|54|55|56|58)[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
       
        [StringLength(9, ErrorMessage = "Please enter a valid Mobile Number", MinimumLength = 9)]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Delivery Address is required!")]
        public string DeliveryAddress { get; set; }
        [Required(ErrorMessage = "Delivery Location is required!")]
        public string DeliveryLocation { get; set; }
        [Required(ErrorMessage = "Emirate is required!")]
        public string DeliveryEmirate { get; set; }

        //[Required(ErrorMessage = "This field is required!")]
        public string BillingAddress { get; set; }
        //[Required(ErrorMessage = "This field is required!")]
        public string BillingLocation { get; set; }
        //[Required(ErrorMessage = "This field is required!")]
        public string BillingEmirate { get; set; }
        public int CustomerId { get; set; }
        public int CheckoutType { get; set; }
     
        public decimal CartTotal { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DeliveryCharge { get; set; }
        public bool CouponApplied { get; set; }
        public bool VoucherApplied { get; set; }

        public string VoucherCode { get; set; }
        public string Coupon { get; set; }
        public decimal DiscountApplied { get; set; }

        public int PaymentType { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
    public class CustomerBilling
    {
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerEmirate { get; set; }
        public string CustomerCountry { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerMobile { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingEmirate { get; set; }
    }
}
