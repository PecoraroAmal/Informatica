using System;
using System.Threading;
using System.Threading.Tasks;

namespace NegozioTask
{
    class Program
    {
        static object lockObject = new object();
        static bool negozioAperto = true;
        static int clientiPresenti = 0;
        static decimal ricavoGiornaliero = 0;

        static async Task Main()
        {
            Task entraClienteTask = Task.Run(() => EntraCliente());
            Task esceClienteTask = Task.Run(() => EsceCliente());
            Task chiudiNegozioTask = Task.Run(() => ChiudiNegozio());

            await Task.WhenAll(entraClienteTask, esceClienteTask, chiudiNegozioTask);

            Console.WriteLine($"Ricavo giornaliero: {ricavoGiornaliero} euro");
        }

        static void EntraCliente()
        {
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
