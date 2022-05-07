namespace WebApiINMO.Middlewares
{

    public static class LoggerResponseHTTPMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggerResponseHttp(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoggerResponseHTTPMiddleware>();
        }
    }


    public class LoggerResponseHTTPMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<LoggerResponseHTTPMiddleware> logger;

        public LoggerResponseHTTPMiddleware(RequestDelegate next, ILogger<LoggerResponseHTTPMiddleware> logger )
        {
            this.next = next;
            this.logger = logger;
        }

        // Invoke o InvokeAsync
        public async Task InvokeAsync(HttpContext context)
        {
            using (var ms = new MemoryStream())
            {
                var respBody = context.Response.Body;

                context.Response.Body = ms;

                await next(context);

                // Cuando terminan todos los md
                ms.Seek(0, SeekOrigin.Begin);
                string resp = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(respBody);
                context.Request.Body = respBody;

                logger.LogInformation(resp);

            }
        }

    }
}
