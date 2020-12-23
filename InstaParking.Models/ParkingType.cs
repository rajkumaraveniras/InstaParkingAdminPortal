using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class ParkingType
    {
        public int ParkingTypeID { get; set; }
        public int ParkingSubTypeID { get; set; }
        public string ParkingTypeCode { get; set; }
        public string ParkingTypeName { get; set; }
        public string ParkingTypeDesc { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }        
    }
}
