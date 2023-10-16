using EsercitazionePerLaVerificaDiOttobre.Model;

namespace EsercitazionePerLaVerificaDiOttobre
{
    internal class Program
    {
        public static List<Persona> persone = new List<Persona>
        {
            new Persona { Nome = "Alice", Cognome = "Rossi", Scuola = "Scuola 1", Sport = "Calcio", Videogioco = "Fortnite" },
            new Persona { Nome = "Bob", Cognome = "Verdi", Scuola = "Scuola 2", Sport = "Basket", Videogioco = "Fortnite" },
            new Persona { Nome = "Charlie", Cognome = "Bianchi", Scuola = "Scuola 2", Sport = "Calcio", Videogioco = "Genshin Impact" },
            new Persona { Nome = "David", Cognome = "Neri", Scuola = "Scuola 3", Sport = "Tennis", Videogioco = "Assassin's Creed" }
        };

        public static List<Scuola> scuole = new List<Scuola>
        {
            new Scuola { Nome = "Scuola 1", Indirizzo = "Via Roma 123" },
            new Scuola { Nome = "Scuola 2", Indirizzo = "Via Milano 456" },
            new Scuola { Nome = "Scuola 3", Indirizzo = "Via Napoli 789" }
        };

        public static List<Sport> sport = new List<Sport>
        {
            new Sport { Nome = "Calcio", Luogo = "Campo Sportivo Comunale" },
            new Sport { Nome = "Basket", Luogo = "Palestra della Scuola" },
            new Sport { Nome = "Tennis", Luogo = "Club Tennis Locale" }
        };

        public static List<Videogiochi> videogiochi = new List<Videogiochi>
        {
            new Videogiochi { Nome = "Fortnite", Tipologia = "Multiplayer" },
            new Videogiochi { Nome = "Genshin Impact", Tipologia = "Multiplayer" },
            new Videogiochi { Nome = "Assassin's Creed", Tipologia = "Single Player" }
        };

        static void Main(string[] args)
        {
            string Scuola;
            string Sport;
            string Videogioco;
            Console.WriteLine("Q1");
            Q1();
            Console.WriteLine("Q2");
            Q2();
            Console.WriteLine("Q3");
            Q3();
            Console.WriteLine("Q4");
            Q4("Scuola 2");
        }
        //Q1) stampare tutti gli allunni di ciscuna scuola
        public static void Q1()
        {
            var alunni = scuole.Join(persone, s => s.Nome, p => p.Scuola, (s, p) => new { s, p }).ToList();
            foreach(var scuola in alunni)
            {
                Console.WriteLine($"Nella {scuola.s.Nome} in via {scuola.s.Indirizzo} è presente {scuola.p.Cognome} " +
                    $"{scuola.p.Nome} che pratica {scuola.p.Sport} e gioca a {scuola.p.Videogioco}");
            }
        }
        //Q2) stampare tutti gli sport di ciascuna persona
        public static void Q2()
        {
            var sportDiPersone = persone.Join(sport, p => p.Sport, s => s.Nome, (p, s) => new { p, s }).ToList();
            foreach(var persona in sportDiPersone)
            {
                Console.WriteLine($"{persona.p.Cognome} {persona.p.Nome} pratica {persona.s.Nome} presso: {persona.s.Luogo}");
            }
        }
        //Q3) stampare tutte le persone che giocano ad un Single Player partendo dalla scuola
        public static void Q3()
        {
            var videogioco = scuole.Join(persone, s => s.Nome, p => p.Scuola, (s, p) => new { s, p })
                .Join(videogiochi, j1 => j1.p.Videogioco, v => v.Nome, (j1, v) => new { j1, v })
                .Where(t => t.v.Tipologia == "Single Player")
                .ToList();
            foreach(var persona in videogioco)
            {
                Console.WriteLine($"Nella {persona.j1.s.Nome} c'è {persona.j1.p.Cognome} {persona.j1.p.Nome} che gioca a {persona.j1.p.Videogioco} che è di tipo {persona.v.Tipologia}");
            }
        }
        //Q4) ricevere in input "Scuola 2" e ricavare chi gicoa a un Multiplayer riportando il nome di esso
        public static void Q4(string scuola)
        {
            var tipologia = scuole.Where(n => n.Nome == scuola)
                .Join(persone, s => s.Nome, p => p.Scuola, (s, p) => new { s, p })
                .Join(videogiochi, j1 => j1.p.Videogioco, v => v.Nome, (j1, v) => new { j1, v })
                .Where(t => t.v.Tipologia == "Multiplayer").ToList();
            foreach (var persona in tipologia)
            {
                Console.WriteLine($"Nella {scuola} c'è {persona.j1.p.Cognome} {persona.j1.p.Nome} che gioca a {persona.j1.p.Videogioco} che è di tipo {persona.v.Tipologia}");
            }
        }
    }
}