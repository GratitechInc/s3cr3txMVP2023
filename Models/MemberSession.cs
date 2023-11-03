using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace s3cr3tx.Models
{
	public class MemberSession
	{
		public MemberSession()
		{
		}
        public long id { get; set; } = 0;
        public string session_code { get; set; } = Guid.NewGuid().ToString();
        public long member_id { get; set; } = 0;
        public string member_token{ get; set; } = Guid.NewGuid().ToString();
        public string member_code { get; set; } = RandomNumberGenerator.GetInt32(1000000).ToString();
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string member_ip { get; set; } = "";
        public string member_email{ get; set; } = "";
        public DateTime session_start { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = false;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public DateTime SessionExpires { get; set; } = DateTime.Now.AddMinutes(20);  
    }
}

