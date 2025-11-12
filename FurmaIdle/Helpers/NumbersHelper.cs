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

            // Até o limite, mostra número "normal"
            if (abs < threshold)
                return value.ToString("N" + decimals, culture);

            // Engenharia (expoente múltiplo de 3): XXXeY
            var exp = (int)Math.Floor(Math.Log10(abs));
            var e3 = exp - (exp % 3);                 // múltiplo de 3
            if (e3 == -0) e3 = 0;

            var mant = abs / Math.Pow(10, e3);
            // Mantissa em [1, 1000)
            if (mant >= 1000) { mant /= 1000; e3 += 3; }

            var sMant = mant.ToString("N" + decimals, culture);
            // Sinal
            var sign = value < 0 ? "-" : "";

            return $"{sign}{sMant}e{e3}";
        }

        // Overloads convenientes
        public static string Padronize(long value, int decimals = 2, double threshold = 1e5, CultureInfo? culture = null)
            => Padronize((double)value, decimals, threshold, culture);

        public static string Padronize(decimal value, int decimals = 2, double threshold = 1e5, CultureInfo? culture = null)
            => Padronize((double)value, decimals, threshold, culture);
    }
}

