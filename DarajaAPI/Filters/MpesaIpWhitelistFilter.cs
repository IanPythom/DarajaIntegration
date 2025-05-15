using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using DarajaAPI.Models.Domain;
using Microsoft.Extensions.Options;

namespace DarajaAPI.Filters
{
    public class MpesaIpWhitelistFilter : IAsyncActionFilter
    {
        private readonly DarajaConfig _config;
        private readonly ILogger<MpesaIpWhitelistFilter> _logger;

        public MpesaIpWhitelistFilter(IOptions<DarajaConfig> config, ILogger<MpesaIpWhitelistFilter> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var whitelist = _config.Security?.IPWhitelist ?? Array.Empty<string>();
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress?.ToString();

            if (!whitelist.Contains(remoteIp))
            {
                _logger.LogWarning("Unauthorized IP attempt: {IP}", remoteIp);
                context.Result = new ContentResult
                {
                    Content = "Forbidden",
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
                return;
            }

            await next();
        }
    }
}
