using Microsoft.AspNetCore.Localization;

namespace SolutionName.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/Settings")]
    public class SettingsController : ControllerBase
    {
        [HttpPost("set-language")]
        public IActionResult SetLanguage([FromQuery] string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    Domain = "localhost",
                    SameSite = SameSiteMode.None,
                    Secure = true

                }
            );
            return NoContent();

        }
    }
}
