using System.ComponentModel.DataAnnotations;

namespace PassAuthWebApp_.Areas.Identity.Data
{
    //    public class PasswordHistory
    //    {
    //        public int Id { get; set; }

    //        public string UserId { get; set; }

    //        public string HashedPassword1 { get; set; }
    //        public string HashedPassword2 { get; set; }
    //        public string HashedPassword3 { get; set; }
    //        public string HashedPassword4 { get; set; }
    //        public string HashedPassword5 { get; set; }
    //        public string HashedPassword6 { get; set; }
    //        public string PasswordCounter { get; set; }

    //        public DateTime CreatedDate { get; set; }

    //        public ApplicationUser User { get; set; }
    //    }
    public class PasswordHistory
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Password1 { get; set; }
        public string Password2 { get; set; }
        public string Password3 { get; set; }
        public string Password4 { get; set; }
        public string Password5 { get; set; }
        public string Password6 { get; set; }

        public ApplicationUser User { get; set; }

        public DateTime LastUpdated { get; set; }  // For managing updates
    }

}
