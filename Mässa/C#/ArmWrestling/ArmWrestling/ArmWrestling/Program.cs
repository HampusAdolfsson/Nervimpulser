using System;

namespace ArmWrestling
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GameMain game = new GameMain("800x450"))
            {
                game.Run();
            }
        }
    }
#endif
}

