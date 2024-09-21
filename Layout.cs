namespace StayPut
{
    public class Layout(string name, List<Zone> zones)
    {
        public string Name { get; set; } = name;
        public List<Zone> Zones { get; set; } = zones;
    }
}
