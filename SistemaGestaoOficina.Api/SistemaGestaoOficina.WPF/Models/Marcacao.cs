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

        public string NomeCliente { get; set; }

        public override string ToString()
        {
            return NomeCliente + " | " +
                   Matricula + " | " +
                   TipoServico + " | " +
                   DataHora.ToString("dd/MM/yyyy HH:mm");
        }
    }
}
