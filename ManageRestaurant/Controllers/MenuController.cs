using ManageRestaurant.DTO;
using ManageRestaurant.Helper;
using ManageRestaurant.Interface;
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
        private readonly IMenuRepository _menuRepository;
        private readonly ManageRestaurantContext context;
       

        public MenuController(ManageRestaurantContext context,IMenuRepository menuRepository)
        {
            this.context = context;
            _menuRepository = menuRepository;
        }

        [HttpGet("GetMenusAsync")]
        [Authorize]
        
        public async Task<ActionResult> GetMenusAsync()
        {
            var menuItems = await _menuRepository.GetMenusAsync();
            return Ok(menuItems);
        }

        [HttpPost("GetMenuByIdAsync")]
        [Authorize(Roles = $"{AppRole.Admin},{AppRole.Staff}")]

        public async Task<ActionResult> GetMenuByIdAsync([FromBody] int id)
        {
            var result = await _menuRepository.GetMenuByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("AddMenuAsync")]
        [Authorize(Roles = $"{AppRole.Admin},{AppRole.Staff}")]

        public async Task<ActionResult> AddMenuAsync([FromBody] Menu menu)
        {
            var result = await _menuRepository.AddMenuAsync(menu);
            return Ok(result);
        }

        [HttpPost("UpdateMenuAsync")]
        [Authorize(Roles = $"{AppRole.Admin},{AppRole.Staff}")]

        public async Task<ActionResult> UpdateMenuAsync(int id, [FromBody] MenuDTO menu)
        {
            var result = await _menuRepository.UpdateMenuAsync(id ,menu);
            return Ok(result);
        }

        [HttpPost("DeleteMenuAsync")]
        [Authorize(Roles = $"{AppRole.Admin},{AppRole.Staff}")]

        public async Task<ActionResult> DeleteMenuAsync([FromBody] int id)
        {
            var result = await _menuRepository.DeleteMenuAsync(id);
            return Ok(result);
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
