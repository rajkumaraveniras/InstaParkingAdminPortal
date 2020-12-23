using InstaParking.DAL;
using InstaParking.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace InstaParking.Controllers
{
    public class ParkingController : Controller
    {
        Zones_DAL zonesDAL_obj = new Zones_DAL();
        Locations_DAL locationsDAL_obj = new Locations_DAL();
        Lots_DAL lotsDAL_obj = new Lots_DAL();


        #region Zones
        public ActionResult Index()
        {
            ViewBag.Menu = "Parking";
            return View();
        }
        public JsonNetResult GetZonesList()
        {
            IList<Zones> zonesList = zonesDAL_obj.GetZonesList();

            try
            {
                return new JsonNetResult()
                {
                    Data = zonesList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public ActionResult CreateZone()
        {
            ViewBag.Menu = "Parking";
            return View();
        }
        public JsonNetResult GetActiveCitiesList()
        {
            IList<City> citiesList = zonesDAL_obj.GetActiveCitiesList();
            try
            {
                return new JsonNetResult()
                {
                    Data = citiesList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }

        [HttpPost]
        public JsonResult SaveZone(Zones ZonesData)
        {
            string success = "";
            try
            {
                success = zonesDAL_obj.InsertAndUpdateZone(ZonesData, Convert.ToString(Session["UserID"]));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditZone()
        {
            ViewBag.Menu = "Parking";
            return View();
        }

        [HttpPost]
        public JsonResult ViewZone(int ZoneID)
        {
            try
            {
                List<Zones> ZonesList = zonesDAL_obj.ViewZone(ZoneID);
                return new JsonResult()
                {
                    Data = ZonesList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                throw ex;
                return Json("Exception", JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        #region Stations
        public ActionResult Stations()
        {
            ViewBag.Menu = "Parking";
            return View();
        }
        public JsonNetResult GetLocationsList()
        {
            IList<Locations> locationsList = locationsDAL_obj.GetLocationsList();

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

        public ActionResult CreateStation()
        {
            ViewBag.Menu = "Parking";
            return View();
        }

        [HttpPost]
        public JsonResult SaveLocation(Locations LocationData)
        {
            string success = "";
            try
            {
                success = locationsDAL_obj.InsertAndUpdateLocation(LocationData, Convert.ToString(Session["UserID"]));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditStation()
        {
            ViewBag.Menu = "Parking";
            return View();
        }

        [HttpPost]
        public JsonResult ViewLocation(int LocationID)
        {
            try
            {
                List<Locations> locationsList = locationsDAL_obj.ViewLocation(LocationID);
                return new JsonResult()
                {
                    Data = locationsList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                throw ex;
                return Json("Exception", JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Lots
        public ActionResult Lots()
        {
            ViewBag.Menu = "Parking";
            return View();
        }
        public JsonNetResult GetLotsList()
        {
            IList<Lots> lotsList = lotsDAL_obj.GetLotsList();

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

        public ActionResult CreateLot()
        {
            ViewBag.Menu = "Parking";
            return View();
        }

        public JsonNetResult GetActiveLocationList()
        {
            IList<Locations> locationsList = locationsDAL_obj.GetActiveLocationList();
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
        public JsonNetResult GetActiveParkingTypesList()
        {
            IList<ParkingType> parkingTypesList = lotsDAL_obj.GetActiveParkingTypesList();
            try
            {
                return new JsonNetResult()
                {
                    Data = parkingTypesList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public JsonNetResult GetActiveVehicleTypesList()
        {
            IList<VehicleType> vehicleTypesList = lotsDAL_obj.GetActiveVehicleTypesList();
            try
            {
                return new JsonNetResult()
                {
                    Data = vehicleTypesList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public JsonNetResult GetActiveParkingBayList()
        {
            IList<ParkingBay> parkingBaysList = lotsDAL_obj.GetActiveParkingBayList();
            try
            {
                return new JsonNetResult()
                {
                    Data = parkingBaysList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public JsonNetResult GetActiveLotVehicleAvailabilityList()
        {
            IList<LotVehicleAvailability> list = lotsDAL_obj.GetActiveLotVehicleAvailabilityList();
            try
            {
                return new JsonNetResult()
                {
                    Data = list,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        [HttpPost]
        public JsonResult SaveLot(Lots LotsData)
        {
            string success = "";
            try
            {
                int result = lotsDAL_obj.InsertAndUpdateLot(LotsData, Convert.ToString(Session["UserID"]));
                System.Web.HttpContext.Current.Session["LotID"] = result;
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
                //if (result == -2)
                //{
                //    success = "Please Change Location Status to Active.";
                //}
                return Json(success, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }           
        }
        public ActionResult EditLot()
        {
            ViewBag.Menu = "Parking";
            return View();
        }
        [HttpPost]
        public JsonResult ViewLot(int LotID)
        {
            try
            {                
                List<Lots> LotsList = lotsDAL_obj.ViewLot(LotID);
                return new JsonResult()
                {
                    Data = LotsList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                throw ex;
                return Json("Exception", JsonRequestBehavior.AllowGet);
            }

        }

        #endregion
        
        #region Lot Timing
        [HttpPost]
        public JsonResult SaveLotTimings(ParkingLotTiming timingsData)
        {
            string resultmsg = string.Empty;
            try
            {
               int result = lotsDAL_obj.SaveLotTimings(timingsData, Convert.ToString(Session["UserID"]));
                
                if (result > 0)
                {
                    resultmsg = "Success";
                }
                else if (result == -1)
                {
                    resultmsg = "Data Exists";
                }
                //else if (result == -2)
                //{
                //    resultmsg = "Please Change Lot Status to Active";
                //}
                else
                {
                    resultmsg = "Failed";
                }

                return Json(resultmsg, JsonRequestBehavior.AllowGet);
                //return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonNetResult GetLotTimings(int LotID)
        {
            IList<ParkingLotTiming> lottimingsList = lotsDAL_obj.GetLotTimings(LotID);
            try
            {
                return new JsonNetResult()
                {
                    Data = lottimingsList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }

        [HttpPost]
        public JsonResult GetLotTimingByID(int ParkingLotTimingID)
        {
            try
            {
                List<ParkingLotTiming> lotTimingList = lotsDAL_obj.GetLotTimingByID(ParkingLotTimingID);
                return new JsonResult()
                {
                    Data = lotTimingList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        [HttpPost]
        public JsonResult DeleteLotTime(int ParkingLotTimingID)
        {
            string resultmsg = string.Empty;
            try
            {
                resultmsg = lotsDAL_obj.DeleteLotTime(ParkingLotTimingID, Convert.ToString(Session["UserID"]));
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Lot Price
        public JsonNetResult GetApplicationTypesList()
        {
            IList<ApplicationType> appTypesList = lotsDAL_obj.GetApplicationTypesList();
            try
            {
                return new JsonNetResult()
                {
                    Data = appTypesList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        [HttpPost]
        public JsonResult SaveLotPrice(LotPrice lotPriceData)
        {
            string resultmsg = string.Empty;
            try
            {
                resultmsg = lotsDAL_obj.SaveLotPrice(lotPriceData, Convert.ToString(Session["UserID"]));
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonNetResult GetLotPrices(int LotID)
        {
            IList<LotPrice> lotpricesList = lotsDAL_obj.GetLotPrices(LotID);
            try
            {
                return new JsonNetResult()
                {
                    Data = lotpricesList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        [HttpPost]
        public JsonResult GetLotPriceByID(int PriceID)
        {
            try
            {
                List<LotPrice> lotPriceList = lotsDAL_obj.GetLotPriceByID(PriceID);
                return new JsonResult()
                {
                    Data = lotPriceList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        [HttpPost]
        public JsonResult DeleteLotPrice(int PriceID)
        {
            string resultmsg = string.Empty;
            try
            {
                resultmsg = lotsDAL_obj.DeleteLotPrice(PriceID, Convert.ToString(Session["UserID"]));
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        
        #region Lot Services
        public JsonResult SaveLotServices(List<ServiceType> servicesArray, int LotID)
        {
            string resultmsg = string.Empty;

            try
            {
                resultmsg = lotsDAL_obj.SaveLotServices(servicesArray, Convert.ToString(Session["UserID"]), Convert.ToInt32(LotID));
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonNetResult GetSelectedLotServiceTypesList(int LotID)
        {
            IList<ServiceType> serviceTypesList = lotsDAL_obj.GetSelectedLotServiceTypesList(LotID);

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
        #endregion

        #region Lot Bays
        [HttpPost]
        public JsonResult SaveLotBays(ParkingBay lotBaysData)
        {
            string resultmsg = string.Empty;
            try
            {
                resultmsg = lotsDAL_obj.SaveLotBays(lotBaysData, Convert.ToString(Session["UserID"]));
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonNetResult GetLotBays(int LotID)
        {
            IList<ParkingBay> lotbaysList = lotsDAL_obj.GetLotBays(LotID);
            try
            {
                return new JsonNetResult()
                {
                    Data = lotbaysList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        [HttpPost]
        public JsonResult GetLotBayByID(int ParkingBayID)
        {
            try
            {
                List<ParkingBay> lotBaysList = lotsDAL_obj.GetLotBayByID(ParkingBayID);
                return new JsonResult()
                {
                    Data = lotBaysList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        [HttpPost]
        public JsonResult DeleteLotBay(int ParkingBayID)
        {
            string resultmsg = string.Empty;
            try
            {
                resultmsg = lotsDAL_obj.DeleteLotBay(ParkingBayID, Convert.ToString(Session["UserID"]));
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetLotCapacityByVehicleType(string LotID)
        {
            string success = "";
            try
            {
                success = lotsDAL_obj.GetLotCapacityByVehicleType(Convert.ToInt32(LotID));
                return Json(success, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Charges
        public ActionResult Charges()
        {
            ViewBag.Menu = "Parking";
            return View();
        }
        public JsonNetResult GetChargesData()
        {
            IList<Charges> chargesData = lotsDAL_obj.GetChargesData();

            try
            {
                return new JsonNetResult()
                {
                    Data = chargesData,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        [HttpPost]
        public JsonResult SaveCharges(Charges chargesData)
        {
            string success = "";
            try
            {
                success = lotsDAL_obj.SaveCharges(chargesData, Convert.ToString(Session["UserID"]));
                      
                return Json(success, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        [HttpPost]
        public string CheckInSession()
        {
            try
            {
                var name = Session["Name"];
                var EmpID = Session["UserID"];

                if (name == null || EmpID == null)
                {
                    Session.Clear();
                    return "false";
                }
                else
                {
                    return "true";
                }
            }
            catch (Exception ex)
            {
                return "false";
            }
        }
    }
}
