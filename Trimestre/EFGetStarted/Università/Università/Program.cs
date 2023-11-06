using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Runtime.CompilerServices;
using Università.Data;
using Università.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Università
{
    internal class Program
    {
        static void Main(string[] args)
        {
            InitTest();
            using var db = new UniversitàContext();
            Console.WriteLine("Q1: Stampare l’elenco degli studenti");
            Q1(db);
            Console.WriteLine("Q2: Stampare l’elenco dei corsi");
            Q2(db);
            Console.WriteLine("Q3: Modificare il docente di un corso di cui è noto l’id");
            Q3(1, 4, db);
            Console.WriteLine("Q4: Stampare il numero di corsi seguiti dallo studente con id = 1");
            Q4(1,db);
            Console.WriteLine("Q5: Stampare il numero di corsi seguiti dallo studente con Nome =“Giovanni” e Cognome =“Casiraghi”");
            Q5("Giovanni", "Casiraghi",db);
            Console.WriteLine("Q6: Stampare il numero di corsi seguiti da ogni studente");
            Q6(db);
            Console.WriteLine("Q7: Stampare i corsi seguiti dallo studente con Nome =“Piero” e Cognome =“Gallo”");
            Q7(db);
        }
        static void InitTest()
        {
            UniversitàContext db = new();
            //Controllo se il database esista già
            if (db.Database.GetService<IRelationalDatabaseCreator>().Exists())
            {
                bool dbErase = false;
                if (dbErase)
                {
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                    PopulateDb();
                    Console.WriteLine("Database ricreato correttamente");
                }
            }
            else
            {
                db.Database.EnsureCreated();
                PopulateDb();
                Console.WriteLine("Database creato correttamente");
            }

            static void PopulateDb()
            {
                //1) inserisco istanze nelle tabelle che non hanno chiavi esterne -->CorsoDiLaurea, Docente
                //creo una lista di CorsoDiLaurea e di Docente
                List<Docente> docenti = new()
                {
                    new (){CodDocente=1, Cognome="Malafronte", Nome="Gennaro",Dipartimento=Dipartimento.IngegneriaInformatica },
                    new (){CodDocente=2, Cognome="Rossi", Nome="Mario", Dipartimento=Dipartimento.Matematica},
                    new (){CodDocente=3, Cognome="Verdi", Nome="Giuseppe", Dipartimento=Dipartimento.Fisica},
                    new (){CodDocente=4, Cognome= "Smith", Nome="Albert", Dipartimento=Dipartimento.Economia}
                };
                List<CorsoLaurea> corsiDiLaurea = new()
                {
                    new (){CorsoLaureaId = 1,TipoLaurea=TipoLaurea.Magistrale, Facoltà=Facoltà.Ingegneria},
                    new (){CorsoLaureaId = 2,TipoLaurea=TipoLaurea.Triennale, Facoltà=Facoltà.MatematicaFisicaScienze},
                    new (){CorsoLaureaId = 3,TipoLaurea=TipoLaurea.Magistrale, Facoltà=Facoltà.Economia},
                };
                using (var db = new UniversitàContext())
                {
                    docenti.ForEach(d => db.Add(d));
                    corsiDiLaurea.ForEach(cl => db.Add(cl));
                    db.SaveChanges();
                }
                //2) inserisco altre istanze: Inserisco istanze di Corso e di Studente
                List<Corso> corsi = new()
                {
                    new (){CodiceCorso=1,Nome="Fondamenti di Informatica 1", CodDocente=1},
                    new (){CodiceCorso=2,Nome="Analisi Matematica 1", CodDocente=2},
                    new (){CodiceCorso=3,Nome="Fisica 1", CodDocente=3},
                    new (){CodiceCorso=4, Nome="Microeconomia 1", CodDocente=4}
                };
                List<Studente> studenti = new()
                {
                    new (){Matricola=1, Nome="Giovanni", Cognome="Casiraghi", CorsoLaureaId=1, AnnoNascita=2000},
                    new (){Matricola=2, Nome="Alberto", Cognome="Angela", CorsoLaureaId=2, AnnoNascita=1999},
                    new (){Matricola=3, Nome="Piero", Cognome="Gallo", CorsoLaureaId=3, AnnoNascita=2000}
                };
                using (var db = new UniversitàContext())
                {
                    corsi.ForEach(c => db.Add(c));
                    studenti.ForEach(s => db.Add(s));
                    db.SaveChanges();
                }
                //4) inserisco le frequenze - è la tabella molti a molti
                List<Frequenta> frequenze = new()
                {
                    new (){Matricola=1, CodCorso=1},// Giovanni Casiraghi frequenta il corso di Fondamenti di Informatica 1
                    new (){Matricola=1, CodCorso=2},// Giovanni Casiraghi frequenta il corso di Analisi Matematica 1
                    new (){Matricola=2, CodCorso=2},
                    new (){Matricola=2, CodCorso=3},
                    new (){Matricola=3, CodCorso=4}
                };
                using (var db = new UniversitàContext())
                {
                    frequenze.ForEach(f => db.Add(f));
                    db.SaveChanges();
                }
            }

        }
        //Q1: Stampare l’elenco degli studenti
        public static void Q1(UniversitàContext db)
        {
            var students = db.Studenti.ToList();
            foreach (var student in students)
            {
                Console.WriteLine($"Matricola: {student.Matricola}, Nome: {student.Nome}, Cognome: {student.Cognome}");
            }
        }
        //Q2: Stampare l’elenco dei corsi
        public static void Q2(UniversitàContext db)
        {
            var courses = db.Corsi.ToList();
            foreach (var course in courses)
            {
                Console.WriteLine($"Codice Corso: {course.CodiceCorso}, Nome: {course.Nome}");
            }
        }

        //Q3: Modificare il docente di un corso di cui è noto l’id
        public static void Q3(int codCorso, int nuovoCodDocente, UniversitàContext db)
        {
            var course = db.Corsi.Find(codCorso);
            if (course != null)
            {
                course.CodDocente = nuovoCodDocente;
                db.SaveChanges();
                Console.WriteLine($"Docente del corso con ID {codCorso} modificato con successo.");
            }
            else
            {
                Console.WriteLine($"Corso con ID {codCorso} non trovato.");
            }
        }

        //Q4: Stampare il numero di corsi seguiti dallo studente con id = 1
        public static void Q4(int id, UniversitàContext db)
        {
            var student = db.Studenti.Include(s => s.Frequenze).FirstOrDefault(s => s.Matricola == id);
            if (student != null)
            {
                var numberOfCourses = student.Frequenze.Count;
                Console.WriteLine($"Lo studente con ID {id} segue {numberOfCourses} corsi.");
            }
            else
            {
                Console.WriteLine($"Studente con ID {id} non trovato.");
            }
        }

        //Q5: Stampare il numero di corsi seguiti dallo studente con Nome =“Giovanni” e Cognome =“Casiraghi”
        public static void Q5(string nome, string cognome, UniversitàContext db)
        {
            var student = db.Studenti.Include(s => s.Frequenze)
                                    .FirstOrDefault(s => s.Nome == nome && s.Cognome == cognome);
            if (student != null)
            {
                var numberOfCourses = student.Frequenze.Count;
                Console.WriteLine($"{nome} {cognome} segue {numberOfCourses} corsi.");
            }
            else
            {
                Console.WriteLine($"Studente con Nome {nome} e Cognome {cognome} non trovato.");
            }
        }

        //Q6: Stampare il numero di corsi seguiti da ogni studente
        public static void Q6(UniversitàContext db)
        {
            var students = db.Studenti.Include(s => s.Frequenze).ToList();
            foreach (var student in students)
            {
                var numberOfCourses = student.Frequenze.Count;
                Console.WriteLine($"Lo studente con ID {student.Matricola} segue {numberOfCourses} corsi.");
            }
        }

        //Q7: Stampare i corsi seguiti dallo studente con Nome =“Piero” e Cognome =“Gallo”
        public static void Q7(UniversitàContext db)
        {
            var student = db.Studenti.Include(s => s.Frequenze)
                                    .FirstOrDefault(s => s.Nome == "Piero" && s.Cognome == "Gallo");
            if (student != null)
            {
                Console.WriteLine($"Corsi seguiti da Piero Gallo:");
                foreach (var frequenza in student.Frequenze)
                {
                    var course = db.Corsi.Find(frequenza.CodCorso);
                    if (course != null)
                    {
                        Console.WriteLine($"Codice Corso: {course.CodiceCorso}, Nome: {course.Nome}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Studente Piero Gallo non trovato.");
            }
        }


    }
}