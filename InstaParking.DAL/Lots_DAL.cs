using InstaParking.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace InstaParking.DAL
{
    public class Lots_DAL
    {
        SqlHelper sqlhelper_obj;
        GetLogo getlogo = new GetLogo();
        private static TimeZoneInfo India_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        #region Lot
        public IList<Lots> GetLotsList()
        {
            IList<Lots> lotsList = new List<Lots>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getlots_obj = new SqlCommand("PARK_PROC_GetAllLots", sqlconn_obj))
                    {
                        sqlcmd_getlots_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getlots_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getlots_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            Lots lots_obj = new Lots();
                            lots_obj.LocationParkingLotID = Convert.ToInt32(dt.Rows[i]["LocationParkingLotID"]);
                            lots_obj.LocationID = Convert.ToInt32(dt.Rows[i]["LocationID"]);
                            lots_obj.LocationName = Convert.ToString(dt.Rows[i]["LocationName"]);
                            lots_obj.ParkingTypeID = Convert.ToInt32(dt.Rows[i]["ParkingTypeID"]);
                            lots_obj.ParkingTypeName = Convert.ToString(dt.Rows[i]["ParkingTypeName"]);
                            lots_obj.LocationParkingLotCode = Convert.ToString(dt.Rows[i]["LocationParkingLotCode"]);
                            lots_obj.LocationParkingLotName = Convert.ToString(dt.Rows[i]["LocationParkingLotName"]);
                            //lots_obj.Lattitude = (float)Convert.ToDouble(dt.Rows[i]["Lattitude"]);
                            //lots_obj.Longitude = (float)Convert.ToDouble(dt.Rows[i]["Longitude"]);
                            lots_obj.Lattitude = Convert.ToDecimal(dt.Rows[i]["Lattitude"]);
                            lots_obj.Longitude = Convert.ToDecimal(dt.Rows[i]["Longitude"]);
                            lots_obj.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            lots_obj.UpdatedOn = dt.Rows[i]["UpdatedOn"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(dt.Rows[i]["UpdatedOn"]);
                            lots_obj.PhoneNumber = Convert.ToString(dt.Rows[i]["PhoneNumber"]);
                            lotsList.Add(lots_obj);
                        }
                    }
                }
                return lotsList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<ParkingType> GetActiveParkingTypesList()
        {
            IList<ParkingType> parkingTypeList = new List<ParkingType>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getparkingType_obj = new SqlCommand("PARK_PROC_GetAllActiveParkingTypes", sqlconn_obj))
                    {
                        sqlcmd_getparkingType_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getparkingType_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getparkingType_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            ParkingType parkingType_obj = new ParkingType();
                            parkingType_obj.ParkingTypeID = Convert.ToInt32(dt.Rows[i]["ParkingTypeID"]);
                            parkingType_obj.ParkingTypeName = Convert.ToString(dt.Rows[i]["ParkingTypeName"]);
                            parkingTypeList.Add(parkingType_obj);
                        }
                    }
                }
                return parkingTypeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<VehicleType> GetActiveVehicleTypesList()
        {
            IList<VehicleType> vehicleTypeList = new List<VehicleType>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getvehicleType_obj = new SqlCommand("PARK_PROC_GetAllActiveVehicleTypes", sqlconn_obj))
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
                            VehicleType vehicleType_obj = new VehicleType();
                            vehicleType_obj.VehicleTypeID = Convert.ToInt32(dt.Rows[i]["VehicleTypeID"]);
                            vehicleType_obj.VehicleTypeName = Convert.ToString(dt.Rows[i]["VehicleTypeName"]);
                            vehicleTypeList.Add(vehicleType_obj);
                        }
                    }
                }
                return vehicleTypeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<ParkingBay> GetActiveParkingBayList()
        {
            IList<ParkingBay> parkingBayList = new List<ParkingBay>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getparkingBay_obj = new SqlCommand("PARK_PROC_GetAllActiveParkingBay", sqlconn_obj))
                    {
                        sqlcmd_getparkingBay_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getparkingBay_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getparkingBay_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            ParkingBay parkingBay_obj = new ParkingBay();
                            parkingBay_obj.ParkingBayID = Convert.ToInt32(dt.Rows[i]["ParkingBayID"]);
                            parkingBay_obj.ParkingBayName = Convert.ToString(dt.Rows[i]["ParkingBayName"]);
                            parkingBayList.Add(parkingBay_obj);
                        }
                    }
                }
                return parkingBayList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public IList<LotVehicleAvailability> GetActiveLotVehicleAvailabilityList()
        //{
        //    IList<LotVehicleAvailability> list = new List<LotVehicleAvailability>();
        //    sqlhelper_obj = new SqlHelper();
        //    try
        //    {
        //        using (SqlConnection sqlconn_obj = new SqlConnection())
        //        {
        //            sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

        //            using (SqlCommand sqlcmd_getparkingBay_obj = new SqlCommand("PARK_PROC_GetLotVehicleAvailability", sqlconn_obj))
        //            {
        //                sqlcmd_getparkingBay_obj.CommandType = CommandType.StoredProcedure;
        //                sqlcmd_getparkingBay_obj.CommandTimeout = 0;
        //                DataSet ds;
        //                using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getparkingBay_obj))
        //                {
        //                    ds = new DataSet();
        //                    da.Fill(ds);
        //                }
        //                DataTable dt = ds.Tables[0];

        //                for (int i = 0; i < dt.Rows.Count; i++)
        //                {
        //                    LotVehicleAvailability obj = new LotVehicleAvailability();
        //                    obj.LotVehicleAvailabilityID = Convert.ToInt32(dt.Rows[i]["LotVehicleAvailabilityID"]);
        //                    obj.LotVehicleAvailabilityCode = Convert.ToString(dt.Rows[i]["LotVehicleAvailabilityCode"]);
        //                    obj.LotVehicleAvailabilityName = Convert.ToString(dt.Rows[i]["LotVehicleAvailabilityName"]);
        //                    list.Add(obj);
        //                }
        //            }
        //        }
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public int InsertAndUpdateLot(Lots lot_data, string CreatedBy, List<VehicleType> VehicleTypeList)
        {
            sqlhelper_obj = new SqlHelper();
            string lot_data_status = string.Empty;
            int result;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SaveLot", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        if (lot_data.LocationParkingLotID.ToString() != "" && lot_data.LocationParkingLotID != 0)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Update");
                            sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotID", lot_data.LocationParkingLotID);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Insert");
                            sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotID", DBNull.Value);
                        }

                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", lot_data.LocationID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@ParkingTypeID", lot_data.ParkingTypeID);
                        //sqlcmd_details_obj.Parameters.AddWithValue("@LotVehicleAvailabilityID", lot_data.LotVehicleAvailabilityID);
                        //sqlcmd_details_obj.Parameters.AddWithValue("@LotVehicleAvailabilityName", lot_data.LotVehicleAvailabilityName);
                        //sqlcmd_details_obj.Parameters.AddWithValue("@ParkingBayID", lot_data.ParkingBayID);

                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotCode", String.IsNullOrEmpty(lot_data.LocationParkingLotCode) ? (object)DBNull.Value : lot_data.LocationParkingLotCode.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotName", String.IsNullOrEmpty(lot_data.LocationParkingLotName) ? (object)DBNull.Value : lot_data.LocationParkingLotName.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@Lattitude", String.IsNullOrEmpty(lot_data.Lattitude.ToString()) ? (object)DBNull.Value : lot_data.Lattitude.ToString().Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@Longitude", String.IsNullOrEmpty(lot_data.Longitude.ToString()) ? (object)DBNull.Value : lot_data.Longitude.ToString().Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@UpdatedBy", CreatedBy);
                        if (lot_data.IsActive.ToString() == "")
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", true);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", Convert.ToBoolean(lot_data.IsActive));
                        }

                        if (lot_data.IsHoliday.ToString() == "")
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsHoliday", false);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsHoliday", Convert.ToBoolean(lot_data.IsHoliday));
                        }

                        sqlcmd_details_obj.Parameters.AddWithValue("@Address", String.IsNullOrEmpty(lot_data.Address) ? (object)DBNull.Value : lot_data.Address.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@PhoneNumber", String.IsNullOrEmpty(lot_data.PhoneNumber) ? (object)DBNull.Value : lot_data.PhoneNumber.Trim());
                        sqlcmd_details_obj.Parameters.Add("@Output_identity", SqlDbType.Int).Direction = ParameterDirection.Output;
                        sqlconn_obj.Open();
                        int res = sqlcmd_details_obj.ExecuteNonQuery();
                        result = Convert.ToInt32(sqlcmd_details_obj.Parameters["@Output_identity"].Value);

                        if (lot_data.LocationParkingLotID.ToString() != "" && lot_data.LocationParkingLotID != 0)
                        {
                            string updateLotVehTyperesult = UpdateLotVehicleTypeMapper(VehicleTypeList, CreatedBy, lot_data.LocationParkingLotID);
                            string parkingBayStatus = UpdateLotParkingBays(CreatedBy, lot_data.LocationParkingLotID);
                            string inactivepricestatus = InactiveLotPricesByVehicleType(lot_data.LocationParkingLotID, CreatedBy);
                            string updatepricestatus = UpdateLotPricesByVehicleType(lot_data.LocationParkingLotID, CreatedBy, VehicleTypeList);
                        }
                        else
                        {
                            string LotVehTyperesult = SaveLotVehicleTypeMapper(VehicleTypeList, CreatedBy, result);
                        }

                        sqlconn_obj.Close();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Violation of UNIQUE KEY constraint 'UC_LotName'. Cannot insert duplicate key in object 'dbo.LocationParkingLot'."))
                {
                    result = -2;
                    return result;
                }
                else
                {
                    throw ex;
                }
            }
        }
        public List<Lots> ViewLot(int LotID)
        {
            sqlhelper_obj = new SqlHelper();
            List<Lots> lots_List = new List<Lots>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_ViewLotDetails", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@LotID", LotID);
                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_lot = ds.Tables[0];
                        if (dt_lot.Rows.Count > 0)
                        {
                            DataRow row = dt_lot.Rows[0];
                            Lots lots_obj = new Lots();
                            lots_obj.LocationParkingLotID = Convert.ToInt32(row["LocationParkingLotID"]);
                            lots_obj.LocationID = Convert.ToInt32(row["LocationID"]);
                            lots_obj.ParkingTypeID = Convert.ToInt32(row["ParkingTypeID"]);
                            // lots_obj.VehicleTypeID = Convert.ToInt32(row["VehicleTypeID"]);
                            //lots_obj.ParkingBayID = Convert.ToInt32(row["ParkingBayID"]);
                            lots_obj.LocationParkingLotCode = Convert.ToString(row["LocationParkingLotCode"]);
                            lots_obj.LocationParkingLotName = Convert.ToString(row["LocationParkingLotName"]);
                            //lots_obj.Lattitude = (float)Convert.ToDouble(row["Lattitude"]);
                            //lots_obj.Longitude = (float)Convert.ToDouble(row["Longitude"]);
                            lots_obj.Lattitude = Convert.ToDecimal(row["Lattitude"]);
                            lots_obj.Longitude = Convert.ToDecimal(row["Longitude"]);
                            lots_obj.IsActive = Convert.ToBoolean(row["IsActive"]);
                            //lots_obj.LotVehicleAvailabilityID = Convert.ToInt32(row["LotVehicleAvailabilityID"]);
                            //lots_obj.LotVehicleAvailabilityName = Convert.ToString(row["LotVehicleAvailabilityName"]);
                            lots_obj.IsHoliday = Convert.ToBoolean(row["IsHoliday"]);

                            lots_obj.Address = Convert.ToString(row["Address"]);
                            lots_obj.PhoneNumber = Convert.ToString(row["PhoneNumber"]);


                            lots_obj.LotImageName = Convert.ToString(row["LotImageName"]);
                            lots_obj.LotImageName2 = Convert.ToString(row["LotImageName2"]);
                            lots_obj.LotImageName3 = Convert.ToString(row["LotImageName3"]);
                            //if (!String.IsNullOrEmpty(Convert.ToString(row["LotImage"])))
                            //{
                            //    lots_obj.LotImage = getlogo.ShowLotImage(Convert.ToString(row["LocationParkingLotID"]), "LotImage");
                            //}
                            //else
                            //{
                            //    lots_obj.LotImage = "";
                            //}

                            //if (!String.IsNullOrEmpty(Convert.ToString(row["LotImage2"])))
                            //{
                            //    lots_obj.LotImage2 = getlogo.ShowLotImage2(Convert.ToString(row["LocationParkingLotID"]), "LotImage2");
                            //}
                            //else
                            //{
                            //    lots_obj.LotImage2 = "";
                            //}

                            //if (!String.IsNullOrEmpty(Convert.ToString(row["LotImage3"])))
                            //{
                            //    lots_obj.LotImage3 = getlogo.ShowLotImage3(Convert.ToString(row["LocationParkingLotID"]), "LotImage3");
                            //}
                            //else
                            //{
                            //    lots_obj.LotImage3 = "";
                            //}

                            lots_List.Add(lots_obj);
                        }
                    }
                }
                return lots_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Lots> GetActiveLotssList(int LocationID)
        {
            sqlhelper_obj = new SqlHelper();
            List<Lots> lots_List = new List<Lots>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetActiveLotsByLocation", sqlconn_obj))
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
                        DataTable dt_lot = ds.Tables[0];
                        for (int i = 0; i < dt_lot.Rows.Count; i++)
                        {
                            Lots lots_obj = new Lots();
                            lots_obj.LocationParkingLotID = Convert.ToInt32(dt_lot.Rows[i]["LocationParkingLotID"]);
                            lots_obj.LocationParkingLotName = Convert.ToString(dt_lot.Rows[i]["LocationParkingLotName"]);
                            lots_List.Add(lots_obj);
                        }
                    }
                }
                return lots_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string InsertLotImageData(string lotName, string lotName2, string lotName3, Lots lots)
        {
            sqlhelper_obj = new SqlHelper();
            string message = string.Empty;

            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_InsertLotImage", sqlconn_obj))
                    {

                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.Parameters.AddWithValue("@LotID", lots.LocationParkingLotID);
                        //if (Convert.ToString(lotData) != "" && Convert.ToString(lotData) != null)
                        //{
                        //    sqlcmd_details_obj.Parameters.AddWithValue("@LotImage", lotData);
                        //}
                        //else
                        //{
                        //    //sqlcmd_details_obj.Parameters.AddWithValue("@LotImage", DBNull.Value);
                        //    sqlcmd_details_obj.Parameters.Add("@LotImage", SqlDbType.VarBinary).Value =  System.Data.SqlTypes.SqlBinary.Null;
                        //}
                        sqlcmd_details_obj.Parameters.AddWithValue("@LotImageName", String.IsNullOrEmpty(lotName) ? (object)DBNull.Value : lotName.Trim());
                        //sqlcmd_details_obj.Parameters.AddWithValue("@LotImageType", String.IsNullOrEmpty(lotType) ? (object)DBNull.Value : lotType.Trim());

                        //if (Convert.ToString(lotData2) != "" && Convert.ToString(lotData2) != null)
                        //{
                        //    sqlcmd_details_obj.Parameters.AddWithValue("@LotImage2", lotData2);
                        //}
                        //else
                        //{
                        //    // sqlcmd_details_obj.Parameters.AddWithValue("@LotImage2", DBNull.Value);
                        //    sqlcmd_details_obj.Parameters.Add("@LotImage2", SqlDbType.VarBinary).Value = System.Data.SqlTypes.SqlBinary.Null;
                        //}
                        sqlcmd_details_obj.Parameters.AddWithValue("@LotImageName2", String.IsNullOrEmpty(lotName2) ? (object)DBNull.Value : lotName2.Trim());
                        //sqlcmd_details_obj.Parameters.AddWithValue("@LotImageType2", String.IsNullOrEmpty(lotType2) ? (object)DBNull.Value : lotType2.Trim());

                        //if (Convert.ToString(lotData3) != "" && Convert.ToString(lotData3) != null)
                        //{
                        //    sqlcmd_details_obj.Parameters.AddWithValue("@LotImage3", lotData3);
                        //}
                        //else
                        //{
                        //    // sqlcmd_details_obj.Parameters.AddWithValue("@LotImage3", DBNull.Value);
                        //    sqlcmd_details_obj.Parameters.Add("@LotImage3", SqlDbType.VarBinary).Value = System.Data.SqlTypes.SqlBinary.Null;
                        //}
                        sqlcmd_details_obj.Parameters.AddWithValue("@LotImageName3", String.IsNullOrEmpty(lotName3) ? (object)DBNull.Value : lotName3.Trim());
                        // sqlcmd_details_obj.Parameters.AddWithValue("@LotImageType3", String.IsNullOrEmpty(lotType3) ? (object)DBNull.Value : lotType3.Trim());


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

        public Stream ShowLotImg(int lotID, string name)
        {
            try
            {
                sqlhelper_obj = new SqlHelper();
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_getlogo = new SqlCommand("PARK_PROC_GetLotImage", sqlconn_obj))
                    {
                        sqlconn_obj.Open();
                        sqlcmd_getlogo.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getlogo.Parameters.AddWithValue("@LotID", lotID);
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

        public string VerifyLotCode(Lots LotsData)
        {
            sqlhelper_obj = new SqlHelper();
            string lot_data_status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_VerifyLotCode", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", LotsData.LocationID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LotCode", String.IsNullOrEmpty(LotsData.LocationParkingLotCode) ? (object)DBNull.Value : LotsData.LocationParkingLotCode.Trim());
                        sqlconn_obj.Open();
                        int result = Convert.ToInt32(sqlcmd_details_obj.ExecuteScalar());
                        sqlconn_obj.Close();
                        if (result == 0)
                        {
                            lot_data_status = "Not Exists";
                        }
                        else
                        {
                            lot_data_status = "Exists";
                        }
                    }
                }
                return lot_data_status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string CheckLocationStatus(int LocationID)
        {
            sqlhelper_obj = new SqlHelper();
            string location_data_status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_CheckLocationStatus", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", LocationID);
                        sqlconn_obj.Open();
                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_lot = ds.Tables[0];
                        sqlconn_obj.Close();
                        if (dt_lot.Rows.Count > 0)
                        {
                            location_data_status = "Success";
                        }
                        else
                        {
                            location_data_status = "Failed";
                        }
                    }
                }
                return location_data_status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Lot Timing
        public int SaveLotTimings(ParkingLotTiming lot_data, string CreatedBy)
        {
            //DateTime dateTime_Indian = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, India_Standard_Time);
            //DateTime dateTime_Indian = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(lot_data.LotOpenTime), India_Standard_Time);
            //string lotopentime = dateTime_Indian.ToString("hh:mm tt");

            //  DateTime dateTime_Indian1 = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(lot_data.LotCloseTime), India_Standard_Time);
            //  string lotclosetime = dateTime_Indian1.ToString("hh:mm tt");

            //DateTime opentime = Convert.ToDateTime(lot_data.LotOpenTime);
            //string lotopentime = opentime.ToString("hh:mm tt");
            //DateTime closetime = Convert.ToDateTime(lot_data.LotCloseTime);
            //string lotclosetime = closetime.ToString("hh:mm tt");

            DateTime ss = DateTime.UtcNow;

            sqlhelper_obj = new SqlHelper();
            string resultmsg = string.Empty;
            int result;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SaveLotTimings", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        if (lot_data.ParkingLotTimingID.ToString() != "" && lot_data.ParkingLotTimingID != 0)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Update");
                            sqlcmd_details_obj.Parameters.AddWithValue("@ParkingLotTimingID", lot_data.ParkingLotTimingID);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Insert");
                            sqlcmd_details_obj.Parameters.AddWithValue("@ParkingLotTimingID", DBNull.Value);
                        }

                        sqlcmd_details_obj.Parameters.AddWithValue("@LotID", lot_data.LotID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@DayOfWeek", String.IsNullOrEmpty(lot_data.DayOfWeek) ? (object)DBNull.Value : lot_data.DayOfWeek.Trim());
                        //sqlcmd_details_obj.Parameters.AddWithValue("@LotOpenTime", lotopentime);
                        //sqlcmd_details_obj.Parameters.AddWithValue("@LotCloseTime", lotclosetime);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LotOpenTime", lot_data.LotOpenTime);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LotCloseTime", lot_data.LotCloseTime);
                        //sqlcmd_details_obj.Parameters.AddWithValue("@LotOpenTime", String.IsNullOrEmpty(lot_data.LotOpenTime) ? (object)DBNull.Value : lot_data.LotOpenTime.Trim());
                        //sqlcmd_details_obj.Parameters.AddWithValue("@LotCloseTime", String.IsNullOrEmpty(lot_data.LotCloseTime.ToString()) ? (object)DBNull.Value : lot_data.LotCloseTime.ToString().Trim());

                        if (lot_data.IsActive.ToString() == "")
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", true);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", Convert.ToBoolean(lot_data.IsActive));
                        }

                        sqlcmd_details_obj.Parameters.AddWithValue("@CreatedBy", CreatedBy);
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
        public IList<ParkingLotTiming> GetLotTimings(int LotID)
        {
            IList<ParkingLotTiming> lottimingsList = new List<ParkingLotTiming>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getlots_obj = new SqlCommand("PARK_PROC_GetLotTimingsByID", sqlconn_obj))
                    {
                        sqlcmd_getlots_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getlots_obj.CommandTimeout = 0;
                        sqlcmd_getlots_obj.Parameters.AddWithValue("@LotID", LotID);
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getlots_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            ParkingLotTiming lottimings_obj = new ParkingLotTiming();
                            lottimings_obj.ParkingLotTimingID = Convert.ToInt32(dt.Rows[i]["ParkingLotTimingID"]);
                            lottimings_obj.LotID = Convert.ToInt32(dt.Rows[i]["LotID"]);
                            lottimings_obj.DayOfWeek = Convert.ToString(dt.Rows[i]["DayOfWeek"]);
                            lottimings_obj.LotOpenTime = Convert.ToString(dt.Rows[i]["LotOpenTime"]);
                            lottimings_obj.LotCloseTime = Convert.ToString(dt.Rows[i]["LotCloseTime"]);
                            //lottimings_obj.LotOpenTime = Convert.ToString(dt.Rows[i]["LotOpenTime"]);
                            //lottimings_obj.LotCloseTime = Convert.ToString(dt.Rows[i]["LotCloseTime"]);
                            lottimings_obj.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            lottimings_obj.UpdatedOn = dt.Rows[i]["UpdatedOn"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(dt.Rows[i]["UpdatedOn"]);
                            lottimingsList.Add(lottimings_obj);
                        }
                    }
                }
                return lottimingsList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ParkingLotTiming> GetLotTimingByID(int ParkingLotTimingID)
        {
            sqlhelper_obj = new SqlHelper();
            List<ParkingLotTiming> lotTiming_List = new List<ParkingLotTiming>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_ViewLotTimingDetails", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@ParkingLotTimingID", ParkingLotTimingID);
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
                            ParkingLotTiming lotTiming_obj = new ParkingLotTiming();
                            lotTiming_obj.ParkingLotTimingID = Convert.ToInt32(row["ParkingLotTimingID"]);
                            lotTiming_obj.LotID = Convert.ToInt32(row["LotID"]);
                            lotTiming_obj.DayOfWeek = Convert.ToString(row["DayOfWeek"]);
                            lotTiming_obj.LotOpenTime = Convert.ToString(row["LotOpenTime"]);
                            lotTiming_obj.LotCloseTime = Convert.ToString(row["LotCloseTime"]);
                            //lotTiming_obj.LotOpenTime = Convert.ToString(row["LotOpenTime"]);
                            //lotTiming_obj.LotCloseTime = Convert.ToString(row["LotCloseTime"]);
                            lotTiming_obj.IsActive = Convert.ToBoolean(row["IsActive"]);
                            lotTiming_List.Add(lotTiming_obj);
                        }
                    }
                }
                return lotTiming_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string DeleteLotTime(int ParkingLotTimingID, string DeletedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string data_status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_DeleteLotTime", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@ParkingLotTimingID", ParkingLotTimingID);
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
        #endregion

        #region Lot Price
        public IList<ApplicationType> GetApplicationTypesList()
        {
            IList<ApplicationType> appTypeList = new List<ApplicationType>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getappType_obj = new SqlCommand("PARK_PROC_GetAllApplicationTypes", sqlconn_obj))
                    {
                        sqlcmd_getappType_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getappType_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getappType_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            ApplicationType appType_obj = new ApplicationType();
                            appType_obj.ApplicationTypeID = Convert.ToInt32(dt.Rows[i]["ApplicationTypeID"]);
                            appType_obj.ApplicationTypeName = Convert.ToString(dt.Rows[i]["ApplicationTypeName"]);
                            appTypeList.Add(appType_obj);
                        }
                    }
                }
                return appTypeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string SaveLotPrice(LotPrice lotprice_data, string CreatedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string resultmsg = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SaveLotPrice", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        if (lotprice_data.PriceID.ToString() != "" && lotprice_data.PriceID != 0)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Update");
                            sqlcmd_details_obj.Parameters.AddWithValue("@PriceID", lotprice_data.PriceID);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Insert");
                            sqlcmd_details_obj.Parameters.AddWithValue("@PriceID", DBNull.Value);
                        }
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotID", Convert.ToInt32(lotprice_data.LocationParkingLotID));
                        sqlcmd_details_obj.Parameters.AddWithValue("@ApplicationTypeID", Convert.ToInt32(lotprice_data.ApplicationTypeID));
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", Convert.ToInt32(lotprice_data.VehicleTypeID));
                        sqlcmd_details_obj.Parameters.AddWithValue("@Price", Convert.ToDecimal(lotprice_data.Price));
                        sqlcmd_details_obj.Parameters.AddWithValue("@Duration", String.IsNullOrEmpty(Convert.ToString(lotprice_data.Duration)) ? (object)DBNull.Value : Convert.ToInt32(lotprice_data.Duration));
                        sqlcmd_details_obj.Parameters.AddWithValue("@CreatedBy", CreatedBy);
                        if (lotprice_data.IsActive.ToString() == "")
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", true);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", Convert.ToBoolean(lotprice_data.IsActive));
                        }
                        sqlcmd_details_obj.Parameters.Add("@Output_identity", SqlDbType.Int).Direction = ParameterDirection.Output;
                        sqlconn_obj.Open();
                        int res = sqlcmd_details_obj.ExecuteNonQuery();
                        int result = Convert.ToInt32(sqlcmd_details_obj.Parameters["@Output_identity"].Value);
                        sqlconn_obj.Close();
                        if (result > 0)
                        {
                            resultmsg = "Success";
                        }
                        else if (result == -1)
                        {
                            resultmsg = "Data Exists";
                        }
                        else
                        {
                            resultmsg = "Failed";
                        }
                    }
                }
                return resultmsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<LotPrice> GetLotPrices(int LotID)
        {
            IList<LotPrice> lotpriceList = new List<LotPrice>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getlots_obj = new SqlCommand("PARK_PROC_GetLotPricesByLotID", sqlconn_obj))
                    {
                        sqlcmd_getlots_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getlots_obj.CommandTimeout = 0;
                        sqlcmd_getlots_obj.Parameters.AddWithValue("@LotID", LotID);
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getlots_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            LotPrice lotprice_obj = new LotPrice();
                            lotprice_obj.PriceID = Convert.ToInt32(dt.Rows[i]["PriceID"]);
                            lotprice_obj.LocationParkingLotID = Convert.ToInt32(dt.Rows[i]["LocationParkingLotID"]);
                            lotprice_obj.VehicleTypeID = Convert.ToInt32(dt.Rows[i]["VehicleTypeID"]);
                            lotprice_obj.VehicleTypeName = Convert.ToString(dt.Rows[i]["VehicleTypeName"]);
                            lotprice_obj.ApplicationTypeID = Convert.ToInt32(dt.Rows[i]["ApplicationTypeID"]);
                            lotprice_obj.ApplicationTypeName = Convert.ToString(dt.Rows[i]["ApplicationTypeName"]);
                            lotprice_obj.Price = Convert.ToDecimal(dt.Rows[i]["Price"]);
                            lotprice_obj.Duration = Convert.ToInt32(dt.Rows[i]["Duration"]);
                            lotprice_obj.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            lotprice_obj.UpdatedOn = dt.Rows[i]["UpdatedOn"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(dt.Rows[i]["UpdatedOn"]);
                            lotpriceList.Add(lotprice_obj);
                        }
                    }
                }
                return lotpriceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<LotPrice> GetLotPriceByID(int priceID)
        {
            sqlhelper_obj = new SqlHelper();
            List<LotPrice> price_List = new List<LotPrice>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_ViewLotPriceDetails", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@PriceID", priceID);
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
                            LotPrice lotprice_obj = new LotPrice();
                            lotprice_obj.PriceID = Convert.ToInt32(row["PriceID"]);
                            lotprice_obj.LocationParkingLotID = Convert.ToInt32(row["LocationParkingLotID"]);
                            lotprice_obj.VehicleTypeID = Convert.ToInt32(row["VehicleTypeID"]);
                            lotprice_obj.ApplicationTypeID = Convert.ToInt32(row["ApplicationTypeID"]);
                            lotprice_obj.Price = Convert.ToDecimal(row["Price"]);
                            lotprice_obj.Duration = Convert.ToInt32(row["Duration"]);
                            lotprice_obj.IsActive = Convert.ToBoolean(row["IsActive"]);
                            price_List.Add(lotprice_obj);
                        }
                    }
                }
                return price_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string DeleteLotPrice(int PriceID, string DeletedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string data_status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_DeleteLotPrice", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@PriceID", PriceID);
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
        #endregion

        #region Lot Services
        public string SaveLotServices(List<ServiceType> list, string CreatedBy, int lotid)
        {
            sqlhelper_obj = new SqlHelper();
            string resultmsg = "";
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    for (var i = 0; i < list.Count; i++)
                    {
                        using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_SaveLotServices", sqlconn_obj))
                        {
                            sqlcmd_obj.CommandType = CommandType.StoredProcedure;
                            sqlcmd_obj.CommandTimeout = 0;
                            sqlcmd_obj.Parameters.AddWithValue("@LocationParkingLotID", lotid);
                            sqlcmd_obj.Parameters.AddWithValue("@ServiceTypeID", list[i].ServiceTypeID);
                            sqlcmd_obj.Parameters.AddWithValue("@IsActive", list[i].selected);
                            sqlcmd_obj.Parameters.AddWithValue("@CreatedBy", CreatedBy);
                            sqlconn_obj.Open();
                            int res = sqlcmd_obj.ExecuteNonQuery();
                            sqlconn_obj.Close();
                            if (res > 0)
                            {
                                resultmsg = "Success";
                            }
                        }
                    }
                }
                return resultmsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<ServiceType> GetSelectedLotServiceTypesList(int LotID)
        {
            IList<ServiceType> featuresList = new List<ServiceType>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_GetSelectedLotServiceTypes", sqlconn_obj))
                    {
                        sqlcmd_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_obj.CommandTimeout = 0;
                        sqlcmd_obj.Parameters.AddWithValue("@LotID", LotID);
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            ServiceType feature_obj = new ServiceType();
                            feature_obj.ServiceTypeID = Convert.ToInt32(dt.Rows[i]["ServiceTypeID"]);
                            feature_obj.ServiceTypeName = Convert.ToString(dt.Rows[i]["ServiceTypeName"]);
                            feature_obj.selected = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            featuresList.Add(feature_obj);
                        }
                    }
                }
                return featuresList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Lot Bays
        public string SaveLotBays(ParkingBay lotbay_data, string CreatedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string resultmsg = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SaveLotBays", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        if (lotbay_data.ParkingBayID.ToString() != "" && lotbay_data.ParkingBayID != 0)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Update");
                            sqlcmd_details_obj.Parameters.AddWithValue("@ParkingBayID", lotbay_data.ParkingBayID);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Insert");
                            sqlcmd_details_obj.Parameters.AddWithValue("@ParkingBayID", DBNull.Value);
                        }

                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotID", Convert.ToInt32(lotbay_data.LocationParkingLotID));
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", Convert.ToInt32(lotbay_data.VehicleTypeID));
                        sqlcmd_details_obj.Parameters.AddWithValue("@NumberOfBays", Convert.ToInt32(lotbay_data.NumberOfBays));
                        sqlcmd_details_obj.Parameters.AddWithValue("@ParkingBayRange", String.IsNullOrEmpty(lotbay_data.ParkingBayRange) ? (object)DBNull.Value : lotbay_data.ParkingBayRange.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@CreatedBy", CreatedBy);
                        if (lotbay_data.IsActive.ToString() == "")
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", true);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", Convert.ToBoolean(lotbay_data.IsActive));
                        }
                        sqlconn_obj.Open();
                        int res = sqlcmd_details_obj.ExecuteNonQuery();
                        sqlconn_obj.Close();
                        if (res > 0)
                        {
                            resultmsg = "Success";
                        }
                    }
                }
                return resultmsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<ParkingBay> GetLotBays(int LotID)
        {
            IList<ParkingBay> lotBaysList = new List<ParkingBay>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getlots_obj = new SqlCommand("PARK_PROC_GetLotParkingBaysByLotID", sqlconn_obj))
                    {
                        sqlcmd_getlots_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getlots_obj.CommandTimeout = 0;
                        sqlcmd_getlots_obj.Parameters.AddWithValue("@LotID", LotID);
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getlots_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            ParkingBay lotbays_obj = new ParkingBay();
                            lotbays_obj.ParkingBayID = Convert.ToInt32(dt.Rows[i]["ParkingBayID"]);
                            lotbays_obj.LocationParkingLotID = Convert.ToInt32(dt.Rows[i]["LocationParkingLotID"]);
                            lotbays_obj.VehicleTypeID = Convert.ToInt32(dt.Rows[i]["VehicleTypeID"]);
                            lotbays_obj.VehicleTypeName = Convert.ToString(dt.Rows[i]["VehicleTypeName"]);
                            lotbays_obj.NumberOfBays = Convert.ToInt32(dt.Rows[i]["NumberOfBays"]);
                            lotbays_obj.ParkingBayRange = Convert.ToString(dt.Rows[i]["ParkingBayRange"]);
                            lotbays_obj.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            lotbays_obj.UpdatedOn = dt.Rows[i]["UpdatedOn"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(dt.Rows[i]["UpdatedOn"]);
                            lotBaysList.Add(lotbays_obj);
                        }
                    }
                }
                return lotBaysList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ParkingBay> GetLotBayByID(int ParkingBayID)
        {
            sqlhelper_obj = new SqlHelper();
            List<ParkingBay> lotBays_List = new List<ParkingBay>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_ViewLotBaysDetails", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@ParkingBayID", ParkingBayID);
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
                            ParkingBay lotBay_obj = new ParkingBay();
                            lotBay_obj.ParkingBayID = Convert.ToInt32(row["ParkingBayID"]);
                            lotBay_obj.LocationParkingLotID = Convert.ToInt32(row["LocationParkingLotID"]);
                            lotBay_obj.VehicleTypeID = Convert.ToInt32(row["VehicleTypeID"]);
                            lotBay_obj.NumberOfBays = Convert.ToInt32(row["NumberOfBays"]);
                            lotBay_obj.ParkingBayRange = Convert.ToString(row["ParkingBayRange"]);
                            lotBay_obj.IsActive = Convert.ToBoolean(row["IsActive"]);
                            lotBays_List.Add(lotBay_obj);
                        }
                    }
                }
                return lotBays_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string DeleteLotBay(int ParkingBayID, string DeletedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string data_status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_DeleteLotBay", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@ParkingBayID", ParkingBayID);
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

        public string GetLotCapacityByVehicleType(int LotID)
        {
            sqlhelper_obj = new SqlHelper();
            string data = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetBayCapacityofLotByVehicleType", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotID", LotID);
                        sqlconn_obj.Open();
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        data = Convert.ToString(ds.Tables[0].Rows[0]["TwoWheelerCapacity"]) + "-" + Convert.ToString(ds.Tables[1].Rows[0]["FourWheelerCapacity"]);
                        sqlconn_obj.Close();
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Charges
        public IList<Charges> GetChargesData(int VehicleTypeID)
        {
            IList<Charges> chargesList = new List<Charges>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_GetCharges", sqlconn_obj))
                    {
                        sqlcmd_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_obj.CommandTimeout = 0;
                        sqlcmd_obj.Parameters.AddWithValue("@VehicleTypeID", VehicleTypeID);
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            Charges charges_obj = new Charges();
                            charges_obj.ChargesID = Convert.ToInt32(dt.Rows[i]["ChargesID"]);
                            charges_obj.ClampFee = dt.Rows[i]["ClampFee"] == DBNull.Value ? 0 : Convert.ToDecimal(dt.Rows[i]["ClampFee"]);
                            charges_obj.NFCTagPrice = dt.Rows[i]["NFCTagPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(dt.Rows[i]["NFCTagPrice"]);
                            charges_obj.BlueToothTagPrice = dt.Rows[i]["BlueToothTagPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(dt.Rows[i]["BlueToothTagPrice"]);
                            charges_obj.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            charges_obj.UpdatedOn = dt.Rows[i]["UpdatedOn"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(dt.Rows[i]["UpdatedOn"]);
                            chargesList.Add(charges_obj);
                        }

                        if(dt.Rows.Count==0)
                        {
                            Charges charges_obj = new Charges();
                            charges_obj.ChargesID = 0;
                            charges_obj.ClampFee = 0;
                            charges_obj.NFCTagPrice = 0;
                            charges_obj.BlueToothTagPrice = 0;
                            charges_obj.IsActive = false;                            
                            chargesList.Add(charges_obj);
                        }
                    }
                }
                return chargesList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string SaveCharges(Charges charges_data, string CreatedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string charges_data_status = string.Empty;
            int result;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SaveCharges", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;

                        if (charges_data.ChargesID.ToString() != "" && charges_data.ChargesID != 0)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Update");
                            sqlcmd_details_obj.Parameters.AddWithValue("@ChargesID", charges_data.ChargesID);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Insert");
                            sqlcmd_details_obj.Parameters.AddWithValue("@ChargesID", DBNull.Value);
                        }
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", charges_data.VehicleTypeID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@ClampFee", String.IsNullOrEmpty(charges_data.ClampFee.ToString()) ? (object)DBNull.Value : charges_data.ClampFee.ToString().Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@NFCTagPrice", String.IsNullOrEmpty(charges_data.NFCTagPrice.ToString()) ? (object)DBNull.Value : charges_data.NFCTagPrice.ToString().Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@BlueToothTagPrice", String.IsNullOrEmpty(charges_data.BlueToothTagPrice.ToString()) ? (object)DBNull.Value : charges_data.BlueToothTagPrice.ToString().Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@CreatedBy", Convert.ToInt32(CreatedBy));
                        sqlconn_obj.Open();
                        int res = sqlcmd_details_obj.ExecuteNonQuery();
                        sqlconn_obj.Close();
                        if (res > 0)
                        {
                            charges_data_status = "Success";
                        }
                        else if (res == 0)
                        {
                            charges_data_status = "You are trying to save the wrong data.";
                        }
                        else
                        {
                            charges_data_status = "Failed";
                        }
                    }
                }
                return charges_data_status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<Charges> GetListofChargesData()
        {
            IList<Charges> chargesList = new List<Charges>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_GetListofChargesData", sqlconn_obj))
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
                            Charges charges_obj = new Charges();
                            charges_obj.ChargesID = Convert.ToInt32(dt.Rows[i]["ChargesID"]);
                            charges_obj.VehicleTypeID= Convert.ToInt32(dt.Rows[i]["VehicleTypeID"]);
                            charges_obj.VehicleTypeCode = Convert.ToString(dt.Rows[i]["VehicleTypeCode"]);
                            charges_obj.ClampFee = dt.Rows[i]["ClampFee"] == DBNull.Value ? 0 : Convert.ToDecimal(dt.Rows[i]["ClampFee"]);
                            charges_obj.NFCTagPrice = dt.Rows[i]["NFCTagPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(dt.Rows[i]["NFCTagPrice"]);
                            charges_obj.BlueToothTagPrice = dt.Rows[i]["BlueToothTagPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(dt.Rows[i]["BlueToothTagPrice"]);
                            chargesList.Add(charges_obj);
                        }
                    }
                }
                return chargesList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<Charges> VieworEditChargesData(int ChargesID)
        {
            IList<Charges> chargesList = new List<Charges>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_VieworEditChargesData", sqlconn_obj))
                    {
                        sqlcmd_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_obj.CommandTimeout = 0;
                        sqlcmd_obj.Parameters.AddWithValue("@ChargesID", ChargesID);
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            Charges charges_obj = new Charges();
                            charges_obj.ChargesID = Convert.ToInt32(dt.Rows[i]["ChargesID"]);
                            charges_obj.VehicleTypeID = Convert.ToInt32(dt.Rows[i]["VehicleTypeID"]);
                            charges_obj.ClampFee = dt.Rows[i]["ClampFee"] == DBNull.Value ? 0 : Convert.ToDecimal(dt.Rows[i]["ClampFee"]);
                            charges_obj.NFCTagPrice = dt.Rows[i]["NFCTagPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(dt.Rows[i]["NFCTagPrice"]);
                            charges_obj.BlueToothTagPrice = dt.Rows[i]["BlueToothTagPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(dt.Rows[i]["BlueToothTagPrice"]);
                            chargesList.Add(charges_obj);
                        }
                    }
                }
                return chargesList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        //26022021 Start
        public string SaveLotVehicleTypeMapper(List<VehicleType> VehicleTypeList, string CreatedBy, int LotID)
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
                        using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SaveLotVehicleTypeMapper", sqlconn_obj))
                        {
                            sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                            sqlcmd_details_obj.CommandTimeout = 0;
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Insert");
                            sqlcmd_details_obj.Parameters.AddWithValue("@LotID", LotID);
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
        public string UpdateLotVehicleTypeMapper(List<VehicleType> VehicleTypeList, string CreatedBy, int LotID)
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
                        using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SaveLotVehicleTypeMapper", sqlconn_obj))
                        {
                            sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                            sqlcmd_details_obj.CommandTimeout = 0;
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Update");
                            sqlcmd_details_obj.Parameters.AddWithValue("@LotID", LotID);
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
        public string UpdateLotParkingBays(string CreatedBy, int LotID)
        {
            sqlhelper_obj = new SqlHelper();
            string status = string.Empty;

            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_UpdateParkingBayByLotID", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@LotID", LotID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@CreatedBy", CreatedBy);
                        sqlconn_obj.Open();
                        int result = sqlcmd_details_obj.ExecuteNonQuery();
                        sqlconn_obj.Close();
                        if (result > 0)
                        {
                            status = "Success";
                        }
                        else
                        {
                            status = "Failed";
                        }
                    }
                }
                return status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string CheckVehicleTypeExistforLot(int LotID, int VehicleTypeID)
        {
            sqlhelper_obj = new SqlHelper();
            string lot_data_status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_CheckVehicleTypeExistforLot", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@LotID", LotID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", VehicleTypeID);
                        sqlconn_obj.Open();
                        DataTable dt;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            dt = new DataTable();
                            da.Fill(dt);
                        }
                        sqlconn_obj.Close();
                        if (dt.Rows.Count > 0)
                        {
                            lot_data_status = "VehicleType Exists";
                        }
                        else
                        {
                            lot_data_status = "Not Exists";
                        }
                    }
                }
                return lot_data_status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string InactiveLotPricesByVehicleType(int LotID, string CreatedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string status = string.Empty;

            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_InactiveLotPricesByVehicleType", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@LotID", LotID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@CreatedBy", CreatedBy);
                        sqlconn_obj.Open();
                        int result = sqlcmd_details_obj.ExecuteNonQuery();
                        sqlconn_obj.Close();
                        if (result > 0)
                        {
                            status = "Success";
                        }
                        else
                        {
                            status = "Failed";
                        }
                    }
                }
                return status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string UpdateLotPricesByVehicleType(int LotID, string CreatedBy, List<VehicleType> VehicleTypeList)
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
                        if (VehicleTypeList[i].selected == true)
                        {
                            using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_UpdateLotPricesByVehicleType", sqlconn_obj))
                            {
                                sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                                sqlcmd_details_obj.CommandTimeout = 0;
                                sqlcmd_details_obj.Parameters.AddWithValue("@LotID", LotID);
                                sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", VehicleTypeList[i].VehicleTypeID);
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

                }
                return mapper_data_status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //26022021 End
    }
}
