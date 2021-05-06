using InstaParking.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace InstaParking.DAL
{
    public class PassType_DAL
    {
        SqlHelper sqlhelper_obj;

        #region PassType
        public IList<Passes> GetPassTypeList()
        {
            IList<Passes> passTypeList = new List<Passes>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getpassType_obj = new SqlCommand("PARK_PROC_GetAllPassTypes", sqlconn_obj))
                    {
                        sqlcmd_getpassType_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getpassType_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getpassType_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            Passes passType_obj = new Passes();
                            passType_obj.PassPriceID = Convert.ToInt32(dt.Rows[i]["PassPriceID"]);
                            passType_obj.PassTypeID = Convert.ToInt32(dt.Rows[i]["PassTypeID"]);                           
                            passType_obj.PassTypeName = Convert.ToString(dt.Rows[i]["PassTypeName"]);
                            passType_obj.PassCode = Convert.ToString(dt.Rows[i]["PassCode"]);
                            passType_obj.PassName = Convert.ToString(dt.Rows[i]["PassName"]);
                            passType_obj.StationAccess = Convert.ToString(dt.Rows[i]["StationAccess"]);
                            passType_obj.Duration = Convert.ToString(dt.Rows[i]["Duration"]);
                            //passType_obj.StartDate = Convert.ToString(dt.Rows[i]["StartDate"]);
                            //passType_obj.EndDate = Convert.ToString(dt.Rows[i]["EndDate"]);
                            passType_obj.StartDate = Convert.ToDateTime(dt.Rows[i]["StartDate"]);
                            passType_obj.EndDate = Convert.ToDateTime(dt.Rows[i]["EndDate"]);
                            //passType_obj.NFCApplicable = Convert.ToBoolean(dt.Rows[i]["NFCApplicable"]);
                            //passType_obj.NFCCardPrice = Convert.ToString(dt.Rows[i]["NFCCardPrice"]);
                            passType_obj.VehicleTypeID = Convert.ToInt32(dt.Rows[i]["VehicleTypeID"]);
                            passType_obj.VehicleTypeName = Convert.ToString(dt.Rows[i]["VehicleTypeName"]);
                            //passType_obj.StartRange = Convert.ToInt32(dt.Rows[i]["StartRange"]);
                            //passType_obj.EndRange = Convert.ToInt32(dt.Rows[i]["EndRange"]);
                            passType_obj.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            passType_obj.UpdatedOn = dt.Rows[i]["UpdatedOn"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(dt.Rows[i]["UpdatedOn"]);
                            passType_obj.Price = Convert.ToDecimal(dt.Rows[i]["Price"]);
                            passTypeList.Add(passType_obj);
                        }
                    }
                }
                return passTypeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string InsertAndUpdatePassType(Passes passType_data, string CreatedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string passType_data_status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SavePassType", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        if (passType_data.PassPriceID.ToString() != "" && passType_data.PassPriceID != 0)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Update");
                            sqlcmd_details_obj.Parameters.AddWithValue("@PassPriceID", passType_data.PassPriceID);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Insert");
                            sqlcmd_details_obj.Parameters.AddWithValue("@PassPriceID", DBNull.Value);
                        }
                        sqlcmd_details_obj.Parameters.AddWithValue("@PassTypeID", passType_data.PassTypeID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@PassCode", String.IsNullOrEmpty(passType_data.PassCode) ? (object)DBNull.Value : passType_data.PassCode.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@PassName", String.IsNullOrEmpty(passType_data.PassName) ? (object)DBNull.Value : passType_data.PassName.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@StationAccess", String.IsNullOrEmpty(passType_data.StationAccess) ? (object)DBNull.Value : passType_data.StationAccess.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@Duration", String.IsNullOrEmpty(passType_data.Duration) ? (object)DBNull.Value : passType_data.Duration.Trim());

                        if (String.IsNullOrEmpty(passType_data.StartDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StartDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StartDate", passType_data.StartDate);
                        }
                        if (String.IsNullOrEmpty(passType_data.EndDate.ToString()))
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@EndDate", DBNull.Value);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@EndDate", passType_data.EndDate);
                        }
                        //if (passType_data.NFCApplicable.ToString() == "")
                        //{
                        //    sqlcmd_details_obj.Parameters.AddWithValue("@NFCApplicable", true);
                        //}
                        //else
                        //{
                        //    sqlcmd_details_obj.Parameters.AddWithValue("@NFCApplicable", Convert.ToBoolean(passType_data.NFCApplicable));
                        //}

                        //sqlcmd_details_obj.Parameters.AddWithValue("@NFCCardPrice", String.IsNullOrEmpty(passType_data.NFCCardPrice) ? (object)DBNull.Value : passType_data.NFCCardPrice.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", passType_data.VehicleTypeID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@Price", String.IsNullOrEmpty(Convert.ToString(passType_data.Price)) ? (object)DBNull.Value : Convert.ToString(passType_data.Price).Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@PassDescription", String.IsNullOrEmpty(passType_data.PassDescription) ? (object)DBNull.Value : passType_data.PassDescription.Trim());

                        sqlcmd_details_obj.Parameters.AddWithValue("@UpdatedBy", CreatedBy);
                        if (passType_data.IsActive.ToString() == "")
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", true);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", Convert.ToBoolean(passType_data.IsActive));
                        }
                        sqlconn_obj.Open();
                        int result = sqlcmd_details_obj.ExecuteNonQuery();
                        sqlconn_obj.Close();
                        if (result > 0)
                        {
                            passType_data_status = "Success";
                        }
                        else
                        {
                            passType_data_status = "Failed";
                        }
                    }
                }
                return passType_data_status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Passes> ViewPassType(int PassPriceID)
        {
            sqlhelper_obj = new SqlHelper();
            List<Passes> passtype_List = new List<Passes>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_ViewPassType", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@PassPriceID", PassPriceID);
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
                            Passes passtype_obj = new Passes();
                            passtype_obj.PassPriceID = Convert.ToInt32(row["PassPriceID"]);
                            passtype_obj.PassTypeID = Convert.ToInt32(row["PassTypeID"]);
                            passtype_obj.PassCode = Convert.ToString(row["PassCode"]);
                            passtype_obj.PassName = Convert.ToString(row["PassName"]);
                            passtype_obj.StationAccess = Convert.ToString(row["StationAccess"]);
                            passtype_obj.Duration = Convert.ToString(row["Duration"]);
                            passtype_obj.StartDate = Convert.ToDateTime(row["StartDate"]);
                            passtype_obj.EndDate = Convert.ToDateTime(row["EndDate"]);
                            //passtype_obj.StartDate = Convert.ToString(row["StartDate"]);
                            //passtype_obj.EndDate = Convert.ToString(row["EndDate"]);

                            // passtype_obj.NFCApplicable = Convert.ToBoolean(row["NFCApplicable"]);
                            // passtype_obj.NFCCardPrice = Convert.ToString(row["NFCCardPrice"]);
                            passtype_obj.VehicleTypeID = Convert.ToInt32(row["VehicleTypeID"]);
                            passtype_obj.Price = row["Price"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Price"]);
                            passtype_obj.PassDescription = Convert.ToString(row["PassDescription"]);
                            passtype_obj.IsActive = Convert.ToBoolean(row["IsActive"]);
                            passtype_List.Add(passtype_obj);
                        }
                    }
                }
                return passtype_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<PassType> GetActivePassTypes()
        {
            IList<PassType> passTypeList = new List<PassType>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_getpassType_obj = new SqlCommand("PARK_PROC_GetActivePassTypes", sqlconn_obj))
                    {
                        sqlcmd_getpassType_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getpassType_obj.CommandTimeout = 0;
                        DataSet ds;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd_getpassType_obj))
                        {
                            ds = new DataSet();
                            da.Fill(ds);
                        }
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            PassType passType_obj = new PassType();
                            passType_obj.PassTypeID = Convert.ToInt32(dt.Rows[i]["PassTypeID"]);
                            passType_obj.PassTypeName = Convert.ToString(dt.Rows[i]["PassTypeName"]);                           
                            passTypeList.Add(passType_obj);
                        }
                    }
                }
                return passTypeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region PassSaleLimit
        public IList<PassSaleLimit> GetPassSaleLimitList()
        {
            IList<PassSaleLimit> passsaleLimitList = new List<PassSaleLimit>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_GetAllPassSaleLimits", sqlconn_obj))
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
                            PassSaleLimit pass_obj = new PassSaleLimit();
                            pass_obj.PassSaleLimitID = Convert.ToInt32(dt.Rows[i]["PassSaleLimitID"]);
                            pass_obj.PassTypeID = Convert.ToInt32(dt.Rows[i]["PassTypeID"]);
                            pass_obj.VehicleTypeID = Convert.ToInt32(dt.Rows[i]["VehicleTypeID"]);
                            pass_obj.LimitPercentage = Convert.ToInt32(dt.Rows[i]["LimitPercentage"]);
                            pass_obj.VehicleTypeName = Convert.ToString(dt.Rows[i]["VehicleTypeName"]);
                            pass_obj.PassTypeName = Convert.ToString(dt.Rows[i]["PassTypeName"]);
                            pass_obj.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            pass_obj.UpdatedOn = dt.Rows[i]["UpdatedOn"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(dt.Rows[i]["UpdatedOn"]);
                            passsaleLimitList.Add(pass_obj);
                        }
                    }
                }
                return passsaleLimitList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int RestrictPassSaleLimit(PassSaleLimit pass_data, string CreatedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string pass_data_status = string.Empty;
            int result;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SavePassSaleLimit", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        if (pass_data.PassSaleLimitID.ToString() != "" && pass_data.PassSaleLimitID != 0)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Update");
                            sqlcmd_details_obj.Parameters.AddWithValue("@PassSaleLimitID", pass_data.PassSaleLimitID);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Insert");
                            sqlcmd_details_obj.Parameters.AddWithValue("@PassSaleLimitID", DBNull.Value);
                        }
                        sqlcmd_details_obj.Parameters.AddWithValue("@PassTypeID", pass_data.PassTypeID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@VehicleTypeID", pass_data.VehicleTypeID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@LimitPercentage", String.IsNullOrEmpty(Convert.ToString(pass_data.LimitPercentage)) ? (object)DBNull.Value : Convert.ToString(pass_data.LimitPercentage).Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@UpdatedBy", Convert.ToInt32(CreatedBy));
                        if (pass_data.IsActive.ToString() == "")
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", true);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", Convert.ToBoolean(pass_data.IsActive));
                        }
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
        public List<PassSaleLimit> EditPassSaleLimit(int PassSaleLimitID)
        {
            sqlhelper_obj = new SqlHelper();
            List<PassSaleLimit> pass_List = new List<PassSaleLimit>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_GetPassSaleLimitByID", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@PassSaleLimitID", PassSaleLimitID);
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
                            PassSaleLimit pass_obj = new PassSaleLimit();
                            pass_obj.PassSaleLimitID = Convert.ToInt32(row["PassSaleLimitID"]);
                            pass_obj.PassTypeID = Convert.ToInt32(row["PassTypeID"]);
                            pass_obj.VehicleTypeID = Convert.ToInt32(row["VehicleTypeID"]);
                            pass_obj.LimitPercentage = Convert.ToInt32(row["LimitPercentage"]);
                            pass_obj.IsActive = Convert.ToBoolean(row["IsActive"]);
                            pass_List.Add(pass_obj);
                        }
                    }
                }
                return pass_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string DeletePassSaleLimit(int PassSaleLimitID, string DeletedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string data_status = string.Empty;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_DeletePassSaleLimitByID", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@PassSaleLimitID", PassSaleLimitID);
                        sqlcmd_details_obj.Parameters.AddWithValue("@DeletedBy", Convert.ToInt32(DeletedBy));
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
    }
}
