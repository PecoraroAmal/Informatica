﻿namespace AggregsteExeceptionExemple
{
    public class Program
    {
        public static void Main()
        {
            var task1 = Task.Run(() => {
                throw new CustomException("This exception isexpected!");
            });
            try
            {
                task1.Wait();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    // Handle the custom exception.
                    if (e is CustomException)
                    {
                        Console.WriteLine(e.Message);
                    }
                    // Rethrow any other exception.
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
    public class CustomException : Exception
    {
        public CustomException(String message) : base(message)
        { }
    }
}
