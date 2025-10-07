namespace Application.Extensions
{
    public static class CollectionExtensions
    {
        public static string AsSingleValue(this IEnumerable<string> collection)
        {
            if (collection == null || !Enumerable.Any(collection))
            {
                return null;
            }

            return string.Join(string.Empty, collection).Trim().DefaultIfNullOrEmpty(null);
        }
    }
}
