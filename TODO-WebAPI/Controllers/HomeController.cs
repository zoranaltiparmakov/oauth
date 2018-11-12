using Microsoft.AspNet.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TODO_WebAPI.Models;

namespace TODO_WebAPI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Manage()
        {
            List<Models.Client> clients = null;
            using (var db = new TodoAppDbContext())
            {
                clients = db.Clients.Where(c => c.User.Username == User.Identity.Name).ToList();
            }
            return View(clients);
        }

        [HttpPost]
        public ActionResult Manage(Client client)
        {
            if(string.IsNullOrEmpty(client.Client_ID))
            {
                client.Client_ID = Guid.NewGuid().ToString();
            }
            if(string.IsNullOrEmpty(client.Client_Secret))
            {
                client.Client_Secret = Guid.NewGuid().ToString();
            }

            using (var db = new TodoAppDbContext())
            {
                User user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
                db.Clients.Add(new Client
                {
                    Client_ID = client.Client_ID,
                    Client_Secret = Guid.NewGuid().ToString(),
                    Redirect_URI = client.Redirect_URI,
                    Created_On = DateTime.Now,
                    User = user
                });

                db.SaveChanges();
            }
            return RedirectToAction("Manage", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Models.User user)
        {
            using (var db = new TodoAppDbContext())
            {
                User userToFind = db.Users.Where(u => u.Username == user.Username).FirstOrDefault();
                if (userToFind != null)
                {
                    if (Crypto.VerifyHashedPassword(userToFind.Password, user.Password)) {
                        SignIn(userToFind);
                    }
                } else
                {
                    ViewBag.Error = "User not found!";
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(Models.User user)
        {
            if (ModelState.IsValid)
            {
                using (var db = new TodoAppDbContext())
                {
                    User userFound = db.Users.SingleOrDefault(u => u.Username == user.Username);
                    if (userFound != null)
                    {
                        ViewBag.Error("User exist");
                        return RedirectToAction("Index", "Home");
                    }

                    db.Users.Add(new Models.User
                    {
                        Email = user.Email,
                        Username = user.Username,
                        Password = Crypto.HashPassword(user.Password)
                    });

                    db.SaveChanges();
                    SignIn(user);
                }
            }
            else
            {
                ModelState.AddModelError("", "Data is incorrect");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult AddClient(Client client)
        {
            if(ModelState.IsValid)
            {
                using (var db = new TodoAppDbContext())
                {
                    db.Clients.Add(new Client
                    {
                        ClientID = client.ClientID,
                        Client_Secret = client.Client_Secret,
                        Redirect_URI = client.Redirect_URI,
                        Created_On = DateTime.Now,
                        UserID = User.Identity.GetUserId<int>()
                    });

                    db.SaveChanges();
                }
            } else
            {
                ModelState.AddModelError("", "Data is incorrect");
            }

            return RedirectToAction("Manage", "Home");
        }

        private void SignIn(User user)
        {
            ClaimsIdentity claims = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
            claims.AddClaim(new Claim(ClaimTypes.Name, user.Username));
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()));

            HttpContext.GetOwinContext().Authentication.SignIn(claims);
        }
    }
}