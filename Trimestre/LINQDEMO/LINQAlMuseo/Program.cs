using LINQAlMuseo;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
//creazione delle collection
//si parte da quelle che non puntano a nulla, ossia quelle che non hanno chiavi esterne
IList<Artista> artisti = new List<Artista>()
            {
                new (){Id=1, Cognome="Picasso", Nome="Pablo", Nazionalita="Spagna"},
                new (){Id=2, Cognome="Dalì", Nome="Salvador", Nazionalita="Spagna"},
                new (){Id=3, Cognome="De Chirico", Nome="Giorgio", Nazionalita="Italia"},
                new (){Id=4, Cognome="Guttuso", Nome="Renato", Nazionalita="Italia"}
            };
//poi le collection che hanno Fk
IList<Opera> opere = new List<Opera>() {
                new (){Id=1, Titolo="Guernica", Quotazione=50000000.00m , FkArtista=1},//opera di Picasso
                new (){Id=2, Titolo="I tre musici", Quotazione=15000000.00m, FkArtista=1},//opera di Picasso
                new (){Id=3, Titolo="Les demoiselles d’Avignon", Quotazione=12000000.00m,  FkArtista=1},//opera di Picasso
                new (){Id=4, Titolo="La persistenza della memoria", Quotazione=16000000.00m,  FkArtista=2},//opera di Dalì
                new (){Id=5, Titolo="Metamorfosi di Narciso", Quotazione=8000000.00m, FkArtista=2},//opera di Dalì
                new (){Id=6, Titolo="Le Muse inquietanti", Quotazione=22000000.00m,  FkArtista=3},//opera di De Chirico
            };
IList<Personaggio> personaggi = new List<Personaggio>() {
                new (){Id=1, Nome="Uomo morente", FkOperaId=1},//un personaggio di Guernica 
                new (){Id=2, Nome="Un musicante", FkOperaId=2},
                new (){Id=3, Nome="una ragazza di Avignone", FkOperaId=3},
                new (){Id=4, Nome="una seconda ragazza di Avignone", FkOperaId=3},
                new (){Id=5, Nome="Narciso", FkOperaId=5},
                new (){Id=6, Nome="Una musa metafisica", FkOperaId=6},
            };
//impostiamo la console in modo che stampi correttamente il carattere dell'euro e che utilizzi le impostazioni di cultura italiana
Console.OutputEncoding = Encoding.UTF8;
Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT");

//Le query da sviluppare sono:
//Effettuare le seguenti query:
//1)    Stampare le opere di un dato autore (ad esempio Picasso)
//2)    Riportare per ogni nazionalità (raggruppare per nazionalità) gli artisti
//3)    Contare quanti sono gli artisti per ogni nazionalità (raggruppare per nazionalità e contare)
//4)    Trovare la quotazione media, minima e massima delle opere di Picasso
//5)    Trovare la quotazione media, minima e massima di ogni artista
//6)    Raggruppare le opere in base alla nazionalità e in base al cognome dell’artista (Raggruppamento in base a più proprietà)
//7)    Trovare gli artisti di cui sono presenti almeno 2 opere
//8)    Trovare le opere che hanno personaggi
//9)    Trovare le opere che non hanno personaggi
//10)   Trovare l’opera con il maggior numero di personaggi
//svolgimento delle query richieste
//1) Stampare le opere di un dato autore (ad esempio Picasso)
Console.WriteLine("1) Stampare le opere di un dato autore (ad esempio Picasso)\n");
//facciamo prima il filtraggio con la Where e poi la join
var opereDiArtista = artisti.Where(a => a.Cognome == "Picasso").Join(opere,a => a.Id,o => o.FkArtista,(a, o) => o.Titolo);
opereDiArtista.ToList().ForEach(t => Console.WriteLine(t));
Console.WriteLine("2) Riportare per ogni nazionalità (raggruppare per nazionalità) gli artisti");
var nazionalita = artisti.GroupBy(a => a.Nazionalita);
foreach(var group in nazionalita)
{
    Console.WriteLine($"Nazionalità: {group.Key}");
    foreach(var artista in group)
        Console.WriteLine($"\t {artista.Cognome} {artista.Nome}");
}
Console.WriteLine("3) Contare quanti sono gli artisti per ogni nazionalità (raggruppare per nazionalità e contare)");
foreach(var group in nazionalita)
    Console.WriteLine($"Nazionalità: {group.Key} con: {group.Count()} artisti");
Console.WriteLine("4) Trovare la quotazione media, minima e massima delle opere di Picasso");
var opereDiPicasso = artisti.Where(a => a.Cognome == "Picasso").Join(opere, a => a.Id, o=> o.FkArtista, (a,o)=>o).ToList();
var quataMediaPicasso = opereDiPicasso.Average(a => a.Quotazione);
var quotaMinimaPicasso = opereDiPicasso.Min(a => a.Quotazione);
var quotaMassimaPicasso = opereDiPicasso.Max(a => a.Quotazione);
Console.WriteLine($"Media: {quataMediaPicasso}\nMinima: {quotaMinimaPicasso}\nMassima: {quotaMassimaPicasso}");
Console.WriteLine("5) Trovare la quotazione media, minima e massima di ogni artista");
var opereGeneriche = opere.GroupBy(o => o.FkArtista);
foreach(var group in opereGeneriche)
{
    Console.WriteLine($"Id artista: {group.Key}");
    Console.WriteLine($"Minimo: {group.Min(q => q.Quotazione)}\nMassimo: {group.Max(q => q.Quotazione)}\n Media: {opere.Average(q => q.Quotazione)}\n");
}
Console.WriteLine("6) Raggruppare le opere in base alla nazionalità e in base al cognome dell’artista (Raggruppamento in base a più proprietà)");
var nazionalitaCognome = artisti.Join(opere, a => a.Id, o => o.FkArtista, (a,o) => new {a,o}).GroupBy(t => new { t.a.Nazionalita, t.a.Cognome});
foreach(var group in nazionalitaCognome)
{
    Console.WriteLine($"{group.Key.Nazionalita}: {group.Key.Cognome}");
    foreach(var opera in group)
        Console.WriteLine($"\t Opera: {opera.o.Titolo}");
}
Console.WriteLine("7) Trovare gli artisti di cui sono presenti almeno 2 opere");
var opereArtisti = opere.GroupBy(o => o.FkArtista).Where(n => n.Count() >= 2).Join(artisti, o => o.Key, a => a.Id, (o,a) => a);
Console.WriteLine("Artisti con almeno 2 opere:");
foreach(var artista in opereArtisti)
    Console.WriteLine(artista);
Console.WriteLine("Potrei non creare la variabile e scrivere:");
opere.GroupBy(o => o.FkArtista).Where(n => n.Count() >= 2).Join(artisti, o => o.Key, a => a.Id, (o,a) => a).ToList().ForEach(Console.WriteLine);
Console.WriteLine("8) Trovare le opere che hanno personaggi");
var opereConPersonaggi = opere.Join(personaggi, o => o.Id, p => p.FkOperaId, (o,p) => o);
foreach(var opera in opereConPersonaggi)
    Console.WriteLine(opera);
Console.WriteLine("9) Trovare le opere che non hanno personaggi");
var opereSenzaPersonaggi = opere.Where(o => !opereConPersonaggi.Contains(o));
foreach (var opera in opereSenzaPersonaggi)
    Console.WriteLine(opera);
Console.WriteLine("10) Trovare l’opera con il maggior numero di personaggi");
var personaggiPerOpera = personaggi.GroupBy(p => p.FkOperaId).Select(group => new { IdOpera = group.Key, NumeroPersonaggi = group.Count() });
var numeroMassimoPersonaggi = personaggiPerOpera.Max(t => t.NumeroPersonaggi);
var opereConMaxNumeroPersonaggi = personaggiPerOpera.Where(t => t.NumeroPersonaggi == numeroMassimoPersonaggi)
    .Join(opere,t => t.IdOpera,o => o.Id,(t, o) => new { id = o.Id, o.Titolo, Personaggi = t.NumeroPersonaggi });
foreach (var item in opereConMaxNumeroPersonaggi)
    Console.WriteLine(item);