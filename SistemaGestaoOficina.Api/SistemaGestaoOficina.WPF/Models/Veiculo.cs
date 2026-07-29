using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestaoOficina.WPF.Models
{
    public class Veiculo
    {
        public int Id { get; set; }

        public string Matricula { get; set; }

        public string Marca { get; set; }

        public string Modelo { get; set; }

        public int Ano {  get; set; }

        public int Quilometragem { get; set; }

        public string Combustivel {  get; set; }

        public int IdCliente { get; set; }
    }
}
