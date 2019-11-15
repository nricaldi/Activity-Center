using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CSharpBeltExam.Models 
{
    public class Plan
    {
        [Key]
        public int PlanId { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        [NoDateInTheFuture]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime Time { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        public string Description { get; set; }

        public string DurationType { get; set; }

        public User Coordinator {get; set; }

        public int CoordinatorId { get; set; }

        public List<Join> Attendees { get; set; }

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        public string GetDate(){
            return Date.ToString("MM/dd/yy");
        }

        public string GetTime(){
            return Time.ToString("h:mm tt");
        }
    }

        public class NoDateInTheFuture : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dateValue = value as DateTime? ?? new DateTime();

            if(dateValue < DateTime.Now)
                return new ValidationResult("Date cannot be in the past.");

            return ValidationResult.Success;
        }
    }
}