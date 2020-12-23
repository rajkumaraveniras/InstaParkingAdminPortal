using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class AssignUserStation
    {
        public int UserLocationMapperID { get; set; }
        public int UserID { get; set; }
        public int LocationID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string LocationName { get; set; }
        public int LotID { get; set; }
        public string LotName { get; set; }
    }
}
