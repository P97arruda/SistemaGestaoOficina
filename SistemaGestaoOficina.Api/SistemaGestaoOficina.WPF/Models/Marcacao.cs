using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestaoOficina.WPF.Models
{
    public class Marcacao
    {
        public int Id { get; set; }

        public int IdCliente { get; set; }

        public int IdVeiculo { get; set; }

        public int IdMecanico { get; set; }

        public string TipoServico { get; set; }

        public DateTime DataHora { get; set; }

        public string Estado { get; set; }

        public string Matricula { get; set; }
    }
}
