using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsercitazionePerLaVerificaDiOttobre.Model
{
    public class Scuola
    {
        public string Nome { get; set; }
        public string Indirizzo { get; set; }

        public override string ToString()
        {
            return $"Nome: {Nome} Indirizzo: {Indirizzo}";
        }
    }
}
