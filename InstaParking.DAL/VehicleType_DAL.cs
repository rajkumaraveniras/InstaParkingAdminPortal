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
    public class VehicleType_DAL
    {
        SqlHelper sqlhelper_obj;
        public IList<VehicleType> GetVehicleTypeList()
        {
            IList<VehicleType> vehicleTypeList = new List<VehicleType>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getvehicleType_obj = new SqlCommand("PARK_PROC_GetAllVehicleTypes", sqlconn_obj))
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
                            vehicleType_obj.VehicleTypeCode = Convert.ToString(dt.Rows[i]["VehicleTypeCode"]);
                            vehicleType_obj.VehicleTypeName = Convert.ToString(dt.Rows[i]["VehicleTypeName"]);
                            vehicleType_obj.WheelCount = Convert.ToInt32(dt.Rows[i]["WheelCount"]);
                            vehicleType_obj.AxleCount = Convert.ToInt32(dt.Rows[i]["AxleCount"]);
                            vehicleType_obj.VehicleTypeDesc = Convert.ToString(dt.Rows[i]["VehicleTypeDesc"]);
                            vehicleType_obj.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            vehicleType_obj.UpdatedOn = dt.Rows[i]["UpdatedOn"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(dt.Rows[i]["UpdatedOn"]);
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
        public int InsertAndUpdateVehicleType(VehicleType vehicleType_data, string CreatedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string vehicleType_data_status = string.Empty;
            int result;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SaveVehicleType", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        if (vehicleType_data.VehicleTypeID.ToString() != "" && vehicleType_data.VehicleTypeID != 0)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Update");
                            sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", vehicleType_data.VehicleTypeID);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Insert");
                            sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", DBNull.Value);
                        }

                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeCode", String.IsNullOrEmpty(vehicleType_data.VehicleTypeCode) ? (object)DBNull.Value : vehicleType_data.VehicleTypeCode.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeName", String.IsNullOrEmpty(vehicleType_data.VehicleTypeName) ? (object)DBNull.Value : vehicleType_data.VehicleTypeName.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeDesc", String.IsNullOrEmpty(vehicleType_data.VehicleTypeDesc) ? (object)DBNull.Value : vehicleType_data.VehicleTypeDesc.Trim());

                        sqlcmd_details_obj.Parameters.AddWithValue("@UpdatedBy", CreatedBy);
                        if (vehicleType_data.IsActive.ToString() == "")
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", true);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", Convert.ToBoolean(vehicleType_data.IsActive));
                        }
                        sqlcmd_details_obj.Parameters.AddWithValue("@WheelCount", vehicleType_data.WheelCount);
                        sqlcmd_details_obj.Parameters.AddWithValue("@AxleCount", vehicleType_data.AxleCount);

                        sqlcmd_details_obj.Parameters.Add("@Output_identity", SqlDbType.Int).Direction = ParameterDirection.Output;

                        sqlconn_obj.Open();
                        int res = sqlcmd_details_obj.ExecuteNonQuery();
                        result = Convert.ToInt32(sqlcmd_details_obj.Parameters["@Output_identity"].Value);
                        sqlconn_obj.Close();
                        //if (result > 0)
                        //{
                        //    vehicleType_data_status = "Success";
                        //}
                        //else
                        //{
                        //    vehicleType_data_status = "Failed";
                        //}
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<VehicleType> ViewVehicleType(int VehicleTypeID)
        {
            sqlhelper_obj = new SqlHelper();
            List<VehicleType> vehicleType_List = new List<VehicleType>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_ViewVehicleType", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", VehicleTypeID);
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
                            VehicleType vehicletype_obj = new VehicleType();
                            vehicletype_obj.VehicleTypeID = Convert.ToInt32(row["VehicleTypeID"]);
                            vehicletype_obj.VehicleTypeCode = Convert.ToString(row["VehicleTypeCode"]);
                            vehicletype_obj.VehicleTypeName = Convert.ToString(row["VehicleTypeName"]);
                            vehicletype_obj.WheelCount = Convert.ToInt32(row["WheelCount"]);
                            vehicletype_obj.AxleCount = Convert.ToInt32(row["AxleCount"]);
                            vehicletype_obj.VehicleTypeDesc = Convert.ToString(row["VehicleTypeDesc"]);                            
                            vehicletype_obj.IsActive = Convert.ToBoolean(row["IsActive"]);
                            vehicletype_obj.VehicleTypeIcon = Convert.ToString(row["VehicleTypeIcon"]);
                            vehicleType_List.Add(vehicletype_obj);
                        }
                    }
                }
                return vehicleType_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string InsertVehicleTypeIconData(string IconName, VehicleType vehicleType)
        {
            sqlhelper_obj = new SqlHelper();
            string message = string.Empty;

            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_InsertVehicleTypeIcon", sqlconn_obj))
                    {

                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", vehicleType.VehicleTypeID);                       
                        sqlcmd_details_obj.Parameters.AddWithValue("@IconName", String.IsNullOrEmpty(IconName) ? (object)DBNull.Value : IconName.Trim());
                       
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
        public IList<VehicleType> GetListofActiveVehicleTypes()
        {
            IList<VehicleType> vehicleTypeList = new List<VehicleType>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getvehicleType_obj = new SqlCommand("PARK_PROC_GetActiveVehicleTypes", sqlconn_obj))
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
                            vehicleType_obj.VehicleTypeCode = Convert.ToString(dt.Rows[i]["VehicleTypeCode"]);
                            vehicleType_obj.selected = false;
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
        public IList<VehicleType> GetListofLocationVehicleTypesByID(int LocationID)
        {
            IList<VehicleType> vehicleTypeList = new List<VehicleType>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getvehicleType_obj = new SqlCommand("PARK_PROC_GetLocationVehicleTypesListByLocationID", sqlconn_obj))
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
                            VehicleType vehicleType_obj = new VehicleType();
                            vehicleType_obj.VehicleTypeID = Convert.ToInt32(dt.Rows[i]["VehicleTypeID"]);
                            vehicleType_obj.VehicleTypeCode = Convert.ToString(dt.Rows[i]["VehicleTypeCode"]);
                            vehicleType_obj.selected = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
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
        public IList<VehicleType> GetVehicleTypesByLotID(int LotID)
        {
            IList<VehicleType> vehicleTypeList = new List<VehicleType>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getvehicleType_obj = new SqlCommand("PARK_PROC_GetLotVehicleTypesListByLotID", sqlconn_obj))
                    {
                        sqlcmd_getvehicleType_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getvehicleType_obj.CommandTimeout = 0;
                        sqlcmd_getvehicleType_obj.Parameters.AddWithValue("@LotID", LotID);
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
                            vehicleType_obj.VehicleTypeCode = Convert.ToString(dt.Rows[i]["VehicleTypeCode"]);
                            vehicleType_obj.selected = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
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
    }
}
