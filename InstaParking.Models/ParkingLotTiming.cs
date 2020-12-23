using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class ParkingLotTiming
    {
        public int ParkingLotTimingID { get; set; }
        public int LotID { get; set; }
        public string DayOfWeek { get; set; }
        public string LotOpenTime { get; set; }
        public string LotCloseTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
