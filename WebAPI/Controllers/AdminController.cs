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
            var userId = await _userService.RegisterUserAsync(_mapper.Map<RegisterUser>(userDto));

            return Ok(new
            {
                success = true,
                message = "Hr successfuly registered",
                id = userId
            });
        }

        [HttpPost("work-group/create")]
        public async Task<IActionResult> InsertWorkGroup(CreateWorkGroupDto request)
        {
            var workGroupId = await _workGroupService.AddAsync(_mapper.Map<CreateWorkGroup>(request));

            return Ok(new
            {
                success = true,
                message = "Work group successfuly created",
                id = workGroupId
            });
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
