using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeddingPlaner.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace WeddingPlaner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        private User ActiveUser
        {
            get
            {
                return dbContext.Users.Where(u => u.UserId == HttpContext.Session.GetInt32("LoggedUser")).FirstOrDefault();
            }
        }
        [HttpGet("")]
        public ViewResult LoginReg()
        {
            return View("loginreg");
        }
        [HttpPost("register")]
        public IActionResult Register(FormWrapper FromForm)
        {
            if(dbContext.Users.Any(u => u.Email == FromForm.Register.Email))
            {
                ModelState.AddModelError("Register.Email", "Email already registered to an account");
            }
            else if(ModelState.IsValid)
            {
                PasswordHasher<User> Haser = new PasswordHasher<User>();
                FromForm.Register.Password = Haser.HashPassword(FromForm.Register, FromForm.Register.Password);
                dbContext.Users.Add(FromForm.Register);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("LoggedUser", FromForm.Register.UserId);
                return Redirect("dashboard");
            }
            return LoginReg();
        }
        [HttpPost("login")]
        public IActionResult Login(FormWrapper FromForm)
        {
            if(ModelState.IsValid)
            {
                User LogUser = dbContext.Users
                    .FirstOrDefault(u => u.Email == FromForm.Login.Email);
                if(LogUser == null)
                {
                    ModelState.AddModelError("Login.Email", "Invalid email address");
                    return LoginReg();
                }
                PasswordHasher<LoginUser> Hasher = new PasswordHasher<LoginUser>();
                var result = Hasher.VerifyHashedPassword(FromForm.Login, LogUser.Password, FromForm.Login.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("Login.Password", "Incorrect Password");
                    return LoginReg();
                }
                HttpContext.Session.SetInt32("LoggedUser", LogUser.UserId);
                return Redirect("dashboard");
            }
            return LoginReg();
        }

        [HttpGet("dashboard")]
        public ViewResult Dashboard ()
        {
            
            if(ActiveUser != null)
            {
                ViewBag.UserId = (int)HttpContext.Session.GetInt32("LoggedUser");
                ViewBag.WeddingInfo = dbContext.Weddings
                    .Include(w => w.GuestList)
                    .ThenInclude(l => l.User);
                return View("dashboard");
            }
            return LoginReg();
        }

        [HttpGet("new/wedding")]
        public ViewResult WeddingForm()
        {
            if(ActiveUser != null)
            {
                return View("newwedding");
            }    
            return LoginReg();
        }

        [HttpPost("new/wedding")]
        public IActionResult NewWedding(Wedding FromForm)
        {
            if(ActiveUser != null)
            {
                FromForm.UserId = (int)HttpContext.Session.GetInt32("LoggedUser");
                if(ModelState.IsValid)
                {
                    dbContext.Weddings.Add(FromForm);
                    dbContext.SaveChanges();
                    return Redirect("/dashboard");
                }
                return WeddingForm();
            }
            return WeddingForm();
        }
        [HttpGet("rsvp/{WedId}")]
        public IActionResult RSVP(int WedId)
        {
            if(ActiveUser != null)
            {
                Guest NewGuest = new Guest
                {
                    UserId = ActiveUser.UserId,
                    WeddingId = WedId
                };
                dbContext.GuestList.Add(NewGuest);
                dbContext.SaveChanges();
                return Dashboard();
            }
            return LoginReg();
        }

        [HttpGet("unrsvp/{WedId}")]
        public IActionResult UnRSVP(int WedId)
        {
            if(ActiveUser != null)
            {
                Guest ToDelete = dbContext.GuestList
                    .Where(g => g.WeddingId == WedId)
                    .SingleOrDefault(gu => gu.UserId == ActiveUser.UserId);
                dbContext.GuestList.Remove(ToDelete);
                dbContext.SaveChanges();
                return Dashboard();
            }
            return LoginReg();
        }

        [HttpGet("delete/{WedId}")]
        public IActionResult Delete(int WedId)
        {   
            if(ActiveUser != null)
            {
            Wedding ToDelete = dbContext.Weddings
                .FirstOrDefault(w => w.WeddingId == WedId);
            dbContext.Remove(ToDelete);
            dbContext.SaveChanges();
            return Dashboard();
            }
            return LoginReg();
        }

        [HttpGet("logout")]
        public ViewResult logout()
        {
            HttpContext.Session.Clear();
            return LoginReg();
        }

        [HttpGet("weddinginfo/{WedId}")]
        public IActionResult WeddingInfo(int WedId)
        {
            if(ActiveUser != null)
            {
                ViewBag.Wedding = dbContext.Weddings
                    .Include(w => w.GuestList)
                    .ThenInclude(g => g.User)
                    .FirstOrDefault(w => w.WeddingId == WedId);
                return View("weddinginfo");
            }
            return LoginReg();
        }



























        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
