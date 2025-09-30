using DataAccessLayer.Data;
using DataAccessLayer.Models;
using System.Security.Cryptography;
using System.Text;

namespace WebAPI.Extensions
{
    public static class SeedDatabaseExtension
    {
        public static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();

            if (context.Users.Any(u => u.Login == "admin")) return app;

            using var hmac = new HMACSHA512();

            var adminUser = new User
            {
                Role = Role.Admin,
                FirstName = "Admin",
                LastName = "Default",
                MiddleName = null,
                Login = "admin",
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
                PasswordSalt = hmac.Key,
            };

            context.Users.Add(adminUser);
            context.SaveChanges();

            return app;
        }
    }
}
