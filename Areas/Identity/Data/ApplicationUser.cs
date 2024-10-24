using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PassAuthWebApp_.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        public DateTime LastPasswordChangedDate { get; set; } = DateTime.UtcNow;

        public DateTime LastLoginDate { get; set; } = DateTime.UtcNow;

        public bool IsDormant { get; set; } = false;
    }
}
