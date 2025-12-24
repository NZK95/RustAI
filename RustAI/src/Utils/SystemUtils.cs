using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;

namespace RustAI
{
    internal static class SystemUtils
    {
        public static void SwapActiveWindow(string processName)
        {
            var proc = Process.GetProcessesByName(processName).FirstOrDefault();
            if (proc == null || proc.MainWindowHandle == IntPtr.Zero)
                return;

            IntPtr hwnd = proc.MainWindowHandle;

            AllowSetForegroundWindow(proc.Id);
            AllowSetForegroundWindow(-1);
            ForceForegroundWindowAggressive(hwnd);
        }

        public static bool IsProcessRunning(string processName) =>
           Process.GetProcessesByName(processName).Length > 0;

        public static bool CheckActiveWindow(string name) =>
            GetActiveWindow() == name;

        public static (int width, int height) GetScreenResolution() =>
            (GetSystemMetrics(SM_CXSCREEN), GetSystemMetrics(SM_CYSCREEN));


        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool AllowSetForegroundWindow(int dwProcessId);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        private const int SW_RESTORE = 9;
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;
        private const int SW_SHOWMINIMIZED = 2;

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private static readonly IntPtr HWND_TOP = new IntPtr(0);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_SHOWWINDOW = 0x0040;

        private const byte VK_MENU = 0x12;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint INPUT_KEYBOARD = 1;

        private static void ForceForegroundWindowAggressive(IntPtr hWnd)
        {
            try
            {
                SimulateAltPress();
                Thread.Sleep(50);

                var placement = new WINDOWPLACEMENT();
                placement.length = Marshal.SizeOf(placement);
                GetWindowPlacement(hWnd, ref placement);

                if (placement.showCmd == SW_MINIMIZE || placement.showCmd == SW_SHOWMINIMIZED)
                {
                    placement.showCmd = SW_RESTORE;
                    SetWindowPlacement(hWnd, ref placement);
                }

                ShowWindow(hWnd, SW_RESTORE);
                Thread.Sleep(50);

                var foregroundWindow = GetForegroundWindow();
                var currentThreadId = GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero);
                var targetThreadId = GetWindowThreadProcessId(hWnd, IntPtr.Zero);
                var myThreadId = GetCurrentThreadId();

                if (currentThreadId != targetThreadId)
                {
                    AttachThreadInput(myThreadId, currentThreadId, true);
                    AttachThreadInput(myThreadId, targetThreadId, true);
                    AttachThreadInput(currentThreadId, targetThreadId, true);
                }

                BringWindowToTop(hWnd);
                ShowWindow(hWnd, SW_SHOW);
                SetForegroundWindow(hWnd);

                SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
                Thread.Sleep(10);
                SetWindowPos(hWnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
                Thread.Sleep(10);
                SetWindowPos(hWnd, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

                SetActiveWindow(hWnd);
                SetFocus(hWnd);
                SetForegroundWindow(hWnd);

                if (currentThreadId != targetThreadId)
                {
                    AttachThreadInput(currentThreadId, targetThreadId, false);
                    AttachThreadInput(myThreadId, targetThreadId, false);
                    AttachThreadInput(myThreadId, currentThreadId, false);
                }

                for (int i = 0; i < 3; i++)
                {
                    if (GetForegroundWindow() != hWnd)
                    {
                        Thread.Sleep(50);
                        SetForegroundWindow(hWnd);
                        SetFocus(hWnd);
                    }
                    else
                        break;
                }
            }
            catch
            {
                ShowWindow(hWnd, SW_RESTORE);
                SetForegroundWindow(hWnd);
            }
        }
        private static void SimulateAltPress()
        {
            var inputs = new INPUT[2];

            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].u.ki.wVk = VK_MENU;
            inputs[0].u.ki.dwFlags = 0;

            inputs[1].type = INPUT_KEYBOARD;
            inputs[1].u.ki.wVk = VK_MENU;
            inputs[1].u.ki.dwFlags = KEYEVENTF_KEYUP;

            SendInput(2, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        private static string GetActiveWindow()
        {
            var handle = GetForegroundWindow();
            var sb = new StringBuilder(256);

            if (GetWindowText(handle, sb, sb.Capacity) > 0)
                return sb.ToString();

            return Constants.NA;
        }
    }
}