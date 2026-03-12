using Microsoft.AspNetCore.Identity;

namespace TestLinks.Models
{
    public class ApplicationUser: IdentityUser
    {
        //public string Fullname { get; set; }
        public string DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        public string MSSV { get; set; }
    }
}
