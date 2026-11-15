# FlashLabel Y43BT — the official driver says 100x150mm, not 4x6

A fix for the **Forc / FlashLabel Y43BT** direct-thermal label printer
(Xiamen Print Future Technology Co., Ltd.) on Windows: after installing
the manufacturer's official driver, software that requires an exact
4x6 paper size refuses to print — because the driver's real paper size
is **100mm x 150mm (394 x 591)**, not exactly 400x600.

## The problem

1. Install the official Windows driver from the manufacturer's support
   portal, [help.flashlabel.com](https://help.flashlabel.com/support/solutions/articles/150000175914-install-driver-on-windows-via-usb-cable)
   (installer `LabelPrinter-Windows-Driver-latest.exe`, v1.1.3.8).
   The printer does **not** come with a USB flash drive with drivers —
   the manufacturer site is the only driver source.
2. Windows creates a **"Y43BT Label"** queue on USB. Its default paper
   size is **"100mm x 150mm"**, which measures **3.94 x 5.91 in**
   (394 x 591 hundredths of an inch).
3. Any software that checks for an exact 4x6 paper size (400 x 600
   hundredths of an inch) rejects it and the job fails — even though
   the label is physically 4x6.

Why 394x591 and not 400x600: the printer is 203 DPI and the driver
reports its real metric size. 100mm at 203 dpi rounds to 3.94 in,
150mm to 5.91 in. It *is* a 4x6 label; the numbers just don't land on
exactly 400x600.

Two related gotchas:

- **Wrong queue.** Windows may also show an old generic **"POS-80"**
  queue (80mm receipt-printer driver) for the same printer. Print to
  the **"Y43BT Label"** queue (the official driver), not POS-80.
- **Bluetooth is mobile-only.** Bluetooth on the Y43BT works with the
  FlashLabel Pro phone app only. On Windows, connect by **USB**.

## The fix

Accept the driver's real size as 4x6. Two rules, in order:

1. **Size within 2% of 400x600 on each dimension counts as 4x6.**
   394 is within 2% of 400, 591 is within 2% of 600. (Checked against
   the driver's full size list: only the 100x150mm sizes fall within
   2%, so nothing else sneaks in.)
2. **Name fallback:** a paper size named "4x6" or "100x150" (after
   lowercasing, stripping spaces/quotes and "mm") counts as 4x6.

Still 4x6 only — the tolerance only widens what counts as 4x6 to the
driver's real metric size. Any other size still fails honestly instead
of printing on the wrong paper.

Reference implementation in C# (from our print bridge, MIT-licensed
here — drop it into your own code):

```csharp
// Find the printer's 4x6 paper size. Exact dimensions first
// (400x600 hundredths of an inch); near-exact second — a size
// within 2% of 400x600 on each dimension, because the official
// FlashLabel driver reports its real 100x150mm size as 394x591;
// name fallback third.
private static PaperSize? Find4x6PaperSize(PrinterSettings settings)
{
    foreach (PaperSize size in settings.PaperSizes)
    {
        if (size.Width == 400 && size.Height == 600)
            return size;
    }
    foreach (PaperSize size in settings.PaperSizes)
    {
        if (IsWithinPercent(size.Width, 400, 2)
            && IsWithinPercent(size.Height, 600, 2))
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
```

The same file is in this repo as `Find4x6PaperSize.cs`.

If you are not writing code, the user-facing version of the fix is:
use the **"Y43BT Label"** queue, and if your software lets you pick a
paper size, pick **"100mm x 150mm"** — that is the driver's 4x6.

## Verified

A 4x6 test label printed successfully on the Y43BT (USB, Windows 11,
official driver v1.1.3.8, "Y43BT Label" queue) on November 15, 2026,
using the tolerance above. Before the fix, the same setup failed at
the exact-400x600 check on every attempt.

## Details

- Printer: Forc / FlashLabel Y43BT — direct thermal, 203 DPI,
  USB + Bluetooth, max label 4x6, label widths 40-110mm.
- Driver: official FlashLabel driver v1.1.3.8, signed by Xiamen Print
  Future Technology Co., Ltd., from help.flashlabel.com.
- By [Creat3d World](https://creat3dworld.com) — we hit this while
  building our shipping-label printing, and published the fix so
  nobody else has to lose a day to it.
