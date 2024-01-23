namespace StayPut
{
    public struct StayPutProfile(List<Zone> zones)
    {
        public List<Zone> Zones { get; set; } = zones;
    }
}
