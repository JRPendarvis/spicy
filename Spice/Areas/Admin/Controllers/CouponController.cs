using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Spice.Areas.Admin.Controllers
{
	public class CouponController : BaseController
	{
		public CouponController(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<IActionResult> Index()
		{
			return View(await _db.Coupon.ToListAsync());
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Coupon coupons)
		{
			if (!ModelState.IsValid) return View(coupons);

			coupons = await GetPhotoForCoupon(coupons);

			await _db.Coupon.AddAsync(coupons);
			await _db.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var coupon = await _db.Coupon.SingleOrDefaultAsync(m =>m.Id == id);
			if (coupon == null)
			{
				return NotFound();
			}

			return View(coupon);

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Coupon coupons)
		{
			if (coupons.Id == 0)
			{
				return NotFound();
			}

			var couponFromDb = await _db.Coupon.Where(c => c.Id == coupons.Id).FirstOrDefaultAsync();

			if (!ModelState.IsValid) return View(coupons);
			couponFromDb               = await GetPhotoForCoupon(couponFromDb);
			couponFromDb.MinimumAmount = coupons.MinimumAmount;
			couponFromDb.Name          = coupons.Name;
			couponFromDb.Discount      = coupons.Discount;
			couponFromDb.CouponType    = coupons.CouponType;
			couponFromDb.IsActive      = coupons.IsActive;

			await _db.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var coupon = await _db.Coupon.FirstOrDefaultAsync(m => m.Id == id);
			if (coupon == null)
			{
				return NotFound();
			}

			return View(coupon);

		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var coupon = await _db.Coupon.SingleOrDefaultAsync(m => m.Id == id);
			if (coupon == null)
			{
				return NotFound();
			}

			return View(coupon);

		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		
		public async Task<IActionResult> DeleteRecord(int id)
		{
			var coupon = await _db.Coupon.SingleOrDefaultAsync(m => m.Id == id);
			_db.Coupon.Remove(coupon);
			await _db.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}


		private async Task<Coupon> GetPhotoForCoupon(Coupon coupons)
		{
			var files = HttpContext.Request.Form.Files;

			if (files.Count <= 0) return coupons;

			byte[] pictureOfCoupon = null;
			await using (var fileStream = files[0].OpenReadStream())
			{
				await using var memoryStream = new MemoryStream();
				fileStream.CopyTo(memoryStream);
				pictureOfCoupon = memoryStream.ToArray();
			}

			coupons.Picture = pictureOfCoupon;

			return coupons;
		}

	}
}
