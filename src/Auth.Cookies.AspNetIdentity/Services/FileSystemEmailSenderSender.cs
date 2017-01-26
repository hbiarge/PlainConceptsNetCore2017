using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Auth.Cookies.AspNetIdentity.Services
{
    public class FileSystemEmailSenderSender : IEmailSender
    {
        private readonly string _basePath;

        public FileSystemEmailSenderSender(IHostingEnvironment env)
        {
            _basePath = Path.Combine(env.ContentRootPath, "Mails");

            if (Directory.Exists(_basePath) == false)
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = $"Email: {email}\r\nSubject: {subject}\r\nMessage: {message}";

            var path = Path.Combine(_basePath, $"{DateTime.Now.ToFileTime()}.txt");

            File.WriteAllText(path, mail);

            return Task.FromResult(0);
        }
    }
}