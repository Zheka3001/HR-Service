using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Constants;
using WebAPI.Extensions;

namespace WebAPI.Controllers
{
    [Authorize(Roles = UserRoleConstants.Hr)]
    public class CheckController : BaseApiController
    {
        private readonly ICheckService _checkService;

        public CheckController(ICheckService checkService)
        {
            _checkService = checkService;
        }

        [HttpPost]
        public async Task<IActionResult> RunCheck([FromBody] string fullName)
        {
            var checkId = await _checkService.StartCheckAsync(fullName, HttpContext.User.GetUserId());

            return Ok(new
            {
                success = true,
                message = "Ckeck successfuly runned",
                id = checkId
            });
        }
    }
}
