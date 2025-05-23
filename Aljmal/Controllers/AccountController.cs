using Aljmal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aljmal.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public AccountController(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _clientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("api/Auth/LoginAdmins", new
            {
                Email = model.Email,
                Password = model.Password
            });
           
            if (response.IsSuccessStatusCode)
            {
                var authResult = await response.Content.ReadFromJsonAsync<AuthResult>();
                string role = authResult.User?.Role;
                var tok = authResult.Token;
                Response.Cookies.Append("AuthToken", authResult.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = authResult.Expiration
                });
                if (role == "Seller")
                {
                    return RedirectToAction("Index", "SellerDashboard");
                }
                if (role == "Admin")
                {
                    return RedirectToAction("Index", "AdminDashboard");
                }
                return View();
            }

            ModelState.AddModelError(string.Empty, $"{response.ReasonPhrase}");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Index", "Home");
        }
    }



    public class AuthResult
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public UserDetails User { get; set; } // Update this to include user details
    }

    public class UserDetails
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}

