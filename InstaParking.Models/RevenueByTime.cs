namespace InstaParking.Models
{
    public class RevenueByTime
    {
        public string TimePeriod { get; set; }
        public string Station { get; set; }
        public string ParkingLot { get; set; }
        public string In { get; set; }
        public string Out { get; set; }
        public string FOC { get; set; }
        public decimal Cash { get; set; }
        public decimal EPay { get; set; }
        public string TotalIn { get; set; }
        public string TotalOut { get; set; }
        public string TotalFOC { get; set; }
        public string TotalCash { get; set; }
        public string TotalEPay { get; set; }
        public decimal Amount { get; set; }
    }
}
