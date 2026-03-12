using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestLinks.Models.VM
{
    public class LoginCredentialViewModel
    {
        [Required]
        public string StudentId { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
