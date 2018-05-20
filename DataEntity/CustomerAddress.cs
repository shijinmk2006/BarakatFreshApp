using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntity
{
    public class CustomerAddress
    {
        public int customerId { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        //[System.Web.Mvc.Remote("doesUserNameExistIndividual", "Member", HttpMethod = "POST", ErrorMessage = "Primary Email already exists")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile Number is mandatory")]
        [Display(Name = "Mobile Number")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
        [StringLength(9, ErrorMessage = "Mobile Number length is invalid", MinimumLength = 9)]
        public string MobileNumber { get; set; }
        [Required]
        public string BillingAddress
        {
            get;
            set;
        }
        [Required]
        public string BillingLocation
        {
            get;set;
        }
        [Required]
        public string BillingEmirate
        {
            get;set;
        }
        [Required]
        public string ShippingAddress
        {
            get;set;
        }
        [Required]
        public string ShippingLocation
        {
            get;set;
        }
        [Required]
        public string ShippingEmirate
        {
            get;set;
        }
    }
}
