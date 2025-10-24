using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<AppUser,
                                                                                                          AppRole,
                                                                                                          string,
                                                                                                          IdentityUserClaim<string>,
                                                                                                          AppUserRole,
                                                                                                          IdentityUserLogin<string>,
                                                                                                          IdentityRoleClaim<string>,
                                                                                                          IdentityUserToken<string>>(options), IApplicationDbContext
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Language> Languages { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
            => base.SaveChangesAsync(cancellationToken);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            
            modelBuilder.Entity<AppUser>().HasQueryFilter(x => !x.IsDeleted);
        }
    }
}