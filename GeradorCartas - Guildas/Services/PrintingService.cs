using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using GeradorCartas___Guildas.Models;

namespace GeradorCartas___Guildas.Services
{
    internal class PrintingService
    {
        // ==========================
        // Config de assets e saída
        // ==========================
        private const string TemplatesDir = @"assets\templates";
        private static readonly string TplType1Png = Path.Combine(TemplatesDir, "design_CharacterModel1.png");
        private static readonly string TplType2Png = Path.Combine(TemplatesDir, "design_CharacterModel2.png");
        private static readonly string TplType3Png = Path.Combine(TemplatesDir, "design_CharacterModel3.png");
        private static readonly string FieldsType1Csv = Path.Combine(TemplatesDir, "fields_CharacterModel1.csv");
        private static readonly string FieldsType2Csv = Path.Combine(TemplatesDir, "fields_CharacterModel2.csv");
        private static readonly string FieldsType3Csv = Path.Combine(TemplatesDir, "fields_CharacterModel3.csv");

        private const string OutputDir = "output";
        private const double TargetDpi = 300.0;

        // ======================================================
        // ENTRADA → monta PDF A4 com grade 3x3 de cartas (63×88)
        // ======================================================
        public void PrintCharacterCards(List<CharacterModel> chars, string outputName = null, string title = null)
        {
            if (chars == null || chars.Count == 0)
                throw new InvalidOperationException("Nenhum personagem para gerar.");

            var drawing = new DrawingService();

            // 1) CSVs por modelo
            var fields1 = drawing.LoadFields(FieldsType1Csv);
            var fields2 = drawing.LoadFields(FieldsType2Csv);
            var fields3 = drawing.LoadFields(FieldsType3Csv);
            if (fields1.Count == 0) throw new InvalidDataException($"CSV vazio ou inválido: {FieldsType1Csv}");
            if (fields2.Count == 0) throw new InvalidDataException($"CSV vazio ou inválido: {FieldsType2Csv}");
            if (fields3.Count == 0) throw new InvalidDataException($"CSV vazio ou inválido: {FieldsType3Csv}");

            // 2) Templates PNG
            using var img1 = (Bitmap)Image.FromFile(TplType1Png);
            using var img2 = (Bitmap)Image.FromFile(TplType2Png);
            using var img3 = (Bitmap)Image.FromFile(TplType3Png);

            // 3) Documento PDF
            var pdf = new PdfDocument
            {
                Info = { Title = string.IsNullOrWhiteSpace(title) ? "Guildas - Cartas (Personagens)" : title }
            };

            // 4) Medidas físicas (A4; carta MTG 63 × 88 mm)
            const double mmToPt = 72.0 / 25.4;
            double pageWmm = 210, pageHmm = 297;  // A4
            double cardWmm = 63, cardHmm = 88;    // Carta
            double marginMm = 10, gapMm = 5;
            int cols = 3, rows = 3;

            double pageWpt = pageWmm * mmToPt, pageHpt = pageHmm * mmToPt;
            double marginPt = marginMm * mmToPt, gapPt = gapMm * mmToPt;
            double cardWpt = cardWmm * mmToPt, cardHpt = cardHmm * mmToPt;

            // Bitmap alvo de cada carta em pixels (300 DPI)
            int cardWpx = (int)Math.Round(cardWmm / 25.4 * TargetDpi);
            int cardHpx = (int)Math.Round(cardHmm / 25.4 * TargetDpi);

            // 5) Grade 3×3 (destinos em pontos)
            var cellRects = BuildGridRects(cols, rows, marginPt, gapPt, cardWpt, cardHpt);
            CenterGridInPage(cellRects, pageWpt, pageHpt, marginPt);

            // 6) Páginas
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

                    // Seleciona modelo (1, 2 ou 3)
                    int model = DetectModel(ch);
                    Bitmap tpl;
                    List<DrawingService.FieldDef> fields;
                    switch (model)
                    {
                        case 2: tpl = img2; fields = fields2; break;
                        case 3: tpl = img3; fields = fields3; break;
                        default: tpl = img1; fields = fields1; break;
                    }

                    // Renderiza a carta em bitmap 300 DPI via DrawingService
                    using var bmp = drawing.RenderCardBitmap(tpl, fields, ch, cardWpx, cardHpx, (float)TargetDpi);

                    // Insere no PDF no retângulo da célula
                    using var ms = new MemoryStream();
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;
                    using var ximg = XImage.FromStream(() => new MemoryStream(ms.ToArray()));
                    gfx.DrawImage(ximg, rect.X, rect.Y, rect.Width, rect.Height);
                }
            }

            // 7) Salva PDF
            Directory.CreateDirectory(OutputDir);
            string name = string.IsNullOrWhiteSpace(outputName)
                ? $"Characters_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                : $"{Sanitize(outputName)}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string outPath = Path.Combine(OutputDir, name);

            pdf.Save(outPath);
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = outPath, UseShellExecute = true });
            }
            catch { }
            pdf.Close();
        }

        // =========================================
        // MODELO → decide 1/2/3 conforme dados
        // =========================================
        private static int DetectModel(CharacterModel c)
        {
            bool hasHab2 = !string.IsNullOrWhiteSpace(c.Hab2);
            if (!hasHab2) return 1;     // Modelo 1: só Hab1
            if (c.HasPrep) return 2;    // Modelo 2: Hab1 + Hab2 + Prep
            return 3;                   // Modelo 3: Hab1 + Hab2 (sem Prep)
        }

        // =========================================
        // GRADE → retângulos e centralização
        // =========================================
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

        // =========================================
        // UTILS
        // =========================================
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
    }
}
