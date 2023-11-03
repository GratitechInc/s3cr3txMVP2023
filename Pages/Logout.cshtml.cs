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
using System.Data;
using System.Security.Cryptography;


namespace s3cr3tx.Pages
{
    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class LogoutModel : PageModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string _message = @"";
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(ILogger<LogoutModel> logger)
        { 
                _logger = logger;
        }

        public void OnGet()
        {
            try
            {
                if (Request.Cookies.TryGetValue(@"s3cr3tx", out string? strValue))
                {
                    Response.Cookies.Delete(@"s3cr3tx");
                    //decrypt the cookie value
                    string strSessionID = Controllers.uspEncDec.Dec(@"sales@gratitech.com", strValue);
                    //get the session object
                    string strConnection = @"Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx";

                    SqlConnection sql = new SqlConnection(strConnection);
                    SqlCommand command = new SqlCommand();
                    command.CommandText = @"dbo.usp_tbl_member_sessions_selCode";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter p5 = new SqlParameter(@"ID", strSessionID);
                    //SqlParameter p4 = new SqlParameter(@"member_code", strResult);
                    command.Parameters.Add(p5);
                    //command2.Parameters.Add(p4);
                    DataSet dataSet = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter();
                    using (sql)
                    {
                        sql.Open();
                        command.Connection = sql;
                        da.SelectCommand = command;
                        da.Fill(dataSet);
                    }

                    MemberSession ms = new MemberSession()
                    {
                        id = (long)dataSet.Tables[0].Rows[0].ItemArray[0],
                        member_id = (long)dataSet.Tables[0].Rows[0].ItemArray[1],
                        member_email = dataSet.Tables[0].Rows[0].ItemArray[2].ToString(),
                        member_token = dataSet.Tables[0].Rows[0].ItemArray[3].ToString(),
                        FirstName = dataSet.Tables[0].Rows[0].ItemArray[4].ToString(),
                        session_start = (DateTime)dataSet.Tables[0].Rows[0].ItemArray[5],
                        IsActive = (bool)dataSet.Tables[0].Rows[0].ItemArray[6],
                        LastActive = (DateTime)dataSet.Tables[0].Rows[0].ItemArray[7],
                        SessionExpires = (DateTime)dataSet.Tables[0].Rows[0].ItemArray[8],
                        member_ip = dataSet.Tables[0].Rows[0].ItemArray[9].ToString(),
                    };
                    //get the member object
                    ms.IsActive = false;
                    SqlConnection sql4 = new SqlConnection(strConnection);
                    SqlCommand command4 = new SqlCommand();
                    command4.CommandText = @"dbo.usp_tbl_member_sessions_ups";
                    command4.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter p70 = new SqlParameter(@"member_id", ms.member_id);
                    SqlParameter p71 = new SqlParameter(@"member_email", ms.member_email);
                    SqlParameter p72 = new SqlParameter(@"member_token", ms.member_token);
                    SqlParameter p73 = new SqlParameter(@"member_name", ms.FirstName + @" " + ms.LastName);
                    SqlParameter p74 = new SqlParameter(@"session_start", ms.session_start);
                    SqlParameter p75 = new SqlParameter(@"session_active", false);
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
                        Controllers.ValuesController.LogIt(@"Error updating session on logout", @"Logout");
                    }

                    Response.Cookies.Delete(@"s3cr3tx");
                    
                }
                else
                {
                    _message = @"Please login again";
                    Response.Redirect(@"https://s3cr3tx.com/Login");
                }
            }
            catch (Exception ex)
            {
                //string strResult = @"";
                string strSource = @"s3cr3tx.api.RegisterPageCS.OnGet";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);
                Redirect(@"https://s3cr3tx.com/Login");
            }
        }
    }
    }
