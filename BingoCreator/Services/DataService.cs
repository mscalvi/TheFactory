using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.ComponentModel.Design;
using BingoCreator.Models;

namespace BingoCreator.Services
{
    public static class DataService
    {
        private static readonly string _connectionString;

        // Conexão
        // Método principal de conexão
        static DataService()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string databaseFolder = Path.Combine(baseDir, "Database");

                Directory.CreateDirectory(databaseFolder);

                string databasePath = Path.Combine(databaseFolder, "BingoManager.db");
                _connectionString = $"Data Source={databasePath};Version=3;";

                InitializeDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar o banco de dados: {ex.Message}");
                throw;
            }
        }

        // Método para abrir uma conexão com o banco de dados
        private static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        // Método para inicializar o banco de dados (criar as tabelas se não existirem)
        public static void InitializeDatabase()
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
                {
                    command.ExecuteNonQuery();
                }

                var createTableCommands = new List<string>
        {
            // ----- Elements -----
            @"
            CREATE TABLE IF NOT EXISTS ElementsTable (
                Id INTEGER PRIMARY KEY NOT NULL UNIQUE,
                Name TEXT NOT NULL,
                CardName TEXT NOT NULL,
                Note1 TEXT,
                Note2 TEXT,
                ImageName TEXT,
                AddTime TEXT NOT NULL
            );",

            // ----- Lists -----
            @"
            CREATE TABLE IF NOT EXISTS ListsTable (
                Id INTEGER PRIMARY KEY,
                Name TEXT,
                Description TEXT,
                ImageName TEXT
            );",

            // ----- Alocation (Element <-> List) -----
            @"
            CREATE TABLE IF NOT EXISTS AlocationTable (
                ElementId INTEGER REFERENCES ElementsTable(Id),
                ListId INTEGER  REFERENCES ListsTable(Id),
                PRIMARY KEY (ElementId, ListId)
            );",

            // ===== Unified Card Sets (4x4 and 5x5) =====
            @"
            CREATE TABLE IF NOT EXISTS CardsSets (
                SetId     INTEGER PRIMARY KEY NOT NULL UNIQUE,
                ListId    INTEGER NOT NULL REFERENCES ListsTable(Id),
                Title     TEXT NOT NULL,
                End       TEXT,
                Quantity  INTEGER NOT NULL,
                Name      TEXT UNIQUE,
                CardsSize INTEGER NOT NULL,
                AddTime   TEXT,
                -- 5x5
                GroupB    TEXT,
                GroupI    TEXT,
                GroupN    TEXT,
                GroupG    TEXT,
                GroupO    TEXT,
                -- 4x4
                Elements  TEXT,
                -- NOVOS CAMPOS
                Theme     TEXT,
                Header    TEXT,
                Model     TEXT
            );
            );",

            // ----- Cards 5x5 (referência agora em CardsSets) -----
            @"
            CREATE TABLE IF NOT EXISTS CardsList5Table (
                Id INTEGER PRIMARY KEY,
                SetId INTEGER NOT NULL REFERENCES CardsSets(SetId),
                ListId INTEGER NOT NULL REFERENCES ListsTable(Id),
                CardNumber INTEGER NOT NULL,
                EleB1 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleB2 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleB3 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleB4 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleB5 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleI1 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleI2 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleI3 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleI4 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleI5 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleN1 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleN2 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleN3 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleN4 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleN5 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleG1 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleG2 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleG3 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleG4 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleG5 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleO1 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleO2 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleO3 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleO4 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleO5 INTEGER NOT NULL REFERENCES ElementsTable(Id)
            );",

            // ----- Cards 4x4 (referência agora em CardsSets) -----
            @"
            CREATE TABLE IF NOT EXISTS CardsList4Table (
                Id INTEGER PRIMARY KEY,
                SetId INTEGER NOT NULL REFERENCES CardsSets(SetId),
                ListId INTEGER NOT NULL REFERENCES ListsTable(Id),
                CardNumber INTEGER NOT NULL,
                Ele1  INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele2  INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele3  INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele4  INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele5  INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele6  INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele7  INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele8  INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele9  INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele10 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele11 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele12 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele13 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele14 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele15 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele16 INTEGER NOT NULL REFERENCES ElementsTable(Id)
            );"
        };

                foreach (var commandText in createTableCommands)
                {
                    using (var command = new SQLiteCommand(commandText, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }


        // Criação
        // Criar Elemento
        public static int CreateElement(string name, string cardName, string note1, string note2, string imageName, string addTime)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string insertQuery = @"
            INSERT INTO ElementsTable (Name, CardName, Note1, Note2, ImageName, AddTime)
            VALUES (@Name, @CardName, @Note1, @Note2, @ImageName, @AddTime);
            SELECT last_insert_rowid();";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@CardName", cardName);
                    command.Parameters.AddWithValue("@Note1", note1);
                    command.Parameters.AddWithValue("@Note2", note2);
                    command.Parameters.AddWithValue("@ImageName", imageName);
                    command.Parameters.AddWithValue("@AddTime", addTime);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        // Criar Lista
        public static int CreateList(string name, string description, string imagename)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string insertQuery = @"
            INSERT INTO ListsTable (Name, Description, ImageName)
            VALUES (@Name, @Description, @ImageName);
            SELECT last_insert_rowid();";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@ImageName", imagename);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        // Criar Conjunto de Cartelas
        public static int CreateCardsSet(CardSetModel cards)
        {
            if (cards == null) throw new ArgumentNullException(nameof(cards));
            if (cards.CardsSize != 4 && cards.CardsSize != 5)
                throw new ArgumentException("CardsSize deve ser 4 ou 5.", nameof(cards.CardsSize));

            string addTime = string.IsNullOrWhiteSpace(cards.AddDate)
                ? DateTime.Now.ToString("MMddyyyy - HH:mm:ss")
                : cards.AddDate;

            using var connection = GetConnection();
            connection.Open();

            if (cards.CardsSize == 5)
            {
                string sql = @"
            INSERT INTO CardsSets
                (ListId, Name, Title, End, Quantity, CardsSize,
                 GroupB, GroupI, GroupN, GroupG, GroupO,
                 AddTime, Theme, Header, Model)
            VALUES
                (@ListId, @Name, @Title, @End, @Quantity, @CardsSize,
                 @GroupB, @GroupI, @GroupN, @GroupG, @GroupO,
                 @AddTime, @Theme, @Header, @Model);
            SELECT last_insert_rowid();";

                using var cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@ListId", cards.ListId);
                cmd.Parameters.AddWithValue("@Name", cards.Name ?? "");
                cmd.Parameters.AddWithValue("@Title", cards.Title ?? "");
                cmd.Parameters.AddWithValue("@End", cards.End ?? "");
                cmd.Parameters.AddWithValue("@Quantity", cards.Quantity);
                cmd.Parameters.AddWithValue("@CardsSize", cards.CardsSize);

                string groupB = !string.IsNullOrWhiteSpace(cards.GroupBIds) ? cards.GroupBIds
                                 : string.Join(",", (cards.GroupB ?? Enumerable.Empty<ElementModel>()).Select(e => e.Id));
                string groupI = !string.IsNullOrWhiteSpace(cards.GroupIIds) ? cards.GroupIIds
                                 : string.Join(",", (cards.GroupI ?? Enumerable.Empty<ElementModel>()).Select(e => e.Id));
                string groupN = !string.IsNullOrWhiteSpace(cards.GroupNIds) ? cards.GroupNIds
                                 : string.Join(",", (cards.GroupN ?? Enumerable.Empty<ElementModel>()).Select(e => e.Id));
                string groupG = !string.IsNullOrWhiteSpace(cards.GroupGIds) ? cards.GroupGIds
                                 : string.Join(",", (cards.GroupG ?? Enumerable.Empty<ElementModel>()).Select(e => e.Id));
                string groupO = !string.IsNullOrWhiteSpace(cards.GroupOIds) ? cards.GroupOIds
                                 : string.Join(",", (cards.GroupO ?? Enumerable.Empty<ElementModel>()).Select(e => e.Id));

                cmd.Parameters.AddWithValue("@GroupB", groupB);
                cmd.Parameters.AddWithValue("@GroupI", groupI);
                cmd.Parameters.AddWithValue("@GroupN", groupN);
                cmd.Parameters.AddWithValue("@GroupG", groupG);
                cmd.Parameters.AddWithValue("@GroupO", groupO);

                cmd.Parameters.AddWithValue("@AddTime", addTime);
                cmd.Parameters.AddWithValue("@Theme", cards.Theme ?? "");
                cmd.Parameters.AddWithValue("@Header", cards.Header ?? "");
                cmd.Parameters.AddWithValue("@Model", cards.Model ?? "");

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            else // 4x4
            {
                string elementsStr;
                if (!string.IsNullOrWhiteSpace(cards.GroupBIds + cards.GroupIIds + cards.GroupNIds + cards.GroupGIds + cards.GroupOIds))
                {
                    var ids16 = string.Join(",", new[] { cards.GroupBIds, cards.GroupIIds, cards.GroupNIds, cards.GroupGIds, cards.GroupOIds }
                        .Where(s => !string.IsNullOrWhiteSpace(s)))
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim()).Where(s => s.Length > 0).Take(16);
                    elementsStr = string.Join(",", ids16);
                }
                else if (cards.AllElements != null && cards.AllElements.Count > 0)
                {
                    elementsStr = string.Join(",", cards.AllElements.Select(e => e.Id).Take(16));
                }
                else elementsStr = "";

                string sql = @"
            INSERT INTO CardsSets
                (ListId, Name, Title, End, Quantity, CardsSize, Elements,
                 AddTime, Theme, Header, Model)
            VALUES
                (@ListId, @Name, @Title, @End, @Quantity, @CardsSize, @Elements,
                 @AddTime, @Theme, @Header, @Model);
            SELECT last_insert_rowid();";

                using var cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@ListId", cards.ListId);
                cmd.Parameters.AddWithValue("@Name", cards.Name ?? "");
                cmd.Parameters.AddWithValue("@Title", cards.Title ?? "");
                cmd.Parameters.AddWithValue("@End", cards.End ?? "");
                cmd.Parameters.AddWithValue("@Quantity", cards.Quantity);
                cmd.Parameters.AddWithValue("@CardsSize", cards.CardsSize);
                cmd.Parameters.AddWithValue("@Elements", elementsStr);
                cmd.Parameters.AddWithValue("@AddTime", addTime);
                cmd.Parameters.AddWithValue("@Theme", cards.Theme ?? "");
                cmd.Parameters.AddWithValue("@Header", cards.Header ?? "");
                cmd.Parameters.AddWithValue("@Model", cards.Model ?? "");

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Criar Cartelas 5x5
        public static void CreateCard5(int listId, List<int> elementsIds, int cardNumber, int setId)
        {
            string query = @"INSERT INTO CardsList5Table 
                     (ListId, SetId, CardNumber, EleB1, EleB2, EleB3, EleB4, EleB5,
                      EleI1, EleI2, EleI3, EleI4, EleI5,
                      EleN1, EleN2, EleN3, EleN4, EleN5,
                      EleG1, EleG2, EleG3, EleG4, EleG5,
                      EleO1, EleO2, EleO3, EleO4, EleO5) 
                     VALUES 
                     (@ListId, @SetId, @CardNumber, @EleB1, @EleB2, @EleB3, @EleB4, @EleB5,
                      @EleI1, @EleI2, @EleI3, @EleI4, @EleI5,
                      @EleN1, @EleN2, @EleN3, @EleN4, @EleN5,
                      @EleG1, @EleG2, @EleG3, @EleG4, @EleG5,
                      @EleO1, @EleO2, @EleO3, @EleO4, @EleO5)";

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ListId", listId);
                    command.Parameters.AddWithValue("@SetId", setId);
                    command.Parameters.AddWithValue("@CardNumber", cardNumber);

                    for (int i = 0; i < 5; i++)
                    {
                        command.Parameters.AddWithValue($"@EleB{i + 1}", elementsIds[i]);
                        command.Parameters.AddWithValue($"@EleI{i + 1}", elementsIds[i + 5]);
                        command.Parameters.AddWithValue($"@EleN{i + 1}", elementsIds[i + 10]);
                        command.Parameters.AddWithValue($"@EleG{i + 1}", elementsIds[i + 15]);
                        command.Parameters.AddWithValue($"@EleO{i + 1}", elementsIds[i + 20]);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        // Criar Cartelas 4x4
        public static void CreateCard4(int listId, List<int> elementsIds, int cardNumber, int setId)
        {
            string query = @"INSERT INTO CardsList4Table 
                     (ListId, SetId, CardNumber, Ele1, Ele2, Ele3, Ele4, Ele5,
                      Ele6, Ele7, Ele8, Ele9, Ele10,
                      Ele11, Ele12, Ele13, Ele14, Ele15,
                      Ele16) 
                     VALUES 
                     (@ListId, @SetId, @CardNumber, @Ele1, @Ele2, @Ele3, @Ele4, @Ele5,
                      @Ele6, @Ele7, @Ele8, @Ele9, @Ele10,
                      @Ele11, @Ele12, @Ele13, @Ele14, @Ele15,
                      @Ele16)";

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ListId", listId);
                    command.Parameters.AddWithValue("@SetId", setId);
                    command.Parameters.AddWithValue("@CardNumber", cardNumber);

                    for (int i = 0; i <= 15; i++)
                    {
                        command.Parameters.AddWithValue($"@Ele{i + 1}", elementsIds[i]);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        // Inserir Elementos em uma Lista
        public static void AlocateElements(int listId, List<int> elementsIds)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                foreach (int elementId in elementsIds)
                {
                    string insertQuery = "INSERT INTO AlocationTable (ListId, ElementId) VALUES (@ListId, @ElementId)";

                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ListId", listId);
                        command.Parameters.AddWithValue("@ElementId", elementId);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }


        // Exportação
        // Método de Conexão
        public static void ExportGameDatabaseToPath(int setId, string outputPath)
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);

            SQLiteConnection.CreateFile(outputPath);
            string connStr = $"Data Source={outputPath};Version=3;";

            using var conn = new SQLiteConnection(connStr);
            conn.Open();

            CreateGameDB5(conn);
            ExportGame5(conn, setId);
        }

        // Criar Banco do Jogo com 5 Colunas
        public static void CreateGameDB5(SQLiteConnection conn)
        {
            var commands = new List<string>
            {
                @"CREATE TABLE ElementsTable (
                    Id INTEGER PRIMARY KEY,
                    Name TEXT,
                    CardName TEXT,
                    ImageName TEXT
                );",

                @"CREATE TABLE CardsSets (
                    SetId INTEGER PRIMARY KEY,
                    Title TEXT,
                    Qnt INTEGER,
                    GroupB TEXT,
                    GroupI TEXT,
                    GroupN TEXT,
                    GroupG TEXT,
                    GroupO TEXT
                );",

                @"CREATE TABLE CardsList (
                    Id INTEGER PRIMARY KEY,
                    CardNumber INTEGER,
                    SetId INTEGER,
                    EleB1 INTEGER, EleB2 INTEGER, EleB3 INTEGER, EleB4 INTEGER, EleB5 INTEGER,
                    EleI1 INTEGER, EleI2 INTEGER, EleI3 INTEGER, EleI4 INTEGER, EleI5 INTEGER,
                    EleN1 INTEGER, EleN2 INTEGER, EleN3 INTEGER, EleN4 INTEGER, EleN5 INTEGER,
                    EleG1 INTEGER, EleG2 INTEGER, EleG3 INTEGER, EleG4 INTEGER, EleG5 INTEGER,
                    EleO1 INTEGER, EleO2 INTEGER, EleO3 INTEGER, EleO4 INTEGER, EleO5 INTEGER
                );"
            };

            foreach (var cmdText in commands)
            {
                using var cmd = new SQLiteCommand(cmdText, conn);
                cmd.ExecuteNonQuery();
            }
        }

        // Selecionar informações do jogo com 5 Colunas
        public static DataRow GetCardSet5ById(int setId)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            string sql = "SELECT * FROM CardsSets5Table WHERE SetId = @SetId LIMIT 1";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@SetId", setId);
            using var adapter = new SQLiteDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public static List<DataRow> GetCards5BySetId(int setId)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            string sql = "SELECT * FROM CardsList5Table WHERE SetId = @SetId";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@SetId", setId);
            using var adapter = new SQLiteDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt.AsEnumerable().ToList();
        }

        // Preenche Bando do Jogo com 5 Colunas
        public static void ExportGame5(SQLiteConnection conn, int setId)
        {
            // 1. Lê dados do conjunto (CardsSets5Table)
            DataRow setRow = GetCardSet5ById(setId);
            if (setRow == null)
                throw new Exception($"Conjunto com SetId={setId} não encontrado.");

            string title = setRow["Title"].ToString();
            int quantity = Convert.ToInt32(setRow["Quantity"]);
            string groupB = setRow["GroupB"].ToString();
            string groupI = setRow["GroupI"].ToString();
            string groupN = setRow["GroupN"].ToString();
            string groupG = setRow["GroupG"].ToString();
            string groupO = setRow["GroupO"].ToString();

            // 2. Insere no CardsSets (com SetId = 0)
            string insertSet = @"
        INSERT INTO CardsSets (SetId, Title, Qnt, GroupB, GroupI, GroupN, GroupG, GroupO)
        VALUES (0, @Title, @Qnt, @GroupB, @GroupI, @GroupN, @GroupG, @GroupO);";

            using (var cmd = new SQLiteCommand(insertSet, conn))
            {
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Qnt", quantity);
                cmd.Parameters.AddWithValue("@GroupB", groupB);
                cmd.Parameters.AddWithValue("@GroupI", groupI);
                cmd.Parameters.AddWithValue("@GroupN", groupN);
                cmd.Parameters.AddWithValue("@GroupG", groupG);
                cmd.Parameters.AddWithValue("@GroupO", groupO);
                cmd.ExecuteNonQuery();
            }

            // 3. Coleta todos os elementos únicos
            var allIds = groupB.Split(',').Concat(groupI.Split(','))
                               .Concat(groupN.Split(',')).Concat(groupG.Split(',')).Concat(groupO.Split(','))
                               .Select(id => int.Parse(id)).Distinct().ToList();

            var allElements = GetElementsByIds(allIds);

            foreach (var e in allElements)
            {
                string insertElement = @"
            INSERT INTO ElementsTable (Id, Name, CardName, ImageName)
            VALUES (@Id, @Name, @CardName, @ImageName);";

                using var cmd = new SQLiteCommand(insertElement, conn);
                cmd.Parameters.AddWithValue("@Id", Convert.ToInt32(e["Id"]));
                cmd.Parameters.AddWithValue("@Name", e["Name"].ToString());
                cmd.Parameters.AddWithValue("@CardName", e["CardName"].ToString());
                cmd.Parameters.AddWithValue("@ImageName", Path.GetFileName(e["ImageName"].ToString()));
                cmd.ExecuteNonQuery();
            }

            // 4. Exporta as cartelas
            var cards = GetCards5BySetId(setId);
            foreach (var card in cards)
            {
                string insertCard = @"
            INSERT INTO CardsList (
                Id, CardNumber, SetId,
                EleB1, EleB2, EleB3, EleB4, EleB5,
                EleI1, EleI2, EleI3, EleI4, EleI5,
                EleN1, EleN2, EleN3, EleN4, EleN5,
                EleG1, EleG2, EleG3, EleG4, EleG5,
                EleO1, EleO2, EleO3, EleO4, EleO5)
            VALUES (
                @Id, @CardNumber, 0,
                @B1, @B2, @B3, @B4, @B5,
                @I1, @I2, @I3, @I4, @I5,
                @N1, @N2, @N3, @N4, @N5,
                @G1, @G2, @G3, @G4, @G5,
                @O1, @O2, @O3, @O4, @O5);";

                using var cmd = new SQLiteCommand(insertCard, conn);
                cmd.Parameters.AddWithValue("@Id", Convert.ToInt32(card["Id"]));
                cmd.Parameters.AddWithValue("@CardNumber", Convert.ToInt32(card["CardNumber"]));
                for (int i = 1; i <= 5; i++)
                {
                    cmd.Parameters.AddWithValue($"@B{i}", Convert.ToInt32(card[$"EleB{i}"]));
                    cmd.Parameters.AddWithValue($"@I{i}", Convert.ToInt32(card[$"EleI{i}"]));
                    cmd.Parameters.AddWithValue($"@N{i}", Convert.ToInt32(card[$"EleN{i}"]));
                    cmd.Parameters.AddWithValue($"@G{i}", Convert.ToInt32(card[$"EleG{i}"]));
                    cmd.Parameters.AddWithValue($"@O{i}", Convert.ToInt32(card[$"EleO{i}"]));
                }
                cmd.ExecuteNonQuery();
            }
        }

        // Criar Banco do Jogo com 4 Colunas
        public static void CreateGameDB4(int setId)
        {

        }


        // Métodos de Busca
        // Encontrar Elemento pelo ID
        public static DataRow GetElementById(int elementId)
        {
            using (var connection = GetConnection())
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

        // Retornar todos os Elementos em uma Lista
        public static List<DataRow> GetElementsByIds(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return new List<DataRow>();

            string idList = string.Join(",", ids);
            string sql = $"SELECT Id, Name, CardName, ImageName FROM ElementsTable WHERE Id IN ({idList})";

            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            using var adapter = new SQLiteDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt.AsEnumerable().ToList();
        }

        // Método para retornar o ID de um Elemento pelo CardName
        public static int GetElementByCardName(string cardName)
        {
            using var connection = GetConnection();
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

        // Conferir se um Elemento já existe em uma Lista
        public static bool ElementExist(string cardName, string listName)
        {
            // Conferir se já existe um elemento em uma lista com o mesmo cardName
            throw new NotImplementedException();
        }

        // Método para retornar todas as Listas
        public static DataTable GetLists()
        {
            using (var connection = GetConnection())
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

            using (var connection = GetConnection())
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

            using (var connection = GetConnection())
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

            using var connection = GetConnection();
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

            static List<ElementModel> LoadElements(List<int> ids)
            {
                if (ids.Count == 0) return new List<ElementModel>();
                var rows = GetElementsByIds(ids); // já existe no seu DataService
                return rows.Select(r => new ElementModel
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Name = r["Name"]?.ToString() ?? "",
                    CardName = r["CardName"]?.ToString() ?? "",
                    // Se precisar: ImageName = r.Table.Columns.Contains("ImageName") ? r["ImageName"]?.ToString() ?? "" : ""
                }).ToList();
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
                var elementsCsv = row["Elements"]?.ToString() ?? "";
                var ids = ParseIds(elementsCsv);
                model.AllElements = LoadElements(ids);

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
                model.AllElements ??= new List<ElementModel>();
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

            using var conn = GetConnection();
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


        // Métodos de Edição
        // Método para atualizar um Elemento existente
        public static void UpdateElement(int id, string name, string cardName, string note1, string note2, string imageName, string addTime)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string updateQuery = @"
                    UPDATE ElementsTable 
                    SET Name = @Name, CardName = @CardName, Note1 = @Note1, Note2 = @Note2, ImageName = @ImageName, AddTime = @AddTime 
                    WHERE Id = @Id;";

                using (var command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@CardName", cardName);
                    command.Parameters.AddWithValue("@Note1", note1);
                    command.Parameters.AddWithValue("@Note2", note2);
                    command.Parameters.AddWithValue("@ImageName", imageName);
                    command.Parameters.AddWithValue("@AddTime", addTime);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Método para remover Elementos de uma Lista
        public static void UnalocateElements(int listId, List<string> elementsIds)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                foreach (string elementId in elementsIds)
                {
                    string deleteQuery = "DELETE FROM AlocationTable WHERE ListId = @ListId AND ElementId = @ElementId";

                    using (var command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ListId", listId);
                        command.Parameters.AddWithValue("@ElementId", elementId);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
