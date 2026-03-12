

using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace TestLinks.Models
{
    public class UploadFilesViewModel
    {
        public ICollection<IFormFile> Files { get; set; }
        public string ReturnUrl { get; set; }
    }

}
