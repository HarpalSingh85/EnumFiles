using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\asinghh8";
            int count = WinAPIEnum.FindFilesAndDirs(path).Count();
            Console.WriteLine(count);
            Console.ReadLine();

        }


    }
}
