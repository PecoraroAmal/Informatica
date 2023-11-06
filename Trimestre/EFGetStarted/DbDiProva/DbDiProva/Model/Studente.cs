using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbDiProva.Model
{
    public class Studente
    {
        [Key]
        public string Nome { get; set; }
        public string Cognome { get; set;}
        public string Scuola { get; set; }
        public string Sport { get; set; }
        public string Videogioco { get; set; }
        public override string ToString()
        {
            return $"Nome: {Nome} Cognome: {Cognome} Scuola: {Scuola} Sport: {Scuola} Videogioco {Videogioco}";
        }
    }
}
