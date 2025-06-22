using Bogus;
using SolutionName.Domain.Entities;

namespace SolutionName.Tests.Common.DataGenerators
{
    public static class TestDataGenerators
    {

        public static Faker<Persistence.Entities.Person> PersonFaker() => new Faker<Persistence.Entities.Person>()
            .RuleFor(p => p.Id, f => 0)
            .RuleFor(p => p.FirstName, f => f.Name.FirstName())
            .RuleFor(p => p.LastName, f => f.Name.LastName());

        public static Faker<User> UserFaker(Persistence.Entities.Person? person = null) => new Faker<User>()
            .RuleFor(u => u.Id, f => 0)
            .RuleFor(u => u.UserName, f => f.Internet.UserName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.EmailConfirmed, f => true)
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(u => u.PhoneNumberConfirmed, f => true)
            .RuleFor(u => u.TwoFactorEnabled, f => false)
            .RuleFor(u => u.LockoutEnd, f => null as DateTimeOffset?)
            .RuleFor(u => u.LockoutEnabled, f => false)
            .RuleFor(u => u.AccessFailedCount, f => 0)
            .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName.ToUpper())
            .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email.ToUpper())
            .RuleFor(u => u.SecurityStamp, f => f.Random.Guid().ToString())
            .RuleFor(u => u.ConcurrencyStamp, f => f.Random.Guid().ToString())
            .RuleFor(u => u.Person, f => person ?? PersonFaker().Generate());

        public static Faker<RefreshToken> RefreshTokenFaker(User? user = null) => new Faker<RefreshToken>()
            .RuleFor(r => r.Id, f => 0)
            .RuleFor(r => r.Token, f => f.Random.AlphaNumeric(32))
            .RuleFor(r => r.User, f => user ?? UserFaker().Generate())
            .RuleFor(r => r.CreatedOn, f => DateTime.UtcNow)
            .RuleFor(r => r.ExpiresOn, f => DateTime.UtcNow.AddDays(7))
            .RuleFor(r => r.RevokedOn, f => null as DateTime?);
    }
}