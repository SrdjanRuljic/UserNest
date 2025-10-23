using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContextInitializer(
        ApplicationDbContext context,
        IDateTimeService dateTimeService,
        ILogger<ApplicationDbContextInitializer> logger,
        RoleManager<AppRole> roleManager,
        UserManager<AppUser> userManager)
    {
        public async Task InitializeAsync()
        {
            try
            {
                if (context.Database.IsSqlServer())
                {
                    await context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initialising the database.");

                throw;
            }
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await TrySeedAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");

                throw;
            }
        }

        public async Task TrySeedAsync(CancellationToken cancellationToken)
        {
            if (!await roleManager.Roles.AnyAsync(cancellationToken))
                await SeedRolesAsync();
            if (!await userManager.Users.AnyAsync(cancellationToken))
                await SeedUsersAsync();
            if (!await context.Languages.AnyAsync(cancellationToken))
                await SeedLanguagesAsync(cancellationToken);
            else
                return;
        }

        #region [Seed Methods]

        private async Task SeedRolesAsync()
        {
            await roleManager.CreateAsync(new AppRole()
            {
                Name = Domain.Enums.Roles.Admin.ToString()
            });
            await roleManager.CreateAsync(new AppRole()
            {
                Name = Domain.Enums.Roles.RegularUser.ToString()
            });
        }

        private async Task SeedUsersAsync()
        {
            AppUser admin = new()
            {
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                UserName = "admin",
                FirstName = "Admin",
                LastName = "Admin",
                Created = dateTimeService.Now,
                CreatedBy = "System"
            };

            AppUser? adminUser = await userManager.FindByNameAsync(admin.UserName);

            if (adminUser == null)
            {
                await userManager.CreateAsync(admin, "Administrator_123!");
                await userManager.AddToRolesAsync(admin, [Domain.Enums.Roles.Admin.ToString()]);
            }
        }

        private async Task SeedLanguagesAsync(CancellationToken cancellationToken)
        {
            var languages = new List<Language>
            {
                new() {
                    Culture = "en",
                    Name = Domain.Enums.Languages.English.ToString(),
                    Description = "English language",
                    FlagPath = "/flags/en.png",
                    IsDeleted = false
                },
                new() {
                    Culture = "sr-Cyrl-RS",
                    Name = Domain.Enums.Languages.Cyrillic.ToString(),
                    Description = "Serbian language in Cyrillic script",
                    FlagPath = "/flags/sr-cyrl.png",
                    IsDeleted = false
                },
                new() {
                    Culture = "sr-Latn-RS",
                    Name = Domain.Enums.Languages.Latin.ToString(),
                    Description = "Serbian language in Latin script",
                    FlagPath = "/flags/sr-latn.png",
                    IsDeleted = false
                },
                new() {
                    Culture = "sl",
                    Name = Domain.Enums.Languages.Slovenian.ToString(),
                    Description = "Slovenian language",
                    FlagPath = "/flags/sl.png",
                    IsDeleted = false
                }
            };

            await context.Languages.AddRangeAsync(languages, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        #endregion [Seed Methods]
    }
}