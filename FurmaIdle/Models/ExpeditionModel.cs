namespace FurmaIdle.Models
{
    public class ExpeditionModel
    {
        public string Id { get; set; }
        public List<string>? PartyIds { get; set; } = new List<string>();
        public TimeOnly TimeStart { get; set; }
        public TimeOnly? TimeFinish { get; set; }
    }
}
