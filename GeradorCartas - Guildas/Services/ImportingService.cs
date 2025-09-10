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
            var ws = wb.Worksheets.First(); // única planilha

            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
            var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;

            // Cabeçalhos exatos (linha 1) — case-insensitive; ordem não importa
            var headerIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int c = 1; c <= lastCol; c++)
            {
                var h = ws.Cell(1, c).GetString().Trim();
                if (!string.IsNullOrEmpty(h) && !headerIndex.ContainsKey(h))
                    headerIndex[h] = c;
            }

            string[] required = {
                "ID","Name","Description","Faction","Lore","Order","Class","Body","Trait","Strength",
                "Cost","Health","Bravery","Atack","Damage","Resistence","Hab1","Hab2","Prep",
                "Credits","Info","Edition"
            };
            foreach (var req in required)
            {
                if (!headerIndex.ContainsKey(req))
                    throw new InvalidDataException($"Cabeçalho obrigatório ausente: '{req}'.");
            }

            _characters.Clear();

            for (int r = 2; r <= lastRow; r++)
            {
                // Parada: ID vazio => fim da tabela
                string id = Read(ws, r, headerIndex["ID"]);
                if (string.IsNullOrWhiteSpace(id))
                    break;

                // Strings
                string name = Read(ws, r, headerIndex["Name"]);
                string description = Read(ws, r, headerIndex["Description"]);
                string faction = Read(ws, r, headerIndex["Faction"]);
                string lore = Read(ws, r, headerIndex["Lore"]);
                string order = Read(ws, r, headerIndex["Order"]);
                string @class = Read(ws, r, headerIndex["Class"]);
                string trait = Read(ws, r, headerIndex["Trait"]);
                string resistence = Read(ws, r, headerIndex["Resistence"]);
                string hab1 = Read(ws, r, headerIndex["Hab1"]);
                string hab2 = Read(ws, r, headerIndex["Hab2"]);
                string credits = Read(ws, r, headerIndex["Credits"]);
                string info = Read(ws, r, headerIndex["Info"]);
                string edition = Read(ws, r, headerIndex["Edition"]);
                string damage = Read(ws, r, headerIndex["Damage"]);

                // Ints
                int body = ReadInt(ws, r, headerIndex["Body"]);
                int strength = ReadInt(ws, r, headerIndex["Strength"]);
                int cost = ReadInt(ws, r, headerIndex["Cost"]);
                int health = ReadInt(ws, r, headerIndex["Health"]);
                int bravery = ReadInt(ws, r, headerIndex["Bravery"]);
                int atack = ReadInt(ws, r, headerIndex["Atack"]);
                int prep = ReadInt(ws, r, headerIndex["Prep"]);

                _characters.Add(new CharacterModel
                {
                    Id = id,
                    Name = name,
                    Description = description,
                    Faction = faction,
                    Lore = lore,
                    Order = order,
                    Class = @class,
                    Body = body,
                    Trait = trait,
                    Strength = strength,
                    Cost = cost,
                    Health = health,
                    Bravery = bravery,
                    Atack = atack,
                    Damage = damage,
                    Resistence = resistence,
                    Hab1 = hab1,
                    Hab2 = hab2,
                    Prep = prep,
                    Credits = credits,
                    Info = info,
                    Edition = edition
                });
            }

            return new List<CharacterModel>(_characters);
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
