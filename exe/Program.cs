// Y43BT-4x6-Fix — double-click tool for the FlashLabel Y43BT.
// Finds the Y43BT printer, shows its real 4x6 paper size, and
// optionally prints a 4x6 test label. Windows only.
using System.Drawing;
using System.Drawing.Printing;

Console.WriteLine("Y43BT 4x6 Fix - Creat3d World");
Console.WriteLine();

// 1. Find Y43BT printers.
var queues = new List<string>();
foreach (string name in PrinterSettings.InstalledPrinters)
{
    if (name.Contains("Y43BT", StringComparison.OrdinalIgnoreCase))
        queues.Add(name);
}

if (queues.Count == 0)
{
    Console.WriteLine("No Y43BT printer found on this computer.");
    Console.WriteLine();
    Console.WriteLine("Check these three things:");
    Console.WriteLine("  1. Plug the printer in by USB and switch it on.");
    Console.WriteLine("  2. Install the official driver from help.flashlabel.com.");
    Console.WriteLine("  3. In Windows Settings > Printers, look for a queue called");
    Console.WriteLine("     'Y43BT Label'. Do not use the 'POS-80' queue.");
    WaitForKey();
    return 1;
}

// Prefer the official 'Y43BT Label' queue over any other Y43BT queue.
string queue = queues.FirstOrDefault(q => q.Contains("Label", StringComparison.OrdinalIgnoreCase))
    ?? queues[0];

PaperSize? size;
try
{
    var settings = new PrinterSettings { PrinterName = queue };
    size = PaperSize4x6.Find4x6PaperSize(settings);
}
catch (Exception ex)
{
    Console.WriteLine($"Printer found: {queue}");
    Console.WriteLine();
    Console.WriteLine($"Could not read that printer's paper sizes: {ex.Message}");
    Console.WriteLine("Reinstall the official driver from help.flashlabel.com,");
    Console.WriteLine("then run this again.");
    WaitForKey();
    return 1;
}

Console.WriteLine($"Printer found: {queue}");
if (queues.Count > 1)
    Console.WriteLine($"(Other Y43BT queues found: {string.Join(", ", queues.Where(q => q != queue))})");

if (size == null)
{
    Console.WriteLine();
    Console.WriteLine("This queue has no 4x6 paper size listed.");
    Console.WriteLine("Reinstall the official driver from help.flashlabel.com,");
    Console.WriteLine("then run this again. Print to the 'Y43BT Label' queue.");
    WaitForKey();
    return 1;
}

double widthIn = size.Width / 100.0;
double heightIn = size.Height / 100.0;
Console.WriteLine($"4x6 paper size: {size.PaperName} ({widthIn:0.##} x {heightIn:0.##} in)");
Console.WriteLine();
Console.WriteLine("That is your 4x6 size, even though it says 100x150mm.");
Console.WriteLine("In your software, print to the 'Y43BT Label' queue");
Console.WriteLine("and pick that paper size. That is 4x6.");

// 3. Offer a test label.
Console.WriteLine();
Console.Write("Print a 4x6 test label now? (Y/N) ");
ConsoleKeyInfo key = Console.ReadKey(intercept: true);
Console.WriteLine();
if (key.Key == ConsoleKey.Y)
{
    try
    {
        using var doc = new PrintDocument
        {
            PrinterSettings = new PrinterSettings { PrinterName = queue },
        };
        doc.DefaultPageSettings.PaperSize = size;
        doc.PrintPage += (_, e) =>
        {
            using var font = new Font("Arial", 14, FontStyle.Bold);
            e.Graphics?.DrawString("Y43BT 4x6 test - Creat3d World", font, Brushes.Black, 10, 10);
            using var small = new Font("Arial", 10);
            e.Graphics?.DrawString($"Paper size: {size.PaperName}", small, Brushes.Black, 10, 40);
            e.HasMorePages = false;
        };
        doc.Print();
        Console.WriteLine("Test label sent to the printer.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"The test label did not print: {ex.Message}");
        Console.WriteLine("Check the printer is on, has labels loaded, and try again.");
    }
}
else
{
    Console.WriteLine("No test label printed.");
}

WaitForKey();
return 0;

static void WaitForKey()
{
    Console.WriteLine();
    Console.WriteLine("Press any key to close.");
    Console.ReadKey(intercept: true);
}
