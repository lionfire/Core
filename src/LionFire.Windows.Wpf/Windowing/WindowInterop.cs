using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Avalon
{
    public static class WindowInterop
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        const int WM_COMMAND = 0x111;
        const int MIN_ALL = 419;
        const int MIN_ALL_UNDO = 416;
        
        public static void MinimizeAllWindows()
        {
            IntPtr lHwnd = FindWindow("Shell_TrayWnd", null);
            SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL, IntPtr.Zero);
        }
        public static void UndoMinimizeAllWindows()
        {
            IntPtr lHwnd = FindWindow("Shell_TrayWnd", null);
            //System.Threading.Thread.Sleep(2000);
            SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL_UNDO, IntPtr.Zero);
        }
    }

    public class MoveToForeground
    {
        [DllImportAttribute("User32.dll")]
        private static extern int FindWindow(String ClassName, String WindowName);

        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_SHOWWINDOW = 0x0040;
        const int SWP_NOACTIVATE = 0x0010;
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        public static void DoOnProcess(string processName)
        {
            var allProcs = Process.GetProcessesByName(processName);
            if (allProcs.Length > 0)
            {
                Process proc = allProcs[0];
                int hWnd = FindWindow(null, proc.MainWindowTitle.ToString());
                // Change behavior by settings the wFlags params. See http://msdn.microsoft.com/en-us/library/ms633545(VS.85).aspx
                SetWindowPos(new IntPtr(hWnd), 0, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW | SWP_NOACTIVATE);
            }
        }
    }
}
