using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace s3cr3tx.Models
{
	public class Login
	{
		public Login()
		{
		}
        public string id { get; set; } = "";
		[Required(ErrorMessage = "Email is Required")]
        [Display(Name = "Email: ")]
        public string member_email { get; set; } ="";
		[Required(ErrorMessage="Password is Required")]
		[DataType(DataType.Password)]
		[Display(Name = "Password: ")]
		public string member_code { get; set; } = "";
        public string Output { get; set; } = "";
        public DateTime session_start { get; set; } = DateTime.Now;  
    }
}

