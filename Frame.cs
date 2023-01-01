using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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

    class Frames : List<Frame>
    {
        public void ExportAsGif(string dest)
        {
            string location = "./.temp";
            Directory.CreateDirectory(location);            
            for(int i=0; i < Count; i++)
            {
                string fileName = string.Format("frame{0}.png", i);
                string imageDest = Path.Combine(location, fileName);
                Trace.WriteLine(imageDest);
                this[i].SaveAsPng(imageDest);
            }

            ProcessStartInfo startInfo = new()
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = "gifski.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = string.Format("-o {0} .temp/frame*.png", dest)
            };

            try
            {
                Process gifSkiProcess = Process.Start(startInfo);
                gifSkiProcess.WaitForExit();
            }
            catch
            {
                Trace.WriteLine("error");
            }
        }
    }
}
