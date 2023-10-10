namespace Romanzi.Model;
public class Autore
{
    public int AutoreId { get; set; }
    public string Nome { get; set; } = null!;
    public string Cognome { get; set; } = null!;
    public string? Nazionalità { get; set; }
    public override string ToString()
    {
        return $"Id: {AutoreId} Cognome: {Cognome} Nome: {Nome} Nazionalità: {Nazionalità}";
    }
}
