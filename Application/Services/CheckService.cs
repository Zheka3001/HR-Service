using Application.Exceptions;
using Application.Services.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services
{
    public class CheckService : ICheckService
    {
        private readonly ICheckRepository _checkRepository;
        private readonly IUserRepository _userRepository;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CheckService(ICheckRepository checkRepository, IUserRepository userRepository, IServiceScopeFactory serviceScopeFactory)
        {
            _checkRepository = checkRepository;
            _userRepository = userRepository;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<int> StartCheckAsync(string fullName, int initiatorId)
        {
            if(!await _userRepository.UserExistsAsync(initiatorId))
            {
                throw new UnauthorizedException("User is not authorized or does not exist.");
            }

            var check = new CheckDao
            {
                InitiationDate = DateTime.UtcNow,
                InitiatorId = initiatorId,
                SearchName = fullName,
                CheckEvents = new List<CheckEventDao>()
            };

            await _checkRepository.InsertAsync(check);
            await _checkRepository.SaveChangesAsync();

            _ = Task.Run(async () => await RunApplicantSearchAsync(check.Id, fullName));
            _ = Task.Run(async () => await RunEmployeeSearchAsync(check.Id, fullName));

            return check.Id;
        }

        private async Task RunApplicantSearchAsync(int checkId, string fullName)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var scopedApplicantRepository = scope.ServiceProvider.GetRequiredService<IApplicantRepository>();
            var scopedCheckRepository = scope.ServiceProvider.GetRequiredService<ICheckRepository>();

            await Task.Delay(3000);

            var result = await scopedApplicantRepository.SearchApplicantsByFullNameAsync(fullName);

            if (result == null) return;

            var checkEvents = result.Select(id => new CheckEventDao()
            {
                CheckId = checkId,
                EntityId = id,
                Type = CheckEventTypeDao.Applicant
            });

            await scopedCheckRepository.AddCheckEventsAsync(checkEvents);
            await scopedCheckRepository.SaveChangesAsync();
        }

        private async Task RunEmployeeSearchAsync(int checkId, string fullName)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var scopedEmployeeRepository = scope.ServiceProvider.GetRequiredService<IEmployeeRepository>();
            var scopedCheckRepository = scope.ServiceProvider.GetRequiredService<ICheckRepository>();

            await Task.Delay(5000);

            var result = await scopedEmployeeRepository.SearchEmployeesByFullNameAsync(fullName);

            if (result == null) return;

            var checkEvents = result.Select(id => new CheckEventDao()
            {
                CheckId = checkId,
                EntityId = id,
                Type = CheckEventTypeDao.Employee
            });

            await scopedCheckRepository.AddCheckEventsAsync(checkEvents);
            await scopedCheckRepository.SaveChangesAsync();
        }
    }
}
