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
using PdfSharpCore.Drawing.Layout.enums;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BingoCreator.Services
{
    internal static class DesignService
    {
        private static XImage? _logo;  // cache em memória

        public static void UseDefaultLogo(string relativePath = "assets\\logo.png")
        {
            try
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var full = Path.Combine(baseDir, relativePath);
                SetLogo(File.Exists(full) ? full : null);  // se não existir, volta para a “bolinha”
            }
            catch
            {
                SetLogo(null);
            }
        }

        // Passe um caminho .png/.jpg. Se null ou inválido, limpa o logo.
        public static void SetLogo(string? path)
        {
            _logo?.Dispose();
            _logo = null;

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return;

            // evita lock no arquivo: carrega em memória
            var bytes = File.ReadAllBytes(path);
            var ms = new MemoryStream(bytes);
            _logo = XImage.FromStream(() => new MemoryStream(bytes));
        }

        public static void ClearLogo()
        {
            _logo?.Dispose();
            _logo = null;
        }

        // cria XFont com embed garantindo (fallback para Arial/Segoe se a fonte não estiver instalada)
        public static XFont CreateFont(string family, double size, XFontStyle style)
        {
            try
            {
                var opts = new XPdfFontOptions(PdfFontEncoding.Unicode); // sua versão aceita 1 argumento
                return new XFont(family, size, style, opts);
            }
            catch
            {
                var fb = OperatingSystem.IsWindows() ? "Segoe UI" : "Arial";
                var opts = new XPdfFontOptions(PdfFontEncoding.Unicode);
                return new XFont(fb, size, style, opts);
            }
        }

        public static void DrawCardBackground(XGraphics gfx, XRect rect, Theme theme, double cornerRadius = 0)
        {
            var bg = new XSolidBrush(theme.Primary);
            if (cornerRadius <= 0)
            {
                gfx.DrawRectangle(bg, rect);
            }
            else
            {
                // canto arredondado simples (aproximação por quatro círculos + retângulos)
                double r = cornerRadius;
                var full = rect;

                // retângulo central
                var mid = new XRect(full.X + r, full.Y, full.Width - 2 * r, full.Height);
                gfx.DrawRectangle(bg, mid);

                // retângulos laterais
                var left = new XRect(full.X, full.Y + r, r, full.Height - 2 * r);
                var right = new XRect(full.X + full.Width - r, full.Y + r, r, full.Height - 2 * r);
                gfx.DrawRectangle(bg, left);
                gfx.DrawRectangle(bg, right);

                // quatro cantos
                gfx.DrawEllipse(bg, full.X, full.Y, 2 * r, 2 * r);
                gfx.DrawEllipse(bg, full.X + full.Width - 2 * r, full.Y, 2 * r, 2 * r);
                gfx.DrawEllipse(bg, full.X, full.Y + full.Height - 2 * r, 2 * r, 2 * r);
                gfx.DrawEllipse(bg, full.X + full.Width - 2 * r, full.Y + full.Height - 2 * r, 2 * r, 2 * r);
            }
        }

        public static void DrawHeaderBand(XGraphics gfx, XRect rect, Theme theme, string title, XFont titleFont)
        {
            // faixa de fundo
            gfx.DrawRectangle(new XSolidBrush(theme.HeaderBg), rect);

            // área reservada à esquerda para ícone/logo
            double leftSlot = 0;

            if (_logo != null)
            {
                // escala proporcional ao retângulo do cabeçalho
                double maxH = Math.Max(0, rect.Height - 6); // respiro
                double ratio = _logo.PointWidth / _logo.PointHeight;   // unidades já em "points" do PDF
                double logoH = Math.Min(maxH, _logo.PointHeight);
                double logoW = logoH * ratio;

                double lx = rect.X + 6;
                double ly = rect.Y + (rect.Height - logoH) / 2.0;

                gfx.DrawImage(_logo, lx, ly, logoW, logoH);
                leftSlot = logoW + 12; // reserva espaço do texto
            }
            else
            {
                // fallback: “bolha”
                double size = Math.Min(rect.Height - 4, 24);
                double cx = rect.X + 8 + size / 2;
                double cy = rect.Y + rect.Height / 2;

                gfx.DrawEllipse(new XSolidBrush(theme.Accent), cx - size / 2, cy - size / 2, size, size);
                gfx.DrawEllipse(new XSolidBrush(theme.HeaderBg), cx - size / 3, cy - size / 3, (2 * size) / 3, (2 * size) / 3);
                leftSlot = size + 12;
            }

            // título centralizado no espaço restante (sem sobrepor logo)
            var textRect = new XRect(rect.X + leftSlot, rect.Y, rect.Width - leftSlot - 6, rect.Height);
            gfx.DrawString(title, titleFont, new XSolidBrush(theme.Text), textRect, XStringFormats.Center);
        }

        public static XPen Pen(Theme theme, double width = 0.8) =>
            new XPen(theme.Border, width);

        public static XBrush TextBrush(Theme theme) => new XSolidBrush(theme.Text);
        public static XBrush AccentBrush(Theme theme) => new XSolidBrush(theme.Accent);
    }
}
