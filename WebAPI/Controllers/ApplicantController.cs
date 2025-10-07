using Application.Models;
using Application.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Search;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QueryResultByCriteria<ApplicantSearchResultDto>))]
        public async Task<IActionResult> SearchAsync(
            [FromQuery][Required] ApplicantSearchCriteria searchCriteria)
        {
            var userId = HttpContext.User.GetUserId();

            var result = await _applicantService.SearchAsync(searchCriteria, userId);

            return Ok(_mapper.Map<QueryResultByCriteria<ApplicantSearchResultDto>>(result));
        }

        [HttpPost("{applicantId}/transfer-to-employee")]
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
