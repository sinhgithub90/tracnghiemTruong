using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestLinks.Models.VM
{
    public class QuizAnswerUploadViewModel
    {
        public string StudentId { get; set; }

        public int LinkId { get; set; }

        public string Token { get; set; }

        public ICollection<QuizAnswerFile> Files { get; set; }

        public UploadLink UploadLink { get; set; }

    }
}
