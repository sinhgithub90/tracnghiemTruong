using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestLinks.Models.VM
{
    public class LinkActivate
    {
        [Required]
        public string Password { get; set; }
    }
}
