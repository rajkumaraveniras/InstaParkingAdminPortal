using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class FOCReport
    {
        public string Station { get; set; }
        public string ParkingLot { get; set; }
        public string FOCReason { get; set; }
        public string FOCCount { get; set; }
        public string DueAmount { get; set; }
        public string Total { get; set; }
    }
}
