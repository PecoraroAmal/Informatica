using DbDiProva.Data;
using DbDiProva.Model;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DbDiProva
{
    internal class Program
    {
        static void Main(string[] args)
        {
            InitTest();
            using var db = new IlMioDbContext();
            Console.WriteLine("Q1: Stampare tutti gli alunni del Villa Greppi");
            Q1(db);
            Console.WriteLine("Q2: Stampare tutte le scuole");
            Q2(db);
            Console.WriteLine("Q3: Stampare tutti i giochi");
            Q3(db);
            Console.WriteLine("Q4: Stampare tutti gli sport");
            Q4(db);
            Console.WriteLine("Q5: Stampare tutti gli alunni che giocano ad un videogico dato in input");
            Q5("Arknights",db);
            Console.WriteLine("Q6: Stampare tutti gli alunni che praticano lo sport dato in input");
            Q6("Scherma",db);
            Console.WriteLine("Q7: Stampare tutti gli alunni che giocano ad un Multiplayer");
            Q7(db);
        }
        static void InitTest()
        {
            IlMioDbContext db = new();
            //Controllo se il database esista già
            if (db.Database.GetService<IRelationalDatabaseCreator>().Exists())
            {
                bool dbErase = true;
                if (dbErase)
                {
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                    PopulateDb();
                }
            }
            else
            {
                db.Database.EnsureCreated();
                PopulateDb();
            }

            static void PopulateDb()
            {
                List<Studente> studenti = new()
                {
                    new(){Nome = "Shinichi", Cognome = "Kudo", Scuola = "Villa Greppi", Videogioco = "Arknights", Sport = "Calcio"},
                    new(){Nome = "Heiji", Cognome = "Attori", Scuola = "Leonardo Da Vinci", Videogioco = "Arknights", Sport = "Scherma"},
                    new(){Nome = "Ran", Cognome = "Mouri", Scuola = "Villa Greppi", Videogioco = "Solitario", Sport = "Karate"},
                    new(){Nome = "Kazuha", Cognome = "Toyama", Scuola = "Leonardo Da Vinci", Videogioco = "Solitario", Sport = "Scherma"},
                    new(){Nome = "Kogorou", Cognome = "Mouri", Scuola = "Agenzia Investigativa", Videogioco = "Tetris", Sport = "Judo"},
                };
                using (var db = new IlMioDbContext())
                {
                    studenti.ForEach(stud => db.Add(stud));
                    db.SaveChanges();
                }
                List<Sport> sport = new()
                {
                    new(){Nome = "Calcio", Città = "Tokyo"},
                    new(){Nome = "Scherma", Città = "Osaka"},
                    new(){Nome = "Karate", Città = "Tokyo"},
                    new(){Nome = "Judo", Città = "Tokyo"}
                };
                using (var db = new IlMioDbContext())
                {
                    sport.ForEach(spo => db.Add(spo));
                    db.SaveChanges();
                }
                List<Scuola> scuola = new()
                {
                    new(){Nome = "Villa Greppi", Città = "Tokyo"},
                    new(){Nome = "Leonardo Da Vinci", Città = "Osaka"},
                    new(){Nome = "Agenzia Investigativa", Città = "Tokyo"}
                };
                using (var db = new IlMioDbContext())
                {
                    scuola.ForEach(scuo => db.Add(scuo));
                    db.SaveChanges();
                }
                List<Videogioco> videogioco = new()
                {
                    new(){Nome = "Arknights", Tipologia = "Multiplayer"},
                    new(){Nome = "Solitario", Tipologia = "Singleplayer"},
                    new(){Nome = "Tetris", Tipologia = "Singleplayer"}
                };
                using (var db = new IlMioDbContext())
                {
                    videogioco.ForEach(gioco => db.Add(gioco));
                    db.SaveChanges();
                }
            }
        }
        // Q1: Stampare tutti gli alunni del Villa Greppi
        public static void Q1(IlMioDbContext db)
        {
            var alunniVillaGreppi = db.studenti
                .Where(s => s.Scuola == "Villa Greppi")
                .ToList();
            foreach (var alunno in alunniVillaGreppi)
            {
                Console.WriteLine($"{alunno.Cognome} {alunno.Nome}");
            }
        }

        // Q2: Stampare tutte le scuole
        public static void Q2(IlMioDbContext db)
        {
            var scuole = db.scuole.ToList();
            foreach (var scuola in scuole)
            {
                Console.WriteLine(scuola.Nome);
            }
        }

        // Q3: Stampare tutti i giochi
        public static void Q3(IlMioDbContext db)
        {
            var giochi = db.videogiochi.ToList();
            foreach (var gioco in giochi)
            {
                Console.WriteLine(gioco.Nome);
            }
        }

        // Q4: Stampare tutti gli sport
        public static void Q4(IlMioDbContext db)
        {
            var sport = db.sport.ToList();
            foreach (var s in sport)
            {
                Console.WriteLine(s.Nome);
            }
        }

        // Q5: Stampare tutti gli alunni che giocano ad un videogioco dato in input
        public static void Q5(string gioco, IlMioDbContext db)
        {
            var alunniGioco = db.studenti
                .Where(s => s.Videogioco == gioco)
                .ToList();
            foreach (var alunno in alunniGioco)
            {
                Console.WriteLine($"{alunno.Cognome} {alunno.Nome} gioca a {alunno.Videogioco}");
            }
        }

        // Q6: Stampare tutti gli alunni che praticano lo sport dato in input
        public static void Q6(string sport, IlMioDbContext db)
        {
            var alunniSport = db.studenti
                .Where(s => s.Sport == sport)
                .ToList();
            foreach (var alunno in alunniSport)
            {
                Console.WriteLine($"{alunno.Cognome} {alunno.Nome} pratica {alunno.Sport}");
            }

        }

        // Q7: Stampare tutti gli alunni che giocano ad un Multiplayer
        public static void Q7(IlMioDbContext db)
        {
            var alunniMultiplayer = db.studenti
                .Where(s => db.videogiochi.Any(v => v.Nome == s.Videogioco && v.Tipologia == "Multiplayer"))
                .ToList();
            foreach (var alunno in alunniMultiplayer)
            {
                Console.WriteLine($"{alunno.Cognome} {alunno.Nome} gioca a un Multiplayer");
            }
        }
    }
}