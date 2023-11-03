using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using s3cr3tx.Models;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Web;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Net.Mail;
//using Microsoft.Data.SqlClient;


namespace s3cr3tx.Pages
{
    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ForgotModel : PageModel
    {
        
        public string _message = @"";
        //private readonly ILogger<ForgotModel> _logger;
        public s3cr3tx.Models.Forgot forgotCurrent;
        public s3cr3tx.Models.ForgotDbContext ForgotContext;
        public string _output;

        public ForgotModel(s3cr3tx.Models.ForgotDbContext forgotDbContext)
        {
            ForgotContext = forgotDbContext;
            forgotCurrent = new Forgot();
        }

        public void OnPostView()
        {
            try
            {
                HttpRequest Request = HttpContext.Request;
                if (Request.Form.TryGetValue("forgotCurrent.member_email", out Microsoft.Extensions.Primitives.StringValues Email))
                {

                    forgotCurrent.member_email = Email[0].ToLower();
                    string strConnection = @"Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx";

                    SqlConnection sql3 = new SqlConnection(strConnection);
                    SqlCommand command3 = new SqlCommand();
                    command3.CommandText = @"dbo.usp_tbl_member_sel_email";
                    command3.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter p5 = new SqlParameter(@"email", forgotCurrent.member_email);
                    //SqlParameter p4 = new SqlParameter(@"member_code", strResult);
                    command3.Parameters.Add(p5);
                    //command2.Parameters.Add(p4);
                    DataSet dataSet = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter();
                    using (sql3)
                    {
                        sql3.Open();
                        command3.Connection = sql3;
                        da.SelectCommand = command3;
                        da.Fill(dataSet);
                    }
                    if (dataSet.Tables.Count >0)
                    { 
                    //create member object
                    Member member = new Member()
                    {
                        id = (long)dataSet.Tables[0].Rows[0].ItemArray[0],
                        email = dataSet.Tables[0].Rows[0].ItemArray[1].ToString(),
                        Code = dataSet.Tables[0].Rows[0].ItemArray[2].ToString(),
                        ConfirmCode = @"",
                        FirstName = dataSet.Tables[0].Rows[0].ItemArray[3].ToString(),
                        LastName = dataSet.Tables[0].Rows[0].ItemArray[4].ToString(),
                        regcode = dataSet.Tables[0].Rows[0].ItemArray[5].ToString(),
                        created = (DateTime)dataSet.Tables[0].Rows[0].ItemArray[6],
                        updated = (DateTime)dataSet.Tables[0].Rows[0].ItemArray[7],
                        enabled = (bool)dataSet.Tables[0].Rows[0].ItemArray[8],
                        confirmed = (bool)dataSet.Tables[0].Rows[0].ItemArray[9],
                        country = dataSet.Tables[0].Rows[0].ItemArray[10].ToString(),
                        state = dataSet.Tables[0].Rows[0].ItemArray[11].ToString(),
                        gender = dataSet.Tables[0].Rows[0].ItemArray[12].ToString(),
                        mobile = dataSet.Tables[0].Rows[0].ItemArray[13].ToString(),
                        MobileCarrier = dataSet.Tables[0].Rows[0].ItemArray[14].ToString(),
                        city = dataSet.Tables[0].Rows[0].ItemArray[15].ToString(),
                        zipcode = dataSet.Tables[0].Rows[0].ItemArray[16].ToString(),
                        address = dataSet.Tables[0].Rows[0].ItemArray[17].ToString(),
                        address2 = dataSet.Tables[0].Rows[0].ItemArray[18].ToString()
                    };
                    string strIP = @"";
                    if (HttpContext.Connection.RemoteIpAddress is not null)
                    {
                        IPAddress iP = HttpContext.Connection.RemoteIpAddress;
                        strIP = iP.ToString();
                    }
                    if (member.confirmed.Equals(true) && (member.enabled.Equals(true)))
                    {
                        //store the forgot data
                        SqlConnection sql4 = new SqlConnection(strConnection);
                        SqlCommand command4 = new SqlCommand();
                        command4.CommandText = @"dbo.usp_tbl_forgot_ins";
                        command4.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter p73 = new SqlParameter(@"member_id", member.id);
                        SqlParameter p71 = new SqlParameter(@"member_email", member.email);
                        SqlParameter p72 = new SqlParameter(@"member_token", forgotCurrent.member_code);
                        SqlParameter p74 = new SqlParameter(@"session_start", forgotCurrent.session_start);
                        SqlParameter p77 = new SqlParameter(@"session_end", forgotCurrent.code_expires);
                        SqlParameter p78 = new SqlParameter(@"member_ip", strIP);
                        command4.Parameters.Add(p71);
                        command4.Parameters.Add(p73);
                        command4.Parameters.Add(p72);
                        command4.Parameters.Add(p74);
                        command4.Parameters.Add(p77);
                        command4.Parameters.Add(p78);
                        int lngResult4 ;
                        using (sql4)
                        {
                            sql4.Open();
                            command4.Connection = sql4;
                            lngResult4 = (int)command4.ExecuteNonQuery();
                        }
                        if (lngResult4.Equals(0))
                        {
                            forgotCurrent.Output = @"Something went wrong.";
                            return;
                        }
                        else
                        {


                            Member _member = new Member();
                            MailMessage mail = new MailMessage();
                            mail.From = new MailAddress(@"support@s3cr3tx.com");
                            mail.Sender = new MailAddress(@"support@s3cr3tx.com");
                            mail.Subject = @"s3cr3tx account verification";
                            mail.To.Add(new MailAddress(forgotCurrent.member_email));
                            mail.IsBodyHtml = true;
                            mail.Body = @"Dear " + member.FirstName + @",<br/><br/>Please click the following link to reset your password: <a href='https://s3cr3tx.com/Freset?=" + System.Web.HttpUtility.UrlEncode(forgotCurrent.member_email) + @"_" + forgotCurrent.member_code + "'>https://s3cr3tx.com/Freset?=" + System.Web.HttpUtility.UrlEncode(forgotCurrent.member_email) + @"_" + forgotCurrent.member_code + @"</a><br/><br/>For your account protection, the link will only be active for 20 minutes. <br/><br/>Thank you!<br/><br/>s3cr3tx sales@gratitech.com";
                            SmtpClient smtp = new SmtpClient();
                            smtp.EnableSsl = true;
                            smtp.Host = GetS3cr3txD(@"SD8d/1ZiIFiNWRiGUYbXbt6fdtOppU3KOipsr5zGlY+Wk/VyHYiOMynMhwvZgv94Xkkewc0ZW7CW/zJ14JaTfLdNDGzeoHTV+Ae/SpTIsmZkLvMX9nkA/HY7F6PzlP96+5gfQkaWufkLEZ19KwdkhOpwJDTUgEOBOZ4LxCH5tQ0ZwrXhH8pHII2mlNkV+zQQAfPjJvBUiiSyk3UoMTw8NZBfYhi9xmFbgkNbFjKO4XUXTxhcFXcikepf80hWwEdZx7ZUKs+mT8us/boGgXvzolyjQZEqBI4+4XeMZ5N5DbBqMWNCy0Fu9kz11ncz+wJUJmOxorYnecPhMuu22AYR0M3tiW65Pb2i9cyzp52pqhCrSpfs7IOlLEmlfZWFINWYXoTvk3MfzgQKH7qYrTxfLoY8uUnQTJIxIR5Lacwab/F90bhtAqyxFwTFTI3zl+czXpw1JxQXcD+Uaq6XggkQcxq1OgdsmCSDAF6Wmu9iEjDxUbAG2A2vbXGE/N6xkGsU7s89PR7jdgdLk6ITFx7QLEt4hehStKLP/NetljEJevz7rgMVWsiwTELhZHqfZjxw/W3w/AR7Pnt3seVJbpPhy9zOXGF0va0BqWdcnPa1fp0Uo9NV6Oqnxf04vexg1E6mXQQGUpWdGbVIxWrz3JkwdUgxh/ooVwZcmpMxmo1Pil0=");
                            smtp.Credentials = new NetworkCredential(GetS3cr3txD(@"V4r2dhIxgnJx6395snHcuvuorVqc+UDasFhCZmgshLxQir9VV3pasUtRwJxd186dSNwbNq4WMt0hb+KOpKBsmNoblLOdY8+hZJpG5vislfztBfh2vWt51CdQApCxxym7Jx8Qx8HfACKNiw02RVjW5Rokv/M14chV8doWM0TXI1kXu7ziHYcI44OH2siR799pwigb2V82+D1aaO/rEiciZa1ZI0GVvCWRwqpz90YEImWbWEGx/FugE9E7WMYxr9kj0iREPImqJztv0xDJsOnZnNTa+m2GWb0XqDak6ujHOFdloWSqfk2YspU69U0a7vR7zOZrP7yk8mYP7eMd/QTEl92aYhLeuOrNjLQKdg+HIK7y2CFyKul+O3rN/Xe6r2QzdZ2R5RMl0uSdJEcCInJBCCkCdHZu1Dn5n5Hcj0/CHFE/bhGK23L5frlysMRIZZUGVWGAL/5JsF6tUfTkPV8nnyJXWgASaMVR6St8jolp4GutYCtP3drVk0ZI4xwo090uU/WYAxWrVv/AnPqT+mx42JCyouYnB4LnzZmUYUE8N/H6sOVtqx0AlPeDRxM6/BKAB+Rib7fi92lWbZr25pX0x5POdMIgifvJfxzpfud0y/LcgJ+3AqG4sDguNqYk5rLiCaaDDht10KC+5GA11fHzEZpgCDTgKVvejS0RzicT9sA="), GetS3cr3txD(@"DrJciccoic0dhIy8s+4QTqOHsjR3HFpxb92ea5spPF9dovYVOmF1kA3haNfX39U3QV+mK7xdvFcFl2QTCCMglICOKTjilwZixKoWZDe8vNauAFtkftBdF2OZM7mb7uzOkGhHJtGP7XtN+DLP587lTq3OaHvklIwhsd01Fgnd6ceMHAcJMY3wdCtVN4jJrm/1Vx4w51imrvlHdzEqc+IjBk8eh2P40Z73aT5NmodMsikXOm4f888d2rTwDF3984Y9jHmv7ogMu6ALS7ZYP2NSXFO1R9RoSRuC+BsLjl4oTNEy2hqAAgDyV457YT6EgrEYp3iOMpi+B187F+SccG4NLEuqWR4W5mg8BoopoUejKrGQbcvAmQwpP+7hp3WXhZZKJEKhqGRf2OiXL61M6evpK70OQQK8/3z2VmOM8lRXZxr+BIP22+KLX85IWOLa0F+FUEtJRf0FM7g7EKf2wegvI+69pBm0s3jFzkey5yMOaUGzBlUdWUBcTJeJQia3/KvdZ+cMpY0en/3oozcxMuciCNvqk5JU2wH7f1H+/0eDqK/Wkqg1c9w5/acIXMZC1x6fpWSVrLMk2x63sHYvdb96h/d/Oi1SvaOGT7XD6erRThf0rhGUoCKgohTsRIn60z9Bd7O++nNbIcs/FZKcDTB7gnXj+xAoArZbOfE7vevfmEI="));
                            smtp.Port = 587;
                            smtp.Send(mail);
                            //_member.message = strResult;
                            _output = @"Please check your email for password reset instructions.";
                            //Response.Redirect(@"https://s3cr3tx.com/Login");
                        }
                    }
                    else
                    {
                        _output = @"Please check for an email from us to confirm your account or contact us at sales@gratitech.com";
                    }
                }
                else
                {
                    _output = @"Something went wrong please contact us at sales@gratitech.com";
                }
            }
                else
            {
                _output = @"Something went wrong please contact us at sales@gratitech.com";
            }
        }
            catch (Exception ex)
            {
                //string strResult = @"";
                string strSource = @"s3cr3tx.api.ConfirmPage.OnGet";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);
                Redirect(@"https://s3cr3tx.com/Login");
            }
        }

        private string GetS3cr3txD(string strS3cr3tx)
        {
            try { 
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
        public class NewK
        {
            [Required]
            public string email { get; set; }
            [Required]
            public string pd { get; set; }
            [Required]
            public string pd2 { get; set; }
        }
        private string CreateBundle(string strEmail)
        {
            try { 
            NewK newK = new NewK();
            newK.pd = @"1";
            newK.pd2 = @"1";
            newK.email = strEmail;
            WebClient wc = new WebClient();
            //wc.Credentials.GetCredential();
            wc.BaseAddress = @"https://s3cr3tx.com/Values";
            WebHeaderCollection webHeader = new WebHeaderCollection();
            wc.Headers = webHeader;
            string result = @"";
            webHeader.Add(@"content-type:application/json");
            NewK nk = new NewK();
            nk.email = strEmail;
            nk.pd = @"1";
            nk.pd2 = @"1";
            string strNk = JsonSerializer.Serialize<NewK>(nk);
            result = wc.UploadString(@"https://s3cr3tx.com/Values", strNk);
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
