using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace RustAI
{
    internal static class SystemUtils
    {
        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);
        
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool AllowSetForegroundWindow(int dwProcessId);

        public const int SW_RESTORE = 9;
        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;

        public static bool CheckActiveWindow(string name)
        {
           return GetActiveWindow() == name;
        }

        public static string GetActiveWindow()
        {
            IntPtr handle = GetForegroundWindow();
            StringBuilder sb = new StringBuilder(256);

            if (GetWindowText(handle, sb, sb.Capacity) > 0)
                return sb.ToString();

            return Constants.NA;
        }

        public static void SwapActiveWindow(string processName)
        {
            var proc = Process.GetProcessesByName(processName).FirstOrDefault();
            if (proc == null)
                return;

            AllowSetForegroundWindow(-1);
            ShowWindow(proc.MainWindowHandle, SW_RESTORE);
            SetForegroundWindow(proc.MainWindowHandle);
        }

        public static bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }
    }
}
