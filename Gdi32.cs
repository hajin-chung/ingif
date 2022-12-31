using System;
using System.Runtime.InteropServices;

namespace ingif
{
    [Flags]
    public enum CopyPixelOperations
    {
        NoMirrorBitmap = -2147483648,

        /// <summary>dest = BLACK, 0x00000042</summary>
        Blackness = 66,

        ///<summary>dest = (NOT src) AND (NOT dest), 0x001100A6</summary>
        NotSourceErase = 1114278,

        ///<summary>dest = (NOT source), 0x00330008</summary>
        NotSourceCopy = 3342344,

        ///<summary>dest = source AND (NOT dest), 0x00440328</summary>
        SourceErase = 4457256,

        /// <summary>dest = (NOT dest), 0x00550009</summary>
        DestinationInvert = 5570569,

        /// <summary>dest = pattern XOR dest, 0x005A0049</summary>
        PatInvert = 5898313,

        ///<summary>dest = source XOR dest, 0x00660046</summary>
        SourceInvert = 6684742,

        ///<summary>dest = source AND dest, 0x008800C6</summary>
        SourceAnd = 8913094,

        /// <summary>dest = (NOT source) OR dest, 0x00BB0226</summary>
        MergePaint = 12255782,

        ///<summary>dest = (source AND pattern), 0x00C000CA</summary>
        MergeCopy = 12583114,

        ///<summary>dest = source, 0x00CC0020</summary>
        SourceCopy = 13369376,

        /// <summary>dest = source OR dest, 0x00EE0086</summary>
        SourcePaint = 15597702,

        /// <summary>dest = pattern, 0x00F00021</summary>
        PatCopy = 15728673,

        /// <summary>dest = DPSnoo, 0x00FB0A09</summary>
        PatPaint = 16452105,

        /// <summary>dest = WHITE, 0x00FF0062</summary>
        Whiteness = 16711778,

        /// <summary>
        /// Capture window as seen on screen.  This includes layered windows 
        /// such as WPF windows with AllowsTransparency="true", 0x40000000
        /// </summary>
        CaptureBlt = 1073741824,
    }
    class Gdi32
    {
        private const string gdi32 = "gdi32.dll";
        [DllImport(gdi32, EntryPoint = "CreateCompatibleDC", SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

        [DllImport(gdi32, EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);

        [DllImport(gdi32, EntryPoint = "BitBlt", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, CopyPixelOperations dwRop);

        [DllImport(gdi32, EntryPoint = "StretchBlt", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StretchBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidthDest, int nHeightDest, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, int nWidthSource, int nHeightSource, CopyPixelOperations dwRop);
        [DllImport(gdi32)]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref IntPtr pbmi, uint pila, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);
        
        [DllImport(gdi32)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
    }
}
