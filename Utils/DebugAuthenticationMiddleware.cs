using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Utils
{
    public class DebugAuthenticationMiddleware
    {
        private const string Key = "Debug:Auth";
        private readonly RequestDelegate _next;
        private readonly DebugAuthenticationOptions _options;

        public DebugAuthenticationMiddleware(RequestDelegate next, DebugAuthenticationOptions options)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _next = next;
            _options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            var requestOrder = GetOrder(context);
            Log.Information("{order}: Request: Before {stage} -> User:{user}", requestOrder, _options.Stage, GetUser(context));

            await _next(context);

            var responseOrder = GetOrder(context);
            Log.Information("{order}: Response: After {stage} -> StatusCode:{statusCode}", responseOrder, _options.Stage, context.Response.StatusCode);
        }

        private static int GetOrder(HttpContext context)
        {
            if (context.Items.TryGetValue(Key, out object order))
            {
                context.Items[Key] = ((int)order) + 1;
            }
            else
            {
                order = 1;
                context.Items[Key] = 2;
            }

            return (int)order;
        }

        private static string GetUser(HttpContext context)
        {
            return context.User.Identity.IsAuthenticated
                ? context.User.Identity.Name
                : "Not authenticated";
        }
    }
}
