using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class Locations
    {
        public int LocationID { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string LocationDesc { get; set; }
        public string Address { get; set; }
        public decimal Lattitude { get; set; }
        public decimal Longitude { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public int TagType { get; set; }/*27022021*/
        public List<Passes> PassAccess { get; set; }
    }
}
