using Application.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.Constants;
using WebAPI.DTOs;
using WebAPI.Extensions;

namespace WebAPI.Controllers
{
    [Authorize(Roles = UserRoleConstants.Hr)]
    public class CheckController : BaseApiController
    {
        private readonly ICheckService _checkService;
        private readonly IMapper _mapper;

        public CheckController(ICheckService checkService, IMapper mapper)
        {
            _checkService = checkService;
            _mapper = mapper;
        }

        [HttpPost("srart")]
        [SwaggerOperation(
            Summary = "Starts check by applicants and employees",
            Description = "This endpoint allows HRs start check by applicants and employees providing full name."
            )]
        [SwaggerResponse(StatusCodes.Status201Created, "The Check was successfuly created and started.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not authorize or have HR role to access this resource.")]
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

        [HttpGet("{checkId}/results")]
        [SwaggerOperation(
            Summary = "Returns results by check id",
            Description = "This endpoint allows HRs to get results prvoding check id. The result contains ids and is grouped by check event type (Applicant, Employee)"
            )]
        [SwaggerResponse(StatusCodes.Status201Created, "The Check result.", typeof(CheckResultDto))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not authorize or have HR role to access this resource.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The check not found")]
        public async Task<ActionResult> GetCheckResults(int checkId)
        {
            var result = await _checkService.GetCheckResultsAsync(checkId);

            return Ok(_mapper.Map<CheckResultDto>(result));
        }
    }
}
