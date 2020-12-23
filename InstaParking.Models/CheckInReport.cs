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
        public int Total { get; set; }
    }
}
