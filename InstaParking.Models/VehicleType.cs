using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class VehicleType
    {
        public int VehicleTypeID { get; set; }
        public string VehicleTypeCode { get; set; }
        public string VehicleTypeName { get; set; }
        public int WheelCount { get; set; }
        public int AxleCount { get; set; }
        public string VehicleTypeDesc { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string VehicleTypeIcon { get; set; }
    }
}
