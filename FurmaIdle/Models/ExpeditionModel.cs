namespace FurmaIdle.Models
{
    public class ExpeditionModel
    {
        public int Id { get; set; }
        public string StageId { get; set; }
        public string? PartyId { get; set; }
        public List<string> ContractsId { get; set; } = new();
        public Dictionary<string, int>? ContractsActiveId { get; set; }
        public Dictionary<int, string>? ContractsLockedId { get; set; }
        public TimeOnly TimeStart { get; set; }
        public TimeOnly? TimeFinish { get; set; }
    }
}
