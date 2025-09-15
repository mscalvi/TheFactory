using GeradorCartas___Guildas.Models;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace GeradorCartas___Guildas.Services
{
    internal class PrintingService
    {
        private const string TemplatesDir = @"assets\templates";
        private const string OutputDir = "output";
        private const double TargetDpi = 300.0;

        // =========================
        // Genérico p/ qualquer T
        // =========================
        public void PrintCards<T>(
            List<T> items,
            Func<T, string> selectModelKey,                    // ex.: c => $"CharacterModel{DetectCharacterVariant(c)}"
            Func<T, string, string> fieldResolver,             // ex.: (c, field) => CharacterFieldResolver(c, field)
            string outputName = null,
            string title = null)
        {
            if (items == null || items.Count == 0)
                throw new InvalidOperationException("Nenhum item para gerar.");

            var drawing = new DrawingService();

            // A4 + Carta 63x88
            const double mmToPt = 72.0 / 25.4;
            double pageWmm = 210, pageHmm = 297;
            double cardWmm = 63, cardHmm = 88;
            double marginMm = 10, gapMm = 5;
            int cols = 3, rows = 3;

            double pageWpt = pageWmm * mmToPt, pageHpt = pageHmm * mmToPt;
            double marginPt = marginMm * mmToPt, gapPt = gapMm * mmToPt;
            double cardWpt = cardWmm * mmToPt, cardHpt = cardHmm * mmToPt;

            int cardWpx = (int)Math.Round(cardWmm / 25.4 * TargetDpi);
            int cardHpx = (int)Math.Round(cardHmm / 25.4 * TargetDpi);

            var cellRects = BuildGridRects(cols, rows, marginPt, gapPt, cardWpt, cardHpt);
            CenterGridInPage(cellRects, pageWpt, pageHpt, marginPt);

            var pdf = new PdfDocument
            {
                Info = { Title = string.IsNullOrWhiteSpace(title) ? "Guildas - Cartas" : title }
            };

            // Cache de assets por ModelKey
            var cache = new Dictionary<string, (Bitmap tpl, List<DrawingService.FieldDef> fields)>(StringComparer.OrdinalIgnoreCase);

            try
            {
                foreach (var chunk in Chunk(items, cols * rows))
                {
                    var page = pdf.AddPage();
                    page.Width = XUnit.FromMillimeter(pageWmm);
                    page.Height = XUnit.FromMillimeter(pageHmm);

                    using var gfx = XGraphics.FromPdfPage(page);

                    for (int i = 0; i < chunk.Count; i++)
                    {
                        var item = chunk[i];
                        var rect = cellRects[i];

                        string modelKey = selectModelKey(item); // ex.: "CharacterModel2"
                        if (string.IsNullOrWhiteSpace(modelKey))
                            throw new InvalidDataException("ModelKey vazio.");

                        if (!cache.TryGetValue(modelKey, out var asset))
                        {
                            string png = Path.Combine(TemplatesDir, $"design_{modelKey}.png");
                            string csv = Path.Combine(TemplatesDir, $"fields_{modelKey}.csv");

                            if (!File.Exists(png)) throw new FileNotFoundException($"Template não encontrado: {png}");
                            if (!File.Exists(csv)) throw new FileNotFoundException($"CSV não encontrado: {csv}");

                            var fields = drawing.LoadFields(csv);
                            if (fields.Count == 0) throw new InvalidDataException($"CSV vazio/ inválido: {csv}");

                            var tpl = (Bitmap)Image.FromFile(png);
                            cache[modelKey] = (tpl, fields);
                            asset = cache[modelKey];
                        }

                        using var bmp = drawing.RenderCardBitmap(
                            asset.tpl,
                            asset.fields,
                            fieldName => fieldResolver(item, fieldName),
                            cardWpx, cardHpx, (float)TargetDpi);

                        using var ms = new MemoryStream();
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Position = 0;
                        using var ximg = XImage.FromStream(() => new MemoryStream(ms.ToArray()));
                        gfx.DrawImage(ximg, rect.X, rect.Y, rect.Width, rect.Height);
                    }
                }

                Directory.CreateDirectory(OutputDir);
                string name = string.IsNullOrWhiteSpace(outputName)
                    ? $"Cards_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                    : $"{Sanitize(outputName)}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string outPath = Path.Combine(OutputDir, name);

                pdf.Save(outPath);
                try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = outPath, UseShellExecute = true }); } catch { }
            }
            finally
            {
                // Dispose dos templates do cache
                foreach (var kv in cache.Values) kv.tpl.Dispose();
                pdf.Close();
            }
        }

        // =========================
        // Compat: Characters 1/2/3
        // =========================
        public void PrintCharacterCards(List<CharacterModel> chars, string outputName = null, string title = null)
        {
            PrintCards(
                chars,
                c => $"CharacterModel{DetectCharacterVariant(c)}",
                CharacterFieldResolver,
                outputName,
                title ?? "Guildas - Cartas (Personagens)");
        }

        public void PrintActionCards(List<ActionsModel> actions, string outputName = null, string title = null)
        {
            PrintCards(
                actions,
                a => "ActionModel1",
                ActionsFieldResolver,
                outputName,
                title ?? "Guildas - Cartas (Ações)");
        }

        public void PrintMapCards(List<MapModel> maps, string outputName = null, string title = null)
        {
            PrintCards(
                maps,
                m => "MapModel1",
                MapFieldResolver,
                outputName,
                title ?? "Guildas - Cartas (Mapa)");
        }

        public void PrintPersonalityCards(List<PersonalityModel> personalities, string outputName = null, string title = null)
        {
            PrintCards(
                personalities,
                p => $"PersonalityModel{DetectPersonalityVariant(p)}",
                PersonalityFieldResolver,
                outputName,
                title ?? "Guildas - Cartas (Personalidades)");
        }

        private static int DetectCharacterVariant(CharacterModel c)
        {
            bool hasHab2 = !string.IsNullOrWhiteSpace(c.Hab2);
            if (!hasHab2) return 1;
            if (c.HasPrep) return 2;
            return 3;
        }

        // ========== Actions ==========
        private static string ActionsFieldResolver(ActionsModel a, string field)
        {
            if (field.Equals("Art", StringComparison.OrdinalIgnoreCase))
            {
                // return a.Art; // ← ORIGINAL (deixe comentado para poder voltar)

                // Usa o prefixo do Name para decidir a arte genérica
                var n = NormalizeType(a.Name ?? string.Empty); // helper que já temos (remove acentos e baixa a caixa)

                if (n.StartsWith("montar")) return "AcMontarModelArt.png";
                if (n.StartsWith("planejar")) return "AcPlanejarModelArt.png";
                if (n.StartsWith("aventurar")) return "AcAventurarModelArt.png";
                if (n.StartsWith("recrutar")) return "AcRecrutarModelArt.png";
                if (n.StartsWith("cochilar")) return "AcCochilarModelArt.png";
                if (n.StartsWith("enfrentar")) return "AcEnfrentarModelArt.png";

                return a.Art; // fallback se não casar
            }

            if (field.Equals("Rules", StringComparison.OrdinalIgnoreCase))
                return JoinRules(a); // (já tínhamos)

            return ReflectiveResolver(a, field);
        }


        private static string JoinRules(ActionsModel a)
        {
            var parts = new[]
            {
        string.IsNullOrWhiteSpace(a.Rules1) ? null : "- " + a.Rules1.Trim(),
        string.IsNullOrWhiteSpace(a.Rules2) ? null : "- " + a.Rules2.Trim(),
        string.IsNullOrWhiteSpace(a.Rules3) ? null : "- " + a.Rules3.Trim(),
        string.IsNullOrWhiteSpace(a.Rules4) ? null : "- " + a.Rules4.Trim(),
    }.Where(s => !string.IsNullOrWhiteSpace(s));

            return string.Join("\n", parts); // uma por linha; quebra automática continua funcionando
        }

        // =========== Map ============
        private static string MapFieldResolver(MapModel m, string field)
        {
            if (field.Equals("Art", StringComparison.OrdinalIgnoreCase))
            {
                // return m.Art; // ← ORIGINAL (descomente para voltar)

                var t = NormalizeType(m.Type ?? string.Empty); // remove acentos e deixa minúsculo

                if (t.StartsWith("desafio")) return "AvDesafioModelArt.png";
                if (t.StartsWith("descanso")) return "AvDescansoModelArt.png";
                if (t.StartsWith("criminosos")) return "AvCriminososModelArt.png";
                if (t.StartsWith("besta")) return "AvBestaModelArt.png";
                if (t.StartsWith("encontro")) return "AvEncontroModelArt.png";
                if (t.StartsWith("evento")           // cobre "Evento" e "Evento Público"
                    || t == "evento publico") return "AvEventoModelArt.png";
                if (t.StartsWith("cidade")) return "AvCidadeModelArt.png";

                return m.Art; // fallback para tipos não mapeados (ex.: "Descanso")
            }

            if (field.Equals("Option1", StringComparison.OrdinalIgnoreCase))
                return PrefixOption(m.Type, m.Option1, isFirst: true);

            if (field.Equals("Option2", StringComparison.OrdinalIgnoreCase))
                return PrefixOption(m.Type, m.Option2, isFirst: false);

            return ReflectiveResolver(m, field);
        }


        private static string PrefixOption(string type, string option, bool isFirst)
        {
            if (string.IsNullOrWhiteSpace(option)) return string.Empty;

            string t = NormalizeType(type);
            string prefix = null;

            // Regras:
            // Desafio → Option1: "Passou: ", Option2: "Falhou: "
            if (t == "desafio")
                prefix = isFirst ? "Passou - " : "Falhou - ";

            // Descanso / Cidade → Option1/2 com "Opção 1: " / "Opção 2: "
            else if (t == "descanso" || t == "cidade")
                prefix = isFirst ? "Opção 1 - " : "Opção 2 - ";

            // Besta / Criminosos → Option1: "O inimigo tem: " / Option2: "Após o combate: "
            else if (t == "besta" || t == "criminosos")
                prefix = isFirst ? "O inimigo tem - " : "Após o combate - ";

            // Evento Público → Option1: "Participar: " / Option2: "Continuar viagem: "
            else if (t == "evento publico") // (diacríticos já normalizados)
                prefix = isFirst ? "Participar - " : "Continuar viagem - ";

            // Encontro → sem prefixo
            else if (t == "encontro")
                prefix = null;

            // Caso não caia em nenhuma categoria, não prefixa.
            return AddPrefixIfMissing(option.Trim(), prefix);
        }

        private static string NormalizeType(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) return string.Empty;
            var s = RemoveDiacritics(type).Trim().ToLowerInvariant();
            // colapsa múltiplos espaços:
            s = string.Join(" ", s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            return s;
        }

        private static string AddPrefixIfMissing(string text, string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) return text;
            // Evita duplicar caso o dado já venha com o mesmo prefixo
            if (text.StartsWith(prefix, true, CultureInfo.InvariantCulture)) return text;
            return prefix + text;
        }

        private static string RemoveDiacritics(string s)
        {
            var norm = s.Normalize(NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder(capacity: s.Length);
            foreach (var ch in norm)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        // ======== Personalities ========
        private static int DetectPersonalityVariant(PersonalityModel p)
        {
            bool hasHab2 = !string.IsNullOrWhiteSpace(p.Hab2);   // Model3/4 têm Hab2
            bool hasPrep = p.Prep > 0;                           // Model2/4 têm Prep

            // Model1: sem Hab2 e sem Prep
            if (!hasHab2 && !hasPrep) return 1;

            // Model2: Hab1 + Prep, sem Hab2
            if (!hasHab2 && hasPrep) return 2;

            // Model3: Hab1 + Hab2, sem Prep
            if (hasHab2 && !hasPrep) return 3;

            // Model4: Hab1 + Hab2 + Prep
            return 4;
        }
        private static string PersonalityFieldResolver(PersonalityModel p, string field)
        {
            // Intercepta somente o campo "Art" para forçar arte genérica por prefixo do Type
            if (field.Equals("Art", StringComparison.OrdinalIgnoreCase))
            {
                // return p.Art; // ← ORIGINAL (descomente para voltar ao comportamento anterior)

                var tNorm = NormalizeType(p.Type ?? string.Empty); // usa a mesma helper do Map (remove acentos e baixa a caixa)
                if (tNorm.StartsWith("item -"))
                    return "PeItemModelArt.png";
                if (tNorm.StartsWith("construcao -"))
                    return "PeConstrucoesModelArt.png";
                if (tNorm.StartsWith("companheiro -"))
                    return "PeCompanheirosModelArt.png";
                if (tNorm.StartsWith("atividade"))
                    return "PeAtividadeModelArt.png";

                // Fallback: mantém o que vier da planilha
                return p.Art;
            }

            // Demais campos vão por reflexão normalmente
            return ReflectiveResolver(p, field);
        }


        // ======== Fallback reflexivo (se você já tiver um, mantenha o seu) ========
        private static string ReflectiveResolver<T>(T obj, string field)
        {
            if (obj == null || string.IsNullOrWhiteSpace(field)) return string.Empty;

            var prop = typeof(T).GetProperties()
                .FirstOrDefault(p => string.Equals(p.Name, field, StringComparison.OrdinalIgnoreCase));
            if (prop == null) return string.Empty;

            var val = prop.GetValue(obj);
            if (val == null) return string.Empty;

            return val switch
            {
                IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
                _ => val.ToString()
            };
        }


        // ===========
        // Grid utils
        // ===========
        private static List<XRect> BuildGridRects(int cols, int rows, double marginPt, double gapPt, double cardWpt, double cardHpt)
        {
            var cellRects = new List<XRect>(cols * rows);
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    double x = marginPt + c * (cardWpt + gapPt);
                    double y = marginPt + r * (cardHpt + gapPt);
                    cellRects.Add(new XRect(x, y, cardWpt, cardHpt));
                }
            return cellRects;
        }

        private static void CenterGridInPage(List<XRect> cellRects, double pageWpt, double pageHpt, double marginPt)
        {
            double gridLeft = cellRects.Min(r => r.Left);
            double gridTop = cellRects.Min(r => r.Top);
            double gridRight = cellRects.Max(r => r.Right);
            double gridBottom = cellRects.Max(r => r.Bottom);

            double gridW = gridRight - gridLeft;
            double gridH = gridBottom - gridTop;

            double offX = (pageWpt - 2 * marginPt - gridW) / 2.0;
            double offY = (pageHpt - 2 * marginPt - gridH) / 2.0;

            for (int i = 0; i < cellRects.Count; i++)
            {
                var r = cellRects[i];
                cellRects[i] = new XRect(r.X + offX, r.Y + offY, r.Width, r.Height);
            }
        }

        private static IEnumerable<List<T>> Chunk<T>(IEnumerable<T> src, int size)
        {
            var buf = new List<T>(size);
            foreach (var item in src)
            {
                buf.Add(item);
                if (buf.Count == size)
                {
                    yield return buf;
                    buf = new List<T>(size);
                }
            }
            if (buf.Count > 0) yield return buf;
        }

        private static string Sanitize(string s)
        {
            foreach (var ch in Path.GetInvalidFileNameChars())
                s = s.Replace(ch, '_');
            return s;
        }

        // ===============================
        // Character → resolver de campos
        // ===============================
        private static string CharacterFieldResolver(CharacterModel c, string fieldName)
        {
            switch ((fieldName ?? "").Trim().ToLowerInvariant())
            {
                case "id": return c.Id;
                case "name": return c.Name;
                case "cost": return c.Cost.ToString(System.Globalization.CultureInfo.InvariantCulture);
                case "class":
                    {
                        string trait = (string.IsNullOrWhiteSpace(c.Trait) || c.Trait == "-" || c.Trait == "–" || c.Trait == "—")
                                       ? null : c.Trait;
                        return trait is null ? (c.Class ?? string.Empty) : $"{c.Class} - {trait}";
                    }
                case "faction":
                    return string.Join(" - ", new[] { c.Order, c.Faction }.Where(s => !string.IsNullOrWhiteSpace(s)));
                case "health": return c.Health.ToString(System.Globalization.CultureInfo.InvariantCulture);
                case "resistence": return c.Resistence;
                case "atack": return c.Atack.ToString(System.Globalization.CultureInfo.InvariantCulture);
                case "damage": return c.Damage;
                case "bravery": return c.Bravery.ToString(System.Globalization.CultureInfo.InvariantCulture);
                case "art":
                    // return c.Art; // ← ORIGINAL (comente para poder voltar)
                    return "CharacterModelArt.png"; // ← FORÇADO (garanta o arquivo em assets\image(s))
                case "lore": return c.Lore;
                case "hab1": return c.Hab1;
                case "hab2": return c.Hab2;
                case "prep": return c.HasPrep ? c.Prep.ToString(System.Globalization.CultureInfo.InvariantCulture) : string.Empty;
                case "credits": return string.Join(" - ", new[] { c.Credits, c.Info, c.Edition }.Where(x => !string.IsNullOrWhiteSpace(x)));
                case "description": return c.Description;
                default: return string.Empty;
            }
        }
    }
}
