namespace InstaParking.Models
{
    public class StationOccupancyDetails
    {
        public string LocationName { get; set; }
        public string LocationParkingLotName { get; set; }
        public string TwoWheelereOccupancy{get;set;}
        public string FourWheelereOccupancy { get; set; }
        public string Operator { get; set; }
        public string Supervisor { get; set; }
        public string Occupancy { get; set; }
    }
}
