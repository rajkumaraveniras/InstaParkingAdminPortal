using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class RevenueByDueAmount
    {
        public string Station { get; set; }
        public string ParkingLot { get; set; }
        public string Cash { get; set; }
        public string EPay { get; set; }
        public string TotalCash { get; set; }
        public string TotalEPay { get; set; }
        public decimal Amount { get; set; }
    }
}
