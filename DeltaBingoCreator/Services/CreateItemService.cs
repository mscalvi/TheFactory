using DeltaBingoCreator.Models;
using DeltaBingoCreator.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace DeltaBingoCreator.Services
{
    class CreateItemService
    {
        public static string Message = "";

        public static void CreateElement(ItemModel element, ListModel list)
        {
            int maxNameLength = 100;
            int maxCardNameLength = 60;
            int maxNotesLength = 250;

            element.Note1 ??= "";
            element.Note2 ??= "";

            if (string.IsNullOrEmpty(element.Name) || string.IsNullOrEmpty(element.CardName))
            {
                Message = "Nome e Nome para Cartela são obrigatórios.";
                return;
            }

            if (element.Name.Length > maxNameLength)
            {
                Message = $"Nome do Elemento deve ter no máximo {maxNameLength} caracteres.";
                return;
            }

            if (element.CardName.Length > maxCardNameLength)
            {
                Message = $"O Nome para Cartela deve ter no máximo {maxCardNameLength} caracteres.";
                return;
            }

            if (element.Note1.Length > maxNotesLength)
            {
                Message = $"A anotação 1 deve ter no máximo {maxNotesLength} caracteres.";
                return;            
            }

            if (element.Note2.Length > maxNotesLength)
            {
                Message = $"A anotação 2 deve ter no máximo {maxNotesLength} caracteres.";
                return;
            }

            try
            {
                string relativePath = Path.Combine("images", ".nolist", element.CardName + ".png");
                element.AddTime = DateTime.Now.ToString("MMddyyyy - HH:mm:ss");
                element.Id = CreatorDataBase.CreateElement(element.Name, element.CardName, element.Note1, element.Note2, relativePath, element.AddTime);

                if (list.Id > 0)
                {
                    List<int> elements = new List<int>();
                    elements.Add(element.Id);

                    try
                    {
                        CreatorDataBase.AlocateElements(list.Id, elements);

                        Message = $"Elemento {element.Name} adicionado à Lista {list.Name} com sucesso.";
                        return;
                    }
                    catch (Exception ex)
                    {
                        Message = $"Falha ao adicionar Elemento {element.Name} à Lista {list.Name}.";
                        return;
                    }
                }
                else
                {
                    Message = $"Elemento {element.Name} criado com sucesso.";
                    return;
                }
            }
            catch (Exception ex)
            {
                Message = $"Falha ao criar Elemento {element.Name}.";
                return;
            }
        }
    }
}
