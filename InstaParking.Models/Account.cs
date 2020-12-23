namespace InstaParking.Models
{
    public class Account
    {
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ContactNumber { get; set; }
        public string AlternateNumber { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string GSTNumber { get; set; }
        public string WhatsAppNumber { get; set; }
        public string SupportContactNumber { get; set; }
        public string SupportEmailID { get; set; }
        public int Distance { get; set; }
        public string CompanyLogo { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
    }
}
