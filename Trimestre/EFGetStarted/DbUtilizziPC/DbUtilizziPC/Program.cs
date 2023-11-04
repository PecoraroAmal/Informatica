using DbUtilizziPC.Data;
using DbUtilizziPC.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace DbUtilizziPC;
internal class Program
{
    static void CreazioneDb(UtilizziPCContext db)
    {
        //verifichiamo se il database esista già
        if (db.Database.GetService<IRelationalDatabaseCreator>().Exists())
        {
            bool dbErase = false;
            if (dbErase)
            {
                //cancelliamo il database se esiste
                db.Database.EnsureDeleted();
                //ricreiamo il database a partire dal model (senza dati --> tabelle vuote)
                db.Database.EnsureCreated();
                //inseriamo i dati nelle tabelle
                PopulateDb(db);
            }
        }
        else //il database non esiste
        {
            //ricreiamo il database a partire dal model (senza dati --> tabelle vuote)
            db.Database.EnsureCreated();
            //popoliamo il database
            PopulateDb(db);
        }

        static void PopulateDb(UtilizziPCContext db)
        {
            //Creazione dei Clienti - gli id vengono generati automaticamente come campi auto-incremento quando si effettua l'inserimento, tuttavia
            //è bene inserire esplicitamente l'id degli oggetti quando si procede all'inserimento massivo gli elementi mediante un foreach perché
            //EF core potrebbe inserire nel database gli oggetti in un ordine diverso rispetto a quello del foreach
            List<Classe> classi = new()
            {
                new (){Id =1, Nome="3IA", Aula="Est 1"},
                new (){Id =2,Nome="4IA", Aula="A32"},
                new (){Id =3,Nome="5IA", Aula="A31"},
                new (){Id =4,Nome="3IB", Aula="Est 2"},
                new (){Id =5,Nome="4IB", Aula="A30"},
                new (){Id =6,Nome="5IB", Aula="A32"},
            };

            List<Studente> studenti = new()
            {
                new (){Id = 1, Nome = "Mario", Cognome = "Rossi", ClasseId =1 },
                new (){Id = 2, Nome = "Giovanni", Cognome = "Verdi", ClasseId =1 },
                new (){Id = 3, Nome = "Piero", Cognome = "Angela", ClasseId = 1 },
                new (){Id = 4, Nome = "Leonardo", Cognome = "Da Vinci", ClasseId = 1 },
                new (){Id = 50, Nome = "Cristoforo", Cognome = "Colombo", ClasseId=2 },
                new (){Id = 51, Nome = "Piero", Cognome = "Della Francesca", ClasseId=2 },
                new (){Id = 82, Nome = "Alessandro", Cognome = "Manzoni", ClasseId=4 },
                new (){Id = 83, Nome = "Giuseppe", Cognome = "Parini", ClasseId=4 },
                new (){Id = 102, Nome = "Giuseppe", Cognome = "Ungaretti", ClasseId=3 },
                new (){Id = 103, Nome = "Luigi", Cognome = "Pirandello", ClasseId=3 },
                new (){Id = 131, Nome = "Enrico", Cognome = "Fermi", ClasseId=6 },
                new (){Id = 132, Nome = "Sandro", Cognome = "Pertini", ClasseId=6 },
            };

            List<Computer> computers = new()
            {
                new (){Id = 1, Modello="Hp 19 inc. 2019", Collocazione = "Bunker-D1-D5"},
                new (){Id = 2, Modello="Hp 19 inc. 2019", Collocazione = "Bunker-D1-D5"},
                new (){Id = 3, Modello="Hp 19 inc. 2019", Collocazione = "Bunker-D1-D5"},
                new (){Id = 4, Modello="Hp 19 inc. 2019", Collocazione = "Bunker-D1-D5"},
                new (){Id = 5, Modello="Hp 19 inc. 2019", Collocazione = "Bunker-D1-D5"},
                new (){Id = 6, Modello="Hp 19 inc. 2019", Collocazione = "Bunker-D6-D10"},
                new (){Id = 7, Modello="Hp 19 inc. 2019", Collocazione = "Bunker-D6-D10"},
                new (){Id = 8, Modello="Hp 19 inc. 2019", Collocazione = "Bunker-D6-D10"},
                new (){Id = 9, Modello="Hp 19 inc. 2019", Collocazione = "Bunker-D6-D10"},
                new (){Id = 10, Modello="Hp 19 inc. 2019", Collocazione = "Bunker-D6-D10"},
                new (){Id = 20, Modello="Lenovo i5 2020", Collocazione = "Bunker-D20-D25"},
                new (){Id = 21, Modello="Lenovo i5 2020", Collocazione = "Bunker-D20-D25"},
                new (){Id = 22, Modello="Lenovo i5 2020", Collocazione = "Bunker-D20-D25"},
                new (){Id = 23, Modello="Lenovo i5 2020", Collocazione = "Bunker-D20-D25"},
                new (){Id = 24, Modello="Lenovo i5 2020", Collocazione = "Bunker-D20-D25"},
                new (){Id = 61, Modello="Lenovo i5 2021", Collocazione = "Carrello-Mobile-S1"},
                new (){Id = 62, Modello="Lenovo i5 2021", Collocazione = "Carrello-Mobile-S2"},
                new (){Id = 63, Modello="Lenovo i5 2021", Collocazione = "Carrello-Mobile-S3"},
                new (){Id = 64, Modello="Lenovo i5 2021", Collocazione = "Carrello-Mobile-S4"},
                new (){Id = 65, Modello="Lenovo i5 2021", Collocazione = "Carrello-Mobile-S5"},
            };

            List<Utilizza> utilizzi = new()
            {
                new (){ComputerId = 61,StudenteId=1,
                    DataOraInizioUtilizzo = DateTime.Now.Add(- new TimeSpan(1,12,0)),
                    DataOraFineUtilizzo = DateTime.Now},
                new (){ComputerId = 61,StudenteId=1,
                    DataOraInizioUtilizzo = DateTime.Now.Add(- new TimeSpan(1,1,12,0)),
                    DataOraFineUtilizzo = DateTime.Now.Add(- new TimeSpan(1,0,0,0))},
                new (){ComputerId = 61,StudenteId=3,
                    DataOraInizioUtilizzo = DateTime.Today.AddDays(-2).AddHours(11),
                    DataOraFineUtilizzo = DateTime.Today.AddDays(-2).AddHours(12)},
                new (){ComputerId = 61,StudenteId=82,
                    DataOraInizioUtilizzo = DateTime.Today.AddDays(-1).AddHours(12),
                    DataOraFineUtilizzo = DateTime.Today.AddDays(-1).AddHours(13) },
                new (){ComputerId = 61,StudenteId=1,
                    DataOraInizioUtilizzo = DateTime.Today.AddHours(11),
                    DataOraFineUtilizzo = DateTime.Today.AddHours(12) },
                new (){ComputerId = 62,StudenteId=2,
                    DataOraInizioUtilizzo = DateTime.Today.AddDays(-2).AddHours(11),
                    DataOraFineUtilizzo = DateTime.Today.AddDays(-2).AddHours(12) },
                new (){ComputerId = 62,StudenteId=2,
                    DataOraInizioUtilizzo = DateTime.Today.AddDays(-1).AddHours(12),
                    DataOraFineUtilizzo = DateTime.Today.AddDays(-1).AddHours(13) },
                new (){ComputerId = 62,StudenteId=4,
                    DataOraInizioUtilizzo = DateTime.Today.AddHours(11),
                    DataOraFineUtilizzo = DateTime.Today.AddHours(11) },
                new (){ComputerId = 1,StudenteId=50,
                    DataOraInizioUtilizzo = DateTime.Today.AddDays(-2).AddHours(11),
                    DataOraFineUtilizzo = DateTime.Today.AddDays(-2).AddHours(12) },
                new (){ComputerId = 1,StudenteId=103,
                    DataOraInizioUtilizzo = DateTime.Today.AddDays(-1).AddHours(12),
                    DataOraFineUtilizzo = DateTime.Today.AddDays(-1).AddHours(13) },
                new (){ComputerId = 1,StudenteId=50,
                    DataOraInizioUtilizzo = DateTime.Today.AddHours(11),
                    DataOraFineUtilizzo = DateTime.Today.AddHours(12) },
                new (){ComputerId = 2,StudenteId=51,
                    DataOraInizioUtilizzo = DateTime.Today.AddDays(-1).AddHours(11),
                    DataOraFineUtilizzo = DateTime.Today.AddDays(-1).AddHours(12) },
                new (){ComputerId = 2,StudenteId=51,
                    DataOraInizioUtilizzo = DateTime.Today.AddDays(-1).AddHours(12),
                    DataOraFineUtilizzo = DateTime.Today.AddDays(-1).AddHours(13) },
                new (){ComputerId = 2,StudenteId=103,
                    DataOraInizioUtilizzo = DateTime.Today.AddHours(11),
                    DataOraFineUtilizzo = DateTime.Today.AddHours(12) },
                new (){ComputerId = 3,StudenteId=82,
                    DataOraInizioUtilizzo = DateTime.Today.AddDays(-2).AddHours(11),
                    DataOraFineUtilizzo = DateTime.Today.AddDays(-2).AddHours(12) },
                new (){ComputerId = 3,StudenteId=82,
                    DataOraInizioUtilizzo = DateTime.Today.AddDays(-1).AddHours(11),
                    DataOraFineUtilizzo = DateTime.Today.AddDays(-1).AddHours(13) },
                new (){ComputerId = 3,StudenteId=83,
                    DataOraInizioUtilizzo = DateTime.Today.AddHours(11),
                    DataOraFineUtilizzo = DateTime.Today.AddHours(12) },
                new (){ComputerId = 20,StudenteId=102,
                    DataOraInizioUtilizzo = DateTime.Today.AddDays(-2).AddHours(11),
                    DataOraFineUtilizzo = DateTime.Today.AddDays(-2).AddHours(12) },
                new (){ComputerId = 20,StudenteId=103,
                    DataOraInizioUtilizzo = DateTime.Today.AddDays(-1).AddHours(11),
                    DataOraFineUtilizzo = DateTime.Today.AddDays(-1).AddHours(12) },
                new (){ComputerId = 20,StudenteId=103,
                    DataOraInizioUtilizzo = DateTime.Today.AddHours(11),
                    DataOraFineUtilizzo = DateTime.Today.AddHours(12) },
                new (){ComputerId = 64,StudenteId=131,
                    DataOraInizioUtilizzo = DateTime.Now.Add(- new TimeSpan(0,12,0)),
                    DataOraFineUtilizzo = null},
                new (){ComputerId = 65,StudenteId=132,
                    DataOraInizioUtilizzo = DateTime.Now.Add(- new TimeSpan(1,12,0)),
                    DataOraFineUtilizzo = null},
            };
            Console.WriteLine("Inseriamo le classi nel database");
            classi.ForEach(c => db.Add(c));
            db.SaveChanges();
            Console.WriteLine("Inseriamo gli studenti nel database");
            studenti.ForEach(s => db.Add(s));
            db.SaveChanges();
            Console.WriteLine("Inseriamo i computers nel database");
            computers.ForEach(c => db.Add(c));
            db.SaveChanges();
            Console.WriteLine("Inseriamo gli utilizzi nel database");
            utilizzi.ForEach(u => db.Add(u));
            db.SaveChanges();
        }
    }
    static void Main(string[] args)
    {
        using var db = new UtilizziPCContext();
        Console.WriteLine("Q1: Contare gli alunni di una classe");
        Q1("4IA", db);
        Console.WriteLine("Q2: Riportare il numero di alunni per ogni classe");
        Q2(db);
        Console.WriteLine("Q3: Stampa gli studenti che non hanno ancora restituito i computer (sono quelli collegati a Utilizza con DataOraFineUtilizzo pari a null)");
        Q3(db);
        Console.WriteLine("Q4: Stampa l’elenco dei computer che sono stati utilizzati dagli studenti della classe specificata in input. ");
        Q4("4IA", db);
        Console.WriteLine("Q5: Dato un computer (di cui si conosce l’Id) riporta l’elenco degli studenti che lo hanno usato negli ultimi 30 giorni, con l'indicazione della DataOraInizioUtilizzo,ordinando i risultati per classe e, a parità di classe, per data (mostrando prima le date più recenti)");
        Q5(1, db);
        Console.WriteLine("Q6: Stampa per ogni classe quanti utilizzi di computer sono stati fatti negli ultimi 30 giorni.");
        Q6(db);
        Console.WriteLine("Q7: Stampa le classi che hanno utilizzato maggiormente i computer (quelle con il maggior numero di utilizzi) negli ultimi 30 giorni");
        Q7(db);
    }
    //Q1: Contare gli alunni di una classe
    private static void Q1(string classe, UtilizziPCContext db)
    {
        var studenti = db.Studenti.Where(s => s.Classe.Nome == classe).Count();
        Console.WriteLine($"Nella {classe} ci sono {studenti} studenti");
    }

    //Q2: Riportare il numero di alunni per ogni classe
    private static void Q2(UtilizziPCContext db)
    {
        db.Studenti
            .GroupBy(s => s.Classe)
            .Select(g => new { g.Key, NumeroStudenti = g.Count() })
            .ToList()
            .ForEach(g => Console.WriteLine($"Nella classe {g.Key.Nome} ci sono {g.NumeroStudenti}"));        
    }

    //Q3: Stampa gli studenti che non hanno ancora restituito i computer (sono quelli collegati a Utilizza con DataOraFineUtilizzo pari a null)
    private static void Q3(UtilizziPCContext db)
    {
        db.Utilizzi
            .Where(u => u.DataOraFineUtilizzo == null)
            .Join(db.Studenti, u => u.StudenteId, s => s.Id, (u, s) => new { u, s })
            .ToList().ForEach(f => Console.WriteLine($"Lo studente {f.s} non ha ancora restituito il computer"));
    }

    //Q4: Stampa l’elenco dei computer che sono stati utilizzati dagli studenti della classe specificata in input. 
    private static void Q4(string classe, UtilizziPCContext db)
    {
        db.Studenti
            .Where(s => s.Classe.Nome == classe)
            .Join(db.Utilizzi, s => s.Id, u => u.StudenteId, (s, u) => new { s, u })
            .Join(db.Computers, j1 => j1.u.ComputerId, c => c.Id, (j1,c) => new {j1,c})
            .Distinct()
            .ToList().ForEach(f => Console.WriteLine($"Nella classe {classe} lo studente {f.j1.s.Cognome} {f.j1.s.Nome} ha utilizzato {f.c.Modello} da {f.j1.u.DataOraInizioUtilizzo} a {f.j1.u.DataOraFineUtilizzo}"));
            //.Where(w => w.u.DataOraFineUtilizzo != null)
    }

    //Q5: Dato un computer (di cui si conosce l’Id) riporta l’elenco degli studenti che lo hanno usato negli ultimi 30 giorni,
    //con l'indicazione della DataOraInizioUtilizzo,ordinando i risultati per classe e, a parità di classe, per data
    //(mostrando prima le date più recenti)
    private static void Q5(int computerId, UtilizziPCContext db)
    {
        db.Utilizzi
            .Where(u => u.ComputerId == computerId && u.DataOraInizioUtilizzo >= DateTime.Now.AddDays(-30))
            .Join(db.Studenti.Include(s => s.Classe),
                u => u.StudenteId,
                s => s.Id,
                (u, s) => new { s, u.DataOraInizioUtilizzo })
            .OrderBy(a => a.s.Classe.Nome)
            .ThenByDescending(a => a.DataOraInizioUtilizzo)
            .ToList()
            .ForEach(a => Console.WriteLine($"Studente = {a.s}, Data utilizzo computer = {a.DataOraInizioUtilizzo}"));
    }
    //Q6: Stampa per ogni classe quanti utilizzi di computer sono stati fatti negli ultimi 30 giorni.
    private static void Q6(UtilizziPCContext db)
    {
        db.Studenti
            .Join(db.Utilizzi.Where(u => u.DataOraFineUtilizzo >= DateTime.Now.AddDays(-30)),
            s => s.Id, u => u.StudenteId, (s,u) => new {s,u})
            .GroupBy(g => g.s.Classe.Nome)
            .Select(select => new {NomeClasse = select.Key, NumeroUtlizzi = select.Count()})
            .ToList().ForEach(f => Console.WriteLine($"Nella {f.NomeClasse} sono stati usati {f.NumeroUtlizzi} computers"));
    }

    //Q7: Stampa le classi che hanno utilizzato maggiormente i computer (quelle con il maggior numero di utilizzi) negli ultimi 30 giorni
    private static void Q7(UtilizziPCContext db)
    {
        var numeroUtilizziPerClasse = db.Studenti
            .Join(db.Utilizzi.Where(u => u.DataOraInizioUtilizzo >= DateTime.Now.AddDays(-30)),
                s => s.Id,
                u => u.StudenteId,
                (s, u) => new { NomeClasse = s.Classe.Nome, u })
            .GroupBy(a => a.NomeClasse)
            .Select(g => new { NomeClasse = g.Key, NumeroUtilizzi = g.Count() });
        var classiConNumeroUtilizziMassimo = numeroUtilizziPerClasse
            .Where(u => u.NumeroUtilizzi == numeroUtilizziPerClasse.Max(u => u.NumeroUtilizzi));
        classiConNumeroUtilizziMassimo
            .ToList()
            .ForEach(Console.WriteLine);
    }}