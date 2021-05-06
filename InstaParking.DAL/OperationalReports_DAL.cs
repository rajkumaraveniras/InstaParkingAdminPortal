using InstaParking.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace InstaParking.DAL
{
    public class OperationalReports_DAL
    {
        SqlHelper sqlhelper_obj;
        public List<CheckInReport> GetCheckInReport(SearchFilters checkinFilterData)
        {
            sqlhelper_obj = new SqlHelper();
            List<CheckInReport> checkin_List = new List<CheckInReport>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetCheckinReport", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@Company", checkinFilterData.Company);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", checkinFilterData.LocationID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotID", checkinFilterData.LocationParkingLotID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@ApplicationTypeID", checkinFilterData.ApplicationTypeID);
                        if (checkinFilterData.Duration != "0" && checkinFilterData.Duration != null)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", checkinFilterData.Duration);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", DBNull.Value);
                        }

                        if (String.IsNullOrEmpty(checkinFilterData.FromDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", checkinFilterData.FromDate);
                        }
                        if (String.IsNullOrEmpty(checkinFilterData.ToDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", checkinFilterData.ToDate);
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
                            CheckInReport checkin_obj = new CheckInReport();
                            checkin_obj.Station = Convert.ToString(dt_location.Rows[i]["LocationName"]);
                            checkin_obj.ParkingLot = Convert.ToString(dt_location.Rows[i]["LocationParkingLotName"]);
                            checkin_obj.App = Convert.ToInt32(dt_location.Rows[i]["App"]);
                            checkin_obj.Pass = Convert.ToInt32(dt_location.Rows[i]["Pass"]);
                            checkin_obj.Operator = Convert.ToInt32(dt_location.Rows[i]["Operator"]);
                            checkin_obj.CallPay = Convert.ToInt32(dt_location.Rows[i]["Call Pay"]);
                            checkin_obj.Total = Convert.ToInt32(dt_location.Rows[i]["Total"]);
                            checkin_obj.Out = dt_location.Rows[i]["Out"]==DBNull.Value?0: Convert.ToInt32(dt_location.Rows[i]["Out"]);
                            checkin_obj.FOC = dt_location.Rows[i]["FOC"] == DBNull.Value ? 0 : Convert.ToInt32(dt_location.Rows[i]["FOC"]);
                            if (i == 0)
                            {
                                checkin_obj.AppTotal = Convert.ToString(dt_location.Compute("Sum(App)", ""));
                                checkin_obj.PassTotal = Convert.ToString(dt_location.Compute("Sum(Pass)", ""));
                                checkin_obj.OperatorTotal = Convert.ToString(dt_location.Compute("Sum(Operator)", ""));
                                checkin_obj.CallPayTotal = Convert.ToString(dt_location.Compute("Sum([Call Pay])", ""));
                                checkin_obj.OutTotal = Convert.ToString(dt_location.Compute("Sum(Out)", ""));
                                checkin_obj.FOCTotal = Convert.ToString(dt_location.Compute("Sum(FOC)", ""));
                            }
                            checkin_List.Add(checkin_obj);
                        }
                    }
                }
                return checkin_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<Lots> GetOperatorLotsList(int operatorID)
        {
            IList<Lots> lotsList = new List<Lots>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getusers_obj = new SqlCommand("PARK_PROC_GetOperatorWorkLotList", sqlconn_obj))
                    {
                        sqlcmd_getusers_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getusers_obj.CommandTimeout = 0;
                        sqlcmd_getusers_obj.Parameters.AddWithValue("@OperatorID", operatorID);
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getusers_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            Lots lots_obj = new Lots();
                            lots_obj.LocationParkingLotID = Convert.ToInt32(dt.Rows[i]["LocationParkingLotID"]);
                            lots_obj.LocationParkingLotName = Convert.ToString(dt.Rows[i]["LocationParkingLotName"]);
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
        public List<OperatorHoursReport> GetReportByOperator(SearchFilters operatorFilterData)
        {
            sqlhelper_obj = new SqlHelper();
            List<OperatorHoursReport> hours_List = new List<OperatorHoursReport>();

            var fromShortDate = operatorFilterData.FromDate.ToShortDateString();
            var toShortDate = operatorFilterData.ToDate.ToShortDateString();
            double days;
            if (fromShortDate == toShortDate)
            {
                days = 1;
            }
            else
            {
                days = (operatorFilterData.ToDate - operatorFilterData.FromDate).TotalDays + 1;
            }
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetReportByOperator", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@SupervisorID", operatorFilterData.SupervisorID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@OperatorID", operatorFilterData.OperatorID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotID", operatorFilterData.LocationParkingLotID);
                        if (operatorFilterData.Duration != "0" && operatorFilterData.Duration != null)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", operatorFilterData.Duration);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", DBNull.Value);
                        }
                        if (String.IsNullOrEmpty(operatorFilterData.FromDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", operatorFilterData.FromDate);
                        }
                        if (String.IsNullOrEmpty(operatorFilterData.ToDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", operatorFilterData.ToDate);
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
                            OperatorHoursReport hours_obj = new OperatorHoursReport();
                            hours_obj.Supervisor = Convert.ToString(dt_location.Rows[i]["Supervisor"]);
                            hours_obj.Operator = Convert.ToString(dt_location.Rows[i]["Operator"]);
                            if (operatorFilterData.Duration != "Previous Month" && operatorFilterData.Duration != "Current Month" && days==1)
                            {
                                hours_obj.LocationParkingLotName = Convert.ToString(dt_location.Rows[i]["LocationParkingLotName"]);
                            }
                            if(days==1 && operatorFilterData.Duration == "0")
                            {
                                hours_obj.LocationParkingLotName = Convert.ToString(dt_location.Rows[i]["LocationParkingLotName"]);
                            }
                            hours_obj.TotalHours = Convert.ToString(dt_location.Rows[i]["TotalHours"]);
                            hours_obj.TotalDays = Convert.ToString(dt_location.Rows[i]["DAYS"]);

                            if (operatorFilterData.Duration == "Today" || operatorFilterData.Duration == "Yesterday" || operatorFilterData.Duration == "Day Before Yesterday" || days==1)
                            {
                                hours_obj.CheckInTime = Convert.ToString(dt_location.Rows[i]["CheckInTime"]);
                                hours_obj.CheckOutTime = Convert.ToString(dt_location.Rows[i]["CheckOutTime"]);
                            }
                            if (i == 0)
                            {
                                hours_obj.Total = Convert.ToString(dt_location.Compute("Sum(DAYS)", ""));
                            }
                            hours_List.Add(hours_obj);
                        }
                    }
                }
                return hours_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<OccupancyReport> GetOccupancyReport(SearchFilters occupancyFilterData)
        {
            sqlhelper_obj = new SqlHelper();
            List<OccupancyReport> occupancy_List = new List<OccupancyReport>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetOccupancyReport", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", occupancyFilterData.LocationID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotID", occupancyFilterData.LocationParkingLotID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", occupancyFilterData.VehicleTypeID);

                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_location = ds.Tables[0];
                        for (int i = 0; i < dt_location.Rows.Count; i++)
                        {
                            OccupancyReport occupancy_obj = new OccupancyReport();
                            occupancy_obj.Station = Convert.ToString(dt_location.Rows[i]["LocationName"]);
                            occupancy_obj.ParkingLot = Convert.ToString(dt_location.Rows[i]["LocationParkingLotName"]);
                            occupancy_obj.Capacity = Convert.ToString(dt_location.Rows[i]["capacity"]);
                            occupancy_obj.CurrentlyParked = Convert.ToString(dt_location.Rows[i]["CurrentlyParked"]);
                            occupancy_obj.Occupancy = Convert.ToString(dt_location.Rows[i]["% Occupancy"]);
                            occupancy_List.Add(occupancy_obj);
                        }
                    }
                }
                return occupancy_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<FOCReport> GetFOCReport(SearchFilters FOCfilterData)
        {
            sqlhelper_obj = new SqlHelper();
            List<FOCReport> FOC_List = new List<FOCReport>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetFOCReport", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@SupervisorID", FOCfilterData.SupervisorID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationID", FOCfilterData.LocationID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LocationParkingLotID", FOCfilterData.LocationParkingLotID);
                        if (FOCfilterData.Duration != "0" && FOCfilterData.Duration != null)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", FOCfilterData.Duration);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", DBNull.Value);
                        }

                        if (String.IsNullOrEmpty(FOCfilterData.FromDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", FOCfilterData.FromDate);
                        }
                        if (String.IsNullOrEmpty(FOCfilterData.ToDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", FOCfilterData.ToDate);
                        }
                        sqlcmd_details_obj.Parameters.AddWithValue("@FOCReasonID", FOCfilterData.FOCReasonID);

                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_location = ds.Tables[0];
                        for (int i = 0; i < dt_location.Rows.Count; i++)
                        {
                            FOCReport FOC_obj = new FOCReport();
                            FOC_obj.Station = Convert.ToString(dt_location.Rows[i]["LocationName"]);
                            FOC_obj.ParkingLot = Convert.ToString(dt_location.Rows[i]["LocationParkingLotName"]);
                            FOC_obj.FOCReason = Convert.ToString(dt_location.Rows[i]["FOCReason"]);
                            FOC_obj.FOCCount = Convert.ToString(dt_location.Rows[i]["FOCCount"]);
                            FOC_obj.DueAmount = Convert.ToString(dt_location.Rows[i]["DueAmount"]);
                            if (i == 0)
                            {
                                FOC_obj.Total = Convert.ToString(dt_location.Compute("Sum(DueAmount)", ""));
                            }
                            FOC_List.Add(FOC_obj);
                        }
                    }
                }
                return FOC_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<ViolationReason> GetActiveFOCReasonList()
        {
            IList<ViolationReason> violationList = new List<ViolationReason>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_GetFOCReasons", sqlconn_obj))
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
                            ViolationReason violation_obj = new ViolationReason();
                            violation_obj.ViolationReasonID = Convert.ToInt32(dt.Rows[i]["ViolationReasonID"]);
                            violation_obj.Reason = Convert.ToString(dt.Rows[i]["Reason"]);
                            violationList.Add(violation_obj);
                        }
                    }
                }
                return violationList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<User> GetOperatorsBySupervisorID(int supervisorID)
        {
            IList<User> usersList = new List<User>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getusers_obj = new SqlCommand("PARK_PROC_GetActiveOperatorsBySupervisorID", sqlconn_obj))
                    {
                        sqlcmd_getusers_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getusers_obj.CommandTimeout = 0;
                        sqlcmd_getusers_obj.Parameters.AddWithValue("@SupervisorID", supervisorID);
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
                            users_obj.UserName = Convert.ToString(dt.Rows[i]["UserName"]);
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
        public List<Allocations> GetAllAllocations(SearchFilters allocationsFilterData)
        {
            sqlhelper_obj = new SqlHelper();
            List<Allocations> allocations_List = new List<Allocations>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetAllAllocations", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@Company", allocationsFilterData.Company);
                        sqlcmd_details_obj.Parameters.AddWithValue("@Location", allocationsFilterData.LocationID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@ParkingLot", allocationsFilterData.LocationParkingLotID);

                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_location = ds.Tables[0];
                        for (int i = 0; i < dt_location.Rows.Count; i++)
                        {
                            Allocations allocations_obj = new Allocations();
                            allocations_obj.EmpId = Convert.ToString(dt_location.Rows[i]["EmpID"]);
                            allocations_obj.EmpName = Convert.ToString(dt_location.Rows[i]["EmpName"]);
                            allocations_obj.Role = Convert.ToString(dt_location.Rows[i]["Role"]);
                            allocations_obj.ReportsTo = Convert.ToString(dt_location.Rows[i]["ReportsTo"]);
                            allocations_obj.Station = Convert.ToString(dt_location.Rows[i]["Station"]);
                            allocations_obj.ParkingLot = Convert.ToString(dt_location.Rows[i]["ParkingLot"]);
                            allocations_obj.LoginTime = Convert.ToString(dt_location.Rows[i]["LoginTime"]);
                            allocations_List.Add(allocations_obj);
                        }
                    }
                }
                return allocations_List;
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

                    using (SqlCommand sqlcmd_getusers_obj = new SqlCommand("PARK_PROC_GetAcitveSupervisorsForRepByOperator", sqlconn_obj))
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
        public List<DuplicateEntries> GetDuplicateEntries(SearchFilters duplicateFilterData)
        {
            sqlhelper_obj = new SqlHelper();
            List<DuplicateEntries> duplicate_List = new List<DuplicateEntries>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetDuplicateVehicleEntries", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        if (duplicateFilterData.Duration != "0" && duplicateFilterData.Duration != null)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", duplicateFilterData.Duration);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", DBNull.Value);
                        }
                        if (String.IsNullOrEmpty(duplicateFilterData.FromDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@FromDate", duplicateFilterData.FromDate);
                        }
                        if (String.IsNullOrEmpty(duplicateFilterData.ToDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@ToDate", duplicateFilterData.ToDate);
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
                            DuplicateEntries duration_obj = new DuplicateEntries();
                            duration_obj.Operator = Convert.ToString(dt_location.Rows[i]["Operator"]);
                            duration_obj.Supervisor = Convert.ToString(dt_location.Rows[i]["Supervisor"]);
                            duration_obj.Station = Convert.ToString(dt_location.Rows[i]["Station"]);
                            duration_obj.ParkingLot = Convert.ToString(dt_location.Rows[i]["ParkingLot"]);
                            duration_obj.RegistrationNumber = Convert.ToString(dt_location.Rows[i]["RegistrationNumber"]);
                            duration_obj.Amount = Convert.ToString(dt_location.Rows[i]["Amount"]);
                            duration_obj.Status = Convert.ToString(dt_location.Rows[i]["Status"]);
                           // duration_obj.ApplicationType = Convert.ToString(dt_location.Rows[i]["ApplicationType"]);
                            duration_obj.TransactionDate = Convert.ToString(dt_location.Rows[i]["CreatedOn"]);
                            duration_obj.NoofTimes = Convert.ToString(dt_location.Rows[i]["NoofTimes"]);
                            duration_obj.GovtVehicle= Convert.ToString(dt_location.Rows[i]["GovtVehicle"]);
                            duration_obj.Violation = Convert.ToString(dt_location.Rows[i]["Violation"]);
                            if (i == 0)
                            {
                                duration_obj.Total = Convert.ToString(dt_location.Compute("Sum(Amount)", ""));
                            }
                            duplicate_List.Add(duration_obj);
                        }
                    }
                }
                return duplicate_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string LogoutEmployee(string EmployeeID,string LoginTime)
        {  
            sqlhelper_obj = new SqlHelper();
            string resultmsg = string.Empty;
          
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_LogoutEmployee", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        
                        sqlcmd_details_obj.Parameters.AddWithValue("@EmpID", EmployeeID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LoginTime", LoginTime);
                        sqlconn_obj.Open();
                        int res = sqlcmd_details_obj.ExecuteNonQuery();
                        sqlconn_obj.Close();
                        if(res>0)
                        {
                            resultmsg = "Success";
                        }
                        else
                        {
                            resultmsg = "Fail";
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
        public List<PassExpiredCustomer> GetPassExpiredCustomersList(SearchFilters passExpiryFilterData)
        {
            sqlhelper_obj = new SqlHelper();
            List<PassExpiredCustomer> customers_List = new List<PassExpiredCustomer>();
           
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetPassExpiredCustomersList", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                       
                        if (passExpiryFilterData.Duration != "0" && passExpiryFilterData.Duration != null)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", passExpiryFilterData.Duration);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@Duration", DBNull.Value);
                        }
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", passExpiryFilterData.VehicleTypeID);
                        DataSet ds;
                        using (SqlDataAdapter sql_dp = new SqlDataAdapter(sqlcmd_details_obj))
                        {
                            ds = new DataSet();
                            sql_dp.Fill(ds);
                        }
                        DataTable dt_location = ds.Tables[0];
                        for (int i = 0; i < dt_location.Rows.Count; i++)
                        {
                            PassExpiredCustomer customer_obj = new PassExpiredCustomer();
                            customer_obj.Name = Convert.ToString(dt_location.Rows[i]["Name"]);
                            customer_obj.PhoneNumber = Convert.ToString(dt_location.Rows[i]["PhoneNumber"]);
                            customer_obj.VehicleType = Convert.ToString(dt_location.Rows[i]["VehicleTypeCode"]);
                            customer_obj.VehicleNumber = Convert.ToString(dt_location.Rows[i]["RegistrationNumber"]);
                            customer_obj.TypeofPass = Convert.ToString(dt_location.Rows[i]["PassTypeName"]);
                            customer_obj.PassExpiryDate = Convert.ToString(dt_location.Rows[i]["ExpiryDate"]);
                            customers_List.Add(customer_obj);
                        }
                    }
                }
                return customers_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string DeleteDuplicateEntries(List<DuplicateEntries> duplicateList,string createdBy)
        {
            sqlhelper_obj = new SqlHelper();
            string resultmsg = "";
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    for (var i = 0; i < duplicateList.Count; i++)
                    {
                        using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_DeleteDuplicates", sqlconn_obj))
                        {
                            sqlcmd_obj.CommandType = CommandType.StoredProcedure;
                            sqlcmd_obj.CommandTimeout = 0;
                            sqlcmd_obj.Parameters.AddWithValue("@RegistrationNumber", duplicateList[i].RegistrationNumber);
                            sqlcmd_obj.Parameters.AddWithValue("@CreatedOn", duplicateList[i].TransactionDate);
                            sqlcmd_obj.Parameters.AddWithValue("@CreatedBy", createdBy);
                            sqlconn_obj.Open();
                            int res = sqlcmd_obj.ExecuteNonQuery();
                            sqlconn_obj.Close();
                            if (res > 0)
                            {
                                resultmsg = "Success";
                            }
                            else
                            {
                                resultmsg = "Failed";
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
    }
}
