namespace TaskWithCustomData
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
                taskArray[i] = Task.Factory.StartNew((object? obj) =>
                {
                    CustomData data = obj as CustomData;
                    if (data == null)
                        return;
                    data.ThreadNum = Thread.CurrentThread.ManagedThreadId;
                }, new CustomData() { Name = i, CreationTime = DateTime.Now.Ticks });
            }
            Task.WaitAll(taskArray);
            foreach (var task in taskArray)
            {
                //AsyncState restituisce l'oggetto che è stato passato quando il Task è stato creato, oppurenull se non è stato passato nulla
                var data = task.AsyncState as CustomData;
                if (data != null)
                    Console.WriteLine($"Task Name = {data.Name}, Task Id = {task.Id}, Task status = {task.Status}, created at {data.CreationTime}, ran on Thread Id = {data.ThreadNum}.");
            }
        }
    }
}