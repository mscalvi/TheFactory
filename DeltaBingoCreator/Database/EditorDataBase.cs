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
    class EditorDataBase
    {

        // Métodos de Edição

        // Apaga TODAS as cartelas (4x4 e 5x5) de um CardSet
        public static int DeleteCardsBySetId(int setId)
        {
            using var conn = MainDataBase.GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            int total = 0;

            using (var cmd = new SQLiteCommand("DELETE FROM CardsList5Table WHERE SetId = @SetId;", conn, tx))
            {
                cmd.Parameters.AddWithValue("@SetId", setId);
                total += cmd.ExecuteNonQuery();
            }

            using (var cmd = new SQLiteCommand("DELETE FROM CardsList4Table WHERE SetId = @SetId;", conn, tx))
            {
                cmd.Parameters.AddWithValue("@SetId", setId);
                total += cmd.ExecuteNonQuery();
            }

            tx.Commit();
            return total; // total de linhas removidas das tabelas de cartelas
        }

        // Apaga o CardSet e todas as suas cartelas
        public static bool DeleteCardSet(int setId)
        {
            using var conn = MainDataBase.GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            // 1) Apaga cartelas relacionadas
            using (var cmd = new SQLiteCommand("DELETE FROM CardsList5Table WHERE SetId = @SetId;", conn, tx))
            {
                cmd.Parameters.AddWithValue("@SetId", setId);
                cmd.ExecuteNonQuery();
            }
            using (var cmd = new SQLiteCommand("DELETE FROM CardsList4Table WHERE SetId = @SetId;", conn, tx))
            {
                cmd.Parameters.AddWithValue("@SetId", setId);
                cmd.ExecuteNonQuery();
            }

            // 2) Apaga o CardSet em si
            int affected;
            using (var cmd = new SQLiteCommand("DELETE FROM CardsSets WHERE SetId = @SetId;", conn, tx))
            {
                cmd.Parameters.AddWithValue("@SetId", setId);
                affected = cmd.ExecuteNonQuery();
            }

            tx.Commit();
            return affected > 0; // true se o CardSet existia e foi apagado
        }

        // Apaga uma Lista e todas suas Alocações
        public static bool DeleteListAndAllocations(int listId)
        {
            using var conn = MainDataBase.GetConnection();
            conn.Open();
            int affected = 0;

            using var tx = conn.BeginTransaction();

            // 1) Checa se a lista está sendo usada por algum conjunto
            const string checkSql = @"SELECT 1 FROM CardsSets WHERE ListId = @ListId LIMIT 1;";
            using (var checkCmd = new SQLiteCommand(checkSql, conn, tx))
            {
                checkCmd.Parameters.AddWithValue("@ListId", listId);
                var inUse = checkCmd.ExecuteScalar();
                if (inUse != null) // há pelo menos um registro
                {
                    tx.Rollback();
                    throw new InvalidOperationException("Esta lista está em uso por um ou mais conjuntos e não pode ser excluída.");
                }
            }

            // 2) Remove alocações (ElementId <-> ListId)
            const string delAllocSql = @"DELETE FROM AlocationTable WHERE ListId = @ListId;";
            using (var delAlloc = new SQLiteCommand(delAllocSql, conn, tx))
            {
                delAlloc.Parameters.AddWithValue("@ListId", listId);
                delAlloc.ExecuteNonQuery();
            }

            // 3) Remove a própria lista
            const string delListSql = @"DELETE FROM ListsTable WHERE Id = @ListId;";
            using (var delList = new SQLiteCommand(delListSql, conn, tx))
            {
                delList.Parameters.AddWithValue("@ListId", listId);
                affected = delList.ExecuteNonQuery();
                if (affected == 0)
                {
                    tx.Rollback();
                    throw new InvalidOperationException("Lista não encontrada para exclusão.");
                }
            }

            tx.Commit();
            return affected > 0;
        }

        // Apaga um Elemento
        public static bool DeleteElement(int elementId)
        {
            using var conn = MainDataBase.GetConnection();
            conn.Open();

            // 1) Bloqueia exclusão se o elemento estiver alocado em alguma lista
            const string sqlAlloc = "SELECT 1 FROM AlocationTable WHERE ElementId = @Id LIMIT 1;";
            using (var cmd = new SQLiteCommand(sqlAlloc, conn))
            {
                cmd.Parameters.AddWithValue("@Id", elementId);
                var hasAlloc = cmd.ExecuteScalar();
                if (hasAlloc != null)
                    return false; // ainda está em alguma lista
            }

            // 2) Bloqueia exclusão se o elemento aparece em alguma cartela 5x5
            //    (qualquer uma das 25 colunas EleB*,EleI*,EleN*,EleG*,EleO*)
            var where5 = new StringBuilder();
            string[] colsB = Enumerable.Range(1, 5).Select(i => $"EleB{i}").ToArray();
            string[] colsI = Enumerable.Range(1, 5).Select(i => $"EleI{i}").ToArray();
            string[] colsN = Enumerable.Range(1, 5).Select(i => $"EleN{i}").ToArray();
            string[] colsG = Enumerable.Range(1, 5).Select(i => $"EleG{i}").ToArray();
            string[] colsO = Enumerable.Range(1, 5).Select(i => $"EleO{i}").ToArray();
            string[] all5 = colsB.Concat(colsI).Concat(colsN).Concat(colsG).Concat(colsO).ToArray();

            for (int i = 0; i < all5.Length; i++)
            {
                if (i > 0) where5.Append(" OR ");
                where5.Append(all5[i]).Append(" = @Id");
            }

            string sql5 = $"SELECT 1 FROM CardsList5Table WHERE {where5} LIMIT 1;";
            using (var cmd = new SQLiteCommand(sql5, conn))
            {
                cmd.Parameters.AddWithValue("@Id", elementId);
                var inCards5 = cmd.ExecuteScalar();
                if (inCards5 != null)
                    return false; // ainda está em alguma cartela 5x5
            }

            // 3) Bloqueia exclusão se o elemento aparece em alguma cartela 4x4 (Ele1..Ele16)
            var where4 = new StringBuilder();
            string[] all4 = Enumerable.Range(1, 16).Select(i => $"Ele{i}").ToArray();
            for (int i = 0; i < all4.Length; i++)
            {
                if (i > 0) where4.Append(" OR ");
                where4.Append(all4[i]).Append(" = @Id");
            }

            string sql4 = $"SELECT 1 FROM CardsList4Table WHERE {where4} LIMIT 1;";
            using (var cmd = new SQLiteCommand(sql4, conn))
            {
                cmd.Parameters.AddWithValue("@Id", elementId);
                var inCards4 = cmd.ExecuteScalar();
                if (inCards4 != null)
                    return false; // ainda está em alguma cartela 4x4
            }

            // 4) Pode excluir
            const string sqlDel = "DELETE FROM ElementsTable WHERE Id = @Id;";
            using (var cmd = new SQLiteCommand(sqlDel, conn))
            {
                cmd.Parameters.AddWithValue("@Id", elementId);
                int affected = cmd.ExecuteNonQuery();
                return affected > 0;
            }
        }

        // Apaga todos os Elementos de uma Lista
        public static (int unallocated, int deleted) DeleteElementsInList(int listId, bool deleteOrphanElements = false)
        {
            using var conn = MainDataBase.GetConnection();
            conn.Open();

            // 1) Coletar IDs de elementos alocados nessa lista
            var elementIds = new List<int>();
            const string selSql = @"SELECT ElementId FROM AlocationTable WHERE ListId = @ListId;";
            using (var selCmd = new SQLiteCommand(selSql, conn))
            {
                selCmd.Parameters.AddWithValue("@ListId", listId);
                using var rd = selCmd.ExecuteReader();
                while (rd.Read())
                {
                    if (rd["ElementId"] != DBNull.Value)
                        elementIds.Add(Convert.ToInt32(rd["ElementId"]));
                }
            }

            if (elementIds.Count == 0)
                return (0, 0); // nada para remover

            // 2) Remover TODAS as alocações desta lista em transação
            int unallocated = 0;
            using (var tx = conn.BeginTransaction())
            {
                const string delAllocSql = @"DELETE FROM AlocationTable WHERE ListId = @ListId;";
                using var delCmd = new SQLiteCommand(delAllocSql, conn, tx);
                delCmd.Parameters.AddWithValue("@ListId", listId);
                unallocated = delCmd.ExecuteNonQuery();
                tx.Commit();
            }

            // 3) Opcional: tentar excluir elementos órfãos (sem refs em outras listas/cartelas)
            int deleted = 0;
            if (deleteOrphanElements)
            {
                foreach (var eid in elementIds.Distinct())
                {
                    // Se ainda restar alocação em outra lista, nem tenta.
                    const string stillAllocSql = @"SELECT 1 FROM AlocationTable WHERE ElementId = @Id LIMIT 1;";
                    using (var checkCmd = new SQLiteCommand(stillAllocSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Id", eid);
                        var hasAlloc = checkCmd.ExecuteScalar();
                        if (hasAlloc != null)
                            continue; // ainda alocado em outra lista
                    }

                    // Usa o DeleteElement que você já tem (bloqueia se estiver em cartelas)
                    if (DeleteElement(eid))
                        deleted++;
                }
            }

            return (unallocated, deleted);
        }

        // Edita um Elemento e salva versão anterior
        public static int EditElement(int oldElementId, ItemModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            using var conn = MainDataBase.GetConnection();
            conn.Open();

            // carrega o antigo
            const string sel = @"
        SELECT Id, Name, CardName, Note1, Note2, ImageName,
               COALESCE(ParentId,0) AS ParentId,
               COALESCE(Version,1)  AS Version
        FROM ElementsTable
        WHERE Id=@Id
        LIMIT 1;";
            DataRow old;
            using (var cmd = new SQLiteCommand(sel, conn))
            {
                cmd.Parameters.AddWithValue("@Id", oldElementId);
                using var adp = new SQLiteDataAdapter(cmd);
                var dt = new DataTable(); adp.Fill(dt);
                if (dt.Rows.Count == 0)
                    throw new InvalidOperationException($"Elemento {oldElementId} não encontrado.");
                old = dt.Rows[0];
            }

            int oldId = Convert.ToInt32(old["Id"]);
            int oldParent = Convert.ToInt32(old["ParentId"]);
            int oldVersion = Convert.ToInt32(old["Version"]);
            int newParent = oldParent > 0 ? oldParent : oldId;
            int newVersion = (oldVersion >= 1 ? oldVersion + 1 : 2);
            string now = DateTime.Now.ToString("MMddyyyy - HH:mm:ss");

            using var tx = conn.BeginTransaction();

            // obsoleta antigo
            using (var upd = new SQLiteCommand("UPDATE ElementsTable SET Obsolete=1 WHERE Id=@Id;", conn, tx))
            {
                upd.Parameters.AddWithValue("@Id", oldId);
                upd.ExecuteNonQuery();
            }

            // cria o novo (usa valores do model; fallback pros antigos se vierem nulos)
            const string ins = @"
        INSERT INTO ElementsTable
            (Name, CardName, Note1, Note2, ImageName, AddTime, Obsolete, ParentId, Version)
        VALUES
            (@Name, @CardName, @Note1, @Note2, @ImageName, @AddTime, 0, @ParentId, @Version);
        SELECT last_insert_rowid();";
            using (var cmd = new SQLiteCommand(ins, conn, tx))
            {
                string name = model.Name ?? old["Name"]?.ToString() ?? "";
                string cardName = model.CardName ?? old["CardName"]?.ToString() ?? "";
                string note1 = model.Note1 ?? old["Note1"]?.ToString() ?? "";
                string note2 = model.Note2 ?? old["Note2"]?.ToString() ?? "";
                string imageName = model.ImageName ?? old["ImageName"]?.ToString() ?? "";

                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@CardName", cardName);
                cmd.Parameters.AddWithValue("@Note1", note1);
                cmd.Parameters.AddWithValue("@Note2", note2);
                cmd.Parameters.AddWithValue("@ImageName", imageName);
                cmd.Parameters.AddWithValue("@AddTime", now);
                cmd.Parameters.AddWithValue("@ParentId", newParent);
                cmd.Parameters.AddWithValue("@Version", newVersion);

                int newId = Convert.ToInt32(cmd.ExecuteScalar());
                tx.Commit();
                return newId;
            }
        }

        // Edita um Elemento em todas as Listas
        public static int EditElementInList(int oldElementId, int newElementId)
        {
            using var conn = MainDataBase.GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            // pega todas as listas onde o elemento antigo está alocado
            var listIds = new List<int>();
            using (var cmd = new SQLiteCommand("SELECT ListId FROM AlocationTable WHERE ElementId=@E;", conn, tx))
            {
                cmd.Parameters.AddWithValue("@E", oldElementId);
                using var rd = cmd.ExecuteReader();
                while (rd.Read()) listIds.Add(Convert.ToInt32(rd["ListId"]));
            }

            int affected = 0;
            foreach (var listId in listIds)
            {
                // insere novo par se não existir
                using (var ins = new SQLiteCommand(
                    "INSERT OR IGNORE INTO AlocationTable (ElementId, ListId) VALUES (@NewE, @L);", conn, tx))
                {
                    ins.Parameters.AddWithValue("@NewE", newElementId);
                    ins.Parameters.AddWithValue("@L", listId);
                    ins.ExecuteNonQuery();
                }
                // remove o antigo
                using (var del = new SQLiteCommand(
                    "DELETE FROM AlocationTable WHERE ElementId=@OldE AND ListId=@L;", conn, tx))
                {
                    del.Parameters.AddWithValue("@OldE", oldElementId);
                    del.Parameters.AddWithValue("@L", listId);
                    affected += del.ExecuteNonQuery();
                }
            }

            tx.Commit();
            return affected; // número de remoções realizadas (aprox. listas afetadas)
        }

        // Edita um Elemento em todas as Cartelas
        public static int EditElementInCardSet(int oldElementId, int newElementId)
        {
            if (oldElementId <= 0 || newElementId <= 0 || oldElementId == newElementId)
                return 0;

            using var conn = MainDataBase.GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            int total = 0;

            // --- 5x5: atualiza todas as 25 colunas possíveis ---
            string[] cols5 =
            {
        "EleB1","EleB2","EleB3","EleB4","EleB5",
        "EleI1","EleI2","EleI3","EleI4","EleI5",
        "EleN1","EleN2","EleN3","EleN4","EleN5",
        "EleG1","EleG2","EleG3","EleG4","EleG5",
        "EleO1","EleO2","EleO3","EleO4","EleO5"
    };

            foreach (var col in cols5)
            {
                string sql = $"UPDATE CardsList5Table SET {col} = @NewId WHERE {col} = @OldId;";
                using var cmd = new SQLiteCommand(sql, conn, tx);
                cmd.Parameters.AddWithValue("@NewId", newElementId);
                cmd.Parameters.AddWithValue("@OldId", oldElementId);
                total += cmd.ExecuteNonQuery();
            }

            // --- 4x4: Ele1..Ele16 ---
            for (int i = 1; i <= 16; i++)
            {
                string col = $"Ele{i}";
                string sql = $"UPDATE CardsList4Table SET {col} = @NewId WHERE {col} = @OldId;";
                using var cmd = new SQLiteCommand(sql, conn, tx);
                cmd.Parameters.AddWithValue("@NewId", newElementId);
                cmd.Parameters.AddWithValue("@OldId", oldElementId);
                total += cmd.ExecuteNonQuery();
            }

            tx.Commit();
            return total;
        }

        // Edita uma Lista (usa ListModel: Id, Name, Description, ImageName)
        public static bool EditList(ListModel list)
        {
            if (list == null || list.Id <= 0) return false;

            using var connection = MainDataBase.GetConnection();
            connection.Open();

            const string sql = @"
        UPDATE ListsTable
        SET Name       = @Name,
            Description= @Description,
            ImageName  = @ImageName
        WHERE Id = @Id;";

            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Id", list.Id);
            cmd.Parameters.AddWithValue("@Name", (list.Name ?? string.Empty).Trim());
            cmd.Parameters.AddWithValue("@Description", (list.Description ?? string.Empty).Trim());
            cmd.Parameters.AddWithValue("@ImageName", (list.ImageName ?? string.Empty).Trim());

            int affected = cmd.ExecuteNonQuery();
            return affected > 0;
        }

        // Edita um Conjunto (usa CardSetModel: Id, Name, Title, End)
        public static bool EditCardSet(CardSetModel set)
        {
            if (set == null || set.Id <= 0) return false;

            using var connection = MainDataBase.GetConnection();
            connection.Open();

            const string sql = @"
        UPDATE CardsSets
        SET Name  = @Name,
            Title = @Title,
            End   = @End
        WHERE SetId = @SetId;";

            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@SetId", set.Id);
            cmd.Parameters.AddWithValue("@Name", (set.Name ?? string.Empty).Trim());
            cmd.Parameters.AddWithValue("@Title", (set.Title ?? string.Empty).Trim());
            cmd.Parameters.AddWithValue("@End", (set.End ?? string.Empty).Trim());

            int affected = cmd.ExecuteNonQuery();
            return affected > 0;
        }

        // Helper para trocar Elemento das Cartelas
        private static string ReplaceIdInCsv(string csv, int oldId, int newId, out bool changed)
        {
            changed = false;
            if (string.IsNullOrWhiteSpace(csv)) return string.Empty;

            var outList = new List<string>();
            foreach (var tok in csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var t = tok.Trim();
                if (t.Length == 0) continue;
                if (int.TryParse(t, out int v) && v == oldId)
                {
                    outList.Add(newId.ToString());
                    changed = true;
                }
                else
                {
                    outList.Add(t);
                }
            }
            return string.Join(",", outList);
        }

        // Atualiza a quantidade declarada no conjunto
        public static void UpdateCardSetQuantity(int setId, int newQuantity)
        {
            using var connection = MainDataBase.GetConnection();
            connection.Open();
            using var cmd = new SQLiteCommand(
                "UPDATE CardsSets SET Quantity=@Qnt WHERE SetId=@SetId;", connection);
            cmd.Parameters.AddWithValue("@Qnt", newQuantity);
            cmd.Parameters.AddWithValue("@SetId", setId);
            cmd.ExecuteNonQuery();
        }

        // Helpers para Retornar ElementModel
        public static ItemModel? GetElementModelById(int elementId)
        {
            var row = FinderDataBase.GetElementById(elementId);
            if (row == null) return null;
            return MapElementRowToModel(row);
        }
        private static ItemModel MapElementRowToModel(DataRow r)
        {
            string Get(string col) =>
                (r.Table?.Columns.Contains(col) == true && r[col] != DBNull.Value) ? r[col]!.ToString()! : string.Empty;

            int GetInt(string col, int def = 0)
            {
                if (r.Table?.Columns.Contains(col) == true && r[col] != DBNull.Value)
                    return Convert.ToInt32(r[col]);
                return def;
            }

            return new ItemModel
            {
                Id = GetInt("Id"),
                Name = Get("Name"),
                CardName = Get("CardName"),
                Note1 = Get("Note1"),
                Note2 = Get("Note2"),
                ImageName = Get("ImageName"),
                AddTime = Get("AddTime"),
                Obsolete = GetInt("Obsolete", 0) != 0,
                ParentId = GetInt("ParentId", 0),
                Version = GetInt("Version", 1),
            };
        }
    }
}
