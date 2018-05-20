using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntity
{
    public class Customer
    {
        //[Required]
        //[Display(Name = "User name")]
        //public string UserName { get; set; }
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

        [Required(ErrorMessage = "Password required")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^[a-zA-Z]+[0-9]+$", ErrorMessage = "Password must be alphanumeric")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Please Accept Terms & Conditions")]
        public bool TermsandCondition { get; set; }
        public string GoogleMapLink { get; set; }

        public bool IsActivated { get; set; }
        public bool Status { get; set; }



    }
}
