using System.Drawing;
using System.Drawing.Imaging;

namespace ingif
{
    class Frame
    {
        Bitmap bitmap;

        public Frame(Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        public void SaveAsPng(string dest)
        {
            bitmap.Save(dest, ImageFormat.Png);
        }
    }
}
