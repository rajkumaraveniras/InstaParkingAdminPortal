namespace InstaParking.Models
{
    public class RevenueBySupervisor
    {
        public string Supervisor { get; set; }
        public string Operator { get; set; }
        public string Station { get; set; }
        public string ParkingLot { get; set; }
        public decimal ClampCash { get; set; }
        public decimal ClampEPay { get; set; }
        public decimal CheckInsCash { get; set; }
        public decimal CheckInsEPay { get; set; }
        public decimal PassesCash { get; set; }
        public decimal PassesEPay { get; set; }
        public decimal NFCCash { get; set; }
        public decimal NFCEPay { get; set; }
        public string TotalClampCash { get; set; }
        public string TotalClampEPay { get; set; }
        public string TotalCheckInsCash { get; set; }
        public string TotalCheckInsEPay { get; set; }
        public string TotalPassesCash { get; set; }
        public string TotalPassesEPay { get; set; }
        public string TotalNFCCash { get; set; }
        public string TotalNFCEPay { get; set; }
        public decimal Amount { get; set; }
       
    }
}
