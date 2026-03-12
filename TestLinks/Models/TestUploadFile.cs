using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TestLinks.Models
{
    public class UploadFile
    {
        [Key]
        public string Id { get; set; }

        public string Ext { get; set; }

        public long Length { get; set; }

        public string Name { get; set; }

        public DateTime CreatedTime { get; set; }

        public virtual string Path { get; set; }

        public long Kb => Length / 1024;

        public double Mb => Math.Round(Length / (1024.0 * 1024.0), 2);

        public string FileName => $"{Name}{Ext}";
    }

    public class TestUploadFile: UploadFile 
    {
        public int? UploadLinkId { get; set; }
        
        [JsonIgnore]
        public virtual UploadLink UploadLink { get; set; }
    }

    public class QuizFile : TestUploadFile
    {

    }

    public class QuizAnswerFile: TestUploadFile
    {
        public int? UploadInfoId { get; set; }

        [JsonIgnore]
        public virtual TestUploadInfo UploadInfo { get; set; }

        public int Order { get; set; }

        public string StudentId { get; set; }

    }
}
