﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace DataPartition01
{
    //Si può applicare solo in casi in cui abbiamo strutture dati complesse
    class Program
    {
        private static int[] array;
        private static int sum1;
        private static int sum2;
        static void Main(string[] args)
        {
            //set length for the array size
            int length = 1000000;
            //create new array of size lenght
            array = new int[length];
            //initialize array element with value of their respective index
            for (int i = 0; i < length; i++)
            {
                array[i] = i;
            }
            //index to split on
            int dataSplitAt = length / 2;
            //create thread t1 using anonymous method
            Thread t1 = new Thread(() =>
            {
                //calculate sum1
                for (int i = 0; i < dataSplitAt; i++)
                {
                    sum1 = sum1 + array[i];
                }
            });
            //create thread t2 using anonymous method
            Thread t2 = new Thread(() =>
            {
                //calculate sum2
                for (int i = dataSplitAt; i < length; i++)
                {
                    sum2 = sum2 + array[i];
                }
            });
            //start thread t1 and t2
            t1.Start();
            t2.Start();
            //wait for thread t1 and t2 to finish their execution
            t1.Join();
            t2.Join();
            //calculate final sum
            int sum = sum1 + sum2;
            //write final sum on screen
            Console.WriteLine("Sum:" + sum);
            Console.WriteLine("Press enter to terminate!");
            Console.ReadLine();
        }
    }
}