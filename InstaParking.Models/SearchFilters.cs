using System;

namespace InstaParking.Models
{
    public class SearchFilters
    {
        public string Company { get; set; }
        public string LocationID { get; set; }
        public string LocationParkingLotID { get; set; }
        public string ApplicationTypeID { get; set; }
        public string VehicleTypeID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string PaymentTypeID { get; set; }
        public string Duration { get; set; }
        public string TimeReportDisplayType { get; set; }
        public string SupervisorID { get; set; }
        public string OperatorID { get; set; }
        public string Display { get; set; }
        public string Reason { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string FromMeridiem { get; set; }
        public string ToMeridiem { get; set; }
        public string FOCReasonID { get; set; }
    }
}
