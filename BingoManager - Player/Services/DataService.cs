using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;        
using System.IO;
using System.Linq;
using BingoManager.Models;

namespace BingoManager.Services
{
    public static class DataService
    {
        private static readonly string _connectionString;

        // Construtor estático: garante a pasta no AppData e a connection-string
        static DataService()
        {
            string exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            string databasePath = Path.Combine(exeFolder, "Content", "CustomBingoDB.db");
            _connectionString = $"Data Source={databasePath};Version=3;";
        }

        private static SQLiteConnection GetConnection() =>
            new SQLiteConnection(_connectionString);

        // Carrega o Set (jogo) e metadados da lista associada
        public static GameModel GetGameInfo(int setId)
        {
            using var connection = GetConnection();
            connection.Open();

            string selectGame = @"
                SELECT Title,
                       Qnt,
                       GroupB,
                       GroupI,
                       GroupN,
                       GroupG,
                       GroupO
                FROM CardsSets
                WHERE SetId = @SetId
                LIMIT 1";
            using var cmdGame = new SQLiteCommand(selectGame, connection);
            cmdGame.Parameters.AddWithValue("@SetId", setId);

            using var adapter = new SQLiteDataAdapter(cmdGame);
            var dtGame = new DataTable();
            adapter.Fill(dtGame);

            if (dtGame.Rows.Count == 0)
                return null; // ou lance exceção, se preferir

            DataRow row = dtGame.Rows[0];
            string title = row["Title"].ToString();
            int quantity = Convert.ToInt32(row["Qnt"]);
            string grpB = row["GroupB"].ToString();
            string grpI = row["GroupI"].ToString();
            string grpN = row["GroupN"].ToString();
            string grpG = row["GroupG"].ToString();
            string grpO = row["GroupO"].ToString();

            List<ElementModel> ElemB = new List<ElementModel>();
            List<ElementModel> ElemI = new List<ElementModel>();
            List<ElementModel> ElemN = new List<ElementModel>();
            List<ElementModel> ElemG = new List<ElementModel>();
            List<ElementModel> ElemO = new List<ElementModel>();

            var gameCards = new List<CardModel>();
            for (int cardNum = 1; cardNum <= quantity; cardNum++)
            {
                CardModel card = GetCardDetails(cardNum);
                if (card != null)
                    gameCards.Add(card);
            }

            ElemB = GetElementsInfo(grpB);
            ElemI = GetElementsInfo(grpI);
            ElemN = GetElementsInfo(grpN);
            ElemG = GetElementsInfo(grpG);
            ElemO = GetElementsInfo(grpO);

            return new GameModel
            {
                GameName = title,
                GameTotal = quantity,
                BElements = ElemB,
                IElements = ElemI,
                NElements = ElemN,
                GElements = ElemG,
                OElements = ElemO,
                GameCards = gameCards
            };
        }

        // A partir de uma string “1,5,8,12,...”, retorna lista de ElementModel (ID, Name, CardName, ImageName)
        public static List<ElementModel> GetElementsInfo(string elementIds)
        {
            var Elements = new List<ElementModel>();
            if (string.IsNullOrWhiteSpace(elementIds))
                return Elements;

            var ids = elementIds.Split(',').Select(x => x.Trim()).ToList();
            string query = $"SELECT Id, Name, CardName, ImageName FROM ElementsTable WHERE Id IN ({string.Join(",", ids)})";

            using var connection = GetConnection();
            connection.Open();
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            var dictionary = new Dictionary<int, ElementModel>();
            while (reader.Read())
            {
                var c = new ElementModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString(),
                    CardName = reader["CardName"].ToString(),
                    ImageName = reader["ImageName"].ToString()
                };
                dictionary[c.Id] = c;
            }

            // Reconstrói na mesma ordem de 'ids'
            Elements = ids.Select(id => dictionary[int.Parse(id)]).ToList();
            return Elements;
        }

        // Carrega a imagem de cada elemento
        public static Image LoadImageFromFile(int elementId)
        {
            // 1. Buscar o nome do arquivo de imagem no banco
            string imageFileName = null;
            using (var connection = GetConnection())
            {
                if (connection == null) return null;
                connection.Open();

                string sql = "SELECT ImageName FROM ElementsTable WHERE Id = @Id";
                using var cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@Id", elementId);
                using var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    imageFileName = reader["ImageName"].ToString();
                }
                else
                {
                    return null;
                }
            }

            if (string.IsNullOrEmpty(imageFileName))
                return null;

            string exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            string imageFolderPath = Path.Combine(exeFolder, "Images");
            string filePath = Path.Combine(imageFolderPath, imageFileName);

            return File.Exists(filePath) ? Image.FromFile(filePath) : null;
        }

        // Para cada cartela, traz todos os 25 IDs de elementos (B, I, N, G, O)
        public static CardModel GetCardDetails(int cardNum)
        {
            using var connection = GetConnection();
            connection.Open();

            string selectQuery = @"
                SELECT Id, CardNumber,
                       EleB1, EleB2, EleB3, EleB4, EleB5,
                       EleI1, EleI2, EleI3, EleI4, EleI5,
                       EleN1, EleN2, EleN3, EleN4, EleN5,
                       EleG1, EleG2, EleG3, EleG4, EleG5,
                       EleO1, EleO2, EleO3, EleO4, EleO5
                FROM CardsList
                WHERE CardNumber = @CardNum";

            using var command = new SQLiteCommand(selectQuery, connection);
            command.Parameters.AddWithValue("@CardNum", cardNum);

            using var reader = command.ExecuteReader();
            if (!reader.Read()) return null;

            var bElements = new List<int>();
            var iElements = new List<int>();
            var nElements = new List<int>();
            var gElements = new List<int>();
            var oElements = new List<int>();

            for (int i = 2; i <= 6; i++) bElements.Add(reader.GetInt32(i));
            for (int i = 7; i <= 11; i++) iElements.Add(reader.GetInt32(i));
            for (int i = 12; i <= 16; i++) nElements.Add(reader.GetInt32(i));
            for (int i = 17; i <= 21; i++) gElements.Add(reader.GetInt32(i));
            for (int i = 22; i <= 26; i++) oElements.Add(reader.GetInt32(i));

            var all = new List<int>();
            all.AddRange(bElements);
            all.AddRange(iElements);
            all.AddRange(nElements);
            all.AddRange(gElements);
            all.AddRange(oElements);

            return new CardModel
            {
                CardId = reader.GetInt32(0),
                CardNumber = reader.GetInt32(1),
                AllElements = all,
                BElements = bElements,
                IElements = iElements,
                NElements = nElements,
                GElements = gElements,
                OElements = oElements,
                Elements1 = new List<int> { bElements[0], iElements[0], nElements[0], gElements[0], oElements[0] },
                Elements2 = new List<int> { bElements[1], iElements[1], nElements[1], gElements[1], oElements[1] },
                Elements3 = new List<int> { bElements[2], iElements[2], nElements[2], gElements[2], oElements[2] },
                Elements4 = new List<int> { bElements[3], iElements[3], nElements[3], gElements[3], oElements[3] },
                Elements5 = new List<int> { bElements[4], iElements[4], nElements[4], gElements[4], oElements[4] }
            };
        }

        // Retorna quais cartelas (e números de cartela) contêm determinado elemento
        public static List<(int CardId, int CardNum)> GetCardsByElementId(int elementId)
        {
            var cards = new List<(int CardId, int CardNum)>();
            using var connection = GetConnection();
            connection.Open();

            string selectQuery = @"
                SELECT Id, CardNumber
                FROM CardsList
                WHERE 
                    (EleB1 = @elementId OR EleB2 = @elementId OR EleB3 = @elementId OR EleB4 = @elementId OR EleB5 = @elementId OR
                     EleI1 = @elementId OR EleI2 = @elementId OR EleI3 = @elementId OR EleI4 = @elementId OR EleI5 = @elementId OR
                     EleN1 = @elementId OR EleN2 = @elementId OR EleN3 = @elementId OR EleN4 = @elementId OR EleN5 = @elementId OR
                     EleG1 = @elementId OR EleG2 = @elementId OR EleG3 = @elementId OR EleG4 = @elementId OR EleG5 = @elementId OR
                     EleO1 = @elementId OR EleO2 = @elementId OR EleO3 = @elementId OR EleO4 = @elementId OR EleO5 = @elementId)";

            using var command = new SQLiteCommand(selectQuery, connection);
            command.Parameters.AddWithValue("@elementId", elementId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                cards.Add((reader.GetInt32(0), reader.GetInt32(1)));
            }

            return cards;
        }

        // Retorna uma lista com todas as cartelas do jogo
        public static List<CardModel> GetGameCards(int setId)
        {
            GameModel game = GetGameInfo(setId);
            if (game == null) return new List<CardModel>();

            var cards = new List<CardModel>();
            for (int cardNum = 1; cardNum <= game.GameTotal; cardNum++)
            {
                CardModel card = GetCardDetails(cardNum);
                if (card != null)
                    cards.Add(card);
            }
            return cards;
        }

    }
}
