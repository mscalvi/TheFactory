using System;
using System.Globalization;

namespace FurmaIdle.Helpers
{
    public static class NumbersHelper
    {
        public static string Padronize(double value, int decimals = 2, double threshold = 1e5, CultureInfo? culture = null)
        {
            culture ??= CultureInfo.CurrentCulture;

            if (double.IsNaN(value)) return "NaN";
            if (double.IsPositiveInfinity(value)) return "+∞";
            if (double.IsNegativeInfinity(value)) return "−∞";
            if (value == 0) return "0";

            var abs = Math.Abs(value);

            // Limite mínimo para ainda valer a pena mostrar em formato "normal"
            // Ex: decimals = 2 => minNormal = 10^-2 = 0.01
            var minNormal = Math.Pow(10, -decimals);

            // Se estiver dentro do range "visível" com 'decimals' casas, mostra normal
            if (abs >= minNormal && abs < threshold)
            {
                var s = value.ToString("N" + decimals, culture);
                return TrimDecimalZeros(s, culture);
            }

            // Fora desse range (muito grande OU muito pequeno) -> notação de engenharia
            // Engenharia (expoente múltiplo de 3): XXXeY
            var exp = (int)Math.Floor(Math.Log10(abs));
            var e3 = exp - (exp % 3);   // múltiplo de 3
            if (e3 == -0) e3 = 0;

            var mant = abs / Math.Pow(10, e3);
            // Mantissa em [1, 1000)
            if (mant >= 1000)
            {
                mant /= 1000;
                e3 += 3;
            }

            var sMant = mant.ToString("N" + decimals, culture);
            sMant = TrimDecimalZeros(sMant, culture);

            var sign = value < 0 ? "-" : "";

            return $"{sign}{sMant}e{e3}";
        }

        // Overloads convenientes
        public static string Padronize(long value, int decimals = 2, double threshold = 1e5, CultureInfo? culture = null)
            => Padronize((double)value, decimals, threshold, culture);

        public static string Padronize(decimal value, int decimals = 2, double threshold = 1e5, CultureInfo? culture = null)
            => Padronize((double)value, decimals, threshold, culture);

        private static string TrimDecimalZeros(string formatted, CultureInfo culture)
        {
            var decSep = culture.NumberFormat.NumberDecimalSeparator;
            var idx = formatted.LastIndexOf(decSep, StringComparison.Ordinal);

            if (idx < 0) return formatted; // não tem parte decimal

            // Verifica se tudo depois do separador são zeros
            for (int i = idx + decSep.Length; i < formatted.Length; i++)
            {
                if (formatted[i] != '0')
                    return formatted; // tem algum decimal não-zero, mantém
            }

            // Só zeros: corta a parte decimal inteira
            return formatted.Substring(0, idx);
        }
    }
}
