using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace SolutionName.Infrastructure.Localization
{
    public class RequestCultureMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var currentLanguage = context.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
            var acceptLanguageHeader = context.Request.Headers["Accept-Language"].ToString();
            var browserLanguage = acceptLanguageHeader.Length >= 2 ? acceptLanguageHeader[..2] : "en";


            if (string.IsNullOrEmpty(currentLanguage))
            {
                var culture = browserLanguage == "ar" ? "ar" : "en";

                var requestCulture = new RequestCulture(culture, culture);
                context.Features.Set<IRequestCultureFeature>(new RequestCultureFeature(requestCulture, null));

                CultureInfo.CurrentCulture = new CultureInfo(culture);
            }

            await next(context);
        }
    }
}
