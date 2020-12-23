using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class StationOccupancy
    {
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public string Occupancy { get; set; }
    }
}
