namespace FurmaIdle.Models
{
    public class StageModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image {  get; set; }
        public string ClickImage { get; set; }
        public string ResourceId { get; set; }
        public bool Unlocked { get; set; } = true;
        public bool Avaliable { get; set; }
        public string DestinationId { get; set; }
        public int Sort { get; set; } = 0;
    }
}
