namespace InstaParking.Models
{
    public class RevenueByPasses
    {
        public string Station { get; set; }
        public string ParkingLot { get; set; }
        //public string VehicleType { get; set; }
        //public string TypeofPass { get; set; }
        public string TypeofPassName { get; set; }
       // public string PassIn { get; set; }
        //public string New { get; set; }
        public string Count { get; set; }
        // public string Renew { get; set; }
       // public string NFC { get; set; }
        public string PassWithNFC { get; set; }
        public string OnlyNFC { get; set; }
        public string Cash { get; set; }
        public string EPay { get; set; }
        public string TotalCount { get; set; }
        public string TotalPassWithNFC { get; set; }
        public string TotalOnlyNFC { get; set; }
        public string TotalCash { get; set; }
        public string TotalEPay { get; set; }
        public decimal Amount { get; set; }
    }
}
