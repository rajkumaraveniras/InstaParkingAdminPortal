using InstaParking.DAL;
using InstaParking.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace InstaParking.Controllers
{
    public class EmployeeController : Controller
    {
        UserType_DAL userTypeDAL_obj = new UserType_DAL();
        Users_DAL usersDAL_obj = new Users_DAL();
        AssignUserStation_DAL assignuserStationDAL_obj = new AssignUserStation_DAL();
        Locations_DAL locationDAL_obj = new Locations_DAL();
        Lots_DAL lotsDAL_obj = new Lots_DAL();
        Module_DAL moduleDAL_obj = new Module_DAL();

        #region Role
        public ActionResult Index()
        {
            ViewBag.Menu = "Employee";
            return View();
        }
        public JsonNetResult GetRolesList()
        {
            IList<UserType> rolesList = userTypeDAL_obj.GetRolesList();

            try
            {

                return new JsonNetResult()
                {
                    Data = rolesList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public ActionResult CreateRole()
        {
            ViewBag.Menu = "Employee";
            return View();
        }
        [HttpPost]
        public JsonResult SaveRole(UserType RolesData)
        {
            string success = "";
            try
            {
                success = userTypeDAL_obj.InsertAndUpdateRole(RolesData, Convert.ToString(Session["UserID"]));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditRole()
        {
            ViewBag.Menu = "Employee";
            return View();
        }
        [HttpPost]
        public JsonResult ViewRole(int RoleID)
        {
            try
            {
                List<UserType> rolesList = userTypeDAL_obj.ViewRole(RoleID);
                return new JsonResult()
                {
                    Data = rolesList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        #endregion

        #region Employees
        public ActionResult Employees()
        {
            ViewBag.Menu = "Employee";
            return View();
        }
        public JsonNetResult GetEmployeesList()
        {
            IList<User> usersList = usersDAL_obj.GetEmployeesList();

            try
            {
                return new JsonNetResult()
                {
                    Data = usersList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public ActionResult CreateEmployee()
        {
            ViewBag.Menu = "Employee";
            return View();
        }
        public JsonNetResult GetActiveRolesList()
        {
            IList<UserType> rolesList = userTypeDAL_obj.GetActiveRolesList();

            try
            {
                return new JsonNetResult()
                {
                    Data = rolesList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public JsonNetResult GetActiveSupervisorsList()
        {
            IList<User> supervisorsList = usersDAL_obj.GetActiveSupervisorsList();

            try
            {
                return new JsonNetResult()
                {
                    Data = supervisorsList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        [HttpPost]
        public JsonResult SaveEmployee(User UsersData)
        {
            string success = "";

            try
            {
                int result = usersDAL_obj.InsertAndUpdateUser(UsersData, Convert.ToString(Session["UserID"]));
                System.Web.HttpContext.Current.Session["EmployeeID"] = result;
                if (result > 0)
                {
                    success = "Success" + "@" + result;
                }
                else
                {
                    success = "Failed";
                }
                if (result == -1)
                {
                    success = "Data Exists";
                }
                return Json(success, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult AssignStationtoUser(UserLocationMapper assignstationdata)
        {
            string success = "";
            try
            {
                int result = usersDAL_obj.AssignStationtoUser(assignstationdata, Convert.ToString(Session["UserID"]));
                if (result > 0)
                {
                    success = "Success";
                }
                else if (result == -1)
                {
                    success = "Data Exists";
                }
                else
                {
                    success = "Failed";
                }

                return Json(success, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }

        }

        public JsonNetResult GetAssignStationList(int UserID)
        {
            IList<UserLocationMapper> lst = usersDAL_obj.GetAssignStationList(UserID);

            try
            {
                return new JsonNetResult()
                {
                    Data = lst,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }

        [HttpPost]
        public JsonResult GetAssignStationByID(int UserLocationMapperID)
        {
            try
            {
                List<UserLocationMapper> lst = usersDAL_obj.GetAssignStationByID(UserLocationMapperID);
                return new JsonResult()
                {
                    Data = lst,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }

        [HttpPost]
        public JsonResult DeleteAssignStation(int UserLocationID)
        {
            string resultmsg = string.Empty;
            try
            {
                resultmsg = usersDAL_obj.DeleteAssignStation(UserLocationID, Convert.ToString(Session["UserID"]));
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditEmployee()
        {
            ViewBag.Menu = "Employee";
            return View();
        }
        [HttpPost]
        public JsonResult ViewEmployee(int EmployeeID)
        {
            try
            {
                List<User> usersList = usersDAL_obj.ViewEmployee(EmployeeID);
                System.Web.HttpContext.Current.Session["EmployeePhotoName"] = usersList[0].Photo;
                return new JsonResult()
                {
                    Data = usersList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }

        public JsonNetResult GetModulesSubModulesList(int UserTypeID)
        {
            IList<Modules> modulesList = moduleDAL_obj.GetModulesSubModulesList(UserTypeID);
            try
            {
                return new JsonNetResult()
                {
                    Data = modulesList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public JsonResult AssignModules(List<Modules> modulesArray, int userTypeID)
        {
            string resultmsg = string.Empty;

            try
            {
                resultmsg = moduleDAL_obj.AssignModules(modulesArray, Convert.ToString(Session["UserID"]), Convert.ToInt32(userTypeID));
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult CheckLocationExistforReportToEmployee(int SupervisorID, int LocationID)
        {
            try
            {
                bool result = usersDAL_obj.CheckLocationExistforReportToEmployee(Convert.ToInt32(SupervisorID), Convert.ToInt32(LocationID));
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = ex.Message };
            }
        }
        #endregion

        #region Assign
        public ActionResult Assign()
        {
            ViewBag.Menu = "Employee";
            return View();
        }
        public JsonNetResult GetAssignList()
        {
            IList<AssignUserStation> assignList = assignuserStationDAL_obj.GetAssignList();

            try
            {
                return new JsonNetResult()
                {
                    Data = assignList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public ActionResult CreateAssign()
        {
            ViewBag.Menu = "Employee";
            return View();
        }

        public JsonNetResult GetActiveUsersList()
        {
            IList<User> userList = usersDAL_obj.GetActiveUsersList();

            try
            {
                return new JsonNetResult()
                {
                    Data = userList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public JsonNetResult GetActiveLocationsList()
        {
            IList<Locations> locationList = locationDAL_obj.GetActiveLocationList();

            try
            {
                return new JsonNetResult()
                {
                    Data = locationList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public JsonNetResult GetActiveLotssList(string LocationID)
        {
            IList<Lots> lotsList = lotsDAL_obj.GetActiveLotssList(Convert.ToInt32(LocationID));

            try
            {
                return new JsonNetResult()
                {
                    Data = lotsList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        [HttpPost]
        public JsonResult AssignStation(AssignUserStation AssignData)
        {
            string success = "";
            try
            {
                success = assignuserStationDAL_obj.AssignStation(AssignData, Convert.ToString(Session["UserID"]));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditAssign()
        {
            ViewBag.Menu = "Employee";
            return View();
        }
        [HttpPost]
        public JsonResult ViewAssign(int AssignID)
        {
            try
            {
                List<AssignUserStation> assignList = assignuserStationDAL_obj.ViewAssign(AssignID);
                return new JsonResult()
                {
                    Data = assignList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        #endregion

        #region Emp Profile
        [HttpPost]
        public JsonResult ViewEmployeeProfile(int EmployeeID)
        {
            try
            {
                List<User> usersList = usersDAL_obj.ViewEmployeeProfile(EmployeeID);
                return new JsonResult()
                {
                    Data = usersList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        #endregion

    }
}
