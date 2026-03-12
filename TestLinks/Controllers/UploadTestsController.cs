using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestLinks.Data;
using TestLinks.Models;
using TestLinks.Models.VM;
using TestLinks.Services;

namespace TestLinks.Controllers
{
    [Authorize]
    public class UploadTestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UploadLinkService _uploadLinkService;
        private readonly FileUploadService _fileUploadService;
        readonly IDiagnosticContext _diagnosticContext;
        private readonly ReCaptcha _captcha;

        public UploadTestsController(
            ReCaptcha captcha,
            IDiagnosticContext diagnosticContext,
            ApplicationDbContext context,
            UploadLinkService uploadLinkService,
            FileUploadService fileUploadService)
        {
            _diagnosticContext = diagnosticContext ??
                throw new ArgumentNullException(nameof(diagnosticContext));
            this._context = context;
            this._uploadLinkService = uploadLinkService;
            this._fileUploadService = fileUploadService;
            _captcha = captcha;
        }

        public async Task<IActionResult> Login(int id)
        {
            var link = await _context.UploadLinks.FindAsync(id);
            if (!link.Available)
            {
                Log.Warning($"Link is not available link_id {id}");
            }

            return View(link);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(int id, [Bind("StudentId,Password")] LoginCredentialViewModel credential)
        {

            if (ModelState.IsValid
                    //|| Request.Form.ContainsKey("g-recaptcha-response")
                )
            {
                //var captcha = Request.Form["g-recaptcha-response"].ToString();
                //if (await _captcha.IsValid(captcha))
                {
                    var verifyToken = await _uploadLinkService.Login(id, credential.StudentId, credential.Password);
                    if (!string.IsNullOrEmpty(verifyToken))
                    {
                        var link = await _context.UploadLinks.FindAsync(id);
                        if (link.Available)
                        {
                            var info = await _context.TestUploadInfos.FirstOrDefaultAsync(e => e.LinkId == link.Id
                                && e.StudentId == credential.StudentId);
                            if (info == null)
                            {
                                info = new TestUploadInfo()
                                {
                                    StudentId = credential.StudentId,
                                    LinkId = link.Id,
                                };
                                info.LastLogin = DateTime.Now;
                                _context.Add(info);
                            }
                            else
                            {
                                info.LastLogin = DateTime.Now;
                                _context.Update(info);
                            }

                            await _context.SaveChangesAsync();

                            return RedirectToAction(nameof(Upload), new
                            {
                                linkId = id,
                                credential.StudentId,
                                verifyToken
                            });
                        }
                        else
                        {
                            Log.Warning($"Access link when it is not available, link_id {id}, student id {credential.StudentId}");
                            var files = (from file in _context.QuizAnswerFiles
                                         where file.UploadLinkId == id && file.StudentId == credential.StudentId
                                         select file).ToList();
                            var vm = new QuizAnswerUploadViewModel()
                            {
                                Files = files,
                                LinkId = id,
                                StudentId = credential.StudentId,
                                Token = verifyToken,
                                UploadLink = link,
                            };
                            return View("Result", vm);
                        }
                    }
                    Log.Error($"Invalid login token student Id {credential.StudentId}");
                }
            }

            return RedirectToAction(nameof(Login), new
            {
                id
            });
        }

        public async Task<IActionResult> Upload(
            int linkId, string studentId, string verifyToken)
        {

            if (await _uploadLinkService.IsValid(linkId, studentId, verifyToken))
            {
                var link = await _context.UploadLinks.FindAsync(linkId);

                var files = (from file in _context.QuizAnswerFiles
                             where file.UploadLinkId == linkId && file.StudentId == studentId
                             select file).ToList();

                var vm = new QuizAnswerUploadViewModel()
                {
                    Files = files,
                    LinkId = linkId,
                    StudentId = studentId,
                    Token = verifyToken,
                    UploadLink = link,
                };

                return View(vm);
            }
            else
            {
                Log.Warning($"Invalid upload student id {studentId} link_id {linkId}");
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(
            int linkId, string studentId, string verifyToken,
            [Bind("Files")] UploadFilesViewModel upload)
        {
            bool isNew = false;

            if (upload == null || upload.Files == null || upload.Files.Count == 0)
                return RedirectToAction(nameof(Upload), new
                {
                    linkId,
                    studentId,
                    verifyToken
                });

            if (await _uploadLinkService.IsValid(linkId, studentId, verifyToken))
            {
                var link = await _context.UploadLinks.FindAsync(linkId);
                var testInfo = await _context.TestUploadInfos.FirstOrDefaultAsync(e => e.LinkId == linkId && e.StudentId == studentId);
                if (testInfo == null)
                {
                    testInfo = new TestUploadInfo()
                    {
                        LinkId = linkId,
                        StudentId = studentId,
                    };
                    isNew = true;
                    _context.TestUploadInfos.Add(testInfo);

                    Log.Information($"New upload info link {linkId} studentId {studentId}");
                }

                foreach (var file in upload.Files)
                {
                    if (testInfo.Files.Count < link.MaxFiles
                        && file.Length < link.MaxLength)
                    {
                        var model = await _fileUploadService.ProcessUpload(file,
                        (fileName, ext, id, length) => new QuizAnswerFile()
                        {
                            Name = fileName,
                            Ext = ext,
                            Id = id,
                            UploadLinkId = linkId,
                            StudentId = studentId,
                            Length = length
                        });

                        if (model != null)
                        {
                            testInfo.Files.Add(model as QuizAnswerFile);
                            //Log.Information($"Upload file {model.FileName} link {linkId} studentId {studentId}");
                        }
                        else
                        {
                            Log.Warning($"Upload failed file {model.FileName} link {linkId} studentId {studentId}");
                        }
                    }
                }

                testInfo.FileCount = testInfo.Files.Count;

                Log.Information($"Total file upload {upload.Files.Count}, total success {testInfo.FileCount} link {linkId} studentId {studentId}");

                if (!isNew)
                {
                    _context.Update(testInfo);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Upload), new
                {
                    linkId,
                    studentId,
                    verifyToken,
                });
            }

            Log.Error($"Invalid Upload for link {linkId} studentId {studentId}");

            return InvalidSession();
        }

        public async Task<IActionResult> Download(string id, int linkId, string studentId, string verifyToken)
        {
            if (await _uploadLinkService.IsValid(linkId, studentId, verifyToken))
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

            Log.Error($"Invalid download link {linkId} studentId {studentId}");


            return InvalidSession();
        }

        public async Task<IActionResult> Remove(string id, int linkId, string studentId, string verifyToken)
        {

            if (await _uploadLinkService.IsValid(linkId, studentId, verifyToken))
            {
                var testInfo = await _context.TestUploadInfos.FirstOrDefaultAsync(e => e.LinkId == linkId && e.StudentId == studentId);
                var info = testInfo.Files.First(e => e.Id == id);

                if (info == null
                    || info.StudentId != studentId
                    || info.UploadLinkId != linkId)
                {
                    return InvalidSession();
                }

                testInfo.Files.Remove(info);
                _context.QuizAnswerFiles.Remove(info);

                testInfo.FileCount = testInfo.Files.Count;
                _context.TestUploadInfos.Update(testInfo);
                _fileUploadService.Remove(id);

                await _context.SaveChangesAsync();

                Log.Information($"Remove success file {id} link {linkId} studentId {studentId}");

                return RedirectToAction(nameof(Upload), new
                {
                    linkId,
                    studentId,
                    verifyToken
                });
            }

            Log.Error($"Invalid removing file {id} link {linkId} studentId {studentId}");

            return InvalidSession();
        }

        private IActionResult InvalidSession()
        {
            return View();
        }
    }
}
