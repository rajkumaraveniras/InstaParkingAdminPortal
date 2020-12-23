using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InstaParking.Models;
using InstaParking.DAL;

namespace InstaParking.Controllers
{
    public class CompanyInfoController : Controller
    {
        CompanyInfo_DAL companyInfoDAL_obj = new CompanyInfo_DAL();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult SaveAccountDetails(Account CompanyInfoData)
        {
            string success = "";
            try
            {
                int result = companyInfoDAL_obj.SaveAccountDetails(CompanyInfoData, Convert.ToString(Session["UserID"]));
                System.Web.HttpContext.Current.Session["AccountID"] = result;
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
        public JsonNetResult GetAccountDetails()
        {
            List<Account> accountList = companyInfoDAL_obj.GetAccountDetails();

            try
            {
                return new JsonNetResult()
                {
                    Data = accountList,
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
