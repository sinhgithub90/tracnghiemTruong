using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace TestLinks.Models.VM
{
    public class UploadFiles
    {
        public ICollection<IFormFile> Files { get; set; }
        public string ReturnUrl { get; set; }
    }
}
