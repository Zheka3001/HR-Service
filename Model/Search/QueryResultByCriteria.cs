namespace Model.Search
{
    public class QueryResultByCriteria<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }

        public int RowsPerPage { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public int PageNumber { get; set; }
    }
}
