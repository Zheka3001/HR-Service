using Application.Models;
using Application.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Constants;
using WebAPI.Controllers;
using WebAPI.DTOs;

namespace HRService.Controllers
{
    [Authorize(Roles = UserRoleConstants.Admin)]
    public class AdminController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IWorkGroupService _workGroupService;
        private readonly IMapper _mapper;

        public AdminController(IUserService userService, IMapper mapper, IWorkGroupService workGroupService)
        {
            _userService = userService;
            _mapper = mapper;
            _workGroupService = workGroupService;
        }

        [HttpPost("register-hr")]
        public async Task<ActionResult> RegisterHRAsync(RegisterUserDto userDto)
        {
            await _userService.RegisterUserAsync(_mapper.Map<RegisterUser>(userDto));

            return Ok();
        }

        [HttpPost("work-group")]
        public async Task<IActionResult> InsertWorkGroup(CreateWorkGroupDto request)
        {
            await _workGroupService.InsertAsync(_mapper.Map<CreateWorkGroup>(request));

            return Ok();
        }

        [HttpPut("work-group/{workGroupId}/hrs")]
        public async Task<IActionResult> MoveHrsToWorkGroupAsync(int workGroupId, [FromBody] MoveHrsRequestDto request)
        {
            await _workGroupService.MoveHrsAsync(new MoveHrsRequest
                {
                    WorkGroupId = workGroupId,
                    UserIds = request.UserIds
                });

            return Ok(new
            {
                success = true,
                message = "HRs and their applicants have been successfuly moved."
            });
        }
    }
}
