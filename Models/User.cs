using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace CSharpBeltExam.Models 
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters long")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [ValidPassword]
        public string Password { get; set; }
        
        [NotMapped]
        [Required(ErrorMessage = "Confirm password")]
        [Compare("Password", ErrorMessage = "Passwords must match")]
        [DataType(DataType.Password)]
        public string Confirm { get; set; }
        
        public List<Join> Plans {get; set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

    }

    public class ValidPassword : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var pass = Convert.ToString(value);
            var hasNumber = new Regex(@"[0-9]+");
            var hasChar = new Regex(@"[A-Z] *| [a-z]+");
            var hasSpecial = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
            var isValidated = hasNumber.IsMatch(pass) && hasChar.IsMatch(pass) && hasSpecial.IsMatch(pass);
            

            if(isValidated)
                return ValidationResult.Success;
            else
                return new ValidationResult("Password must contain at least one number, one lowecase letter ,one capital letter and one special character");

        }
    }
}