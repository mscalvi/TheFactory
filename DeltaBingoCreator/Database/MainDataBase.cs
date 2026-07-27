using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;

namespace DeltaBingoCreator.Database
{
    class MainDataBase
    {
        private static readonly string _connectionString;

        // Conexão
        // Método principal de conexão
        static MainDataBase()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string databaseFolder = Path.Combine(baseDir, "Database");

                Directory.CreateDirectory(databaseFolder);

                string databasePath = Path.Combine(databaseFolder, "DeltaBingo.db");
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
        public static SQLiteConnection GetConnection()
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
                Obsolete INTEGER NOT NULL DEFAULT 0,
                ParentId INTEGER NULL,
                Version INTEGER NOT NULL DEFAULT 1,
                AddTime TEXT NOT NULL
            );",

            // ----- Lists -----
            @"
            CREATE TABLE IF NOT EXISTS ListsTable (
                Id INTEGER PRIMARY KEY,
                Name TEXT,
                Description TEXT,
                Obsolete INTEGER NOT NULL DEFAULT 0, 
                ParentId INTEGER NULL, 
                Version INTEGER NOT NULL DEFAULT 1,
                ImageName TEXT
            );",

            // ----- Alocation (Element <-> List) -----
            @"
            CREATE TABLE IF NOT EXISTS AlocationTable (
                ElementId INTEGER REFERENCES ElementsTable(Id),
                ListId INTEGER  REFERENCES ListsTable(Id),
                Obsolete INTEGER,
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
                ImageName TEXT,
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
                Model     TEXT,
                Obsolete INTEGER NOT NULL DEFAULT 0, 
                ParentId INTEGER NULL, 
                Version INTEGER NOT NULL DEFAULT 1
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
    }
}
