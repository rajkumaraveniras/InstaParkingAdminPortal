using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InstaParking.Models;
using InstaParking.DAL;

namespace InstaParking.Controllers
{
    public class OfferSpaceController : Controller
    {
        CompanyInfo_DAL companyInfoDAL_obj = new CompanyInfo_DAL();
        public ActionResult OfferSpaces()
        {
            return View();
        }
        public JsonNetResult GetOfferedSpaceDetails()
        {
            IList<OfferedSpaces> spacesList = companyInfoDAL_obj.GetOfferedSpaceDetails();
            try
            {
                return new JsonNetResult()
                {
                    Data = spacesList,
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
