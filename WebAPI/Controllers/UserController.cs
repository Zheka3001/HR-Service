using Application.Models;
using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Constants;
using WebAPI.Controllers;
using WebAPI.DTOs;

namespace HRService.Controllers
{
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [Authorize(Roles = UserRoleConstants.Admin)]
        [HttpPost("register")]
        public async Task<ActionResult> RegisterHRAsync(RegisterUserDto userDto)
        {
            await _userService.RegisterUserAsync(_mapper.Map<RegisterUser>(userDto));

            return Ok();
        }
    }
}
