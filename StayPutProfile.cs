namespace StayPut
{
    public class StayPutProfile(List<Layout> layouts)
    {
        public int LastIndex { get; set; } = 0;
        public List<Layout> Layouts { get; set; } = layouts;
    }
}
