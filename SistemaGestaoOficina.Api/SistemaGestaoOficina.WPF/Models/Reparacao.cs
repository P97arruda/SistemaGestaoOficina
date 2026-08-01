using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestaoOficina.WPF.Models
{
    internal class Reparacao
    {
        public int Id { get; set; }

        public int IdMarcacao { get; set; }

        public string Pecas { get; set; }

        public decimal CustoTotal { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        public bool Concluida { get; set; }

        public string TipoServico { get; set; }
    }
}
