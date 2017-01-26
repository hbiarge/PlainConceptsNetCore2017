using System;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace Auth.Cookies.AspNetIdentity.Services
{
    public class FileSystemSmsSender : ISmsSender
    {
        private readonly string _basePath;

        public FileSystemSmsSender(IHostingEnvironment env)
        {
            _basePath = Path.Combine(env.ContentRootPath, "Sms");

            if (Directory.Exists(_basePath) == false)
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public Task SendSmsAsync(string number, string message)
        {
            var sms = $"Number: {number}\r\nMessage: {message}";

            var path = Path.Combine(_basePath, $"{DateTime.Now.ToFileTime()}.txt");

            File.WriteAllText(path, sms);

            return Task.FromResult(0);
        }
    }
}
