﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace LockKeyword
{
    class Program
    {
        private static int sum;
        private static readonly object _lock = new object();
        static void Main(string[] args)
        {
            //create thread t1 using anonymous method
            Thread t1 = new Thread(() => {
                for (int i = 0; i < 10000000; i++)
                {
                    lock (_lock)//Mutex -> Monitor in mutua esclusione
                    {
                        //increment sum value
                        sum++;
                    }
                }
            });
            //create thread t2 using anonymous method
            Thread t2 = new Thread(() => {
                for (int i = 0; i < 10000000; i++)
                {
                    lock (_lock)
                    {
                        //increment sum value
                        sum++;
                    }
                }
            });
            //start thread t1 and t2
            t1.Start();
            t2.Start();
            //wait for thread t1 and t2 to finish their execution
            t1.Join();
            t2.Join();
            //write final sum on screen
            Console.WriteLine("sum: " + sum);
            Console.WriteLine("Press enter to terminate!");
            Console.ReadLine();
        }
    }
}