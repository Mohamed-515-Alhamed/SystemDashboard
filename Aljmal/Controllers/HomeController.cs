using Aljmal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

namespace Aljmal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string baseUrl;
        private readonly IHttpClientFactory _clientFactory;

        
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _clientFactory=httpClientFactory;
        }
        //[Authorize]
        public async Task<IActionResult> Index()
        {
            // Check local cookie first
            if (Request.Cookies["DashboardType"] != null)
            {
                return RedirectBasedOnCookie();
            }

            // Call API to verify roles
            var client = _clientFactory.CreateClient("ApiClient");
            var token = Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync("api/Auth/userinfo");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadFromJsonAsync<UserInfoResponse>();
                    var c=await response.Content.ReadAsStringAsync();
                    if (content.Role == "Admin")
                    {
                        Response.Cookies.Append("DashboardType", "Admin");
                        return RedirectToAction("Index", "AdminDashboard");
                    }
                    else if (content.Role == "Seller")
                    {
                        Response.Cookies.Append("DashboardType", "Seller");
                        return RedirectToAction("Index", "SellerDashboard");
                    }
                }
            }
            catch (HttpRequestException)
            {
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("Login","Account");
            
        }
        private IActionResult RedirectBasedOnCookie()
        {
            var dashboardType = Request.Cookies["DashboardType"];
            return dashboardType switch
            {
                "Admin" => RedirectToAction("Index", "AdminDashboard"),
                "Seller" => RedirectToAction("Index", "SellerDashboard"),
                _ => RedirectToAction("AccessDenied", "Home")
            };
        }
        public IActionResult AccessDenied()
        {
            return RedirectToAction("Login","Account");
        }
        public class UserInfoResponse
        {
            public string Username { get; set; }
            public string Role { get; set; }
        }
    }
}