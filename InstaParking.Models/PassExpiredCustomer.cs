using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class PassExpiredCustomer
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string VehicleType { get; set; }
        public string VehicleNumber { get; set; }
        public string TypeofPass { get; set; }
        public string PassExpiryDate { get; set; }
    }
}
