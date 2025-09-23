using BingoCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingoCreator.Services
{
    internal class ExportingService
    {
        public static void ExportDataBase(int cardsetid)
        {
            CardSetModel cards = new CardSetModel();

            //Função para pegar CardSet pelo Id

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string dbPath = Path.Combine(desktop, cards.Name, "CustomBingoDB.db");

            DataService.ExportGameDatabaseToPath(cards.Id, dbPath);
        }
    }
}
