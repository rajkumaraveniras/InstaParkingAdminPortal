using InstaParking.DAL;
using InstaParking.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace InstaParking.Controllers
{
    public class SupervisorController : Controller
    {
        UserLocationMapper_DAL userlocationMapperDAL_obj = new UserLocationMapper_DAL();

        public ActionResult AssignOperators()
        {
            ViewBag.Menu = "Supervisor";
            return View();
        }
        public JsonNetResult GetOperatorsLoginStatusList()
        {
            List<UserLocationMapper> usersList = userlocationMapperDAL_obj.GetOperatorsLoginStatusList(Convert.ToInt32(Session["UserID"]), Convert.ToString(Session["UserType"]));

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
        public JsonNetResult GetOperatorsBySupervisorID()
        {
            IList<User> usersList = userlocationMapperDAL_obj.GetOperatorsBySupervisorID(Convert.ToInt32(Session["UserID"]));

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
        public JsonNetResult GetLocationsofSupervisor()
        {
            IList<Locations> locationsList = userlocationMapperDAL_obj.GetLocationsofSupervisor(Convert.ToInt32(Session["UserID"]), Convert.ToString(Session["UserType"]));

            try
            {
                return new JsonNetResult()
                {
                    Data = locationsList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public JsonNetResult GetLotsByLocationofSupervisor(string LocationID)
        {
            IList<Lots> lotsList = userlocationMapperDAL_obj.GetLotsByLocationofSupervisor(Convert.ToInt32(LocationID), Convert.ToInt32(Session["UserID"]), Convert.ToString(Session["UserType"]));

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
        public JsonNetResult GetOperatorsLoginStatusByLocationandLot(UserLocationMapper SearchData)
        {
            IList<UserLocationMapper> usersList = userlocationMapperDAL_obj.GetOperatorsLoginStatusByLocationandLot(SearchData, Convert.ToInt32(Session["UserID"]), Convert.ToString(Session["UserType"]));

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
        [HttpPost]
        public ActionResult AssignOperatortoLot(string operatorID, UserLocationMapper LotDetails, string absentUserID)
        {
            string success = "";
            //string[] values = operatorID.Split(',');           

            try
            {
                success = userlocationMapperDAL_obj.AssignOperatortoLot(Convert.ToString(operatorID), LotDetails, Convert.ToInt32(Session["UserID"]), Convert.ToString(Session["UserID"]), Convert.ToInt32(absentUserID));
                //for (int i = 0; i < values.Length; i++)
                //{   
                //    success = userlocationMapperDAL_obj.AssignOperatortoLot(Convert.ToInt32(values[i].ToString()), LotDetails, Convert.ToInt32(Session["UserID"]), Convert.ToString(Session["UserID"]),Convert.ToInt32(absentUserID));
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddOperatortoLot(string operatorID, UserLocationMapper LotDetails)
        {
            string success = "";

            try
            {
                success = userlocationMapperDAL_obj.AddOperatortoLot(Convert.ToString(operatorID), LotDetails, Convert.ToInt32(Session["UserID"]), Convert.ToString(Session["UserID"]));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
    }
}
