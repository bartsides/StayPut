namespace StayPut
{
    public struct Zone(string name, int x, int y, int width, int height, List<Window> windows)
    {
        public string Name { get; set; } = name;
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
        public int Width { get; set; } = width;
        public int Height { get; set; } = height;
        public List<Window> Windows { get; set; } = windows;

        //public string Serialize()
        //{
        //    var result = "";
        //    foreach (var win in Windows)
        //    {
        //        result +=
        //    }
        //    return $"new {GetType().Name}(\"{ProcessName}\", \"{WindowTitle}\", {X}, {Y}, {Width}, {Height})";
        //}

        //override
        //public string ToString()
        //{
        //    return $"Process: '{ProcessName}'({WindowTitle}) ({X},{Y}) {Width}x{Height}";
        //}
    }
}
