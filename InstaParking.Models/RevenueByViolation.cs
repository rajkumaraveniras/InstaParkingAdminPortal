namespace InstaParking.Models
{
    public class RevenueByViolation
    {
        public string Reason { get; set; }
        public string Station { get; set; }
        public string ParkingLot { get; set; }
        //public string ClampFee { get; set; }
        public string Clamps { get; set; }
        public string Cash { get; set; }
        public string EPay { get; set; }
        public string TotalClamps { get; set; }
        public string TotalCash { get; set; }
        public string TotalEPay { get; set; }
        public decimal Amount { get; set; }
        
    }
}
