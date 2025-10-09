using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public sealed class ContractRun
    {
        public string ContractId { get; init; }
        public double ProgressSec { get; set; } = 0;
    }

    public class ExpeditionModel
    {
        public string StageId { get; set; }
        public string ExpansionId { get; set; }
        public List<string> PartyId { get; set; } = new();
        public ExpeditionEnum.ExpeditionStatus ExpeditionStatus { get; set; }

        public Dictionary<string, ContractModel> Contracts { get; set; } = new();
        public List<ContractRun> ActiveContracts { get; set; } = new();
        public Dictionary<int, string> LockedContractByLevel { get; set; } = new();

        public DateTimeOffset? Start { get; set; }
    }
}
