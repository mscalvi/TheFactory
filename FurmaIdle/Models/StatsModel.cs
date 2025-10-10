using FurmaIdle.Helpers;

namespace FurmaIdle.Models
{
    public class StatsModel
    {
        public int ExpeditionsStage1 { get; set; }
        public int ExpeditionsStage2 { get; set; }
        public int ExpeditionsExpansion { get; set; }
        public int ExpeditionsTotal { get; set; }
        
        public int ClicksExpansion { get; set; }
        public int ClicksTotal { get; set; }
        public ResetPersistenceEnum.ResetPersistence Persistence { get; set; } = ResetPersistenceEnum.ResetPersistence.Permanent;
    }
}
