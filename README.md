# FlashLabel Y43BT 4x6 Driver Fix

Free fix for the **Forc / FlashLabel Y43BT** thermal label printer on Windows.
The official driver reports its 4x6 label as 100x150mm, so software that
wants an exact 4x6 size refuses to print. This fix finds that size and
treats it as 4x6.

## Use it

1. Install the official driver from help.flashlabel.com, printer on USB.
2. Unzip, then double-click **Y43BT-4x6-Fix.exe** — no install, no .NET needed.
3. It finds your Y43BT and shows the queue and paper size. Press **Y** to
   print a 4x6 test label.
4. In your software, print to the **Y43BT Label** queue and pick **100mm x
   150mm** if it asks for a paper size. That is 4x6.

If it says no printer was found: check the USB cable, check the printer is
on, and look for a **Y43BT Label** queue in Windows Settings > Printers.
Do not use the **POS-80** queue. Bluetooth works with the FlashLabel Pro
phone app only, not on Windows.

Developers: the paper-size logic is `Find4x6PaperSize.cs` (MIT). Source:
https://github.com/creat3dworld/flashlabel-y43bt-4x6-fix

Verified: a 4x6 test label printed on the Y43BT (USB, Windows 11, official
driver, Y43BT Label queue) on November 15, 2026.

By [Creat3d World](https://creat3dworld.com).
