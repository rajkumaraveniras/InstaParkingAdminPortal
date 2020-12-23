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
    public class Locations_DAL
    {
        SqlHelper sqlhelper_obj;
        public IList<Locations> GetLocationsList()
        {
            IList<Locations> locationsList = new List<Locations>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getlocations_obj = new SqlCommand("PARK_PROC_GetAllLocations", sqlconn_obj))
                    {
                        sqlcmd_getlocations_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getlocations_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getlocations_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            Locations locations_obj = new Locations();
                            locations_obj.LocationID = Convert.ToInt32(dt.Rows[i]["LocationID"]);
                            locations_obj.LocationCode = Convert.ToString(dt.Rows[i]["LocationCode"]);
                            locations_obj.LocationName = Convert.ToString(dt.Rows[i]["LocationName"]);
                            locations_obj.LocationDesc = Convert.ToString(dt.Rows[i]["LocationDesc"]);
                            locations_obj.Address = Convert.ToString(dt.Rows[i]["Address"]);
                            //locations_obj.Lattitude = (float)Convert.ToDouble(dt.Rows[i]["Lattitude"]);
                            //locations_obj.Longitude = (float)Convert.ToDouble(dt.Rows[i]["Longitude"]);
                            locations_obj.Lattitude = Convert.ToDecimal(dt.Rows[i]["Lattitude"]);
                            locations_obj.Longitude = Convert.ToDecimal(dt.Rows[i]["Longitude"]);
                            locations_obj.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            locations_obj.UpdatedOn = dt.Rows[i]["UpdatedOn"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(dt.Rows[i]["UpdatedOn"]);
                            locationsList.Add(locations_obj);
                        }
                    }
                }
                return locationsList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string InsertAndUpdateLocation(Locations locations_data, string CreatedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string locations_data_status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SaveLocation", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        if (locations_data.LocationID.ToString() != "" && locations_data.LocationID != 0)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Update");
                            sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", locations_data.LocationID);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Insert");
                            sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", DBNull.Value);
                        }
                                                
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationCode", String.IsNullOrEmpty(locations_data.LocationCode) ? (object)DBNull.Value : locations_data.LocationCode.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationName", String.IsNullOrEmpty(locations_data.LocationName) ? (object)DBNull.Value : locations_data.LocationName.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@Address", String.IsNullOrEmpty(locations_data.Address) ? (object)DBNull.Value : locations_data.Address.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@Lattitude", String.IsNullOrEmpty(locations_data.Lattitude.ToString()) ? (object)DBNull.Value : locations_data.Lattitude.ToString().Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@Longitude", String.IsNullOrEmpty(locations_data.Longitude.ToString()) ? (object)DBNull.Value : locations_data.Longitude.ToString().Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@UpdatedBy", CreatedBy);
                        if (locations_data.IsActive.ToString() == "")
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", true);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", Convert.ToBoolean(locations_data.IsActive));
                        }

                        sqlconn_obj.Open();
                        int result = sqlcmd_details_obj.ExecuteNonQuery();
                        sqlconn_obj.Close();
                        if (result > 0)
                        {
                            locations_data_status = "Success";
                        }
                        else
                        {
                            locations_data_status = "Failed";
                        }
                    }
                }
                return locations_data_status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Locations> ViewLocation(int LocationID)
        {
            sqlhelper_obj = new SqlHelper();
            List<Locations> locations_List = new List<Locations>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_ViewLocationDetails", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", LocationID);
                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_location = ds.Tables[0];
                        if (dt_location.Rows.Count > 0)
                        {
                            DataRow row = dt_location.Rows[0];
                            Locations location_obj = new Locations();
                            location_obj.LocationID = Convert.ToInt32(row["LocationID"]);
                            location_obj.LocationCode = Convert.ToString(row["LocationCode"]);
                            location_obj.LocationName = Convert.ToString(row["LocationName"]);
                            location_obj.Address = Convert.ToString(row["Address"]);
                            //location_obj.Lattitude = (float)Convert.ToDouble(row["Lattitude"]);
                            //location_obj.Longitude = (float)Convert.ToDouble(row["Longitude"]);
                            location_obj.Lattitude = Convert.ToDecimal(row["Lattitude"]);
                            location_obj.Longitude = Convert.ToDecimal(row["Longitude"]);
                            location_obj.IsActive = Convert.ToBoolean(row["IsActive"]);
                            locations_List.Add(location_obj);
                        }
                    }
                }
                return locations_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<Locations> GetActiveLocationList()
        {
            IList<Locations> locationsList = new List<Locations>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getlocation_obj = new SqlCommand("PARK_PROC_GetAllActiveLocations", sqlconn_obj))
                    {
                        sqlcmd_getlocation_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getlocation_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getlocation_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            Locations location_obj = new Locations();
                            location_obj.LocationID = Convert.ToInt32(dt.Rows[i]["LocationID"]);
                            location_obj.LocationName = Convert.ToString(dt.Rows[i]["LocationName"]);
                            locationsList.Add(location_obj);
                        }
                    }
                }
                return locationsList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
