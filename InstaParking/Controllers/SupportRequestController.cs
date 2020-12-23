using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InstaParking.Models;
using InstaParking.DAL;

namespace InstaParking.Controllers
{
    public class SupportRequestController : Controller
    {
        CompanyInfo_DAL companyInfoDAL_obj = new CompanyInfo_DAL();
        public ActionResult SupportRequests()
        {
            return View();
        }
        public JsonNetResult GetSupportRequestDetails()
        {
            IList<SupportRequests> requestList = companyInfoDAL_obj.GetSupportRequestDetails();
            try
            {
                return new JsonNetResult()
                {
                    Data = requestList,
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
