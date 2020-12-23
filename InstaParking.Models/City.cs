using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class City
    {
        public int CityID { get; set; }
        public int StateID { get; set; }
        public string StateName { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
