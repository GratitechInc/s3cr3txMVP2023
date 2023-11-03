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
using System.Web;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace s3cr3tx.Pages
{
    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ValidateModel : PageModel
    {
        public string _message = @"";
        private readonly ILogger<ValidateModel> _logger;

        public ValidateModel(ILogger<ValidateModel> logger)
        { 
                _logger = logger;
        }

        public void OnGet()
        {
            try
            {
                string strQueryString = Request.QueryString.ToString();
                string[] strQryString = strQueryString.Replace(@"?=",@"").Split(@"_");
                string strEmail = System.Web.HttpUtility.UrlDecode(strQryString[0]);
                string strConnection = @"Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx";
                string strCode = strQryString[1];
                SqlConnection sql = new SqlConnection(strConnection);
                SqlCommand command = new SqlCommand();
                command.CommandText = @"dbo.usp_tbl_member_sel_reg_confirm";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter p5 = new SqlParameter(@"email", strEmail);
                SqlParameter p4 = new SqlParameter(@"reg_number", strCode);
                command.Parameters.Add(p5);
                command.Parameters.Add(p4);
                long lngResult = 0;
                using (sql)
                {
                    sql.Open();
                    command.Connection = sql;
                    lngResult = (long)command.ExecuteScalar();
                }
                if (!lngResult.Equals(0))
                    {
                    //set confirmed
                    int intResult = 0;
                    SqlConnection sql2 = new SqlConnection(strConnection);
                    SqlCommand command2 = new SqlCommand();
                    command2.CommandText = @"dbo.usp_tbl_member_set_confirmed";
                    command2.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter p6 = new SqlParameter(@"email", strEmail);
                    SqlParameter p7 = new SqlParameter(@"reg_number", strCode);
                    command2.Parameters.Add(p6);
                    command2.Parameters.Add(p7);
                    using (sql2)
                    {
                        sql2.Open();
                        command2.Connection = sql2;
                        intResult = (int)command2.ExecuteNonQuery();
                    }
                    if (intResult.Equals(1))
                    {
                        string strBundleCreate = CreateBundle(strEmail);
                    }//else to indicate something went wrong output message to contact support
                    else
                    {
                        string strMessage = @"Something went wrong please email sales@gratitech.com";
                        return;
                    }
                }

                //Response.Redirect(@"https://s3cr3tx.com/Login");
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
            webHeader.Add(@"AuthCode:" + strAuth);
            webHeader.Add(@"APIToken:" + strCode);
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
