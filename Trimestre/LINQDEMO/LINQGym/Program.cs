namespace LINQGym
{
    class Student
    {
        public int StudentID { get; set; }
        public string? StudentName { get; set; }
        public int Age { get; set; }
        public double MediaVoti { get; set; }

        public override string ToString()
        {
            return String.Format($"[StudentID = {StudentID}, StudentName = {StudentName}, Age = {Age}, MediaVoti = {MediaVoti}]");
        }
    }
    class Assenza
    {
        public int ID { get; set; }
        public DateTime Giorno { get; set; }
        public int StudentID { get; set; }
    }

    class Persona
    {
        public string? Nome { get; set; }
        public int Eta { get; set; }

        public override string ToString()
        {
            return String.Format($"[Nome = {Nome}, Età = {Eta}]");
        }
    }
    class Program
    {
        //stiamo definendo un tipo di puntatore a funzione
        delegate bool CondizioneRicerca(Student s);
        public static void AzioneSuElemento(Student s) { Console.WriteLine(s); }
        //metodo statico
        public static bool VerificaCondizione(Student s) { return s.Age >= 18 && s.Age <= 25; }

        static void Main(string[] args)
        {
            //condizione: non devono esistere due studenti con lo stesso StudentID, in questo caso si dice che StudentID è chiave primaria della collection
            Student[] studentArray1 =
            { //new Student() { StudentID = 1, StudentName = "John", Age = 18 , MediaVoti= 6.5},
            new () { StudentID = 1, StudentName = "John", Age = 18 , MediaVoti= 6.5},
            new () { StudentID = 2, StudentName = "Steve",  Age = 21 , MediaVoti= 8},
            new () { StudentID = 3, StudentName = "Bill",  Age = 25, MediaVoti= 7.4},
            new () { StudentID = 4, StudentName = "Ram" , Age = 20, MediaVoti = 10},
            new () { StudentID = 5, StudentName = "Ron" , Age = 31, MediaVoti = 9},
            new () { StudentID = 6, StudentName = "Chris",  Age = 17, MediaVoti = 8.4},
            new () { StudentID = 7, StudentName = "Rob", Age = 19  , MediaVoti = 7.7},
            new () { StudentID = 8, StudentName = "Robert", Age = 22, MediaVoti = 8.1},
            new () { StudentID = 9, StudentName = "Alexander", Age = 18, MediaVoti = 4},
            new () { StudentID = 10, StudentName = "John", Age = 18 , MediaVoti = 6},
            new () { StudentID = 11, StudentName = "John", Age = 21 , MediaVoti = 8.5},
            new () { StudentID = 12, StudentName = "Bill", Age = 25, MediaVoti = 7},
            new () { StudentID = 13, StudentName = "Ram" , Age = 20, MediaVoti = 9 },
            new () { StudentID = 14, StudentName = "Ron" , Age = 31, MediaVoti = 9.5},
            new () { StudentID = 15, StudentName = "Chris", Age = 17, MediaVoti = 8},
            new () { StudentID = 16, StudentName = "Rob", Age = 19  , MediaVoti = 7},
            new () { StudentID = 17, StudentName = "Robert", Age = 22, MediaVoti = 8},
            new () { StudentID = 18, StudentName = "Alexander", Age = 18, MediaVoti = 9},
            };
            //definiamo delle condizioni di ricerca
            //primo modo: uso di Func con lambda
            Func<Student, bool> condizioneDiRicerca = s => s.Age >= 18 && s.Age <= 25;
            //secondo modo: uso di un delegato implementato attraverso lambda
            CondizioneRicerca condizioneDiRicerca2 = s => s.Age >= 18 && s.Age <= 25;
            //terzo modo: uso di un delegato che punta a un metodo precedentemente definito
            CondizioneRicerca condizioneDiRicerca3 = VerificaCondizione;
            //quarto modo: usiamo direttamente la lambda - il più comodo
            Student[] studentResultArray;
            List<Student> studentResultList;
            //creo una lista con gli stessi oggetti presenti nell'array
            List<Student> studentList1 = studentArray1.ToList();
            //studio la clausola where
            //trovare tutti gli studenti che hanno età compresa tra 18 e 25 anni, caso dell'array
            studentResultArray = studentArray1.Where(s => s.Age >= 18 && s.Age <= 25).ToArray();
            studentResultList = studentArray1.Where(s => s.Age >= 18 && s.Age <= 25).ToList();
            //verifichiamo che il risultato sia corretto con una stampa
            Console.WriteLine("Array:");
            foreach (Student student in studentResultArray) { Console.WriteLine(student); }
            Console.WriteLine("\nLista:");
            foreach (Student student in studentResultList) { Console.WriteLine(student); }
            Console.WriteLine("\nEsempio di Action su array:");
            Array.ForEach(studentResultArray, AzioneSuElemento);
            Console.WriteLine("\nStampare media voti dell'array:");
            Array.ForEach(studentResultArray, s => {Console.Write(s.MediaVoti + " - ");});
            Console.WriteLine("\nStampare media voti della lista:");
            studentResultList.ForEach(s => Console.Write(s.MediaVoti + " - "));
        }
    }
}