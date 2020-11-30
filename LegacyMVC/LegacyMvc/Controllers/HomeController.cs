using ETaxPoc.Models.UserModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Xml.Serialization;


namespace ETaxPoc.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {
        }

        public IActionResult Index()
        {
            var isAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            if (isAuthenticated)
            {
                TempDataMessage("message", "success", $"Hi {HttpContext.User.Identity.Name}!!!");
                return View("Authenticated", "Home");
            }
            return View();
        }

        [Authorize]
        public IActionResult Authenticated()
        {
            var isAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            TempDataMessage("message", "success", $"Hi {HttpContext.User.Identity.Name}!!!");
            return View();
        }

        // Return JWT token which can be used to authenticate this user in the new backend
        [Authorize]
        public string Token()
        {
            var isAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                throw new UnauthorizedAccessException();
            }
            return generateJwtToken(new User () {
                Username = HttpContext.User.Identity.Name 
            });
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = model.Email == "test@test.com" && model.Password == "password" ? new User()
                {
                    Username = "David John"
                } : null;

                if (user == null)
                {
                    TempDataMessage("message", "danger", $"Incorrect Password or Email.");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Username.ToString()) }, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(new ClaimsPrincipal(identity),
                        new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.MaxValue,
                            AllowRefresh = true
                        });
                }
                return RedirectToAction("Authenticated", "Home");
            }
            else
            {
                TempDataMessage("message", "danger", $"Login form datas is not valid");
                return RedirectToAction("Index", "Home");
            }
        }
        public void TempDataMessage(string key, string alert, string value)
        {
            try
            {
                TempData.Remove(key);
                TempData.Add(key, value);
                TempData.Add("alertType", alert);
            }
            catch
            {
                Debug.WriteLine("TempDataMessage Error");
            }
        }

        private string generateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("this is a very long secret which need to be the same in both backend");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("id", user.Username.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
