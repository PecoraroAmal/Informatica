using GestioneFattureClienti.Data;
using System.Text;
using GestioneFattureClienti.Model;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;

Console.OutputEncoding = Encoding.UTF8;
Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("it-IT");
//creazione della tabella
FattureClientiContext db = new();
//PopulateDb(db);
LetturaDb();
static void PopulateDb(FattureClientiContext db)
{
    //Creazione dei Clienti - gli id vengono generati automaticamente come campi auto-incremento quando si effettua l'inserimento, tuttavia
    //è bene inserire esplicitamente l'id degli oggetti quando si procede all'inserimento massivo gli elementi mediante un foreach perché
    //EF core potrebbe inserire nel database gli oggetti in un ordine diverso rispetto a quello del foreach
    // https://stackoverflow.com/a/54692592
    // https://stackoverflow.com/questions/11521057/insertion-order-of-multiple-records-in-entity-framework/
    List<Cliente> listaClienti = new()
        {
            new (){ClienteId=1, RagioneSociale= "Cliente 1", PartitaIVA= "1111111111", Citta = "Napoli", Via="Via dei Mille", Civico= "23", CAP="80100"},
            new (){ClienteId=2, RagioneSociale= "Cliente 2", PartitaIVA= "1111111112", Citta = "Roma", Via="Via dei Fori Imperiali", Civico= "1", CAP="00100"},
            new (){ClienteId=3, RagioneSociale= "Cliente 3", PartitaIVA= "1111111113", Citta = "Firenze", Via="Via Raffaello", Civico= "10", CAP="50100"}
        };

    //Creazione delle Fatture
    List<Fattura> listaFatture = new()
        {
            new (){FatturaId=1, Data= DateTime.Now.Date, Importo = 1200.45m, ClienteId = 1},//controllo integrità del db
            new (){FatturaId=2, Data= DateTime.Now.AddDays(-5).Date, Importo = 3200.65m, ClienteId = 1},
            new (){FatturaId=3, Data= new DateTime(2019,10,20).Date, Importo = 5200.45m, ClienteId = 1},
            new (){FatturaId=4, Data= DateTime.Now.Date, Importo = 5200.45m, ClienteId = 2},
            new (){FatturaId=5, Data= new DateTime(2019,08,20).Date, Importo = 7200.45m, ClienteId = 2}
        };
    Console.WriteLine("Inseriamo i clienti nel database");
    listaClienti.ForEach(c => db.Add(c));
    db.SaveChanges();
    Console.WriteLine("Inseriamo le fatture nel database");
    listaFatture.ForEach(f => db.Add(f));
    db.SaveChanges();
}
static void LetturaDb()
{
    //recuperiamo i dati dal database
    FattureClientiContext db = new();
    //Nel caso in cui il database fosse stato eliminato ricreiamo un nuovo database vuoto a partire dal Model
    db.Database.EnsureCreated();
    //Il codice seguente è scritto in modo che se anche non ci fossero dati nelle tabelle non verrebbero sollevate eccezioni
    Console.WriteLine("Recuperiamo i dati dal database - senza alcuna elaborazione");
    List<Cliente> listaClienti = db.Clienti.ToList();
    List<Fattura> listaFatture = db.Fatture.ToList();
    Console.WriteLine("Stampa dei clienti");
    listaClienti.ForEach(c => Console.WriteLine(c));
    Console.WriteLine("Stampa delle fatture");
    listaFatture.ForEach(f => Console.WriteLine(f));

    Console.WriteLine("Recuperiamo i dati dal database - uso di WHERE");
    Console.WriteLine("Recuperiamo i dati dal database - trovare le fatture fatte da almeno tre giorni");
    db.Fatture.Where(f => f.Data < DateTime.Now.AddDays(-2)).ToList().ForEach(f => Console.WriteLine(f));
    Console.WriteLine("Recuperiamo i dati dal database - trovare l'importo complessivo delle fatture fatte da almeno tre giorni ");
    Console.WriteLine($"Importo complessivo: {db.Fatture.Where(f => f.Data < DateTime.Now.AddDays(-2)).Sum(f => (double)f.Importo):C2}");
    Console.WriteLine("Recuperiamo i dati dal database - trovare l'importo medio delle fatture fatte da almeno tre giorni ");
    static bool searchPredicate(Fattura f) => f.Data < DateTime.Now.AddDays(-2);
    var count = db.Fatture.Where(searchPredicate).Count();
    //se non ci sono elementi nella collection i metodi Average, Max e Min sollevano l'eccezione InvalidOperationException
    if (count > 0)
    {
        Console.WriteLine($"Importo medio: {db.Fatture.Where(searchPredicate).Average(f => (double)f.Importo):C2}");
        Console.WriteLine("Recuperiamo i dati dal database - trovare l'importo massimo delle fatture fatte da almeno tre giorni ");
        Console.WriteLine($"Importo massimo: {db.Fatture.Where(searchPredicate).Max(f => (double)f.Importo):C2}");
        Console.WriteLine("Recuperiamo i dati dal database - trovare l'importo minimo delle fatture fatte da almeno tre giorni ");
        Console.WriteLine($"Importo minimo: {db.Fatture.Where(searchPredicate).Min(f => (double)f.Importo):C2}");
        Console.WriteLine("Recuperiamo i dati dal database - trovare il numero delle fatture fatte da almeno tre giorni ");
        Console.WriteLine("Numero fatture: " + count);
    }
    Console.WriteLine("Recuperiamo i dati dal database - uso di WHERE e JOIN");
    Console.WriteLine("trovare il nome e l'indirizzo dei clienti che hanno speso più di 5000 EUR");
    var clientiConSpesa5000Plus = db.Fatture.Where(f => f.Importo > 5000).Join(db.Clienti,
        f => f.ClienteId,
        c => c.ClienteId,
        (f, c) => new { NumeroFattura = f.FatturaId, DataFattura = 
        f.Data, NomeCliente = c.RagioneSociale, Indirizzo = c.Via + " N." + c.Civico + " " + c.CAP + " " + c.Citta });
    clientiConSpesa5000Plus.ToList().ForEach(c => Console.WriteLine(c));

    //altro modo - uso delle navigation properties
    Console.WriteLine("Recuperiamo i dati dal database - Uso di Navigation Property per ottenere i dati dei clienti a partire dalle fatture");
    var clientiConSpesa5000Plus2 = db.Fatture.Where(f => f.Importo > 5000).
        Select(f => new
        {
            NumeroFattura = f.FatturaId,
            DataFattura = f.Data,
            NomeCliente = f.Cliente.RagioneSociale,
            Indirizzo = f.Cliente.Via + " N." + f.Cliente.Civico + " " + f.Cliente.CAP + " " + f.Cliente.Citta
        });
    clientiConSpesa5000Plus2.ToList().ForEach(c => Console.WriteLine(c));
}