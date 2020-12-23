namespace InstaParking.Models
{
    public class RevenueByChannel
    {
        public string Channel { get; set; }
        public string Station { get; set; }
        public string ParkingLot { get; set; }
        public int CheckIns { get; set; }
        public string Cash { get; set; }
        public string EPay { get; set; }
        public string TotalCheckIns { get; set; }
        public string TotalCash { get; set; }
        public string TotalEPay { get; set; }
        public decimal Amount { get; set; }
    }
}
