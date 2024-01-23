namespace StayPut
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var stayPut = new StayPut();
            stayPut.MoveWindowsOnce();
        }
    }
}
