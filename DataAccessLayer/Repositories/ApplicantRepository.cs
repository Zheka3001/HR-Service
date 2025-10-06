using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ApplicantRepository : IApplicantRepository
    {
        private readonly DataContext _context;

        public ApplicantRepository(DataContext context)
        {
            _context = context;
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
    }
}
