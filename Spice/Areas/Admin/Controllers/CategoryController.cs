﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spice.Data;

namespace Spice.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CategoryController : Controller
	{
		private readonly ApplicationDbContext _db;
		
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
	}
}