using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsercitazionePerLaVerificaDiOttobre.Model
{
    public class Sport
    {
        public string Nome { get; set; }
        public string Luogo { get; set; }

        public override string ToString()
        {
            return $"Nome: {Nome} Luogo: {Luogo}";
        }
    }
}
