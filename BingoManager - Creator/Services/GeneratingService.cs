using BingoCreator.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace BingoCreator.Services
{
    internal class GeneratingService
    {
        public static int CreateCards(CardSetModel cards)
        {
            Random random = new Random();

            List<List<DataRow>> allCards = new List<List<DataRow>>();

            List<DataRow> ElementsList = DataService.GetElementsInList(cards.ListId);

            ElementsList = ElementsList.OrderBy(x => random.Next()).ToList();

            cards.AllElements = ElementsList.Select(ToElementModel).ToList();

            int elementsPerColumn = 1;
            int remainder = 1;

            cards.AddDate = DateTime.Now.ToString("MMddyyyy - HH:mm:ss");

            if (cards.CardsSize == 5)
            {
                elementsPerColumn = ElementsList.Count / 5;
                remainder = ElementsList.Count % 5;

                List<DataRow> columnB = ElementsList.Take(elementsPerColumn + (remainder > 0 ? 1 : 0)).ToList();
                List<DataRow> columnI = ElementsList.Skip(columnB.Count).Take(elementsPerColumn + (remainder > 1 ? 1 : 0)).ToList();
                List<DataRow> columnN = ElementsList.Skip(columnB.Count + columnI.Count).Take(elementsPerColumn + (remainder > 2 ? 1 : 0)).ToList();
                List<DataRow> columnG = ElementsList.Skip(columnB.Count + columnI.Count + columnN.Count).Take(elementsPerColumn + (remainder > 3 ? 1 : 0)).ToList();
                List<DataRow> columnO = ElementsList.Skip(columnB.Count + columnI.Count + columnN.Count + columnG.Count).Take(elementsPerColumn).ToList();

                cards.GroupB = columnB.Select(ToElementModel).ToList();
                cards.GroupI = columnI.Select(ToElementModel).ToList();
                cards.GroupN = columnN.Select(ToElementModel).ToList();
                cards.GroupG = columnG.Select(ToElementModel).ToList();
                cards.GroupO = columnO.Select(ToElementModel).ToList();

                cards.GroupBIds = string.Join(",", columnB.Select(c => c["Id"].ToString()));
                cards.GroupIIds = string.Join(",", columnI.Select(c => c["Id"].ToString()));
                cards.GroupNIds = string.Join(",", columnN.Select(c => c["Id"].ToString()));
                cards.GroupGIds = string.Join(",", columnG.Select(c => c["Id"].ToString()));
                cards.GroupOIds = string.Join(",", columnO.Select(c => c["Id"].ToString()));

                int setId = DataService.CreateCardsSet(cards);

                for (int i = 1; i <= cards.Quantity; i++)
                {
                    var tempB = new List<DataRow>(columnB);
                    var tempI = new List<DataRow>(columnI);
                    var tempN = new List<DataRow>(columnN);
                    var tempG = new List<DataRow>(columnG);
                    var tempO = new List<DataRow>(columnO);
                    var selected = new List<DataRow>();

                    selected.AddRange(SelectAndRemoveFromGroup(tempB, 5, random));
                    selected.AddRange(SelectAndRemoveFromGroup(tempI, 5, random));
                    selected.AddRange(SelectAndRemoveFromGroup(tempN, 5, random));
                    selected.AddRange(SelectAndRemoveFromGroup(tempG, 5, random));
                    selected.AddRange(SelectAndRemoveFromGroup(tempO, 5, random));

                    var companyIds = selected.Select(c => Convert.ToInt32(c["Id"])).ToList();
                    if (companyIds.Count == 25)
                    {
                        DataService.CreateCard5(cards.ListId, companyIds, i, setId);
                        allCards.Add(selected);
                    }
                }
                return setId;

            } else if (cards.CardsSize == 4)
            {
                int setId = DataService.CreateCardsSet(cards);

                for (int i = 1; i <= cards.Quantity; i++)
                {
                    var tempList = new List<DataRow>(ElementsList);

                    var selected = SelectAndRemoveFromGroup(tempList, 16, random);

                    var elementIds = selected
                        .Select(c => Convert.ToInt32(c["Id"]))
                        .ToList();

                    if (elementIds.Count == 16)
                    {
                        DataService.CreateCard4(cards.ListId, elementIds, i, setId);
                        allCards.Add(selected);
                    }
                }

                return setId;
            } else
            {
                return -1;
            }
        }

        private static List<DataRow> SelectAndRemoveFromGroup(List<DataRow> group, int count, Random random)
        {
            var selected = new List<DataRow>();
            for (int i = 0; i < count && group.Count > 0; i++)
            {
                int idx = random.Next(group.Count);
                selected.Add(group[idx]);
                group.RemoveAt(idx);
            }
            return selected;
        }

        private static ElementModel ToElementModel(DataRow r) => new ElementModel
        {
            Id = Convert.ToInt32(r["Id"]),
            Name = r["Name"]?.ToString() ?? "",
            CardName = r["CardName"]?.ToString() ?? "",
            ImageName = r["ImageName"]?.ToString() ?? ""
        };
    }
}
