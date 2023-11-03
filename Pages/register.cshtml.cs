using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.AspNetCore.Http;
using s3cr3tx.Models;
using System.Text.Json;
using s3cr3tx.Controllers;
using System.Data.SqlClient;
using System.Security;
using System.Security.Cryptography;
using System.Net.Mail;

namespace s3cr3tx.Pages
{
    public class RegisterModel : PageModel
    {
        // private readonly ILogger<IndexModel> _logger;

        public s3cr3tx.Models.Member _member;
        public s3cr3tx.Models.MemberDbContext _members;
        public string _output;

        //public s3cr3tx.Models.MemberSession memberSession;
        //public string _output;
        public RegisterModel(s3cr3tx.Models.MemberDbContext members)
        {
            _members = members;
            _member = new Member();
            _output = _member.message;
        }

        //public s3cr3tx.S3cr3tx S3Cr3Tx;

        public void OnGet([Bind("FirstName", "LastName", "email", "Code", "ConfirmCode", "mobile", "MobileCarrier", "country", "state", "city", "zipcode", "address", "address2", "gender", "message")] s3cr3tx.Models.Member member)
        {
            try
            {

                _member = member;
                _member.message = _output;
            }
            catch (Exception ex)
            {
                string strResult = @"";
                string strSource = @"s3cr3tx.api.RegisterPageCS.OnGet";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);

            }
        }
        public string Message { get; set; } = "";

        public class NewK
        {
            public string email { get; set; }
            public string pd { get; set; }
            public string pd2 { get; set; }
        }
        public void OnPostView([Bind("FirstName", "LastName", "email", "Code", "ConfirmCode", "mobile", "MobileCarrier", "country", "state", "city", "zipcode", "address", "address2", "gender", "message")] s3cr3tx.Models.Member member)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return;
                //}
                string email = _member.email; //= member;

                HttpRequest Request = HttpContext.Request;
                if (Request.Form.TryGetValue("_member.email", out Microsoft.Extensions.Primitives.StringValues Email))
                {
                    if (Request.Form.TryGetValue("_member.FirstName", out Microsoft.Extensions.Primitives.StringValues FirstName))
                    {
                        if (Request.Form.TryGetValue("_member.LastName", out Microsoft.Extensions.Primitives.StringValues LastName))
                        {
                            if (Request.Form.TryGetValue("_member.Code", out Microsoft.Extensions.Primitives.StringValues Code))
                            {
                                if (Request.Form.TryGetValue("_member.ConfirmCode", out Microsoft.Extensions.Primitives.StringValues Code2))
                                {
                                    if (Request.Form.TryGetValue("_member.mobile", out Microsoft.Extensions.Primitives.StringValues Mobile))
                                    {
                                        _member.mobile = Mobile[0];
                                    }
                                    if (Request.Form.TryGetValue("_member.MobileCarrier", out Microsoft.Extensions.Primitives.StringValues MobileCarrier))
                                    {
                                        _member.MobileCarrier = MobileCarrier[0];
                                    }
                                            if (Request.Form.TryGetValue("_member.country", out Microsoft.Extensions.Primitives.StringValues Country))
                                            {
                                                _member.country = Country[0];
                                            }
                                            if (Request.Form.TryGetValue("_member.state", out Microsoft.Extensions.Primitives.StringValues State))
                                            {
                                                _member.state = State[0];
                                            }
                                            if (Request.Form.TryGetValue("_member.city", out Microsoft.Extensions.Primitives.StringValues City))
                                            {
                                                _member.city = City[0];
                                            }
                                            if (Request.Form.TryGetValue("_member.zipcode", out Microsoft.Extensions.Primitives.StringValues zip))
                                            {
                                                _member.zipcode = zip[0];
                                            }
                                            if (Request.Form.TryGetValue("_member.address", out Microsoft.Extensions.Primitives.StringValues Address))
                                            {
                                                _member.address = Address[0];
                                            }
                                            if (Request.Form.TryGetValue("_member.Country", out Microsoft.Extensions.Primitives.StringValues address2))
                                            {
                                                _member.address2 = address2[0];
                                            }
                                            if (Request.Form.TryGetValue("_member.Gender", out Microsoft.Extensions.Primitives.StringValues Gender))
                                            {
                                                _member.gender = Gender[0];
                                            }

                                            _member.email = Email[0].ToLower();
                                            _member.FirstName = FirstName[0];
                                            _member.LastName = LastName[0];
                                            string strResult = @"";
                                            string strRslt = @"";
                                            strResult = uspEncDec.EncDec(@"sales@gratitech.com", Code[0], true, false, out strRslt);

                                            // _member.Code = Controllers.uspEncDec.Enc(@"support@gratitech.com", Code[0])//_member.Code = System.Convert.ToBase64String(System.Security.Cryptography.SHA512.HashData(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding(0), System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding(0).GetBytes(Code[0] + _member.regcode))));
                                            strRslt = Controllers.uspEncDec.Enc(@"sales@gratitech.com", Code[0]);
                                            _member.Code = strResult;
                                            //_member.mobile = Mobile[0];
                                            //_member.MobileCarrier = MobileCarrier[0];
                                            //now insert new member into the database
                                            DateTime dtTimeStamp = DateTime.Now;
                                            //string strResult = @"";
                                            string strConnection = @"Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx";
                                            SqlConnection sql = new SqlConnection(strConnection);
                                            SqlCommand command = new SqlCommand();
                                            command.CommandText = @"dbo.usp_tbl_member_ins";
                                            command.CommandType = System.Data.CommandType.StoredProcedure;
                                            SqlParameter p1 = new SqlParameter(@"member_email", _member.email.Trim());
                                            SqlParameter p2 = new SqlParameter(@"member_code", _member.Code);
                                            SqlParameter p3 = new SqlParameter(@"member_first_name", _member.FirstName.Trim());
                                            SqlParameter p4 = new SqlParameter(@"member_last_name", _member.LastName.Trim());
                                            SqlParameter p5 = new SqlParameter(@"member_reg_number", _member.regcode);
                                            SqlParameter p6 = new SqlParameter(@"member_create_date", dtTimeStamp);
                                            SqlParameter p7 = new SqlParameter(@"member_country", _member.country);
                                            SqlParameter p8 = new SqlParameter(@"member_state", _member.state);
                                            SqlParameter p9 = new SqlParameter(@"member_city", _member.city);
                                            SqlParameter p10 = new SqlParameter(@"member_zip", _member.zipcode);
                                            SqlParameter p11 = new SqlParameter(@"member_address", _member.address);
                                            SqlParameter p12 = new SqlParameter(@"member_address2", _member.address2);
                                            SqlParameter p13 = new SqlParameter(@"member_gender", _member.gender);
                                            SqlParameter p14 = new SqlParameter(@"member_update_date", dtTimeStamp);
                                            SqlParameter p15 = new SqlParameter(@"member_mobile_phone", _member.mobile);
                                            SqlParameter p16 = new SqlParameter(@"member_mobile_carrier", _member.MobileCarrier);

                                            command.Parameters.Add(p1);
                                            command.Parameters.Add(p2);
                                            command.Parameters.Add(p3);
                                            command.Parameters.Add(p4);
                                            command.Parameters.Add(p5);
                                            command.Parameters.Add(p6);
                                            command.Parameters.Add(p7);
                                            command.Parameters.Add(p8);
                                            command.Parameters.Add(p9);
                                            command.Parameters.Add(p10);
                                            command.Parameters.Add(p11);
                                            command.Parameters.Add(p12);
                                            command.Parameters.Add(p13);
                                            command.Parameters.Add(p14);
                                            command.Parameters.Add(p15);
                                            command.Parameters.Add(p16);
                                            long result = 0;
                                            using (sql)
                                            {
                                                sql.Open();
                                                command.Connection = sql;
                                                result = (long)command.ExecuteScalar();
                                            }
                                            //handle result if valid
                                            if (result > 0)
                                            {
                                                //send verification email
                                                MailMessage mail = new MailMessage();
                                                mail.From = new MailAddress(@"support@s3cr3tx.com");
                                                mail.Sender = new MailAddress(@"support@s3cr3tx.com");
                                                mail.Subject = @"s3cr3tx account verification";
                                                mail.To.Add(new MailAddress(_member.email));
                                                mail.IsBodyHtml = true;
                                                mail.Body = @"Dear " + _member.FirstName + @",<br/><br/>Welcome to s3cr3tx! Please click the following link to verify your account: <a href='https://s3cr3tx.com/Validate?=" + System.Web.HttpUtility.UrlEncode(_member.email) + @"_" + _member.regcode + "'>https://s3cr3tx.com/Validate?=" + System.Web.HttpUtility.UrlEncode(_member.email) + @"_" + _member.regcode + @"</a><br/><br/>Thank you!<br/><br/>s3cr3tx sales@gratitech.com";
                                                SmtpClient smtp = new SmtpClient();
                                                smtp.EnableSsl = true;
                                                smtp.Host = GetS3cr3txD(@"SD8d/1ZiIFiNWRiGUYbXbt6fdtOppU3KOipsr5zGlY+Wk/VyHYiOMynMhwvZgv94Xkkewc0ZW7CW/zJ14JaTfLdNDGzeoHTV+Ae/SpTIsmZkLvMX9nkA/HY7F6PzlP96+5gfQkaWufkLEZ19KwdkhOpwJDTUgEOBOZ4LxCH5tQ0ZwrXhH8pHII2mlNkV+zQQAfPjJvBUiiSyk3UoMTw8NZBfYhi9xmFbgkNbFjKO4XUXTxhcFXcikepf80hWwEdZx7ZUKs+mT8us/boGgXvzolyjQZEqBI4+4XeMZ5N5DbBqMWNCy0Fu9kz11ncz+wJUJmOxorYnecPhMuu22AYR0M3tiW65Pb2i9cyzp52pqhCrSpfs7IOlLEmlfZWFINWYXoTvk3MfzgQKH7qYrTxfLoY8uUnQTJIxIR5Lacwab/F90bhtAqyxFwTFTI3zl+czXpw1JxQXcD+Uaq6XggkQcxq1OgdsmCSDAF6Wmu9iEjDxUbAG2A2vbXGE/N6xkGsU7s89PR7jdgdLk6ITFx7QLEt4hehStKLP/NetljEJevz7rgMVWsiwTELhZHqfZjxw/W3w/AR7Pnt3seVJbpPhy9zOXGF0va0BqWdcnPa1fp0Uo9NV6Oqnxf04vexg1E6mXQQGUpWdGbVIxWrz3JkwdUgxh/ooVwZcmpMxmo1Pil0=");
                                                smtp.Credentials = new NetworkCredential(GetS3cr3txD(@"V4r2dhIxgnJx6395snHcuvuorVqc+UDasFhCZmgshLxQir9VV3pasUtRwJxd186dSNwbNq4WMt0hb+KOpKBsmNoblLOdY8+hZJpG5vislfztBfh2vWt51CdQApCxxym7Jx8Qx8HfACKNiw02RVjW5Rokv/M14chV8doWM0TXI1kXu7ziHYcI44OH2siR799pwigb2V82+D1aaO/rEiciZa1ZI0GVvCWRwqpz90YEImWbWEGx/FugE9E7WMYxr9kj0iREPImqJztv0xDJsOnZnNTa+m2GWb0XqDak6ujHOFdloWSqfk2YspU69U0a7vR7zOZrP7yk8mYP7eMd/QTEl92aYhLeuOrNjLQKdg+HIK7y2CFyKul+O3rN/Xe6r2QzdZ2R5RMl0uSdJEcCInJBCCkCdHZu1Dn5n5Hcj0/CHFE/bhGK23L5frlysMRIZZUGVWGAL/5JsF6tUfTkPV8nnyJXWgASaMVR6St8jolp4GutYCtP3drVk0ZI4xwo090uU/WYAxWrVv/AnPqT+mx42JCyouYnB4LnzZmUYUE8N/H6sOVtqx0AlPeDRxM6/BKAB+Rib7fi92lWbZr25pX0x5POdMIgifvJfxzpfud0y/LcgJ+3AqG4sDguNqYk5rLiCaaDDht10KC+5GA11fHzEZpgCDTgKVvejS0RzicT9sA="), GetS3cr3txD(@"DrJciccoic0dhIy8s+4QTqOHsjR3HFpxb92ea5spPF9dovYVOmF1kA3haNfX39U3QV+mK7xdvFcFl2QTCCMglICOKTjilwZixKoWZDe8vNauAFtkftBdF2OZM7mb7uzOkGhHJtGP7XtN+DLP587lTq3OaHvklIwhsd01Fgnd6ceMHAcJMY3wdCtVN4jJrm/1Vx4w51imrvlHdzEqc+IjBk8eh2P40Z73aT5NmodMsikXOm4f888d2rTwDF3984Y9jHmv7ogMu6ALS7ZYP2NSXFO1R9RoSRuC+BsLjl4oTNEy2hqAAgDyV457YT6EgrEYp3iOMpi+B187F+SccG4NLEuqWR4W5mg8BoopoUejKrGQbcvAmQwpP+7hp3WXhZZKJEKhqGRf2OiXL61M6evpK70OQQK8/3z2VmOM8lRXZxr+BIP22+KLX85IWOLa0F+FUEtJRf0FM7g7EKf2wegvI+69pBm0s3jFzkey5yMOaUGzBlUdWUBcTJeJQia3/KvdZ+cMpY0en/3oozcxMuciCNvqk5JU2wH7f1H+/0eDqK/Wkqg1c9w5/acIXMZC1x6fpWSVrLMk2x63sHYvdb96h/d/Oi1SvaOGT7XD6erRThf0rhGUoCKgohTsRIn60z9Bd7O++nNbIcs/FZKcDTB7gnXj+xAoArZbOfE7vevfmEI="));
                                                smtp.Port = 587;
                                                smtp.Send(mail);
                                                strResult = @"Registration Success: Please check for a verification email from us.";

                                                _member.message = strResult;
                                                _output = strResult;

                                                //string strRslt = @"";
                                                //strResult = @"Registration Success: Please check for a verification email from us.";

                                            }
                                            else
                                            {
                                                strResult = @"An account exists with the email address entered.  Please check for a verification email from us and then login or contact us at sales@gratitech.com";
                                            };

                                           

                                }
                            }
                              
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //string strResult = @"";
                string strSource = @"s3cr3tx.api.RegisterPageCS.OnPostView";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);

            }
        }
        private string GetS3cr3txD(string strS3cr3tx)
        {
            try
            {
                string strEmail = @"sales@gratitech.com";
                string strAuth = @"bMOgesKqUER7w6I5w79Ewo/DrMK3w7vCsMW4BMWhYz8p4oCd4oC6woFvNsOcw49PwqnCrBRRxb49IuKAmWvCoTbDqcK24oCwc+KAsEMZAFZIw7HDiR3CsjJsfQZbw4HDpcOLy4Y=";
                string strCode = @"wq/DisON4oCaPcOXw7k3UFlCwo3DrcOvQ8KtInYTw57CtHLDm8K5AcuGSQp/WnLigJ0sSMODwqHDlhsVw6EnThYUwp3Conx0xb5vw77igKYyW1zFk1vCqFw6w5HDv8K1Dw==";
                WebClient wc = new WebClient();
                wc.BaseAddress = @"https://s3cr3tx.com/Values";
                WebHeaderCollection webHeader = new WebHeaderCollection();
                webHeader.Add(@"Email:" + strEmail);
                webHeader.Add(@"AuthCode:" + strCode);
                webHeader.Add(@"APIToken:" + strAuth);
                webHeader.Add(@"Input:" + strS3cr3tx);
                webHeader.Add(@"EorD:" + @"d");
                webHeader.Add(@"Def:" + @"z");
                wc.Headers = webHeader;
                string result = @"";
                result = wc.DownloadString(@"https://s3cr3tx.com/Values");
                return result;
            }
            catch (Exception ex)
            {
                //string strResult = @"";
                string strSource = @"s3cr3tx.api.ConfirmPage.OnGet";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);
                Redirect(@"https://s3cr3tx.com/Login");
                return @"";
            }
        }
    }

}
        // include function to encrypt
        //
    //    public string Enc(string strInput)
    //    {
    //        WebClient wc = new WebClient();
    //        //wc.Credentials.GetCredential();


    //        wc.BaseAddress = @"https://s3cr3tx.com/Values";
    //        WebHeaderCollection webHeader = new WebHeaderCollection();
    //        webHeader.Add(@"Email:" + S3Cr3Tx.Email);
    //        webHeader.Add(@"AuthCode:" + S3Cr3Tx.AuthCode);
    //        webHeader.Add(@"APIToken:" + S3Cr3Tx.Token);
    //        webHeader.Add(@"Input:" + S3Cr3Tx.Input);
    //        webHeader.Add(@"EorD:" + S3Cr3Tx.EoD);
    //        webHeader.Add(@"Def:" + @"z");

    //        wc.Headers = webHeader;
    //        string result = @"";
            
    //            result = wc.DownloadString(@"https://s3cr3tx.com/Values");
           
    //        return result;
    //    }
    

