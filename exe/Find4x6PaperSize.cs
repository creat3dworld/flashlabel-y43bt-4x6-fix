// Find the printer's 4x6 paper size — reference implementation.
// MIT licensed. Drop into your own C# printing code.
//
// Exact dimensions first (400x600 hundredths of an inch);
// near-exact second — a size within 2% of 400x600 on each
// dimension, because the official FlashLabel Y43BT driver reports
// its real 100x150mm size as 394x591 (100mm at 203 dpi rounds to
// 3.94 in, 150mm to 5.91 in); name fallback third — drivers name it
// variously ("4 x 6", "4x6", "100 x 150mm", "4\" x 6\""), and "mm"
// is stripped before matching so "100mmx150mm" matches "100x150".
// A size that is neither 4x6 by size nor by name is not a 4x6 size:
// returning null fails the job honestly instead of printing on the
// wrong paper. Still 4x6 ONLY.

using System.Drawing.Printing;

public static class PaperSize4x6
{
    private const int Paper4x6WidthHundredths = 400;
    private const int Paper4x6HeightHundredths = 600;

    public static PaperSize? Find4x6PaperSize(PrinterSettings settings)
    {
        foreach (PaperSize size in settings.PaperSizes)
        {
            if (size.Width == Paper4x6WidthHundredths && size.Height == Paper4x6HeightHundredths)
                return size;
        }
        foreach (PaperSize size in settings.PaperSizes)
        {
            if (IsWithinPercent(size.Width, Paper4x6WidthHundredths, 2)
                && IsWithinPercent(size.Height, Paper4x6HeightHundredths, 2))
                return size;
        }
        foreach (PaperSize size in settings.PaperSizes)
        {
            if (NormalizePaperName(size.PaperName).Contains("4x6")
                || NormalizePaperName(size.PaperName).Contains("100x150"))
                return size;
        }
        return null;
    }

    private static string NormalizePaperName(string? name)
        => (name ?? "").ToLowerInvariant().Replace(" ", "").Replace("\"", "")
            .Replace("'", "").Replace("mm", "");

    private static bool IsWithinPercent(int value, int target, int percent)
        => Math.Abs(value - target) * 100 <= target * percent;
}
