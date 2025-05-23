using Aljmal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Aljmal.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public AdminDashboardController(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory=clientFactory;
            _configuration=configuration;
        }
        public ActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> ShowSellers()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            var token = Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync("api/Admin/GetAllSellers");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var products = await response.Content.ReadFromJsonAsync<IEnumerable<SubAdminDto>>();
                    if (products == null)
                    {
                        return RedirectToAction("Index", "SellerDashboard");
                    }
                    return View("Seller", products);
                }

            }
            catch (HttpRequestException ex)
            {
                // API call failed
                Console.WriteLine(ex.Message);
            }
            return View("Index");

        }
        [HttpGet]
        public ActionResult CreateSeller()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateSeller(Register model)
        {
            if (model.image == null || model.image.Length == 0)
            {
                ModelState.AddModelError("", "Photo is required");
                return View(model);
            }

            var client = _clientFactory.CreateClient("ApiClient");
            var token = Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new MultipartFormDataContent();

            // Add all properties as individual form fields (not JSON)
            content.Add(new StringContent(model.Name), "Name");
            content.Add(new StringContent(model.Email), "Email");
            content.Add(new StringContent(model.Password), "Password");
            content.Add(new StringContent(model.PublicNaame), "PublicName");
            content.Add(new StringContent(model.PhoneNumber), "PhoneNumber");
            content.Add(new StringContent(model.Role), "Role");
            // Add other properties like subId if needed

            // Add the image file
            var streamContent = new StreamContent(model.image.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(model.image.ContentType);
            content.Add(streamContent, "image", model.image.FileName);

            // Send to API
            HttpResponseMessage response = await client.PostAsync("api/Admin/CreateSellers", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Seller", "AdminDashboard");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"API Error: {errorContent}");
                return View(model);
            }
            ModelState.AddModelError("", "photo is rreguerd");
            return View(model);
            
        }
        [HttpPost]
        public async Task<ActionResult> DeleteSeller(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            var token = Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Call the API to delete the product
            HttpResponseMessage response = await client.DeleteAsync($"api/Admin/deleteSeller/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Seller", "AdminDashboard");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Failed to delete seller: {errorContent}";
                return RedirectToAction("ShowProduct", "SellerDashboard");
            }
        }

        

    }
}
