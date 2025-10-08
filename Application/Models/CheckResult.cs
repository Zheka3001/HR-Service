using DataAccessLayer.Models;

namespace Application.Models
{
    public class CheckResult
    {
        public string SearchName { get; set; }

        public int InitiatorId { get; set; }

        public DateTime InitiationDate { get; set; }

        public Dictionary<CheckEventType, List<int>> Results { get; set; }
    }
}
