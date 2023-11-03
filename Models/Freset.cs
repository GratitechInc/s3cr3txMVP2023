using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace s3cr3tx.Models
{
	public class Freset
	{
		public Freset()
		{
		}
        public long id { get; set; } = 0;
        [Required(ErrorMessage = "Current Password is Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password: ")]
        public string codeCurrent { get; set; } = "";
        [Required(ErrorMessage="Password is Required")]
		[DataType(DataType.Password)]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()<_>+=]).{8,40})", ErrorMessage = "Password must be at least 8 characters long, contain lower and uppercase characters and any of the following symbols !@#$%^&*()<_>+=")]
        [Display(Name = "New Password: ")]
		public string member_code { get; set; } = "";
        [Required(ErrorMessage = "Confirm Password is Required")]
        [DataType(DataType.Password)]
        [Compare("member_code", ErrorMessage = @"Password and Confirm Password must match")]
        [Display(Name = "Confirm New Password: ")]
        public string member_code2 { get; set; } = "";
        public string Output { get; set; } = "";
        public string email { get; set; } = "";
        public DateTime session_expires { get; set; } = DateTime.Now;
        public DateTime session_start { get; set; } = DateTime.Now;
    }
}

