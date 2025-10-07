using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using System.Data;
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

            AddAdmin(context);
            AddWorkGroupHrsAndApplicants(context);

            context.SaveChanges();

            return app;
        }

        private static void AddAdmin(DataContext context)
        {
            var adminLogin = "admin@test.com";

            if (context.Users.Any(u => u.Login == adminLogin)) return;

            using var hmac = new HMACSHA512();

            var adminUser = new UserDao
            {
                Id = 1,
                Role = RoleDao.Admin,
                FirstName = "Admin",
                LastName = "Default",
                MiddleName = null,
                Login = adminLogin,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
                PasswordSalt = hmac.Key,
            };

            context.Users.Add(adminUser);
        }

        private static void AddWorkGroupHrsAndApplicants(DataContext context)
        {
            if (context.Users.Any(u => u.WorkGroupId != null)
                || context.WorkGroups.Any()
                || context.Applicants.Any()) return;

            using var hmac = new HMACSHA512();

            var workGroups = new List<WorkGroupDao>()
            {
                new WorkGroupDao
                {
                    Id = 1,
                    Name = "first"
                },
                new WorkGroupDao
                {
                    Id = 2,
                    Name = "second"
                }
            };

            context.WorkGroups.AddRange(workGroups);

            var hrs = new List<UserDao>()
            {
                new UserDao
                {
                    Id = 2,
                    FirstName = "first",
                    LastName = "last",
                    MiddleName = "middle",
                    Role = RoleDao.HR,
                    Login = "firstHr@test.com",
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
                    PasswordSalt = hmac.Key,
                    WorkGroupId = 1,
                },
                new UserDao
                {
                    Id = 3,
                    FirstName = "first1",
                    LastName = "last1",
                    MiddleName = "middle1",
                    Role = RoleDao.HR,
                    Login = "secondHr@test.com",
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
                    PasswordSalt = hmac.Key,
                    WorkGroupId = 2,
                }
            };

            context.Users.AddRange(hrs);

            var applicants = new List<ApplicantDao>()
            {
                new ApplicantDao
                {
                    Id = 1,
                    WorkSchedule = WorkScheduleDao.Office,
                    CreatedById = 2,
                    LastUpdatedById = 2,
                    WorkGroupId = 1,
                    ApplicantInfo = new ApplicantInfoDao()
                    {
                        Id = 1,
                        FirstName = "first",
                        LastName = "first",
                        MiddleName = "first",
                        Email = "first@test.com",
                        PhoneNumber = "1234567890",
                        Country = "Belarus",
                        DateOfBirth = DateTime.UtcNow.AddYears(-20),
                        SocialNetworks = new List<SocialNetworkDao>()
                        {
                            new SocialNetworkDao()
                            {
                                Id = 1,
                                UserName = "firstGitHub",
                                Type = SocialNetworkTypeDao.GitHub
                            },
                            new SocialNetworkDao()
                            {
                                Id = 2,
                                UserName = "firstLinkedIn",
                                Type = SocialNetworkTypeDao.LinkedIn
                            }
                        }
                    }
                },
                new ApplicantDao
                {
                    Id = 2,
                    WorkSchedule = WorkScheduleDao.Remote,
                    CreatedById = 2,
                    LastUpdatedById = 2,
                    WorkGroupId = 1,
                    ApplicantInfo = new ApplicantInfoDao()
                    {
                        Id = 2,
                        FirstName = "second",
                        LastName = "second",
                        MiddleName = "second",
                        Email = "second@test.com",
                        PhoneNumber = "1234567890",
                        Country = "Poland",
                        DateOfBirth = DateTime.UtcNow.AddYears(-25),
                        SocialNetworks = new List<SocialNetworkDao>()
                        {
                            new SocialNetworkDao()
                            {
                                Id = 3,
                                UserName = "secondGitHub",
                                Type = SocialNetworkTypeDao.GitHub
                            },
                            new SocialNetworkDao()
                            {
                                Id = 4,
                                UserName = "secondTwitter",
                                Type = SocialNetworkTypeDao.Twitter
                            }
                        }
                    }
                },
                new ApplicantDao
                {
                    Id = 3,
                    WorkSchedule = WorkScheduleDao.Remote,
                    CreatedById = 3,
                    LastUpdatedById = 3,
                    WorkGroupId = 2,
                    ApplicantInfo = new ApplicantInfoDao()
                    {
                        Id = 3,
                        FirstName = "third",
                        LastName = "third",
                        MiddleName = "third",
                        Email = "third@test.com",
                        PhoneNumber = "1234567890",
                        Country = "Litva",
                        DateOfBirth = DateTime.UtcNow.AddYears(-23),
                        SocialNetworks = new List<SocialNetworkDao>()
                        {
                            new SocialNetworkDao()
                            {
                                Id = 5,
                                UserName = "thirdGitHub",
                                Type = SocialNetworkTypeDao.GitHub
                            },
                            new SocialNetworkDao()
                            {
                                Id = 6,
                                UserName = "thirdFacebook",
                                Type = SocialNetworkTypeDao.Facebook
                            }
                        }
                    }
                },
                new ApplicantDao
                {
                    Id = 4,
                    WorkSchedule = WorkScheduleDao.Hybrid,
                    CreatedById = 3,
                    LastUpdatedById = 3,
                    WorkGroupId = 2,
                    ApplicantInfo = new ApplicantInfoDao()
                    {
                        Id = 4,
                        FirstName = "forth",
                        LastName = "forth",
                        MiddleName = "forth",
                        Email = "forth@test.com",
                        PhoneNumber = "1234567890",
                        Country = "Poland",
                        DateOfBirth = DateTime.UtcNow.AddYears(-21),
                        SocialNetworks = new List<SocialNetworkDao>()
                        {
                            new SocialNetworkDao()
                            {
                                Id = 7,
                                UserName = "forthGitHub",
                                Type = SocialNetworkTypeDao.GitHub
                            },
                            new SocialNetworkDao()
                            {
                                Id = 8,
                                UserName = "forthLinkedIn",
                                Type = SocialNetworkTypeDao.LinkedIn
                            }
                        }
                    }
                }
            };

            context.Applicants.AddRange(applicants);
        }
    }
}
