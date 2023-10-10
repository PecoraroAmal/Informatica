//Almeno 5 autori di nazionalità compresa tra "Americana", "Belga", "Inglese".
using Romanzi.Model;
using System.Linq;

List<Autore> autori = new()
{
    new (){AutoreId=1, Nome="Ernest",Cognome="Hemingway", Nazionalità="Americana"},//AutoreId=1
    new (){AutoreId=2,Nome="Philip",Cognome="Roth", Nazionalità="Americana"},//AutoreId=2
    new (){AutoreId=3,Nome="Thomas",Cognome="Owen", Nazionalità="Belga"},//AutoreId=3
    new (){AutoreId=4,Nome="William",Cognome="Shakespeare", Nazionalità="Inglese"},//AutoreId=4
    new (){AutoreId=5,Nome="Charles",Cognome="Dickens", Nazionalità="Inglese"},//AutoreId=5
};
//Almeno 10 romanzi degli autori precedentemente inseriti
List<Romanzo> romanzi = new()
{
    new (){RomanzoId=1, Titolo="For Whom the Bell Tolls", AnnoPubblicazione=1940, AutoreId=1},//RomanzoId=1
    new (){RomanzoId=2,Titolo="The Old Man and the Sea", AnnoPubblicazione=1952, AutoreId=1},
    new (){RomanzoId=3,Titolo="A Farewell to Arms",AnnoPubblicazione=1929, AutoreId=1},
    new (){RomanzoId=4,Titolo="Letting Go", AnnoPubblicazione=1962, AutoreId=2},
    new (){RomanzoId=5,Titolo="When She Was Good", AnnoPubblicazione=1967, AutoreId=2},
    new (){RomanzoId=6,Titolo="Destination Inconnue", AnnoPubblicazione=1942, AutoreId=3},
    new (){RomanzoId=7,Titolo="Les Fruits de l'orage", AnnoPubblicazione=1984, AutoreId=3},
    new (){RomanzoId=8,Titolo="Giulio Cesare", AnnoPubblicazione=1599, AutoreId=4},
    new (){RomanzoId=9,Titolo="Otello", AnnoPubblicazione=1604, AutoreId=4},
    new (){RomanzoId=10,Titolo="David Copperfield", AnnoPubblicazione=1849, AutoreId=5},
};
//Almeno 5 personaggi presenti nei romanzi precedentemente inseriti
List<Personaggio> personaggi = new()
{
    new (){PersonaggioId=1, Nome="Desdemona", Ruolo="Protagonista", Sesso="Femmina", RomanzoId=9},//PersonaggioId=1
    new (){PersonaggioId=2,Nome="Jago", Ruolo="Protagonista", Sesso="Maschio", RomanzoId=9},
    new (){PersonaggioId=3,Nome="Robert", Ruolo="Protagonista", Sesso="Maschio", RomanzoId=1},
    new (){PersonaggioId=4,Nome="Cesare", Ruolo="Protagonista", Sesso="Maschio", RomanzoId=8},
    new (){PersonaggioId=5,Nome="David", Ruolo="Protagonista", Sesso="Maschio", RomanzoId=10}
};
Console.WriteLine("Q1");
Q1("Inglese");
Console.WriteLine("Q2");
Q2("Shakespeare", "William");
Console.WriteLine("Q3");
Q3("Inglese");
Console.WriteLine("Q4");
Q4();
Console.WriteLine("Q5");
Q5("Inglese");
//Q1: creare un metodo che prende in input la nazionalità e stampa gli autori che hanno la nazionalità specificata
void Q1(string nazionalita)
{
    var autoriPerNazionalita = autori.Where(n => n.Nazionalità == nazionalita).ToList();
    foreach(var item in autoriPerNazionalita)
        Console.WriteLine(item);
}
//Q2: creare un metodo che prende in input il nome e il cognome di un autore e stampa tutti i romanzi di quell’autore
void Q2(string cognome, string nome)
{
    var romanziDiAutore = autori.Where(c => c.Cognome == cognome && c.Nome == nome).Join(romanzi, a => a.AutoreId, r => r.AutoreId, (a, r) => new {r.Titolo}).ToList();
    foreach (var item in romanziDiAutore)
        Console.WriteLine(item);
}
//Q3: creare un metodo che prende in input la nazionalità e stampa quanti romanzi di quella nazionalità sono presenti nel database
void Q3(string nazionalita)
{
    var romanziDiNazionalita = autori.Where(n => n.Nazionalità == nazionalita).Join(romanzi, a => a.AutoreId, r => r.AutoreId, (a,r) => new {r.Titolo}).ToList();
    foreach (var item in romanziDiNazionalita)
        Console.WriteLine(item);
}
//Q4: creare un metodo che per ogni nazionalità stampa quanti romanzi di autori di quella nazionalità sono presenti nel database
void Q4()//non funziona
{
    var romanziPerNazionalita = autori.Join(romanzi, a => a.AutoreId, r => r.RomanzoId, (a, r) => new { a.Nazionalità, a.AutoreId, r.RomanzoId })
        .GroupBy(n => n.Nazionalità).ToList();
    foreach(var item in romanziPerNazionalita)
    {
        Console.WriteLine($"{item.Key} {item.Count()}");
    }
}
//Q5: creare un metodo che stampa il nome dei personaggi presenti in romanzi di autori di una data nazionalità
void Q5(string nazionalita)
{
    var personaggiDiRomanziDiNazionalita = autori.Where(n => n.Nazionalità == nazionalita)
        .Join(romanzi, a => a.AutoreId, r => r.AutoreId, (a, r) => r)
        .Join(personaggi, r => r.RomanzoId, p => p.RomanzoId, (r, p) => p.Nome).ToList();
    foreach (var item in personaggiDiRomanziDiNazionalita)
    {
        Console.WriteLine(item);
    }
}
//void Q4()
//{
//    var romanziPerNazionalita = autori.GroupBy(n => n.Nazionalità)
//                                      .Select(n => new { Nazionalita = n.Key, NumeroRomanzi = n.Join(romanzi, a => a.AutoreId, r => r.AutoreId, (a, r) => r).Count() });

//    foreach (var nazionalita in romanziPerNazionalita)
//    {
//        Console.WriteLine($"Nazionalità {nazionalita.Nazionalita} ci sono {nazionalita.NumeroRomanzi} romanzi");
//    }
//}
