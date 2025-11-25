namespace FurmaIdle.Models
{
    public class TooltipModel
    {
        public string Name { get; set; }

        public string? CostAmount { get; set; }
        public string? CostIcon { get; set; }
        public string? CostName { get; set; }

        public string Description { get; set; }

        public string Lore { get; set; }

        public Dictionary<string, string> Info { get; set; } = new();
    }
}
