using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Data.Seeds
{
    public interface IDatabaseSeeder
    {
        Task SeedAsync();
    }
}
