using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestLinks.Models
{
    public class Course
    {
        [Key]
        [Display(Name ="Mã học phần")]
        public string Id { get; set; }

        [Required]
        [Display(Name="Tên học phần")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Khoa")]
        public string DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        public virtual ICollection<UploadLink> UploadLinks { get; set; }
    }
}