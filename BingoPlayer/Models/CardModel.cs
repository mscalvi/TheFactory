namespace BingoPlayer.Models
{
    public class CardModel
    {
        public int CardId { get; set; }
        public int CardNumber { get; set; }
        public List<int> AllElements { get; set; }
        public List<int> BElements { get; set; }
        public List<int> IElements { get; set; }
        public List<int> NElements { get; set; }
        public List<int> GElements { get; set; }
        public List<int> OElements { get; set; }
        public List<int> Elements1 { get; set; }
        public List<int> Elements2 { get; set; }
        public List<int> Elements3 { get; set; }
        public List<int> Elements4 { get; set; }
        public List<int> Elements5 { get; set; }
    }
}
