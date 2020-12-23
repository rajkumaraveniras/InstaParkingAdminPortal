using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class Modules
    {
        public Modules()
        {
            SubModules = new List<SubModule>();
        }
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public int Parent_ModuleID { get; set; }
        public string Parent_ModuleName { get; set; }

        public string RootUrl { get; set; }
        public int MenuItemOrder { get; set; }

        public bool IsAssign { get; set; }
        public bool master { get; set; }
        public IList<SubModule> SubModules { get; set; }

        public string IconName { get; set; }
    }
}
