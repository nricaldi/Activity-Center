using Microsoft.EntityFrameworkCore;
using CSharpBeltExam.Models;

namespace CSharpBeltExam.Models 
{
    public class MyContext : DbContext 
    {
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users {get;set;}
        public DbSet<Plan> Plans {get;set;}
        public DbSet<Join> Joins {get;set;}
    }
}