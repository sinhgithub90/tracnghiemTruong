using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestLinks.Models
{
    public class TestUploadInfo
    {
        [Display(Name = "Mã số sinh viên")]
        public string StudentId { get; set; }

        public virtual UploadLink Link { get; set; }

        public int LinkId { get; set; }

        public virtual ICollection<QuizAnswerFile> Files { get; set; } = new List<QuizAnswerFile>();

        [Display(Name = "Số file nộp")]
        public int FileCount { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime LastSubmit { get; set; }

        public bool Checked { get; set; }

        public string Fullname { get; set; }

    }
}
