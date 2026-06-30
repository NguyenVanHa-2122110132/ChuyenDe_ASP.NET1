/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 25/06/2026
    Mô tả    : MVC Controller quản lý cấu hình khoảng giá (Trang Admin)
*/
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    public class PriceSettingController : BaseAdminController
    {
        private readonly ApplicationDbContext _context;

        public PriceSettingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /PriceSetting
        public async Task<IActionResult> Index()
        {
            var setting = await _context.PriceSettings.FirstOrDefaultAsync();
            if (setting == null)
            {
                setting = new PriceSetting
                {
                    MinPrice = 0,
                    MaxPrice = 10000000,
                    Label = "Tất cả"
                };
            }
            return View(setting);
        }

        // POST: /PriceSetting/Save
        [HttpPost]
        public async Task<IActionResult> Save(decimal minPrice, decimal maxPrice, string? label)
        {
            if (minPrice < 0 || maxPrice <= 0 || minPrice >= maxPrice)
            {
                TempData["Error"] = "Khoảng giá không hợp lệ!";
                return RedirectToAction("Index");
            }

            var setting = await _context.PriceSettings.FirstOrDefaultAsync();
            if (setting == null)
            {
                _context.PriceSettings.Add(new PriceSetting
                {
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    Label = label ?? "Tất cả",
                    UpdatedAt = DateTime.Now
                });
            }
            else
            {
                setting.MinPrice = minPrice;
                setting.MaxPrice = maxPrice;
                setting.Label = label ?? setting.Label;
                setting.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật khoảng giá thành công!";
            return RedirectToAction("Index");
        }
    }
}