using AsyncBreakfast;
using System.Diagnostics;

namespace AsyncBreakfast
{
    internal class Bacon
    {
    }
    internal class Egg
    {
    }
    internal class Coffee
    {
    }
    internal class Juice
    {
    }
    internal class Toast
    {
    }
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Prepariamo la colazione - versione sincrona");
            ColazioneSincrona();
            Console.WriteLine("\n\nPrepariamo la colazione - versione parallela");
            ColazioneParallela();
            //Prepariamo la colazione - versione asincrona
            Console.WriteLine("\n\nPrepariamo la colazione - versione asincrona");
            await ColazioneAsincrona();
            //Prepariamo la colazione - versione asincrona ottimizzata
            Console.WriteLine("\n\nPrepariamo la colazione - versione asincrona ottimizzata");
            await ColazioneAsincronaOttimizzata();
        }
        private static void ColazioneSincrona()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Coffee cup = PourCoffee();
            Console.WriteLine("coffee is ready");
            List<Egg> eggs = FryEggs(2);
            Console.WriteLine("eggs are ready");
            List<Bacon> bacon = FryBacon(3);
            Console.WriteLine("bacon is ready");
            List<Toast> toast = ToastBread(2);
            ApplyButter(toast);
            ApplyJam(toast);
            Console.WriteLine("toast is ready");
            Juice oj = PourOJ();
            Console.WriteLine("oj is ready");
            Console.WriteLine("Breakfast is ready!");
            sw.Stop();
            Console.WriteLine($"Il tempo per la colazione sincrona è {sw.ElapsedMilliseconds} ms");
        }
        private static void ColazioneParallela()
        {
            //in questo esempio ci sono alcune attività che sono svolte in parallelo
            //e alcune attività che sono svolte in maniera sincrona
            Stopwatch sw = new Stopwatch();
            sw.Start();
            PourCoffee();//attività sincrona
            Console.WriteLine("coffee is ready");
            Task<List<Egg>> eggs = Task.Factory.StartNew(() => FryEggs(2));
            Task<List<Bacon>> bacon = Task.Factory.StartNew(() => FryBacon(3));
            Task<List<Toast>> toast = Task.Factory.StartNew(() => ToastBread(2));
            toast.Wait();
            ApplyButter(toast);//attività sincrona
            ApplyJam(toast);//attività sincrona
            Console.WriteLine("toast is ready");
            Juice oj = PourOJ();//attività sincrona
            Console.WriteLine("oj is ready");
            //quando tutto è pronto la colazione è pronta
            Task.WaitAll(eggs, bacon, toast);
            Console.WriteLine("eggs are ready");
            Console.WriteLine("bacon is ready");
            Console.WriteLine("Breakfast is ready!");
            sw.Stop();
            Console.WriteLine($"Il tempo per la colazione parallela è {sw.ElapsedMilliseconds} ms");
        }

        private static async Task ColazioneAsincrona()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            PourCoffee();
            //un metodo asincrono restituisce un Task
            //Il task può non restituire nulla, nel caso di azione asincrona (operazione che non restituisce alcun valore al chiamante)
            //oppure può restituire un oggetto, come ad esempio nel caso di una lista di Egg o di Bacon
            //ci sono due modi per utilizzare un metodo asincrono:
            //1) aspettare il completamento del task restituito dal metodo asincrono, come nel caso di:
            //Task<List<Egg>> eggs = FryEggsAsync(2);
            //2) aspettare in maniera asincrona direttamente il risultato del task, come nel caso di
            //List<Toast> toast = await MakeToastWithButterAndJamAsync();
            //in questo secondo caso si utilizza la keyword await, che vuol dire asyncronous wait.
            //con l'utilizzo della keyword await dopo il segno di = si ottiene non un Task, ma direttamente il valore restituito
            //dal task, se esiste, oppure si attende la fine del task se questo non restituisce nessun valore
            Task<List<Egg>> eggs = FryEggsAsync(2);
            Task<List<Bacon>> bacon = FryBaconAsync(3);
            List<Toast> toast = await MakeToastWithButterAndJamAsync(2);
            await bacon;
            Console.WriteLine("bacon is ready");
            await eggs;
            Console.WriteLine("eggs are ready");
            Console.WriteLine("toast is ready");
            Juice oj = PourOJ();
            Console.WriteLine("oj is ready");
            //quando tutto è pronto la colazione è pronta
            Console.WriteLine("Breakfast is ready!");
            sw.Stop();
            Console.WriteLine($"Il tempo per la colazione asincrona è {sw.ElapsedMilliseconds} ms");
        }
        private static async Task ColazioneAsincronaOttimizzata()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Coffee cup = PourCoffee();
            Console.WriteLine("coffee is ready");
            var eggsTask = FryEggsAsync(2);
            var baconTask = FryBaconAsync(3);
            var toastTask = MakeToastWithButterAndJamAsync(2);
            var breakfastTasks = new List<Task> { eggsTask, baconTask, toastTask };
            while (breakfastTasks.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(breakfastTasks);
                if (finishedTask == eggsTask)
                {
                    Console.WriteLine("eggs are ready");
                }
                else if (finishedTask == baconTask)
                {
                    Console.WriteLine("bacon is ready");
                }
                else if (finishedTask == toastTask)
                {
                    Console.WriteLine("toast is ready");
                }
                breakfastTasks.Remove(finishedTask);
            }
            Juice oj = PourOJ();
            Console.WriteLine("oj is ready");
            Console.WriteLine("Breakfast is ready!");
            sw.Stop();
            Console.WriteLine($"Il tempo per la colazione asincrona è {sw.ElapsedMilliseconds} ms");
        }
        private static Coffee PourCoffee()
        {
            Console.WriteLine($"Sto iniziando a preparare il caffè");
            Task.Delay(1000).Wait();
            return new Coffee();
        }
        //Un metodo asincrono è definito tramite la keyword async e restituisce un  Task<TResult> oppure Task se non restituisce nulla
        //la keyword async si utilizza in combinazione con la keyword await all'interno del metodo
        private static async Task<List<Egg>> FryEggsAsync(int v)
        {
            Console.WriteLine($"Sto iniziando a friggere {v} uova");
            List<Egg> uova = new List<Egg>();
            for (int i = 0; i < v; i++)
            {
                await Task.Delay(200);
                uova.Add(new Egg());
            }
            return uova;
        }
        private static async Task<List<Bacon>> FryBaconAsync(int v)
        {
            Console.WriteLine($"Sto iniziando a friggere {v} fette di pancetta");
            List<Bacon> fetteDiPancetta = new List<Bacon>();
            for (int i = 0; i < v; i++)
            {
                await Task.Delay(200);
                fetteDiPancetta.Add(new Bacon());
            }
            return fetteDiPancetta;
        }
        private static async Task<List<Toast>> MakeToastWithButterAndJamAsync(int v)
        {
            List<Toast> toast = await ToastBreadAsync(v);
            //await toast;
            ApplyButter(toast);//attività sincrona
            ApplyJam(toast);//attività sincrona
            return toast;
        }
        private static async Task<List<Toast>> ToastBreadAsync(int v)
        {
            Console.WriteLine($"Sto iniziando a tostare {v} fette di pane");
            List<Toast> toasts = new List<Toast>();
            for (int i = 0; i < v; i++)
            {
                Console.WriteLine($"\tTosto la {i + 1}-ma fetta");
                await Task.Delay(200);
                toasts.Add(new Toast());
            }
            return toasts;

        }

        //usa gli stessi metodi della versione sincrona, ma in task paralleli, ad eccezione dei seguenti metodi:
        private static void ApplyButter(Task<List<Toast>> toast)
        {
            Console.WriteLine("Sto iniziando a spalmare il burro ");
            for (int i = 0; i < toast.Result.Count; i++)
            {
                Task.Delay(300).Wait();
                Console.WriteLine($"\tSto spalmando il burro sulla {i + 1}-ma fetta ");
            }
        }
        private static void ApplyJam(Task<List<Toast>> toast)
        {
            Console.WriteLine("Sto iniziando a spalmare la marlellata ");
            for (int i = 0; i < toast.Result.Count; i++)
            {
                Task.Delay(500).Wait();
                Console.WriteLine($"\tSto spalmando la marmellata sulla {i + 1}-ma fetta");
            }
        }
        private static List<Egg> FryEggs(int v)
        {
            Console.WriteLine($"Sto iniziando a friggere {v} uova");
            List<Egg> uova = new List<Egg>();
            for (int i = 0; i < v; i++)
            {
                // https://stackoverflow.com/questions/20082221/when-to-use-task-delay-whento-use-thread-sleep
                Task.Delay(200).Wait();
                uova.Add(new Egg());
            }
            return uova;
        }
        private static List<Bacon> FryBacon(int v)
        {
            Console.WriteLine($"Sto iniziando a friggere {v} fette di pancetta");
            List<Bacon> fetteDiPancetta = new List<Bacon>();
            for (int i = 0; i < v; i++)
            {
                Task.Delay(200).Wait();
                fetteDiPancetta.Add(new Bacon());
            }
            return fetteDiPancetta;
        }
        private static List<Toast> ToastBread(int v)
        {
            Console.WriteLine($"Sto iniziando a tostare {v} fette di pane");
            List<Toast> toasts = new List<Toast>();
            for (int i = 0; i < v; i++)
            {
                Console.WriteLine($"\tTosto la {i + 1}-ma fetta");
                Task.Delay(200).Wait();
                toasts.Add(new Toast());
            }
            return toasts;
        }
        private static void ApplyButter(List<Toast> toast)
        {
            Console.WriteLine("Sto iniziando a spalmare il burro ");
            for (int i = 0; i < toast.Count; i++)
            {
                Task.Delay(300).Wait();
                Console.WriteLine($"\tSto spalmando il burro sulla {i + 1}-ma fetta ");
            }
        }
        private static void ApplyJam(List<Toast> toast)
        {
            Console.WriteLine("Sto iniziando a spalmare la marlellata ");
            for (int i = 0; i < toast.Count; i++)
            {
                Task.Delay(500).Wait();
                Console.WriteLine($"\tSto spalmando la marmellata sulla {i + 1}-ma fetta");
            }
        }
        private static Juice PourOJ()
        {
            Console.WriteLine("Sto iniziando a spremere le arance");
            Task.Delay(1000).Wait();
            return new Juice();
        }
    }
}
