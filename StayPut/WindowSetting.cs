namespace StayPut
{
    class WindowSetting
    {
        public string ProcessName { get; set; }
        public string WindowTitle { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public WindowSetting() { }

        public WindowSetting(string processName, string windowTitle, int x, int y, int width, int height)
        {
            this.ProcessName = processName;
            this.WindowTitle = windowTitle;
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public WindowSetting(string processName, string windowTitle, Position position, Size size) :
            this(processName, windowTitle, position.X, position.Y, size.X, size.Y)
        {
        }

        public string Serialize()
        {
            return $"new {this.GetType().Name}(\"{ProcessName}\", \"{WindowTitle}\", {X}, {Y}, {Width}, {Height})";
        }

        override
        public string ToString()
        {
            return $"Process: '{ProcessName}'({WindowTitle}) ({X},{Y}) {Width}x{Height}";
        }
    }
}
