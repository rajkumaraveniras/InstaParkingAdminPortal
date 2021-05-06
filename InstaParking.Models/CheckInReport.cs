namespace InstaParking.Models
{
    public class CheckInReport
    {
        public string Station { get; set; }
        public string ParkingLot { get; set; }
        public int App { get; set; }
        public int Pass { get; set; }
        public int Operator { get; set; }
        public int CallPay { get; set; }
        public int Out { get; set; }
        public int FOC { get; set; }
        public string AppTotal { get; set; }
        public string PassTotal { get; set; }
        public string OperatorTotal { get; set; }
        public string CallPayTotal { get; set; }
        public string OutTotal { get; set; }
        public string FOCTotal { get; set; }
        public int Total { get; set; }
    }
}
