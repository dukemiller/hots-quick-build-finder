using System;
using System.Runtime.InteropServices;

namespace hots_quick_build_finder.Classes
{
    public static class WindowsApi
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string sClassName, string sAppName);

        private const int Restore = 9;

        /// <summary>
        ///     Focus the opened downloader.
        /// </summary>
        public static void FocusBuildFinder()
        {
            var hwnd = FindWindow(null, "HotS quick build finder");
            ShowWindow(hwnd, Restore);
            SetForegroundWindow(hwnd);
        }
    }
}
