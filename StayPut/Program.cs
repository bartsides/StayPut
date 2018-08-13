using System;

namespace StayPut
{
    class Program
    {
        [STAThreadAttribute]
        static void Main()
        {
            WindowHandler.HandleWindows();
        }
    }
}
