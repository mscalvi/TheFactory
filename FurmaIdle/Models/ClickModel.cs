namespace FurmaIdle.Models
{
    public class ClickModel
    {
        public string Id { get; set; }
        public string StageId { get; set; }
        public double BaseGain { get; set; }

        // Modifiers
        public List<ModifierModel> Modifiers { get; set; }
    }
}
