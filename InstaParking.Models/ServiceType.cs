using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class ServiceType
    {
        public int ServiceTypeID { get; set; }
        public string ServiceTypeCode { get; set; }
        public string ServiceTypeName { get; set; }
        public string ServiceTypeDesc { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }        
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }

        public string ServiceTypeImage { get; set; }
        public string IconName { get; set; }
        public string IconType { get; set; }
        public bool selected { get; set; } 
    }
}
