using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class ParkingBay
    {
        public int ParkingBayID { get; set; }
        public int LocationParkingLotID { get; set; }
        public int VehicleTypeID { get; set; }
        public string VehicleTypeName { get; set; }
        public string ParkingBayCode { get; set; }
        public string ParkingBayName { get; set; }
        public int NumberOfBays { get; set; }
        public string ParkingBayRange { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
