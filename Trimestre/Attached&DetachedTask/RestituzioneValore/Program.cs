namespace RestituzioneValore
{
    internal class Program
    {
        static void Main()
        {
            Detached();
            Attached();
            NoAttached();
        }
        static void Detached()
        {
            Console.WriteLine("Esempio di Detached task figlio");
            var outer = Task<int>.Factory.StartNew(() => {
                Console.WriteLine("Outer task executing.");
                var nested = Task<int>.Factory.StartNew(() => {
                    Console.WriteLine("Nested task starting.");
                    Thread.SpinWait(5000000);
                    Console.WriteLine("Nested task completing.");
                    return 42;
                });
                // Parent will wait for this detached child.
                return nested.Result;
            });
            Console.WriteLine("Outer has returned {0}.", outer.Result);
        }
        public static void Attached()
        {
            Console.WriteLine("Esempio di Attached task figlio");
            var parent = Task.Factory.StartNew(() => {
                Console.WriteLine("Parent task executing.");
                var child = Task.Factory.StartNew(() => {
                    Console.WriteLine("Attached child starting.");
                    Thread.SpinWait(5000000);
                    Console.WriteLine("Attached child completing.");
                }, TaskCreationOptions.AttachedToParent);
            });
            parent.Wait();
            Console.WriteLine("Parent has completed.");
        }
        public static void NoAttached()
        {
            Console.WriteLine("Il genitore vieta il collegamento con il figlio tramite TaskCreationOptions.DenyChildAttach");
            var parent = Task.Factory.StartNew(() => {
                Console.WriteLine("Parent task executing.");
                var child = Task.Factory.StartNew(() => {
                    Console.WriteLine("Attached child starting.");
                    Thread.SpinWait(5000000);
                    Console.WriteLine("Attached child completing.");
                }, TaskCreationOptions.AttachedToParent);
            }, TaskCreationOptions.DenyChildAttach);
            parent.Wait();
            Console.WriteLine("Parent has completed.");
        }
        //usare task.run è come usare startenew con l'opzione TaskCreationOptions.DenyChildAttach
    }
}
