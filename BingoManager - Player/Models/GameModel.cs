using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingoManager.Models
{
    public class GameModel
    {
        //Nome do Jogo
        public string GameName { get; set; }

        //Total de Cartelas
        public int GameTotal { get; set; }

        //Listas de Elementos
        public List<ElementModel> BElements { get; set; }
        public List<ElementModel> IElements { get; set; }
        public List<ElementModel> NElements { get; set; }
        public List<ElementModel> GElements { get; set; }
        public List<ElementModel> OElements { get; set; }

        //Lista de Cartelas
        public List<CardModel> GameCards { get; set; }
    }
}
