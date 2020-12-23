using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class SubModule
    {
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public string RootUrl { get; set; }
        public int MenuItemOrder { get; set; }
        public bool IsAssign { get; set; }
        public int Parent_ModuleID { get; set; }
    }
}
