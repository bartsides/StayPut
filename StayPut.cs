using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace StayPut
{
    public class StayPut
    {
        #region user32 dllimports
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(HandleRef hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(HandleRef hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        #endregion

        const short SWP_NOMOVE = 0X2;
        const short SWP_NOSIZE = 1;
        const short SWP_NOZORDER = 0X4;
        const int SWP_SHOWWINDOW = 0x0040;
        const string PROFILE_FILE = "profile.txt";

        private StayPutProfile profile;

        private void SaveProfile()
        {
            using StreamWriter sw = new(PROFILE_FILE);
            sw.WriteLine(JsonSerializer.Serialize(profile));
        }

        private void LoadProfile()
        {
            using StreamReader sr = new(PROFILE_FILE);
            profile = JsonSerializer.Deserialize<StayPutProfile>(sr.ReadToEnd());
        }

        internal void HandleWindows()
        {
            LoadProfile();
            ShowWindow(GetConsoleWindow(), SW_HIDE);
            ManHandle();
        }

        internal void ManHandleConstantly()
        {
            while (true)
            {
                ManHandle();
                Thread.Sleep(5000);
            }
        }

        internal void ManHandle()
        {
            Process[] processes = Process.GetProcesses(".");
            foreach (var process in processes)
            {
                var handle = process.MainWindowHandle;
                if (handle == IntPtr.Zero)
                    continue;

                foreach (var zone in profile.Zones)
                foreach (var window in zone.Windows.Where(w => IsCorrectWindow(process, w)))
                {
                    SetWindowPos(handle, 0, zone.X, zone.Y, zone.Width, zone.Height, SWP_NOZORDER | SWP_SHOWWINDOW);
                }
            }
        }

        private void GetCurrentWindowSettings()
        {
            // TODO: Redo get current window settings
            //List<WindowPlacement> settings = new List<WindowPlacement>();

            //Process[] processes = Process.GetProcesses(".");
            //foreach (var process in processes)
            //{
            //    IntPtr handle = process.MainWindowHandle;
            //    if (handle != IntPtr.Zero)
            //    {
            //        var handleRef = new HandleRef(process, handle);
            //        if (!GetWindowRect(handleRef, out var rect))
            //            continue;

            //        WindowPlacement ws = new WindowPlacement
            //        {
            //            ProcessName = process.ProcessName,
            //            WindowTitle = GetWindowTitle(handleRef),
            //            X = rect.Left,
            //            Y = rect.Top,
            //            Width = rect.Right - rect.Left,
            //            Height = rect.Bottom - rect.Top
            //        };

            //        Console.WriteLine(ws);

            //        settings.Add(ws);
            //    }
            //}

            //string settingsString = string.Empty;
            //foreach (WindowPlacement setting in settings)
            //    settingsString += setting.Serialize() + "," + Environment.NewLine;

            // TODO: Fix Clipboard
            //Clipboard.SetText(settingsString);
        }

        private static bool IsCorrectWindow(Process process, Window window)
        {
            if (!process.ProcessName.Equals(window.ProcessName))
                return false;

            var windowTitle = GetWindowTitle(new HandleRef(process, process.MainWindowHandle));
            if (window.WindowTitle != null && (windowTitle == null || !windowTitle.Contains(window.WindowTitle)))
                return false;

            return true;
        }

        private static string GetWindowTitle(HandleRef handleRef)
        {
            StringBuilder sb = new StringBuilder(GetWindowTextLength(handleRef) * 2);
            GetWindowText(handleRef, sb, sb.Capacity);
            return sb.ToString();
        }
    }
}
