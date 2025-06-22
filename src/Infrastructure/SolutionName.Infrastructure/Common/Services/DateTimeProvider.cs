using SolutionName.Application.Abstractions.Services;

namespace SolutionName.Infrastructure.Common.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
