using Aljmal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
namespace Aljmal.Controllers
{
    //[Authorize(Roles = "Seller")]
    public class SellerDashboardController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public SellerDashboardController(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.ActivePage = "dashboard";
            var client = _clientFactory.CreateClient("ApiClient");
            //var token = Request.Cookies["AuthToken"];
            ////var token= "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI0M2VkMTY4ZC1hY2FkLTRiMGYtOTJjNS0wYmI2ZjBiOTdhYmIiLCJqdGkiOiJlZGIwYWM1My03MzcyLTQ4NzktOTI5Ny1jMjhkNWNjYmJjYTgiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJTZWxsZXIiLCJleHAiOjE3NTEwMDg2NDB9._ - _8A6zlLXT6B - m85yXjFKC - lhu3yzMAydWptt97Vr4";
            //if (string.IsNullOrEmpty(token))
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync("api/Seller/GetSellerInfo");

                if (response.IsSuccessStatusCode)
                {
                    var cont = await response.Content.ReadAsStringAsync();
                    var content = await response.Content.ReadFromJsonAsync<sellerInfo>();

                    if (content == null)
                    {
                        return RedirectToAction("Index", "SellerDashboard");
                    }
                    return View(content);
                }
                else
                {
                    var res = response.RequestMessage;
                }

            }
            catch (HttpRequestException ex)
            {
                // API call failed
                Console.WriteLine(ex.Message);
            }
            return View();
        }
        [HttpGet]
        public IActionResult ImportExcel()
        {
            ViewBag.ActivePage = "excel";
            return View();
        }
        public async Task<IActionResult> ShowProduct()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            //var token = Request.Cookies["AuthToken"];

            //if (string.IsNullOrEmpty(token))
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync("api/Seller/GetAllProductsBySeller");

                if (response.IsSuccessStatusCode)
                {
                    var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
                    if (products == null)
                    {
                        ViewBag.ActivePage = "dashboard";
                        return RedirectToAction("Index", "SellerDashboard");
                    }
                    ViewBag.ActivePage = "products";
                    return View("Products", products);
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
        public IActionResult CreateProduct()
        {
            ViewBag.ActivePage = "products";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct(CreateProductDto model)
        {
            ViewBag.ActivePage = "products";
            if (model.image == null || model.image.Length == 0)
            {
                ModelState.AddModelError("", "Photo is required");
                return View(model);
            }

            var client = _clientFactory.CreateClient("ApiClient");
            //var token = Request.Cookies["AuthToken"];

            //if (string.IsNullOrEmpty(token))
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new MultipartFormDataContent();

            // Add all properties as individual form fields (not JSON)
            content.Add(new StringContent(model.ProductName), "ProductName");
            content.Add(new StringContent(model.EnglishName), "EnglishName");
            content.Add(new StringContent(model.Price.ToString()), "Price"); // Ensure correct data type
            content.Add(new StringContent(model.Detials), "Detials");
            // Add other properties like subId if needed

            // Add the image file
            var streamContent = new StreamContent(model.image.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(model.image.ContentType);
            content.Add(streamContent, "image", model.image.FileName);

            // Send to API
            HttpResponseMessage response = await client.PostAsync("api/Seller/CreateProduct", content);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.ActivePage = "products";

                return RedirectToAction("ShowProduct", "SellerDashboard");
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
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            //var token = Request.Cookies["AuthToken"];

            //if (string.IsNullOrEmpty(token))
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //// Call the API to delete the product
            HttpResponseMessage response = await client.DeleteAsync($"api/Seller/deleteProduct/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ShowProduct", "SellerDashboard");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Failed to delete product: {errorContent}";
                return RedirectToAction("ShowProduct", "SellerDashboard");
            }
        }
        public async Task<ActionResult> GetAllOrders()
        {
            ViewBag.ActivePage = "orders";
            var client = _clientFactory.CreateClient("ApiClient");
            //var token = Request.Cookies["AuthToken"];

            //if (string.IsNullOrEmpty(token))
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync("api/Seller/sellerOrders");

                if (response.IsSuccessStatusCode)
                {
                    var orders = await response.Content.ReadFromJsonAsync<IEnumerable<GetOrdersDto>>();
                    if (orders == null)
                    {
                        return RedirectToAction("Index", "SellerDashboard");
                    }
                    return View("Orders", orders);
                }

            }
            catch (HttpRequestException ex)
            {
                // API call failed
                Console.WriteLine(ex.Message);
            }
            return View("Index");
        }
        public async Task<ActionResult> OrderDetails(int id)
        {
            ViewBag.ActivePage = "orders";
            var client = _clientFactory.CreateClient("ApiClient");
            //var token = Request.Cookies["AuthToken"];

            //if (string.IsNullOrEmpty(token))
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync($"api/Seller/GetOrderById/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var orders = await response.Content.ReadFromJsonAsync<List<GetOrderInfo>>();
                    if (orders == null)
                    {
                        return RedirectToAction("Index", "SellerDashboard");
                    }
                    return View(orders);
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
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.ActivePage = "products";
            var client = _clientFactory.CreateClient("ApiClient");
            //var token = Request.Cookies["AuthToken"];

            //if (string.IsNullOrEmpty(token))
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync($"api/Seller/getProductById/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var product = await response.Content.ReadFromJsonAsync<ProductDto>();

                    return View(product);
                }

            }
            catch (HttpRequestException ex)
            {
                // API call failed
                Console.WriteLine(ex.Message);
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateProductDto model, IFormFile? image)
        {
            ViewBag.ActivePage = "products";
            var client = _clientFactory.CreateClient("ApiClient");
            //var token = Request.Cookies["AuthToken"];

            //if (string.IsNullOrEmpty(token))
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new MultipartFormDataContent();

            // Add all properties as individual form fields (not JSON)
            content.Add(new StringContent(model.Id.ToString()), "Id");
            content.Add(new StringContent(model.ProductName), "ProductName");
            content.Add(new StringContent(model.EnglishName), "EnglishName");
            content.Add(new StringContent(model.Price.ToString()), "Price"); // Ensure correct data type
            content.Add(new StringContent(model.Detials), "Detials");
            // Add other properties like subId if needed

            // Add the image file
            if (image != null)
            {
                var streamContent = new StreamContent(image.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
                content.Add(streamContent, "image", image.FileName);
            }
            // Send to API
            HttpResponseMessage response = await client.PutAsync("api/Seller/updateProduct", content);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.ActivePage = "products";

                return RedirectToAction("ShowProduct", "SellerDashboard");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"API Error: {errorContent}");
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> DownloadOrderPdf(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            //var token = Request.Cookies["AuthToken"];

            //if (string.IsNullOrEmpty(token))
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync($"api/Seller/GetOrderInfo/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var orders = await response.Content.ReadFromJsonAsync<OrderToFileDto>();
                    if (orders == null)
                    {
                        return RedirectToAction("Index", "SellerDashboard");
                    }
                    var document = new InvoiceDocument(orders);
                    var stream = new MemoryStream();
                    document.GeneratePdf(stream);
                    stream.Position = 0;

                    return File(stream, "application/pdf", "Invoice.pdf");
                }
            }
            catch (HttpRequestException ex)
            {
                // API call failed
                Console.WriteLine(ex.Message);
            }
            return View();
        }
    }
}

//var pdfDocument = Document.Create(container =>
//{
//    container.Page(page =>
//    {
//        page.Margin(30);
//        page.Content().Column(column =>
//        {

//            column.Item().Text("طلب خالد شيخ").FontSize(20).Bold().AlignCenter();
//            column.Item().Text("تاريخ: 24/5/2025").FontSize(16).AlignCenter();

//            column.Item().Table(table =>
//            {
//                table.ColumnsDefinition(columns =>
//                {
//                    columns.RelativeColumn();
//                    columns.RelativeColumn();
//                    columns.RelativeColumn();
//                    columns.RelativeColumn();
//                    columns.RelativeColumn();
//                });

//                table.Header(header =>
//                {
//                    header.Cell().Text("اسم المنتج").Bold();
//                    header.Cell().Text("سعر الوحدة").Bold();
//                    header.Cell().Text("الكمية").Bold();
//                    header.Cell().Text("المجموع").Bold();
//                });

//                // Add rows here
//                table.Cell().Text("المنتج 1");
//                table.Cell().Text("40 ر.ي");
//                table.Cell().Text("5");
//                table.Cell().Text("200 ر.ي");

//                table.Cell().Text("المنتج 2");
//                table.Cell().Text("80 ر.ي");
//                table.Cell().Text("2");
//                table.Cell().Text("160 ر.ي");

//                table.Footer(footer =>
//                {
//                    footer.Cell().ColumnSpan(3).Text("المجموع:").Bold();
//                    footer.Cell().Text("800 ر.ي").Bold();
//                });
//            });
//        });
//    });
//});
//var pdfBytes = pdfDocument.GeneratePdf();
//return File(pdfBytes, "application/pdf", "Order.pdf");
//        }
