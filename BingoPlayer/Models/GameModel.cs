namespace BingoPlayer.Models
{
    public class GameModel
    {
        //Nome do Jogo
        public string GameName { get; set; }

        //Total de Cartelas
        public int GameQuant { get; set; }

        //Design do Jogo
        public string GameTheme { get; set; }
        public string GameHeader { get; set; }
        public string GameTitle { get; set; }

        //Lista de Cartelas
        public List<CardModel> GameCards { get; set; }

        //Listas de Elementos
        public List<ElementModel> AllElements { get; set; }
        public List<ElementModel> BElements { get; set; }
        public List<ElementModel> IElements { get; set; }
        public List<ElementModel> NElements { get; set; }
        public List<ElementModel> GElements { get; set; }
        public List<ElementModel> OElements { get; set; }
    }
}
