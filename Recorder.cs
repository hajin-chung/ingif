using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace ingif
{
    class Recorder
    {
        public int fps { get; set; }
        private Task captureTask;
        private CancellationToken ct;
        private CancellationTokenSource ts;
        public List<Frame> frames;
        
        private readonly IntPtr _desktopWindow = IntPtr.Zero;
        protected IntPtr WindowDeviceContext;
        protected IntPtr CompatibleDeviceContext;
        protected IntPtr CompatibleBitmap;
        private IntPtr _oldBitmap;

        public Recorder()
        {
            fps = 15;
            frames = new List<Frame>();
        }

        private void Capture(int X, int Y, int Width, int Height, CancellationToken ct) 
        { 
            while(true)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }
                var success = Gdi32.StretchBlt(CompatibleDeviceContext, 0, 0, )
                Bitmap bitmap = Bitmap.FromHbitmap(CompatibleBitmap);
                bitmap.Save("./test.bmp");
                frames.Append(Frame.FromScreen(X, Y, Width, Height));
                Thread.Sleep(Convert.ToInt32(1000 / fps));
            }
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
        }

        public void Stop()
        {
            ts.Cancel();
        }
    }
}
