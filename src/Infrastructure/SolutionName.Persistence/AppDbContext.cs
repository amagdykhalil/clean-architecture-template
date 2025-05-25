using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SolutionName.Domain.Entities;
using SolutionName.Persistence.Extensions;
using SolutionName.Persistence.Identity;

namespace SolutionName.Persistence
{
    public class AppDbContext
        : IdentityDbContext<User, IdentityRole<int>, int, IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DbSet<Person> People { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        private readonly IMediator mediator;
        public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator) : base(options)
        {
            this.mediator = mediator;
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            int result = await base.SaveChangesAsync(cancellationToken);

            await mediator.DispatchDomainEventsAsync(this);

            return result;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}


