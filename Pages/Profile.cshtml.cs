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
using s3cr3tx.Controllers;

namespace s3cr3tx.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly ILogger<ProfileModel> _logger;
        
        public s3cr3tx.Models.S3cr3tx S3Cr3Tx;
        public s3cr3tx.Models.S3cr3txDbContext _s3cr3tx;
        public s3cr3tx.Models.memberSessionDbContext _memberSessionDB;
        public s3cr3tx.Models.MemberDbContext _memberDB;
        public s3cr3tx.Models.Member _member;
        private s3cr3tx.Models.MemberSession _msession;
        public string _output;
        public ProfileModel(s3cr3tx.Models.S3cr3txDbContext s3Cr3tx, s3cr3tx.Models.memberSessionDbContext memberSessionDb, s3cr3tx.Models.MemberDbContext memberDb)
        {
            _s3cr3tx = s3Cr3tx;
            S3Cr3Tx = new S3cr3tx();
            _member = new Member();
            _output = S3Cr3Tx.Output;
            _memberSessionDB = memberSessionDb;
            _memberDB = memberDb;
        }
        //public s3cr3tx.S3cr3tx S3Cr3Tx;

        public void OnGet([Bind("Email", "AuthCode", "Token", "EoD", "Input", "Output")] s3cr3tx.Models.S3cr3tx _S3Cr3Tx, [Bind("id","confirmed","email","enabled","confirmed")]s3cr3tx.Models.Member member, [Bind("id","IsActive","SessionExpires")] s3cr3tx.Models.MemberSession session)
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
                        address2 = dataSet2.Tables[0].Rows[0].ItemArray[18].ToString(),
                        orderNumber = dataSet2.Tables[0].Rows[0].ItemArray[19].ToString()
                    };

                    if ((memberCurrent.enabled) && (memberCurrent.confirmed) && (ms.IsActive) && (DateTime.Now < ms.SessionExpires))
                    {
                    S3Cr3Tx = _S3Cr3Tx;
                        _member = memberCurrent;

                        //S3Cr3Tx.Output = result;
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
            catch(Exception ex)
            {
                //string strResult = @"";
                string strSource = @"s3cr3tx.api.RegisterPageCS.OnGet";
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
                            SqlParameter p2 = new SqlParameter(@"ID", strSessionID);
                            //SqlParameter p4 = new SqlParameter(@"member_code", strResult);
                            command.Parameters.Add(p2);
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
                    long lngID = ms.member_id;
                            string email = ms.member_email; //= member;
                    SqlConnection sql3 = new SqlConnection(strConnection);
                    SqlCommand command3 = new SqlCommand();
                    command3.CommandText = @"dbo.usp_tbl_member_sel";
                    command3.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter p67 = new SqlParameter(@"member_id", ms.member_id);
                    //SqlParameter p4 = new SqlParameter(@"member_code", strResult);
                    command3.Parameters.Add(p67);
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
                        address2 = dataSet2.Tables[0].Rows[0].ItemArray[18].ToString(),
                        orderNumber = dataSet2.Tables[0].Rows[0].ItemArray[19].ToString()

                    };

                    if ((memberCurrent.enabled) && (memberCurrent.confirmed) && (ms.IsActive) && (DateTime.Now < ms.SessionExpires))
                    {



                        HttpRequest Request = HttpContext.Request;
                            if (Request.Form.TryGetValue("_member.email", out Microsoft.Extensions.Primitives.StringValues Email))
                            {
                                if (Request.Form.TryGetValue("_member.FirstName", out Microsoft.Extensions.Primitives.StringValues FirstName))
                                {
                                if (Request.Form.TryGetValue("_member.LastName", out Microsoft.Extensions.Primitives.StringValues LastName))
                                {
                                    if (Request.Form.TryGetValue("_member.mobile", out Microsoft.Extensions.Primitives.StringValues Mobile))
                                    {
                                        if (Request.Form.TryGetValue("_member.MobileCarrier", out Microsoft.Extensions.Primitives.StringValues MobileCarrier))
                                        {
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
                                            if (Request.Form.TryGetValue("_member.orderNumber", out Microsoft.Extensions.Primitives.StringValues OrderNumber))
                                            {
                                                _member.gender = Gender[0];
                                            }
                                            _member.email = Email[0];
                                            _member.FirstName = FirstName[0];
                                            _member.LastName = LastName[0];
                                            string strResult = @"";
                                            //string strRslt = @"";
                                            //strResult = uspEncDec.EncDec(@"support@gratitech.com", Code[0], true, false, out strRslt);
                                            _member.orderNumber = OrderNumber[0];
                                            // _member.Code = Controllers.uspEncDec.Enc(@"support@gratitech.com", Code[0])//_member.Code = System.Convert.ToBase64String(System.Security.Cryptography.SHA512.HashData(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding(0), System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding(0).GetBytes(Code[0] + _member.regcode))));
                                            //strRslt = Controllers.uspEncDec.Enc(@"support@gratitech.com", Code[0]);
                                            //_member.Code = strResult;
                                            _member.mobile = Mobile[0];
                                            _member.MobileCarrier = MobileCarrier[0];
                                            //now insert new member into the database
                                            DateTime dtTimeStamp = DateTime.Now;
                                            //string strResult = @"";
                                            //string strConnection1 = @"Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx";
                                            SqlConnection sql1 = new SqlConnection(strConnection);
                                            SqlCommand command1 = new SqlCommand();
                                            command1.CommandText = @"dbo.usp_tbl_member_update";
                                            command1.CommandType = System.Data.CommandType.StoredProcedure;
                                            SqlParameter p1 = new SqlParameter(@"member_email", _member.email.Trim());
                                            SqlParameter p22 = new SqlParameter(@"member_id", memberCurrent.id);
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
                                            SqlParameter p17 = new SqlParameter(@"member_order", _member.orderNumber);

                                            command1.Parameters.Add(p1);
                                            command1.Parameters.Add(p22);
                                            command1.Parameters.Add(p3);
                                            command1.Parameters.Add(p4);
                                            command1.Parameters.Add(p5);
                                            command1.Parameters.Add(p6);
                                            command1.Parameters.Add(p7);
                                            command1.Parameters.Add(p8);
                                            command1.Parameters.Add(p9);
                                            command1.Parameters.Add(p10);
                                            command1.Parameters.Add(p11);
                                            command1.Parameters.Add(p12);
                                            command1.Parameters.Add(p13);
                                            command1.Parameters.Add(p14);
                                            command1.Parameters.Add(p15);
                                            command1.Parameters.Add(p16);
                                            command1.Parameters.Add(p17);
                                            dataSet2 = new DataSet();
                                            da2 = new SqlDataAdapter();
                                            using (sql1)
                                            {
                                                sql1.Open();
                                                command1.Connection = sql1;
                                                da2.SelectCommand = command1;
                                                da2.Fill(dataSet2);
                                            }
                                            //create member object
                                            Member memberCurrent2 = new Member()
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
                                                address2 = dataSet2.Tables[0].Rows[0].ItemArray[18].ToString(),
                                            orderNumber = dataSet2.Tables[0].Rows[0].ItemArray[19].ToString()
                                            };

                                            if ((memberCurrent2.enabled) && (memberCurrent2.confirmed) && (ms.IsActive) && (DateTime.Now < ms.SessionExpires))
                                            {
                                                //string strRslt = @"";
                                                strResult = @"Profile update success: Your s3cr3tx Profile has been updated";

                                            }
                                            else
                                            {
                                                strResult = @"Your session expired.  Please login again.";
                                                Redirect(@"https://s3cr3tx.com/Login");
                                            };

                                            _member.message = strResult;
                                            _output = strResult;



                                        }
                                    }

                                } 
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string strSource = @"s3cr3tx.api.ProfilePageCS.OnPost";
                        s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);

                    }
        }
    }
}
