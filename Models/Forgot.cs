using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace s3cr3tx.Models
{
	public class Forgot
	{
		public Forgot()
		{
		}
		public string id { get; set; } = "";
		[Required(ErrorMessage = "Email is Required")]
		[Display(Name = "Email: ")]
		public string member_email { get; set; } = "";
		public string member_code { get; set; } = Guid.NewGuid().ToString();
		public string Output { get; set; } = "";
		public string IP { get; set; } = "";
		public DateTime session_start { get; set; } = DateTime.Now;
		public DateTime code_expires { get; set;} = DateTime.Now.AddMinutes(20);
    }
}

