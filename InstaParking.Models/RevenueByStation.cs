namespace InstaParking.Models
{
    public class RevenueByStation
    {
        public string Station { get; set; }
        public string ParkingLot { get; set; }
        public string OperatorIn { get; set; }
        public string PassesIn { get; set; }
        public string AppIn { get; set; }
        public string CallIn { get; set; }
        public string Out { get; set; }
        public string FOC { get; set; }
        public string Clamps { get; set; }
        public string Cash { get; set; }
        public string EPay { get; set; }
        public string TotalOperatorIn { get; set; }
        public string TotalPassesIn { get; set; }
        public string TotalAppIn { get; set; }
        public string TotalCallIn { get; set; }
        public string TotalOut { get; set; }
        public string TotalFOC { get; set; }
        public string TotalClamps { get; set; }
        public string TotalCash { get; set; }
        public string TotalEPay { get; set; }
        public decimal Amount { get; set; }        
    }
}
