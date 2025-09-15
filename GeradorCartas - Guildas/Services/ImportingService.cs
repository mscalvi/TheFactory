using ClosedXML.Excel;
using GeradorCartas___Guildas.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GeradorCartas___Guildas.Services
{
    internal class ImportingService
    {
        // Storage
        private readonly List<MapModel> _maps = new();
        public IReadOnlyList<MapModel> Maps => _maps;

        private readonly List<CharacterModel> _characters = new();
        public IReadOnlyList<CharacterModel> Characters => _characters;

        private readonly List<ActionsModel> _actions = new();
        public IReadOnlyList<ActionsModel> Actions => _actions;

        private readonly List<PersonalityModel> _personalities = new();
        public IReadOnlyList<PersonalityModel> Personalities => _personalities;

        private readonly List<RelicModel> _relics = new();
        public IReadOnlyList<RelicModel> Relics => _relics;

        // =========================
        // Entradas públicas
        // =========================

        public List<MapModel> ImportMapsList(string filePath)
        {
            _maps.Clear();
            _maps.AddRange(ImportList<MapModel>(
                filePath,
                requiredHeaders: new[] { "Id" },
                postProcess: (m, _, __) =>
                {
                    if (string.IsNullOrWhiteSpace(m.Art)) m.Art = m.Id;
                }));
            return new List<MapModel>(_maps);
        }

        public List<CharacterModel> ImportCharactersList(string filePath)
        {
            _characters.Clear();
            _characters.AddRange(ImportList<CharacterModel>(
                filePath,
                requiredHeaders: new[] { "Id" },
                postProcess: (c, _, __) =>
                {
                    c.HasPrep = c.Prep > 0;
                    // Se quiser forçar Art = Id:
                    // if (string.IsNullOrWhiteSpace(c.Art)) c.Art = c.Id;
                }));
            return new List<CharacterModel>(_characters);
        }

        public List<ActionsModel> ImportActionsList(string filePath)
        {
            _actions.Clear();
            _actions.AddRange(ImportList<ActionsModel>(
                filePath,
                requiredHeaders: new[] { "Id" },
                postProcess: (a, _, __) =>
                {
                    if (string.IsNullOrWhiteSpace(a.Art)) a.Art = a.Id;
                }));
            return new List<ActionsModel>(_actions);
        }

        public List<PersonalityModel> ImportPersonalitiesList(string filePath)
        {
            _personalities.Clear();
            _personalities.AddRange(ImportList<PersonalityModel>(
                filePath,
                requiredHeaders: new[] { "Id" },
                postProcess: (p, _, __) =>
                {
                    if (string.IsNullOrWhiteSpace(p.Art)) p.Art = p.Id;
                }));
            return new List<PersonalityModel>(_personalities);
        }

        public List<RelicModel> ImportRelicsList(string filePath)
        {
            _relics.Clear();
            _relics.AddRange(ImportList<RelicModel>(
                filePath,
                requiredHeaders: new[] { "Id" },
                postProcess: (r, _, __) =>
                {
                    if (string.IsNullOrWhiteSpace(r.Art)) r.Art = r.Id;
                }));
            return new List<RelicModel>(_relics);
        }

        // =========================
        // Importador genérico
        // =========================
        private static List<T> ImportList<T>(
            string filePath,
            Action<T, Dictionary<string, int>, IXLWorksheet>? postProcess = null,
            IEnumerable<string>? requiredHeaders = null) where T : new()
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Caminho do arquivo não informado.", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Arquivo não encontrado.", filePath);

            using var wb = new XLWorkbook(filePath);
            var ws = wb.Worksheets.First();

            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
            var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;

            // Cabeçalhos na linha 1 (case-insensitive)
            var headerIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int c = 1; c <= lastCol; c++)
            {
                var h = ws.Cell(1, c).GetString().Trim();
                if (!string.IsNullOrEmpty(h) && !headerIndex.ContainsKey(h))
                    headerIndex[h] = c;
            }

            if (requiredHeaders != null)
                foreach (var req in requiredHeaders)
                    if (!headerIndex.ContainsKey(req))
                        throw new InvalidDataException($"Cabeçalho obrigatório ausente: '{req}'.");

            // Propriedades settable do T
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(p => p.CanWrite)
                                 .ToArray();
            var propMap = props.ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

            // Coluna Id (aceita "Id" ou "ID")
            int idCol = -1;
            if (headerIndex.TryGetValue("Id", out var c1)) idCol = c1;
            else if (headerIndex.TryGetValue("ID", out var c2)) idCol = c2;

            var list = new List<T>();

            for (int r = 2; r <= lastRow; r++)
            {
                bool breakRow = false;
                if (idCol > 0)
                {
                    var idVal = ReadString(ws, r, idCol);
                    if (string.IsNullOrWhiteSpace(idVal)) breakRow = true;
                }
                else
                {
                    bool any = false;
                    for (int c = 1; c <= lastCol; c++)
                        if (!string.IsNullOrWhiteSpace(ws.Cell(r, c).GetString())) { any = true; break; }
                    if (!any) breakRow = true;
                }
                if (breakRow) break;

                var item = new T();

                // Seta propriedades a partir de colunas de mesmo nome
                foreach (var kv in propMap)
                {
                    if (!headerIndex.TryGetValue(kv.Key, out var col)) continue;

                    string s = ReadString(ws, r, col);
                    try
                    {
                        object? converted = ConvertStringTo(kv.Value.PropertyType, s);
                        if (converted != null)
                            kv.Value.SetValue(item, converted);
                    }
                    catch { /* ignora conversões inválidas */ }
                }

                postProcess?.Invoke(item, headerIndex, ws);
                list.Add(item);
            }

            return list;
        }

        // =========================
        // Helpers
        // =========================
        private static string ReadString(IXLWorksheet ws, int row, int col)
        {
            if (col <= 0) return string.Empty;
            var cell = ws.Cell(row, col);
            return cell?.GetFormattedString()?.Trim() ?? string.Empty;
        }

        private static object? ConvertStringTo(Type t, string s)
        {
            if (t == typeof(string))
                return s ?? string.Empty;

            if (t == typeof(int) || t == typeof(int?))
            {
                int v = ParseIntSafe(s);
                return t == typeof(int?) ? (int?)v : v;
            }

            if (t == typeof(double) || t == typeof(double?))
            {
                double v = ParseDoubleSafe(s);
                return t == typeof(double?) ? (double?)v : v;
            }

            if (t == typeof(decimal) || t == typeof(decimal?))
            {
                decimal v = ParseDecimalSafe(s);
                return t == typeof(decimal?) ? (decimal?)v : v;
            }

            if (t == typeof(bool) || t == typeof(bool?))
            {
                bool v = ParseBoolSafe(s);
                return t == typeof(bool?) ? (bool?)v : v;
            }

            if (t.IsEnum)
            {
                try { return Enum.Parse(t, s, ignoreCase: true); }
                catch { return Activator.CreateInstance(t); }
            }

            return null; // tipos complexos: ignorar
        }

        private static int ParseIntSafe(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            s = s.Trim();
            if (s == "-" || s == "–" || s == "—") return 0;

            if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var vi)) return vi;
            if (int.TryParse(s, NumberStyles.Integer, CultureInfo.CurrentCulture, out vi)) return vi;

            s = s.Replace(',', '.');
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var vd))
                return (int)Math.Round(vd);

            return 0;
        }

        private static double ParseDoubleSafe(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0d;
            s = s.Trim();
            if (s == "-" || s == "–" || s == "—") return 0d;

            if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var v)) return v;
            if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out v)) return v;

            s = s.Replace(',', '.');
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v)) return v;

            return 0d;
        }

        private static decimal ParseDecimalSafe(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0m;
            s = s.Trim();
            if (s == "-" || s == "–" || s == "—") return 0m;

            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v)) return v;
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.CurrentCulture, out v)) return v;

            s = s.Replace(',', '.');
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out v)) return v;

            return 0m;
        }

        private static bool ParseBoolSafe(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim();

            if (bool.TryParse(s, out var vb)) return vb;

            // Inteiros 0/1
            if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var vi))
                return vi != 0;
            if (int.TryParse(s, NumberStyles.Integer, CultureInfo.CurrentCulture, out vi))
                return vi != 0;

            // "sim"/"não"
            if (string.Equals(s, "sim", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(s, "nao", StringComparison.OrdinalIgnoreCase)) return false;
            if (string.Equals(s, "não", StringComparison.OrdinalIgnoreCase)) return false;

            return false;
        }
    }
}
