using System;

namespace _1.Семенар
{
    class Program
    {
        static void Main(string[] args)
        {
            int i;
            int y;
            i = int.Parse(Console.ReadLine());
            Console.WriteLine($"It is what it is {i * i}.....");
            if (int.TryParse(Console.ReadLine(), out y))
            {
                Console.WriteLine(y);
            }
            else
            //kasdklajslkdjalkdjalkjdlakjsdlkajdlkjalkdjalkdjalksjdlkajsdlkasjslkdjalksjlks
            {
                Console.WriteLine("stop!! that ill{}egal!!");
            }
            int a = y > 5 ? 1 : 0;
            Console.WriteLine(a);
        }
    }
}
