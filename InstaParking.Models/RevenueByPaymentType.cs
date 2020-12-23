namespace InstaParking.Models
{
    public class RevenueByPaymentType
    {
        public string PaymentType { get; set; } 
        //public string In { get; set; }
        //public string Out { get; set; }
      
        public string OperatorIn { get; set; }
        public string AppIn { get; set; }
        public string CallIn { get; set; }
        public string OperatorInTotal { get; set; }
        public string AppInTotal { get; set; }
        public string CallInTotal { get; set; }
        public decimal Amount { get; set; }
    }
}
