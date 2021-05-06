using System;

namespace InstaParking.Models
{
    public class Charges
    {
        public int ChargesID { get; set; }
        public int VehicleTypeID { get; set; }
        public decimal ClampFee { get; set; }
        public decimal NFCTagPrice { get; set; }
        public decimal BlueToothTagPrice { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string VehicleTypeCode { get; set; }
      
    }
}
