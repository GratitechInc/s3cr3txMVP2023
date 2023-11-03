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
using s3cr3tx.Controllers;

namespace s3cr3tx.Pages
{
    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class FresetModel : PageModel
    {
        public string _output = @"";
        private FresetDbContext _fresetDb;
        private string strEmail = @"";
        public Freset fresetCurrent;
        private long lngID = 0;
        private DateTime dtExpires;


        public FresetModel(s3cr3tx.Models.FresetDbContext fresetDb)
        { 
                _fresetDb = fresetDb;
            fresetCurrent = new Freset();
        }
        public void OnGet([Bind("id", "session_start", "session_expires")] Freset freset)
        {
            try
            {

                fresetCurrent = freset;
                if (!QueryString.Empty.Equals(Request.QueryString))
                {
                    string strQueryString = Request.QueryString.ToString();
                    string[] strQryString = strQueryString.Replace(@"?=", @"").Split(@"_");
                    if (strQryString is not null && strQryString.Length.Equals(2))
                    {
                        strEmail = System.Web.HttpUtility.UrlDecode(strQryString[0]);
                        string strConnection = @"Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx";
                        string strCode = strQryString[1];
                        SqlConnection sql = new SqlConnection(strConnection);
                        SqlCommand command = new SqlCommand();
                        command.CommandText = @"dbo.usp_tbl_forgot_sel";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter p5 = new SqlParameter(@"member_email", strEmail);
                        SqlParameter p4 = new SqlParameter(@"Member_token", strCode);
                        command.Parameters.Add(p5);
                        command.Parameters.Add(p4);
                        DataSet ds = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter();
                        using (sql)
                        {
                            sql.Open();
                            command.Connection = sql;
                            da.SelectCommand = command;
                            da.Fill(ds);
                        }
                        fresetCurrent.id = (long)ds.Tables[0].Rows[0].ItemArray[0];
                        //lngID = fresetCurrent.id;
                        fresetCurrent.session_expires = (DateTime)ds.Tables[0].Rows[0].ItemArray[1];
                        fresetCurrent.email = strQryString[0];
                        if (DateTime.Now > fresetCurrent.session_expires)
                        {
                            _output = @"Time has expired.  Please request another password reset from the login page.";
                            Thread.Sleep(15000);
                           Response.Redirect(@"https://s3cr3tx.com/Login");
                        }
                    }
                    else
                    {
                        Response.Redirect(@"https://s3cr3tx.com/Login");
                    }
                }
                else { Response.Redirect(@"https://s3cr3tx.com/Login"); }

            }
            catch (Exception ex)
            {
                //string strResult = @"";
                string strSource = @"s3cr3tx.api.ConfirmPage.OnGet";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);
                Redirect(@"https://s3cr3tx.com/Login");
            }
        }

        public void OnPostView([Bind("id", "session_start", "session_expires")] Freset freset)
        {
            try
            {
                //fresetCurrent = freset;
                HttpRequest Request = HttpContext.Request;
                if (Request.Form.TryGetValue("fresetCurrent.id", out Microsoft.Extensions.Primitives.StringValues Id))
                {
                    if (Request.Form.TryGetValue("fresetCurrent.session_expires", out Microsoft.Extensions.Primitives.StringValues Expires))
                    {
                        if (Request.Form.TryGetValue("fresetCurrent.member_code", out Microsoft.Extensions.Primitives.StringValues Code))
                        {
                            if (Request.Form.TryGetValue("fresetCurrent.email", out Microsoft.Extensions.Primitives.StringValues Email))
                            {
                                fresetCurrent.session_expires = DateTime.Parse(Expires[0]);

                                if (fresetCurrent.session_expires > DateTime.Now)
                                {
                                    //set confirmed
                                    string strResult = @"";
                                    string strRslt = @"";
                                    strResult = uspEncDec.EncDec(@"sales@gratitech.com", Code[0], true, false, out strRslt);


                                    string strConnection = @"Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx";

                                    SqlConnection sql2 = new SqlConnection(strConnection);
                                    SqlCommand command2 = new SqlCommand();
                                    command2.CommandText = @"dbo.usp_tbl_member_set_newp";
                                    command2.CommandType = System.Data.CommandType.StoredProcedure;
                                    SqlParameter p6 = new SqlParameter(@"email", System.Web.HttpUtility.UrlDecode(Email[0]));
                                    SqlParameter p7 = new SqlParameter(@"member_code", strResult);
                                    SqlParameter p8 = new SqlParameter(@"member_id", Id[0]);
                                    command2.Parameters.Add(p6);
                                    command2.Parameters.Add(p7);
                                    command2.Parameters.Add(p8);
                                    int intResult = 0;
                                    using (sql2)
                                    {
                                        sql2.Open();
                                        command2.Connection = sql2;
                                        intResult = command2.ExecuteNonQuery();
                                    }
                                    if (intResult.Equals(1))
                                    {
                                        _output = @"Your password has been changed.";
                                        return;
                                    }
                                    else//else to indicate something went wrong output message to contact support
                                    {
                                        _output = @"Something went wrong";
                                        Response.Redirect(@"https://s3cr3tx.com/Login");
                                        return;
                                    }
                                }
                                else { Response.Redirect(@"https://s3cr3tx.com/Login"); }
                           }
                            else { Response.Redirect(@"https://s3cr3tx.com/Login"); }
                           }
                        else { Response.Redirect(@"https://s3cr3tx.com/Login"); }
                        }
                    else { Response.Redirect(@"https://s3cr3tx.com/Login"); }
                }
                else { Response.Redirect(@"https://s3cr3tx.com/Login"); }
              }

            catch (Exception ex)
            {
                //string strResult = @"";
                string strSource = @"s3cr3tx.api.ConfirmPage.OnGet";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);
                Redirect(@"https://s3cr3tx.com/Login");
            }
        }

        


    }
    }
