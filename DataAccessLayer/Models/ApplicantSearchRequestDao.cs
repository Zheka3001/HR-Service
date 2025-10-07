using Model.Enums;

namespace DataAccessLayer.Models
{
    public class ApplicantSearchRequestDao
    {
        public ApplicantSearchRequestDao(
            string? name,
            IEnumerable<WorkScheduleDao> workScheduleList,
            bool? onlyMine,
            int creatorId,
            int workGroupId,
            PaginationDao pagination,
            bool sortByUpdateDate = false,
            SortOrder sortOrder = SortOrder.Ascending)
        {
            Name = name;
            WorkScheduleList = workScheduleList;
            OnlyMine = onlyMine;
            WorkGroupId = workGroupId;
            SortByUpdateDate = sortByUpdateDate;
            Pagination = pagination;
            CreatorId = creatorId;
            SortOrder = sortOrder;
        }

        public string? Name { get; }

        public IEnumerable<WorkScheduleDao> WorkScheduleList { get; }

        public bool? OnlyMine { get; }

        public int CreatorId { get; }

        public int WorkGroupId { get; }

        public bool SortByUpdateDate { get; }

        public SortOrder SortOrder { get; }

        public PaginationDao Pagination { get; }
    }
}
