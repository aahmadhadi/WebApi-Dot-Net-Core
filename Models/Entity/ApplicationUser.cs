using Microsoft.AspNetCore.Identity;

namespace BasicWebApi.Models.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string Address { get; set; }
    }
}