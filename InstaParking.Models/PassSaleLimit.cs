using System;

namespace InstaParking.Models
{
    public class PassSaleLimit
    {
        public int PassSaleLimitID { get; set; }
        public int PassTypeID { get; set; }
        public int VehicleTypeID { get; set; }
        public int LimitPercentage { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string VehicleTypeName { get; set; }
        public string PassTypeName { get; set; }
    }
}
