using System.Diagnostics.CodeAnalysis;

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

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            if (obj is Zone zone)
            {
                return zone.X == X &&
                    zone.Y == Y &&
                    zone.Width == Width &&
                    zone.Height == Height;
            }

            return false;
        }

        public static bool operator ==(Zone left, Zone right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Zone left, Zone right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
