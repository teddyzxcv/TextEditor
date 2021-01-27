using System;
using System.IO;

namespace FileExam
{
    class Program
    {
        static void Main(string[] args)
        {



            string[] FileLine = File.ReadAllLines("Groceries_dataset.csv");
            foreach (string i in FileLine)
            {
                Console.WriteLine(i);



            }





        }
    }
}
