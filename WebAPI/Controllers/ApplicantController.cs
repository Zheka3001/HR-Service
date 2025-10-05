using Application.Models;
using Application.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public async Task<IActionResult> CreateApplicantAsync(CreateApplicantRequestDto request)
        {
            var userId = HttpContext.User.GetUserId();

            var applicant = await _applicantService.CreateApplicantAsync(_mapper.Map<CreateApplicantRequest>(request), userId);

            return Ok(_mapper.Map<CreateApplicantResponseDto>(applicant));
        }
    }
}
