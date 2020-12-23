using System;

namespace InstaParking.Models
{
    public class Charges
    {
        public int ChargesID { get; set; }
        public decimal ClampFee { get; set; }
        public decimal ClampFeeLimit { get; set; }
        public decimal ClampFeefor4W { get; set; }
        public decimal ClampFeeLimitfor4W { get; set; }
        public decimal PriceLimitForTwoWheller { get; set; }
        public decimal PriceLimitForFourWheller { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
      
    }
}
