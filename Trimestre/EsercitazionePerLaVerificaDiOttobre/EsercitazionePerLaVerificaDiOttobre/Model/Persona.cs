using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsercitazionePerLaVerificaDiOttobre.Model
{
    public class Persona
    {
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Scuola { get; set; }
        public string Sport { get; set; }
        public string Videogioco { get; set; }

        public override string ToString()
        {
            return $"Nome: {Nome} Cognome: {Cognome} Scuola: {Scuola} Sport: {Sport} Videogioco: {Videogioco}";
        }
    }
}
