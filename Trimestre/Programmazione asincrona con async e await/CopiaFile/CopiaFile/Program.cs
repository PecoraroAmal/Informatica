using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
namespace CopiaFile
{
    class Program
    {
        static async Task CopyFiles(string startDirectory, string endDirectory)
        {
            foreach (string fileName in Directory.EnumerateFiles(startDirectory))
            {
                using (FileStream SourceStream = File.Open(fileName, FileMode.Open))
                {
                    using (FileStream DestinationStream = File.Create(endDirectory + fileName.Substring(fileName.LastIndexOf('\\'))))
                    {
                        await SourceStream.CopyToAsync(DestinationStream);
                        //introduco appositamente un ritardo per poter apprezzare il tempo che passa
                        //Thread.Sleep(20);
                        await Task.Delay(20);
                    }
                }
            }
            foreach (string directoryName in Directory.EnumerateDirectories(startDirectory))
            {
                //creo la cartella destinazione
                DirectoryInfo destinationDir = Directory.CreateDirectory(endDirectory +
               directoryName.Substring(directoryName.LastIndexOf('\\')));
                //chiamata ricorsiva del metodo
                await CopyFiles(directoryName, destinationDir.FullName);
            }
            //la copia poteva essere fatta anche come descritto qui:
            // https://docs.microsoft.com/en-us/dotnet/api/system.io.directoryinfo
        }
        static async Task Main(string[] args)
        {
            //creo un task che copia i file
            //i percorsi sorgente e destinazione devono esistere
            string StartDirectory = @"C:\Users\Amal\Documents\MPLAB Mindi";
            string EndDirectory = @"C:\Users\Amal\Documents\Momentanea";
            Task copyTask = CopyFiles(StartDirectory, EndDirectory);
            //creo un task che perde tempo, ma che interrompo appena ho finito a copiare i task
            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;
            //lancio il task perdiTempo
            Task perdiTempo = Task.Run(() => {
                ct.ThrowIfCancellationRequested();
                int count = 0;
                while (true)
                {
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine($"Sono le ore {DateTime.Now} e tutto va bene, è la {count++} volta");
                    Thread.Sleep(100);
                    // Poll on this property if you have to do
                    // other cleanup before throwing.
                    if (ct.IsCancellationRequested)
                    {
                        // Clean up here, then...
                        ct.ThrowIfCancellationRequested();
                    }
                }
            }, ct);
            //avvio il copytask
            await copyTask;
            Console.WriteLine("Copia effettuata correttamente");
            //appena ho finito di copiare interrompo il task perdiTempo
            tokenSource.Cancel();
            try
            {
                await perdiTempo;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Interrotto il Task Perditempo");
            }
            finally
            {
                tokenSource.Dispose();
            }
        }
    }
}
