using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
   public class CardType
    {
        public int CardTypeID { get; set; }
        public string CardTypeCode { get; set; }
        public string CardTypeName { get; set; }
        public bool IsActive { get; set; }
    }
}
