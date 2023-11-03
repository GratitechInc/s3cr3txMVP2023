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
using System.Data;
using System.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace s3cr3tx.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public s3cr3tx.Models.S3cr3tx S3Cr3Tx;
        public s3cr3tx.Models.S3cr3txDbContext _s3cr3tx;
        public s3cr3tx.Models.memberSessionDbContext _memberSessionDB;
        public s3cr3tx.Models.MemberDbContext _memberDB;
        private s3cr3tx.Models.Member _member;
        private s3cr3tx.Models.MemberSession _msession;
        public string _output;
        public IndexModel(s3cr3tx.Models.S3cr3txDbContext s3Cr3tx, s3cr3tx.Models.memberSessionDbContext memberSessionDb, s3cr3tx.Models.MemberDbContext memberDb)
        {
            _s3cr3tx = s3Cr3tx;
            S3Cr3Tx = new S3cr3tx();
            _output = S3Cr3Tx.Output;
            _memberSessionDB = memberSessionDb;
            _memberDB = memberDb;
        }
        //public s3cr3tx.S3cr3tx S3Cr3Tx;

        public void OnGet([Bind("Email", "AuthCode", "Token", "EoD", "Input", "Output")] s3cr3tx.Models.S3cr3tx _S3Cr3Tx, [Bind("id", "confirmed", "email", "enabled", "confirmed")] s3cr3tx.Models.Member member, [Bind("id", "IsActive", "SessionExpires")] s3cr3tx.Models.MemberSession session)
        {
            try
            {
                if (Request.Cookies.TryGetValue(@"s3cr3tx", out string? strValue))
                {
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

                    SqlConnection sql3 = new SqlConnection(strConnection);
                    SqlCommand command3 = new SqlCommand();
                    command3.CommandText = @"dbo.usp_tbl_member_sel";
                    command3.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter p6 = new SqlParameter(@"member_id", ms.member_id);
                    //SqlParameter p4 = new SqlParameter(@"member_code", strResult);
                    command3.Parameters.Add(p6);
                    //command2.Parameters.Add(p4);
                    DataSet dataSet2 = new DataSet();
                    SqlDataAdapter da2 = new SqlDataAdapter();
                    using (sql3)
                    {
                        sql3.Open();
                        command3.Connection = sql3;
                        da2.SelectCommand = command3;
                        da2.Fill(dataSet2);
                    }
                    //create member object
                    Member memberCurrent = new Member()
                    {
                        id = (long)dataSet2.Tables[0].Rows[0].ItemArray[0],
                        email = dataSet2.Tables[0].Rows[0].ItemArray[1].ToString(),
                        Code = dataSet2.Tables[0].Rows[0].ItemArray[2].ToString(),
                        ConfirmCode = @"",
                        FirstName = dataSet2.Tables[0].Rows[0].ItemArray[3].ToString(),
                        LastName = dataSet2.Tables[0].Rows[0].ItemArray[4].ToString(),
                        regcode = dataSet2.Tables[0].Rows[0].ItemArray[5].ToString(),
                        created = (DateTime)dataSet2.Tables[0].Rows[0].ItemArray[6],
                        updated = (DateTime)dataSet2.Tables[0].Rows[0].ItemArray[7],
                        enabled = (bool)dataSet2.Tables[0].Rows[0].ItemArray[8],
                        confirmed = (bool)dataSet2.Tables[0].Rows[0].ItemArray[9],
                        country = dataSet2.Tables[0].Rows[0].ItemArray[10].ToString(),
                        state = dataSet2.Tables[0].Rows[0].ItemArray[11].ToString(),
                        gender = dataSet2.Tables[0].Rows[0].ItemArray[12].ToString(),
                        mobile = dataSet2.Tables[0].Rows[0].ItemArray[13].ToString(),
                        MobileCarrier = dataSet2.Tables[0].Rows[0].ItemArray[14].ToString(),
                        city = dataSet2.Tables[0].Rows[0].ItemArray[15].ToString(),
                        zipcode = dataSet2.Tables[0].Rows[0].ItemArray[16].ToString(),
                        address = dataSet2.Tables[0].Rows[0].ItemArray[17].ToString(),
                        address2 = dataSet2.Tables[0].Rows[0].ItemArray[18].ToString()
                    };

                    if ((memberCurrent.enabled) && (memberCurrent.confirmed) && (ms.IsActive) && (DateTime.Now < ms.SessionExpires))
                    {
                        S3Cr3Tx = _S3Cr3Tx;
                        var memberBundle = Controllers.ebundle.GetEbundle(ms.member_email);
                        S3Cr3Tx.Email = ms.member_email;
                        S3Cr3Tx.Token = memberBundle.strapikey;
                        S3Cr3Tx.AuthCode = memberBundle.strauth;
                        WebClient wc = new WebClient();
                        //wc.Credentials.GetCredential();


                        wc.BaseAddress = @"https://s3cr3tx.com/Values";
                        WebHeaderCollection webHeader = new WebHeaderCollection();
                        webHeader.Add(@"Email:" + S3Cr3Tx.Email);
                        webHeader.Add(@"AuthCode:" + S3Cr3Tx.AuthCode);
                        webHeader.Add(@"APIToken:" + S3Cr3Tx.Token);
                        webHeader.Add(@"Input:" + S3Cr3Tx.Input);
                        webHeader.Add(@"EorD:" + S3Cr3Tx.EoD);
                        webHeader.Add(@"Def:" + @"z");

                        wc.Headers = webHeader;
                        string result = @"";
                        if ((!S3Cr3Tx.Input.IsNullOrEmpty())&&(S3Cr3Tx.AuthCode != S3Cr3Tx.Token) &&(S3Cr3Tx.AuthCode is not null)&&(!S3Cr3Tx.AuthCode.Equals(@"")))
                        {
                            result = wc.DownloadString(@"https://s3cr3tx.com/Values");
                            string strSource = @"s3cr3tx.api.IndexCS.OnPostView";
                            s3cr3tx.Controllers.ValuesController.LogIt(result, strSource);
                        }
                        S3Cr3Tx.Output = result;
                    }
                    else
                    {
                        _output = @"Please login again";
                        Response.Redirect(@"https://s3cr3tx.com/Login");
                    }
                }
                else
                {
                    _output = @"Please login again";
                    Response.Redirect(@"https://s3cr3tx.com/Login");
                }
            }
            catch (Exception ex)
            {
                //string strResult = @"";
                string strSource = @"s3cr3tx.api.IndexCS.OnPostView";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);
                Redirect(@"https://s3cr3tx.com/Login");
            }
        }
        public string Message { get; set; } = "";

        public class NewK
        {
            public string email { get; set; }
            public string pd { get; set; }
            public string pd2 { get; set; }
        }
        public void OnPostView([Bind("Email", "AuthCode", "Token", "EoD", "Input", "Output")] s3cr3tx.Models.S3cr3tx _S3Cr3Tx)
        {
            try
            {
                if (Request.Cookies.TryGetValue(@"s3cr3tx", out string? strValue))
                {
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

                    HttpRequest Request = HttpContext.Request;
                    if (Request.Form.TryGetValue("S3Cr3Tx.Email", out Microsoft.Extensions.Primitives.StringValues vEmail))
                    {
                        if (Request.Form.TryGetValue("S3Cr3Tx.AuthCode", out Microsoft.Extensions.Primitives.StringValues vCode))
                        {
                            if (Request.Form.TryGetValue("S3Cr3Tx.Token", out Microsoft.Extensions.Primitives.StringValues vToken))
                            {
                                if (Request.Form.TryGetValue("S3Cr3Tx.EoD", out Microsoft.Extensions.Primitives.StringValues vEoD))
                                {
                                    if (Request.Form.TryGetValue("S3Cr3Tx.Input", out Microsoft.Extensions.Primitives.StringValues vInput))
                                    {
                                        WebClient wc = new WebClient();
                                        //wc.Credentials.GetCredential();
                                        wc.BaseAddress = @"https://localhost:7192/Values";
                                        WebHeaderCollection webHeader = new WebHeaderCollection();
                                        //webHeader.Add(@"content-type:text/plain; charset=utf-8");
                                        webHeader.Add(@"accept:text/plain; charset=utf-8");
                                        webHeader.Add(@"Email:" + vEmail[0]);
                                        webHeader.Add(@"AuthCode:" + vCode[0]);
                                        webHeader.Add(@"APIToken:" + vToken[0]);
                                        webHeader.Add(@"Input:" + vInput[0]);
                                        webHeader.Add(@"EorD:" + vEoD[0]);
                                        webHeader.Add(@"Def:" + @"z");

                                        wc.Headers = webHeader;
                                        string result = @"";
                                        if (vCode[0] == vToken[0])
                                        {
                                            webHeader.Add(@"content-type:application/json");
                                            NewK nk = new NewK();
                                            nk.email = vEmail[0];
                                            nk.pd = vToken[0];
                                            nk.pd2 = vCode[0];
                                            string strNk = JsonSerializer.Serialize<NewK>(nk);
                                            //string strUTF8 = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding(0), System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding(0).GetBytes(strNk)));
                                            //wc.upload
                                            result = wc.UploadString(@"https://s3cr3tx.com/Values", strNk);
                                        }
                                        else
                                        {
                                            result = wc.DownloadString(@"https://s3cr3tx.com/Values");
                                        }
                                        Message = result;
                                        S3Cr3Tx.Output = result;
                                        var memberBundle = Controllers.ebundle.GetEbundle(ms.member_email);
                                        S3Cr3Tx.Email = ms.member_email;
                                        S3Cr3Tx.Token = memberBundle.strapikey;
                                        S3Cr3Tx.AuthCode = memberBundle.strauth;

                                    }
                                }
                            }

                        }
                    }



                }
                else
                {
                    _output = @"Please login again";
                    Response.Redirect(@"https://s3cr3tx.com/Login");
                }
            }
            catch (Exception ex)
            {
                string strSource = @"s3cr3tx.api.RegisterPageCS.OnPostView";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);
                Redirect(@"https://s3cr3tx.com/Login");

            }
        }
    }
}
