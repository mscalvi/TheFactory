using DeltaBingoCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;

namespace DeltaBingoCreator.Database
{
    class CreatorDataBase
    {
        // Criação
        // Criar Elemento
        public static int CreateElement(string name, string cardName, string note1, string note2, string imageName, string addTime)
        {
            using (var connection = MainDataBase.GetConnection())
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
            using (var connection = MainDataBase.GetConnection())
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

            using var connection = MainDataBase.GetConnection();
            connection.Open();

            if (cards.CardsSize == 5)
            {
                string sql = @"
            INSERT INTO CardsSets
                (ListId, Name, Title, End, Quantity, CardsSize, ImageName,
                 GroupB, GroupI, GroupN, GroupG, GroupO, Elements,
                 AddTime, Theme, Header, Model)
            VALUES
                (@ListId, @Name, @Title, @End, @Quantity, @CardsSize, @ImageName,
                 @GroupB, @GroupI, @GroupN, @GroupG, @GroupO, @Elements,
                 @AddTime, @Theme, @Header, @Model);
            SELECT last_insert_rowid();";

                using var cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@ListId", cards.ListId);
                cmd.Parameters.AddWithValue("@Name", cards.Name ?? "");
                cmd.Parameters.AddWithValue("@Title", cards.Title ?? "");
                cmd.Parameters.AddWithValue("@End", cards.End ?? "");
                cmd.Parameters.AddWithValue("@Quantity", cards.Quantity);
                cmd.Parameters.AddWithValue("@CardsSize", cards.CardsSize);
                cmd.Parameters.AddWithValue("@ImageName", cards.ImageName ?? "");

                string groupB = !string.IsNullOrWhiteSpace(cards.GroupBIds) ? cards.GroupBIds
                                 : string.Join(",", (cards.GroupB ?? Enumerable.Empty<ItemModel>()).Select(e => e.Id));
                string groupI = !string.IsNullOrWhiteSpace(cards.GroupIIds) ? cards.GroupIIds
                                 : string.Join(",", (cards.GroupI ?? Enumerable.Empty<ItemModel>()).Select(e => e.Id));
                string groupN = !string.IsNullOrWhiteSpace(cards.GroupNIds) ? cards.GroupNIds
                                 : string.Join(",", (cards.GroupN ?? Enumerable.Empty<ItemModel>()).Select(e => e.Id));
                string groupG = !string.IsNullOrWhiteSpace(cards.GroupGIds) ? cards.GroupGIds
                                 : string.Join(",", (cards.GroupG ?? Enumerable.Empty<ItemModel>()).Select(e => e.Id));
                string groupO = !string.IsNullOrWhiteSpace(cards.GroupOIds) ? cards.GroupOIds
                                 : string.Join(",", (cards.GroupO ?? Enumerable.Empty<ItemModel>()).Select(e => e.Id));

                string ElementsCsv()
                {
                    var seen = new HashSet<string>();
                    var order = new[] { groupB, groupI, groupN, groupG, groupO };
                    var list = new List<string>();

                    foreach (var part in order)
                    {
                        if (string.IsNullOrWhiteSpace(part)) continue;
                        foreach (var tok in part.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var id = tok.Trim();
                            if (id.Length == 0) continue;
                            if (seen.Add(id)) list.Add(id);
                        }
                    }
                    return string.Join(",", list);
                }
                string elements = ElementsCsv();

                cmd.Parameters.AddWithValue("@GroupB", groupB);
                cmd.Parameters.AddWithValue("@GroupI", groupI);
                cmd.Parameters.AddWithValue("@GroupN", groupN);
                cmd.Parameters.AddWithValue("@GroupG", groupG);
                cmd.Parameters.AddWithValue("@GroupO", groupO);
                cmd.Parameters.AddWithValue("@Elements", elements);
                cmd.Parameters.AddWithValue("@AddTime", addTime);
                cmd.Parameters.AddWithValue("@Theme", cards.Theme ?? "");
                cmd.Parameters.AddWithValue("@Header", cards.Header ?? "");
                cmd.Parameters.AddWithValue("@Model", cards.Model ?? "");

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            else // 4x4
            {
                // Fonte primária: o pool completo vindo no modelo
                // (cards.AllElements deve conter TODOS os elementos candidatos do set 4x4)
                IEnumerable<int> idsSource;

                if (cards.AllElements != null && cards.AllElements.Count > 0)
                {
                    idsSource = cards.AllElements.Select(e => e.Id);
                }
                else
                {
                    // Fallback: carrega TODOS os elementos da lista associada (não apenas 16)
                    idsSource = FinderDataBase.GetElementsInList(cards.ListId)
                        .Select(r => Convert.ToInt32(r["Id"]));
                }

                // Limpa, remove duplicados — **sem** limitar a 16
                var idsAll = idsSource
                    .Where(id => id > 0)
                    .Distinct()
                    .ToList();

                if (idsAll.Count < 16)
                    throw new InvalidOperationException("Para 4x4, é necessário pelo menos 16 elementos no pool.");

                string elementsStr = string.Join(",", idsAll);

                string sql = @"
        INSERT INTO CardsSets
            (ListId, Name, Title, End, Quantity, CardsSize, Elements, ImageName,
             AddTime, Theme, Header, Model)
        VALUES
            (@ListId, @Name, @Title, @End, @Quantity, @CardsSize, @Elements, @ImageName,
             @AddTime, @Theme, @Header, @Model);
        SELECT last_insert_rowid();";

                using var cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@ListId", cards.ListId);
                cmd.Parameters.AddWithValue("@Name", cards.Name ?? "");
                cmd.Parameters.AddWithValue("@Title", cards.Title ?? "");
                cmd.Parameters.AddWithValue("@End", cards.End ?? "");
                cmd.Parameters.AddWithValue("@Quantity", cards.Quantity);
                cmd.Parameters.AddWithValue("@CardsSize", cards.CardsSize);
                cmd.Parameters.AddWithValue("@ImageName", cards.ImageName ?? "");
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

            using (var connection = MainDataBase.GetConnection())
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

            using (var connection = MainDataBase.GetConnection())
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

        // Alocar Elementos em Lista
        public static void AlocateElements(int listId, List<int> elementsIds)
        {
            using (var connection = MainDataBase.GetConnection())
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
    }
}
