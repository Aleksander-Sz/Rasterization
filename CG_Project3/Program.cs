using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace CG_Project3
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
        static public byte[] ImageToByteArray(Bitmap Image, out int stride)
        {
            Rectangle rect = new Rectangle(0, 0, Image.Width, Image.Height);
            BitmapData bmpData = Image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            stride = bmpData.Stride;
            int bytes = Math.Abs(bmpData.Stride) * Image.Height;
            byte[] rgbValues = new byte[bytes];

            Marshal.Copy(bmpData.Scan0, rgbValues, 0, bytes);
            Image.UnlockBits(bmpData);

            return rgbValues;
        }
        static public Bitmap ByteArrayToImage(byte[] rgbValues, int width, int height, int stride)
        {
            Bitmap Image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = Image.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            Marshal.Copy(rgbValues, 0, bmpData.Scan0, rgbValues.Length);
            Image.UnlockBits(bmpData);

            return Image;
        }
    }
}