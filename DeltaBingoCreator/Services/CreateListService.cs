using DeltaBingoCreator.Database;
using DeltaBingoCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace DeltaBingoCreator.Services
{
    class CreateListService
    {
        public static string Message = "";

        public static void CreateList(ListModel list)
        {
            int maxNameLength = 100;
            int maxDescriptionLength = 500;

            if (string.IsNullOrEmpty(list.Description))
            {
                list.Description = "*";
            }

            if (string.IsNullOrEmpty(list.Name))
            {
                Message = "Nome da lista não pode estar em branco.";
                return;
            }

            if (list.Name.Length > maxNameLength)
            {
                Message = $"O nome da Lista deve ter no máximo {maxNameLength} caracteres.";
                return;
            }

            if (list.Description.Length > maxDescriptionLength)
            {
                Message = $"A descrição da Lista deve ter no máximo {maxDescriptionLength} caracteres.";
                return;
            }

            try
            {
                string relativePath = Path.Combine("images", "Capa.png");
                list.Id = CreatorDataBase.CreateList(list.Name, list.Description, relativePath);

                Message = $"Lista {list.Name} criada com sucesso.";
                return;
            }
            catch (Exception ex)
            {
                Message = $"Falha ao criar a lista {list.Name}.";
                return;
            }
        }
    }
}
