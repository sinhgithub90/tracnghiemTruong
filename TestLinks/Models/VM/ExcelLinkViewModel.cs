using System;

namespace TestLinks.Models.VM
{
    public class ExcelLinkViewModel
    {
        // Tên	Tên HP	Số TC	Khoa QL môn	Buổi thi	Số SV dự thi	Giờ BĐ	Ngày thi	Phòng thi	Link phòng thi
        public string CourseId   { get; set; } // 2
        public string CourseIntanceId { get; set; }
        public string CourseName { get; set; }
        public string CourseInstanceName { get; set; }
        public double Credits { get; set; }
        public string DepartmentId { get; set; }
        public string DeparmentName { get; set; }
        public double Order { get; set; }
        public double StudentAttendants { get; set; }
        public string Time { get; set; }
        public DateTime Date { get; set; }
        public string Room { get; set; }
        public string Link { get; set; }
    }
}
