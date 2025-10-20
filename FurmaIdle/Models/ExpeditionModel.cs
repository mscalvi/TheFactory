namespace FurmaIdle.Models
{
    public class ExpeditionModel
    {
        public string Id { get; set; }
        public List<string>? PartyIds { get; set; }
        public Dictionary<string, int>? ContractsActiveId { get; set; }
        public Dictionary<int, List<string>>? ContractsLockedId { get; set; }
        public TimeOnly TimeStart { get; set; }
        public TimeOnly? TimeFinish { get; set; }
    }
}
