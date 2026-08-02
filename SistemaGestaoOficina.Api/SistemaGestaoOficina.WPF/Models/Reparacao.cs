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

        public string NomeCliente { get; set; }

        public string Matricula { get; set; }

        public string ModeloVeiculo { get; set; }

        public override string ToString()
        {
            return NomeCliente + " | " +
                   ModeloVeiculo + " | " +
                   Matricula + " | " +
                   TipoServico + " | " +
                   CustoTotal.ToString("0.00") + "€ | " +
                   DataInicio.ToString("dd/MM/yyyy") + " - " +
                   DataFim.Value.ToString("dd/MM/yyyy");
        }

    }
}
