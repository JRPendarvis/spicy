﻿using System;
using System.IO;
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

			if (files.Any())
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

	}
}
