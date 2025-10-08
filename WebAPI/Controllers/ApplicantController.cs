using Application.Models;
using Application.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Search;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using WebAPI.Constants;
using WebAPI.DTOs;
using WebAPI.Extensions;

namespace WebAPI.Controllers
{
    [Authorize(Roles = UserRoleConstants.Hr)]
    public class ApplicantController : BaseApiController
    {
        private readonly IApplicantService _applicantService;
        private readonly IMapper _mapper;

        public ApplicantController(IApplicantService applicantService, IMapper mapper)
        {
            _applicantService = applicantService;
            _mapper = mapper;
        }

        [HttpPost("create")]
        [SwaggerOperation(
            Summary = "Create new Applicant",
            Description = "This endpoint allows hrs to create new Applicant by providing the applicant details."
            )]
        [SwaggerResponse(StatusCodes.Status201Created, "The Applicant was successfuly created.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not authorize or have HR role to access this resource.")]
        public async Task<IActionResult> CreateApplicantAsync(CreateApplicantRequestDto request)
        {
            var userId = HttpContext.User.GetUserId();

            var applicantId = await _applicantService.CreateApplicantAsync(_mapper.Map<CreateApplicantRequest>(request), userId);

            return Ok(new
            {
                success = true,
                message = "Applicant successfuly created",
                id = applicantId
            });
        }

        [HttpPut("update")]
        [SwaggerOperation(
            Summary = "Update Applicant info",
            Description = "This endpoint allows hrs to update existing Applicant info by providing the new applicant details.\n\n" +
            "Existed on start Applicant ids:\n\n" +
            "  - Work group 1 (1, 2)\n\n" +
            "  - Work group 2 (3, 4)"
            )]
        [SwaggerResponse(StatusCodes.Status200OK, "The Applicant was successfuly updated.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not authorize or have HR role to access this resource.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbid action.")]
        public async Task<IActionResult> UpdateApplicantAsync(UpdateApplicantRequestDto request)
        {
            var userId = HttpContext.User.GetUserId();

            await _applicantService.UpdateApplicantAsync(_mapper.Map<UpdateApplicantRequest>(request), userId);

            return Ok(new
            {
                success = true,
                message = "Applicant successfuly updated",
            });
        }

        [HttpGet("search")]
        [SwaggerOperation(
            Summary = "Search for applicants",
            Description = "This endpoint allows HRs to search for applicants by providing filters, pagination and sorting."
            )]
        [SwaggerResponse(StatusCodes.Status200OK, "The Applicants that match the filters.", typeof(QueryResultByCriteria<ApplicantSearchResultDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not authorize or have HR role to access this resource.")]
        public async Task<IActionResult> SearchAsync(
            [FromQuery][Required] ApplicantSearchCriteria searchCriteria)
        {
            var userId = HttpContext.User.GetUserId();

            var result = await _applicantService.SearchAsync(searchCriteria, userId);

            return Ok(_mapper.Map<QueryResultByCriteria<ApplicantSearchResultDto>>(result));
        }


        [HttpPost("{applicantId}/transfer-to-employee")]
        [SwaggerOperation(
            Summary = "Transfer applicant to employee",
            Description = "This endpoint allows HRs to transfer applicant to employee by creating employee, bind applicant info and delete applicant.\n\n" +
            "Existed on start Applicant ids:\n\n" +
            "  - Work group 1 (1, 2)\n\n" +
            "  - Work group 2 (3, 4)"
            )]
        [SwaggerResponse(StatusCodes.Status200OK, "The Applicant was successfuly trasfered to Employee.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not authorize or have HR role to access this resource.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbid action")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Applicant not found")]
        public async Task<IActionResult> TransferToEmployeeAsync(int applicantId)
        {
            var userId = HttpContext.User.GetUserId();

            var employeeId = await _applicantService.TransferToEmployeeAsync(userId, applicantId);

            return Ok(new
            {
                employeeId,
                success = true,
                message = "Applicant successfully transfered to employee"
            });
        }
    }
}
