using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class ApplicationType
    {
        public int ApplicationTypeID { get; set; }
        public string ApplicationTypeCode { get; set; }
        public string ApplicationTypeName { get; set; }
        public string ApplicationTypeDesc { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
