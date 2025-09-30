using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using WebAPI.DTOs;

namespace HRService.Controllers
{
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpPost("register")]
        public async Task<ActionResult> RegisterHRAsync(RegisterUserDto userDto)
        {
            await _userService.RegisterUserAsync(new RegisterUser()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                MiddleName = userDto.MiddleName,
                Login = userDto.Login,
                Password = userDto.Password
            });

            return Ok();
        }
    }
}
