using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http;

namespace ManageRestaurantsClient.Pages.Admin
{
    [Authorize]
    public class menuModel : PageModel
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly IHttpClientFactory _clientFactory;

        public menuModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostImportExcelAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please upload a valid Excel file.");

            try
            {
                var client = _clientFactory.CreateClient();
                using (var content = new MultipartFormDataContent())
                {
                    var token = Request.Cookies["AuthToken"];
                    if (string.IsNullOrEmpty(token))
                        return Unauthorized();

                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, "file", file.FileName);

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await client.PostAsync("https://localhost:5000/api/Menu/importExcel", content);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Import Success";
                        return RedirectToPage("/Admin/Menu"); // Redirect to a success page or show a success message
                    }
                    else
                    {
                        TempData["Error"] = "Vui long nhap file dung dinh dang";
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        return RedirectToPage("/Admin/Menu");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred", Details = ex.Message });
            }
        }
    }
}
