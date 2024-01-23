using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

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
        #endregion

        const short SWP_NOZORDER = 0X4;
        const int SWP_SHOWWINDOW = 0x0040;
        const string PROFILE_FILE = "profile.txt";

        private StayPutProfile profile;

        public void MoveWindowsOnce()
        {
            LoadProfile();
            MoveWindows();
        }

        private void MoveWindows()
        {
            foreach (var process in Process.GetProcesses("."))
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

        public void GetCurrentWindowSettings()
        {
            StayPutProfile current = new([]);

            foreach (var process in Process.GetProcesses("."))
            {
                var handle = process.MainWindowHandle;
                if (handle == IntPtr.Zero)
                    continue;

                var handleRef = new HandleRef(process, handle);
                if (!GetWindowRect(handleRef, out var rect))
                    continue;

                var zone = new Zone("", rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, []);
                if (zone.Width < 10 || zone.Height < 10)
                    continue;
                if (current.Zones.Any(z => z == zone))
                    zone = current.Zones.First(z => z == zone);
                else
                    current.Zones.Add(zone);
                zone.Windows.Add(new Window(process.ProcessName, GetWindowTitle(handleRef)));
            }

           Clipboard.SetText(JsonConvert.SerializeObject(current, Formatting.Indented));
        }

        private void SaveProfile()
        {
            using StreamWriter sw = new(PROFILE_FILE);
            sw.WriteLine(JsonConvert.SerializeObject(profile, Formatting.Indented));
        }

        private void LoadProfile()
        {
            using StreamReader sr = new(PROFILE_FILE);
            profile = JsonConvert.DeserializeObject<StayPutProfile>(sr.ReadToEnd());
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
            StringBuilder sb = new(GetWindowTextLength(handleRef) * 2);
            GetWindowText(handleRef, sb, sb.Capacity);
            return sb.ToString();
        }
    }
}
