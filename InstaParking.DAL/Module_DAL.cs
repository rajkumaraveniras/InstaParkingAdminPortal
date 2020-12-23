using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstaParking.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace InstaParking.DAL
{
    public class Module_DAL
    {
        SqlHelper sqlhelper_obj;
        public IList<Modules> GetModulesSubModulesList(int UserTypeID)
        {
            IList<Modules> modulesList = new List<Modules>();
            // IList<SubModule> subModulesList = new List<SubModule>();
            sqlhelper_obj = new SqlHelper();
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_GetModulesSubmodulesData", sqlconn_obj))
                    {
                        sqlcmd_obj.CommandType = CommandType.StoredProcedure;
                        sqlcmd_obj.Parameters.AddWithValue("@UserTypeID", UserTypeID);
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
                            if (Convert.ToString(dt.Rows[i]["Parent_ModuleID"]) == null || Convert.ToString(dt.Rows[i]["Parent_ModuleID"]) == "")
                            {
                                Modules modules_obj = new Modules();
                                modules_obj.ModuleID = Convert.ToInt32(dt.Rows[i]["ModuleID"]);
                                modules_obj.ModuleName = Convert.ToString(dt.Rows[i]["ModuleName"]);
                                modules_obj.IsAssign = dt.Rows[i]["IsAssign"] == DBNull.Value ? false : Convert.ToBoolean(dt.Rows[i]["IsAssign"]);
                                modules_obj.Parent_ModuleID = dt.Rows[i]["Parent_ModuleID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[i]["Parent_ModuleID"]);
                                modulesList.Add(modules_obj);
                            }
                        }

                        for (int k = 0; k < modulesList.Count; k++)
                        {
                            IList<SubModule> subModulesList = new List<SubModule>();
                            for (int j = 0; j < dt.Rows.Count; j++)
                            {
                                if (Convert.ToString(dt.Rows[j]["Parent_ModuleID"]) != null && Convert.ToString(dt.Rows[j]["Parent_ModuleID"]) != "")
                                {
                                    if (Convert.ToInt32(dt.Rows[j]["Parent_ModuleID"]) == modulesList[k].ModuleID)
                                    {
                                        SubModule submodules_obj = new SubModule();
                                        submodules_obj.ModuleID = Convert.ToInt32(dt.Rows[j]["ModuleID"]);
                                        submodules_obj.ModuleName = Convert.ToString(dt.Rows[j]["ModuleName"]);
                                        submodules_obj.IsAssign = dt.Rows[j]["IsAssign"] == DBNull.Value ? false : Convert.ToBoolean(dt.Rows[j]["IsAssign"]);
                                        submodules_obj.Parent_ModuleID = dt.Rows[j]["Parent_ModuleID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[j]["Parent_ModuleID"]);
                                        subModulesList.Add(submodules_obj);
                                    }
                                }
                            }
                            modulesList[k].SubModules = subModulesList;
                        }
                    }
                }
                return modulesList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string AssignModules(List<Modules> nameValueCollection_list, string CreatedBy, int userTypeID)
        {
            sqlhelper_obj = new SqlHelper();
            string resultmsg = "";
            try
            {
                using (SqlConnection sqlconn_obj = new SqlConnection())
                {
                    sqlconn_obj.ConnectionString = sqlhelper_obj.GetConnectionSrting();

                    for (var i = 0; i < nameValueCollection_list.Count; i++)
                    {
                        using (SqlCommand sqlcmd_obj = new SqlCommand("PARK_PROC_AssignModulestoUser", sqlconn_obj))
                        {
                            sqlcmd_obj.CommandType = CommandType.StoredProcedure;
                            sqlcmd_obj.CommandTimeout = 0;
                            sqlcmd_obj.Parameters.AddWithValue("@UserTypeID", userTypeID);
                            sqlcmd_obj.Parameters.AddWithValue("@ModuleID", nameValueCollection_list[i].ModuleID);
                            sqlcmd_obj.Parameters.AddWithValue("@IsAssign", nameValueCollection_list[i].IsAssign);
                            sqlcmd_obj.Parameters.AddWithValue("@CreatedBy", CreatedBy);
                            sqlconn_obj.Open();
                            int res = sqlcmd_obj.ExecuteNonQuery();
                            sqlconn_obj.Close();
                        }
                    }
                    for (int j = 0; j < nameValueCollection_list.Count; j++)
                    {
                        for (int k = 0; k < nameValueCollection_list[j].SubModules.Count; k++)
                        {
                            using (SqlCommand sqlcmd_submodule_obj = new SqlCommand("PARK_PROC_AssignModulestoUser", sqlconn_obj))
                            {
                                sqlcmd_submodule_obj.CommandType = CommandType.StoredProcedure;
                                sqlcmd_submodule_obj.CommandTimeout = 0;
                                sqlcmd_submodule_obj.Parameters.AddWithValue("@UserTypeID", userTypeID);
                                sqlcmd_submodule_obj.Parameters.AddWithValue("@ModuleID", nameValueCollection_list[j].SubModules[k].ModuleID);
                                sqlcmd_submodule_obj.Parameters.AddWithValue("@IsAssign", nameValueCollection_list[j].SubModules[k].IsAssign);
                                sqlcmd_submodule_obj.Parameters.AddWithValue("@CreatedBy", CreatedBy);
                                sqlconn_obj.Open();
                                int res = sqlcmd_submodule_obj.ExecuteNonQuery();
                                sqlconn_obj.Close();
                                if (res > 0)
                                {
                                    resultmsg = "Success";
                                }
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
