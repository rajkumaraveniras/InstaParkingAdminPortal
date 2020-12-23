using InstaParking.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace InstaParking.DAL
{
    public class Dashboard_DAL
    {
        SqlHelper sqlhelper_obj;
        public IList<StationOccupancy> GetListofStationOccupancy()
        {
            IList<StationOccupancy> stationOccupancyList = new List<StationOccupancy>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARk_PROC_GetStationOverallOccupancy", sqlconn_obj))
                    {
                        sqlcmd_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            StationOccupancy stationOccupancy_obj = new StationOccupancy();
                            stationOccupancy_obj.LocationID = Convert.ToInt32(dt.Rows[i]["LocationID"]);
                            stationOccupancy_obj.LocationName = Convert.ToString(dt.Rows[i]["LocationName"]);
                            stationOccupancy_obj.Occupancy = Convert.ToString(dt.Rows[i]["Occupancy"]);                            
                            stationOccupancyList.Add(stationOccupancy_obj);
                        }
                    }
                }
                return stationOccupancyList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<StationOccupancyDetails> GetOccupancyDetailsByStation(int LocationID)
        {
            List<StationOccupancyDetails> stationOccupancyList = new List<StationOccupancyDetails>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_GetStationOccupancyAllDetails", sqlconn_obj))
                    {
                        sqlcmd_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_obj.CommandTimeout = 0;
                        sqlcmd_obj.Parameters.AddWithValue("@LocationID", LocationID);
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];
                        DataTable dt1 = ds.Tables[1];

                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            StationOccupancyDetails stationOccupancy_obj = new StationOccupancyDetails();
                            if(i==0)
                            {
                                stationOccupancy_obj.LocationName = Convert.ToString(dt.Rows[0]["LocationName"]);
                                stationOccupancy_obj.Occupancy = Convert.ToString(dt.Rows[0]["Occupancy"]);
                                stationOccupancy_obj.Supervisor = Convert.ToString(dt.Rows[0]["SupervisorName"]);
                            }
                            
                            stationOccupancy_obj.LocationParkingLotName = Convert.ToString(dt1.Rows[i]["LocationParkingLotName"]);
                            stationOccupancy_obj.TwoWheelereOccupancy = Convert.ToString(dt1.Rows[i]["2W-Occupancy"]);
                            stationOccupancy_obj.FourWheelereOccupancy = Convert.ToString(dt1.Rows[i]["4W-Occupancy"]);
                            stationOccupancy_obj.Operator = Convert.ToString(dt1.Rows[i]["Operator"]);
                            stationOccupancyList.Add(stationOccupancy_obj);
                        }
                    }
                }
                return stationOccupancyList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
