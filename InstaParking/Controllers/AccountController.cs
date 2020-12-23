using InstaParking.DAL;
using InstaParking.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using System.IO;

namespace InstaParking.Controllers
{
    public class AccountController : Controller
    {
        Users_DAL UsersDAL_obj = new Users_DAL();
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();

        public ActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }

        public ActionResult Login(string returnUrl)
        {
            ViewBag.Message = TempData["message"];
            ViewBag.ReturnUrl = returnUrl;
            if (Session["UserID"] != "" && Session["UserID"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Login(FormCollection form_obj)
        {
            try
            {
                ds = UsersDAL_obj.ValidateUser(Convert.ToString(form_obj["UserName"]), Convert.ToString(form_obj["Password"]));
                dt = ds.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[1];
                    IList<Modules> modulesList = new List<Modules>();

                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        if (Convert.ToString(dt1.Rows[i]["Parent_ModuleID"]) == null || Convert.ToString(dt1.Rows[i]["Parent_ModuleID"]) == "")
                        {
                            Modules modules_obj = new Modules();
                            modules_obj.ModuleID = Convert.ToInt32(dt1.Rows[i]["ModuleID"]);
                            modules_obj.ModuleName = Convert.ToString(dt1.Rows[i]["ModuleName"]);
                            modules_obj.RootUrl = dt1.Rows[i]["RootUrl"] == DBNull.Value ? "" : Convert.ToString(dt1.Rows[i]["RootUrl"]);
                            modules_obj.MenuItemOrder = dt1.Rows[i]["MenuItemOrder"] == DBNull.Value ? 0 : Convert.ToInt32(dt1.Rows[i]["MenuItemOrder"]);
                            modules_obj.IconName = Convert.ToString(dt1.Rows[i]["IconName"]);
                            modulesList.Add(modules_obj);
                        }
                    }

                    for (int k = 0; k < modulesList.Count; k++)
                    {
                        IList<SubModule> subModulesList = new List<SubModule>();
                        for (int j = 0; j < dt1.Rows.Count; j++)
                        {
                            if (Convert.ToString(dt1.Rows[j]["Parent_ModuleID"]) != null && Convert.ToString(dt1.Rows[j]["Parent_ModuleID"]) != "")
                            {
                                if (Convert.ToInt32(dt1.Rows[j]["Parent_ModuleID"]) == modulesList[k].ModuleID)
                                {
                                    SubModule submodules_obj = new SubModule();
                                    submodules_obj.ModuleID = Convert.ToInt32(dt1.Rows[j]["ModuleID"]);
                                    submodules_obj.ModuleName = Convert.ToString(dt1.Rows[j]["ModuleName"]);
                                    submodules_obj.RootUrl = dt1.Rows[j]["RootUrl"] == DBNull.Value ? "" : Convert.ToString(dt1.Rows[j]["RootUrl"]);
                                    submodules_obj.MenuItemOrder = dt1.Rows[j]["MenuItemOrder"] == DBNull.Value ? 0 : Convert.ToInt32(dt1.Rows[j]["MenuItemOrder"]);
                                    subModulesList.Add(submodules_obj);
                                    modulesList[k].SubModules = subModulesList;
                                }
                            }
                        }
                    }
                    Session["Modules"] = modulesList;
                }
               
                string invalidMessage = string.Empty;
                
                if (dt.Rows.Count > 0)
                {
                    Session["Name"] = Convert.ToString(dt.Rows[0]["UserName"]);
                    Session["UserID"] = Convert.ToInt32(dt.Rows[0]["UserID"]);
                    Session["UserType"] = Convert.ToString(dt.Rows[0]["UserTypeName"]);
                    Session["UserCode"] = Convert.ToString(dt.Rows[0]["UserCode"]);

                    string empPhotourl = "";
                    if (ds.Tables[2].Rows[0]["EmpPhoto"] != null && Convert.ToString(ds.Tables[2].Rows[0]["EmpPhoto"]) != "")
                    {
                        if (System.IO.File.Exists(HttpContext.Server.MapPath("~/EmployeeImages/" + Convert.ToString(ds.Tables[2].Rows[0]["EmpPhoto"]))))
                        {
                            empPhotourl = "EmployeeImages/"+Convert.ToString(ds.Tables[2].Rows[0]["EmpPhoto"]);
                        }
                        else
                        {
                            empPhotourl = "Images/loginimage.png";
                        }
                        Session["EmpPhoto"] = empPhotourl;
                    }
                    else
                    {
                        empPhotourl = "Images/loginimage.png";
                        Session["EmpPhoto"] = empPhotourl;
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    invalidMessage = "Username and/or password is incorrect.";
                    ViewBag.Message = invalidMessage;
                    //return View();
                   // TempData["message"] = " dashborad index";
                    //return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
                return RedirectToAction("Login", "Account");
            }
            return View();
            //return RedirectToAction("Login", "Account");
        }

        //LogOut User
        public ActionResult LogOut()
        {
            Session.Remove("Name");
            Session.Abandon();
            if (Request.Cookies["SMSESSION"] != null)
            {
                Response.Cookies["SMSESSION"].Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["SMSESSION"].Value = "";
            }
            if (Request.Cookies["SMIDENTITY"] != null)
            {
                Response.Cookies["SMIDENTITY"].Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["SMIDENTITY"].Value = "";
            }

            //return Redirect(ConfigurationManager.AppSettings["LogOutURL"].ToString());
            return RedirectToAction("Login", "Account");
        }

    }
}
