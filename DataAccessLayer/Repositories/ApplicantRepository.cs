using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Interfaces;
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

        public async Task InsertAsync(Applicant applicant)
        {
            await _context.AddAsync(applicant);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
