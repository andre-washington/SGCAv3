using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGCAv1.Models
{
    public class Caixa
    {
        public int CaixaID { get; set; }
        public int CaixaQtdCritica { get; set; }
        public String CaixaSituacao { get; set; }
        //public ICollection<Nota> Nota{ get; set; }
    }
}
