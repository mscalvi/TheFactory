using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GeradorCartas___Guildas.Services
{
    internal class DrawingService
    {
        private const float FONT_SCALE = 0.88f;
        private const float PADDING_PCT = 0.020f;
        private const float PAD_HAB_EXTRA = 0.010f;
        private const float LINE_SPACING = 0.92f;
        private const string FONT_FAMILY = "Segoe UI";

        private const float NAME_BOTTOM_RATIO = 0.75f;
        private const float NAME_LINESPACE = 0.75f;

        private static readonly HashSet<string> DefaultSingleLine = new(StringComparer.OrdinalIgnoreCase)
        { "name","id","cost","health","resistence","atack","damage","bravery","prep","body","strength" };

        private static readonly string[] ImageDirs = {
            @"assets\image", @"assets\images",
            @"Assets\Image", @"Assets\Images"
        };

        internal record FieldDef(
            string Name, double Xp, double Yp, double Wp, double Hp,
            string Align, float FontMin, float FontMax, bool Bold, bool SingleLine);

        internal List<FieldDef> LoadFields(string csvPath)
        {
            var list = new List<FieldDef>();
            if (!File.Exists(csvPath)) return list;

            var lines = File.ReadAllLines(csvPath);
            if (lines.Length <= 1) return list;

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(';');
                if (parts.Length < 2) parts = line.Split('\t');
                if (parts.Length < 9) continue;

                string name = parts[0].Trim();
                double xp = ParseDouble(parts[1]);
                double yp = ParseDouble(parts[2]);
                double wp = ParseDouble(parts[3]);
                double hp = ParseDouble(parts[4]);
                string align = (parts[5] ?? "").Trim().ToLowerInvariant();
                float fmin = (float)ParseDouble(parts[6]) * FONT_SCALE;
                float fmax = (float)ParseDouble(parts[7]) * FONT_SCALE;
                bool bold = ParseBool(parts[8]);

                bool singleLine = parts.Length >= 10
                    ? ParseBool(parts[9])
                    : DefaultSingleLine.Contains(name);

                xp = Clamp01(xp); yp = Clamp01(yp);
                wp = Math.Max(0.01, Clamp01(wp));
                hp = Math.Max(0.01, Clamp01(hp));
                if (fmin < 2) fmin = 2;
                if (fmax < 2) fmax = 2;
                if (fmax < fmin) (fmax, fmin) = (fmin, fmax);

                list.Add(new FieldDef(name, xp, yp, wp, hp, align, fmin, fmax, bold, singleLine));
            }
            return list;
        }

        internal Bitmap RenderCardBitmap(
            Bitmap template,
            List<FieldDef> fields,
            Func<string, string> valueResolver,
            int cardWidthPx,
            int cardHeightPx,
            float targetDpi)
        {
            var bmp = new Bitmap(cardWidthPx, cardHeightPx, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            bmp.SetResolution(targetDpi, targetDpi);

            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.Transparent);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // Fundo
            g.DrawImage(template,
                        new Rectangle(0, 0, cardWidthPx, cardHeightPx),
                        new Rectangle(0, 0, template.Width, template.Height),
                        GraphicsUnit.Pixel);

            foreach (var f in fields)
            {
                float rx = (float)(f.Xp * cardWidthPx);
                float ry = (float)(f.Yp * cardHeightPx);
                float rw = (float)(f.Wp * cardWidthPx);
                float rh = (float)(f.Hp * cardHeightPx);

                float pad = MathF.Min(rw, rh) * PADDING_PCT;
                if (f.Name.Equals("Hab1", StringComparison.OrdinalIgnoreCase) ||
                    f.Name.Equals("Hab2", StringComparison.OrdinalIgnoreCase))
                    pad += MathF.Min(rw, rh) * PAD_HAB_EXTRA;

                rx += pad; ry += pad; rw -= pad * 2f; rh -= pad * 2f;
                if (rw < 1f || rh < 1f) continue;

                var rect = new RectangleF(rx, ry, rw, rh);
                var style = f.Bold ? FontStyle.Bold : FontStyle.Regular;

                if (f.Name.Equals("Lore", StringComparison.OrdinalIgnoreCase)
                    || f.Name.Equals("Lore1", StringComparison.OrdinalIgnoreCase)
                    || f.Name.Equals("Lore2", StringComparison.OrdinalIgnoreCase))
                {
                    style |= FontStyle.Italic;
                }

                // ART (usa "Art" e "Id" do resolver)
                if (f.Name.Equals("Art", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        string art = valueResolver("Art");
                        string id = valueResolver("Id");
                        var artPath = ResolveArtPath(art, id);
                        if (!string.IsNullOrEmpty(artPath) && File.Exists(artPath))
                        {
                            using var artImg = Image.FromFile(artPath);
                            DrawImageCover(g, artImg, rect);
                        }
                    }
                    catch { }
                    continue;
                }

                string val = valueResolver(f.Name);
                if (string.IsNullOrWhiteSpace(val)) continue;

                if (f.Name.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    DrawNameTwoLines(g, rect, val, style, f.FontMax, f.FontMin);
                    continue;
                }

                using var font = FitFontAuto(g, val, FONT_FAMILY, f.FontMax, f.FontMin, rect, style, f.SingleLine, LINE_SPACING);
                DrawText(g, val, font, rect, f.Align, f.SingleLine, LINE_SPACING);
            }

            return bmp;
        }

        private static void DrawText(Graphics g, string text, Font font, RectangleF rect, string hAlign, bool singleLine, float lineSpacing)
        {
            if (singleLine)
            {
                var size = g.MeasureString(text, font, new SizeF(float.MaxValue, float.MaxValue), StringFormat.GenericTypographic);
                float x = hAlign switch
                {
                    "center" => rect.X + (rect.Width - size.Width) / 2f,
                    "right" => rect.Right - size.Width,
                    _ => rect.X
                };
                float y = rect.Y + (rect.Height - font.GetHeight(g)) / 2f;
                g.DrawString(text, font, Brushes.Black, new PointF(x, y), StringFormat.GenericTypographic);
                return;
            }

            var lines = WrapLines(g, text, font, rect.Width);
            float lineH = font.GetHeight(g) * lineSpacing;
            float totalH = lineH * lines.Count;
            float yStart = rect.Y + (rect.Height - totalH) / 2f;

            foreach (var line in lines)
            {
                var size = g.MeasureString(line, font, new SizeF(float.MaxValue, float.MaxValue), StringFormat.GenericTypographic);
                float x = hAlign switch
                {
                    "center" => rect.X + (rect.Width - size.Width) / 2f,
                    "right" => rect.Right - size.Width,
                    _ => rect.X
                };
                g.DrawString(line, font, Brushes.Black, new PointF(x, yStart), StringFormat.GenericTypographic);
                yStart += lineH;
            }
        }

        private static void DrawNameTwoLines(Graphics g, RectangleF rect, string nameValue, FontStyle style, float fontMax, float fontMin)
        {
            var (top, bottom) = SplitNameKeepComma(nameValue);

            var prevClip = g.Clip;
            g.SetClip(rect);
            try
            {
                if (string.IsNullOrWhiteSpace(bottom))
                {
                    using var f = new Font(FONT_FAMILY, Math.Max(2f, fontMax), style, GraphicsUnit.Point);
                    using var sf = new StringFormat(StringFormatFlags.NoWrap)
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };
                    g.DrawString(top, f, Brushes.Black, rect, sf);
                    return;
                }

                float sizeTop = Math.Max(2f, fontMax);
                float sizeBot = Math.Max(2f, fontMax * NAME_BOTTOM_RATIO);

                using var fontTop = new Font(FONT_FAMILY, sizeTop, style, GraphicsUnit.Point);
                using var fontBot = new Font(FONT_FAMILY, sizeBot, style, GraphicsUnit.Point);

                float ht = fontTop.GetHeight(g);
                float hb = fontBot.GetHeight(g);
                float offset = ht * NAME_LINESPACE;

                float blockH = Math.Max(ht, offset + hb);
                float yStart = rect.Y + (rect.Height - blockH) / 2f;

                var rectTop = new RectangleF(rect.X, yStart, rect.Width, ht);
                var rectBot = new RectangleF(rect.X, yStart + offset, rect.Width, hb);

                var sfCenter = new StringFormat(StringFormatFlags.NoWrap)
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Near,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                g.DrawString(top, fontTop, Brushes.Black, rectTop, sfCenter);
                g.DrawString(bottom, fontBot, Brushes.Black, rectBot, sfCenter);
            }
            finally { g.Clip = prevClip; }
        }

        private static Font FitFontAuto(Graphics g, string text, string family, float maxPt, float minPt, RectangleF rect, FontStyle style, bool singleLine, float lineSpacing)
        {
            const float WIDTH_FUDGE = 0.98f;
            const float HEIGHT_FUDGE = 0.98f;

            float lo = Math.Max(2f, Math.Min(minPt, maxPt));
            float hi = Math.Max(2f, Math.Max(minPt, maxPt));
            float best = lo;

            for (int i = 0; i < 20; i++)
            {
                float mid = (lo + hi) / 2f;
                using var f = new Font(family, mid, style, GraphicsUnit.Point);

                if (singleLine)
                {
                    var size = g.MeasureString(text, f, new SizeF(float.MaxValue, float.MaxValue), StringFormat.GenericTypographic);
                    float h = f.GetHeight(g);
                    if (size.Width <= rect.Width * WIDTH_FUDGE && h <= rect.Height * HEIGHT_FUDGE)
                    { best = mid; lo = mid; }
                    else { hi = mid; }
                }
                else
                {
                    var lines = WrapLines(g, text, f, rect.Width * WIDTH_FUDGE);
                    float lineH = f.GetHeight(g) * lineSpacing;
                    float totalH = lineH * lines.Count;
                    if (totalH <= rect.Height * HEIGHT_FUDGE)
                    { best = mid; lo = mid; }
                    else { hi = mid; }
                }
            }
            return new Font(family, best, style, GraphicsUnit.Point);
        }

        private static List<string> WrapLines(Graphics g, string text, Font font, float maxWidth)
        {
            var lines = new List<string>();
            foreach (var paragraph in (text ?? string.Empty).Replace("\r\n", "\n").Split('\n'))
            {
                var words = paragraph.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (words.Length == 0) { lines.Add(string.Empty); continue; }

                string current = words[0];
                for (int i = 1; i < words.Length; i++)
                {
                    string test = current + " " + words[i];
                    var size = g.MeasureString(test, font, new SizeF(float.MaxValue, float.MaxValue), StringFormat.GenericTypographic);
                    if (size.Width <= maxWidth) current = test;
                    else { lines.Add(current); current = words[i]; }
                }
                lines.Add(current);
            }
            return lines;
        }

        private static (string top, string bottom) SplitNameKeepComma(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return (string.Empty, string.Empty);
            int idx = name.IndexOf(',');
            if (idx < 0) return (name.Trim(), string.Empty);
            string top = name.Substring(0, idx + 1).TrimEnd();
            string bottom = name.Substring(idx + 1).TrimStart();
            return (top, bottom);
        }

        private static string ResolveArtPath(string art, string id)
        {
            IEnumerable<string> Candidates()
            {
                if (!string.IsNullOrWhiteSpace(art))
                    foreach (var d in ImageDirs) yield return Path.Combine(d, art);

                if (!string.IsNullOrWhiteSpace(id))
                    foreach (var d in ImageDirs)
                    {
                        yield return Path.Combine(d, $"{id}.png");
                        yield return Path.Combine(d, $"{id}.jpg");
                        yield return Path.Combine(d, $"{id}.jpeg");
                    }
            }
            return Candidates().FirstOrDefault(File.Exists);
        }

        private static void DrawImageCover(Graphics g, Image img, RectangleF dest)
        {
            float imgAspect = (float)img.Width / img.Height;
            float destAspect = dest.Width / dest.Height;

            RectangleF src;
            if (imgAspect > destAspect)
            {
                float srcH = img.Height;
                float srcW = srcH * destAspect;
                float sx = (img.Width - srcW) / 2f;
                src = new RectangleF(sx, 0, srcW, srcH);
            }
            else
            {
                float srcW = img.Width;
                float srcH = srcW / destAspect;
                float sy = (img.Height - srcH) / 2f;
                src = new RectangleF(0, sy, srcW, srcH);
            }
            g.DrawImage(img, dest, src, GraphicsUnit.Pixel);
        }

        private static double Clamp01(double v) => v < 0 ? 0 : (v > 1 ? 1 : v);

        private static double ParseDouble(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            s = s.Trim().Replace("%", "").Trim();
            s = s.Replace('\u00A0', ' ').Replace(" ", "");
            s = s.Replace(',', '.');
            double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v);
            return v;
        }

        private static bool ParseBool(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            return s.Trim().Equals("true", StringComparison.OrdinalIgnoreCase) || s.Trim().Equals("1");
        }
    }
}
