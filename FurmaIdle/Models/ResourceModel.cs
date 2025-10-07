namespace FurmaIdle.Models
{
    public class ResourceModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Total { get; set; }
        public double Actual { get; set; }
        public double PerSecond { get; set; }
        public bool Unlocked { get; set; } = false;
        public string? Icon { get; set; }
        public int Sort { get; set; }
    }
}
