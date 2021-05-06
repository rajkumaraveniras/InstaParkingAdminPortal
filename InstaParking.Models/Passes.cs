using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
   public class Passes
    {
        public int PassPriceID { get; set; }
        public int PassTypeID { get; set; }
        public string PassTypeName { get; set; }
        public string PassCode { get; set; }
        public String PassName { get; set; }
        public String StationAccess { get; set; }
        public String Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
       // public bool NFCApplicable { get; set; }
       // public String NFCCardPrice { get; set; }
        public int VehicleTypeID { get; set; }
        public string VehicleTypeName { get; set; }
        public decimal Price { get; set; }
        public string PassDescription { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool selected { get; set; }
    }
}
