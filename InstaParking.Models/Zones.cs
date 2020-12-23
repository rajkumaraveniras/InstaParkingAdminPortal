using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class Zones
    {
        public int ZoneID { get; set; }
        public int CityID { get; set; }
        public string City { get; set; }
        public string ZoneCode { get; set; }
        public string ZoneName { get; set; }
        public string ZoneDesc { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
