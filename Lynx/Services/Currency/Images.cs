using Discord;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lynx.Services.Currency
{

    public class Images
    {
        public class LockBitsImage : IDisposable
        {
            private bool disposed = false;
            private BitmapData bmpData;
            public Bitmap bmpSource;

            public int[] Pixels { get; private set; }
            public int Width { get; }
            public int Height { get; }

            public LockBitsImage(Bitmap bmp)
            {
                bmpSource = bmp;
                Width = bmpSource.Width;
                Height = bmpSource.Height;
                bmpData = bmpSource.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, bmpSource.PixelFormat);
                Pixels = new int[Width * Height];
                Marshal.Copy(bmpData.Scan0, Pixels, 0, Pixels.Length);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected void Dispose(bool disposing)
            {
                if (!disposed)
                {
                    if (Pixels != null)
                        Marshal.Copy(Pixels, 0, bmpData.Scan0, Pixels.Length);
                    bmpSource.UnlockBits(bmpData);
                    Pixels = null;
                    bmpData = null;
                    disposed = true;
                }
            }

            public System.Drawing.Color GetPixel(int x, int y)
            {
                return System.Drawing.Color.FromArgb(Pixels[y * Width + x]);
            }

            public void SetPixel(int x, int y, System.Drawing.Color clr)
            {
                Pixels[y * Width + x] = clr.ToArgb();
            }
            public static Bitmap Transparent2Color(Bitmap bmp1, System.Drawing.Color target)
            {
                Bitmap bmp2 = new Bitmap(bmp1.Width, bmp1.Height);
                Rectangle rect = new Rectangle(Point.Empty, bmp1.Size);
                using (Graphics G = Graphics.FromImage(bmp2))
                {
                    G.Clear(target);
                    G.DrawImageUnscaledAndClipped(bmp1, rect);
                }
                return bmp2;
            }
            public static void ConvertFormat(ref Bitmap bmp, PixelFormat TargetFormat = PixelFormat.Format32bppArgb)
            {
                try
                {
                    Bitmap bmp2 = new Bitmap(bmp.Width, bmp.Height, TargetFormat);
                    using (Graphics g = Graphics.FromImage(bmp2))
                    {
                        bmp.SetResolution(96, 96);
                        g.DrawImageUnscaled(bmp, Point.Empty);
                    }
                    bmp = bmp2;
                }
                catch
                {
                    throw new FormatException("Could not convert the bitmap to the standard format.");
                }
            }
        }
    }
}
