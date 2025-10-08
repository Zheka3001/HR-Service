using Application.Models;
using Application.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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
        [SwaggerOperation(
            Summary = "Create new HR",
            Description = "This endpoint allows admin to create new HR by providing the HR's details."
            )]
        [SwaggerResponse(StatusCodes.Status201Created, "The HR was successfuly created.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not authorize or have admin role to access this resource.")]
        public async Task<ActionResult> RegisterHRAsync(RegisterUserDto userDto)
        {
            var userId = await _userService.RegisterUserAsync(_mapper.Map<RegisterUser>(userDto));

            return CreatedAtAction(
            nameof(RegisterHRAsync),    
            new
            {
                success = true,
                message = "Hr successfuly registered",
                id = userId
            });
        }

        [HttpPost("work-group/create")]
        [SwaggerOperation(
            Summary = "Create new Work Group",
            Description = "This endpoint allows admin to create new Work Group by providing the work group name details."
            )]
        [SwaggerResponse(StatusCodes.Status201Created, "The Work Group was successfuly created.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not authorize or have admin role to access this resource.")]
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
        [SwaggerOperation(
            Summary = "Move HRs to another work group",
            Description = "This endpoint allows admin to move HRs to another work group by providing the id of destination Work Group and list of HR's id.\n\n" +
            "Existed on start work group ids: 1, 2"
            )]
        [SwaggerResponse(StatusCodes.Status200OK, "The HR's was successfuly moved to another Work Group.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not authorize or have admin role to access this resource.")]
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
