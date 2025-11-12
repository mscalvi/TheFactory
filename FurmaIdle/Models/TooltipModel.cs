namespace FurmaIdle.Models
{
    public class TooltipModel
    {
        public string Type { get; set; }
        public string Name { get; set; }

        public string Lore { get; set; }

        public Dictionary<string, string> Info { get; set; } = new();
    }
}
