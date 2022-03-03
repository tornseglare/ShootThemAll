using System;

namespace ShootThemAll
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using ShootThemAllGame theGame = new();
            theGame.Run();
        }
    }
}