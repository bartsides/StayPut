namespace StayPut
{
    public struct Zone
    {
        public Position Position { get; set; }
        public Size Size { get; set; }

        public Zone(Position position, Size size)
        {
            Position = position;
            Size = size;
        }
    }
}
