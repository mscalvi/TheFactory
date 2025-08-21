using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingoManager.Services
{
    public static class PlayService
    {
        private static List<int> drawnElements = new List<int>();
        private static List<int> drawnCards = new List<int>();

        // Método para adicionar uma empresa à lista das sorteadas
        public static void AddElement(int elementId)
        {
            if (!drawnElements.Contains(elementId))
            {
                drawnElements.Add(elementId);
            } else
            {
                drawnElements.Remove(elementId);
            }
        }

        //Método para conferir cartelas que possuem a última empresa sorteada
        public static List<int> CheckCards(int elementId)
        {
            var cardData = DataService.GetCardsByElementId(elementId);

            List<int> cardNumbers = new List<int>();
            foreach (var (CardId, CardNum) in cardData)
            {
                cardNumbers.Add(CardNum);
            }

            return cardNumbers; // Retorna a lista de números das cartelas
        }

        //Método para conferir bingo
        public static List<int> CheckBingo(List<int> chosenCards, int bingoPhase, int compId)
        {
            List<int> winningCards = new List<int>();
            List<int> drawnNumbers = drawnElements;

            foreach (var card in chosenCards)
            {
                var cardDetails = DataService.GetCardDetails(card);


                if (bingoPhase == 2)
                {
                    bool isFullBingo = cardDetails.AllElements.All(num => drawnNumbers.Contains(num));
                    if (isFullBingo)
                    {
                        winningCards.Add(card);
                        continue;
                    }
                }

                if (bingoPhase == 1)
                {
                    // Verificar em qual linha (Elements1-5) está o elemento sorteado
                    List<List<int>> rows = new List<List<int>>
                    {
                        cardDetails.Elements1,
                        cardDetails.Elements2,
                        cardDetails.Elements3,
                        cardDetails.Elements4,
                        cardDetails.Elements5
                    };

                    int rowIndex = rows.FindIndex(row => row.Contains(compId));

                    // Verificar em qual coluna (B-I-N-G-O) está a empresa sorteada
                    List<List<int>> columns = new List<List<int>>
                    {
                        cardDetails.BElements,
                        cardDetails.IElements,
                        cardDetails.NElements,
                        cardDetails.GElements,
                        cardDetails.OElements
                    };

                    int colIndex = columns.FindIndex(col => col.Contains(compId));

                    bool rowComplete = false;
                    bool colComplete = false;

                    if (rowIndex != -1)
                    {
                        rowComplete = rows[rowIndex].All(num => drawnNumbers.Contains(num));
                    }

                    if (colIndex != -1)
                    {
                        colComplete = columns[colIndex].All(num => drawnNumbers.Contains(num));
                    }

                    if (rowComplete || colComplete)
                    {
                        winningCards.Add(card);
                    }
                }
            }

            return winningCards;
        }

        public static void ResetGame()
        {
            drawnElements.Clear(); 
            drawnCards.Clear();
        }

    }
}
