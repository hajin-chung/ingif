using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.IO;

namespace ingif
{
    class Recorder
    {
        #region settings
        public int fps { get; set; }
        #endregion

        private Task captureTask;
        private CancellationToken ct;
        private CancellationTokenSource ts;

        public List<Frame> frames;
        
        private readonly IntPtr _desktopWindow = IntPtr.Zero;
        protected IntPtr WindowDeviceContext;
        protected IntPtr CompatibleDeviceContext;
        protected IntPtr CompatibleBitmap;
        private IntPtr _oldBitmap;
        private CopyPixelOperations PixelOperations;

        public Recorder()
        {
            fps = 15;
            frames = new List<Frame>();
            PixelOperations = CopyPixelOperations.SourceCopy | CopyPixelOperations.CaptureBlt;
        }

        private List<Frame> Capture(int X, int Y, int Width, int Height, CancellationToken ct) 
        { 
            List<Frame> frames = new List<Frame>();
            while(true)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }
                bool success = Gdi32.StretchBlt(CompatibleDeviceContext, 0, 0, Width, Height, WindowDeviceContext, X, Y, Width, Height, PixelOperations);
                if (success) 
                {
                    Bitmap bitmap = Image.FromHbitmap(CompatibleBitmap);
                    frames.Append(new Frame(bitmap));
                    int len = frames.Count;
                    frames.Last().SaveAsPng(string.Format("{0}.png", len));
                }
                Thread.Sleep(Convert.ToInt32(1000 / fps));
            }
            return frames;
        }

        public void Start(int X, int Y, int Width, int Height)
        {
            ts = new CancellationTokenSource();
            ct = ts.Token;
            frames = new List<Frame>();

            WindowDeviceContext = User32.GetWindowDC(_desktopWindow);
            CompatibleDeviceContext = Gdi32.CreateCompatibleDC(WindowDeviceContext);
            CompatibleBitmap = Gdi32.CreateCompatibleBitmap(WindowDeviceContext, Width, Height);
            _oldBitmap = Gdi32.SelectObject(CompatibleDeviceContext, CompatibleBitmap);

            captureTask = new Task(() =>
            {
                Capture(X, Y, Width, Height, ct);
            }, ct);
            captureTask.Start();
        }

        public void Stop()
        {
            ts.Cancel();
            captureTask.Dispose();
            SaveFrames("./");
        }

        public void SaveFrames(string location)
        {
            Trace.WriteLine(frames.Count);
            for(int i=0; i < frames.Count; i++)
            {
                String fileName = String.Format("{0}.png", i);
                String dest = Path.Combine(location, fileName);
                Trace.WriteLine(dest);
                frames[i].SaveAsPng(dest);
            }
        }
    }
}
