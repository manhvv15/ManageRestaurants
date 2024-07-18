using ManageRestaurant.DTO;
using ManageRestaurant.Helper;
using ManageRestaurant.Models;
using ManageRestaurant.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ManageRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly ManageRestaurantContext context;
        public MenuController(ManageRestaurantContext context)
        {
            this.context = context;
        }

        [HttpGet("getMenu")]
        [Authorize]
        
        public async Task<ActionResult<Menu>> GetAllMenuItems()
        {
            //    var menuItems = await context.Menus
            //        .Select(mi => new MenuDTO
            //        {
            //            Name = mi.Name,
            //            Address = mi.Restaurant.Address,
            //            Phone = mi.Restaurant.Phone,
            //            Email = mi.Restaurant.Email,
            //            Description = mi.Description,
            //            Rating = (double)mi.Restaurant.Rating
            //        })
            //        .ToListAsync();

            return Ok();
        }


        [HttpPost("importExcel")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportMenusFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please upload a valid Excel file.");

            var menus = new List<Menu>();
            var errors = new List<MenuError>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0);
                Excel excel = new Excel();
                // Use CheckFileMau function to validate the file
                string validationMessage = excel.CheckFileMau(sheet, "Menu", 2);
                if (!string.IsNullOrEmpty(validationMessage))
                {
                    return BadRequest(validationMessage);
                }

                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    if (row == null) continue;

                    try
                    {
                        var menu = new Menu
                        {
                            Name = row.GetCell(0)?.ToString() ?? string.Empty,
                            Description = row.GetCell(1)?.ToString() ?? string.Empty,
                            Price = row.GetCell(2) != null && decimal.TryParse(row.GetCell(2).ToString(), out decimal price) ? price : 0,
                            ImageUrl = row.GetCell(3)?.ToString() ?? string.Empty
                        };

                        menus.Add(menu);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new MenuError
                        {
                            NameDuLieu = "Menu Data",
                            ViTri = $"Row {rowIndex}",
                            ThuocTinh = "Unknown",
                            DienGiai = ex.Message,
                            RowError = rowIndex.ToString()
                        });
                    }
                }
            }

            if (errors.Count > 0)
            {
                return BadRequest(new { Message = "Errors occurred during import", Errors = errors });
            }

            await context.Menus.AddRangeAsync(menus);
            await context.SaveChangesAsync();

            return Ok("Menus imported successfully.");
        }

    }
}
