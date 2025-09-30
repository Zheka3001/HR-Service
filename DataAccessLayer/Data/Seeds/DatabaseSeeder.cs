using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataAccessLayer.Data.Seeds
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly DataContext _context;

        public DatabaseSeeder(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (!await _context.Users.AnyAsync(u => u.Role == Role.Admin))
            {
                var basePath = Path.GetDirectoryName(typeof(DatabaseSeeder).Assembly.Location);
                var seedFilePath = Path.Combine(basePath ?? string.Empty, "Data", "Seeds", "AdminSeed.json");

                if (File.Exists(seedFilePath))
                {
                    var adminData = await File.ReadAllTextAsync("Data/Seeds/AdminSeed.json");
                    var adminUsers = JsonSerializer.Deserialize<List<SeedUser>>(adminData);

                    if (adminUsers != null && adminUsers.Any())
                    {
                        var users = new List<User>();

                        foreach (var admin in adminUsers)
                        {
                            using var hmac = new HMACSHA512();

                            await _context.Users.AddAsync(new User
                            {
                                FirstName = admin.FirstName,
                                LastName = admin.LastName,
                                MiddleName = admin.MiddleName,
                                Login = admin.Login,
                                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(admin.Password)),
                                PasswordSalt = hmac.Key
                            });
                        }

                        await _context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
