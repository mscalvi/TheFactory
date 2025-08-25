using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using System.Diagnostics;
using PdfSharpCore.Drawing.Layout.enums;
using System.Text.RegularExpressions;

namespace BingoCreator.Services
{
    internal class PrintingService
    {
        // Métodos de Suporte
            // Desenha texto com quebra de linha dentro do retângulo, centralizado (H e V).
        private static void DrawWrappedCenteredText(XGraphics gfx, string text, XFont font, XRect rect)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            // Altura de linha (um pequeno leading para não “grudar”)
            double lineH = gfx.MeasureString("Ag", font).Height * 1.1;

            // Quebra por largura
            var lines = WrapByWidth(gfx, text, font, rect.Width);

            // Limita pela altura disponível
            int maxLines = Math.Max(1, (int)Math.Floor(rect.Height / lineH));
            if (lines.Count > maxLines)
            {
                lines = lines.GetRange(0, maxLines);
                // adiciona reticências na última linha se precisar
                string last = lines[^1];
                while (last.Length > 0 && gfx.MeasureString(last + "…", font).Width > rect.Width)
                    last = last[..^1];
                lines[^1] = last.Length > 0 ? last + "…" : "…";
            }

            // Centraliza verticalmente
            double totalH = lines.Count * lineH;
            double y = rect.Y + (rect.Height - totalH) / 2.0;

            // Desenha cada linha centralizada horizontalmente
            foreach (var ln in lines)
            {
                gfx.DrawString(ln, font, XBrushes.Black,
                    new XRect(rect.X, y, rect.Width, lineH), XStringFormats.TopCenter);
                y += lineH;
            }
        }

            // Quebra “por palavras”; se a palavra for maior que a largura, quebra por caracteres.
        private static List<string> WrapByWidth(XGraphics gfx, string text, XFont font, double maxW)
        {
            var tokens = Regex.Split(text, @"(\s+)"); // preserva espaços
            var lines = new List<string>();
            var sb = new StringBuilder();

            foreach (var tok in tokens)
            {
                string candidate = sb.Length == 0 ? tok.TrimStart() : sb.ToString() + tok;
                if (gfx.MeasureString(candidate, font).Width <= maxW)
                {
                    sb.Clear(); sb.Append(candidate);
                    continue;
                }

                // fecha a linha atual (se tiver algo)
                if (sb.Length > 0)
                {
                    lines.Add(sb.ToString().TrimEnd());
                    sb.Clear();
                }

                // token sozinho não cabe: quebra por caracteres
                string t = tok.Trim();
                if (t.Length == 0) continue;

                int start = 0;
                while (start < t.Length)
                {
                    int len = 1;
                    while (start + len <= t.Length &&
                           gfx.MeasureString(t.AsSpan(start, len).ToString(), font).Width <= maxW)
                        len++;
                    if (len > 1) len--; // último que coube
                    lines.Add(t.Substring(start, len));
                    start += len;
                }
            }

            if (sb.Length > 0)
                lines.Add(sb.ToString().TrimEnd());

            return lines;
        }

        private static void DrawCropMarks(XGraphics gfx, double margin, double pageW, double pageH, double markLen = 10)
        {
            var cropPen = new XPen(XColors.Gray, 0.6);

            // topo-esquerda
            gfx.DrawLine(cropPen, margin - markLen, margin, margin, margin);
            gfx.DrawLine(cropPen, margin, margin - markLen, margin, margin);

            // topo-direita
            gfx.DrawLine(cropPen, pageW - margin + markLen, margin, pageW - margin, margin);
            gfx.DrawLine(cropPen, pageW - margin, margin - markLen, pageW - margin, margin);

            // base-esquerda
            gfx.DrawLine(cropPen, margin - markLen, pageH - margin, margin, pageH - margin);
            gfx.DrawLine(cropPen, margin, pageH - margin + markLen, margin, pageH - margin);

            // base-direita
            gfx.DrawLine(cropPen, pageW - margin + markLen, pageH - margin, pageW - margin, pageH - margin);
            gfx.DrawLine(cropPen, pageW - margin, pageH - margin + markLen, pageW - margin, pageH - margin);
        }

        private static string SanitizeFileName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder(name.Length);
            foreach (var ch in name)
                sb.Append(invalid.Contains(ch) ? '_' : ch);
            return sb.ToString();
        }

        // Imprimir Cartelas
        public static void PrintCards5x5(string setName, List<List<DataRow>> allCards, int cardsQnt, string cardsTitle, string cardsEnd, string themeKey, string headerKey, string modelKey)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string fileName = $"Cartelas - {setName}.pdf";
            string filePath = Path.Combine(desktop, fileName);

            var theme = ThemeCatalog.Get(themeKey);
            DesignService.Header5x5 = string.IsNullOrWhiteSpace(headerKey) ? "SORTE" : headerKey.Trim();
            DesignService.CellStyle = string.IsNullOrWhiteSpace(modelKey) ? "SQUARE" : modelKey.Trim();

            var document = new PdfDocument();
            document.Info.Title = $"Cartelas - {cardsTitle}";

            const double margin = 40;
            const double cellHeight = 40;
            double pageWidth, pageHeight;
            double cardWidth = 0;
            double cardHeight = cellHeight * 8;

            var titleFont = DesignService.CreateFont(theme.FontTitle, 17, XFontStyle.Bold);
            var headerFont = DesignService.CreateFont(theme.FontTitle, 15, XFontStyle.Bold);
            var compFont = DesignService.CreateFont(theme.FontBody, 10, XFontStyle.Regular);
            var footerFont = DesignService.CreateFont(theme.FontTitle, 12, XFontStyle.Bold);
            var numberFont = DesignService.CreateFont(theme.FontTitle, 12, XFontStyle.Bold);

            var pen = DesignService.Pen(theme, 0.8);

            XGraphics gfx = null;
            PdfPage page = null;

            for (int i = 0; i < cardsQnt; i++)
            {
                if (i % 2 == 0)
                {
                    page = document.AddPage();
                    page.Size = PdfSharpCore.PageSize.A4;
                    pageWidth = page.Width;
                    pageHeight = page.Height;
                    cardWidth = pageWidth - 2 * margin;

                    gfx = XGraphics.FromPdfPage(page);
                }

                double y0 = margin + (i % 2) * (cardHeight + 20);

                DrawCards5x5(
                    gfx, margin, y0, cardWidth, cardHeight,
                    allCards[i], i + 1, cardsTitle, cardsEnd,
                    theme, pen, titleFont, headerFont, compFont, footerFont, numberFont);
            }

            document.Save(filePath);
            MessageBox.Show($"Cartelas 4×4 salvas no Desktop:\n{fileName}", "Sucesso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void PrintCards4x4(string setName, List<List<DataRow>> allCards, int cardsQnt, string cardsTitle, string cardsEnd, string themeKey, string modelKey)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"Cartelas - {setName}.pdf";
            string filePath = Path.Combine(desktop, fileName);

            var theme = ThemeCatalog.Get(themeKey);
            DesignService.CellStyle = string.IsNullOrWhiteSpace(modelKey) ? "SQUARE" : modelKey.Trim();

            var document = new PdfDocument();
            document.Info.Title = $"Cartelas 4×4 – {cardsTitle}";

            const double margin = 36;
            const double gap = 14;

            XGraphics gfx = null;
            PdfPage page = null;
            double pageWidth = 0, pageHeight = 0, cardWidth = 0, cardHeight = 0;

            var titleFont = DesignService.CreateFont(theme.FontTitle, 16, XFontStyle.Bold);
            var compFont = DesignService.CreateFont(theme.FontBody, 10, XFontStyle.Regular);
            var footerFont = DesignService.CreateFont(theme.FontTitle, 11, XFontStyle.Bold);
            var numberFont = DesignService.CreateFont(theme.FontTitle, 11, XFontStyle.Bold);

            var pen = DesignService.Pen(theme, 0.8);   // bordas na cor do tema

            for (int i = 0; i < cardsQnt; i++)
            {
                // ✅ nova página a cada 3 cartelas
                if (i % 3 == 0)
                {
                    page = document.AddPage();
                    page.Size = PdfSharpCore.PageSize.A4; // retrato
                    gfx = XGraphics.FromPdfPage(page);

                    pageWidth = page.Width;
                    pageHeight = page.Height;

                    cardWidth = pageWidth - 2 * margin;
                    // ✅ 3 cartelas empilhadas (2 gaps entre elas)
                    double available = pageHeight - 2 * margin - 2 * gap;
                    cardHeight = available / 3.0;
                }

                int rowInPage = i % 3; // 0..2
                double y0 = margin + rowInPage * (cardHeight + gap);

                DrawCards4x4(gfx, margin, y0, cardWidth, cardHeight,
                             allCards[i], i + 1, cardsTitle, cardsEnd,
                             theme, pen, titleFont, compFont, footerFont, numberFont);
            }

            document.Save(filePath);
            MessageBox.Show($"Cartelas 4×4 salvas no Desktop:\n{fileName}", "Sucesso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Desenho das Cartelas
        private static void DrawCards5x5(XGraphics gfx, double x, double y, double width, double height, List<DataRow> cardsElements, int cardNumber, string titleText, string footerText, Theme theme, XPen pen, XFont titleFont, XFont headerFont, XFont elementFont, XFont footerFont, XFont numberFont)
        {
            double padding = 8;
            double cellH = 40;     // altura fixa que você usa hoje
            double headerH = cellH;

            var cardRect = new XRect(x, y, width, height);
            DesignService.DrawCardBackground(gfx, cardRect, theme, cornerRadius: 12);

            // Faixa de título
            var titleRect = new XRect(x + padding, y + padding, width - 2 * padding, headerH - padding);
            DesignService.DrawHeaderBand(gfx, titleRect, theme, titleText, titleFont);

            // Cabeçalho (SORTE/BINGO) conforme seleção
            string[] headers = DesignService.GetHeader5x5Letters();
            double cellW = width / 5.0;

            for (int j = 0; j < 5; j++)
            {
                var r = new XRect(x + j * cellW, y + headerH, cellW, cellH);
                var headOverlay = new XSolidBrush(DesignService.WithOpacity(theme.HeaderBg, DesignService.CellOverlayOpacity));
                gfx.DrawRectangle(headOverlay, r);
                gfx.DrawRectangle(pen, r);
                gfx.DrawString(headers[j], headerFont, DesignService.TextBrush(theme), r, XStringFormats.Center);

            }

            // Grid 5×5 com estilo de célula
            var modelKey = DesignService.CellStyle; // "SQUARE" ou "ROUNDED"
            double radius = 9;

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    double xx = x + col * cellW;
                    double yy = y + 2 * cellH + row * cellH;
                    var cell = new XRect(xx, yy, cellW, cellH);

                    DesignService.FillCellBackground(gfx, cell, theme, modelKey, radius);
                    DesignService.StrokeCellBorder(gfx, cell, theme, pen, modelKey, radius);

                    int idx = col * 5 + row;
                    if (idx < cardsElements.Count)
                    {
                        string name = cardsElements[idx]?["Name"]?.ToString() ?? string.Empty;
                        DrawWrappedCenteredText(gfx, name, elementFont,
                            new XRect(cell.X + 3, cell.Y + 3, cell.Width - 6, cell.Height - 6));
                    }
                }
            }

            // Rodapé (faixa + texto responsivo em 2 linhas, limitado a 80% do título)
            double footerY = y + 7 * cellH;
            double bandPad = padding;

            var bandRect = new XRect(x + bandPad, footerY + bandPad / 2.0, width - 2 * bandPad, cellH - bandPad);

            // faixa com transparência (como no header)
            var footerBg = DesignService.WithOpacity(theme.HeaderBg, DesignService.FooterBandOpacity);
            gfx.DrawRectangle(new XSolidBrush(footerBg), bandRect);

            // 72% / 28% (mensagem / número)
            double leftW = bandRect.Width * 0.72;
            double rightW = bandRect.Width - leftW;

            var footerRect = new XRect(bandRect.X, bandRect.Y, leftW, bandRect.Height);
            var numRect = new XRect(footerRect.Right, bandRect.Y, rightW, bandRect.Height);

            // limite da mensagem: máx 80% do título
            double footerMaxPt = Math.Min(Math.Min(cellH * 0.60, 18), titleFont.Size * 0.80);

            // fonte da MENSAGEM (até 2 linhas)
            var footerFontFit = DesignService.FitFontForTwoLines(
                gfx, footerText, theme.FontTitle, XFontStyle.Bold,
                maxWidth: footerRect.Width - 6,
                maxHeight: footerRect.Height - 4,
                maxPointSize: footerMaxPt,
                minPointSize: 9
            );

            // desenha a MENSAGEM (2 linhas máx.)
            DrawWrappedCenteredText(
                gfx,
                footerText,
                footerFontFit,
                new XRect(footerRect.X + 3, footerRect.Y + 2, footerRect.Width - 6, footerRect.Height - 4)
            );

            // NÚMERO = 80% do tamanho FINAL da mensagem (e ainda respeita a caixa)
            string idText = $"Cartela {cardNumber:0000}";
            double numTargetPt = footerFontFit.Size * 0.80;                    // << regra pedida
            double numberMaxPt = Math.Min(numTargetPt, Math.Min(cellH * 0.55, 16));

            var numberFontFit = DesignService.FitFontToRect(
                gfx, idText, theme.FontTitle, XFontStyle.Bold,
                maxWidth: numRect.Width - 6, maxHeight: numRect.Height - 4,
                maxPointSize: numberMaxPt, minPointSize: 8
            );

            // textos em preto
            gfx.DrawString(idText, numberFontFit, DesignService.TextBrush(theme), numRect, XStringFormats.Center);

        }

        private static void DrawCards4x4( XGraphics gfx, double x, double y, double width, double height, List<DataRow> cardsElements, int cardNumber, string titleText, string footerText, Theme theme, XPen pen, XFont titleFont, XFont elementFont, XFont footerFont, XFont numberFont)
        {
            double padding = 8;
            double cellH = height / 6.0; // 1 título + 4 grid + 1 rodapé
            double headerH = cellH;

            var cardRect = new XRect(x, y, width, height);
            DesignService.DrawCardBackground(gfx, cardRect, theme, cornerRadius: 12);

            // Cabeçalho
            var titleRect = new XRect(x + padding, y + padding, width - 2 * padding, headerH - padding);
            DesignService.DrawHeaderBand(gfx, titleRect, theme, titleText, titleFont);

            // Grid 4×4 com estilo de célula
            double gridTop = y + headerH;
            double cellW = width / 4.0;

            var modelKey = DesignService.CellStyle; // "SQUARE" ou "ROUNDED"
            double radius = 9;

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    double xx = x + col * cellW;
                    double yy = gridTop + row * cellH;
                    var cell = new XRect(xx, yy, cellW, cellH);

                    DesignService.FillCellBackground(gfx, cell, theme, modelKey, radius);
                    DesignService.StrokeCellBorder(gfx, cell, theme, pen, modelKey, radius);

                    int idx = row * 4 + col;
                    if (idx < cardsElements.Count)
                    {
                        string name = cardsElements[idx]?["Name"]?.ToString() ?? string.Empty;
                        DrawWrappedCenteredText(gfx, name, elementFont,
                            new XRect(cell.X + 3, cell.Y + 3, cell.Width - 6, cell.Height - 6));
                    }
                }
            }

            // Rodapé (faixa + texto responsivo em 2 linhas, limitado a 80% do título)
            double footerY = y + headerH + 4 * cellH;
            double bandPad = padding;

            var bandRect = new XRect(x + bandPad, footerY + bandPad / 2.0, width - 2 * bandPad, cellH - bandPad);

            // faixa com transparência
            var footerBg = DesignService.WithOpacity(theme.HeaderBg, DesignService.FooterBandOpacity);
            gfx.DrawRectangle(new XSolidBrush(footerBg), bandRect);

            double leftW = bandRect.Width * 0.72;
            double rightW = bandRect.Width - leftW;

            var footerRect = new XRect(bandRect.X, bandRect.Y, leftW, bandRect.Height);
            var numRect = new XRect(footerRect.Right, bandRect.Y, rightW, bandRect.Height);

            // máx 80% do título
            double footerMaxPt = Math.Min(Math.Min(cellH * 0.60, 16), titleFont.Size * 0.80);

            // fonte da MENSAGEM (até 2 linhas)
            var footerFontFit = DesignService.FitFontForTwoLines(
                gfx, footerText, theme.FontTitle, XFontStyle.Bold,
                maxWidth: footerRect.Width - 6,
                maxHeight: footerRect.Height - 4,
                maxPointSize: footerMaxPt,
                minPointSize: 9
            );

            DrawWrappedCenteredText(
                gfx,
                footerText,
                footerFontFit,
                new XRect(footerRect.X + 3, footerRect.Y + 2, footerRect.Width - 6, footerRect.Height - 4)
            );

            // NÚMERO = 80% do tamanho FINAL da mensagem
            string idText = $"Cartela {cardNumber:0000}";
            double numTargetPt = footerFontFit.Size * 0.80;
            double numberMaxPt = Math.Min(numTargetPt, Math.Min(cellH * 0.55, 14));

            var numberFontFit = DesignService.FitFontToRect(
                gfx, idText, theme.FontTitle, XFontStyle.Bold,
                maxWidth: numRect.Width - 6, maxHeight: numRect.Height - 4,
                maxPointSize: numberMaxPt, minPointSize: 8
            );

            gfx.DrawString(idText, numberFontFit, DesignService.TextBrush(theme), numRect, XStringFormats.Center);

        }


        // Imprimir Listas
        public static void PrintList5(string setName, List<DataRow> groupB, List<DataRow> groupI, List<DataRow> groupN, List<DataRow> groupG, List<DataRow> groupO)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string fileName = $"Lista - {setName}.pdf";
            string filePath = Path.Combine(desktop, fileName);

            PdfDocument document = new PdfDocument();
            document.Info.Title = $"Lista de Elementos {setName}";

            XFont font = new XFont("Segoe UI", 14);

            var groups = new Dictionary<string, List<DataRow>>
            {
                { "Coluna B", groupB },
                { "Coluna I", groupI },
                { "Coluna N", groupN },
                { "Coluna G", groupG },
                { "Coluna O", groupO }
            };

            int index = 1;
            const double margin = 40;
            const double lineSpacing = 6;

            foreach (var kv in groups)
            {
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                double y = margin;

                gfx.DrawString(kv.Key,
                               new XFont("Segoe UI", 18),
                               XBrushes.Black,
                               new XRect(margin, y, page.Width - 2 * margin, 20),
                               XStringFormats.Center);
                y += 24;

                foreach (var row in kv.Value)
                {
                    string text = $"{index}- {row["CardName"]}";
                    gfx.DrawString(text, font, XBrushes.Black,
                                   new XRect(margin, y, page.Width - 2 * margin, font.Height),
                                   XStringFormats.TopLeft);
                    y += font.GetHeight() + lineSpacing;
                    index++;

                    if (y + font.GetHeight() + margin > page.Height)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        y = margin;
                    }
                }
            }

            document.Save(filePath);
            MessageBox.Show($"Lista salva no Desktop:\n{fileName}", "Sucesso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void PrintList4(string setName, List<DataRow> elementsList, string themeKey, string preferColumn = "CardName")
        {
            // tenta usar CardName; se não existir/estiver vazio, usa Name
            static string GetName(DataRow r, string preferColumn)
            {
                string TryCol(string col) =>
                    (r.Table?.Columns.Contains(col) == true && r[col] != DBNull.Value) ? r[col].ToString() : null;

                return TryCol(preferColumn) ?? TryCol("Name") ?? string.Empty;
            }

            var items = elementsList
                .Select(r => GetName(r, preferColumn))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"Lista - {setName}.pdf";
            string filePath = Path.Combine(desktop, fileName);

            var document = new PdfDocument();
            document.Info.Title = $"Relação – {setName}";

            const double margin = 40;
            const double colGap = 20;

            var theme = ThemeCatalog.Get(themeKey);
            var titleFont = DesignService.CreateFont(theme.FontTitle, 18, XFontStyle.Bold);
            var itemFont = DesignService.CreateFont(theme.FontBody, 12, XFontStyle.Regular);

            var textBrush = DesignService.TextBrush(theme);

            PdfPage page = document.AddPage();
            page.Size = PdfSharpCore.PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);

            double pageW = page.Width;
            double pageH = page.Height;

            // título
            var titleRect = new XRect(margin, margin, pageW - 2 * margin, 28);
            gfx.DrawString($"Relação de Elementos – {setName}", titleFont,
               DesignService.AccentBrush(theme), titleRect, XStringFormats.Center);

            // área útil abaixo do título
            double top = titleRect.Bottom + 12;
            double usableH = pageH - margin - top;

            // colunas
            double colW = (pageW - 2 * margin - colGap) / 2.0;
            var col1 = new XRect(margin, top, colW, usableH);
            var col2 = new XRect(margin + colW + colGap, top, colW, usableH);

            // desenho linha a linha, com quebra e paginação
            int idx = 0;
            DrawListColumn(gfx, items, ref idx, col1, itemFont, textBrush);
            DrawListColumn(gfx, items, ref idx, col2, itemFont, textBrush);

            while (idx < items.Count)
            {
                page = document.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;
                gfx = XGraphics.FromPdfPage(page);

                // título por página
                titleRect = new XRect(margin, margin, page.Width - 2 * margin, 28);
                gfx.DrawString($"Relação de Elementos – {setName}", titleFont, XBrushes.Black, titleRect, XStringFormats.Center);

                top = titleRect.Bottom + 12;
                usableH = page.Height - margin - top;
                colW = (page.Width - 2 * margin - colGap) / 2.0;
                col1 = new XRect(margin, top, colW, usableH);
                col2 = new XRect(margin + colW + colGap, top, colW, usableH);

                DrawListColumn(gfx, items, ref idx, col1, itemFont, textBrush);
                DrawListColumn(gfx, items, ref idx, col2, itemFont, textBrush);
            }

            document.Save(filePath);
            MessageBox.Show($"Lista salva no Desktop:\n{fileName}", "Sucesso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void DrawListColumn(XGraphics gfx, IList<string> items, ref int idx, XRect col, XFont font, XBrush textBrush)
        {
            double lineH = gfx.MeasureString("Ag", font).Height * 1.35;
            double y = col.Y;

            while (idx < items.Count)
            {
                string line = $"{idx + 1:00} – {items[idx]}";
                var wrapped = WrapByWidth(gfx, line, font, col.Width);

                double blockH = wrapped.Count * lineH;
                if (y + blockH > col.Bottom) break;

                foreach (var ln in wrapped)
                {
                    gfx.DrawString(ln, font, textBrush,  // << usar a brush recebida
                        new XRect(col.X, y, col.Width, lineH), XStringFormats.TopLeft);
                    y += lineH;
                }
                idx++;
            }
        }


        // Imprimir Papéis de Sorteio

        public static void PrintCutPapers(string setName, List<DataRow> elementsList, string themeKey, int copiesPerItem = 1, int cols = 4, int rows = 10, string preferColumn = "CardName", bool showCropMarks = true)
        {
            // 1) Extrai nomes da lista (CardName > Name)
            static string GetName(DataRow r, string preferCol)
            {
                string TryCol(string col) =>
                    (r.Table?.Columns.Contains(col) == true && r[col] != DBNull.Value)
                        ? r[col]?.ToString()
                        : null;

                return TryCol(preferCol) ?? TryCol("Name") ?? string.Empty;
            }

            var baseItems = elementsList
                .Select(r => GetName(r, preferColumn))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            // 2) Replica para múltiplas cópias por item
            copiesPerItem = Math.Max(1, copiesPerItem);
            var items = new List<string>(baseItems.Count * copiesPerItem);
            foreach (var s in baseItems)
                for (int k = 0; k < copiesPerItem; k++)
                    items.Add(s);

            // 3) Setup do PDF
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string fileName = $"Fichas - {SanitizeFileName(setName)}.pdf";
            string filePath = Path.Combine(desktop, fileName);

            var doc = new PdfDocument();
            doc.Info.Title = $"Fichas de Sorteio – {setName}";

            const double margin = 36;     // ~0,5"
            const double gapYTitle = 8;   // espaço após o título

            var theme = ThemeCatalog.Get(themeKey);
            var titleFont = DesignService.CreateFont(theme.FontTitle, 14, XFontStyle.Bold);
            var cellFont = DesignService.CreateFont(theme.FontBody, 11, XFontStyle.Regular);
            var pen = DesignService.Pen(theme, 0.6);

            int perPage = Math.Max(1, cols) * Math.Max(1, rows);
            int total = items.Count;
            int pages = (int)Math.Ceiling(total / (double)perPage);

            int idx = 0;
            for (int p = 0; p < pages; p++)
            {
                var page = doc.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;
                var gfx = XGraphics.FromPdfPage(page);

                double pageW = page.Width;
                double pageH = page.Height;

                // Título
                var titleRect = new XRect(margin, margin, pageW - 2 * margin, 22);
                gfx.DrawString($"Fichas de Sorteio – {setName}", titleFont,
               DesignService.AccentBrush(theme), titleRect, XStringFormats.Center);

                // Área do grid
                double top = titleRect.Bottom + gapYTitle;
                double gridW = pageW - 2 * margin;
                double gridH = pageH - margin - top;

                double cellW = gridW / cols;
                double cellH = gridH / rows;

                // Grid + texto
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        double x = margin + c * cellW;
                        double y = top + r * cellH;

                        var cellRect = new XRect(x, y, cellW, cellH);
                        gfx.DrawRectangle(pen, cellRect);

                        if (idx < total)
                        {
                            DrawWrappedCenteredText(gfx, items[idx], cellFont, new XRect(cellRect.X + 4, cellRect.Y + 3, cellRect.Width - 8, cellRect.Height - 6));
                            idx++;
                        }
                    }
                }

                if (showCropMarks)
                    DrawCropMarks(gfx, margin, pageW, pageH);
            }

            doc.Save(filePath);
            System.Windows.Forms.MessageBox.Show(
                $"Fichas salvas no Desktop:\n{fileName}",
                "Sucesso",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Information
            );
        }

    }
}
