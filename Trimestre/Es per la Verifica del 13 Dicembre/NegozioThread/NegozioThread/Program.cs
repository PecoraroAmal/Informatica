using System;
using System.Threading;

namespace NegozioThread

{
    static class Program
    {

        static object lockObject = new object();
        static bool negozioAperto = true;
        static int clientiPresenti = 0;
        static decimal ricavoGiornaliero = 0;

        static void Main()
        {
            Thread entraClienteThread = new Thread(EntraCliente);
            Thread esceClienteThread = new Thread(EsceCliente);
            Thread chiudiNegozioThread = new Thread(ChiudiNegozio);

            entraClienteThread.Start();
            esceClienteThread.Start();
            chiudiNegozioThread.Start();

            entraClienteThread.Join();
            esceClienteThread.Join();
            chiudiNegozioThread.Join();

            Console.WriteLine($"Ricavo giornaliero: {ricavoGiornaliero} euro");
        }

        static void EntraCliente()
        {
            Random random = new Random();

            while (negozioAperto)
            {
                Thread.Sleep(1500);
                lock (lockObject)
                {
                    if (clientiPresenti < 5)
                    {
                        clientiPresenti++;
                        Console.WriteLine($"Cliente entrato. Clienti presenti: {clientiPresenti}");
                    }
                }
            }
        }

        static void EsceCliente()
        {
            Random random = new Random();

            while (negozioAperto)
            {
                Thread.Sleep(5500);
                lock (lockObject)
                {
                    if (clientiPresenti > 0)
                    {
                        clientiPresenti--;
                        ricavoGiornaliero += 20;
                        Console.WriteLine($"Cliente uscito. Clienti presenti: {clientiPresenti}.");
                    }
                }
            }
        }

        static void ChiudiNegozio()
        {
            Thread.Sleep(15000);

            lock (lockObject)
            {
                negozioAperto = false;
                Console.WriteLine("Negozio chiuso.");
                clientiPresenti = 0;
            }
        }
    }
}
