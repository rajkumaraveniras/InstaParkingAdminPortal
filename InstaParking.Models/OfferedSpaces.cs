using System;

namespace InstaParking.Models
{
    public class OfferedSpaces
    {
        public int OfferMySpaceID { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string OtherDetails { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}
