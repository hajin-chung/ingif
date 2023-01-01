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
        public int Fps { get; set; }
        #endregion

        private CancellationToken ct;
        private CancellationTokenSource ts;

        public Frames frames;
        Task<Frames> captureTask;
        private readonly IntPtr _desktopWindow = IntPtr.Zero;
        protected IntPtr WindowDeviceContext;
        protected IntPtr CompatibleDeviceContext;
        protected IntPtr CompatibleBitmap;
        private IntPtr _oldBitmap;
        private CopyPixelOperations PixelOperations;

        public Recorder()
        {
            Fps = 15;
            frames = new();
            PixelOperations = CopyPixelOperations.SourceCopy | CopyPixelOperations.CaptureBlt;
            ts = new CancellationTokenSource();
            ct = ts.Token;
        }

        private Frames Capture(int X, int Y, int Width, int Height, CancellationToken ct) 
        { 
            Frames frames = new();
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
                    Frame frame= new(bitmap);
                    frames.Add(frame);
                }
                Thread.Sleep(Convert.ToInt32(1000 / Fps));
            }
            return frames;
        }

        public void Start(int X, int Y, int Width, int Height)
        {
            frames = new();

            WindowDeviceContext = User32.GetWindowDC(_desktopWindow);
            CompatibleDeviceContext = Gdi32.CreateCompatibleDC(WindowDeviceContext);
            CompatibleBitmap = Gdi32.CreateCompatibleBitmap(WindowDeviceContext, Width, Height);
            _oldBitmap = Gdi32.SelectObject(CompatibleDeviceContext, CompatibleBitmap);

            captureTask = Task<Frames>.Factory.StartNew(() =>
            {
                return Capture(X, Y, Width, Height, ct);
            }, ct);
        }

        public void Stop()
        {
            ts.Cancel();
            frames = captureTask.Result;
            frames.ExportAsGif("./test.gif");
            SaveFrames("./");
        }

        public void SaveFrames(string location)
        {
            Trace.WriteLine(frames.Count);
        }
    }
}
