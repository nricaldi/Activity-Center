using Microsoft.EntityFrameworkCore;
using CSharpBeltExam.Models;
using System.ComponentModel.DataAnnotations;

namespace CSharpBeltExam.Models 
{   
    public class Join 
    {
        [Key]
        public int JoinId { get; set; }
        public User User {get; set;}
        public Plan Plan {get; set;}
        public int UserId {get; set;}
        public int PlanId {get; set;}
    }
}