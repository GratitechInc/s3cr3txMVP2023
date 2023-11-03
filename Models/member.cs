using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
namespace s3cr3tx.Models
{
    
    public class Member
    {
        public Member()
        {
        }
        [Display(Name = @"First Name")]
        [Required(ErrorMessage = @"First Name is Required")]
        public string FirstName { get; set; } = "";
        [Display(Name = @"Last Name")]
        [Required(ErrorMessage = @"Last Name is Required")]
        public string LastName { get; set; } = "";
        [Display(Name = @"Email")]
        [Required(ErrorMessage = @"Email is Required")]
        [RegularExpression("^[a-zA-Z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid email format.")]
        public string email { get; set; } = "";
        [DataType(DataType.Password)]
        [Required(ErrorMessage = @"Password is Required")]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[-!@#$%^&*()<_>+=]).{8,40})",ErrorMessage = "Password must be at least 8 characters long, contain lower and uppercase characters and any of the following symbols !@#$%^&*()<_>+=")]
        [Display(Name = @"Password")]
        public string Code { get; set; } = "";
        [Required(ErrorMessage = @"Confirm Password is Required")]
        [DataType(DataType.Password)]
        [Compare("Code", ErrorMessage = @"Password and Confirm Password must match")]
        [Display(Name = @"Confirm Password")]
        public string ConfirmCode { get; set; } = "";
        [Required]
        public long id { get; set; } = 0;

        public string regcode { get; set; } = Guid.NewGuid().ToString();

        public bool isValid { get; set; } = false;

        public bool isLoggedIn { get; set; } = false;
        //[Required(ErrorMessage = "Mobile is required")]
        [RegularExpression(@"\d{10}", ErrorMessage = "Please enter 10 digit Mobile No.")]
        [Display(Name = @"Mobile")]
        public string mobile
        {
            get;
            set;
        } = "";
        [Display(Name = @"Mobile Carrier")]
        //[Required(ErrorMessage = "Mobile is required")]
        public string MobileCarrier
        {
            get;
            set;
        } = "";
        [Display(Name = @"Country")]
        public string country
        {
            get;
            set;
        } = "";
        [Display(Name = @"State")]
        public string state
        {
            get;
            set;
        } = "";
        [Display(Name = @"City")]
        public string city
        {
            get;
            set;
        } = "";
        [Display(Name = @"Zip Code")]
        [RegularExpression(@"\d{5}", ErrorMessage = "Please enter 5 digit Zip Code")]
        public string zipcode
        {
            get;
            set;
        } = "";
        [Display(Name = @"Address")]
        public string address
        {
            get;
            set;
        } = "";
        [Display(Name = @"Address Line 2")]
        public string address2
        {
            get;
            set;
        } = "";
        [Display(Name = @"Gender")]
        public string gender
        {
            get;
            set;
        } = "";
        public string message
        {
            get;
            set;
        } = "";
        private static DateTime now1 = DateTime.Now;
        [Display(Name = @"Created")]
        public DateTime created
        {
            get;
            set;
        } = now1;
        [Display(Name = @"Updated")]
        public DateTime updated
        {
            get;
            set;
        } = now1;
        [Display(Name = @"Enabled")]
        public bool enabled
        {
            get;
            set;
        } = false;
        [Display(Name = @"Confirmed")]
        public bool confirmed
        {
            get;
            set;
        } = false;
        public string orderNumber
        {
            get;
            set;
        } = "";
    }
}
