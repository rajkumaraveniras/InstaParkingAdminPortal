using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstaParking.Models;
using System.Data;
using System.Data.SqlClient;

namespace InstaParking.DAL
{
    public class AssignUserStation_DAL
    {
        SqlHelper sqlhelper_obj;
        public IList<AssignUserStation> GetAssignList()
        {
            IList<AssignUserStation> assignList = new List<AssignUserStation>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getassigns_obj = new SqlCommand("PARK_PROC_GetAllAssigns", sqlconn_obj))
                    {
                        sqlcmd_getassigns_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getassigns_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getassigns_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            AssignUserStation assign_obj = new AssignUserStation();
                            assign_obj.UserLocationMapperID = Convert.ToInt32(dt.Rows[i]["UserLocationMapperID"]);                            
                            assign_obj.UserCode = Convert.ToString(dt.Rows[i]["UserCode"]);
                            assign_obj.UserName = Convert.ToString(dt.Rows[i]["UserName"]);
                            assign_obj.LocationName = Convert.ToString(dt.Rows[i]["LocationName"]);
                            assign_obj.LotName = Convert.ToString(dt.Rows[i]["LocationParkingLotName"]);
                            assign_obj.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            assign_obj.UpdatedOn = dt.Rows[i]["UpdatedOn"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(dt.Rows[i]["UpdatedOn"]);
                            assignList.Add(assign_obj);
                        }
                    }
                }
                return assignList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string AssignStation(AssignUserStation userstation_data, string CreatedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string data_status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_AssignStationtoUser", sqlconn_obj))
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
                        sqlcmd_details_obj.Parameters.AddWithValue("@LotID", userstation_data.LotID);

                        sqlcmd_details_obj.Parameters.AddWithValue("@UpdatedBy", CreatedBy);
                        if (userstation_data.IsActive.ToString() == "")
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", true);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", Convert.ToBoolean(userstation_data.IsActive));
                        }

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

        public List<AssignUserStation> ViewAssign(int AssignID)
        {
            sqlhelper_obj = new SqlHelper();
            List<AssignUserStation> assign_List = new List<AssignUserStation>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_ViewAssignDetails", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@AssignID", AssignID);
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
                            AssignUserStation assign_obj = new AssignUserStation();
                            assign_obj.UserLocationMapperID = Convert.ToInt32(row["UserLocationMapperID"]);
                            assign_obj.UserID = Convert.ToInt32(row["UserID"]);
                            assign_obj.LocationID = Convert.ToInt32(row["LocationID"]);
                            assign_obj.LotID = Convert.ToInt32(row["LotID"]);
                            assign_obj.IsActive = Convert.ToBoolean(row["IsActive"]);
                            assign_List.Add(assign_obj);
                        }
                    }
                }
                return assign_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
