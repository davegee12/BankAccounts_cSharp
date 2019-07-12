using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankAccounts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BankAccounts.Controllers
{
    public class HomeController : Controller
    {
        private UsersContext dbContext;

        // here we can "inject" our context service into the constructor
        public HomeController(UsersContext context)
        {
            dbContext = context;
        }

        // Display Index Page
        [HttpGet("")]
        public ViewResult Index()
        {
            return View("Index");
        }

        // Create RegUser POST route
        [HttpPost("create/register")]
        public IActionResult CreateRegUser(RegUser newUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<RegUser> Hasher = new PasswordHasher<RegUser>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    dbContext.Add(newUser);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("LoggedInId", newUser.RegUserId);
                    return RedirectToAction("Success");
                }
            }
            else
            {
                return View("Index");
            }
        }

        // Login LogUser POST route
        [HttpPost("login")]
        public IActionResult CreateLogUser(LogUser LoggedIn)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == LoggedIn.LogEmail);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LogUser>();
                var result = hasher.VerifyHashedPassword(LoggedIn, userInDb.Password, LoggedIn.LogPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("LoggedInId", userInDb.RegUserId);
                return RedirectToAction("Success");
            }
            else
            {
                return View("Index");
            }
        }

        // Success Display Page
        [HttpGet("success")]
        public IActionResult Success()
        {
            int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
            if (IntVariable == null)
            {
                HttpContext.Session.Clear();
                ModelState.AddModelError("LoggedIn", "Please log in");
                return RedirectToAction("Index");
            }
            else
            {
                var LoggedInUser = dbContext.Users
                .Include(user => user.MadeTransactions)
                .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));

                var UserTransactions = dbContext.Transactions.OrderByDescending( t => t.CreatedAt)
                .Where( t => t.RegUserId == LoggedInUser.RegUserId);

                // Calculate the user's current balance:
                double currentBalance = 0;
                foreach(var action in LoggedInUser.MadeTransactions)
                {
                    currentBalance += action.Amount;
                }
                Console.WriteLine(currentBalance);
                ViewBag.currentBalance = currentBalance;
                ViewBag.LoggedInUser = LoggedInUser;
                ViewBag.UserTransactions = UserTransactions;
                return View("Success");
            }
        }

        // Log Out
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        // Create Transaction Post route
        [HttpPost("create/transaction")]
        public IActionResult CreateTransaction(Transaction newTransaction)
        {
            if(ModelState.IsValid)
            {
                int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
                if (IntVariable == null)
                {
                    HttpContext.Session.Clear();
                    ModelState.AddModelError("LoggedIn", "Please log in");
                    return RedirectToAction("Index");
                }
                else
                {
                    var LoggedInUser = dbContext.Users
                    .Include(user => user.MadeTransactions)
                    .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));

                    var UserTransactions = dbContext.Transactions.OrderByDescending( t => t.CreatedAt)
                    .Where( t => t.RegUserId == LoggedInUser.RegUserId);

                    // Calculate the user's current balance:
                    double currentBalance = 0;
                    foreach(var action in LoggedInUser.MadeTransactions)
                    {
                        currentBalance += action.Amount;
                    }

                    ViewBag.currentBalance = currentBalance;
                    ViewBag.LoggedInUser = LoggedInUser;
                    ViewBag.UserTransactions = UserTransactions;

                    if(newTransaction.Amount > 0)
                    {
                        dbContext.Add(newTransaction);
                        dbContext.SaveChanges();
                        return RedirectToAction("Success");
                    }
                    else
                    {
                        if(currentBalance + newTransaction.Amount < 0)
                        {
                            ModelState.AddModelError("Amount", "Cannot process transaction");
                            return View("Success");
                        }
                        else
                        {
                            dbContext.Add(newTransaction);
                            dbContext.SaveChanges();
                            return RedirectToAction("Success");
                        }
                    }
                }
            }
            else
                {
                    return View("Success");
                }
        }
    }
}
