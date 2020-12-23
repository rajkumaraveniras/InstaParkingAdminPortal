using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
   public class RevenueByVehicle
    {
        public string VehicleType { get; set; }
        public string Station { get; set; }
        public string ParkingLot { get; set; }
        public string In { get; set; }
        public string Out { get; set; }
        public string FOC { get; set; }
        public string Clamps { get; set; }
        public string Cash { get; set; }
        public string EPay { get; set; }
        public string TotalIn { get; set; }
        public string TotalOut { get; set; }
        public string TotalFOC { get; set; }
        public string TotalClamps { get; set; }
        public string TotalCash { get; set; }
        public string TotalEPay { get; set; }
        public decimal Amount { get; set; }
    }
}
