using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace LionFire.Shell
{
    public static class MoveWindowToForeground
    {
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        private static extern int FindWindow(String ClassName, String WindowName);

        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_SHOWWINDOW = 0x0040;
        const int SWP_NOACTIVATE = 0x0010;

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        public static void DoOnProcess(string processName)
        {
            var allProcs = Process.GetProcessesByName(processName);
            if (allProcs.Length == 1)
            {
                Process proc = allProcs[0];
                DoOnProcess(proc);
            }
            else if (allProcs.Length > 1)
            {
                foreach (var p in allProcs)
                {
                    DoOnProcess(p);
                }
            }
            else
            {
                throw new Exception("Proc not found: " + processName);
            }
        }

        public static void DoOnProcess(Process proc)
        {

            int hWnd = FindWindow(null, proc.MainWindowTitle.ToString());
            // Change behavior by settings the wFlags params. See http://msdn.microsoft.com/en-us/library/ms633545(VS.85).aspx
            SetWindowPos(new IntPtr(hWnd), 0, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW | SWP_NOACTIVATE);
        }
    }
}

