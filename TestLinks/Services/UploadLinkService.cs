using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TestLinks.Data;
using System.Text;
using TestLinks.Utils;

namespace TestLinks.Services
{
    public class UploadLinkService
    {
        private readonly ApplicationDbContext _context;

        public UploadLinkService(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<string> Login(int linkId, string studentId, string password)
        {
            var token = String.Empty;
            
            var link = await _context.UploadLinks.FindAsync(linkId);

            if (link != null && link.IsValid(password)) {
                token = CryptoUtils.CreateToken(studentId, link.Salt);
            }

            return token;
        }

        public async Task<bool> IsValid(int linkId, string studentId, string token)
        {
            var link = await _context.UploadLinks.FindAsync(linkId);
            if (link != null)
            {
                var _token = CryptoUtils.CreateToken(studentId, link.Salt);
                if (token == _token)
                {
                    if (link.Available)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
