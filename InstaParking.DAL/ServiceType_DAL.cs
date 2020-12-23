using InstaParking.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace InstaParking.DAL
{
    public class ServiceType_DAL
    {
        SqlHelper sqlhelper_obj;
        GetLogo getlogo = new GetLogo();

        public IList<ServiceType> GetFeaturesList()
        {
            IList<ServiceType> featuresList = new List<ServiceType>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_GetAllServiceTypes", sqlconn_obj))
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
                            ServiceType feature_obj = new ServiceType();
                            feature_obj.ServiceTypeID = Convert.ToInt32(dt.Rows[i]["ServiceTypeID"]);
                            feature_obj.ServiceTypeName = Convert.ToString(dt.Rows[i]["ServiceTypeName"]);
                            feature_obj.ServiceTypeCode = Convert.ToString(dt.Rows[i]["ServiceTypeCode"]);
                            feature_obj.ServiceTypeDesc = Convert.ToString(dt.Rows[i]["ServiceTypeDesc"]);
                            feature_obj.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]);
                            feature_obj.UpdatedOn = dt.Rows[i]["UpdatedOn"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(dt.Rows[i]["UpdatedOn"]);

                            feature_obj.IconName = Convert.ToString(dt.Rows[i]["IconName"]);
                            //if (!String.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["ServiceTypeImage"])))
                            //{
                            //    feature_obj.ServiceTypeImage = getlogo.ShowServiceTypeIcon(Convert.ToString(dt.Rows[i]["ServiceTypeID"]), "Icon");
                            //}
                            //else
                            //{
                            //    feature_obj.ServiceTypeImage = "";
                            //}

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

        public Stream ShowServiceTypeIcons(int serviceTypeID, string name)
        {
            try
            {
                sqlhelper_obj = new SqlHelper();
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_getlogo = new SqlCommand("PARK_PROC_GetServiceTypeIcons", sqlconn_obj))
                    {
                        sqlconn_obj.Open();
                        sqlcmd_getlogo.CommandType = CommandType.StoredProcedure;
                        sqlcmd_getlogo.Parameters.AddWithValue("@ServiceTypeID", serviceTypeID);
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

        public int InsertAndUpdateServiceType(ServiceType features_data, string CreatedBy)
        {
            sqlhelper_obj = new SqlHelper();
            string features_data_status = string.Empty;
            int result;
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_SaveServiceType", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        if (features_data.ServiceTypeID.ToString() != "" && features_data.ServiceTypeID != 0)
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Update");
                            sqlcmd_details_obj.Parameters.AddWithValue("@ServiceTypeID", features_data.ServiceTypeID);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@StmtType", "Insert");
                            sqlcmd_details_obj.Parameters.AddWithValue("@ServiceTypeID", DBNull.Value);
                        }

                        //sqlcmd_details_obj.Parameters.AddWithValue("@ServiceTypeCode", String.IsNullOrEmpty(features_data.UserTypeCode) ? (object)DBNull.Value : features_data.UserTypeCode.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@ServiceTypeName", String.IsNullOrEmpty(features_data.ServiceTypeName) ? (object)DBNull.Value : features_data.ServiceTypeName.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@ServiceTypeDesc", String.IsNullOrEmpty(features_data.ServiceTypeDesc) ? (object)DBNull.Value : features_data.ServiceTypeDesc.Trim());
                        sqlcmd_details_obj.Parameters.AddWithValue("@UpdatedBy", CreatedBy);
                        if (features_data.IsActive.ToString() == "")
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", true);
                        }
                        else
                        {
                            sqlcmd_details_obj.Parameters.AddWithValue("@IsActive", Convert.ToBoolean(features_data.IsActive));
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

        public string InsertServiceTypeIconData(string IconName, ServiceType serviceType)
        {
            sqlhelper_obj = new SqlHelper();
            string message = string.Empty;

            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_InsertServiceTypeIcon", sqlconn_obj))
                    {

                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.Parameters.AddWithValue("@ServiceTypeID", serviceType.ServiceTypeID);
                        //if (IconData.ToString() != "" && IconData.ToString() != null)
                        //{
                        //    sqlcmd_details_obj.Parameters.AddWithValue("@ServiceTypeImage", IconData);
                        //}
                        //else
                        //{
                        //    sqlcmd_details_obj.Parameters.AddWithValue("@ServiceTypeImage", DBNull.Value);
                        //}
                        sqlcmd_details_obj.Parameters.AddWithValue("@IconName", String.IsNullOrEmpty(IconName) ? (object)DBNull.Value : IconName.Trim());
                        //sqlcmd_details_obj.Parameters.AddWithValue("@IconType", String.IsNullOrEmpty(IconType) ? (object)DBNull.Value : IconType.Trim());

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

        public List<ServiceType> ViewServiceType(int serviceTypeID)
        {
            sqlhelper_obj = new SqlHelper();
            List<ServiceType> serviceType_List = new List<ServiceType>();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();
                    using (SqlCommand sqlcmd_details_obj = new SqlCommand("PARK_PROC_ViewServiceTyeDetails", sqlconn_obj))
                    {
                        sqlcmd_details_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_details_obj.CommandTimeout = 0;
                        sqlcmd_details_obj.Parameters.AddWithValue("@ServiceTypeID", serviceTypeID);
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
                            ServiceType serviceType_obj = new ServiceType();
                            serviceType_obj.ServiceTypeID = Convert.ToInt32(row["ServiceTypeID"]);
                            serviceType_obj.ServiceTypeName = Convert.ToString(row["ServiceTypeName"]);
                            serviceType_obj.ServiceTypeDesc = Convert.ToString(row["ServiceTypeDesc"]);
                            serviceType_obj.IsActive = Convert.ToBoolean(row["IsActive"]);

                            serviceType_obj.IconName = Convert.ToString(row["IconName"]);
                            //if (!String.IsNullOrEmpty(Convert.ToString(row["ServiceTypeImage"])))
                            //{
                            //    serviceType_obj.ServiceTypeImage = getlogo.ShowServiceTypeIcon(Convert.ToString(row["ServiceTypeID"]), "Icon");
                            //}
                            //else
                            //{
                            //    serviceType_obj.ServiceTypeImage = "";
                            //}

                            serviceType_List.Add(serviceType_obj);
                        }
                    }
                }
                return serviceType_List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<ServiceType> GetActiveServiceTypesList()
        {
            IList<ServiceType> featuresList = new List<ServiceType>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_GetActiveServiceTypes", sqlconn_obj))
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
                            ServiceType feature_obj = new ServiceType();
                            feature_obj.ServiceTypeID = Convert.ToInt32(dt.Rows[i]["ServiceTypeID"]);
                            feature_obj.ServiceTypeName = Convert.ToString(dt.Rows[i]["ServiceTypeName"]);
                            feature_obj.selected = false;
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
    }
}
