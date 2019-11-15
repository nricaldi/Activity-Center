
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CSharpBeltExam.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CSharpBeltExam.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        // ==================== Rendering Index ==================== // 
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        // ==================== Rendering Dashboard ==================== // 
        public void removeInThePast(List<Plan> AllPlans){
            DateTime now = DateTime.Now;
            foreach(var plan in AllPlans){
                if(now > plan.Date){
                    dbContext.Plans.Remove(plan);
                }
            } 
            dbContext.SaveChanges();  
        }
        
        [HttpGet("dashboard")]
        public IActionResult Dashboard(){
            int? userId = HttpContext.Session.GetInt32("UserId");
            if(userId == null)
                return Redirect("/");
            
            User current = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
            
            List<Plan> plans = dbContext.Plans
            .OrderBy(p => p.Date)
            .ThenBy(p => p.Time)
            .Include(p => p.Coordinator)
            .Include(p => p.Attendees)
            .ThenInclude(p => p.User)
            .ToList();

            removeInThePast(plans);

            ViewBag.User = current;
            ViewBag.AllPlans = plans;
            return View();
        }

        [HttpGet("addPlan")]
        public IActionResult AddPlan(){
            return View();
        }



        // ==================== Create Plan ==================== // 
        [HttpPost("createPlan")]
        public IActionResult CreatePlan(Plan plan){
            if(ModelState.IsValid){
                int? userId = HttpContext.Session.GetInt32("UserId");
                User current = dbContext.Users.FirstOrDefault(u => u.UserId == userId);

                dbContext.Plans.Add(plan);
                plan.Coordinator = current;
                plan.CoordinatorId = (int)userId;
                dbContext.SaveChanges();

                return RedirectToAction("Dashboard");
            }
            else
                return View("AddPlan");

        }


        // ==================== View Plan ==================== // 
        [HttpGet("plan/{PlanId}")]
        public IActionResult Plan(int planId){
            int? userId = HttpContext.Session.GetInt32("UserId");
            User currentUser = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
            Plan currentPlan = dbContext.Plans
            .Include(p => p.Coordinator)
            .Include(p => p.Attendees)
            .FirstOrDefault(p => p.PlanId == planId);
            
            ViewBag.Plan = currentPlan;
            ViewBag.User = currentUser;

            return View("Plan");
        }


        // ==================== Login / Register ==================== // 
        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid){
                User userMatchingEmail = dbContext.Users.FirstOrDefault(u => u.Email == user.Email);
                if(userMatchingEmail != null)
                {
                    ModelState.AddModelError("Email", "Email already in use.");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.Password = Hasher.HashPassword(user, user.Password);
                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    return Redirect("dashboard");                
                }
            }
            else
                return View("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser user)
        {
            if(ModelState.IsValid)
            {
                User userMatchingEmail = dbContext.Users.FirstOrDefault(u => u.Email == user.LoginEmail);
                if(userMatchingEmail == null)
                {
                    ModelState.AddModelError("LoginEmail", "Unknown Email");
                    return View("Index");
                }
                else
                {
                    var hasher = new PasswordHasher<LoginUser>();
                    var result = hasher.VerifyHashedPassword(user, userMatchingEmail.Password, user.LoginPassword);
                    if(result == 0)
                    {
                        ModelState.AddModelError("LoginPassword", "Incorrect password");
                        return View("Index");                        
                    }
                    HttpContext.Session.SetInt32("UserId", userMatchingEmail.UserId);
                    return Redirect("dashboard");
                }
            }
            return View("Index");
        }



        // ==================== Join ==================== // 
        [HttpGet("join/{Planid}")]
        public IActionResult Join(int planId){
            int? userId = HttpContext.Session.GetInt32("UserId");
            User currentUser = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
            Plan planToJoin = dbContext.Plans.FirstOrDefault(p => p.PlanId == planId);
            Join newJ = new Join();

            newJ.User = currentUser;
            newJ.UserId = currentUser.UserId;
            newJ.Plan = planToJoin;
            newJ.PlanId = planToJoin.PlanId;

            dbContext.Joins.Add(newJ);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }


        // ==================== Leave ==================== // 
        [HttpGet("leave/{Planid}")]
        public IActionResult Leave(int planId){
            int? userId = HttpContext.Session.GetInt32("UserId");
            User currentUser = dbContext.Users.FirstOrDefault(u => u.UserId == userId);            
            Plan planToLeave = dbContext.Plans.FirstOrDefault(p => p.PlanId == planId);
            Join joinToDelete = dbContext.Joins
            .Where(j => j.UserId == currentUser.UserId)
            .FirstOrDefault(j => j.PlanId == planToLeave.PlanId);

            dbContext.Joins.Remove(joinToDelete);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }


        // ==================== Delete ==================== // 
        [HttpGet("delete/{PlanId}")]
        public IActionResult Delete(int planId)
        {
            Plan planToDelete = dbContext.Plans.FirstOrDefault(p => p.PlanId == planId);
            dbContext.Plans.Remove(planToDelete);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }



        // ==================== Logout ==================== // 
        [HttpGet("logout")]
        public IActionResult Logout(){
            HttpContext.Session.Clear();
            return RedirectToAction("Index");

            
        }

    }   
}
