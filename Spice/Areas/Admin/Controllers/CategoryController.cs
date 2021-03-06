using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Utility;
using System.Threading.Tasks;

namespace Spice.Areas.Admin.Controllers
{
	public class CategoryController : BaseController
	{
	
		public CategoryController(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<IActionResult> Index()
		{
			return View(await _db.Category.ToListAsync());
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Category category)
		{
			if (ModelState.IsValid)
			{
				await _db.Category.AddAsync(category);
				await _db.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}
			else
			{
				return View(category);
			}
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var category = await _db.Category.FindAsync(id);
			if (category == null)
			{
				return NotFound();
			}

			return View(category);

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Category category)
		{
			if (!ModelState.IsValid) return View((category));
			
			_db.Update(category);
			await _db.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var category = await _db.Category.FindAsync(id);
			if (category == null)
			{
				return NotFound();
			}

			return View(category);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			
			var category = await _db.Category.FindAsync(id);
			if (category == null)
			{
				return View();
			}
			_db.Category.Remove(category);
			await _db.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var category = await _db.Category.FindAsync(id);
			if (category == null)
			{
				return NotFound();
			}

			return View(category);
		}

		[HttpPost, ActionName("Details")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DetailActionResult(int? id)
		{

			var category = await _db.Category.FindAsync(id);
			if (category == null)
			{
				return View();
			}

			return RedirectToAction("Edit",new { id });
		}
	}
}
