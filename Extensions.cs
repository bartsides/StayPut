namespace StayPut
{
    public static class Extensions
    {
        public static int Width(this Rect rect) => rect.Right - rect.Left;
        public static int Height(this Rect rect) => rect.Bottom - rect.Top;
    }
}
