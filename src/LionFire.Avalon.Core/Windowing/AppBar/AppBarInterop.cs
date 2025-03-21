using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace LionFire.Avalon.Windowing.AppBar
{
    public enum AppBarEdge
    {
        Left = AppBarInterop.ABE_LEFT,
        Top = AppBarInterop.ABE_TOP,
        Right = AppBarInterop.ABE_RIGHT,
        Bottom = AppBarInterop.ABE_BOTTOM,
    }

    public static class AppBarInterop
    {
        #region Window Docking

        [DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
        public static extern uint SHAppBarMessage(int dwMessage, ref APPBARDATA pData);
        [DllImport("User32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int cx, int cy, bool repaint);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int RegisterWindowMessage(string msg);
        
        public const int ABM_NEW = 0;
        public const int ABM_REMOVE = 1;
        public const int ABM_QUERYPOS = 2;
        public const int ABM_SETPOS = 3;
        public const int ABM_GETSTATE = 4;
        public const int ABM_GETTASKBARPOS = 5;
        public const int ABM_ACTIVATE = 6;
        public const int ABM_GETAUTOHIDEBAR = 7;
        public const int ABM_SETAUTOHIDEBAR = 8;
        public const int ABM_WINDOWPOSCHANGED = 9;
        public const int ABM_SETSTATE = 10;

        public const int ABN_STATECHANGE = 0;
        public const int ABN_POSCHANGED = 1;
        public const int ABN_FULLSCREENAPP = 2;
        public const int ABN_WINDOWARRANGE = 3;

        public const int ABE_LEFT = 0;
        public const int ABE_TOP = 1;
        public const int ABE_RIGHT = 2;
        public const int ABE_BOTTOM = 3;
        

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        #endregion

        #region Glass extending

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
    
        [DllImport("dwmapi.dll")]
        public extern static int DwmIsCompositionEnabled(ref int en);
   
        public const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
        
        #endregion

        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int WM_EXITSIZEMOVE = 0x0232;

    }
}
