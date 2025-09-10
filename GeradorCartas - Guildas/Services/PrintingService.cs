using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using GeradorCartas___Guildas.Models;

namespace GeradorCartas___Guildas.Services
{
    internal class PrintingService
    {
        // Caminhos dos assets
        private const string TemplatesDir = @"assets\templates";
        private static readonly string TplType1Png = Path.Combine(TemplatesDir, "design_CharacterModel1.png");
        private static readonly string TplType2Png = Path.Combine(TemplatesDir, "design_CharacterModel2.png");
        private static readonly string FieldsType1Csv = Path.Combine(TemplatesDir, "fields_CharacterModel1.csv");
        private static readonly string FieldsType2Csv = Path.Combine(TemplatesDir, "fields_CharacterModel2.csv");

        private const string OutputDir = "output";
        private const double TargetDpi = 300.0; // DPI para render do bitmap da carta

        private record FieldDef(string Name, double Xp, double Yp, double Wp, double Hp,
                                string Align, float FontMin, float FontMax, bool Bold);

        public void PrintCharacterCards(List<CharacterModel> chars, string outputName = null, string title = null)
        {
            if (chars == null || chars.Count == 0)
                throw new InvalidOperationException("Nenhum personagem para gerar.");

            // Carrega CSVs (um por modelo) — aceita ; ou TAB
            var fields1 = LoadFields(FieldsType1Csv);
            var fields2 = LoadFields(FieldsType2Csv);
            if (fields1.Count == 0) throw new InvalidDataException($"CSV vazio ou inválido: {FieldsType1Csv}");
            if (fields2.Count == 0) throw new InvalidDataException($"CSV vazio ou inválido: {FieldsType2Csv}");

            // Carrega templates
            using var img1 = (Bitmap)Image.FromFile(TplType1Png);
            using var img2 = (Bitmap)Image.FromFile(TplType2Png);

            var pdf = new PdfDocument();
            pdf.Info.Title = string.IsNullOrWhiteSpace(title) ? "Guildas - Cartas (Personagens)" : title;

            // --- Medidas físicas (A4 e carta MTG 63×88 mm) ---
            const double mmToPt = 72.0 / 25.4;
            double pageWmm = 210, pageHmm = 297;        // A4
            double cardWmm = 63, cardHmm = 88;         // MTG
            double marginMm = 10, gapMm = 5;
            int cols = 3, rows = 3;

            // Conversões
            double pageWpt = pageWmm * mmToPt, pageHpt = pageHmm * mmToPt;
            double marginPt = marginMm * mmToPt, gapPt = gapMm * mmToPt;
            double cardWpt = cardWmm * mmToPt, cardHpt = cardHmm * mmToPt;

            // Tamanho do bitmap alvo em pixels (para 300 DPI)
            int cardWpx = (int)Math.Round(cardWmm / 25.4 * TargetDpi);
            int cardHpx = (int)Math.Round(cardHmm / 25.4 * TargetDpi);

            // Calcula grade 3×3 (destinos em PONTOS)
            var cellRects = new List<XRect>(cols * rows);
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    double x = marginPt + c * (cardWpt + gapPt);
                    double y = marginPt + r * (cardHpt + gapPt);
                    cellRects.Add(new XRect(x, y, cardWpt, cardHpt));
                }

            // Centraliza grade na página
            double gridW = cols * cardWpt + (cols - 1) * gapPt;
            double gridH = rows * cardHpt + (rows - 1) * gapPt;
            double offX = (pageWpt - 2 * marginPt - gridW) / 2.0;
            double offY = (pageHpt - 2 * marginPt - gridH) / 2.0;
            for (int i = 0; i < cellRects.Count; i++)
            {
                var r = cellRects[i];
                cellRects[i] = new XRect(r.X + offX, r.Y + offY, r.Width, r.Height);
            }

            // Render e composição em páginas
            foreach (var chunk in Chunk(chars, cols * rows))
            {
                var page = pdf.AddPage();
                page.Width = XUnit.FromMillimeter(pageWmm);
                page.Height = XUnit.FromMillimeter(pageHmm);

                using var gfx = XGraphics.FromPdfPage(page);

                for (int i = 0; i < chunk.Count; i++)
                {
                    var ch = chunk[i];
                    var rect = cellRects[i];

                    bool isType2 = ch.Prep > 0;            // regra: Model2 se Prep>0
                    var tpl = isType2 ? img2 : img1;
                    var fields = isType2 ? fields2 : fields1;

                    // Renderiza a carta como bitmap no tamanho final (300 DPI)
                    using var bmp = RenderCardBitmap(tpl, fields, ch, cardWpx, cardHpx);

                    // Desenha preenchendo a célula (em PONTOS)
                    using var ms = new MemoryStream();
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;
                    using var ximg = XImage.FromStream(() => new MemoryStream(ms.ToArray()));
                    gfx.DrawImage(ximg, rect.X, rect.Y, rect.Width, rect.Height);
                }
            }

            // Salva arquivo
            Directory.CreateDirectory(OutputDir);
            string name = string.IsNullOrWhiteSpace(outputName)
                ? $"Characters_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                : $"{Sanitize(outputName)}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string outPath = Path.Combine(OutputDir, name);

            pdf.Save(outPath);
            try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = outPath, UseShellExecute = true }); } catch { }
            pdf.Close();
        }

        // --- CSV: aceita ; ou TAB. Cada LINHA é um campo (nome + retângulo + tipografia). ---
        private static List<FieldDef> LoadFields(string csvPath)
        {
            var list = new List<FieldDef>();
            if (!File.Exists(csvPath)) return list;

            var lines = File.ReadAllLines(csvPath);
            if (lines.Length <= 1) return list;

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                // tenta ;, senão TAB
                string[] parts = line.Split(';');
                if (parts.Length < 2) parts = line.Split('\t');
                if (parts.Length < 9) continue; // linha inválida

                string name = parts[0].Trim();
                double xp = ParseDouble(parts[1]);
                double yp = ParseDouble(parts[2]);
                double wp = ParseDouble(parts[3]);
                double hp = ParseDouble(parts[4]);
                string align = (parts[5] ?? "").Trim().ToLowerInvariant();
                float fmin = (float)ParseDouble(parts[6]);
                float fmax = (float)ParseDouble(parts[7]);
                bool bold = ParseBool(parts[8]);

                // sanidade básica
                xp = Clamp01(xp); yp = Clamp01(yp);
                wp = Math.Max(0.01, Clamp01(wp));
                hp = Math.Max(0.01, Clamp01(hp));
                if (fmin < 2) fmin = 2;
                if (fmax < 2) fmax = 2;
                if (fmax < fmin) (fmax, fmin) = (fmin, fmax);

                list.Add(new FieldDef(name, xp, yp, wp, hp, align, fmin, fmax, bold));
            }
            return list;
        }

        private static Bitmap RenderCardBitmap(Bitmap template, List<FieldDef> fields, CharacterModel ch, int cardWidthPx, int cardHeightPx)
        {
            var bmp = new Bitmap(cardWidthPx, cardHeightPx, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            bmp.SetResolution((float)TargetDpi, (float)TargetDpi);

            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.Transparent);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Fundo (template) redimensionado para o tamanho alvo
            g.DrawImage(template,
                        new Rectangle(0, 0, cardWidthPx, cardHeightPx),
                        new Rectangle(0, 0, template.Width, template.Height),
                        GraphicsUnit.Pixel);

            // Desenha os campos do CSV (valores vindos do CharacterModel)
            foreach (var f in fields)
            {
                string val = GetValueForField(ch, f.Name);
                if (string.IsNullOrWhiteSpace(val)) continue;

                float rx = (float)(f.Xp * cardWidthPx);
                float ry = (float)(f.Yp * cardHeightPx);
                float rw = (float)(f.Wp * cardWidthPx);
                float rh = (float)(f.Hp * cardHeightPx);

                if (rw < 1f) rw = 1f; if (rh < 1f) rh = 1f;
                if (rx < 0f) rx = 0f; if (ry < 0f) ry = 0f;
                if (rx + rw > cardWidthPx) rw = cardWidthPx - rx;
                if (ry + rh > cardHeightPx) rh = cardHeightPx - ry;

                var rect = new RectangleF(rx, ry, rw, rh);

                var style = f.Bold ? FontStyle.Bold : FontStyle.Regular;
                using var font = FitFont(g, val, "Segoe UI", f.FontMax, f.FontMin, rect, style);
                using var brush = new SolidBrush(Color.Black);
                using var sf = BuildStringFormat(f.Align);

                g.DrawString(val, font, brush, rect, sf);
            }

            return bmp;
        }

        // Mapeamento EXATO para o seu CharacterModel atual
        private static string GetValueForField(CharacterModel c, string fieldName)
        {
            switch (fieldName.Trim().ToLowerInvariant())
            {
                case "id": return c.Id;
                case "name": return c.Name;
                case "cost": return c.Cost.ToString(CultureInfo.InvariantCulture);
                case "class": return string.Join(" - ", new[] { c.Order, c.Class, string.IsNullOrWhiteSpace(c.Trait) ? null : c.Trait }.Where(x => !string.IsNullOrWhiteSpace(x)));
                case "faction": return c.Faction;
                case "health": return c.Health.ToString(CultureInfo.InvariantCulture);
                case "resistence": return c.Resistence;
                case "atack": return c.Atack.ToString(CultureInfo.InvariantCulture);
                case "damage": return c.Damage;
                case "bravery": return c.Bravery.ToString(CultureInfo.InvariantCulture);
                case "art": return c.Art;
                case "lore": return c.Lore;
                case "hab1": return c.Hab1;
                case "credits": return string.Join(" - ", new[] { c.Credits, c.Info, c.Edition }.Where(x => !string.IsNullOrWhiteSpace(x)));
                case "hab2": return c.Hab2;
                case "prep": return c.Prep > 0 ? c.Prep.ToString(CultureInfo.InvariantCulture) : string.Empty;
                case "description": return c.Description;
                default: return string.Empty;
            }
        }

        private static StringFormat BuildStringFormat(string align)
        {
            var sf = new StringFormat(StringFormatFlags.LineLimit);
            sf.Trimming = StringTrimming.EllipsisWord;
            sf.Alignment = align switch
            {
                "center" => StringAlignment.Center,
                "right" => StringAlignment.Far,
                _ => StringAlignment.Near
            };
            sf.LineAlignment = StringAlignment.Near;
            return sf;
        }

        private static Font FitFont(Graphics g, string text, string family, float maxPt, float minPt, RectangleF rect, FontStyle style)
        {
            if (maxPt < minPt) (maxPt, minPt) = (minPt, maxPt);
            if (minPt < 2) minPt = 2;
            if (maxPt < 2) maxPt = 2;

            float size = maxPt;
            while (size > minPt)
            {
                using var f = new Font(family, size, style, GraphicsUnit.Point);
                var m = Measure(g, text, f, rect);
                if (m.Width <= rect.Width + 0.5f && m.Height <= rect.Height + 0.5f)
                    return new Font(family, size, style, GraphicsUnit.Point);
                size -= 0.5f;
            }
            return new Font(family, minPt, style, GraphicsUnit.Point);
        }

        private static SizeF Measure(Graphics g, string text, Font font, RectangleF rect)
        {
            using var sf = new StringFormat(StringFormatFlags.LineLimit) { Trimming = StringTrimming.EllipsisWord };
            return g.MeasureString(text, font, new SizeF(Math.Max(1, rect.Width), 10000), sf);
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

        private static double Clamp01(double v) => v < 0 ? 0 : (v > 1 ? 1 : v);

        private static double ParseDouble(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            s = s.Trim().Replace('%', ' ').Trim();
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v)) return v;
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out v)) return v;
            s = s.Replace(',', '.'); double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out v);
            return v;
        }

        private static bool ParseBool(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            return s.Trim().Equals("true", StringComparison.OrdinalIgnoreCase) || s.Trim().Equals("1");
        }

        private static string Sanitize(string s)
        {
            foreach (var ch in Path.GetInvalidFileNameChars())
                s = s.Replace(ch, '_');
            return s;
        }
    }
}
