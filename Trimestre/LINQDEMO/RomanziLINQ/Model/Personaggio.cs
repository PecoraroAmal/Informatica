namespace Romanzi.Model;
public class Personaggio
{
    public int PersonaggioId { get; set; }
    public string Nome { get; set; } = null!;
    public int RomanzoId { get; set; }
    public string? Sesso { get; set; }
    public string? Ruolo { get; set; }
    public override string ToString()
    {
        return $"PersonaggiId: {PersonaggioId} Nome: {Nome} RomanzoId: {RomanzoId} Sesso: {Sesso} Ruolo: {Ruolo}";
    }
}