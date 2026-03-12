using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TestLinks.Models
{
    public class Department
    {
        [Key]
        [Display(Name ="Mã khoa")]
        public string Id { get; set; }
        
        [Required]
        [Display(Name = "Tên khoa")]
        public string Name { get; set; }

        [InverseProperty("Department")]
        public virtual ICollection<Course> Courses { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }


    }
}
