using System;

namespace _2.Seminar
{
    class Program
    {
        static void Main(string[] args)
        {

            Random Rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(Rand.Next());
                

            }
            

        }
    }
}
