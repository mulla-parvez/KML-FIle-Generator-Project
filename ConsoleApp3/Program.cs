using System;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            var locationObj = new Location();
            locationObj.ReadCSV();
            var sectionObj = new Map();
            sectionObj.Classify(locationObj);
            Console.WriteLine("converted");
            Console.ReadLine();
        }
    }
}
