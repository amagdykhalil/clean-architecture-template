using System.Text.Json.Serialization;

namespace SolutionName.Application.Features.Auth
{
    public class AuthDTO
    {
        public int UserId { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpiresOn { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
        [JsonIgnore]
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
