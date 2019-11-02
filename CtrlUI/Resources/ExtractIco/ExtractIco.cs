using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ExtractIco
{
    public class ExtractIco
    {
        [DllImport("Shell32.dll")]
        private static extern int SHDefExtractIconW(IntPtr pszIconFile, int iIndex, uint uFlags, ref IntPtr phiconLarge, ref IntPtr phiconSmall, uint nIconSize);

        [DllImport("User32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        private static int CalculateIconSize(int Largest, int Smallest)
        {
            return (Smallest << 16) | (Largest & 0xFFFF);
        }

        public static Bitmap GetBitmapFromExePath(string ExePath)
        {
            try
            {
                IntPtr PtrIconLarge = IntPtr.Zero;
                IntPtr PtrIconSmall = IntPtr.Zero;
                uint LargestSmallest = Convert.ToUInt32(CalculateIconSize(256, 1));

                IntPtr IntPtrExePath = Marshal.StringToHGlobalUni(ExePath);
                int IconExtractResult = SHDefExtractIconW(IntPtrExePath, 0, 0, ref PtrIconLarge, ref PtrIconSmall, LargestSmallest);
                if (IconExtractResult == 0)
                {
                    if (PtrIconLarge != IntPtr.Zero)
                    {
                        using (Icon IconRaw = Icon.FromHandle(PtrIconLarge))
                        {
                            Bitmap IconBitmap = IconRaw.ToBitmap();
                            if (PtrIconLarge != IntPtr.Zero) { DestroyIcon(PtrIconLarge); }
                            if (PtrIconSmall != IntPtr.Zero) { DestroyIcon(PtrIconSmall); }
                            return IconBitmap;
                        }
                    }
                    if (PtrIconSmall != IntPtr.Zero)
                    {
                        using (Icon IconRaw = Icon.FromHandle(PtrIconSmall))
                        {
                            Bitmap IconBitmap = IconRaw.ToBitmap();
                            if (PtrIconLarge != IntPtr.Zero) { DestroyIcon(PtrIconLarge); }
                            if (PtrIconSmall != IntPtr.Zero) { DestroyIcon(PtrIconSmall); }
                            return IconBitmap;
                        }
                    }
                }
                return null;
            }
            catch { return null; }
        }
    }
}