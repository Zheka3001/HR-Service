using Application.Models;
using Application.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(IMapper mapper, IAuthService authService)
        {
            _mapper = mapper;
            _authService = authService;
        }

        [HttpPost("log-in")]
        [SwaggerOperation(
            Summary = "Login into HR service",
            Description = "This endpoint allows to login into HR service by providing valid Admin or HR login and password.\n\n" +
            "Available admin login: admin@test.com\n\n" +
            "Available HR logins: firstHr@test.com, secondHr@test.com\n\n" +
            "Password for all are the same: Pa$$w0rd"
            )]
        [SwaggerResponse(StatusCodes.Status200OK, "Succesfull login.", typeof(AuthTokens))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid login or password.")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var tokens = await _authService.AuthenticateAsync(_mapper.Map<LoginRequest>(request));

            return Ok(tokens);
        }

        [HttpPost("refresh-token")]
        [SwaggerOperation(
            Summary = "Refresh tokens",
            Description = "This endpoint allows refresh tokens by providing expired access token and valid refresh token."
            )]
        [SwaggerResponse(StatusCodes.Status200OK, "Tokens are refreshed.", typeof(AuthTokens))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid access or refresh tokens.")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequestDto request)
        {
            var tokens = await _authService.RefreshTokensAsync(request.ExpiredAccessToken, request.RefreshToken);

            return Ok(tokens);
        }
    }
}
