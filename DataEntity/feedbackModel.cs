using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntity
{
    public class feedbackModel
    {
        [Required(ErrorMessage = "Field Is Required")]
        public string FeedbackType { get; set; }
        [Required(ErrorMessage = "Field Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Field Is Required")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Field Is Required")]
        public string Feedback { get; set; }
        [Required(ErrorMessage = "Invalid Mobile Number")]
        [Display(Name = "Mobile Number")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
        [StringLength(12, ErrorMessage = "Invalid Mobile Number", MinimumLength = 12)]
        public string Mobile { get; set; }
    }
}
