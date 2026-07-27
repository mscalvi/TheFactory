using DeltaBingoCreator.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltaBingoCreator.Database
{
    class FinderDataBase
    {
        // Elementos
        // Encontrar Elemento pelo ID
        public static DataRow GetElementById(int elementId)
        {
            using (var connection = MainDataBase.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM ElementsTable WHERE Id = @Id";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", elementId);

                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable elementsTable = new DataTable();
                        adapter.Fill(elementsTable);

                        if (elementsTable.Rows.Count > 0)
                        {
                            return elementsTable.Rows[0];
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        // Método para retornar o ID de um Elemento pelo CardName
        public static int GetElementByCardName(string cardName)
        {
            using var connection = MainDataBase.GetConnection();
            connection.Open();

            string query = "SELECT Id FROM ElementsTable WHERE CardName = @CardName LIMIT 1";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@CardName", cardName);

            object result = command.ExecuteScalar();
            if (result != null && int.TryParse(result.ToString(), out int id))
            {
                return id;
            }

            return 0;
        }

        // Método para retornar todos os Elementos criados
        public static DataTable GetAllElements()
        {
            using (var connection = MainDataBase.GetConnection())
            {
                connection.Open();
                const string sql = @"
                    SELECT Id,
                           CASE WHEN IFNULL(TRIM(CardName),'') <> '' THEN CardName ELSE Name END AS CardName,
                           Name
                    FROM ElementsTable
                    ORDER BY CardName, Name;";
                using var cmd = new SQLiteCommand(sql, connection);
                using var adp = new SQLiteDataAdapter(cmd);
                var dt = new DataTable();
                adp.Fill(dt);
                return dt;
            }
        }


        // Listas
        // Confere se Elemento já está em uma Lista
        public static bool IsElementInList(int listId, int elementId)
        {
            const string sql = @"SELECT 1 FROM AlocationTable 
                         WHERE ListId=@ListId AND ElementId=@ElementId LIMIT 1;";
            using (var connection = MainDataBase.GetConnection())
            {
                connection.Open();
                using var cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@ListId", listId);
                cmd.Parameters.AddWithValue("@ElementId", elementId);
                return cmd.ExecuteScalar() != null;
            }
        }

        // Retornar todos os IDs de Elementos em uma Lista
        public static List<DataRow> GetElementsByIds(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return new List<DataRow>();

            string idList = string.Join(",", ids);
            string sql = $"SELECT Id, Name, CardName, ImageName FROM ElementsTable WHERE Id IN ({idList})";

            using (var connection = MainDataBase.GetConnection())
            {
                connection.Open();
                using var cmd = new SQLiteCommand(sql, connection);
                using var adapter = new SQLiteDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);
                return dt.AsEnumerable().ToList();
            }
        }

        // Método para retornar todas as Listas
        public static DataTable GetLists()
        {
            using (var connection = MainDataBase.GetConnection())
            {
                connection.Open();
                string selectQuery = "SELECT * FROM ListsTable;";

                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        // Método para retornar todos os Elementos de uma Lista
        public static List<DataRow> GetElementsInList(int listId)
        {
            List<DataRow> elementsList = new List<DataRow>();

            string query = "SELECT c.Id, c.Name, c.CardName, c.ImageName " +
                           "FROM ElementsTable c " +
                           "INNER JOIN AlocationTable a ON c.Id = a.ElementId " +
                           "WHERE a.ListId = @ListId";

            using (var connection = MainDataBase.GetConnection())
            {
                connection.Open();

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ListId", listId);

                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable elementsTable = new DataTable();
                        adapter.Fill(elementsTable);

                        elementsList = elementsTable.AsEnumerable().ToList();
                    }
                }
            }

            return elementsList;
        }

        // Método para retornar uma Lista pelo Id
        public static ListModel? GetListById(int listId)
        {
            const string sql = @"
        SELECT 
            l.Id,
            l.Name,
            l.Description,
            l.ImageName,
            COUNT(a.ElementId) AS ElementCount
        FROM ListsTable l
        LEFT JOIN AlocationTable a ON a.ListId = l.Id
        WHERE l.Id = @ListId
        GROUP BY l.Id, l.Name, l.Description, l.ImageName
        LIMIT 1;";

            using (var connection = MainDataBase.GetConnection())
            {
                connection.Open();

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ListId", listId);

                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        if (dt.Rows.Count == 0) return null;

                        var row = dt.Rows[0];
                        return new ListModel
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Name = row["Name"]?.ToString() ?? "",
                            Description = row["Description"]?.ToString() ?? "",
                            ImageName = row["ImageName"]?.ToString() ?? "",
                            ElementCount = row["ElementCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["ElementCount"])
                        };
                    }
                }
            }
        }

        // Método para retornar todas as Listas que contém determinado Elemento
        public static string GetListsForElement(int elementId)
        {
            const string sql = @"
        SELECT l.Name
        FROM AlocationTable a
        INNER JOIN ListsTable l ON l.Id = a.ListId
        WHERE a.ElementId = @ElementId
        ORDER BY l.Name;";

            using (var connection = MainDataBase.GetConnection())
            {
                connection.Open();

                using var cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@ElementId", elementId);

                using var reader = cmd.ExecuteReader();

                var names = new List<string>();
                while (reader.Read())
                {
                    var name = reader["Name"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(name))
                        names.Add(name.Trim());
                }

                // retorna string vazia se não houver listas
                return string.Join("; ", names);
            }
        }


        // Cards
        // Método para retornar um Card Set pelo Id
        public static CardSetModel? GetCardSetById(int setId)
        {
            const string sql = @"
        SELECT
            SetId,
            ListId,
            Name,
            Title,
            End,
            Quantity,
            CardsSize,
            AddTime,
            GroupB,
            GroupI,
            GroupN,
            GroupG,
            GroupO,
            Elements,
            Theme,
            Header,
            Model
        FROM CardsSets
        WHERE SetId = @SetId
        LIMIT 1;";

            using var connection = MainDataBase.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@SetId", setId);

            using var adapter = new SQLiteDataAdapter(command);
            var dt = new DataTable();
            adapter.Fill(dt);

            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];

            // ————— Helpers locais —————
            static List<int> ParseIds(string? csv)
            {
                if (string.IsNullOrWhiteSpace(csv)) return new List<int>();
                return csv
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => s.Length > 0 && int.TryParse(s, out _))
                    .Select(int.Parse)
                    .ToList();
            }

            //static List<ElementModel> LoadElements(List<int> ids)
            //{
            //    if (ids.Count == 0) return new List<ElementModel>();
            //    var rows = GetElementsByIds(ids); // já existe no seu DataService
            //    return rows.Select(r => new ElementModel
            //    {
            //        Id = Convert.ToInt32(r["Id"]),
            //        Name = r["Name"]?.ToString() ?? "",
            //        CardName = r["CardName"]?.ToString() ?? "",
            //        // Se precisar: ImageName = r.Table.Columns.Contains("ImageName") ? r["ImageName"]?.ToString() ?? "" : ""
            //    }).ToList();
            //}

            static List<ItemModel> LoadElements(List<int> ids)
            {
                if (ids.Count == 0)
                    return new List<ItemModel>();

                var rows = GetElementsByIds(ids);

                var map = rows.ToDictionary(
                    r => Convert.ToInt32(r["Id"])
                );

                return ids
                    .Where(id => map.ContainsKey(id))
                    .Select(id =>
                    {
                        var r = map[id];

                        return new ItemModel
                        {
                            Id = id,
                            Name = r["Name"]?.ToString() ?? "",
                            CardName = r["CardName"]?.ToString() ?? ""
                        };
                    })
                    .ToList();
            }
            // ————————————————————————

            var model = new CardSetModel
            {
                Id = Convert.ToInt32(row["SetId"]),
                ListId = Convert.ToInt32(row["ListId"]),
                Name = row["Name"]?.ToString() ?? "",
                Title = row["Title"]?.ToString() ?? "",
                End = row["End"]?.ToString() ?? "",
                Quantity = Convert.ToInt32(row["Quantity"]),
                CardsSize = Convert.ToInt32(row["CardsSize"]),
                AddDate = row["AddTime"]?.ToString() ?? "",

                // CSVs dos grupos/elements
                GroupBIds = row["GroupB"]?.ToString() ?? "",
                GroupIIds = row["GroupI"]?.ToString() ?? "",
                GroupNIds = row["GroupN"]?.ToString() ?? "",
                GroupGIds = row["GroupG"]?.ToString() ?? "",
                GroupOIds = row["GroupO"]?.ToString() ?? "",

                // 4x4
                // Elements está na coluna "Elements" — será usado para preencher AllElements

                // Estilo/tema/cabeçalho
                Theme = row.Table.Columns.Contains("Theme") ? (row["Theme"]?.ToString() ?? "") : "",
                Header = row.Table.Columns.Contains("Header") ? (row["Header"]?.ToString() ?? "") : "",
                Model = row.Table.Columns.Contains("Model") ? (row["Model"]?.ToString() ?? "") : ""
            };

            // Preencher ListName e ListSize (ElementCount) via ListsTable
            var list = GetListById(model.ListId); // seu método que retorna ListModel?
            model.ListName = list?.Name ?? "";
            model.ListSize = list?.ElementCount ?? 0;

            // Carregar elementos conforme o tamanho das cartelas
            if (model.CardsSize == 5)
            {
                var idsB = ParseIds(model.GroupBIds);
                var idsI = ParseIds(model.GroupIIds);
                var idsN = ParseIds(model.GroupNIds);
                var idsG = ParseIds(model.GroupGIds);
                var idsO = ParseIds(model.GroupOIds);

                model.GroupB = LoadElements(idsB);
                model.GroupI = LoadElements(idsI);
                model.GroupN = LoadElements(idsN);
                model.GroupG = LoadElements(idsG);
                model.GroupO = LoadElements(idsO);

                // União para AllElements (sem duplicatas)
                model.AllElements = new[] { model.GroupB, model.GroupI, model.GroupN, model.GroupG, model.GroupO }
                    .Where(g => g != null)
                    .SelectMany(g => g!)
                    .GroupBy(e => e.Id)
                    .Select(g => g.First())
                    .ToList();
            }
            else if (model.CardsSize == 4)
            {
                var rows = GetElementsInList(model.ListId); // DataRows: Id, Name, CardName, ImageName
                model.AllElements = rows
                    .Select(r => new ItemModel
                    {
                        Id = Convert.ToInt32(r["Id"]),
                        Name = r["Name"]?.ToString() ?? "",
                        CardName = r["CardName"]?.ToString() ?? ""
                    })
                    .OrderBy(e => string.IsNullOrWhiteSpace(e.CardName) ? e.Name : e.CardName)
                    .ToList();

                // Mantém Group* como null em 4x4
                model.GroupB = null;
                model.GroupI = null;
                model.GroupN = null;
                model.GroupG = null;
                model.GroupO = null;
            }
            else
            {
                // Se algum dado antigo tiver CardsSize diferente, apenas evita nulls
                model.AllElements ??= new List<ItemModel>();
            }

            return model;
        }

        // Método par retornar todas as Cards de um Set
        public static List<DataRow> GetCardsBySetId(int setId)
        {
            const string metaSql = @"
        SELECT CardsSize
        FROM CardsSets
        WHERE SetId = @SetId
        LIMIT 1;";

            using var conn = MainDataBase.GetConnection();
            conn.Open();

            using (var metaCmd = new SQLiteCommand(metaSql, conn))
            {
                metaCmd.Parameters.AddWithValue("@SetId", setId);
                object? cs = metaCmd.ExecuteScalar();
                if (cs == null) return new List<DataRow>();

                int cardsSize = Convert.ToInt32(cs);

                string listSql = cardsSize == 5
                    ? "SELECT * FROM CardsList5Table WHERE SetId = @SetId ORDER BY CardNumber;"
                    : "SELECT * FROM CardsList4Table WHERE SetId = @SetId ORDER BY CardNumber;";

                using var listCmd = new SQLiteCommand(listSql, conn);
                listCmd.Parameters.AddWithValue("@SetId", setId);

                using var adp = new SQLiteDataAdapter(listCmd);
                var dt = new DataTable();
                adp.Fill(dt);
                return dt.AsEnumerable().ToList();
            }
        }

        // Método para retornar todos os Elementos de uma cartela por CardSet
        public static List<List<ItemModel>> GetCardElementsBySet(List<DataRow> setCards)
        {
            var result = new List<List<ItemModel>>();
            if (setCards == null || setCards.Count == 0) return result;

            // 1) Coletar TODOS os IDs utilizados em todas as cartelas (para uma única ida ao DB)
            var allIds = setCards
                .SelectMany(GetIdsFromCardRowAuto) // detecta 5x5 ou 4x4 pela presença das colunas
                .Distinct()
                .ToList();

            if (allIds.Count == 0) return result;

            // 2) Buscar os elementos e indexar por Id
            var elementRows = GetElementsByIds(allIds); // seu método existente
            var elemById = elementRows.ToDictionary(r => Convert.ToInt32(r["Id"]));

            // 3) Montar, para cada cartela, a lista de ElementModel na ordem esperada
            foreach (var row in setCards)
            {
                var ids = GetIdsFromCardRowAuto(row);
                var oneCard = new List<ItemModel>();

                foreach (var id in ids)
                {
                    if (!elemById.TryGetValue(id, out var er)) continue;

                    oneCard.Add(new ItemModel
                    {
                        Id = id,
                        Name = er["Name"]?.ToString() ?? "",
                        CardName = er["CardName"]?.ToString() ?? "",
                        // Se precisar:
                        // ImageName = er.Table.Columns.Contains("ImageName") ? er["ImageName"]?.ToString() ?? "" : ""
                    });
                }

                result.Add(oneCard);
            }

            return result;

            // ===== Helpers =====

            // Detecta automaticamente o tipo de cartela pela existência das colunas
            static IEnumerable<int> GetIdsFromCardRowAuto(DataRow r)
            {
                if (r.Table.Columns.Contains("EleB1"))
                {
                    // 5x5: B1..B5, I1..I5, N1..N5, G1..G5, O1..O5
                    for (int i = 1; i <= 5; i++) yield return ToId(r[$"EleB{i}"]);
                    for (int i = 1; i <= 5; i++) yield return ToId(r[$"EleI{i}"]);
                    for (int i = 1; i <= 5; i++) yield return ToId(r[$"EleN{i}"]);
                    for (int i = 1; i <= 5; i++) yield return ToId(r[$"EleG{i}"]);
                    for (int i = 1; i <= 5; i++) yield return ToId(r[$"EleO{i}"]);
                }
                else
                {
                    // 4x4: Ele1..Ele16
                    for (int i = 1; i <= 16; i++) yield return ToId(r[$"Ele{i}"]);
                }
            }

            static int ToId(object value)
            {
                // defensivo contra DBNull/strings
                return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
            }
        }

        // Método para retornar todos os Card Sets criados
        public static DataTable GetAllCardSets()
        {
            using (var connection = MainDataBase.GetConnection())
            {
                connection.Open();

                const string query = @"
                    SELECT SetId AS Id, Name, Title, CardsSize, Quantity, AddTime
                    FROM CardsSets
                    ORDER BY datetime(AddTime) DESC;";
                using var cmd = new SQLiteCommand(query, connection);
                using var adp = new SQLiteDataAdapter(cmd);
                var dt = new DataTable();
                adp.Fill(dt);
                return dt;
            }
        }

        // Método para retornar todos os CardSets que usam determinada Lista
        public static DataTable GetCardSetsByListId(int listId)
        {
            using var conn = MainDataBase.GetConnection();
            conn.Open();

            // Obs.: se quiser por data, troque para ORDER BY datetime(AddTime) DESC,
            // mas seu formato "MMddyyyy - HH:mm:ss" pode não ordenar bem como datetime do SQLite.
            const string sql = @"
        SELECT SetId, Name, Title, CardsSize, Quantity, AddTime
        FROM CardsSets
        WHERE ListId = @ListId
        ORDER BY SetId DESC;";

            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ListId", listId);

            using var adp = new SQLiteDataAdapter(cmd);
            var dt = new DataTable();
            adp.Fill(dt);
            return dt;
        }

        // Máximo número de cartela já existente no set (0 se não há)
        public static int GetMaxCardNumberBySetId(int setId)
        {
            using var conn = MainDataBase.GetConnection();
            conn.Open();

            // tenta nas 5x5
            using (var cmd = new SQLiteCommand(
                "SELECT IFNULL(MAX(CardNumber),0) FROM CardsList5Table WHERE SetId=@SetId;", conn))
            {
                cmd.Parameters.AddWithValue("@SetId", setId);
                var val = Convert.ToInt32(cmd.ExecuteScalar());
                if (val > 0) return val;
            }

            // tenta nas 4x4
            using (var cmd = new SQLiteCommand(
                "SELECT IFNULL(MAX(CardNumber),0) FROM CardsList4Table WHERE SetId=@SetId;", conn))
            {
                cmd.Parameters.AddWithValue("@SetId", setId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Assinaturas existentes (5x5): "B1-B2-...-O5"
        public static HashSet<string> GetExistingSignatures5(int setId)
        {
            var set = new HashSet<string>(StringComparer.Ordinal);
            using var conn = MainDataBase.GetConnection();
            conn.Open();

            using var cmd = new SQLiteCommand(
                "SELECT EleB1,EleB2,EleB3,EleB4,EleB5," +
                "       EleI1,EleI2,EleI3,EleI4,EleI5," +
                "       EleN1,EleN2,EleN3,EleN4,EleN5," +
                "       EleG1,EleG2,EleG3,EleG4,EleG5," +
                "       EleO1,EleO2,EleO3,EleO4,EleO5 " +
                "FROM CardsList5Table WHERE SetId=@SetId;", conn);
            cmd.Parameters.AddWithValue("@SetId", setId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var ids = new List<int>(25);
                for (int i = 0; i < 25; i++) ids.Add(Convert.ToInt32(r.GetValue(i)));
                set.Add(string.Join("-", ids));
            }
            return set;
        }

        // Assinaturas existentes (4x4): "E1-E2-...-E16"
        public static HashSet<string> GetExistingSignatures4(int setId)
        {
            var set = new HashSet<string>(StringComparer.Ordinal);
            using var conn = MainDataBase.GetConnection();
            conn.Open();

            using var cmd = new SQLiteCommand(
                "SELECT Ele1,Ele2,Ele3,Ele4,Ele5,Ele6,Ele7,Ele8," +
                "       Ele9,Ele10,Ele11,Ele12,Ele13,Ele14,Ele15,Ele16 " +
                "FROM CardsList4Table WHERE SetId=@SetId;", conn);
            cmd.Parameters.AddWithValue("@SetId", setId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var ids = new List<int>(16);
                for (int i = 0; i < 16; i++) ids.Add(Convert.ToInt32(r.GetValue(i)));
                set.Add(string.Join("-", ids));
            }
            return set;
        }
    }
}
