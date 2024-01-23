namespace StayPut
{
    public struct Window(string processName, string windowTitle = "")
    {
        public string ProcessName { get; set; } = processName;
        public string WindowTitle { get; set; } = windowTitle;
    }
}
