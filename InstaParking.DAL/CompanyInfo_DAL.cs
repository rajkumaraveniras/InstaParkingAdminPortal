using InstaParking.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace InstaParking.DAL
{
    public class CompanyInfo_DAL
    {
        SqlHelper sqlhelper_obj;
        public int SaveAccountDetails(Account account_data, string CreatedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string account_data_status = string.Empty;
            int result;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SaveCompanyInfo", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;

                        if (account_data.AccountID.ToString() != "" && account_data.AccountID != 0)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Update");
                            sqlcmd_details_obj.Parameters.AddWithValue("@AccountID", account_data.AccountID);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Insert");
                            sqlcmd_details_obj.Parameters.AddWithValue("@AccountID", DBNull.Value);
                        }

                        sqlcmd_details_obj.Parameters.AddWithValue("@AccountName", account_data.AccountName);
                        sqlcmd_details_obj.Parameters.AddWithValue("@Address1", String.IsNullOrEmpty(account_data.Address1) ? (object)DBNull.Value : account_data.Address1.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@Address2", String.IsNullOrEmpty(account_data.Address2) ? (object)DBNull.Value : account_data.Address2.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@ContactNumber", String.IsNullOrEmpty(account_data.ContactNumber) ? (object)DBNull.Value : account_data.ContactNumber.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@AlternateNumber", String.IsNullOrEmpty(account_data.AlternateNumber) ? (object)DBNull.Value : account_data.AlternateNumber.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@Email", String.IsNullOrEmpty(account_data.Email) ? (object)DBNull.Value : account_data.Email.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@Website", String.IsNullOrEmpty(account_data.Website) ? (object)DBNull.Value : account_data.Website.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@GSTNumber", String.IsNullOrEmpty(account_data.GSTNumber) ? (object)DBNull.Value : account_data.GSTNumber.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@WhatsAppNumber", String.IsNullOrEmpty(account_data.WhatsAppNumber) ? (object)DBNull.Value : account_data.WhatsAppNumber.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@SupportContactNumber", String.IsNullOrEmpty(account_data.SupportContactNumber) ? (object)DBNull.Value : account_data.SupportContactNumber.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@SupportEmailID", String.IsNullOrEmpty(account_data.SupportEmailID) ? (object)DBNull.Value : account_data.SupportEmailID.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@CreatedBy", Convert.ToInt32(CreatedBy));
                        if (account_data.IsActive.ToString() == "")
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", true);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", Convert.ToBoolean(account_data.IsActive));
                        }

                        sqlcmd_details_obj.Parameters.Add("@Output_identity", SqlDbType.Int).Direction = ParameterDirection.Output;
                        sqlconn_obj.Open();
                        int res = sqlcmd_details_obj.ExecuteNonQuery();
                        result = Convert.ToInt32(sqlcmd_details_obj.Parameters["@Output_identity"].Value);
                        sqlconn_obj.Close();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string InsertCompanyLogo(string ImageName, Account accountObj)
        {
            sqlhelper_obj = new SqlHelper();
            string message = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_InsertCompanyLogo", sqlconn_obj))
                    {

                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.Parameters.AddWithValue("@AccountID", accountObj.AccountID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@ImageName", String.IsNullOrEmpty(ImageName) ? (object)DBNull.Value : ImageName.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@UpdatedBy", accountObj.CreatedBy);
                        sqlconn_obj.Open();
                        int result = sqlcmd_details_obj.ExecuteNonQuery();
                        sqlconn_obj.Close();
                        if (result > 0)
                        {
                            message = "Record Sucessfully Created.";
                        }
                        else
                        {
                            message = "Failed To Insert.";
                        }
                    }
                }
                return message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Account> GetAccountDetails()
        {
            sqlhelper_obj = new SqlHelper();
            List<Account> account_List = new List<Account>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetAccountDetails", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_user = ds.Tables[0];
                        if (dt_user.Rows.Count > 0)
                        {
                            DataRow row = dt_user.Rows[0];
                            Account account_obj = new Account();
                            account_obj.AccountID = Convert.ToInt32(row["AccountID"]);
                            account_obj.AccountName = Convert.ToString(row["AccountName"]);
                            account_obj.Address1 = Convert.ToString(row["Address1"]);
                            account_obj.Address2 = Convert.ToString(row["Address2"]);
                            account_obj.ContactNumber = Convert.ToString(row["ContactNumber"]);
                            account_obj.AlternateNumber = Convert.ToString(row["AlternateNumber"]);
                            account_obj.Email = Convert.ToString(row["Email"]);
                            account_obj.Website = Convert.ToString(row["Website"]);
                            account_obj.GSTNumber = Convert.ToString(row["GSTNumber"]);
                            account_obj.WhatsAppNumber = Convert.ToString(row["WhatsAppNumber"]);
                            account_obj.SupportContactNumber = Convert.ToString(row["SupportContactNumber"]);
                            account_obj.SupportEmailID = Convert.ToString(row["SupportEmailID"]);
                            account_obj.IsActive = Convert.ToBoolean(row["IsActive"]);
                            account_obj.CompanyLogo = Convert.ToString(row["CompanyLogo"]);
                            account_List.Add(account_obj);
                        }
                    }
                }
                return account_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<OfferedSpaces> GetOfferedSpaceDetails()
        {
            IList<OfferedSpaces> offerSpacesList = new List<OfferedSpaces>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getzones_obj = new SqlCommand("PARK_PROC_GetOfferedSpacesList", sqlconn_obj))
                    {
                        sqlcmd_getzones_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getzones_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getzones_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            OfferedSpaces space_obj = new OfferedSpaces();
                            space_obj.OfferMySpaceID = Convert.ToInt32(dt.Rows[i]["OfferMySpaceID"]);
                            space_obj.Name = Convert.ToString(dt.Rows[i]["Name"]);
                            space_obj.PhoneNumber = Convert.ToString(dt.Rows[i]["PhoneNumber"]);
                            space_obj.Email = Convert.ToString(dt.Rows[i]["Email"]);
                            space_obj.OtherDetails = Convert.ToString(dt.Rows[i]["OtherDetails"]);
                            space_obj.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            space_obj.CreatedOn = dt.Rows[i]["CreatedOn"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(dt.Rows[i]["CreatedOn"]);
                            offerSpacesList.Add(space_obj);
                        }
                    }
                }
                return offerSpacesList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<SupportRequests> GetSupportRequestDetails()
        {
            IList<SupportRequests> requestList = new List<SupportRequests>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getzones_obj = new SqlCommand("PARK_PROC_GetSupportRequestsList", sqlconn_obj))
                    {
                        sqlcmd_getzones_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getzones_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getzones_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            SupportRequests request_obj = new SupportRequests();
                            request_obj.SupportRequestID = Convert.ToInt32(dt.Rows[i]["SupportRequestID"]);
                            request_obj.Name = Convert.ToString(dt.Rows[i]["Name"]);
                            request_obj.PhoneNumber = Convert.ToString(dt.Rows[i]["PhoneNumber"]);
                            request_obj.Email = Convert.ToString(dt.Rows[i]["Email"]);
                            request_obj.Message = Convert.ToString(dt.Rows[i]["Message"]);
                            request_obj.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            request_obj.CreatedOn = dt.Rows[i]["CreatedOn"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(dt.Rows[i]["CreatedOn"]);
                            requestList.Add(request_obj);
                        }
                    }
                }
                return requestList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
