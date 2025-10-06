using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class EmployeeDao
    {
        public int Id { get; set; }

        public DateTime HireDate { get; set; }

        // Relationship
        public int ApplicantInfoId { get; set; }
        public ApplicantInfoDao ApplicantInfo { get; set; }
    }
}
