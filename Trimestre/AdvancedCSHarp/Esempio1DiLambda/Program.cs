namespace Esempio1DiLambda
{
    public class Program
    {
        delegate bool IsTeenAger(Student stud);
        delegate bool IsYoungerThan(Student stud, int youngAge);
        public static void Main()
        {
            //IsTeenAger isTeenAger = delegate (Student s) 
            //{
            //    return s.Age > 12 && s.Age < 20;
            //};
            IsTeenAger isTeenAger = s => s.Age > 12 && s.Age < 20;
            IsYoungerThan isYoungerThan = (s, i) => s.Age < i;
            Student stud = new Student() { Age = 25 };
            Console.WriteLine(isTeenAger(stud));
            Console.WriteLine(isYoungerThan(stud, 26));
            Console.ReadLine();
        }
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}