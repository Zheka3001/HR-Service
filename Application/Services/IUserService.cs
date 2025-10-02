using Application.Models;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IUserService
    {
        Task<AuthTokens> AuthenticateAsync(LoginRequest request);
        Task RegisterUserAsync(RegisterUser user);
    }
}
