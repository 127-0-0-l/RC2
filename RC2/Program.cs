using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC2
{
    class Program
    {
        static void Main(string[] args)
        {
            RC2 rc2 = new RC2();
            Console.WriteLine(rc2.Encript("hello world", "hello"));
        }
    }
}
