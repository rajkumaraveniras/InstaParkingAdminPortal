using InstaParking.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace InstaParking.DAL
{
    public class Users_DAL
    {
        SqlHelper sqlhelper_obj;
        GetLogo getlogo = new GetLogo();
        PasswordEncryptDecrypt pwdEnDecrypt_obj = new PasswordEncryptDecrypt();
        public DataSet ValidateUser(string UserName, string Password)
        {
            try
            {
                string EncryptionKey = getEncryptdata(UserName);
                //string EncryptionKey = getEncryptdata1(UserName);
                string Encrypted = pwdEnDecrypt_obj.Encrypt(Password, EncryptionKey);

                sqlhelper_obj = new SqlHelper();
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_login_obj = new SqlCommand("PARK_PROC_LoginUser", sqlconn_obj))
                    {
                        sqlcmd_login_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_login_obj.CommandTimeout = 0;
                        sqlcmd_login_obj.Parameters.AddWithValue("@UserName", UserName);
                        sqlcmd_login_obj.Parameters.AddWithValue("@Password", Encrypted);

                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_login_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];
                        return ds;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<User> GetEmployeesList()
        {
            IList<User> usersList = new List<User>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getusers_obj = new SqlCommand("PARK_PROC_GetAllEmployees", sqlconn_obj))
                    {
                        sqlcmd_getusers_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getusers_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getusers_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            User users_obj = new User();
                            users_obj.UserID = Convert.ToInt32(dt.Rows[i]["UserID"]);
                            users_obj.UserTypeName = Convert.ToString(dt.Rows[i]["UserTypeName"]);
                            users_obj.UserCode = Convert.ToString(dt.Rows[i]["UserCode"]);
                            users_obj.UserName = Convert.ToString(dt.Rows[i]["UserName"]);
                            users_obj.Supervisor = Convert.ToString(dt.Rows[i]["Supervisor"]);
                            users_obj.Password = Convert.ToString(dt.Rows[i]["Password"]);
                            users_obj.PhoneNumber = Convert.ToString(dt.Rows[i]["PhoneNumber"]);
                            users_obj.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            users_obj.UpdatedOn = dt.Rows[i]["UpdatedOn"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(dt.Rows[i]["UpdatedOn"]);
                            // users_obj.AssignedLocationID = Convert.ToString(dt.Rows[i]["AssignedTo"]);
                            usersList.Add(users_obj);
                        }
                    }
                }
                return usersList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<User> GetActiveSupervisorsList()
        {
            IList<User> userList = new List<User>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getusers_obj = new SqlCommand("PARK_PROC_GetAcitveSupervisors", sqlconn_obj))
                    {
                        sqlcmd_getusers_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getusers_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getusers_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            User user_obj = new User();
                            user_obj.UserID = Convert.ToInt32(dt.Rows[i]["UserID"]);
                            user_obj.UserName = Convert.ToString(dt.Rows[i]["UserName"]);
                            userList.Add(user_obj);
                        }
                    }
                }
                return userList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int InsertAndUpdateUser(User user_data, string CreatedBy)
        {
            string pwd = Convert.ToString(user_data.Password);
            string EncryptionKey = pwdEnDecrypt_obj.GenerateEncryptionKey();
            string Encrypted = pwdEnDecrypt_obj.Encrypt(pwd, EncryptionKey);


            sqlhelper_obj = new SqlHelper();
            string users_data_status = string.Empty;
            int result;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SaveUser", sqlconn_obj))
                    {
                        // PARK_PROC_SaveUser
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        if (user_data.UserID.ToString() != "" && user_data.UserID != 0)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Update");
                            sqlcmd_details_obj.Parameters.AddWithValue("@UserID", user_data.UserID);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Insert");
                            sqlcmd_details_obj.Parameters.AddWithValue("@UserID", DBNull.Value);
                        }

                        sqlcmd_details_obj.Parameters.AddWithValue("@UserTypeID", user_data.UserTypeID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@UserCode", String.IsNullOrEmpty(user_data.UserCode) ? (object)DBNull.Value : user_data.UserCode.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@UserName", String.IsNullOrEmpty(user_data.UserName) ? (object)DBNull.Value : user_data.UserName.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@PhoneNumber", String.IsNullOrEmpty(user_data.PhoneNumber) ? (object)DBNull.Value : user_data.PhoneNumber.Trim());
                        // sqlcmd_details_obj.Parameters.AddWithValue("@Password", String.IsNullOrEmpty(user_data.Password) ? (object)DBNull.Value : user_data.Password.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@Password", Encrypted.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@EncryptionKey", EncryptionKey.Trim());

                        if (user_data.SupervisorID.ToString() == "" || user_data.SupervisorID == 0)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@SupervisorID", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@SupervisorID", Convert.ToInt32(user_data.SupervisorID));
                        }
                        sqlcmd_details_obj.Parameters.AddWithValue("@UpdatedBy", CreatedBy);
                        if (user_data.IsActive.ToString() == "")
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", true);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", Convert.ToBoolean(user_data.IsActive));
                        }

                        sqlcmd_details_obj.Parameters.Add("@Output_identity", SqlDbType.Int).Direction = ParameterDirection.Output;

                        if (String.IsNullOrEmpty(user_data.JoiningDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@JoiningDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@JoiningDate", user_data.JoiningDate);
                        }
                        sqlcmd_details_obj.Parameters.AddWithValue("@Salary", String.IsNullOrEmpty(user_data.Salary.ToString()) ? (object)DBNull.Value : user_data.Salary);
                        sqlcmd_details_obj.Parameters.AddWithValue("@EPFAccountNumber", String.IsNullOrEmpty(user_data.EPFAccountNumber) ? (object)DBNull.Value : user_data.EPFAccountNumber.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@AltPhoneNumber", String.IsNullOrEmpty(user_data.AltPhoneNumber) ? (object)DBNull.Value : user_data.AltPhoneNumber.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@AadharNumber", String.IsNullOrEmpty(user_data.AadharNumber) ? (object)DBNull.Value : user_data.AadharNumber.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@PANNumber", String.IsNullOrEmpty(user_data.PANNumber) ? (object)DBNull.Value : user_data.PANNumber.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@Address", String.IsNullOrEmpty(user_data.Address) ? (object)DBNull.Value : user_data.Address.Trim());

                        if (user_data.IsOperator.ToString() == "")
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsOperator", false);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsOperator", Convert.ToBoolean(user_data.IsOperator));
                        }

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

        public int AssignStationtoUser(UserLocationMapper userstation_data, string CreatedBy)
        {
            sqlhelper_obj = new SqlHelper();
            int result;
            string data_status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    //if (Convert.ToString(userstation_data.AssignedLotID) != "" && userstation_data.AssignedLotID != 0)
                    //{
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_AssignStationtoEmployee", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;

                        if (userstation_data.UserLocationMapperID.ToString() != "" && userstation_data.UserLocationMapperID != 0)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Update");
                            sqlcmd_details_obj.Parameters.AddWithValue("@UserLocationMapperID", userstation_data.UserLocationMapperID);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Insert");
                            sqlcmd_details_obj.Parameters.AddWithValue("@UserLocationMapperID", DBNull.Value);
                        }

                        sqlcmd_details_obj.Parameters.AddWithValue("@UserID", userstation_data.UserID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", userstation_data.LocationID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LotID", userstation_data.LocationParkingLotID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@UpdatedBy", CreatedBy);
                        sqlcmd_details_obj.Parameters.Add("@Output_identity", SqlDbType.Int).Direction = ParameterDirection.Output;

                        if (userstation_data.IsActive.ToString() == "")
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", true);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", Convert.ToBoolean(userstation_data.IsActive));
                        }

                        sqlconn_obj.Open();
                        int res = sqlcmd_details_obj.ExecuteNonQuery();
                        result = Convert.ToInt32(sqlcmd_details_obj.Parameters["@Output_identity"].Value);
                        sqlconn_obj.Close();

                    }
                    // }
                    //else
                    //{

                    //    for (int k = 0; k < userstation_data.AssignedLocationID.ToString().Length - 1; k++)
                    //    {
                    //        int location = Convert.ToInt32(userstation_data.AssignedLocationID.ToString().Split(',')[k]);


                    //        using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_AssignStationtoEmployee", sqlconn_obj))
                    //        {
                    //            sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                    //            sqlcmd_details_obj.CommandTimeout = 0;

                    //            sqlcmd_details_obj.Parameters.AddWithValue("@UserID", userstation_data.UserID);
                    //            sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", location);
                    //            sqlcmd_details_obj.Parameters.AddWithValue("@LotID", DBNull.Value);

                    //            sqlcmd_details_obj.Parameters.AddWithValue("@UpdatedBy", CreatedBy);

                    //            sqlconn_obj.Open();
                    //            int result = sqlcmd_details_obj.ExecuteNonQuery();
                    //            sqlconn_obj.Close();
                    //            if (result > 0)
                    //            {
                    //                data_status = "Success";
                    //            }
                    //            else
                    //            {
                    //                data_status = "Failed";
                    //            }
                    //        }
                    //    }
                    //}
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UserLocationMapper> GetAssignStationByID(int UserLocationMapperID)
        {
            sqlhelper_obj = new SqlHelper();
            List<UserLocationMapper> list = new List<UserLocationMapper>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_ViewUserLocationMapperDetails", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@UserLocationMapperID", UserLocationMapperID);
                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            UserLocationMapper map_obj = new UserLocationMapper();
                            map_obj.UserLocationMapperID = Convert.ToInt32(row["UserLocationMapperID"]);
                            map_obj.UserID = Convert.ToInt32(row["UserID"]);
                            map_obj.LocationID = Convert.ToInt32(row["LocationID"]);
                            map_obj.LocationParkingLotID = Convert.ToInt32(row["LotID"]);
                            map_obj.IsActive = Convert.ToBoolean(row["IsActive"]);
                            list.Add(map_obj);
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string DeleteAssignStation(int userlocationid, string DeletedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string data_status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_DeleteUserLocationMapper", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@UserLocationMapperID", userlocationid);
                        sqlcmd_details_obj.Parameters.AddWithValue("@DeletedBy", DeletedBy);
                        sqlconn_obj.Open();
                        int result = sqlcmd_details_obj.ExecuteNonQuery();
                        sqlconn_obj.Close();
                        if (result > 0)
                        {
                            data_status = "Success";
                        }
                        else
                        {
                            data_status = "Failed";
                        }
                    }
                }
                return data_status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<User> ViewEmployee(int EmployeeID)
        {
            sqlhelper_obj = new SqlHelper();
            List<User> users_List = new List<User>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_ViewUserDetails", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@UserID", EmployeeID);
                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_user = ds.Tables[0];

                        //string locationsids = "";
                        //for (int i = 0; i < dt_user.Rows.Count; i++)
                        //{
                        //    locationsids += Convert.ToString(dt_user.Rows[i]["LocationID"]) + ",";
                        //}

                        //locationsids = locationsids.Remove(locationsids.Length - 1, 1);

                        if (dt_user.Rows.Count > 0)
                        {
                            DataRow row = dt_user.Rows[0];
                            User users_obj = new User();
                            users_obj.UserID = Convert.ToInt32(row["UserID"]);
                            users_obj.UserTypeID = Convert.ToInt32(row["UserTypeID"]);
                            //users_obj.UserTypeName = Convert.ToString(row["UserTypeName"]);
                            users_obj.UserCode = Convert.ToString(row["UserCode"]);
                            users_obj.UserName = Convert.ToString(row["UserName"]);
                            string Decrypted = pwdEnDecrypt_obj.Decrypt(Convert.ToString(row["Password"]), Convert.ToString(row["EncryptedKey"]));
                            users_obj.Password = Decrypted;
                            users_obj.PhoneNumber = Convert.ToString(row["PhoneNumber"]);
                            users_obj.SupervisorID = row["SupervisorID"] == DBNull.Value ? 0 : Convert.ToInt32(row["SupervisorID"]);
                            users_obj.IsActive = Convert.ToBoolean(row["IsActive"]);

                            users_obj.JoiningDate = Convert.ToString(row["JoiningDate"]);
                            users_obj.Salary = row["Salary"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Salary"]);
                            users_obj.EPFAccountNumber = Convert.ToString(row["EPFAccountNumber"]);
                            users_obj.AltPhoneNumber = Convert.ToString(row["AltPhoneNumber"]);
                            users_obj.Address = Convert.ToString(row["Address"]);
                            users_obj.AadharNumber = Convert.ToString(row["AadharNumber"]);
                            users_obj.PANNumber = Convert.ToString(row["PANNumber"]);

                            users_obj.Photo = Convert.ToString(row["EmpPhoto"]);
                            users_obj.AadharPhoto = Convert.ToString(row["EmpAadharPhoto"]);
                            users_obj.PANPhoto = Convert.ToString(row["EmpPANPhoto"]);

                            users_obj.IsOperator = row["IsOperator"] == DBNull.Value ? false : Convert.ToBoolean(row["IsOperator"]);
                            //if (!String.IsNullOrEmpty(Convert.ToString(row["LotImage"])))

                            //if (!String.IsNullOrEmpty(Convert.ToString(row["Photo"])))
                            //{
                            //    users_obj.Photo = getlogo.ShowEmployeePhoto(Convert.ToString(row["UserID"]), "Photo");
                            //}
                            //else
                            //{
                            //    users_obj.Photo = "";
                            //}
                            //if (!String.IsNullOrEmpty(Convert.ToString(row["AadharPhoto"])))
                            //{
                            //    users_obj.AadharPhoto = getlogo.ShowAadhar(Convert.ToString(row["UserID"]), "Aadhar");
                            //}
                            //else
                            //{
                            //    users_obj.AadharPhoto = "";
                            //}
                            //if (!String.IsNullOrEmpty(Convert.ToString(row["PANPhoto"])))
                            //{
                            //    users_obj.PANPhoto = getlogo.ShowPAN(Convert.ToString(row["UserID"]), "PAN");
                            //}
                            //else
                            //{
                            //    users_obj.PANPhoto = "";
                            //}
                            users_List.Add(users_obj);
                        }
                    }
                }
                return users_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Stream ShowImage(int UserID, string name)
        {
            try
            {
                sqlhelper_obj = new SqlHelper();
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_getlogo = new SqlCommand("PARK_PROC_GetEmployeeImages", sqlconn_obj))
                    {
                        sqlconn_obj.Open();
                        sqlcmd_getlogo.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getlogo.Parameters.AddWithValue("@UserID", UserID);
                        sqlcmd_getlogo.Parameters.AddWithValue("@name", name);
                        object img = sqlcmd_getlogo.ExecuteScalar();
                        sqlconn_obj.Close();
                        return new MemoryStream((byte[])img);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<User> GetActiveUsersList()
        {
            IList<User> userList = new List<User>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getusers_obj = new SqlCommand("PARK_PROC_GetAcitveUsers", sqlconn_obj))
                    {
                        sqlcmd_getusers_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getusers_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getusers_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            User user_obj = new User();
                            user_obj.UserID = Convert.ToInt32(dt.Rows[i]["UserID"]);
                            user_obj.UserName = Convert.ToString(dt.Rows[i]["UserName"]);
                            userList.Add(user_obj);
                        }
                    }
                }
                return userList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string InsertEmployeeFiles(string EmpPhotoData, string AadharData, string PANData, User user)
        {
            sqlhelper_obj = new SqlHelper();
            string message = string.Empty;

            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_InsertEmployeeImages", sqlconn_obj))
                    {

                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.Parameters.AddWithValue("@UserID", user.UserID);
                        //if (EmpPhotoData.ToString() != "" && EmpPhotoData.ToString() != null)
                        //{
                        //    sqlcmd_details_obj.Parameters.AddWithValue("@Photo", EmpPhotoData);
                        //}
                        //else
                        //{
                        //    sqlcmd_details_obj.Parameters.AddWithValue("@Photo", DBNull.Value);
                        //}
                        //if (AadharData.ToString() != "" && AadharData.ToString() != null)
                        //{
                        //    sqlcmd_details_obj.Parameters.AddWithValue("@AadharProof", AadharData);
                        //}
                        //else
                        //{
                        //    sqlcmd_details_obj.Parameters.AddWithValue("@AadharProof", DBNull.Value);
                        //}
                        //if (PANData.ToString() != "" && PANData.ToString() != null)
                        //{
                        //    sqlcmd_details_obj.Parameters.AddWithValue("@PANProof", PANData);
                        //}
                        //else
                        //{
                        //    sqlcmd_details_obj.Parameters.AddWithValue("@PANProof", DBNull.Value);
                        //}
                        sqlcmd_details_obj.Parameters.AddWithValue("@EmpPhoto", String.IsNullOrEmpty(EmpPhotoData) ? (object)DBNull.Value : EmpPhotoData.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@EmpAadharPhoto", String.IsNullOrEmpty(AadharData) ? (object)DBNull.Value : AadharData.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@EmpPANPhoto", String.IsNullOrEmpty(PANData) ? (object)DBNull.Value : PANData.Trim());

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

        public IList<User> GetDashboardSupervisorsList()
        {
            IList<User> usersList = new List<User>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getusers_obj = new SqlCommand("PARK_PROC_GetActiveSupervisors", sqlconn_obj))
                    {
                        sqlcmd_getusers_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getusers_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getusers_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            User users_obj = new User();
                            users_obj.UserID = Convert.ToInt32(dt.Rows[i]["UserID"]);
                            users_obj.UserCode = Convert.ToString(dt.Rows[i]["UserCode"]);
                            users_obj.UserName = Convert.ToString(dt.Rows[i]["UserName"]);
                            users_obj.AssignedLocationID = Convert.ToString(dt.Rows[i]["AssignedTo"]);
                            users_obj.PhoneNumber = Convert.ToString(dt.Rows[i]["PhoneNumber"]);
                            users_obj.Address = Convert.ToString(dt.Rows[i]["Address"]);
                            usersList.Add(users_obj);
                        }
                    }
                }
                return usersList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<UserLocationMapper> GetAssignStationList(int UserID)
        {
            IList<UserLocationMapper> lst = new List<UserLocationMapper>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getlots_obj = new SqlCommand("PARK_PROC_GetAssignedStationsbyUserID", sqlconn_obj))
                    {
                        sqlcmd_getlots_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getlots_obj.CommandTimeout = 0;
                        sqlcmd_getlots_obj.Parameters.AddWithValue("@UserID", UserID);
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getlots_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            UserLocationMapper user_obj = new UserLocationMapper();
                            user_obj.UserLocationMapperID = Convert.ToInt32(dt.Rows[i]["UserLocationMapperID"]);
                            user_obj.UserID = Convert.ToInt32(dt.Rows[i]["UserID"]);
                            user_obj.LocationID = Convert.ToInt32(dt.Rows[i]["LocationID"]);
                            user_obj.LocationName = Convert.ToString(dt.Rows[i]["LocationName"]);
                            user_obj.LocationParkingLotID = Convert.ToInt32(dt.Rows[i]["LotID"]);
                            user_obj.LocationParkingLotName = Convert.ToString(dt.Rows[i]["LocationParkingLotName"]);
                            user_obj.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            user_obj.UpdatedOn = dt.Rows[i]["UpdatedOn"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(dt.Rows[i]["UpdatedOn"]);
                            lst.Add(user_obj);
                        }
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getEncryptdata(string UserName)
        {
            string encryptkey = "";
            try
            {
                sqlhelper_obj = new SqlHelper();
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_GetEncryptKey", sqlconn_obj))
                    {
                        sqlcmd_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_obj.CommandTimeout = 0;
                        sqlcmd_obj.Parameters.AddWithValue("@UserName", UserName.Trim());
                        DataTable dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_obj))
                        {
                            da.Fill(dt);
                        }
                        encryptkey = Convert.ToString(dt.Rows[0]["EncryptedKey"]);
                    }
                }
                return encryptkey;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckLocationExistforReportToEmployee(int SupervisorID,int LocationID)
        {
            sqlhelper_obj = new SqlHelper();
            string users_data_status = string.Empty;
            int result;
            bool res;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_CheckLocationExistforReportToEmployee", sqlconn_obj))
                    {  
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@SupervisorID", SupervisorID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", LocationID);
                        sqlconn_obj.Open();
                        result = Convert.ToInt32(sqlcmd_details_obj.ExecuteScalar());
                        if (result > 0)
                        {
                            res = true;
                        }
                        else {
                            res = false;
                        }
                        sqlconn_obj.Close();
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public string getEncryptdata1(string UserName)
        //{
        //    sqlhelper_obj = new SqlHelper();

        //    string encryptkey = "";
        //    try
        //    {
        //        using (SqlConnection sqlconn_obj = new SqlConnection())
        //        {
        //            sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
        //            using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_GetEncryptKey", sqlconn_obj))
        //            {
        //                sqlcmd_obj.CommandType = CommandType.StoredProcedure;
        //                sqlcmd_obj.Parameters.AddWithValue("@UserName", UserName.Trim());
        //               // sqlconn_obj.Open();
        //                DataTable resultdt = new DataTable();
        //                // SqlDataAdapter sqldap = new SqlDataAdapter(sqlcmd_obj);
        //                //sqldap.Fill(resultdt);
        //                using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_obj))
        //                {
        //                    da.Fill(resultdt);
        //                }
        //                if (resultdt.Rows.Count > 0)
        //                {
        //                    encryptkey = Convert.ToString(resultdt.Rows[0]["EncryptedKey"]);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //objExceptionlog.InsertException("WebAPI", ex.Message, "DALUserLoginVerification", "Proc: " + "PARK_PROC_GetEncryptKey", "getEncryptdata");
        //    }


        //    return encryptkey;

        //}

        public List<User> ViewEmployeeProfile(int EmployeeID)
        {
            sqlhelper_obj = new SqlHelper();
            List<User> users_List = new List<User>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetEmployeeProfile", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@UserID", EmployeeID);
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
                            User users_obj = new User();
                            users_obj.UserID = Convert.ToInt32(row["UserID"]);
                            users_obj.UserTypeID = Convert.ToInt32(row["UserTypeID"]);
                            users_obj.UserTypeName = Convert.ToString(row["UserTypeName"]);
                            users_obj.UserCode = Convert.ToString(row["UserCode"]);
                            users_obj.UserName = Convert.ToString(row["UserName"]);
                            users_obj.PhoneNumber = Convert.ToString(row["PhoneNumber"]);
                            users_obj.SupervisorID = row["SupervisorID"] == DBNull.Value ? 0 : Convert.ToInt32(row["SupervisorID"]);
                            users_obj.IsActive = Convert.ToBoolean(row["IsActive"]);

                            users_obj.JoiningDate = Convert.ToString(row["JoiningDate"]);
                            users_obj.Salary = row["Salary"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Salary"]);
                            users_obj.EPFAccountNumber = Convert.ToString(row["EPFAccountNumber"]);
                            users_obj.AltPhoneNumber = Convert.ToString(row["AltPhoneNumber"]);
                            users_obj.Address = Convert.ToString(row["Address"]);
                            users_obj.AadharNumber = Convert.ToString(row["AadharNumber"]);
                            users_obj.PANNumber = Convert.ToString(row["PANNumber"]);
                            users_List.Add(users_obj);
                        }
                    }
                }
                return users_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
