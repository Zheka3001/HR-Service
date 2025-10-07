using Model.Enums;

namespace Application.Models
{
    public class ApplicantSearchCriteria
    {
        public const int DefaultPageSize = 10;
        public const int DefaultPageNumber = 1;

        public int PageSize { get; init; } = DefaultPageSize;

        public int PageNumber { get; init; } = DefaultPageNumber;

        public string? Name { get; init; }

        public IEnumerable<WorkSchedule>? WorkSchedules { get; init; }

        public bool OnlyMine { get; init; } = false;

        public bool SortByLastUpdateDate { get; init; }

        public SortOrder SortOder { get; init; }
    }
}
