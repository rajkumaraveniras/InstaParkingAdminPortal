using InstaParking.Models;
using System;
using System.Collections.Generic;
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
        public string InsertAndUpdateLocation(Locations locations_data, string CreatedBy, List<VehicleType> VehicleTypeList, List<int> PassList)
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

                        sqlcmd_details_obj.Parameters.AddWithValue("@TagType", Convert.ToInt32(locations_data.TagType));

                        sqlcmd_details_obj.Parameters.Add("@Output_identity", SqlDbType.Int).Direction = ParameterDirection.Output;
                        sqlconn_obj.Open();
                        int result = sqlcmd_details_obj.ExecuteNonQuery();
                        int locationID_result = Convert.ToInt32(sqlcmd_details_obj.Parameters["@Output_identity"].Value);

                        if (locations_data.LocationID.ToString() != "" && locations_data.LocationID != 0)
                        {
                            string updateLocationVehTyperesult = UpdateLocationVehicleTypeMapper(VehicleTypeList, CreatedBy, locations_data.LocationID);
                            string deleteLocPassMapperresult = DeleteLocationPassMapper(locationID_result);
                            string LocationPassresult = SaveLocationPassMapper(PassList, CreatedBy, locationID_result);
                        }
                        else
                        {
                            string LocationVehTyperesult = SaveLocationVehicleTypeMapper(VehicleTypeList, CreatedBy, locationID_result);
                            string LocationPassresult = SaveLocationPassMapper(PassList, CreatedBy, locationID_result);
                        }

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
                if (ex.Message.Contains("Violation of UNIQUE KEY constraint 'UC_Location'. Cannot insert duplicate key in object 'dbo.Location'."))
                {
                    locations_data_status = locations_data.LocationName.Trim() + " already exist in System.";
                    return locations_data_status;
                }
                else
                {
                    throw ex;
                }
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
                            location_obj.TagType = row["TagType"] == DBNull.Value ? 0 : Convert.ToInt32(row["TagType"]);
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
        public string VerifyLocationCode(Locations locations_data)
        {
            sqlhelper_obj = new SqlHelper();
            string locations_data_status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_VerifyLocationCode", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationCode", String.IsNullOrEmpty(locations_data.LocationCode) ? (object)DBNull.Value : locations_data.LocationCode.Trim());
                        sqlconn_obj.Open();
                        int result = Convert.ToInt32(sqlcmd_details_obj.ExecuteScalar());
                        sqlconn_obj.Close();
                        if (result == 0)
                        {
                            locations_data_status = "Not Exists";
                        }
                        else
                        {
                            locations_data_status = "Exists";
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
        public string SaveLocationVehicleTypeMapper(List<VehicleType> VehicleTypeList, string CreatedBy, int LocationID)
        {
            sqlhelper_obj = new SqlHelper();
            string mapper_data_status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    for (var i = 0; i < VehicleTypeList.Count; i++)
                    {
                        using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SaveLocationVehicleTypeMapper", sqlconn_obj))
                        {
                            sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                            sqlcmd_details_obj.CommandTimeout = 0;
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Insert");
                            sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", LocationID);
                            sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", VehicleTypeList[i].VehicleTypeID);
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", VehicleTypeList[i].selected);
                            sqlcmd_details_obj.Parameters.AddWithValue("@CreatedBy", CreatedBy);
                            sqlconn_obj.Open();
                            int result = sqlcmd_details_obj.ExecuteNonQuery();
                            sqlconn_obj.Close();
                            if (result > 0)
                            {
                                mapper_data_status = "Success";
                            }
                            else
                            {
                                mapper_data_status = "Failed";
                            }
                        }
                    }
                }
                return mapper_data_status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string UpdateLocationVehicleTypeMapper(List<VehicleType> VehicleTypeList, string CreatedBy, int LocationID)
        {
            sqlhelper_obj = new SqlHelper();
            string mapper_data_status = string.Empty;

            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    for (var i = 0; i < VehicleTypeList.Count; i++)
                    {
                        using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SaveLocationVehicleTypeMapper", sqlconn_obj))
                        {
                            sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                            sqlcmd_details_obj.CommandTimeout = 0;
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Update");
                            sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", LocationID);
                            sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", VehicleTypeList[i].VehicleTypeID);
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", VehicleTypeList[i].selected);
                            sqlcmd_details_obj.Parameters.AddWithValue("@CreatedBy", CreatedBy);
                            sqlconn_obj.Open();
                            int result = sqlcmd_details_obj.ExecuteNonQuery();
                            sqlconn_obj.Close();
                            if (result > 0)
                            {
                                mapper_data_status = "Success";
                            }
                            else
                            {
                                mapper_data_status = "Failed";
                            }
                        }
                    }

                }
                return mapper_data_status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<CardType> GetListofActiveTagTypes()
        {
            IList<CardType> tagTypeList = new List<CardType>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getvehicleType_obj = new SqlCommand("PARK_PROC_GetActiveCardTypes", sqlconn_obj))
                    {
                        sqlcmd_getvehicleType_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getvehicleType_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getvehicleType_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            CardType cardType_obj = new CardType();
                            cardType_obj.CardTypeID = Convert.ToInt32(dt.Rows[i]["CardTypeID"]);
                            cardType_obj.CardTypeCode = Convert.ToString(dt.Rows[i]["CardTypeCode"]);
                            tagTypeList.Add(cardType_obj);
                        }
                    }
                }
                return tagTypeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<Passes> GetListofActivePasses()
        {
            IList<Passes> passList = new List<Passes>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_GetActivePasses", sqlconn_obj))
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
                            Passes pass_obj = new Passes();
                            pass_obj.PassPriceID = Convert.ToInt32(dt.Rows[i]["PassPriceID"]);
                            pass_obj.PassCode = Convert.ToString(dt.Rows[i]["PassCode"]);
                            pass_obj.PassName = Convert.ToString(dt.Rows[i]["PassName"]);
                            passList.Add(pass_obj);
                        }
                    }
                }
                return passList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string SaveLocationPassMapper(List<int> passesList, string CreatedBy, int LocationID)
        {
            sqlhelper_obj = new SqlHelper();
            string mapper_data_status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    for (var i = 0; i < passesList.Count; i++)
                    {
                        using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SaveLocationPassAccess", sqlconn_obj))
                        {
                            sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                            sqlcmd_details_obj.CommandTimeout = 0;
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Insert");
                            sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", LocationID);
                            sqlcmd_details_obj.Parameters.AddWithValue("@PassID", Convert.ToInt32(passesList[i]));
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", true);
                            sqlcmd_details_obj.Parameters.AddWithValue("@CreatedBy", CreatedBy);
                            sqlconn_obj.Open();
                            int result = sqlcmd_details_obj.ExecuteNonQuery();
                            sqlconn_obj.Close();
                            if (result > 0)
                            {
                                mapper_data_status = "Success";
                            }
                            else
                            {
                                mapper_data_status = "Failed";
                            }
                        }
                    }
                }
                return mapper_data_status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<int> GetPassesByLocationID(int LocationID)
        {
            IList<int> passList = new List<int>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getvehicleType_obj = new SqlCommand("PARK_PROC_GetLocationPassMappersList", sqlconn_obj))
                    {
                        sqlcmd_getvehicleType_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getvehicleType_obj.CommandTimeout = 0;
                        sqlcmd_getvehicleType_obj.Parameters.AddWithValue("@LocationID", LocationID);
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getvehicleType_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            int passID = Convert.ToInt32(dt.Rows[i]["PassID"]);
                            passList.Add(passID);
                        }
                    }
                }
                return passList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string DeleteLocationPassMapper(int LocationID)
        {
            sqlhelper_obj = new SqlHelper();
            string mapper_data_status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_DeleteLocationPassMappersListByLocationID", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", LocationID);
                        sqlconn_obj.Open();
                        int result = sqlcmd_details_obj.ExecuteNonQuery();
                        sqlconn_obj.Close();
                        if (result > 0)
                        {
                            mapper_data_status = "Success";
                        }
                        else
                        {
                            mapper_data_status = "Failed";
                        }
                    }

                }
                return mapper_data_status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<Passes> GetListofActivePassesByVehicleID(string VehicleTypeIDs)
        {
            IList<Passes> passList = new List<Passes>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getvehicleType_obj = new SqlCommand("PARK_PROC_GetActivePassesByVehicleTypeID", sqlconn_obj))
                    {
                        sqlcmd_getvehicleType_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getvehicleType_obj.CommandTimeout = 0;
                        sqlcmd_getvehicleType_obj.Parameters.AddWithValue("@VehicleTypeID", VehicleTypeIDs);
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getvehicleType_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            Passes passobj = new Passes();
                            passobj.PassPriceID= Convert.ToInt32(dt.Rows[i]["PassPriceID"]);
                            passobj.PassName = Convert.ToString(dt.Rows[i]["PassName"]);
                            passList.Add(passobj);
                        }
                    }
                }
                return passList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
