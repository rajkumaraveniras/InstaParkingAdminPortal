using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
   public class UserLocationMapper
    {
        public UserLocationMapper()
        {
            userslist = new List<User>();
        }
        public int UserLocationMapperID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public int LocationParkingLotID { get; set; }
        public string LocationParkingLotName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
            
        public string LoginTime { get; set; }
        public List<User> userslist { get; set; }

        public int AssignedUserID { get; set; }
        public string AssignedUserName { get; set; }
        public string AssignUserLoginTime { get; set; }

        public int AbsentUserID { get; set; }
       public bool OperatorExist { get; set; }
    }
}
