namespace FurmaIdle.Models
{
    public class GuildModel
    {
        public int PartyCapMax { get; set; } = 3;
        public HashSet<string> Roster { get; set; } = new();
    }
}
