using System;
using System.Collections.Generic;
using PdfSharpCore.Drawing;

namespace BingoCreator.Services
{
    internal static class ThemeCatalog
    {
        static XColor Hex(string hex, double alpha = 1)
        {
            hex = hex.TrimStart('#');
            byte r = Convert.ToByte(hex.Substring(0, 2), 16);
            byte g = Convert.ToByte(hex.Substring(2, 2), 16);
            byte b = Convert.ToByte(hex.Substring(4, 2), 16);
            byte a = (byte)Math.Round(alpha * 255);
            return XColor.FromArgb(a, r, g, b);
        }

        public static readonly Theme Standart = new(
            "STANDART", "Padrão",
            Hex("FFFFFF"), Hex("F2F2F2"), Hex("222222"),
            Hex("000000"), Hex("666666"),
            fontTitle: "Poppins", fontBody: "Nunito", eco: true
        );

        public static readonly Theme Black = new(
            "BLACK", "Preto",
            Hex("F6F7F9"), Hex("E9ECEF"), Hex("4F4F4F"),
            Hex("000000"), Hex("6C757D"),
            fontTitle: "Poppins", fontBody: "Nunito"
        );

        public static readonly Theme Blue = new(
            "BLUE", "Azul",
            Hex("EDF4FF"), Hex("D6E7FF"), Hex("3A7BD5"),
            Hex("111111"), Hex("7DA6F6"),
            fontTitle: "Poppins", fontBody: "Nunito"
        );

        public static readonly Theme Orange = new(
            "ORANGE", "Laranja",
            Hex("FFF6E5"), Hex("FFE7BF"), Hex("7A5C4B"),
            Hex("111111"), Hex("FFC47D"),
            fontTitle: "Poppins", fontBody: "Nunito"
        );

        private static readonly IReadOnlyDictionary<string, Theme> _all =
            new Dictionary<string, Theme>(StringComparer.OrdinalIgnoreCase)
            {
                ["STANDART"] = Standart,
                ["BLACK"] = Black,
                ["BLUE"] = Blue,
                ["ORANGE"] = Orange
            };

        public static Theme Get(string key) =>
            _all.TryGetValue(key ?? "", out var t) ? t : Black;

        internal static IReadOnlyDictionary<string, Theme> All => _all;
    }
}
