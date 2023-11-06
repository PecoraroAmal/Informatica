using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbDiProva.Model
{
    public class Sport
    {
        [Key]
        public string Nome { get; set; }
        public string Città { get; set; }
        public override string ToString()
        {
            return $"Nome: {Nome} Città {Città}";
        }
    }
}
