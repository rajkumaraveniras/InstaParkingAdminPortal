using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaParking.Models
{
    public class User
    {
        public int UserID { get; set; }
        public int UserTypeID { get; set; }
        public string UserTypeName { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public int SupervisorID { get; set; }

        public string Supervisor { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string JoiningDate { get; set; }
        public decimal Salary { get; set; }
        public string EPFAccountNumber { get; set; }
        public string AltPhoneNumber { get; set; }
        public string AadharNumber { get; set; }
        public string PANNumber { get; set; }
        public string Photo { get; set; }
        public string AadharPhoto { get; set; }
        public string PANPhoto { get; set; }

        public string AssignedLocationID { get; set; }
        public int AssignedLotID { get; set; }
        public string Address { get; set; }

        public string AssignUserLoginTime { get; set; }
        public bool IsOperator { get; set; }

        public int AbsentUserID { get; set; }
       public bool OperatorExist { get; set; }
    }
}
