using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Utils
{
    public class RequestSeparatorMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestSeparatorMiddleware(RequestDelegate next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Log.Information("Begin Request: {verb} {path}", context.Request.Method, context.Request.Path);

            await _next(context);

            if (context.Response.StatusCode == 302)
            {
                Log.Information("Redirected to: {redirect}", context.Response.Headers["Location"].First());
            }

            Log.Information("End Response -------------------------------------");
        }
    }
}