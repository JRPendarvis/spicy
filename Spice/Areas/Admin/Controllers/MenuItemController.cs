using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class MenuItemController : Controller
	{
		private readonly ApplicationDbContext _db;
		private readonly IWebHostEnvironment _hostingEnvironment;

		[BindProperty]
		public MenuItemViewModel MenuItemVM { get; set; }

		public MenuItemController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
		{
			_db = db;
			_hostingEnvironment = hostEnvironment;
			MenuItemVM = new MenuItemViewModel()
			{
				Category = _db.Category,
				MenuItem = new MenuItem()
			};
		}
		public async Task<IActionResult> Index()
		{
			var menuItems = await _db.MenuItem.Include(m=>m.Category).Include(m=>m.SubCategory).ToListAsync();
			return View(menuItems);
		}

		//Get Create
		public IActionResult Create()
		{
			return View(MenuItemVM);
		}

		[HttpPost, ActionName("Create")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreatePost()
		{
			MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

			if (!ModelState.IsValid)
			{
				return View(MenuItemVM);
			}

			_db.MenuItem.Add(MenuItemVM.MenuItem);
			await _db.SaveChangesAsync();

			string webRootPath = _hostingEnvironment.WebRootPath;

			var files = HttpContext.Request.Form.Files;

			var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

			if (EnumerableExtensions.Any(files))
			{
				var uploads = Path.Combine(webRootPath, "images");
				var extention = Path.GetExtension(files[0].FileName);

				await using (var fileStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extention),
					FileMode.Create))
				{
					await files[0].CopyToAsync(fileStream);
				}

				menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extention;
			}
			else
			{
				var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
				System.IO.File.Copy(uploads, webRootPath+@"\images\" + MenuItemVM.MenuItem.Id + ".png");

				menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".png";
			}

			await _db.SaveChangesAsync();

			return RedirectToAction(nameof(Index));

		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
				return NotFound();

			MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category)
				                                    .Include(m => m.SubCategory)
				                                    .SingleOrDefaultAsync(m => m.Id == id);

			MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

			if (MenuItemVM.MenuItem == null)
				return NotFound();

			return View(MenuItemVM);
		}

		[HttpPost, ActionName("Edit")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditPost(int? id)
		{
			if (id == null)
				return NotFound();


			MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

			if (!ModelState.IsValid)
			{
				MenuItemVM.SubCategory = await _db.SubCategory
					.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId)
					.ToListAsync();
				return View(MenuItemVM);
			}


			string webRootPath = _hostingEnvironment.WebRootPath;
			var files = HttpContext.Request.Form.Files;

			var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

			if (EnumerableExtensions.Any(files))
			{
				var uploads = Path.Combine(webRootPath, "images");
				var extention_new = Path.GetExtension(files[0].FileName);

				var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));

				if(System.IO.File.Exists(imagePath))
				{
					System.IO.File.Delete(imagePath);
				}


				await using (var fileStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extention_new),
					FileMode.Create))
				{
					await files[0].CopyToAsync(fileStream);
				}

				menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extention_new;
			}

			menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
			menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
			menuItemFromDb.Spicyness = MenuItemVM.MenuItem.Spicyness;
			menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
			menuItemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;

			await _db.SaveChangesAsync();

			return RedirectToAction(nameof(Index));

		}

		//GET Details
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category)
				                             .Include(m => m.SubCategory)
				                             .SingleOrDefaultAsync(m => m.Id == id);
			if (MenuItemVM.MenuItem == null)
			{
				return NotFound();
			}

			return View(MenuItemVM);
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category)
													.Include(m => m.SubCategory)
													.SingleOrDefaultAsync(m => m.Id == id);
			if (MenuItemVM.MenuItem == null)
			{
				return NotFound();
			}

			return View(MenuItemVM);
		}

		//POST Delete
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			string webRootPath = _hostingEnvironment.WebRootPath;

			MenuItem menuItem = await _db.MenuItem.FindAsync(id);
			if (menuItem == null) return RedirectToAction(nameof(Index));

			var imagePath = Path.Combine(webRootPath, menuItem.Image.TrimStart('\\'));

			if (System.IO.File.Exists(imagePath))
			{
				System.IO.File.Delete(imagePath);
			}
			_db.MenuItem.Remove(menuItem);
			await _db.SaveChangesAsync();


			return RedirectToAction(nameof(Index));
		}

	}
}
