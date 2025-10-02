using Application.Models;
using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthController(IUserService userService, ITokenService tokenService, IMapper mapper)
        {
            _userService = userService;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var tokens = await _userService.AuthenticateAsync(_mapper.Map<LoginRequest>(request));

            return Ok(tokens);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequestDto request)
        {
            var tokens = await _tokenService.RefreshTokensAsync(request.ExpiredAccessToken, request.RefreshToken);

            return Ok(tokens);
        }
    }
}
