using InstaParking.DAL;
using InstaParking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InstaParking.Controllers
{
    public class FeaturesController : Controller
    {
        ServiceType_DAL serviceTypeDAL_obj = new ServiceType_DAL();
        public ActionResult features()
        {
            return View();
        }
        public JsonNetResult GetFeaturesList()
        {
            IList<ServiceType> serviceTypesList = serviceTypeDAL_obj.GetFeaturesList();

            try
            {
                return new JsonNetResult()
                {
                    Data = serviceTypesList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        [HttpPost]
        public JsonResult SaveServiceType(ServiceType serviceTypeData)
        {
            string success = "";
            try
            {
                int result = serviceTypeDAL_obj.InsertAndUpdateServiceType(serviceTypeData, Convert.ToString(Session["UserID"]));
                System.Web.HttpContext.Current.Session["ServiceTypeID"] = result;
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
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
            
        }

        [HttpPost]
        public JsonResult ViewServiceType(int ServiceTypeID)
        {
            try
            {
                List<ServiceType> serviceTypeList = serviceTypeDAL_obj.ViewServiceType(ServiceTypeID);
                return new JsonResult()
                {
                    Data = serviceTypeList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }

        public JsonNetResult GetActiveServiceTypesList()
        {
            IList<ServiceType> serviceTypesList = serviceTypeDAL_obj.GetActiveServiceTypesList();

            try
            {
                return new JsonNetResult()
                {
                    Data = serviceTypesList,
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
