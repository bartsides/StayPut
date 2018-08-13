using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace StayPut
{
    public static class WindowHandler
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

        // Single Monitor
        //private static List<WindowSetting> userSettings = new List<WindowSetting> {
        //    new WindowSetting("Steam", "Friends", 2279, 22, 252, 620),
        //    new WindowSetting("Discord", null, 1577, 383, 954, 516),
        //    new WindowSetting("Spotify", null, 1731, 642, 800, 732)
        //};

        // Dual Monitor
        private static List<WindowSetting> userSettings = new List<WindowSetting>
        {
            new WindowSetting("Spotify", null, -1920, 441, 800, 600),
            new WindowSetting("Discord", null, -1920, 0, 940, 500),
            new WindowSetting("Battle.net", "Friends", -571, 0, 320, 617),
            new WindowSetting("steamwebhelper", "Friends List", -252, 0, 252, 617)
        };

        internal static void HandleWindows()
        {
            ShowWindow(GetConsoleWindow(), SW_HIDE);
            ManHandleConstantly();
        }

        internal static void ManHandleConstantly()
        {
            while (true)
            {
                ManHandle();
                Thread.Sleep(5000);
            }
        }

        internal static void ManHandle()
        {   
            const short SWP_NOMOVE = 0X2;
            const short SWP_NOSIZE = 1;
            const short SWP_NOZORDER = 0X4;
            const int SWP_SHOWWINDOW = 0x0040;
            
            Process[] processes = Process.GetProcesses(".");
            foreach (var process in processes)
            {
                IntPtr handle = process.MainWindowHandle;
                if (handle != IntPtr.Zero)
                {
                    foreach (WindowSetting setting in userSettings)
                    {
                        if (IsCorrectSetting(process, setting))
                        {
                            //Console.WriteLine($"Found {setting.ProcessName}");
                            SetWindowPos(handle, 0, setting.X, setting.Y, setting.Width, setting.Height, SWP_NOZORDER | SWP_SHOWWINDOW);
                        }
                    }
                }
            }
        }

        private static void GetCurrentWindowSettings()
        {
            List<WindowSetting> settings = new List<WindowSetting>();

            Process[] processes = Process.GetProcesses(".");
            foreach (var process in processes)
            {
                IntPtr handle = process.MainWindowHandle;
                if (handle != IntPtr.Zero)
                {
                    HandleRef handleRef = new HandleRef(process, handle);
                    RECT rect;
                    if (!GetWindowRect(handleRef, out rect))
                        continue;

                    WindowSetting ws = new WindowSetting {
                        ProcessName = process.ProcessName,
                        WindowTitle = GetWindowTitle(handleRef),
                        X = rect.Left,
                        Y = rect.Top,
                        Width = rect.Right - rect.Left,
                        Height = rect.Bottom - rect.Top
                    };

                    Console.WriteLine(ws);

                    settings.Add(ws);
                }
            }

            string settingsString = string.Empty;
            foreach (WindowSetting setting in settings)
                settingsString += setting.Serialize() + "," + Environment.NewLine;

            Clipboard.SetText(settingsString);
        }

        private static bool IsCorrectSetting (Process process, WindowSetting setting)
        {
            if (!process.ProcessName.Equals(setting.ProcessName))
                return false;
            if (setting.WindowTitle != null && !setting.WindowTitle.Equals(GetWindowTitle(new HandleRef(process, process.MainWindowHandle))))
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
