using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using InstaParking.Models;

namespace InstaParking.DAL
{
    public class UserType_DAL
    {
        SqlHelper sqlhelper_obj;
        public IList<UserType> GetRolesList()
        {
            IList<UserType> userTypeList = new List<UserType>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getroles_obj = new SqlCommand("PARK_PROC_GetAllRoles", sqlconn_obj))
                    {
                        sqlcmd_getroles_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getroles_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getroles_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            UserType userType_obj = new UserType();
                            userType_obj.UserTypeID = Convert.ToInt32(dt.Rows[i]["UserTypeID"]);
                            userType_obj.UserTypeCode = Convert.ToString(dt.Rows[i]["UserTypeCode"]);
                            userType_obj.UserTypeName = Convert.ToString(dt.Rows[i]["UserTypeName"]);
                            userType_obj.UserTypeDesc = Convert.ToString(dt.Rows[i]["UserTypeDesc"]);
                            userType_obj.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            userType_obj.UpdatedOn = dt.Rows[i]["UpdatedOn"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(dt.Rows[i]["UpdatedOn"]);
                            userTypeList.Add(userType_obj);
                        }
                    }
                }
                return userTypeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string InsertAndUpdateRole(UserType roles_data, string CreatedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string roles_data_status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SaveRole", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        if (roles_data.UserTypeID.ToString() != "" && roles_data.UserTypeID != 0)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Update");
                            sqlcmd_details_obj.Parameters.AddWithValue("@UserTypeID", roles_data.UserTypeID);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Insert");
                            sqlcmd_details_obj.Parameters.AddWithValue("@UserTypeID", DBNull.Value);
                        }

                        sqlcmd_details_obj.Parameters.AddWithValue("@UserTypeCode", String.IsNullOrEmpty(roles_data.UserTypeCode) ? (object)DBNull.Value : roles_data.UserTypeCode.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@UserTypeName", String.IsNullOrEmpty(roles_data.UserTypeName) ? (object)DBNull.Value : roles_data.UserTypeName.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@UserTypeDesc", String.IsNullOrEmpty(roles_data.UserTypeDesc) ? (object)DBNull.Value : roles_data.UserTypeDesc.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@UpdatedBy", CreatedBy);
                        if (roles_data.IsActive.ToString() == "")
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", true);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", Convert.ToBoolean(roles_data.IsActive));
                        }

                        sqlconn_obj.Open();
                        int result = sqlcmd_details_obj.ExecuteNonQuery();
                        sqlconn_obj.Close();
                        if (result > 0)
                        {
                            roles_data_status = "Success";
                        }
                        else
                        {
                            roles_data_status = "Failed";
                        }
                    }
                }
                return roles_data_status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserType> ViewRole(int RoleID)
        {
            sqlhelper_obj = new SqlHelper();
            List<UserType> roles_List = new List<UserType>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_ViewRoleDetails", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@UserTypeID", RoleID);
                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_role = ds.Tables[0];
                        if (dt_role.Rows.Count > 0)
                        {
                            DataRow row = dt_role.Rows[0];
                            UserType roles_obj = new UserType();
                            roles_obj.UserTypeID = Convert.ToInt32(row["UserTypeID"]);
                            roles_obj.UserTypeCode = Convert.ToString(row["UserTypeCode"]);
                            roles_obj.UserTypeName = Convert.ToString(row["UserTypeName"]);
                            roles_obj.UserTypeDesc = Convert.ToString(row["UserTypeDesc"]);
                            roles_obj.IsActive = Convert.ToBoolean(row["IsActive"]);
                            roles_List.Add(roles_obj);
                        }
                    }
                }
                return roles_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<UserType> GetActiveRolesList()
        {
            IList<UserType> userTypeList = new List<UserType>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getroles_obj = new SqlCommand("PARK_PROC_GetAcitveRoles", sqlconn_obj))
                    {
                        sqlcmd_getroles_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getroles_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getroles_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            UserType userType_obj = new UserType();
                            userType_obj.UserTypeID = Convert.ToInt32(dt.Rows[i]["UserTypeID"]);
                            userType_obj.UserTypeCode = Convert.ToString(dt.Rows[i]["UserTypeCode"]);
                            userType_obj.UserTypeName = Convert.ToString(dt.Rows[i]["UserTypeName"]);
                            userType_obj.UserTypeDesc = Convert.ToString(dt.Rows[i]["UserTypeDesc"]);
                            userType_obj.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            userType_obj.UpdatedOn = dt.Rows[i]["UpdatedOn"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(dt.Rows[i]["UpdatedOn"]);
                            userTypeList.Add(userType_obj);
                        }
                    }
                }
                return userTypeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
