using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.ExtendedProperties;
using GeradorCartas___Guildas.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace GeradorCartas___Guildas.Services
{
    internal class ImportingService
    {
        // Creation
        private readonly List<MapModel> _maps = new();
        public IReadOnlyList<MapModel> Maps => _maps;

        private readonly List<CharacterModel> _characters = new();
        public IReadOnlyList<CharacterModel> Characters => _characters;

        // Import Maps
        public List<MapModel> ImportMapsList(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Caminho do arquivo não informado.", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Arquivo não encontrado.", filePath);

            using var wb = new XLWorkbook(filePath);
            var ws = wb.Worksheets.First(); // única planilha

            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
            var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;

            // Cabeçalhos exatos (linha 1)
            var headerIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int c = 1; c <= lastCol; c++)
            {
                var h = ws.Cell(1, c).GetString().Trim();
                if (!string.IsNullOrEmpty(h) && !headerIndex.ContainsKey(h))
                    headerIndex[h] = c;
            }

            string[] required = { "Id", "Exp", "Type", "Name", "Description", "Option1", "Option2", "Lore", "Credits", "Info", "Edition" };
            foreach (var req in required)
            {
                if (!headerIndex.ContainsKey(req))
                    throw new InvalidDataException($"Cabeçalho obrigatório ausente: '{req}'.");
            }

            _maps.Clear();

            for (int r = 2; r <= lastRow; r++)
            {
                // PARADA: se Id estiver vazio, fim da tabela
                string id = Read(ws, r, headerIndex["Id"]);
                if (string.IsNullOrWhiteSpace(id))
                    break;

                string exp = Read(ws, r, headerIndex["Exp"]);
                string type = Read(ws, r, headerIndex["Type"]);
                string name = Read(ws, r, headerIndex["Name"]);
                string desc = Read(ws, r, headerIndex["Description"]);
                string op1 = Read(ws, r, headerIndex["Option1"]);
                string op2 = Read(ws, r, headerIndex["Option2"]);
                string lore = Read(ws, r, headerIndex["Lore"]);
                string cred = Read(ws, r, headerIndex["Credits"]);
                string info = Read(ws, r, headerIndex["Info"]);
                string edit = Read(ws, r, headerIndex["Edition"]);

                _maps.Add(new MapModel
                {
                    Id = id,
                    Exp = exp,
                    Type = type,
                    Name = name,
                    Description = desc,
                    Option1 = op1,
                    Option2 = op2,
                    Art = id,     // mantém sua adaptação
                    Lore = lore,
                    Credits = cred,
                    Info = info,
                    Edition = edit
                });
            }

            return new List<MapModel>(_maps);
        }

        // Import Characters
        public List<CharacterModel> ImportCharactersList(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Caminho do arquivo não informado.", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Arquivo não encontrado.", filePath);

            using var wb = new XLWorkbook(filePath);
            var ws = wb.Worksheets.First(); // uma única planilha

            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
            var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;

            // Mapear cabeçalhos exatos na linha 1 (case-insensitive)
            var headerIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int c = 1; c <= lastCol; c++)
            {
                var h = ws.Cell(1, c).GetString().Trim();
                if (!string.IsNullOrEmpty(h) && !headerIndex.ContainsKey(h))
                    headerIndex[h] = c;
            }

            // Cabeçalhos esperados conforme sua planilha
            string[] required = {
                "ID","Name","Description","Faction","Lore","Order","Class","Body","Trait","Strength",
                "Cost","Health","Bravery","Atack","Damage","Resistence","Hab1","Hab2","Prep",
                "Credits","Info","Edition"
            };
            foreach (var req in required)
                if (!headerIndex.ContainsKey(req))
                    throw new InvalidDataException($"Cabeçalho obrigatório ausente: '{req}'.");

            var list = new List<CharacterModel>();

            for (int r = 2; r <= lastRow; r++)
            {
                // Lê todos os campos como string
                string id = Read(ws, r, headerIndex["ID"]);
                if (string.IsNullOrWhiteSpace(id))
                    break; // Fim da tabela quando Id vazio

                string name = Read(ws, r, headerIndex["Name"]);
                string desc = Read(ws, r, headerIndex["Description"]);
                string fact = Read(ws, r, headerIndex["Faction"]);
                string lore = Read(ws, r, headerIndex["Lore"]);
                string order = Read(ws, r, headerIndex["Order"]);
                string cls = Read(ws, r, headerIndex["Class"]);
                string bodyT = Read(ws, r, headerIndex["Body"]);
                string trait = Read(ws, r, headerIndex["Trait"]);
                string strT = Read(ws, r, headerIndex["Strength"]);
                string costT = Read(ws, r, headerIndex["Cost"]);
                string hpT = Read(ws, r, headerIndex["Health"]);
                string brvT = Read(ws, r, headerIndex["Bravery"]);
                string atkT = Read(ws, r, headerIndex["Atack"]);
                string dmg = Read(ws, r, headerIndex["Damage"]);
                string res = Read(ws, r, headerIndex["Resistence"]);
                string hab1 = Read(ws, r, headerIndex["Hab1"]);
                string hab2 = Read(ws, r, headerIndex["Hab2"]);
                string prepT = Read(ws, r, headerIndex["Prep"]);
                string cred = Read(ws, r, headerIndex["Credits"]);
                string info = Read(ws, r, headerIndex["Info"]);
                string edit = Read(ws, r, headerIndex["Edition"]);

                // HasPrep: true se veio algum valor "real" (0,1,2,...) — false se vazio/traço
                bool hasPrep = !(string.IsNullOrWhiteSpace(prepT) ||
                                 prepT.Trim() == "-" || prepT.Trim() == "–" || prepT.Trim() == "—");

                // Parse seguro para inteiros (aceita cultura atual e invariant; "-" vira 0)
                static int ParseInt(string s)
                {
                    if (string.IsNullOrWhiteSpace(s)) return 0;
                    s = s.Trim();
                    if (s == "-" || s == "–" || s == "—") return 0;
                    if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v)) return v;
                    if (int.TryParse(s, NumberStyles.Integer, CultureInfo.CurrentCulture, out v)) return v;
                    // fallback: tenta double e arredonda
                    s = s.Replace(',', '.');
                    if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var dv))
                        return (int)Math.Round(dv);
                    return 0;
                }

                int body = ParseInt(bodyT);
                int str = ParseInt(strT);
                int cost = ParseInt(costT);
                int hp = ParseInt(hpT);
                int brv = ParseInt(brvT);
                int atk = ParseInt(atkT);
                int prepV = hasPrep ? ParseInt(prepT) : 0; 

                list.Add(new CharacterModel
                {
                    Id = id,
                    Name = string.IsNullOrWhiteSpace(name) ? string.Empty : name,
                    Description = desc,
                    Faction = fact,
                    Lore = lore,
                    Order = order,
                    Class = cls,
                    Body = body,
                    Trait = trait,
                    Strength = str,
                    Cost = cost,
                    Health = hp,
                    Bravery = brv,
                    Atack = atk,
                    Damage = dmg,
                    Resistence = res,
                    Hab1 = hab1,
                    Hab2 = hab2,
                    Prep = prepV,
                    HasPrep = hasPrep,       
                    Credits = cred,
                    Info = info,
                    Edition = edit
                });
            }

            return list;
        }

        // Helpers
        private static string Read(IXLWorksheet ws, int row, int col)
        {
            var cell = ws.Cell(row, col);
            return cell?.GetFormattedString()?.Trim() ?? string.Empty;
        }
        private static int ReadInt(IXLWorksheet ws, int row, int col)
        {
            if (col <= 0) return 0;
            var cell = ws.Cell(row, col);
            if (cell == null) return 0;

            if (cell.TryGetValue<double>(out var d))
                return (int)Math.Round(d);

            var s = cell.GetString()?.Trim();
            if (string.IsNullOrEmpty(s)) return 0;

            if (int.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out var vi)) return vi;
            if (int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out vi)) return vi;

            if (double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out var vd)) return (int)Math.Round(vd);
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out vd)) return (int)Math.Round(vd);

            return 0;
        }

    }
}
