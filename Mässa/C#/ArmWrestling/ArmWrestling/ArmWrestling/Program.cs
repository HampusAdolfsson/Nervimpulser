using System;

namespace ArmWrestling
{
#if WINDOWS || XBOX
    internal static class Program
    {
        private static void Main(string[] args)
        {
            using (var game = new GameMain())
            {
                game.Run();
            }
        }
    }
#endif
}

