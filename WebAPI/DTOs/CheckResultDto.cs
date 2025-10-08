namespace WebAPI.DTOs
{
    public class CheckResultDto
    {
        public string SearchName { get; set; }

        public int InitiatorId { get; set; }

        public DateTime InitiationDate { get; set; }

        public Dictionary<CheckEventTypeDto, List<int>> Results { get; set; }
    }
}
