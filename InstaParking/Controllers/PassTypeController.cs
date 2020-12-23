using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InstaParking.DAL;
using InstaParking.Models;

namespace InstaParking.Controllers
{
    public class PassTypeController : Controller
    {
        PassType_DAL passTypeDAL_obj = new PassType_DAL();

        #region PassType
        public ActionResult Index()
        {
            ViewBag.Menu = "PassType";
            return View();
        }
        public ActionResult CreatePass()
        {
            ViewBag.Menu = "PassType";
            return View();
        }
        public JsonNetResult GetPassTypeList()
        {
            IList<Passes> passTypeList = passTypeDAL_obj.GetPassTypeList();

            try
            {
                return new JsonNetResult()
                {
                    Data = passTypeList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        [HttpPost]
        public JsonResult SavePassType(Passes PassTypeData)
        {
            string success = "";
            try
            {
                success = passTypeDAL_obj.InsertAndUpdatePassType(PassTypeData, Convert.ToString(Session["UserID"]));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditPass()
        {
            ViewBag.Menu = "PassType";
            return View();
        }
        [HttpPost]
        public JsonResult ViewPassType(int PassPriceID)
        {
            try
            {
                List<Passes> passTypeList = passTypeDAL_obj.ViewPassType(PassPriceID);
                return new JsonResult()
                {
                    Data = passTypeList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }

        public JsonNetResult GetActivePassTypes()
        {
            IList<PassType> passTypesList = passTypeDAL_obj.GetActivePassTypes();
            try
            {
                return new JsonNetResult()
                {
                    Data = passTypesList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        #endregion

        #region PassSaleLimit
        public JsonNetResult GetPassSaleLimitList()
        {
            IList<PassSaleLimit> passsaleLimitList = passTypeDAL_obj.GetPassSaleLimitList();

            try
            {
                return new JsonNetResult()
                {
                    Data = passsaleLimitList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        [HttpPost]
        public JsonResult RestrictPassSaleLimit(PassSaleLimit PassSaleLimitData)
        {
            string success = "";
            try
            {
                int result = passTypeDAL_obj.RestrictPassSaleLimit(PassSaleLimitData, Convert.ToString(Session["UserID"]));
                if (result > 0)
                {
                    success = "Success";
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
        [HttpPost]
        public JsonResult EditPassSaleLimit(int PassSaleLimitID)
        {
            try
            {
                List<PassSaleLimit> passList = passTypeDAL_obj.EditPassSaleLimit(PassSaleLimitID);
                return new JsonResult()
                {
                    Data = passList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        [HttpPost]
        public JsonResult DeletePassSaleLimit(int PassSaleLimitID)
        {
            string resultmsg = string.Empty;
            try
            {
                resultmsg = passTypeDAL_obj.DeletePassSaleLimit(PassSaleLimitID, Convert.ToString(Session["UserID"]));
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
