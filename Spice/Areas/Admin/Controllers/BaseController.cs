using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spice.Data;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
	[Authorize(Roles = SD.ManagerUser)]
	[Area("Admin")]
	public class BaseController : Controller
	{
		public ApplicationDbContext _db;
	}
}
