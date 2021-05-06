using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class LocationVehicleTypeMapper
    {
        public int LocationVehicleTypeMapperID { get; set; }
        public int LocationID { get; set; }
        public int VehicleTypeID { get; set; }
        public bool selected { get; set; }
    }
}
