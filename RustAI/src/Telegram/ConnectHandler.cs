
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace RustAI
{
    internal static class ConnectHandler
    {
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

        const int SW_RESTORE = 9;

        public static bool CheckActiveWindow(string name)
        {
            IntPtr handle = GetForegroundWindow();
            StringBuilder sb = new StringBuilder(256);

            if (GetWindowText(handle, sb, sb.Capacity) > 0)
                return sb.ToString() == name;

            return false;
        }

        public static void SwapWindow(string processName)
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
