using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using InstaParking.DAL;
using InstaParking.Models;

namespace InstaParking.Controllers
{
    public class DashboardController : Controller
    {
        Users_DAL userDAL_obj = new Users_DAL();
        Dashboard_DAL dashboardDAL_obj = new Dashboard_DAL();
        public ActionResult Index()
        {
          //  return RedirectToAction("DownloadPDF");
          return View();
        }
        public ActionResult Carrier() {
            return View();
        }

        public JsonNetResult GetActiveSupervisorsList()
        {
            IList<User> usersList = userDAL_obj.GetDashboardSupervisorsList();

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
        public JsonNetResult GetListofStationOccupancy()
        {
            IList<StationOccupancy> stationoccupancyList = dashboardDAL_obj.GetListofStationOccupancy();

            try
            {
                return new JsonNetResult()
                {
                    Data = stationoccupancyList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }

        public ActionResult StationOccupancy()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetOccupancyDetailsByStation(int LocationID)
        {
            try
            {
                List<StationOccupancyDetails> occupancyList = dashboardDAL_obj.GetOccupancyDetailsByStation(LocationID);
                return new JsonResult()
                {
                    Data = occupancyList,
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
