using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogUsers
{
    using System.Threading;

    using LogTest;

    class Program
    {
        static void Main(string[] args)
        {
            ILog  logger = new AsyncLog();

            for (int i = 0; i < 15; i++)
            {
                logger.Write("Number with Flush: " + i.ToString());
                Thread.Sleep(50);
            }

            logger.StopWithFlush();

            ILog logger2 = new AsyncLog();

            for (int i = 5000; i > 0; i--)
            {
                logger2.Write("Number with No flush: " + i.ToString());

            }

            logger2.StopWithoutFlush();

            Console.ReadLine();

			Console.WriteLine("Day Change: " + (new Tests ()).TestDayChange ());
			Console.WriteLine("Line Logged: " + (new Tests ()).TestCallToWrite ());
			Console.WriteLine("Stop With Flush: " + (new Tests ()).TestStopWithFlush ());
			Console.WriteLine("Stop Without Flush (test not deterministic!): " + (new Tests ()).TestStopWithoutFlush ());
        }
    }
}
