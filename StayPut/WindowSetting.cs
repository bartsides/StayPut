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

        public WindowSetting(string ProcessName, string WindowTitle, int X, int Y, int Width, int Height)
        {
            this.ProcessName = ProcessName;
            this.WindowTitle = WindowTitle;
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
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
