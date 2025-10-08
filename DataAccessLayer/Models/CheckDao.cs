namespace DataAccessLayer.Models
{
    public class CheckDao
    {
        public int Id { get; set; }

        public DateTime InitiationDate { get; set; }

        public int InitiatorId { get; set; }

        public string SearchName { get; set; }

        // Navigation properties
        public UserDao Initiator { get; set; }

        public ICollection<CheckEventDao> CheckEvents { get; set; }
    }
}
