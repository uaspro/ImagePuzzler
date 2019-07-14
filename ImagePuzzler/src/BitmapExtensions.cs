using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ImagePuzzler
{
    internal static class BitmapExtensions
    {
        public static Bitmap ExtractMask(this Bitmap input, Rectangle inputRect, BitmapData bitsMask, Rectangle maskRect, int maskX = 0, int maskY = 0)
        {
            var outputMask = new Bitmap(maskRect.Width, maskRect.Height, PixelFormat.Format32bppArgb);
            var bitsOutputMask = outputMask.LockBits(maskRect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            lock (input)
            {
                var bitsInput = input.LockBits(inputRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                unsafe
                {
                    Parallel.For(0, inputRect.Height, y =>
                    {
                        var ptrInput = (byte*)bitsInput.Scan0 + y * bitsInput.Stride;
                        for (int x = 0; x < inputRect.Width; x++)
                        {
                            if (y < maskY || y > maskY + maskRect.Height - 1 ||
                                x < maskX || x > maskX + maskRect.Width - 1)
                            {
                                continue;
                            }

                            var maskYPos = y - maskY;
                            var maskXPos = x - maskX;

                            var ptrMask = (byte*)bitsMask.Scan0 + maskYPos * bitsMask.Stride;
                            var ptrOutputMask = (byte*)bitsOutputMask.Scan0 + maskYPos * bitsOutputMask.Stride;

                            var maskAlpha = ptrMask[4 * maskXPos];
                            var reverseMaskValue = (byte)(byte.MaxValue - maskAlpha);
                            ptrInput[4 * x + 3] = reverseMaskValue;

                            ptrOutputMask[4 * maskXPos] = ptrInput[4 * x];
                            ptrOutputMask[4 * maskXPos + 1] = ptrInput[4 * x + 1];
                            ptrOutputMask[4 * maskXPos + 2] = ptrInput[4 * x + 2];
                            ptrOutputMask[4 * maskXPos + 3] = maskAlpha;
                        }
                    });
                }

                input.UnlockBits(bitsInput);
            }

            outputMask.UnlockBits(bitsOutputMask);

            return outputMask;
        }

        public static void DrawTo(this Bitmap source, Bitmap destination, Point? position = null, int? width = null, int? height = null)
        {
            lock (destination)
            {
                using (var g = Graphics.FromImage(destination))
                {
                    if (width.HasValue && height.HasValue)
                    {
                        g.DrawImage(source, position?.X ?? 0, position?.Y ?? 0, width.Value, height.Value);
                    }
                    else
                    {
                        g.DrawImage(source, position ?? new Point(0, 0));
                    }
                }
            }
        }
    }
}
