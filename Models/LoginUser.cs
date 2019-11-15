using System.ComponentModel.DataAnnotations;

namespace CSharpBeltExam.Models 
{
    public class LoginUser
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress (ErrorMessage = "Please enter a valid email")]
        [Display(Name = "Email")]
        public string LoginEmail {get; set;}

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string LoginPassword {get; set;}
    }
}