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
    public class Zones_DAL
    {
        SqlHelper sqlhelper_obj;
        public IList<Zones> GetZonesList()
        {
            IList<Zones> zonesList = new List<Zones>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getzones_obj = new SqlCommand("PARK_PROC_GetAllZones", sqlconn_obj))
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
                            Zones zones_obj = new Zones();
                            zones_obj.ZoneID = Convert.ToInt32(dt.Rows[i]["ZoneID"]);
                            zones_obj.City = Convert.ToString(dt.Rows[i]["CityName"]);
                            zones_obj.ZoneCode = Convert.ToString(dt.Rows[i]["ZoneCode"]);
                            zones_obj.ZoneName = Convert.ToString(dt.Rows[i]["ZoneName"]);
                            zones_obj.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            zones_obj.UpdatedOn = dt.Rows[i]["UpdatedOn"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(dt.Rows[i]["UpdatedOn"]);
                            zonesList.Add(zones_obj);
                        }
                    }
                }
                return zonesList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<City> GetActiveCitiesList()
        {
            IList<City> cityList = new List<City>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getcity_obj = new SqlCommand("PARK_PROC_GetActiveCities", sqlconn_obj))
                    {
                        sqlcmd_getcity_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getcity_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getcity_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            City city_obj = new City();
                            city_obj.CityID = Convert.ToInt32(dt.Rows[i]["CityID"]);
                            city_obj.CityName = Convert.ToString(dt.Rows[i]["CityName"]);
                            cityList.Add(city_obj);
                        }
                    }
                }
                return cityList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string InsertAndUpdateZone(Zones zones_data, string CreatedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string zones_data_status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SaveZone", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        if (zones_data.ZoneID.ToString() != "" && zones_data.ZoneID != 0)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Update");
                            sqlcmd_details_obj.Parameters.AddWithValue("@ZoneID", zones_data.ZoneID);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Insert");
                            sqlcmd_details_obj.Parameters.AddWithValue("@ZoneID", DBNull.Value);
                        }

                        sqlcmd_details_obj.Parameters.AddWithValue("@CityID", zones_data.CityID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@ZoneCode", String.IsNullOrEmpty(zones_data.ZoneCode) ? (object)DBNull.Value : zones_data.ZoneCode.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@ZoneName", String.IsNullOrEmpty(zones_data.ZoneName) ? (object)DBNull.Value : zones_data.ZoneName.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@UpdatedBy", CreatedBy);
                        if (zones_data.IsActive.ToString() == "")
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", true);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", Convert.ToBoolean(zones_data.IsActive));
                        }

                        sqlconn_obj.Open();
                        int result = sqlcmd_details_obj.ExecuteNonQuery();
                        sqlconn_obj.Close();
                        if (result > 0)
                        {
                            zones_data_status = "Success";
                        }
                        else
                        {
                            zones_data_status = "Failed";
                        }
                    }
                }
                return zones_data_status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Zones> ViewZone(int ZoneID)
        {
            sqlhelper_obj = new SqlHelper();
            List<Zones> zones_List = new List<Zones>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_ViewZoneDetails", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@ZoneID", ZoneID);
                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_zone = ds.Tables[0];
                        if (dt_zone.Rows.Count > 0)
                        {
                            DataRow row = dt_zone.Rows[0];
                            Zones zones_obj = new Zones();
                            zones_obj.ZoneID = Convert.ToInt32(row["ZoneID"]);
                            zones_obj.CityID = Convert.ToInt32(row["CityID"]);
                            zones_obj.ZoneCode = Convert.ToString(row["ZoneCode"]);
                            zones_obj.ZoneName = Convert.ToString(row["ZoneName"]);
                            zones_obj.IsActive = Convert.ToBoolean(row["IsActive"]);
                            zones_List.Add(zones_obj);
                        }
                    }
                }
                return zones_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
