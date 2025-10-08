namespace DataAccessLayer.Models
{
    public class CheckEventDao
    {
        public int Id { get; set; }

        public CheckEventTypeDao Type { get; set; }

        public int EntityId { get; set; }

        public int CheckId { get; set; }

        // Navigation properties

        public CheckDao Check { get; set; }
    }
}
