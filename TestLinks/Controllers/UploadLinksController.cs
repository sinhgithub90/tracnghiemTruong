using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestLinks.Data;
using TestLinks.Models;
using TestLinks.Models.VM;
using TestLinks.Services;

namespace TestLinks.Controllers
{
    [Authorize]
    public class UploadLinksController : Controller
    {
        private readonly FileUploadService _fileUploadService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UploadLinksController> _logger;

        public UploadLinksController(ApplicationDbContext context, 
            ILogger<UploadLinksController> logger,
            FileUploadService fileUploadService)
        {
            _logger = logger;
            _fileUploadService = fileUploadService;
            _context = context;
        }

        [Authorize(Roles = Constants.RoleTeacher)]
        // GET: TestSchedules
        public async Task<IActionResult> Index(int? pageIndex,
            [FromServices] UserManager<ApplicationUser> userManager,
            string searchString)
        {
            //_logger.LogInformation("called weather forecast");
            var q = (from l in _context.UploadLinks
                     select l );

            if(!User.IsInRole(Constants.RoleAdmin))
            {
                var user = await userManager.GetUserAsync(HttpContext.User);
                q = q.Where(e => e.Course.DepartmentId == user.DepartmentId);
            }

            //q = q.OrderByDescending(e => e.Id);
            q  = q.OrderBy(e => e.TimeOpen)
                .ThenBy(e => e.CourseId);

            if (!String.IsNullOrEmpty(searchString))
            {
                q = q.Where(e => e.Description.Contains(searchString)
                || e.Course.Name.Contains(searchString));
            }

            var vm = new PaginatedViewModel<UploadLink>()
            {
                Items = await PaginatedList<UploadLink>.CreateAsync(q,
                  pageIndex ?? 1),
                Filters = {
                    { "searchString", searchString },
                }
            };
            return View(vm);
        }

        [Authorize(Roles = Constants.RoleTeacher)]
        // GET: TestSchedules/Details/5
        public async Task<IActionResult> Details(int? id, string searchString, int? pageIndex)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testSchedule = await _context.UploadLinks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testSchedule == null)
            {
                return NotFound();
            }

            ViewData["searchString"] = searchString;
            ViewData["pageIndex"] = pageIndex;

            return View(testSchedule);
        }

        [Authorize(Roles = Constants.RoleAdmin)]
        // GET: TestSchedules/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = CreateCourseSelectGroup();
            return View(new UploadLink());
        }

        // POST: TestSchedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = Constants.RoleAdmin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,TimeOpen,TimeClose,CourseId,CourseYear,CourseTerm,MaxFileSize,MaxFiles,AlllowFileTypes")] UploadLink testSchedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(testSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["CourseId"] = CreateCourseSelectGroup();

            return View(testSchedule);
        }


        private List<SelectListItem> CreateCourseSelectGroup()
        {
            var groups = new Dictionary<string, SelectListGroup>();
            foreach (var item in _context.Departments)
            {
                groups[item.Id] = new SelectListGroup() {
                    Name = item.Name
                };
            }
            var courses = new List<SelectListItem>();
            foreach (var item in _context.Courses)
            {
                courses.Add(new SelectListItem() { 
                    Value = item.Id,
                    Text = item.Name,
                    Group = groups[item.DepartmentId]
                });
            }
            return courses;
        }


        // GET: TestSchedules/Edit/5
        [Authorize(Roles = Constants.RoleAdmin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testSchedule = await _context.UploadLinks.FindAsync(id);
            if (testSchedule == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = CreateCourseSelectGroup();

            return View(testSchedule);
        }

        // POST: TestSchedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.RoleAdmin)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,TimeOpen,TimeClose,CourseId,CourseYear,CourseTerm,MaxFileSize,MaxFiles,AlllowFileTypes")] UploadLink testSchedule)
        {
            var model = await _context.UploadLinks.FindAsync(id);

            if (id != testSchedule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (await TryUpdateModelAsync(model)) {
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestScheduleExists(testSchedule.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = CreateCourseSelectGroup();

            return View(testSchedule);
        }

        // GET: TestSchedules/Delete/5
        [Authorize(Roles = Constants.RoleAdmin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testSchedule = await _context.UploadLinks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testSchedule == null)
            {
                return NotFound();
            }

            return View(testSchedule);
        }

        // POST: TestSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.RoleAdmin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var testSchedule = await _context.UploadLinks.FindAsync(id);
            _context.UploadLinks.Remove(testSchedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TestScheduleExists(int id)
        {
            return _context.UploadLinks.Any(e => e.Id == id);
        }

        [HttpPost]
        [Authorize(Roles = Constants.RoleTeacher)]
        public async Task<IActionResult> UploadFiles(int id,
           [Bind("Files,ReturnUrl")] UploadFiles uploadFiles)
        {
            // save file
            foreach (var file in uploadFiles.Files)
            {
                var model = await _fileUploadService.ProcessUpload(file, (fileName,  ext,  fileId, length)
                 => new QuizFile()
                 {
                     Name = fileName,
                     Ext = ext,
                     Id = fileId,
                     UploadLinkId = id,
                     Length = length
                 });

                if(model != null)
                    _context.Add(model);
            }   // persist models to database
            await _context.SaveChangesAsync();

            return Redirect(uploadFiles.ReturnUrl);
        }

        [Authorize(Roles = Constants.RoleTeacher)]
        public async Task<IActionResult> Download(string id)
        {
            var info = await _context.UploadFiles.FindAsync(id);
            if (info == null)
            {
                return NotFound();
            }
            var contentType = MimeTypes.MimeTypeMap.GetMimeType(info.Ext);
            var file = _fileUploadService.GetFile(id);
            var fileName = $"{info.Name}{info.Ext}";
            return PhysicalFile(file, contentType, fileName);
        }

        [Authorize(Roles = Constants.RoleTeacher)]
        public async Task<IActionResult> Remove(string id, string url)
        {
            var info = await _context.UploadFiles.FindAsync(id);
            if (info == null)
            {
                return NotFound();
            }
            _context.Remove(info);
            _fileUploadService.Remove(id);
            await _context.SaveChangesAsync();
            return Redirect(url);
        }

        [Authorize(Roles = Constants.RoleAdmin)]
        public async Task<IActionResult> Activate(int id,
            int? pageIndex,
            string searchString)
        {
            var link = await _context.UploadLinks.FindAsync(id);
            if (link != null)
            {
                ViewData["pageIndex"] = pageIndex;
                ViewData["searchString"] = searchString;

                return View(link);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = Constants.RoleAdmin)]
        public async Task<IActionResult> Activate(int id,
            int? pageIndex,
            string searchString, 
            [Bind("Password")] LinkActivate model)
        {
            var link = await _context.UploadLinks.FindAsync(id);
            if (link != null)
            {
                if (ModelState.IsValid)
                {
                    link.Activate(model.Password);
                    _context.Update(link);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new
                    {
                        pageIndex,
                        searchString,
                    });
                }

                return View(link);
            }
            return NotFound();
        }

        [Authorize(Roles = Constants.RoleAdmin)]
        public async Task<IActionResult> Deactivate(int id, int? pageIndex,
            string searchString)
        {
            var link = await _context.UploadLinks.FindAsync(id);
            if (link != null)
            {
                link.Deactivate();
                _context.Update(link);
                await _context.SaveChangesAsync();

                return RedirectToAction(
                    nameof(Index), new
                    {
                        pageIndex,
                        searchString
                    });
            }
            return NotFound();
        }

        [Authorize(Roles = Constants.RoleTeacher)]
        public async Task<IActionResult> DownloadFiles(int id)
        {
            try
            {
                var link = await _context.UploadLinks.FindAsync(id);
                var files = link.QuizAnswerFiles;

                var zipFileName = $"{link.CourseId}_{link.CourseYear}_{link.CourseTerm}_{link.Id}.zip";
                
                if (!Directory.Exists(_fileUploadService.TemporyDir)) {
                    Directory.CreateDirectory(_fileUploadService.TemporyDir);
                }

                string zipPath = Path.Combine(_fileUploadService.TemporyDir, zipFileName);
                if (System.IO.File.Exists(zipPath))
                {
                    _logger.LogInformation($"Delete zip {zipFileName}");
                    System.IO.File.Delete(zipPath);
                }

                using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    foreach (var entry in files)
                    {
                        var file = _fileUploadService.GetFile(entry.Id);
                        if (file != null)
                        {
                            var entryName = $"{entry.StudentId}/{entry.Name}{entry.Ext}";
                            _logger.LogInformation($"create entry {file}: {entryName}");
                            archive.CreateEntryFromFile(file,  entryName);
                        }
                        else 
                        {
                            _logger.LogWarning($"zip null {file}");
                        }
                    }
                    var quizFiles = link.QuizFiles;
                    foreach (var entry in quizFiles)
                    {
                        var file = _fileUploadService.GetFile(entry.Id);
                        if (file != null)
                        {
                            var entryName = $"GiaoVienBoSung/{entry.Name}{entry.Ext}";
                            _logger.LogInformation($"create entry {file}: {entryName}");
                            archive.CreateEntryFromFile(file, entryName);
                        }
                        else
                        {
                            _logger.LogWarning($"zip null {file}");
                        }
                    }
                }

                var contentType = MimeTypes.MimeTypeMap.GetMimeType(".zip");
                return PhysicalFile(zipPath, contentType, zipFileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        [Authorize(Roles = Constants.RoleTeacher)]
        public async Task<IActionResult> ExportLink()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = Constants.RoleTeacher)]
        public async Task<IActionResult> ExportLink(int year, int term)
        {
            var links = _context.UploadLinks
                .Where(e => e.CourseYear == year && e.CourseTerm == term)
                .OrderBy(e => e.CourseId)
                .ThenBy(e => e.Order);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("links");
                var currentRow = 1;
                
                worksheet.Cell(currentRow, 1).Value = "Mã học phần";
                worksheet.Cell(currentRow, 2).Value = "Mã lớp học phần";
                worksheet.Cell(currentRow, 3).Value = "Tên học phần";
                worksheet.Cell(currentRow, 4).Value = "Giờ bắt đầu";
                worksheet.Cell(currentRow, 5).Value = "Giờ kết thúc";
                worksheet.Cell(currentRow, 6).Value = "Link";
                worksheet.Cell(currentRow, 7).Value = "Phòng";
                worksheet.Cell(currentRow, 8).Value = "Mật khẩu";

                foreach (var link in links)
                {
                    var fullUrl = this.Url.Action("Login", "UploadTests", new { id = link.Id }, this.Request.Scheme);
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = link.CourseId;
                    worksheet.Cell(currentRow, 2).Value = link.CourseInstanceId;
                    worksheet.Cell(currentRow, 3).Value = link.Course.Name;
                    worksheet.Cell(currentRow, 4).Value = link.TimeOpen.ToString("dd/MM/yyyy HH:mm") ;
                    worksheet.Cell(currentRow, 5).Value = link.TimeClose.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cell(currentRow, 6).Value = fullUrl;
                    worksheet.Cell(currentRow, 7).Value = $"P.{link.Room}";
                    worksheet.Cell(currentRow, 8).Value = link.Password;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"link_{year}_{term}.xlsx");
                }
            }
        }

        [Authorize(Roles = Constants.RoleTeacher)]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = Constants.RoleTeacher)]
        public async Task<IActionResult> Upload(IFormFile file, int year, int term)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                var wb = new XLWorkbook(stream);
                var ws = wb.Worksheet(1);
             
      
                var links = new List<ExcelLinkViewModel>();
                var departments = _context.Departments.ToList();
                var courses = _context.Courses.ToList();

                var rows = ws.RangeUsed().RowsUsed().Skip(1); // Skip header row

                foreach (var row in rows)
                {
                    var info = new ExcelLinkViewModel()
                    {
                        // Tên	Tên HP	Số TC	Khoa QL     Môn	    Buổi thi	Số SV dự thi	Giờ BĐ	Ngày thi	Phòng thi	Link phòng thi
                        CourseId = row.Cell(2).GetString(),
                        CourseIntanceId = row.Cell(3).GetString(),
                        CourseName = row.Cell(4).GetString(),
                        CourseInstanceName = row.Cell(5).GetString(),
                        Credits = row.Cell(6).GetDouble(),
                        DepartmentId = row.Cell(7).GetString(),
                        DeparmentName = row.Cell(8).GetString(),
                        Order = row.Cell(9).GetDouble(),
                        StudentAttendants = row.Cell(10).GetDouble(),
                        Time = row.Cell(11).GetString(),
                        Date = row.Cell(12).GetDateTime(),
                        Room = row.Cell(13).GetString(),
                        Link = row.Cell(14).GetString(),
                    };
                    links.Add(info);
                }

                CultureInfo provider = CultureInfo.InvariantCulture;
                links = links.OrderBy(e => e.Order).ToList();
                foreach (var info in links)
                {
                    var course = courses.FirstOrDefault(e => e.Id.Equals(info.CourseId, StringComparison.OrdinalIgnoreCase));
                    var deparment = departments.FirstOrDefault(e => e.Id.Equals(info.DepartmentId, StringComparison.OrdinalIgnoreCase));

                    if (course == null) 
                    {
                        course = new Course()
                        {
                            Id = info.CourseId,
                            Name = info.CourseName,
                            UploadLinks = new List<UploadLink>()
                        };
                       
                        deparment.Courses.Add(course);
                        courses.Add(course);
                        _context.Update(deparment);
                    }
                    var date = CustomParse(info.Time, info.Date.ToString("dd/MM/yyyy"), provider);
                    int extraTime = 20;
                    int duration = UploadLink.CalculateTestDuration((int)info.Credits, extraTime);
                    var link = new UploadLink()
                    {
                        CourseInstanceId = info.CourseIntanceId,
                        Description = info.CourseInstanceName,
                        CourseYear = year,
                        CourseTerm = term,
                        TimeOpen = date,
                        TimeClose = date.AddMinutes(duration),
                        MaxFiles = 1,
                        Duration = duration,
                        ExtraTime = extraTime,
                        Room = info.Room,
                        Order = (int)info.Order,
                    };

                    link.Activate();

                    course.UploadLinks.Add(link);

                    if (_context.Entry(course).State != EntityState.Detached)
                    { 
                        _context.Update(course);
                    }
                }
                
                await _context.SaveChangesAsync();

            }

            
            return RedirectToAction(nameof(Index));
        }

        public  DateTime CustomParse( string time, string date, CultureInfo provider)
        {
            string str = $"{date.Substring(0, 10)} {time.Replace("h", ":")}";
            string format = "dd/MM/yyyy HH:mm";
            return DateTime.ParseExact(str, format, provider);
        }

        [Authorize(Roles = Constants.RoleTeacher)]
        public async Task<IActionResult> UpdateStats(int id)
        {
            var link = await _context.UploadLinks.FindAsync(id);

            var groups = (from info in link.QuizAnswerFiles
                group info by info.StudentId);

            foreach (var group in groups)
            {
                var info = link.UploadInfos.FirstOrDefault(e => e.LinkId == id && e.StudentId == group.Key);
                if (info == null)
                {
                    info = new TestUploadInfo()
                    {
                        StudentId = group.Key,
                        LinkId = link.Id,
                    };
                    link.UploadInfos.Add(info);
                }
                foreach (var file in group)
                {
                    if (!info.Files.Any(e => e.Id == file.Id))
                    {
                        info.Files.Add(file);
                    }
                }
            }

            _context.Update(link);
            foreach (var info in link.UploadInfos)
            {
                info.FileCount = info.Files.Count;
                if (_context.Entry(info).State != EntityState.Added)
                {
                    _context.Update(info);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new
            {
                id
            });
        }

        [Authorize(Roles = Constants.RoleTeacher)]
        public async Task<ActionResult> SubmitDetails(string studentId, int linkId)
        {
            var info = await _context.TestUploadInfos.Where(
                e => e.LinkId == linkId && e.StudentId == studentId
                ).FirstOrDefaultAsync();

            if (info == null)
            {
                return NotFound();
            }

            return View(info);
        }

        [Authorize(Roles = Constants.RoleTeacher)]
        public async Task<IActionResult> DownloadSubmit(string id)
        {
            var info = await _context.UploadFiles.FindAsync(id);
            if (info == null)
            {
                return NotFound();
            }
            var contentType = MimeTypes.MimeTypeMap.GetMimeType(info.Ext);
            var file = _fileUploadService.GetFile(id);
            var fileName = $"{info.Name}{info.Ext}";

            return PhysicalFile(file, contentType, fileName);
        }

        [Authorize(Roles = Constants.RoleTeacher)]
        public async Task<IActionResult> GetFile(string id)
        {
            var info = await _context.UploadFiles.FindAsync(id);
            if (info == null)
            {
                return NotFound();
            }
            var contentType = MimeTypes.MimeTypeMap.GetMimeType(info.Ext);
            var file = _fileUploadService.GetFile(id);
            var stream = new FileStream(file, FileMode.Open);
            return new FileStreamResult(stream, contentType);
        }

        [Authorize(Roles = Constants.RoleTeacher)]
        public async Task<IActionResult> VerifyUpload(int linkId, string studentId)
        {
            var info = await _context.TestUploadInfos
                .FirstOrDefaultAsync(e => e.LinkId == linkId && e.StudentId == studentId);

            info.Checked = true;
            _context.Update(info);
            await _context.SaveChangesAsync();

            //return Ok(info.Files);
            return RedirectToAction(nameof(Details), new
            {
                id = linkId
            });
        }

        [Authorize(Roles = Constants.RoleTeacher)]
        public async Task<IActionResult> GetUploadInfo(string studentId, int linkId)
        {
            var info = await _context.TestUploadInfos
               .FirstOrDefaultAsync(e => e.LinkId == linkId && e.StudentId == studentId);

            return Ok(info.Files);
        }
    }


}
