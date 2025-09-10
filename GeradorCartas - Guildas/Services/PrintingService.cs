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
        private const string TemplatesDir = @"assets\templates";
        private static readonly string TplType1Png = Path.Combine(TemplatesDir, "design_CharacterModel1.png");
        private static readonly string TplType2Png = Path.Combine(TemplatesDir, "design_CharacterModel2.png");
        private static readonly string FieldsType1Csv = Path.Combine(TemplatesDir, "fields_CharacterModel1.csv");
        private static readonly string FieldsType2Csv = Path.Combine(TemplatesDir, "fields_CharacterModel2.csv");

        private const string OutputDir = "output";

        private record FieldDef(
            string Name, double Xp, double Yp, double Wp, double Hp,
            string Align, float FontMin, float FontMax, bool Bold);

        /// <summary>
        /// Gera o PDF (3x3 em A4) com as cartas e salva/abre o arquivo.
        /// </summary>
        public void PrintCharacterCards(List<CharacterModel> chars, string outputName = null, string title = null)
        {
            if (chars == null || chars.Count == 0)
                throw new InvalidOperationException("Nenhum personagem para gerar.");

            if (!File.Exists(TplType1Png) || !File.Exists(TplType2Png))
                throw new FileNotFoundException("Templates PNG não encontrados em assets/templates.");

            var fields1 = LoadFields(FieldsType1Csv);
            var fields2 = LoadFields(FieldsType2Csv);

            using var img1 = (Bitmap)Image.FromFile(TplType1Png);
            using var img2 = (Bitmap)Image.FromFile(TplType2Png);

            // Sanidade do tamanho dos templates (evita bitmaps absurdos)
            EnsureReasonableSize(img1);
            EnsureReasonableSize(img2);

            var pdf = new PdfDocument();
            pdf.Info.Title = string.IsNullOrWhiteSpace(title) ? "Guildas - Cartas (Personagens)" : title;

            // medidas base (mm → pt)
            const double mmToPt = 72.0 / 25.4;
            double cardWmm = 63, cardHmm = 88;
            double marginMm = 10, gapMm = 5;
            int cols = 3, rows = 3;

            // Página A4 retrato
            var pageWidthPt = XUnit.FromMillimeter(210).Point;
            var pageHeightPt = XUnit.FromMillimeter(297).Point;
            var marginPt = marginMm * mmToPt;
            var gapPt = gapMm * mmToPt;
            var cardWpt = cardWmm * mmToPt;
            var cardHpt = cardHmm * mmToPt;

            // grade base (antes de centralizar)
            var cellRects = new List<XRect>();
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    double x = marginPt + c * (cardWpt + gapPt);
                    double y = marginPt + r * (cardHpt + gapPt);
                    cellRects.Add(new XRect(x, y, cardWpt, cardHpt));
                }
            }

            // Centralização (se a grade ficar maior que a área útil, o offset pode ser negativo — ok)
            double gridW = cols * cardWpt + (cols - 1) * gapPt;
            double gridH = rows * cardHpt + (rows - 1) * gapPt;
            double offsetX = (pageWidthPt - 2 * marginPt - gridW) / 2.0;
            double offsetY = (pageHeightPt - 2 * marginPt - gridH) / 2.0;

            for (int i = 0; i < cellRects.Count; i++)
            {
                var r = cellRects[i];
                cellRects[i] = new XRect(r.X + offsetX, r.Y + offsetY, r.Width, r.Height);
            }

            foreach (var chunk in Chunk(chars, cols * rows))
            {
                var page = pdf.AddPage();
                // **Use XUnit para evitar overflow interno**
                page.Width = XUnit.FromMillimeter(210);
                page.Height = XUnit.FromMillimeter(297);

                using var gfx = XGraphics.FromPdfPage(page);

                for (int i = 0; i < chunk.Count; i++)
                {
                    var ch = chunk[i];
                    var rect = cellRects[i];

                    // Escolha do template: **apenas Prep > 0 ativa o Model2**
                    bool isType2 = ch.Prep > 0;
                    var tpl = isType2 ? img2 : img1;
                    var fields = isType2 ? fields2 : fields1;

                    using var bmp = RenderCardBitmap(tpl, fields, ch);

                    // bitmap → PNG bytes → XImage
                    byte[] pngBytes;
                    using (var ms = new MemoryStream())
                    {
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        pngBytes = ms.ToArray();
                    }

                    using var ximg = XImage.FromStream(() => new MemoryStream(pngBytes));
                    gfx.DrawImage(ximg, rect);
                }

                DrawCutMarks(gfx, cellRects, cols, rows);
            }

            Directory.CreateDirectory(OutputDir);
            string outName = string.IsNullOrWhiteSpace(outputName)
                ? $"Characters_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                : $"{Sanitize(outputName)}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string outPath = Path.Combine(OutputDir, outName);

            pdf.Save(outPath);
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = outPath,
                    UseShellExecute = true
                });
            }
            catch { /* abrir é melhor esforço */ }

            pdf.Close();
        }

        private static void EnsureReasonableSize(Bitmap bmp)
        {
            // 300–1200 dpi equivalem, para 63×88 mm, a ~744×1039 até ~2976×4157
            // Aceitamos até 6000 px em qualquer dimensão como guarda-chuva
            if (bmp.Width <= 0 || bmp.Height <= 0 || bmp.Width > 6000 || bmp.Height > 6000)
                throw new InvalidDataException($"Template PNG com tamanho inesperado: {bmp.Width}x{bmp.Height}.");
        }

        private static List<FieldDef> LoadFields(string csvPath)
        {
            if (!File.Exists(csvPath))
                throw new FileNotFoundException("CSV de campos não encontrado.", csvPath);

            var list = new List<FieldDef>();
            var lines = File.ReadAllLines(csvPath);
            if (lines.Length <= 1) return list;

            // Cabeçalho: Field;x%;y%;w%;h%;align;fontMinPt;fontMaxPt;bold
            foreach (var raw in lines.Skip(1))
            {
                var line = raw?.Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(';');
                if (parts.Length < 9) continue;

                string name = parts[0].Trim();
                double xp = ParseDouble(parts[1]);
                double yp = ParseDouble(parts[2]);
                double wp = ParseDouble(parts[3]);
                double hp = ParseDouble(parts[4]);
                string align = parts[5].Trim().ToLowerInvariant();
                float fmin = (float)ParseDouble(parts[6]);
                float fmax = (float)ParseDouble(parts[7]);
                bool bold = ParseBool(parts[8]);

                // clamp de percentuais e fontes (defensivo)
                xp = Clamp01(xp); yp = Clamp01(yp);
                wp = Math.Max(0.01, Clamp01(wp)); // evita 0
                hp = Math.Max(0.01, Clamp01(hp));
                if (fmin < 2) fmin = 2;
                if (fmax < 2) fmax = 2;
                if (fmax < fmin) (fmax, fmin) = (fmin, fmax);

                list.Add(new FieldDef(name, xp, yp, wp, hp, align, fmin, fmax, bold));
            }
            return list;
        }

        private static double Clamp01(double v) => v < 0 ? 0 : (v > 1 ? 1 : v);

        private static double ParseDouble(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            s = s.Trim().Replace('%', ' ').Trim();
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v)) return v;
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out v)) return v;
            s = s.Replace(',', '.');
            double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out v);
            return v;
        }

        private static bool ParseBool(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            return s.Trim().Equals("true", StringComparison.OrdinalIgnoreCase) ||
                   s.Trim().Equals("1");
        }

        private static Bitmap RenderCardBitmap(Bitmap template, List<FieldDef> fields, CharacterModel ch)
        {
            var bmp = new Bitmap(template.Width, template.Height);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.Transparent);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.DrawImageUnscaled(template, 0, 0);

            foreach (var f in fields)
            {
                string val = GetValueForField(ch, f.Name);
                if (string.IsNullOrWhiteSpace(val)) continue;

                // retângulo em px (com clamp defensivo)
                float rx = (float)(f.Xp * template.Width);
                float ry = (float)(f.Yp * template.Height);
                float rw = (float)(f.Wp * template.Width);
                float rh = (float)(f.Hp * template.Height);

                if (float.IsNaN(rx) || float.IsNaN(ry) || float.IsNaN(rw) || float.IsNaN(rh))
                    continue;

                // manter dentro da imagem e com tamanho mínimo
                if (rw < 1f) rw = 1f;
                if (rh < 1f) rh = 1f;
                if (rx < 0f) rx = 0f;
                if (ry < 0f) ry = 0f;
                if (rx + rw > template.Width) rw = template.Width - rx;
                if (ry + rh > template.Height) rh = template.Height - ry;

                var rect = new RectangleF(rx, ry, rw, rh);

                var style = f.Bold ? FontStyle.Bold : FontStyle.Regular;
                using var font = FitFont(g, val, "Segoe UI", f.FontMax, f.FontMin, rect, style);
                using var brush = new SolidBrush(Color.Black);
                using var sf = BuildStringFormat(f.Align);

                try
                {
                    DrawStringWrapped(g, val, font, brush, rect, sf);
                }
                catch (OverflowException ex)
                {
                    throw new OverflowException(
                        $"Overflow ao desenhar '{f.Name}' ret={rect} tpl={template.Width}x{template.Height} val='{Trunc(val, 60)}'",
                        ex
                    );
                }
            }

            return bmp;
        }

        private static string Trunc(string s, int n)
        {
            if (string.IsNullOrEmpty(s) || s.Length <= n) return s;
            return s.Substring(0, n) + "…";
        }

        private static string GetValueForField(CharacterModel c, string fieldName)
        {
            switch (fieldName.Trim().ToLowerInvariant())
            {
                case "id": return c.Id;
                case "name": return c.Name;
                case "cost": return c.Cost.ToString();
                case "class": return JoinNonEmpty(" - ", c.Order, c.Class, string.IsNullOrWhiteSpace(c.Trait) ? null : c.Trait);
                case "faction": return c.Faction;
                case "health": return c.Health.ToString();
                case "resistence": return c.Resistence; // você padronizou assim no CSV/Model
                case "atack": return c.Atack.ToString();
                case "damage": return c.Damage;     // "Damage" = AtackType (string) conforme seu ajuste
                case "bravery": return c.Bravery.ToString();
                case "art": return c.Art;
                case "lore": return c.Lore;
                case "hab1": return c.Hab1;
                case "credits": return JoinNonEmpty(" - ", c.Credits, c.Info, c.Edition);
                // auxiliares (se tiver no CSV e quiser mostrar)
                case "hab2": return c.Hab2;
                case "prep": return c.Prep > 0 ? c.Prep.ToString() : string.Empty;
                case "description": return c.Description;
                default: return string.Empty;
            }
        }

        private static string JoinNonEmpty(string sep, params string[] parts)
        {
            return string.Join(sep, parts?.Where(p => !string.IsNullOrWhiteSpace(p)) ?? Array.Empty<string>());
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
            // guardas para evitar valores inválidos que explodam o GDI+
            if (maxPt < 2) maxPt = 2;
            if (minPt < 2) minPt = 2;
            if (maxPt < minPt) (maxPt, minPt) = (minPt, maxPt);

            float size = maxPt;
            while (size > minPt)
            {
                using var f = new Font(family, size, style, GraphicsUnit.Point);
                var m = MeasureMultiline(g, text, f, rect);
                if (m.Width <= rect.Width + 0.5f && m.Height <= rect.Height + 0.5f)
                    return new Font(family, size, style, GraphicsUnit.Point);
                size -= 0.5f;
            }
            return new Font(family, minPt, style, GraphicsUnit.Point);
        }

        private static SizeF MeasureMultiline(Graphics g, string text, Font font, RectangleF rect)
        {
            using var sf = new StringFormat(StringFormatFlags.LineLimit);
            sf.Trimming = StringTrimming.EllipsisWord;
            return g.MeasureString(text, font, new SizeF(Math.Max(1, rect.Width), 10000), sf);
        }

        private static void DrawStringWrapped(Graphics g, string text, Font font, Brush brush, RectangleF rect, StringFormat sf)
        {
            // evitar largura/altura zero
            if (rect.Width < 1f || rect.Height < 1f) return;
            g.DrawString(text, font, brush, rect, sf);
        }

        private static IEnumerable<List<T>> Chunk<T>(IEnumerable<T> src, int size)
        {
            var list = new List<T>(size);
            foreach (var item in src)
            {
                list.Add(item);
                if (list.Count == size)
                {
                    yield return list;
                    list = new List<T>(size);
                }
            }
            if (list.Count > 0) yield return list;
        }

        private static void DrawCutMarks(XGraphics gfx, List<XRect> cellRects, int cols, int rows)
        {
            var pen = new XPen(XColors.DarkGray, 0.4) { DashStyle = XDashStyle.Dot };

            // verticais
            for (int c = 1; c < cols; c++)
            {
                double x = cellRects[c].Left;
                double yTop = cellRects.First().Top;
                double yBottom = cellRects[^1].Bottom;
                gfx.DrawLine(pen, x, yTop, x, yBottom);
            }

            // horizontais
            for (int r = 1; r < rows; r++)
            {
                double y = cellRects[r * cols].Top;
                double xLeft = cellRects.First().Left;
                double xRight = cellRects[^1].Right;
                gfx.DrawLine(pen, xLeft, y, xRight, y);
            }
        }

        private static string Sanitize(string s)
        {
            foreach (var ch in Path.GetInvalidFileNameChars())
                s = s.Replace(ch, '_');
            return s;
        }
    }
}
