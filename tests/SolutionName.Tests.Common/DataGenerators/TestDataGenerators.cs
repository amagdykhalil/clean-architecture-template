using Bogus;
using Microsoft.AspNetCore.Identity;
using SolutionName.Domain.Entities;
using System.Data;

namespace SolutionName.Tests.Common.DataGenerators
{
    public static class TestDataGenerators
    {

        public static Faker<Domain.Entities.Person> PersonFaker() => new Faker<Domain.Entities.Person>()
            .RuleFor(p => p.Id, f => 0)
            .RuleFor(p => p.FirstName, f => f.Name.FirstName())
            .RuleFor(p => p.LastName, f => f.Name.LastName());

        public static Faker<User> UserFaker(Domain.Entities.Person? person = null) => new Faker<User>()
            .RuleFor(u => u.Id, f => 0)
            .RuleFor(u => u.UserName, f => $"user_{f.UniqueIndex}")
            .RuleFor(u => u.Email, (f, u) => $"{u.UserName}@example.com")
            .RuleFor(u => u.EmailConfirmed, f => true)
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(u => u.PhoneNumberConfirmed, f => true)
            .RuleFor(u => u.PasswordHash, f => "PasswordHash")
            .RuleFor(u => u.TwoFactorEnabled, f => false)
            .RuleFor(u => u.LockoutEnd, f => null as DateTimeOffset?)
            .RuleFor(u => u.LockoutEnabled, f => false)
            .RuleFor(u => u.AccessFailedCount, f => 0)
            .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName.ToUpper())
            .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email.ToUpper())
            .RuleFor(u => u.SecurityStamp, f => f.Random.Guid().ToString())
            .RuleFor(u => u.ConcurrencyStamp, f => f.Random.Guid().ToString());

        public static Faker<RefreshToken> RefreshTokenFaker(User? user = null) => new Faker<RefreshToken>()
            .RuleFor(r => r.Id, f => 0)
            .RuleFor(r => r.Token, f => f.Random.AlphaNumeric(32))
            .RuleFor(r => r.User, f => user ?? UserFaker().Generate())
            .RuleFor(r => r.CreatedOn, f => DateTime.UtcNow)
            .RuleFor(r => r.ExpiresOn, f => DateTime.UtcNow.AddDays(7))
            .RuleFor(r => r.RevokedOn, f => null as DateTime?);

        public static Faker<IdentityRole<int>> RoleFaker() => new Faker<IdentityRole<int>>()
        .RuleFor(r => r.Id, f => 0)
        .RuleFor(r => r.Name, f => $"Role_{f.UniqueIndex}")         // unique
        .RuleFor(r => r.NormalizedName, (f, r) => r.Name.ToUpperInvariant())
        .RuleFor(r => r.ConcurrencyStamp, f => f.Random.Guid().ToString());
    }
}