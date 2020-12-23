using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class LotPrice
    {
        public int PriceID { get; set; }
        public int LocationParkingLotID { get; set; }
        public int ApplicationTypeID { get; set; }
        public string ApplicationTypeName { get; set; }
        public int VehicleTypeID { get; set; }
        public string VehicleTypeName { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
