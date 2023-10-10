namespace Romanzi.Model;
public class Romanzo
{
    public int RomanzoId { get; set; }
    public string Titolo { get; set; } = null!;
    public int AutoreId { get; set; }
    public int? AnnoPubblicazione { get; set; }
    public override string ToString()
    {
        return $"RoamnzoId: {RomanzoId} Titolo: {Titolo} AutoreId: {AutoreId} AnnoPubblicazione: {AnnoPubblicazione}";
    }

}
