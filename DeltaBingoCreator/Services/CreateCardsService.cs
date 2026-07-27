using BingoCreator.Services;
using DeltaBingoCreator.Database;
using DeltaBingoCreator.Models;
using DeltaBingoCreator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltaBingoCreator.Services
{
    class CreateCardsService
    {
        public static string Message = "";

        public static void CreateCards(CardSetModel cards)
        {
            int maxNameLength = 50;
            int maxTitleLength = 120;
            int maxEndLength = 200;
            int minElements = 0;

            if (cards.CardsSize == 4)
            {
                minElements = 40;
            }
            else
            {
                minElements = 50;
            }

            ListModel list = FinderDataBase.GetListById(cards.ListId);
            cards.ListName = list.Name;
            cards.ListSize = list.ElementCount;
            cards.ImageName = list.ImageName;

            if (string.IsNullOrEmpty(cards.Name))
            {
                Message = "O nome do conjunto é obrigatório!";
                return;
            }

            if (cards.Name.Length > maxNameLength)
            {
                Message = $"O nome do conunto deve ter no máximo {maxNameLength} caracteres!";
                return;
            }

            if (string.IsNullOrEmpty(cards.Title))
            {
                Message = "O título da cartela é obrigatório!";
                return;
            }

            if (cards.Title.Length > maxTitleLength)
            {
                Message = $"O título da cartela deve ter no máximo {maxTitleLength} caracteres!";
                return;
            }

            if (string.IsNullOrEmpty(cards.End))
            {
                Message = "A mensagem final da cartela é obrigatória!";
                return;
            }

            if (cards.End.Length > maxTitleLength)
            {
                Message = $"A mensagem final da cartela deve ter no máximo {maxEndLength} caracteres!";
                return;
            }

            if (cards.ListSize < minElements)
            {
                Message = $"A lista {cards.ListName} tem apenas {cards.ListSize} Elementos, o mínimo é de {minElements}.";
                return;
            }

            try
            {
                cards.Id = GeneratingService.CreateCards(cards);

                Message = "Cartelas adicionadas ao Bando de Dados com sucesso.";
                return;
            }
            catch (Exception ex)
            {
                Message = $"Falha ao criar as cartelas {cards.Name}.";
                return;
            }
        }
    }
}
