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
using System.Data.SqlClient;
using System.Data;
using Microsoft.Net.Http.Headers;

namespace s3cr3tx.Pages
{
    public class LoginModel : PageModel
    {
        //private readonly ILogger<IndexModel> _logger;
        
        public s3cr3tx.Models.Login loginCurrent;
        public s3cr3tx.Models.LoginDbContext LoginContext;
        public string _output;
        public LoginModel(s3cr3tx.Models.LoginDbContext loginDbContext)
        {
            LoginContext = loginDbContext;
            loginCurrent = new Login();
            _output = loginCurrent.Output;
        }
        //public s3cr3tx.S3cr3tx S3Cr3Tx;

        public void OnGet([Bind("member_email", "member_code", "Output")] s3cr3tx.Models.Login login)
        {
            try
            {

                loginCurrent = login;
                loginCurrent.Output = _output;
            }
            catch (Exception ex)
            {
                string strSource = @"s3cr3tx.api.RegisterPageCS.OnGet";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);

            }
        }
        public string Message { get; set; } = "";

        public void OnPostView([Bind("member_email", "member_code","Output")] s3cr3tx.Models.Login loginC)
        {
            try
            {
                string email = loginC.member_email; //= member;

                HttpRequest Request = HttpContext.Request;
                if (Request.Form.TryGetValue("loginCurrent.member_email", out Microsoft.Extensions.Primitives.StringValues Email))
                {
                    if (Request.Form.TryGetValue("loginCurrent.member_code", out Microsoft.Extensions.Primitives.StringValues Code))
                    {
                        loginCurrent.member_email = Email[0].ToLower();
                        loginCurrent.member_code = Code[0];

                        //now insert new member into the database
                        DateTime dtTimeStamp = DateTime.Now;
                        string strResult = @"";
                        string strConnection = @"Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx";
                        //SqlConnection sql = new SqlConnection(strConnection);
                        //SqlCommand command = new SqlCommand();
                        //command.CommandText = @"dbo.usp_tbl_member_sel_reg";
                        //command.CommandType = System.Data.CommandType.StoredProcedure;
                        //SqlParameter p1 = new SqlParameter(@"email", loginCurrent.member_email.Trim());
                        //command.Parameters.Add(p1);
                        //strResult = @"";
                        //using (sql)
                        //{
                        //    sql.Open();
                        //    command.Connection = sql;
                        //    strResult = (string)command.ExecuteScalar();
                        //}
                        //if (!strResult.Equals(@""))
                        //{
                        //    //valid user
                        //    strResult = System.Convert.ToBase64String(System.Security.Cryptography.SHA512.HashData(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding(0), System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding(0).GetBytes(loginCurrent.member_code + strResult))));
                        //}
                        SqlConnection sql2 = new SqlConnection(strConnection);
                        SqlCommand command2 = new SqlCommand();
                        command2.CommandText = @"dbo.GetCode";
                        command2.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter p3 = new SqlParameter(@"email", loginCurrent.member_email.Trim());
                        //SqlParameter p4 = new SqlParameter(@"pword", strResult);
                        command2.Parameters.Add(p3);
                        //command2.Parameters.Add(p4);
                        string strResult1 = @"";
                        using (sql2)
                        {
                            sql2.Open();
                            command2.Connection = sql2;
                            strResult1 = (string)command2.ExecuteScalar();
                        }
                        string strCompare = Controllers.uspEncDec.Dec(@"sales@gratitech.com",strResult1);
                        //strCompare = strCompare.Replace(@"""",@"");
                        if (strResult1.Equals(@"") || !strCompare.Equals(loginCurrent.member_code))
                        {
                            loginCurrent.Output = @"The email or password you entered did not match our records";
                            return;
                        }
                        else
                        {
                            SqlConnection sql3 = new SqlConnection(strConnection);
                            SqlCommand command3 = new SqlCommand();
                            command3.CommandText = @"dbo.usp_tbl_member_sel_email";
                            command3.CommandType = System.Data.CommandType.StoredProcedure;
                            SqlParameter p5 = new SqlParameter(@"email", loginCurrent.member_email);
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
                            //create member session and member objects
                            MemberSession ms = new MemberSession()
                            {
                                member_id = member.id,
                                member_email = member.email,
                                FirstName = member.FirstName,
                                LastName = member.LastName,
                                IsActive = true,
                                member_ip = strIP
                            };
                            //Store Session Info:
                            SqlConnection sql4 = new SqlConnection(strConnection);
                            SqlCommand command4 = new SqlCommand();
                            command4.CommandText = @"dbo.usp_tbl_member_sessions_ups";
                            command4.CommandType = System.Data.CommandType.StoredProcedure;
                            SqlParameter p70 = new SqlParameter(@"member_id", ms.member_id);
                            SqlParameter p71 = new SqlParameter(@"member_email", ms.member_email);
                            SqlParameter p72 = new SqlParameter(@"member_token", ms.member_token);
                            SqlParameter p73 = new SqlParameter(@"member_name", ms.FirstName + @" " + ms.LastName);
                            SqlParameter p74 = new SqlParameter(@"session_start", ms.session_start);
                            SqlParameter p75 = new SqlParameter(@"session_active", true);
                            SqlParameter p76 = new SqlParameter(@"session_last_active", ms.LastActive);
                            SqlParameter p77 = new SqlParameter(@"session_end", ms.SessionExpires);
                            SqlParameter p78 = new SqlParameter(@"member_ip", ms.member_ip);
                            command4.Parameters.Add(p70);
                            command4.Parameters.Add(p71);
                            command4.Parameters.Add(p72);
                            command4.Parameters.Add(p73);
                            command4.Parameters.Add(p74);
                            command4.Parameters.Add(p75);
                            command4.Parameters.Add(p76);
                            command4.Parameters.Add(p77);
                            command4.Parameters.Add(p78);
                            long lngResult4 = 0;
                            using (sql4)
                            {
                                sql4.Open();
                                command4.Connection = sql4;
                                lngResult4 = (long)command4.ExecuteScalar();
                            }
                            if (lngResult4.Equals(0))
                            {
                                loginCurrent.Output = @"Something went wrong.";
                                return;
                            }
                            else
                            {
                                ms.id = lngResult4;
                                //redirect to index
                                //put a cookie in the cookie jar
                                string strCookieContent = Controllers.uspEncDec.Enc(@"sales@gratitech.com", ms.member_token);

                                //var resp = new HttpResponseMessage();

                                //var cookie = new CookieHeaderValue("session-id", strCookieContent);
                                //cookie.Expires = DateTimeOffset.Now.AddDays(1);
                                //cookie.Domain = Request.RequestUri.Host;
                                //cookie.Path = "/";

                                //resp.Headers.AddCookies(new CookieHeaderValue[] { cookie });
                                // Response.m;
                                Response.Cookies.Delete(@"s3cr3tx");
                                Response.Cookies.Append(@"s3cr3tx", strCookieContent, new CookieOptions()
                                {
                                    Secure = true,
                                    HttpOnly = true,
                                    //Domain = @"s3cr3tx.com",
                                    Expires = DateTime.Now.AddYears(1),
                                    //SameSite = SameSiteMode.Strict,
                                    IsEssential = true
                                });
                               // Response.WriteAsync(@"Login Success");
                              // Response.Cookies.
                                Response.Redirect(@"https://s3cr3tx.com/");
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                string strSource = @"s3cr3tx.api.RegisterPageCS.OnGet";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);

            }
        }
    }
}
