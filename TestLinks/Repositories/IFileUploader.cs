using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TestLinks.Models;

namespace TestLinks.Services
{
    public interface IFileUploader
    {
        string GetResourceName();
        Task<IActionResult> UploadFiles(int id, UploadFilesViewModel uploadFiles);
        Task<IActionResult> RemoveFile(int id, string name, string returnUrl);
        Task<IActionResult> DownloadFile(int id, string name);
        ClaimsPrincipal User { get; }
    }
}
