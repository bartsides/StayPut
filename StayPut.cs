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
        static extern bool GetWindowRect(HandleRef hWnd, out Rect lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(HandleRef hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(HandleRef hWnd, StringBuilder lpString, int nMaxCount);
        #endregion

        const short SWP_NOZORDER = 0X4;
        const int SWP_SHOWWINDOW = 0x0040;
        const string PROFILE_FILE = "profile.json";

        private StayPutProfile LoadedProfile { get; set; }
        private Layout LoadedLayout
        {
            get
            {
                if (LoadedProfile.LastIndex < LoadedProfile.Layouts.Count)
                    return LoadedProfile.Layouts[LoadedProfile.LastIndex];
                return LoadedProfile.Layouts[0];
            }
        }

        public void MoveWindowsOnce()
        {
            //GetCurrentWindowSettings();
            LoadProfile();
            MoveWindows();
            SaveProfile();
        }

        private void MoveWindows()
        {
            foreach (var process in Process.GetProcesses("."))
            {
                var handle = process.MainWindowHandle;
                if (handle == IntPtr.Zero)
                    continue;

                foreach (var zone in LoadedLayout.Zones)
                foreach (var window in zone.Windows.Where(w => IsCorrectWindow(process, w)))
                {
                    SetWindowPos(handle, 0, zone.X, zone.Y, zone.Width, zone.Height, SWP_NOZORDER | SWP_SHOWWINDOW);
                }
            }
        }

        #region Profile
        private void LoadProfile()
        {
            using StreamReader sr = new(PROFILE_FILE);
            LoadedProfile = JsonConvert.DeserializeObject<StayPutProfile>(sr.ReadToEnd());
            // Cycle to next layout if current layout is already applied
            if (LoadedProfile.Layouts.Count > 1 && IsLayoutApplied())
                LoadedProfile.LastIndex = (LoadedProfile.LastIndex + 1) % LoadedProfile.Layouts.Count;
        }

        private void SaveProfile()
        {
            using StreamWriter sw = new(PROFILE_FILE);
            sw.WriteLine(JsonConvert.SerializeObject(LoadedProfile, Formatting.Indented));
        }
        #endregion

        #region Util
        private static void GetCurrentWindowSettings()
        {
            List<Zone> zones = [];

            foreach (var process in Process.GetProcesses(".").Where(p => p.MainWindowHandle != IntPtr.Zero))
            {
                var handle = process.MainWindowHandle;
                var handleRef = new HandleRef(process, handle);
                if (!GetWindowRect(handleRef, out var rect))
                    continue;

                var zone = new Zone("", rect.Left, rect.Top, rect.Width(), rect.Height(), []);
                if (zone.Width < 10 || zone.Height < 10)
                    continue;
                if (zones.Any(z => z == zone))
                    zone = zones.First(z => z == zone);
                else
                    zones.Add(zone);
                zone.Windows.Add(new Window(process.ProcessName, GetWindowTitle(handleRef)));
            }

            StayPutProfile profile = new([new("Default", zones)]);
            Clipboard.SetText(JsonConvert.SerializeObject(profile, Formatting.Indented));
        }

        private bool IsLayoutApplied()
        {
            foreach (var process in Process.GetProcesses(".").Where(p => p.MainWindowHandle != IntPtr.Zero))
            {
                var zone = LoadedLayout.Zones.FirstOrDefault(z => z.Windows.Any(w => IsCorrectWindow(process, w)));
                if (zone is null) continue;

                var handle = process.MainWindowHandle;
                var handleRef = new HandleRef(process, handle);
                if (!GetWindowRect(handleRef, out var rect))
                    continue;

                // Skip minimized windows
                if (rect.Left < -20000) continue;

                var width = rect.Width();
                var height = rect.Height();

                if (!WithinTolerance(rect.Left, zone.X) ||
                    !WithinTolerance(rect.Top, zone.Y) ||
                    !WithinTolerance(rect.Width(), width) ||
                    !WithinTolerance(rect.Height(), height))
                {
                    return false;
                }
            }
            return true;
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

        private static bool WithinTolerance(int x, int y, int tolerance = 2) 
            => x >= y - tolerance && x <= y + tolerance;
        #endregion
    }
}
