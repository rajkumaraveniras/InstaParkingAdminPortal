using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
   public  class OperatorHoursReport
    {
        public string Supervisor { get; set; }
        public string Operator { get; set; }
        public string LocationParkingLotName { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }
        public string TotalHours { get; set; }
        public string TotalDays { get; set; }        
        public string Total { get; set; }
    }
}
