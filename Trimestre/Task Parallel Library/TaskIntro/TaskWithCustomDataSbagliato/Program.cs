namespace TaskWithCustomDataSbagliato
{
    class CustomData
    {
        public long CreationTime;
        public int Name;
        public int ThreadNum;
    }
    class Program
    {
        public static void Main()
        {
            Task[] taskArray = new Task[10];
            for (int i = 0; i < taskArray.Length; i++)
            {
                taskArray[i] = Task.Factory.StartNew((object? obj) => {
                    var data = new CustomData() { Name = i, CreationTime = DateTime.Now.Ticks };
                    data.ThreadNum = Thread.CurrentThread.ManagedThreadId;
                    Console.WriteLine($"Task Name = {data.Name}, created at {data.CreationTime}, ran on Thread Id = {data.ThreadNum}.");
                }, i);
            }
            Task.WaitAll(taskArray);
            foreach (var task in taskArray)
            {
                //AsyncState restituisce l'oggetto che è stato passato quando il Task è stato creato, oppure null se non è stato passato nulla
                var data = task.AsyncState as int?;
                if (data != null)
                    Console.WriteLine($"Supplied object= {data}, Task Id = {task.Id}, Task status = {task.Status}.");
            }
        }
    }
}