using InstaParking.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace InstaParking.DAL
{
    public class RevenueReports_DAL
    {
        SqlHelper sqlhelper_obj;
        public IList<ApplicationType> GetActiveChannelsList()
        {
            IList<ApplicationType> applicationTypeList = new List<ApplicationType>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_GetActiveApplicationTypes", sqlconn_obj))
                    {
                        sqlcmd_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_obj.CommandTimeout = 0;
                        DataTable dt;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_obj))
                        {
                            dt = new DataTable();
                            da.Fill(dt);
                        }

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            ApplicationType applicationType_obj = new ApplicationType();
                            applicationType_obj.ApplicationTypeID = Convert.ToInt32(dt.Rows[i]["ApplicationTypeID"]);
                            applicationType_obj.ApplicationTypeName = Convert.ToString(dt.Rows[i]["ApplicationTypeName"]);
                            applicationTypeList.Add(applicationType_obj);
                        }
                    }
                }
                return applicationTypeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<ApplicationType> GetChannelListforPasses()
        {
            IList<ApplicationType> applicationTypeList = new List<ApplicationType>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_GetActiveApplicationTypesForPasses", sqlconn_obj))
                    {
                        sqlcmd_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_obj.CommandTimeout = 0;
                        DataTable dt;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_obj))
                        {
                            dt = new DataTable();
                            da.Fill(dt);
                        }

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            ApplicationType applicationType_obj = new ApplicationType();
                            applicationType_obj.ApplicationTypeID = Convert.ToInt32(dt.Rows[i]["ApplicationTypeID"]);
                            applicationType_obj.ApplicationTypeName = Convert.ToString(dt.Rows[i]["ApplicationTypeName"]);
                            applicationTypeList.Add(applicationType_obj);
                        }
                    }
                }
                return applicationTypeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<ApplicationType> GetChannelListforPaymentType()
        {
            IList<ApplicationType> applicationTypeList = new List<ApplicationType>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_GetActiveApplicationTypesForPaymentType", sqlconn_obj))
                    {
                        sqlcmd_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_obj.CommandTimeout = 0;
                        DataTable dt;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_obj))
                        {
                            dt = new DataTable();
                            da.Fill(dt);
                        }

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            ApplicationType applicationType_obj = new ApplicationType();
                            applicationType_obj.ApplicationTypeID = Convert.ToInt32(dt.Rows[i]["ApplicationTypeID"]);
                            applicationType_obj.ApplicationTypeName = Convert.ToString(dt.Rows[i]["ApplicationTypeName"]);
                            applicationTypeList.Add(applicationType_obj);
                        }
                    }
                }
                return applicationTypeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<ViolationReason> GetActiveReasonsList()
        {
            IList<ViolationReason> reasonList = new List<ViolationReason>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_GetActiveReasonsList", sqlconn_obj))
                    {
                        sqlcmd_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_obj.CommandTimeout = 0;
                        DataTable dt;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_obj))
                        {
                            dt = new DataTable();
                            da.Fill(dt);
                        }

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            ViolationReason reason_obj = new ViolationReason();
                            //reason_obj.ViolationReasonID = Convert.ToInt32(dt.Rows[i]["ViolationReasonID"]);
                            reason_obj.Reason = Convert.ToString(dt.Rows[i]["Reason"]);
                            reasonList.Add(reason_obj);
                        }
                    }
                }
                return reasonList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<RevenueByStation> GetReportByStation(SearchFilters stationFilterData)
        {
            sqlhelper_obj = new SqlHelper();
            List<RevenueByStation> revenue_List = new List<RevenueByStation>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetReportByStation", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@Company", stationFilterData.Company);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", stationFilterData.LocationID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotID", stationFilterData.LocationParkingLotID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@ApplicationTypeID", stationFilterData.ApplicationTypeID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", stationFilterData.VehicleTypeID);
                        if (stationFilterData.Duration != "0" && stationFilterData.Duration != null)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", stationFilterData.Duration);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", DBNull.Value);
                        }
                        if (String.IsNullOrEmpty(stationFilterData.FromDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", stationFilterData.FromDate);
                        }
                        if (String.IsNullOrEmpty(stationFilterData.ToDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", stationFilterData.ToDate);
                        }
                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_location = ds.Tables[0];
                        if (dt_location.Rows.Count == 0)
                        {
                            //RevenueByStation station_obj = new RevenueByStation();
                            //station_obj.Station = "Testing Data";
                            //revenue_List.Add(station_obj);

                            SqlCommand sqlcmdException = new SqlCommand("PARK_PROC_SaveLog", sqlconn_obj);
                            sqlcmdException.CommandType = CommandType.StoredProcedure;
                            sqlcmdException.CommandTimeout = 0;
                            sqlcmdException.Parameters.AddWithValue("@ApplicationType", "Parking");
                            sqlcmdException.Parameters.AddWithValue("@ExceptionMessage", string.Concat(stationFilterData.LocationID, ",", stationFilterData.LocationParkingLotID, ",", stationFilterData.FromDate, ",", stationFilterData.ToDate));
                            sqlcmdException.Parameters.AddWithValue("@Model", "ReportModel");
                            sqlcmdException.Parameters.AddWithValue("@Procedure", "PARK_PROC_GetReportByStation");
                            sqlcmdException.Parameters.AddWithValue("@ApplicationMethod", "GetReportByStation");
                            sqlconn_obj.Open();
                            sqlcmdException.ExecuteNonQuery();
                            sqlconn_obj.Close();
                        }
                        else
                        {
                            for (int i = 0; i < dt_location.Rows.Count; i++)
                            {
                                RevenueByStation station_obj = new RevenueByStation();
                                station_obj.Station = Convert.ToString(dt_location.Rows[i]["LocationName"]);
                                station_obj.ParkingLot = Convert.ToString(dt_location.Rows[i]["LocationParkingLotName"]);
                                station_obj.OperatorIn = Convert.ToString(dt_location.Rows[i]["OperatorIn"]);
                                station_obj.PassesIn = Convert.ToString(dt_location.Rows[i]["PassesIn"]);
                                station_obj.AppIn = Convert.ToString(dt_location.Rows[i]["AppIn"]);
                                station_obj.CallIn = Convert.ToString(dt_location.Rows[i]["CallIn"]);
                                station_obj.Out = Convert.ToString(dt_location.Rows[i]["Out"]);
                                station_obj.FOC = Convert.ToString(dt_location.Rows[i]["FOC"]);
                                station_obj.Clamps = Convert.ToString(dt_location.Rows[i]["Clamp"]);
                                station_obj.Cash = Convert.ToString(dt_location.Rows[i]["Cash"]);
                                station_obj.EPay = Convert.ToString(dt_location.Rows[i]["Epay"]);
                                station_obj.Amount = Convert.ToDecimal(dt_location.Rows[i]["Amount"]);

                                if (i == 0)
                                {
                                    station_obj.TotalOperatorIn = Convert.ToString(dt_location.Compute("Sum(OperatorIn)", ""));
                                    station_obj.TotalPassesIn = Convert.ToString(dt_location.Compute("Sum(PassesIn)", ""));
                                    station_obj.TotalAppIn = Convert.ToString(dt_location.Compute("Sum(AppIn)", ""));
                                    station_obj.TotalCallIn = Convert.ToString(dt_location.Compute("Sum(CallIn)", ""));
                                    station_obj.TotalOut = Convert.ToString(dt_location.Compute("Sum(Out)", ""));
                                    station_obj.TotalFOC = Convert.ToString(dt_location.Compute("Sum(FOC)", ""));
                                    station_obj.TotalClamps = Convert.ToString(dt_location.Compute("Sum(Clamp)", ""));

                                    station_obj.TotalCash = Convert.ToString(dt_location.Compute("Sum(Cash)", ""));
                                    station_obj.TotalEPay = Convert.ToString(dt_location.Compute("Sum(Epay)", ""));
                                }

                                revenue_List.Add(station_obj);
                            }
                        }
                    }
                }
                return revenue_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<RevenueByVehicle> GetReportByVehicle(SearchFilters vehicleFilterData)
        {
            sqlhelper_obj = new SqlHelper();
            List<RevenueByVehicle> revenue_List = new List<RevenueByVehicle>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetReportByVehicleType", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@Company", vehicleFilterData.Company);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", vehicleFilterData.LocationID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotID", vehicleFilterData.LocationParkingLotID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@ApplicationTypeID", vehicleFilterData.ApplicationTypeID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", vehicleFilterData.VehicleTypeID);
                        if (vehicleFilterData.Duration != "0" && vehicleFilterData.Duration != null)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", vehicleFilterData.Duration);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", DBNull.Value);
                        }
                        if (String.IsNullOrEmpty(vehicleFilterData.FromDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", vehicleFilterData.FromDate);
                        }
                        if (String.IsNullOrEmpty(vehicleFilterData.ToDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", vehicleFilterData.ToDate);
                        }
                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_location = ds.Tables[0];
                        for (int i = 0; i < dt_location.Rows.Count; i++)
                        {
                            RevenueByVehicle vehicle_obj = new RevenueByVehicle();
                            vehicle_obj.VehicleType = Convert.ToString(dt_location.Rows[i]["VehicleTypeName"]);
                            vehicle_obj.Station = Convert.ToString(dt_location.Rows[i]["LocationName"]);
                            vehicle_obj.ParkingLot = Convert.ToString(dt_location.Rows[i]["LocationParkingLotName"]);
                            vehicle_obj.In = Convert.ToString(dt_location.Rows[i]["In"]);
                            vehicle_obj.Out = Convert.ToString(dt_location.Rows[i]["Out"]);
                            vehicle_obj.FOC = Convert.ToString(dt_location.Rows[i]["FOC"]);
                            vehicle_obj.Clamps = Convert.ToString(dt_location.Rows[i]["Clamp"]);
                            vehicle_obj.Cash = Convert.ToString(dt_location.Rows[i]["Cash"]);
                            vehicle_obj.EPay = Convert.ToString(dt_location.Rows[i]["Epay"]);
                            vehicle_obj.Amount = dt_location.Rows[i]["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dt_location.Rows[i]["Amount"]);

                            if (i == 0)
                            {
                                vehicle_obj.TotalIn = Convert.ToString(dt_location.Compute("Sum([In])", ""));
                                vehicle_obj.TotalOut = Convert.ToString(dt_location.Compute("Sum([Out])", ""));
                                vehicle_obj.TotalFOC = Convert.ToString(dt_location.Compute("Sum(FOC)", ""));
                                vehicle_obj.TotalClamps = Convert.ToString(dt_location.Compute("Sum(Clamp)", ""));

                                vehicle_obj.TotalCash = Convert.ToString(dt_location.Compute("Sum(Cash)", ""));
                                vehicle_obj.TotalEPay = Convert.ToString(dt_location.Compute("Sum(Epay)", ""));
                            }

                            revenue_List.Add(vehicle_obj);
                        }
                    }
                }
                return revenue_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<RevenueByPaymentType> GetReportByPaymentType(SearchFilters paymentFilterData)
        {
            sqlhelper_obj = new SqlHelper();
            List<RevenueByPaymentType> revenue_List = new List<RevenueByPaymentType>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetReportByPaymentType", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@Company", paymentFilterData.Company);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", paymentFilterData.LocationID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotID", paymentFilterData.LocationParkingLotID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@ApplicationTypeID", paymentFilterData.ApplicationTypeID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", paymentFilterData.VehicleTypeID);
                        if (paymentFilterData.Duration != "0" && paymentFilterData.Duration != null)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", paymentFilterData.Duration);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", DBNull.Value);
                        }
                        if (String.IsNullOrEmpty(paymentFilterData.FromDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", paymentFilterData.FromDate);
                        }
                        if (String.IsNullOrEmpty(paymentFilterData.ToDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", paymentFilterData.ToDate);
                        }
                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_location = ds.Tables[0];
                        for (int i = 0; i < dt_location.Rows.Count; i++)
                        {
                            RevenueByPaymentType payment_obj = new RevenueByPaymentType();
                            payment_obj.PaymentType = Convert.ToString(dt_location.Rows[i]["PaymentTypeName"]);
                            // payment_obj.In = Convert.ToString(dt_location.Rows[i]["In"]);
                            // payment_obj.Out = Convert.ToString(dt_location.Rows[i]["Out"]);
                            //payment_obj.FOC = Convert.ToString(dt_location.Rows[i]["FOC"]);
                            payment_obj.OperatorIn = Convert.ToString(dt_location.Rows[i]["OperatorIn"]);
                            payment_obj.AppIn = Convert.ToString(dt_location.Rows[i]["AppIn"]);
                            payment_obj.CallIn = Convert.ToString(dt_location.Rows[i]["CallIn"]);
                            payment_obj.Amount = dt_location.Rows[i]["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dt_location.Rows[i]["Amount"]);
                            if (i == 0)
                            {
                                payment_obj.OperatorInTotal = Convert.ToString(dt_location.Compute("Sum(OperatorIn)", ""));
                                payment_obj.AppInTotal = Convert.ToString(dt_location.Compute("Sum(AppIn)", ""));
                                payment_obj.CallInTotal = Convert.ToString(dt_location.Compute("Sum(CallIn)", ""));
                            }
                            revenue_List.Add(payment_obj);
                        }
                    }
                }
                return revenue_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<RevenueByPasses> GetReportByPasses(SearchFilters passFilterData)
        {
            sqlhelper_obj = new SqlHelper();
            List<RevenueByPasses> revenue_List = new List<RevenueByPasses>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetRevenueReportByPass", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@Company", passFilterData.Company);
                        //sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", "All");
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", passFilterData.LocationID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", passFilterData.VehicleTypeID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@ApplicationTypeID", passFilterData.ApplicationTypeID);
                        if (passFilterData.Duration != "0" && passFilterData.Duration != null)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", passFilterData.Duration);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", DBNull.Value);
                        }
                        if (String.IsNullOrEmpty(passFilterData.FromDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", passFilterData.FromDate);
                        }
                        if (String.IsNullOrEmpty(passFilterData.ToDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", passFilterData.ToDate);
                        }
                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_location = ds.Tables[0];
                        for (int i = 0; i < dt_location.Rows.Count; i++)
                        {
                            RevenueByPasses passes_obj = new RevenueByPasses();
                            passes_obj.Station = Convert.ToString(dt_location.Rows[i]["Station"]);
                            passes_obj.ParkingLot = Convert.ToString(dt_location.Rows[i]["LotName"]);
                            //passes_obj.VehicleType = Convert.ToString(dt_location.Rows[i]["VehicleTypeName"]);
                            //passes_obj.TypeofPass = Convert.ToString(dt_location.Rows[i]["PassCode"]);
                            passes_obj.TypeofPassName = Convert.ToString(dt_location.Rows[i]["PassName"]);
                            //passes_obj.PassIn = Convert.ToString(dt_location.Rows[i]["PassIn"]);
                            passes_obj.Count = Convert.ToString(dt_location.Rows[i]["New"]);
                            //passes_obj.NFC = Convert.ToString(dt_location.Rows[i]["NFC"]);
                            passes_obj.PassWithNFC = Convert.ToString(dt_location.Rows[i]["PassWithNFC"]);
                            passes_obj.OnlyNFC = Convert.ToString(dt_location.Rows[i]["OnlyNFC"]);
                            passes_obj.Cash = Convert.ToString(dt_location.Rows[i]["Cash"]);
                            passes_obj.EPay = Convert.ToString(dt_location.Rows[i]["EPay"]);
                            passes_obj.Amount = dt_location.Rows[i]["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dt_location.Rows[i]["Amount"]);
                            if (i == 0)
                            {
                                passes_obj.TotalCount = Convert.ToString(dt_location.Compute("Sum(New)", ""));
                                passes_obj.TotalPassWithNFC = Convert.ToString(dt_location.Compute("Sum(PassWithNFC)", ""));
                                passes_obj.TotalOnlyNFC = Convert.ToString(dt_location.Compute("Sum(OnlyNFC)", ""));

                                passes_obj.TotalCash = Convert.ToString(dt_location.Compute("Sum(Cash)", ""));
                                passes_obj.TotalEPay = Convert.ToString(dt_location.Compute("Sum(Epay)", ""));
                            }
                            revenue_List.Add(passes_obj);
                        }
                    }
                }
                return revenue_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<RevenueByChannel> GetReportByChannel(SearchFilters channelFilterData)
        {
            sqlhelper_obj = new SqlHelper();
            List<RevenueByChannel> revenue_List = new List<RevenueByChannel>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetRevenueReportByChannel", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@Company", channelFilterData.Company);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", channelFilterData.LocationID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotID", channelFilterData.LocationParkingLotID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@ApplicationTypeID", channelFilterData.ApplicationTypeID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", channelFilterData.VehicleTypeID);
                        if (channelFilterData.Duration != "0" && channelFilterData.Duration != null)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", channelFilterData.Duration);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", DBNull.Value);
                        }
                        if (String.IsNullOrEmpty(channelFilterData.FromDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", channelFilterData.FromDate);
                        }
                        if (String.IsNullOrEmpty(channelFilterData.ToDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", channelFilterData.ToDate);
                        }
                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_location = ds.Tables[0];
                        for (int i = 0; i < dt_location.Rows.Count; i++)
                        {
                            RevenueByChannel channel_obj = new RevenueByChannel();
                            channel_obj.Channel = Convert.ToString(dt_location.Rows[i]["ApplicationTypeName"]);
                            channel_obj.Station = Convert.ToString(dt_location.Rows[i]["LocationName"]);
                            channel_obj.ParkingLot = Convert.ToString(dt_location.Rows[i]["LocationParkingLotName"]);
                            channel_obj.CheckIns = Convert.ToInt32(dt_location.Rows[i]["CheckIns"]);
                            channel_obj.Cash = Convert.ToString(dt_location.Rows[i]["Cash"]);
                            channel_obj.EPay = Convert.ToString(dt_location.Rows[i]["EPay"]);
                            channel_obj.Amount = dt_location.Rows[i]["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dt_location.Rows[i]["Amount"]);
                            if (i == 0)
                            {
                                channel_obj.TotalCheckIns = Convert.ToString(dt_location.Compute("Sum(CheckIns)", ""));
                                channel_obj.TotalCash = Convert.ToString(dt_location.Compute("Sum(Cash)", ""));
                                channel_obj.TotalEPay = Convert.ToString(dt_location.Compute("Sum(Epay)", ""));
                            }
                            revenue_List.Add(channel_obj);
                        }
                    }
                }
                return revenue_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<RevenueByViolation> GetReportByViolation(SearchFilters violationFilterData)
        {
            sqlhelper_obj = new SqlHelper();
            List<RevenueByViolation> revenue_List = new List<RevenueByViolation>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetRevenueReportByViolation", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@Company", violationFilterData.Company);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", violationFilterData.LocationID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotID", violationFilterData.LocationParkingLotID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", violationFilterData.VehicleTypeID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@Reason", violationFilterData.Reason);
                        if (violationFilterData.Duration != "0" && violationFilterData.Duration != null)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", violationFilterData.Duration);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", DBNull.Value);
                        }
                        if (String.IsNullOrEmpty(violationFilterData.FromDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", violationFilterData.FromDate);
                        }
                        if (String.IsNullOrEmpty(violationFilterData.ToDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", violationFilterData.ToDate);
                        }
                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_location = ds.Tables[0];
                        for (int i = 0; i < dt_location.Rows.Count; i++)
                        {
                            RevenueByViolation violation_obj = new RevenueByViolation();
                            violation_obj.Reason = Convert.ToString(dt_location.Rows[i]["StatusName"]);
                            violation_obj.Station = Convert.ToString(dt_location.Rows[i]["LocationName"]);
                            violation_obj.ParkingLot = Convert.ToString(dt_location.Rows[i]["LocationParkingLotName"]);
                            // violation_obj.ClampFee = Convert.ToString(dt_location.Rows[i]["ClampFee"]);
                            violation_obj.Clamps = Convert.ToString(dt_location.Rows[i]["Clamp"]);
                            violation_obj.Cash = Convert.ToString(dt_location.Rows[i]["Cash"]);
                            violation_obj.EPay = Convert.ToString(dt_location.Rows[i]["EPay"]);
                            violation_obj.Amount = dt_location.Rows[i]["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dt_location.Rows[i]["Amount"]);
                            if (i == 0)
                            {
                                violation_obj.TotalClamps = Convert.ToString(dt_location.Compute("Sum(Clamp)", ""));
                                violation_obj.TotalCash = Convert.ToString(dt_location.Compute("Sum(Cash)", ""));
                                violation_obj.TotalEPay = Convert.ToString(dt_location.Compute("Sum(Epay)", ""));
                            }
                            revenue_List.Add(violation_obj);
                        }
                    }
                }
                return revenue_List;
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
                            if (Convert.ToInt32(dt.Rows[i]["UserTypeID"]) != 1)
                            {
                                User user_obj = new User();
                                user_obj.UserID = Convert.ToInt32(dt.Rows[i]["UserID"]);
                                user_obj.UserName = Convert.ToString(dt.Rows[i]["UserName"]);
                                userList.Add(user_obj);
                            }
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
        public IList<Locations> GetSupervisorLocationList(int supervisorID)
        {
            IList<Locations> locationList = new List<Locations>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getusers_obj = new SqlCommand("PARK_PROC_GetLocationsBySupervisor", sqlconn_obj))
                    {
                        sqlcmd_getusers_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getusers_obj.CommandTimeout = 0;
                        sqlcmd_getusers_obj.Parameters.AddWithValue("@supervisorID", supervisorID);
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getusers_obj))
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
                            locationList.Add(location_obj);
                        }
                    }
                }
                return locationList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<RevenueBySupervisor> GetReportBySupervisor(SearchFilters supervisorFilterData)
        {
            sqlhelper_obj = new SqlHelper();
            List<RevenueBySupervisor> revenue_List = new List<RevenueBySupervisor>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetRevenueReportBySupervisor", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@Company", supervisorFilterData.Company);
                        sqlcmd_details_obj.Parameters.AddWithValue("@SupervisorID", supervisorFilterData.SupervisorID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", supervisorFilterData.LocationID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotID", supervisorFilterData.LocationParkingLotID);
                        if (supervisorFilterData.Duration != "0" && supervisorFilterData.Duration != null)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", supervisorFilterData.Duration);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", DBNull.Value);
                        }
                        if (String.IsNullOrEmpty(supervisorFilterData.FromDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", supervisorFilterData.FromDate);
                        }
                        if (String.IsNullOrEmpty(supervisorFilterData.ToDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", supervisorFilterData.ToDate);
                        }
                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_location = ds.Tables[0];
                        for (int i = 0; i < dt_location.Rows.Count; i++)
                        {
                            RevenueBySupervisor supervisor_obj = new RevenueBySupervisor();
                            supervisor_obj.Supervisor = Convert.ToString(dt_location.Rows[i]["Supervisor"]);
                            supervisor_obj.Station = Convert.ToString(dt_location.Rows[i]["Station"]);
                            supervisor_obj.Operator = Convert.ToString(dt_location.Rows[i]["Operator"]);
                            supervisor_obj.ParkingLot = Convert.ToString(dt_location.Rows[i]["LotName"]);
                            supervisor_obj.CheckInsCash = dt_location.Rows[i]["CheckInsCash"] == DBNull.Value ? 0 : Convert.ToDecimal(dt_location.Rows[i]["CheckInsCash"]);
                            supervisor_obj.CheckInsEPay = dt_location.Rows[i]["CheckInsEPay"] == DBNull.Value ? 0 : Convert.ToDecimal(dt_location.Rows[i]["CheckInsEPay"]);
                            supervisor_obj.PassesCash = dt_location.Rows[i]["PassesCash"] == DBNull.Value ? 0 : Convert.ToDecimal(dt_location.Rows[i]["PassesCash"]);
                            supervisor_obj.PassesEPay = dt_location.Rows[i]["PassesEPay"] == DBNull.Value ? 0 : Convert.ToDecimal(dt_location.Rows[i]["PassesEPay"]);
                            supervisor_obj.Amount = dt_location.Rows[i]["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dt_location.Rows[i]["Amount"]);
                            supervisor_obj.ClampCash = dt_location.Rows[i]["ClampCash"] == DBNull.Value ? 0 : Convert.ToDecimal(dt_location.Rows[i]["ClampCash"]);
                            supervisor_obj.ClampEPay = dt_location.Rows[i]["ClampEPay"] == DBNull.Value ? 0 : Convert.ToDecimal(dt_location.Rows[i]["ClampEPay"]);
                            supervisor_obj.NFCCash = dt_location.Rows[i]["NFCCash"] == DBNull.Value ? 0 : Convert.ToDecimal(dt_location.Rows[i]["NFCCash"]);
                            supervisor_obj.NFCEPay = dt_location.Rows[i]["NFCEPay"] == DBNull.Value ? 0 : Convert.ToDecimal(dt_location.Rows[i]["NFCEPay"]);
                            supervisor_obj.DueCash = dt_location.Rows[i]["DueCash"] == DBNull.Value ? 0 : Convert.ToDecimal(dt_location.Rows[i]["DueCash"]);
                            supervisor_obj.DueEPay = dt_location.Rows[i]["DueEPay"] == DBNull.Value ? 0 : Convert.ToDecimal(dt_location.Rows[i]["DueEPay"]);
                            if (i == 0)
                            {
                                supervisor_obj.TotalClampCash = Convert.ToString(dt_location.Compute("Sum(ClampCash)", ""));
                                supervisor_obj.TotalClampEPay = Convert.ToString(dt_location.Compute("Sum(ClampEPay)", ""));
                                supervisor_obj.TotalCheckInsCash = Convert.ToString(dt_location.Compute("Sum(CheckInsCash)", ""));
                                supervisor_obj.TotalCheckInsEPay = Convert.ToString(dt_location.Compute("Sum(CheckInsEPay)", ""));
                                supervisor_obj.TotalPassesCash = Convert.ToString(dt_location.Compute("Sum(PassesCash)", ""));
                                supervisor_obj.TotalPassesEPay = Convert.ToString(dt_location.Compute("Sum(PassesEPay)", ""));
                                supervisor_obj.TotalNFCCash = Convert.ToString(dt_location.Compute("Sum(NFCCash)", ""));
                                supervisor_obj.TotalNFCEPay = Convert.ToString(dt_location.Compute("Sum(NFCEPay)", ""));
                                supervisor_obj.TotalDueCash = Convert.ToString(dt_location.Compute("Sum(DueCash)", ""));
                                supervisor_obj.TotalDueEPay = Convert.ToString(dt_location.Compute("Sum(DueEPay)", ""));
                            }
                            revenue_List.Add(supervisor_obj);
                        }
                    }
                }
                return revenue_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<RevenueByTime> GetReportByTime(SearchFilters timeFilterData)
        {
            sqlhelper_obj = new SqlHelper();
            List<RevenueByTime> revenue_List = new List<RevenueByTime>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetRevenueReportByTime", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@Company", timeFilterData.Company);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", timeFilterData.LocationID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotID", timeFilterData.LocationParkingLotID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@ApplicationTypeID", timeFilterData.ApplicationTypeID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", timeFilterData.VehicleTypeID);
                        if (timeFilterData.Duration != "0" && timeFilterData.Duration != null)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", timeFilterData.Duration);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", DBNull.Value);
                        }
                        if (String.IsNullOrEmpty(timeFilterData.FromDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", timeFilterData.FromDate);
                        }
                        if (String.IsNullOrEmpty(timeFilterData.ToDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", timeFilterData.ToDate);
                        }
                        sqlcmd_details_obj.Parameters.AddWithValue("@Display", timeFilterData.Display);
                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_location = ds.Tables[0];
                        for (int i = 0; i < dt_location.Rows.Count; i++)
                        {
                            RevenueByTime time_obj = new RevenueByTime();
                            time_obj.TimePeriod = Convert.ToString(dt_location.Rows[i]["TimePeriod"]);
                            time_obj.Station = Convert.ToString(dt_location.Rows[i]["LocationName"]);
                            time_obj.ParkingLot = Convert.ToString(dt_location.Rows[i]["LocationParkingLotName"]);
                            time_obj.In = Convert.ToString(dt_location.Rows[i]["In"]);
                            time_obj.Out = Convert.ToString(dt_location.Rows[i]["Out"]);
                            time_obj.FOC = Convert.ToString(dt_location.Rows[i]["FOC"]);
                            time_obj.Cash = Convert.ToDecimal(dt_location.Rows[i]["Cash"]);
                            time_obj.EPay = Convert.ToDecimal(dt_location.Rows[i]["EPay"]);
                            time_obj.Amount = dt_location.Rows[i]["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dt_location.Rows[i]["Amount"]);
                            if (i == 0)
                            {
                                time_obj.TotalIn = Convert.ToString(dt_location.Compute("Sum([In])", ""));
                                time_obj.TotalOut = Convert.ToString(dt_location.Compute("Sum([Out])", ""));
                                time_obj.TotalFOC = Convert.ToString(dt_location.Compute("Sum(FOC)", ""));

                                time_obj.TotalCash = Convert.ToString(dt_location.Compute("Sum(Cash)", ""));
                                time_obj.TotalEPay = Convert.ToString(dt_location.Compute("Sum(Epay)", ""));
                            }
                            revenue_List.Add(time_obj);
                        }
                    }
                }
                return revenue_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Account GetCompanyInfoDetails()
        {
            Account account_obj;
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_GetCompanyDetails", sqlconn_obj))
                    {
                        sqlcmd_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_obj.CommandTimeout = 0;
                        DataTable dt;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_obj))
                        {
                            dt = new DataTable();
                            da.Fill(dt);
                        }

                        account_obj = new Account();
                        account_obj.AccountName = Convert.ToString(dt.Rows[0]["AccountName"]);
                        account_obj.Address1 = Convert.ToString(dt.Rows[0]["Address1"]);
                        account_obj.Address2 = Convert.ToString(dt.Rows[0]["Address2"]);
                        account_obj.GSTNumber = Convert.ToString(dt.Rows[0]["GSTNumber"]);
                        account_obj.ContactNumber = Convert.ToString(dt.Rows[0]["ContactNumber"]);
                        account_obj.SupportEmailID = Convert.ToString(dt.Rows[0]["SupportEmailID"]);
                        account_obj.CompanyLogo = Convert.ToString(dt.Rows[0]["CompanyLogo"]);
                    }
                }
                return account_obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //04032021
        public List<RevenueByDueAmount> GetDueAmountReport(SearchFilters filterData)
        {
            sqlhelper_obj = new SqlHelper();
            List<RevenueByDueAmount> revenue_List = new List<RevenueByDueAmount>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetReportByDueAmount", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@Company", filterData.Company);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", filterData.LocationID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotID", filterData.LocationParkingLotID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@ApplicationTypeID", filterData.ApplicationTypeID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", filterData.VehicleTypeID);
                        if (filterData.Duration != "0" && filterData.Duration != null)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", filterData.Duration);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", DBNull.Value);
                        }
                        if (String.IsNullOrEmpty(filterData.FromDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", filterData.FromDate);
                        }
                        if (String.IsNullOrEmpty(filterData.ToDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", filterData.ToDate);
                        }
                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_location = ds.Tables[0];
                        if (dt_location.Rows.Count == 0)
                        {
                            SqlCommand sqlcmdException = new SqlCommand("PARK_PROC_SaveLog", sqlconn_obj);
                            sqlcmdException.CommandType = CommandType.StoredProcedure;
                            sqlcmdException.CommandTimeout = 0;
                            sqlcmdException.Parameters.AddWithValue("@ApplicationType", "Parking");
                            sqlcmdException.Parameters.AddWithValue("@ExceptionMessage", string.Concat(filterData.LocationID, ",", filterData.LocationParkingLotID, ",", filterData.FromDate, ",", filterData.ToDate));
                            sqlcmdException.Parameters.AddWithValue("@Model", "ReportModel");
                            sqlcmdException.Parameters.AddWithValue("@Procedure", "PARK_PROC_GetReportByDueAmount");
                            sqlcmdException.Parameters.AddWithValue("@ApplicationMethod", "GetDueCollectedReport");
                            sqlconn_obj.Open();
                            sqlcmdException.ExecuteNonQuery();
                            sqlconn_obj.Close();
                        }
                        else
                        {
                            for (int i = 0; i < dt_location.Rows.Count; i++)
                            {
                                RevenueByDueAmount due_obj = new RevenueByDueAmount();
                                due_obj.Station = Convert.ToString(dt_location.Rows[i]["LocationName"]);
                                due_obj.ParkingLot = Convert.ToString(dt_location.Rows[i]["LocationParkingLotName"]);
                                due_obj.Cash = Convert.ToString(dt_location.Rows[i]["Cash"]);
                                due_obj.EPay = Convert.ToString(dt_location.Rows[i]["Epay"]);
                                due_obj.Amount = Convert.ToDecimal(dt_location.Rows[i]["Amount"]);

                                if (i == 0)
                                {
                                    due_obj.TotalCash = Convert.ToString(dt_location.Compute("Sum(Cash)", ""));
                                    due_obj.TotalEPay = Convert.ToString(dt_location.Compute("Sum(Epay)", ""));
                                }

                                revenue_List.Add(due_obj);
                            }
                        }
                    }
                }
                return revenue_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
