using InstaParking.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace InstaParking.DAL
{
    public class UserLocationMapper_DAL
    {
        SqlHelper sqlhelper_obj;
        public List<UserLocationMapper> GetOperatorsLoginStatusList(int supervisorID, string UserType)
        {
            List<UserLocationMapper> userslocationList = new List<UserLocationMapper>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getusers_obj = new SqlCommand("PARK_PROC_GetAssignOperatorsForSupervisor", sqlconn_obj))
                    {
                        sqlcmd_getusers_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getusers_obj.CommandTimeout = 0;
                        sqlcmd_getusers_obj.Parameters.AddWithValue("@SupervisorID", supervisorID);
                        sqlcmd_getusers_obj.Parameters.AddWithValue("@UserType", UserType);
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getusers_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable second_loop = ds.Tables[0];

                        for (int j = 0; j < second_loop.Rows.Count; j++)
                        {
                            UserLocationMapper users_obj = new UserLocationMapper();
                            User user;
                            List<User> list = new List<User>();

                            if (second_loop.Rows[j]["UserID"] != null && second_loop.Rows[j]["UserID"] != DBNull.Value)//031220
                            {//031220

                                if (!userslocationList.Any(t => (t.UserID == Convert.ToInt32(second_loop.Rows[j]["UserID"])
                                        && t.LocationParkingLotID == Convert.ToInt32(second_loop.Rows[j]["LocationParkingLotID"])
                                        )))
                                {

                                    if (!userslocationList.Any(t => t.LocationParkingLotID == Convert.ToInt32(second_loop.Rows[j]["LocationParkingLotID"])))//New
                                    {//New

                                        users_obj.UserID = Convert.ToInt32(second_loop.Rows[j]["UserID"]);
                                        users_obj.LocationID = Convert.ToInt32(second_loop.Rows[j]["LocationID"]);
                                        users_obj.LocationParkingLotID = Convert.ToInt32(second_loop.Rows[j]["LocationParkingLotID"]);
                                        users_obj.UserName = Convert.ToString(second_loop.Rows[j]["UserName"]);
                                        users_obj.LocationName = Convert.ToString(second_loop.Rows[j]["LocationName"]);
                                        users_obj.LocationParkingLotName = Convert.ToString(second_loop.Rows[j]["LocationParkingLotName"]);
                                        users_obj.LoginTime = Convert.ToString(second_loop.Rows[j]["LoginTime"]);

                                        if (Convert.ToString(second_loop.Rows[j]["AssignedUserName"]) != "" && Convert.ToString(second_loop.Rows[j]["AssignedUserID"]) != "" && Convert.ToString(second_loop.Rows[j]["LoginTime"]) != "")//login time block add
                                        {
                                            user = new User();
                                            user.UserName = Convert.ToString(second_loop.Rows[j]["AssignedUserName"]);
                                            user.UserID = second_loop.Rows[j]["AssignedUserID"] == DBNull.Value ? 0 : Convert.ToInt32(second_loop.Rows[j]["AssignedUserID"]);
                                            user.AssignUserLoginTime = Convert.ToString(second_loop.Rows[j]["AssignUserLoginTime"]);

                                            user.AbsentUserID = second_loop.Rows[j]["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(second_loop.Rows[j]["UserID"]);//New
                                            user.OperatorExist = Convert.ToBoolean(second_loop.Rows[j]["OperatorExist"]);

                                            list.Add(user);
                                            users_obj.userslist = list;
                                        }
                                        else if (Convert.ToString(second_loop.Rows[j]["AssignedUserName"]) != "" && Convert.ToString(second_loop.Rows[j]["AssignedUserID"]) != "" && Convert.ToString(second_loop.Rows[j]["LoginTime"]) == "")//login time block add
                                        {
                                            user = new User();
                                            user.UserName = Convert.ToString(second_loop.Rows[j]["AssignedUserName"]);
                                            user.UserID = second_loop.Rows[j]["AssignedUserID"] == DBNull.Value ? 0 : Convert.ToInt32(second_loop.Rows[j]["AssignedUserID"]);
                                            user.AssignUserLoginTime = Convert.ToString(second_loop.Rows[j]["AssignUserLoginTime"]);

                                            user.AbsentUserID = second_loop.Rows[j]["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(second_loop.Rows[j]["UserID"]);//New
                                            user.OperatorExist = Convert.ToBoolean(second_loop.Rows[j]["OperatorExist"]);

                                            list.Add(user);
                                            users_obj.userslist = list;
                                        }
                                        users_obj.AssignedUserID = second_loop.Rows[j]["AssignedUserID"] == DBNull.Value ? 0 : Convert.ToInt32(second_loop.Rows[j]["AssignedUserID"]);
                                        users_obj.AssignedUserName = Convert.ToString(second_loop.Rows[j]["AssignedUserName"]);

                                        users_obj.AssignUserLoginTime = Convert.ToString(second_loop.Rows[j]["AssignUserLoginTime"]);

                                        users_obj.AbsentUserID = second_loop.Rows[j]["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(second_loop.Rows[j]["UserID"]);//New
                                        users_obj.OperatorExist = Convert.ToBoolean(second_loop.Rows[j]["OperatorExist"]);

                                        userslocationList.Add(users_obj);
                                    }//New
                                    else//New
                                    {//New
                                        int index1 = userslocationList.FindIndex(t => (t.LocationParkingLotID == Convert.ToInt32(second_loop.Rows[j]["LocationParkingLotID"])));

                                        if (Convert.ToString(second_loop.Rows[j]["AssignedUserName"]) != "" && Convert.ToString(second_loop.Rows[j]["AssignedUserID"]) != "" && Convert.ToString(second_loop.Rows[j]["LoginTime"]) != "")//login time block add
                                        {

                                            user = new User();
                                            user.UserName = Convert.ToString(second_loop.Rows[j]["AssignedUserName"]);
                                            user.UserID = second_loop.Rows[j]["AssignedUserID"] == DBNull.Value ? 0 : Convert.ToInt32(second_loop.Rows[j]["AssignedUserID"]);
                                            user.AssignUserLoginTime = Convert.ToString(second_loop.Rows[j]["AssignUserLoginTime"]);

                                            user.AbsentUserID = second_loop.Rows[j]["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(second_loop.Rows[j]["UserID"]);//New
                                            user.OperatorExist = Convert.ToBoolean(second_loop.Rows[j]["OperatorExist"]);

                                            list.Add(user);
                                            users_obj.userslist = list;
                                            userslocationList[index1].userslist.Add(user);
                                        }
                                        else if (Convert.ToString(second_loop.Rows[j]["AssignedUserName"]) != "" && Convert.ToString(second_loop.Rows[j]["AssignedUserID"]) != "" && Convert.ToString(second_loop.Rows[j]["LoginTime"]) == "")//login time block add
                                        {
                                            user = new User();
                                            user.UserName = Convert.ToString(second_loop.Rows[j]["AssignedUserName"]);
                                            user.UserID = second_loop.Rows[j]["AssignedUserID"] == DBNull.Value ? 0 : Convert.ToInt32(second_loop.Rows[j]["AssignedUserID"]);
                                            user.AssignUserLoginTime = Convert.ToString(second_loop.Rows[j]["AssignUserLoginTime"]);

                                            user.AbsentUserID = second_loop.Rows[j]["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(second_loop.Rows[j]["UserID"]);//New
                                            user.OperatorExist = Convert.ToBoolean(second_loop.Rows[j]["OperatorExist"]);

                                            list.Add(user);
                                            users_obj.userslist = list;
                                            userslocationList[index1].userslist.Add(user);
                                        }
                                        else
                                        {
                                            user = new User();
                                            user.UserName = Convert.ToString(second_loop.Rows[j]["UserName"]);
                                            user.UserID = second_loop.Rows[j]["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(second_loop.Rows[j]["UserID"]);
                                            user.AssignUserLoginTime = Convert.ToString(second_loop.Rows[j]["LoginTime"]);

                                            user.AbsentUserID = second_loop.Rows[j]["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(second_loop.Rows[j]["UserID"]);//New
                                            user.OperatorExist = Convert.ToBoolean(second_loop.Rows[j]["OperatorExist"]);

                                            list.Add(user);
                                            users_obj.userslist = list;
                                            userslocationList[index1].userslist.Add(user);
                                        }

                                        //02122020
                                        //  if (!userslocationList.Any(t => (t.UserID == Convert.ToInt32(second_loop.Rows[j]["UserID"])
                                        //&& t.LocationParkingLotID == Convert.ToInt32(second_loop.Rows[j]["LocationParkingLotID"])
                                        //)))
                                        //  {                                      
                                        //      user = new User();
                                        //      user.UserName = Convert.ToString(second_loop.Rows[j]["UserName"]);
                                        //      user.UserID = second_loop.Rows[j]["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(second_loop.Rows[j]["UserID"]);
                                        //      user.AssignUserLoginTime = Convert.ToString(second_loop.Rows[j]["LoginTime"]);

                                        //      user.AbsentUserID = second_loop.Rows[j]["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(second_loop.Rows[j]["UserID"]);//New

                                        //      list.Add(user);
                                        //      users_obj.userslist = list;
                                        //      userslocationList[index1].userslist.Add(user);
                                        //  }
                                        //02122020
                                    }//New


                                }
                                else
                                {
                                    if (Convert.ToString(second_loop.Rows[j]["AssignedUserName"]) != "" && Convert.ToString(second_loop.Rows[j]["AssignedUserID"]) != "" && Convert.ToString(second_loop.Rows[j]["LoginTime"]) != "")//login time block add
                                    {
                                        int index = userslocationList.FindIndex(t => (t.UserID == Convert.ToInt32(second_loop.Rows[j]["UserID"])
                                                                && t.LocationParkingLotID == Convert.ToInt32(second_loop.Rows[j]["LocationParkingLotID"])));
                                        user = new User();
                                        user.UserName = Convert.ToString(second_loop.Rows[j]["AssignedUserName"]);
                                        user.UserID = second_loop.Rows[j]["AssignedUserID"] == DBNull.Value ? 0 : Convert.ToInt32(second_loop.Rows[j]["AssignedUserID"]);
                                        user.AssignUserLoginTime = Convert.ToString(second_loop.Rows[j]["AssignUserLoginTime"]);
                                        user.OperatorExist = Convert.ToBoolean(second_loop.Rows[j]["OperatorExist"]);
                                        list.Add(user);
                                        users_obj.userslist = list;
                                        userslocationList[index].userslist.Add(user);
                                    }
                                    else if (Convert.ToString(second_loop.Rows[j]["AssignedUserName"]) != "" && Convert.ToString(second_loop.Rows[j]["AssignedUserID"]) != "" && Convert.ToString(second_loop.Rows[j]["LoginTime"]) == "")//login time block add
                                    {
                                        int index = userslocationList.FindIndex(t => (t.UserID == Convert.ToInt32(second_loop.Rows[j]["UserID"])
                                                                && t.LocationParkingLotID == Convert.ToInt32(second_loop.Rows[j]["LocationParkingLotID"])));
                                        user = new User();
                                        user.UserName = Convert.ToString(second_loop.Rows[j]["AssignedUserName"]);
                                        user.UserID = second_loop.Rows[j]["AssignedUserID"] == DBNull.Value ? 0 : Convert.ToInt32(second_loop.Rows[j]["AssignedUserID"]);
                                        user.AssignUserLoginTime = Convert.ToString(second_loop.Rows[j]["AssignUserLoginTime"]);
                                        user.OperatorExist = Convert.ToBoolean(second_loop.Rows[j]["OperatorExist"]);
                                        list.Add(user);
                                        users_obj.userslist = list;
                                        userslocationList[index].userslist.Add(user);

                                        //02122020
                                        // if (!userslocationList.Any(t => (t.UserID == Convert.ToInt32(second_loop.Rows[j]["UserID"])
                                        //&& t.LocationParkingLotID == Convert.ToInt32(second_loop.Rows[j]["LocationParkingLotID"])
                                        //)))
                                        // {
                                        //     int index11 = userslocationList.FindIndex(t => (t.UserID == Convert.ToInt32(second_loop.Rows[j]["UserID"])
                                        //                         && t.LocationParkingLotID == Convert.ToInt32(second_loop.Rows[j]["LocationParkingLotID"])));
                                        //     user = new User();
                                        //     user.UserName = Convert.ToString(second_loop.Rows[j]["UserName"]);
                                        //     user.UserID = second_loop.Rows[j]["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(second_loop.Rows[j]["UserID"]);
                                        //     user.AssignUserLoginTime = Convert.ToString(second_loop.Rows[j]["LoginTime"]);

                                        //     user.AbsentUserID = second_loop.Rows[j]["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(second_loop.Rows[j]["UserID"]);//New

                                        //     list.Add(user);
                                        //     users_obj.userslist = list;
                                        //     userslocationList[index11].userslist.Add(user);
                                        // }
                                        //02122020
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                            }//031220
                            else if (!userslocationList.Any(t => (t.LocationParkingLotID == Convert.ToInt32(second_loop.Rows[j]["LocationParkingLotID"]))))//031220
                            {//031220


                                users_obj.LocationID = Convert.ToInt32(second_loop.Rows[j]["LocationID"]);
                                users_obj.LocationParkingLotID = Convert.ToInt32(second_loop.Rows[j]["LocationParkingLotID"]);
                                users_obj.LocationName = Convert.ToString(second_loop.Rows[j]["LocationName"]);
                                users_obj.LocationParkingLotName = Convert.ToString(second_loop.Rows[j]["LocationParkingLotName"]);
                                users_obj.LoginTime = Convert.ToString(second_loop.Rows[j]["LoginTime"]);
                                users_obj.OperatorExist = false;
                                userslocationList.Add(users_obj);
                            }//031220
                        }
                    }
                }

                #region New Code

                for (int y = 0; y < userslocationList.Count; y++)
                {
                    userslocationList[y].userslist = userslocationList[y].userslist.GroupBy(x => x.UserID).Select(x => x.First()).ToList();
                }
                #endregion
                return userslocationList;
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

                    using (SqlCommand sqlcmd_getusers_obj = new SqlCommand("PARK_PROC_GetOperatorsBySupervisorID", sqlconn_obj))
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
        public IList<Locations> GetLocationsofSupervisor(int supervisorID, string UserType)
        {
            IList<Locations> locationsList = new List<Locations>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getusers_obj = new SqlCommand("PARK_PROC_GetLocationBySupervisor", sqlconn_obj))
                    {
                        sqlcmd_getusers_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getusers_obj.CommandTimeout = 0;
                        sqlcmd_getusers_obj.Parameters.AddWithValue("@SupervisorID", supervisorID);
                        sqlcmd_getusers_obj.Parameters.AddWithValue("@UserType", UserType);
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getusers_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            Locations locations_obj = new Locations();
                            locations_obj.LocationID = Convert.ToInt32(dt.Rows[i]["LocationID"]);
                            locations_obj.LocationName = Convert.ToString(dt.Rows[i]["LocationName"]);
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
        public IList<Lots> GetLotsByLocationofSupervisor(int LocationID, int supervisorID, string UserType)
        {
            IList<Lots> lotsList = new List<Lots>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_GetLotByLocationBySupervisor", sqlconn_obj))
                    {
                        sqlcmd_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_obj.CommandTimeout = 0;
                        sqlcmd_obj.Parameters.AddWithValue("@LocationID", LocationID);
                        sqlcmd_obj.Parameters.AddWithValue("@SupervisorID", supervisorID);
                        sqlcmd_obj.Parameters.AddWithValue("@UserType", UserType);
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_obj))
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
        public List<UserLocationMapper> GetOperatorsLoginStatusByLocationandLot(UserLocationMapper SearchData, int supervisorID, string UserType)
        {
            List<UserLocationMapper> userslocationList = new List<UserLocationMapper>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getusers_obj = new SqlCommand("PARK_PROC_GetAssignOperatorsForSupervisorByLocation", sqlconn_obj))
                    {
                        sqlcmd_getusers_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getusers_obj.CommandTimeout = 0;
                        sqlcmd_getusers_obj.Parameters.AddWithValue("@LocationID", Convert.ToInt32(SearchData.LocationID));
                        if (Convert.ToString(SearchData.LocationParkingLotID) != "" && SearchData.LocationParkingLotID != 0)
                        {
                            sqlcmd_getusers_obj.Parameters.AddWithValue("@LocationParkingLotID", Convert.ToInt32(SearchData.LocationParkingLotID));
                        }
                        else
                        {
                            sqlcmd_getusers_obj.Parameters.AddWithValue("@LocationParkingLotID", DBNull.Value);
                        }
                        sqlcmd_getusers_obj.Parameters.AddWithValue("@SupervisorID", supervisorID);
                        sqlcmd_getusers_obj.Parameters.AddWithValue("@UserType", UserType);
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getusers_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        #region New Code
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            UserLocationMapper users_obj = new UserLocationMapper();
                            User user;
                            List<User> list = new List<User>();

                            if (dt.Rows[j]["UserID"] != null && dt.Rows[j]["UserID"] != DBNull.Value)//031220
                            {//031220
                                if (!userslocationList.Any(t => (t.UserID == Convert.ToInt32(dt.Rows[j]["UserID"])
                                    && t.LocationParkingLotID == Convert.ToInt32(dt.Rows[j]["LocationParkingLotID"]))))
                                {

                                    if (!userslocationList.Any(t => t.LocationParkingLotID == Convert.ToInt32(dt.Rows[j]["LocationParkingLotID"])))//New
                                    {//New

                                        users_obj.UserID = Convert.ToInt32(dt.Rows[j]["UserID"]);
                                        users_obj.LocationID = Convert.ToInt32(dt.Rows[j]["LocationID"]);
                                        users_obj.LocationParkingLotID = Convert.ToInt32(dt.Rows[j]["LocationParkingLotID"]);
                                        users_obj.UserName = Convert.ToString(dt.Rows[j]["UserName"]);
                                        users_obj.LocationName = Convert.ToString(dt.Rows[j]["LocationName"]);
                                        users_obj.LocationParkingLotName = Convert.ToString(dt.Rows[j]["LocationParkingLotName"]);
                                        users_obj.LoginTime = Convert.ToString(dt.Rows[j]["LoginTime"]);

                                        if (Convert.ToString(dt.Rows[j]["AssignedUserName"]) != "" && Convert.ToString(dt.Rows[j]["AssignedUserID"]) != "" && Convert.ToString(dt.Rows[j]["LoginTime"]) != "")//login time block add
                                        {
                                            user = new User();
                                            user.UserName = Convert.ToString(dt.Rows[j]["AssignedUserName"]);
                                            user.UserID = dt.Rows[j]["AssignedUserID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[j]["AssignedUserID"]);
                                            user.AssignUserLoginTime = Convert.ToString(dt.Rows[j]["AssignUserLoginTime"]);

                                            user.AbsentUserID = dt.Rows[j]["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[j]["UserID"]);//New
                                            user.OperatorExist = Convert.ToBoolean(dt.Rows[j]["OperatorExist"]);


                                            list.Add(user);
                                            users_obj.userslist = list;
                                        }
                                        else if (Convert.ToString(dt.Rows[j]["AssignedUserName"]) != "" && Convert.ToString(dt.Rows[j]["AssignedUserID"]) != "" && Convert.ToString(dt.Rows[j]["LoginTime"]) == "")//login time block add
                                        {
                                            user = new User();
                                            user.UserName = Convert.ToString(dt.Rows[j]["AssignedUserName"]);
                                            user.UserID = dt.Rows[j]["AssignedUserID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[j]["AssignedUserID"]);
                                            user.AssignUserLoginTime = Convert.ToString(dt.Rows[j]["AssignUserLoginTime"]);

                                            user.AbsentUserID = dt.Rows[j]["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[j]["UserID"]);//New
                                            user.OperatorExist = Convert.ToBoolean(dt.Rows[j]["OperatorExist"]);

                                            list.Add(user);
                                            users_obj.userslist = list;
                                        }
                                        users_obj.AssignedUserID = dt.Rows[j]["AssignedUserID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[j]["AssignedUserID"]);
                                        users_obj.AssignedUserName = Convert.ToString(dt.Rows[j]["AssignedUserName"]);

                                        users_obj.AssignUserLoginTime = Convert.ToString(dt.Rows[j]["AssignUserLoginTime"]);

                                        users_obj.AbsentUserID = dt.Rows[j]["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[j]["UserID"]);//New
                                        users_obj.OperatorExist = Convert.ToBoolean(dt.Rows[j]["OperatorExist"]);

                                        userslocationList.Add(users_obj);
                                    }//New
                                    else//New
                                    {//New
                                        int index1 = userslocationList.FindIndex(t => (t.LocationParkingLotID == Convert.ToInt32(dt.Rows[j]["LocationParkingLotID"])));

                                        if (Convert.ToString(dt.Rows[j]["AssignedUserName"]) != "" && Convert.ToString(dt.Rows[j]["AssignedUserID"]) != "" && Convert.ToString(dt.Rows[j]["LoginTime"]) != "")//login time block add
                                        {

                                            user = new User();
                                            user.UserName = Convert.ToString(dt.Rows[j]["AssignedUserName"]);
                                            user.UserID = dt.Rows[j]["AssignedUserID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[j]["AssignedUserID"]);
                                            user.AssignUserLoginTime = Convert.ToString(dt.Rows[j]["AssignUserLoginTime"]);

                                            user.AbsentUserID = dt.Rows[j]["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[j]["UserID"]);//New
                                            user.OperatorExist = Convert.ToBoolean(dt.Rows[j]["OperatorExist"]);

                                            list.Add(user);
                                            users_obj.userslist = list;
                                            userslocationList[index1].userslist.Add(user);
                                        }
                                        else if (Convert.ToString(dt.Rows[j]["AssignedUserName"]) != "" && Convert.ToString(dt.Rows[j]["AssignedUserID"]) != "" && Convert.ToString(dt.Rows[j]["LoginTime"]) == "")//login time block add
                                        {
                                            user = new User();
                                            user.UserName = Convert.ToString(dt.Rows[j]["AssignedUserName"]);
                                            user.UserID = dt.Rows[j]["AssignedUserID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[j]["AssignedUserID"]);
                                            user.AssignUserLoginTime = Convert.ToString(dt.Rows[j]["AssignUserLoginTime"]);

                                            user.AbsentUserID = dt.Rows[j]["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[j]["UserID"]);//New
                                            user.OperatorExist = Convert.ToBoolean(dt.Rows[j]["OperatorExist"]);

                                            list.Add(user);
                                            users_obj.userslist = list;
                                            userslocationList[index1].userslist.Add(user);
                                        }
                                        else
                                        {
                                            user = new User();
                                            user.UserName = Convert.ToString(dt.Rows[j]["UserName"]);
                                            user.UserID = dt.Rows[j]["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[j]["UserID"]);
                                            user.AssignUserLoginTime = Convert.ToString(dt.Rows[j]["LoginTime"]);

                                            user.AbsentUserID = dt.Rows[j]["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[j]["UserID"]);//New
                                            user.OperatorExist = Convert.ToBoolean(dt.Rows[j]["OperatorExist"]);

                                            list.Add(user);
                                            users_obj.userslist = list;
                                            userslocationList[index1].userslist.Add(user);
                                        }

                                    }//New


                                }
                                else
                                {
                                    if (Convert.ToString(dt.Rows[j]["AssignedUserName"]) != "" && Convert.ToString(dt.Rows[j]["AssignedUserID"]) != "" && Convert.ToString(dt.Rows[j]["LoginTime"]) != "")//login time block add
                                    {
                                        int index = userslocationList.FindIndex(t => (t.UserID == Convert.ToInt32(dt.Rows[j]["UserID"])
                                                                && t.LocationParkingLotID == Convert.ToInt32(dt.Rows[j]["LocationParkingLotID"])));
                                        user = new User();
                                        user.UserName = Convert.ToString(dt.Rows[j]["AssignedUserName"]);
                                        user.UserID = dt.Rows[j]["AssignedUserID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[j]["AssignedUserID"]);
                                        user.AssignUserLoginTime = Convert.ToString(dt.Rows[j]["AssignUserLoginTime"]);
                                        user.OperatorExist = Convert.ToBoolean(dt.Rows[j]["OperatorExist"]);
                                        list.Add(user);
                                        users_obj.userslist = list;
                                        userslocationList[index].userslist.Add(user);
                                    }
                                    else if (Convert.ToString(dt.Rows[j]["AssignedUserName"]) != "" && Convert.ToString(dt.Rows[j]["AssignedUserID"]) != "" && Convert.ToString(dt.Rows[j]["LoginTime"]) == "")//login time block add
                                    {
                                        int index = userslocationList.FindIndex(t => (t.UserID == Convert.ToInt32(dt.Rows[j]["UserID"])
                                                                && t.LocationParkingLotID == Convert.ToInt32(dt.Rows[j]["LocationParkingLotID"])));
                                        user = new User();
                                        user.UserName = Convert.ToString(dt.Rows[j]["AssignedUserName"]);
                                        user.UserID = dt.Rows[j]["AssignedUserID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[j]["AssignedUserID"]);
                                        user.AssignUserLoginTime = Convert.ToString(dt.Rows[j]["AssignUserLoginTime"]);
                                        user.OperatorExist = Convert.ToBoolean(dt.Rows[j]["OperatorExist"]);
                                        list.Add(user);
                                        users_obj.userslist = list;
                                        userslocationList[index].userslist.Add(user);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }//031220
                            else if (!userslocationList.Any(t => (t.LocationParkingLotID == Convert.ToInt32(dt.Rows[j]["LocationParkingLotID"]))))//031220
                            {//031220


                                users_obj.LocationID = Convert.ToInt32(dt.Rows[j]["LocationID"]);
                                users_obj.LocationParkingLotID = Convert.ToInt32(dt.Rows[j]["LocationParkingLotID"]);
                                users_obj.LocationName = Convert.ToString(dt.Rows[j]["LocationName"]);
                                users_obj.LocationParkingLotName = Convert.ToString(dt.Rows[j]["LocationParkingLotName"]);
                                users_obj.LoginTime = Convert.ToString(dt.Rows[j]["LoginTime"]);
                                users_obj.OperatorExist = false;
                                userslocationList.Add(users_obj);
                            }//031220
                        }
                        #endregion

                        #region Old Code
                        //for (int j = 0; j < dt.Rows.Count; j++)
                        //{
                        //    UserLocationMapper users_obj = new UserLocationMapper();
                        //    User user;
                        //    List<User> list = new List<User>();

                        //    if (!userslocationList.Any(t => (t.UserID == Convert.ToInt32(dt.Rows[j]["UserID"])
                        //            && t.LocationParkingLotID == Convert.ToInt32(dt.Rows[j]["LocationParkingLotID"]))))
                        //    {
                        //        users_obj.UserID = Convert.ToInt32(dt.Rows[j]["UserID"]);
                        //        users_obj.LocationID = Convert.ToInt32(dt.Rows[j]["LocationID"]);
                        //        users_obj.LocationParkingLotID = Convert.ToInt32(dt.Rows[j]["LocationParkingLotID"]);
                        //        users_obj.UserName = Convert.ToString(dt.Rows[j]["UserName"]);
                        //        users_obj.LocationName = Convert.ToString(dt.Rows[j]["LocationName"]);
                        //        users_obj.LocationParkingLotName = Convert.ToString(dt.Rows[j]["LocationParkingLotName"]);
                        //        users_obj.LoginTime = Convert.ToString(dt.Rows[j]["LoginTime"]);

                        //        if (Convert.ToString(dt.Rows[j]["AssignedUserName"]) != "" && Convert.ToString(dt.Rows[j]["AssignedUserID"]) != "")
                        //        {
                        //            user = new User();
                        //            user.UserName = Convert.ToString(dt.Rows[j]["AssignedUserName"]);
                        //            user.UserID = dt.Rows[j]["AssignedUserID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[j]["AssignedUserID"]);
                        //            user.AssignUserLoginTime = Convert.ToString(dt.Rows[j]["AssignUserLoginTime"]);
                        //            list.Add(user);
                        //            users_obj.userslist = list;
                        //        }
                        //        users_obj.AssignedUserID = dt.Rows[j]["AssignedUserID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[j]["AssignedUserID"]);
                        //        users_obj.AssignedUserName = Convert.ToString(dt.Rows[j]["AssignedUserName"]);

                        //        users_obj.AssignUserLoginTime = Convert.ToString(dt.Rows[j]["AssignUserLoginTime"]);
                        //        userslocationList.Add(users_obj);
                        //    }
                        //    else
                        //    {
                        //        if (Convert.ToString(dt.Rows[j]["AssignedUserName"]) != "" && Convert.ToString(dt.Rows[j]["AssignedUserID"]) != "")
                        //        {
                        //            int index = userslocationList.FindIndex(t => (t.UserID == Convert.ToInt32(dt.Rows[j]["UserID"])
                        //                                    && t.LocationParkingLotID == Convert.ToInt32(dt.Rows[j]["LocationParkingLotID"])));
                        //            user = new User();
                        //            user.UserName = Convert.ToString(dt.Rows[j]["AssignedUserName"]);
                        //            user.UserID = dt.Rows[j]["AssignedUserID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[j]["AssignedUserID"]);
                        //            user.AssignUserLoginTime = Convert.ToString(dt.Rows[j]["AssignUserLoginTime"]);
                        //            list.Add(user);
                        //            users_obj.userslist = list;
                        //            userslocationList[index].userslist.Add(user);
                        //        }
                        //        else
                        //        {
                        //            break;
                        //        }
                        //    }
                        //}
                        #endregion
                    }
                }
                for (int y = 0; y < userslocationList.Count; y++)
                {
                    userslocationList[y].userslist = userslocationList[y].userslist.GroupBy(x => x.UserID).Select(x => x.First()).ToList();
                }
                return userslocationList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public string AssignOperatortoLot(int operatorID, UserLocationMapper data, int supervisorID, string CreatedBy,int absentUserID)
        public string AssignOperatortoLot(string operatorID, UserLocationMapper data, int supervisorID, string CreatedBy, int absentUserID)
        {
            sqlhelper_obj = new SqlHelper();
            string status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_AssignLottoOperatorBySupervisor", sqlconn_obj))
                    {
                        //PARK_PROC_AssignLottoOperatorBySupervisor
                        sqlcmd_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_obj.CommandTimeout = 0;
                        sqlcmd_obj.Parameters.AddWithValue("@UserID", operatorID);
                        sqlcmd_obj.Parameters.AddWithValue("@LocationID", data.LocationID);
                        sqlcmd_obj.Parameters.AddWithValue("@LocationParkingLotID", data.LocationParkingLotID);
                        sqlcmd_obj.Parameters.AddWithValue("@SupervisorID", supervisorID);
                        sqlcmd_obj.Parameters.AddWithValue("@CreatedBy", CreatedBy);
                        sqlcmd_obj.Parameters.AddWithValue("@AbsentUserID", absentUserID);
                        sqlconn_obj.Open();
                        int result = sqlcmd_obj.ExecuteNonQuery();
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

        public string AddOperatortoLot(string operatorID, UserLocationMapper data, int supervisorID, string CreatedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_AddLottoOperatorBySupervisor", sqlconn_obj))
                    {
                        sqlcmd_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_obj.CommandTimeout = 0;
                        sqlcmd_obj.Parameters.AddWithValue("@UserID", operatorID);
                        sqlcmd_obj.Parameters.AddWithValue("@LocationID", data.LocationID);
                        sqlcmd_obj.Parameters.AddWithValue("@LocationParkingLotID", data.LocationParkingLotID);
                        sqlcmd_obj.Parameters.AddWithValue("@SupervisorID", supervisorID);
                        sqlcmd_obj.Parameters.AddWithValue("@CreatedBy", CreatedBy);
                        sqlconn_obj.Open();
                        int result = sqlcmd_obj.ExecuteNonQuery();
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
    }
}
