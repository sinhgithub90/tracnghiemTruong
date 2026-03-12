
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestLinks.Data;
using TestLinks.Models;

namespace TestLinks.Services
{
    public delegate UploadFile UploadFileDelegate(string fileName, string ext, string id, long length);

    public class FileUploadService
    {
        public string UploadDir { get; private set; }

        public string TemporyDir => Path.Combine(UploadDir, "Temp");

        public FileUploadService( IConfiguration configuration)
        {
            UploadDir = configuration["UploadDir"];
            if (!Directory.Exists(UploadDir))
            {
                Directory.CreateDirectory(UploadDir);
            }
            if (!Directory.Exists(TemporyDir))
            {
                Directory.CreateDirectory(TemporyDir);
            }
            // TODO: init upload dir
        }

        public async Task<UploadFile> ProcessUpload(IFormFile file, UploadFileDelegate modelCreator)
        {
            if (file.Length != 0) {
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var ext = Path.GetExtension(file.FileName);

                var newFileName =  Guid.NewGuid().ToString();
                var filePath = Path.Combine(UploadDir, newFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return modelCreator(fileName, ext, newFileName, file.Length);
            }
            return null;
        }

        internal string GetFile(string id)
        {
            var file = Path.Combine(UploadDir, id);
            string ret = null;
            if (System.IO.File.Exists(file))
            {
                ret = file;
            }
            return ret;
        }

        public void Remove(string id)
        {
            var file = Path.Combine(UploadDir, id);
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

    }
}
