using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheBlogProject.Data;
using TheBlogProject.Enums;
using TheBlogProject.Models;


namespace TheBlogProject.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;

        public DataService(ApplicationDbContext dbContext, 
                           RoleManager<IdentityRole> roleManager, 
                           UserManager<BlogUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }


        public async Task ManageDataAsync()
        {
            // 0th: Create the DB from the Migrations
            await _dbContext.Database.MigrateAsync();

            // 1st: seed a few roles into the system: reach out to DB and create roles
            await SeedRolesAsync();

            // 2nd: Seed a few users into the system: reach out to DB and create users
            await SeedUsersAsync();

        }

        private async Task SeedRolesAsync()
        {
            // If already roles in system, do nothing. 
            if (_dbContext.Roles.Any()) return;
            
            // Otherwise, make some roles
            foreach(var role in Enum.GetNames(typeof(BlogRole)))
            {
                // Need to use Role Manager to create roles
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

        }

        private async Task SeedUsersAsync()
        {
            // If already users in system, do nothing.
            if (_dbContext.Users.Any()) return;

            // Otherwise, create admin user as instance of BlogUser
            var adminUser = new BlogUser()
            {
                Email = "ephillips.tx@gmail.com",
                UserName = "ephillips.tx@gmail.com",
                FirstName = "Eric",
                LastName = "Phillips",
                PhoneNumber = "(817) 000-0000",
                EmailConfirmed = true,

            };

            // Use the UserManager to create a new user that is defined by the adminUser variable
            await _userManager.CreateAsync(adminUser, "Password1!");

            // Add new user to administrator role
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());

            // Create moderator user  |  Next level would be to store this user data in the appsettings.json file
            // & use injected instance of configuration to pull the data out of the config file. 
            var modUser = new BlogUser()
            {
                Email = "This@email.com",
                UserName = "This@email.com",
                FirstName = "Bob",
                LastName = "Builder",
                PhoneNumber = "(000) 000-0000",
                EmailConfirmed = true
            };

            // Use the UserManager to create a new user that is defined by the modUser variable
            await _userManager.CreateAsync(modUser, "Password1!");

            // Add new user to moderator role
            await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());
        }

    }
}
