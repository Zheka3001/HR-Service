using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Enums;

namespace DataAccessLayer.Repositories
{
    public class ApplicantRepository : IApplicantRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ApplicantRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task DeleteAsync(int applicantId)
        {
            await _context.Applicants.Where(a => a.Id == applicantId).ExecuteDeleteAsync();
        }

        public async Task<ApplicantDao?> GetByIdAsync(int id)
        {
            return await _context.Applicants
                .Include(a => a.ApplicantInfo)
                .ThenInclude(ai => ai.SocialNetworks)
                .FirstOrDefaultAsync(a =>  a.Id == id);
        }

        public async Task InsertAsync(ApplicantDao applicant)
        {
            await _context.AddAsync(applicant);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<ApplicantSearchResultDao> SearchAsync(ApplicantSearchRequestDao searchRequest)
        {
            var query = _context.Applicants
                .Include(a => a.ApplicantInfo)
                .ThenInclude(ai => ai.SocialNetworks)
                .AsQueryable();

            query = query.Where(a => a.WorkGroupId == searchRequest.WorkGroupId);

            if (!string.IsNullOrWhiteSpace(searchRequest.Name))
            {
                var lowerName = searchRequest.Name.ToLower();
                query = query.Where(a =>
                    (a.ApplicantInfo.FirstName + " " + a.ApplicantInfo.LastName + " " + (a.ApplicantInfo.MiddleName ?? "")).TrimEnd().ToLower().Contains(lowerName) ||
                    a.ApplicantInfo.Email.ToLower().Contains(lowerName) ||
                    a.ApplicantInfo.SocialNetworks.Any(sn => sn.UserName.ToLower().Contains(lowerName))
                );
            }

            if (searchRequest.WorkScheduleList != null && searchRequest.WorkScheduleList.Any())
            {
                query = query.Where(a => searchRequest.WorkScheduleList.Contains(a.WorkSchedule));
            }

            if (searchRequest.OnlyMine.HasValue && searchRequest.OnlyMine.Value)
            {
                query = query.Where(a => a.CreatedById == searchRequest.CreatorId);
            }

            if (searchRequest.SortByUpdateDate)
            {
                query = searchRequest.SortOrder switch
                {
                    SortOrder.Ascending => query.OrderBy(a => a.LastUpdateDate),
                    SortOrder.Descending => query.OrderByDescending(a => a.LastUpdateDate),
                    _ => query
                };
            }
            else
            {
                query = searchRequest.SortOrder switch
                {
                    SortOrder.Ascending => query.OrderBy(a => a.Id),
                    SortOrder.Descending => query.OrderByDescending(a => a.Id),
                    _ => query
                };
            }

            var totalItems = await query.CountAsync();

            var pagedQuery = query
                .Skip((searchRequest.Pagination.PageNumber - 1) * searchRequest.Pagination.PageSize)
                .Take(searchRequest.Pagination.PageSize);

            var items = await pagedQuery
                .ProjectTo<ApplicantDao>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new ApplicantSearchResultDao
            {
                SearchedItems = items,
                ItemsRange = new SearchedItemsRangeDao
                {
                    TotalSearchedItems = totalItems,
                    PageNumber = searchRequest.Pagination.PageNumber,
                    PageSize = searchRequest.Pagination.PageSize,
                    TotalPages = totalItems / searchRequest.Pagination.PageSize + 1,
                }
            };
        }
    }
}
