﻿namespace Esempio3
{
    class Student
    {
        public int StudentID { get; set; }
        public string? StudentName { get; set; }
        public int Age { get; set; }

        public override string ToString()
        {
            return string.Format($"[StudentID = {StudentID}, StudentName = {StudentName}, Age = {Age}]");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Student[] studentArray =
            {
            new Student() { StudentID = 1, StudentName = "John", Age = 18},
            new Student() { StudentID = 2, StudentName = "Steve",  Age = 21},
            new Student() { StudentID = 3, StudentName = "Bill",  Age = 25},
            new Student() { StudentID = 4, StudentName = "Ram" , Age = 20},
            new Student() { StudentID = 5, StudentName = "Ron" , Age = 31},
            new Student() { StudentID = 6, StudentName = "Chris",  Age = 17},
            new Student() { StudentID = 7, StudentName = "Rob", Age = 19},
            };
            // Use LINQ to find teenager students
            Student[] teenAgerStudents = studentArray.Where(s => s.Age > 12 && s.Age < 20).ToArray();
            // Use LINQ to find first student whose name is Bill 
            Student? bill = studentArray.Where(s => s.StudentName == "Bill").FirstOrDefault();//tolist se voglio tutti gli studenti con un certo nome
            // Use LINQ to find student whose StudentID is 5
            Student? student5 = studentArray.Where(s => s.StudentID == 5).FirstOrDefault();
            //write result
            foreach (var studente in teenAgerStudents)
            {
                Console.WriteLine(studente);
            }
            Console.WriteLine("bill: " + bill);
            Console.WriteLine("student5: " + student5);
        }
    }
}