using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsercitazionePerLaVerificaDiOttobre.Model
{
    public class Videogiochi
    {
        public string Nome { get; set; }
        public string Tipologia { get; set; }

        public override string ToString()
        {
            return $"Nome: {Nome} Tipologia: {Tipologia}";
        }
    }
}