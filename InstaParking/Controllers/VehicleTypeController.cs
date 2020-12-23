using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InstaParking.Models;
using InstaParking.DAL;

namespace InstaParking.Controllers
{
    public class VehicleTypeController : Controller
    {
        VehicleType_DAL vehicleTypeDAL_obj = new VehicleType_DAL();

        public ActionResult Index()
        {
            ViewBag.Menu = "VehicleType";
            return View();
        }

        public JsonNetResult GetVehicleTypeList()
        {
            IList<VehicleType> vehicleTypeList = vehicleTypeDAL_obj.GetVehicleTypeList();

            try
            {
                return new JsonNetResult()
                {
                    Data = vehicleTypeList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public ActionResult CreateVehicleType()
        {
            ViewBag.Menu = "VehicleType";
            return View();
        }
        [HttpPost]
        public JsonResult SaveVehicleType(VehicleType VehicleTypeData)
        {
            string success = "";
            try
            {
                int result = vehicleTypeDAL_obj.InsertAndUpdateVehicleType(VehicleTypeData, Convert.ToString(Session["UserID"]));
                System.Web.HttpContext.Current.Session["VehicleTypeID"] = result;
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
                throw ex;
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
           
        }
        public ActionResult EditVehicleType()
        {
            ViewBag.Menu = "VehicleType";
            return View();
        }
        [HttpPost]
        public JsonResult ViewVehicleType(int VehicleTypeID)
        {
            try
            {
                List<VehicleType> vehicleTypeList = vehicleTypeDAL_obj.ViewVehicleType(VehicleTypeID);
                return new JsonResult()
                {
                    Data = vehicleTypeList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }

       
    }
}
