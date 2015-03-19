using System;

namespace GameProject
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GuessNumber.GuessNumber game = new GuessNumber.GuessNumber())
            {
                game.Run();
            }
        }
    }
#endif
}

