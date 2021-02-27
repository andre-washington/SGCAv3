using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGCAv1.Models
{
    public class Nota
    {
        public int NotaID { get; set; }
        public int NotaQuantidade { get; set; }
        public int NotaValor { get; set; }
        public int CaixaID { get; set; }
    }
}
