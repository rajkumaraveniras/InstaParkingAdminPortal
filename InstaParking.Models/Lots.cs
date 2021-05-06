using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class Lots
    {
        public int LocationParkingLotID { get; set; }
        public int LocationID { get; set; }
        public String LocationName { get; set; }
        public int ParkingTypeID { get; set; }
        public string ParkingTypeName { get; set; }
        public int VehicleTypeID { get; set; }
        public string VehicleTypeName { get; set; }
        public int ParkingBayID { get; set; }
        public string ParkingBayName { get; set; }
        public int ParentLocationParkingLotID { get; set; }
        public string LocationParkingLotCode { get; set; }
        public string LocationParkingLotName { get; set; }
        public decimal Lattitude { get; set; }
        public decimal Longitude { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string Address { get; set; }
        public string LotImage { get; set; }
        public string LotImageName { get; set; }
        public string LotImageType { get; set; }
        public string PhoneNumber { get; set; }

        public string LotImage2 { get; set; }
        public string LotImageName2 { get; set; }
        public string LotImageType2 { get; set; }

        public string LotImage3 { get; set; }
        public string LotImageName3 { get; set; }
        public string LotImageType3 { get; set; }

        //public int LotVehicleAvailabilityID { get; set; }
        //public string LotVehicleAvailabilityName { get; set; }
        public bool IsHoliday { get; set; }
    }
}
