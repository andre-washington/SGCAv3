using System;
using System.Collections.Generic;

#nullable disable

namespace SGCAv1
{
    public partial class Sgca
    {
        public int? CaixaId { get; set; }

        public virtual Caixa Caixa { get; set; }
    }
}
