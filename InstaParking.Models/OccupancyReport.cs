using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class OccupancyReport
    {
        public string Station { get; set; }
        public string ParkingLot { get; set; }
        public string Capacity { get; set; }
        public string CurrentlyParked { get; set; }
        public string Occupancy { get; set; }
    }
}
