using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace Spice.Models
{
	public class User
	{
		[Key]
		public int ID { get; set; }
		[Required]
		public string FName { get; set; }
		[Required]
		public string LName { get; set; }

		public string MI { get; set; }
	}
}
