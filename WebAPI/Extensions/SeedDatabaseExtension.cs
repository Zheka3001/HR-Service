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

            var adminLogin = "admin@test.com";

            if (context.Users.Any(u => u.Login == adminLogin)) return app;

            using var hmac = new HMACSHA512();

            var adminUser = new UserDao
            {
                Role = RoleDao.Admin,
                FirstName = "Admin",
                LastName = "Default",
                MiddleName = null,
                Login = adminLogin,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
                PasswordSalt = hmac.Key,
            };

            context.Users.Add(adminUser);
            context.SaveChanges();

            return app;
        }
    }
}
