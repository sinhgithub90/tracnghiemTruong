using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TestLinks.Utils;
using System.Linq;

namespace TestLinks.Models
{
    public class UploadInfo
    {
        public string Controller { get; set; }
        public string Area { get; set; }
        public int ID { get; set; }
        public bool ReadOnly { get; set; } = false;
        public string Action { get; set; } = "UploadFiles";
        public string ReturnUrl { get; set; }
    }

    public class UploadLink
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ghi chú")]
        public string Description { get; set; }

        [Display(Name = "Bắt đầu nộp bài lúc")]
        public DateTime TimeOpen { get; set; } = DateTime.Today;

        public bool IsValid(string password)
        {
            var hashString = this.CreateHash(password);
            return hashString == HashedPassword;
        }

        private string CreateHash(string password)
        {
            var str = $"{password}{this.Salt}";
            return CryptoUtils.CreateMD5Hash(str);
        }

        [Display(Name = "Thời hạn")]
        public DateTime TimeClose { get; set; } = DateTime.Today.AddDays(1);

        [Required]
        [Display(Name = "Học phần")]
        public string CourseId { get; set; }

        [Display(Name = "Mã LHP")]
        public string CourseInstanceId { get; set; }

        public virtual Course Course { get; set; }

        [Display(Name ="Năm học")]
        public int CourseYear { get; set; } = DateTime.Now.Year;

        [Display(Name = "Học kì")]
        public int CourseTerm { get; set; } = 1;

        [Display(Name = "Kích thước file tối đa (Mb)")]
        public decimal MaxFileSize { get; set; } = 50;

        public decimal MaxLength => MaxFileSize * 1024 * 1024;

        [Display(Name ="Số lượng file tối đa")]
        public int MaxFiles { get; set; } = 5;

        [Display(Name ="Loại file")]
        public string AlllowFileTypes { get; set; } = "jpg,jpeg,pdf,png";

        public string Password { get; set; }

        public string HashedPassword { get; set; }

        public string Salt { get; set; }

        public bool Active { get; set; }

        public virtual ICollection<QuizFile> QuizFiles { get; set; }

        public virtual ICollection<QuizAnswerFile> QuizAnswerFiles { get; set; }

        internal void Activate(string password)
        {
            this.Salt = Guid.NewGuid().ToString();
            this.HashedPassword = CreateHash(password);
            this.Password = password;
            this.Active = true;
        }

        internal void Activate(int length = 6)
        {
            var pass = RandomString(length);
            this.Activate(pass);
        }

        public static string RandomString(int length)
        {
            var rnd = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }

        public void Deactivate()
        {
            this.Active = false;
        }

        public string[] FileTypes {
            get {
                if (string.IsNullOrEmpty(this.AlllowFileTypes)) 
                    return new[] { ".*" };

                var tokens = this.AlllowFileTypes.Split(",");
                return (from token in tokens
                        select $".{token}").ToArray();
            }
        }

        public bool Available {
            get {
                return DateTime.Now >= TimeOpen && DateTime.Now <= TimeClose;
            }
        }

        public int Duration { get; set; }

        public int ExtraTime { get; set; } = 20;


        [Display(Name = "Phòng thi")]
        public string Room { get; set; }

        public int Order { get; set; }

        public static int CalculateTestDuration(int credits, int extraMinutes = 20)
        {
            // Quy định 1 tín chỉ 60p , 2tc 90, 3 trở lên là 120
            var duration = 0;
            if (credits == 1) duration = 60;
            else if (credits == 2) duration = 90;
            else duration = 120;

            duration = duration + extraMinutes;
            return duration;
        }

        public virtual ICollection<TestUploadInfo> UploadInfos { get; set; }
    }
}