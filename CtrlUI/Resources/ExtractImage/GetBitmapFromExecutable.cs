using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CtrlUI
{
    public partial class ExtractImage
    {
        [DllImport("Shell32.dll")]
        private static extern int SHDefExtractIconW(IntPtr pszIconFile, int iIndex, uint uFlags, ref IntPtr phiconLarge, ref IntPtr phiconSmall, uint nIconSize);

        [DllImport("User32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        private static int CalculateIconSize(int largestIcon, int smallestIcon)
        {
            return (smallestIcon << 16) | (largestIcon & 0xFFFF);
        }

        public static Bitmap GetBitmapFromExecutable(string executablePath, int iconIndex)
        {
            try
            {
                IntPtr ptrIconLarge = IntPtr.Zero;
                IntPtr ptrIconSmall = IntPtr.Zero;
                uint largestSmallest = Convert.ToUInt32(CalculateIconSize(256, 1));

                IntPtr intPtrExePath = Marshal.StringToHGlobalUni(executablePath);
                int iconExtractResult = SHDefExtractIconW(intPtrExePath, iconIndex, 0, ref ptrIconLarge, ref ptrIconSmall, largestSmallest);
                if (iconExtractResult == 0)
                {
                    if (ptrIconLarge != IntPtr.Zero)
                    {
                        using (Icon IconRaw = Icon.FromHandle(ptrIconLarge))
                        {
                            Bitmap IconBitmap = IconRaw.ToBitmap();
                            if (ptrIconLarge != IntPtr.Zero) { DestroyIcon(ptrIconLarge); }
                            if (ptrIconSmall != IntPtr.Zero) { DestroyIcon(ptrIconSmall); }
                            return IconBitmap;
                        }
                    }
                    if (ptrIconSmall != IntPtr.Zero)
                    {
                        using (Icon IconRaw = Icon.FromHandle(ptrIconSmall))
                        {
                            Bitmap IconBitmap = IconRaw.ToBitmap();
                            if (ptrIconLarge != IntPtr.Zero) { DestroyIcon(ptrIconLarge); }
                            if (ptrIconSmall != IntPtr.Zero) { DestroyIcon(ptrIconSmall); }
                            return IconBitmap;
                        }
                    }
                }
            }
            catch { }
            return null;
        }
    }
}