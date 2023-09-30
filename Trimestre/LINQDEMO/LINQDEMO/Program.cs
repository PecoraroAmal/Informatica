namespace LINQDEMO
{
    internal class Program
    {
        static void Main()
        {
            // Data source
            string[] names = { "Bill", "Steve", "James", "Johan" };

            // LINQ Query 
            var myLinqQuery = from name in names
                              where name.Contains('a')
                              select name;
            // Query execution
            foreach (var name in myLinqQuery)
                Console.Write(name + " ");
        }
    }
}