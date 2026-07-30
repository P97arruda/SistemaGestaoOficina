using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestaoOficina.WPF.Models
{
    public class Mecanico
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Especialidade { get; set; }

        public string Horario { get; set; }

        public string Contacto { get; set; }

        public bool Ativo { get; set; }

        public override string ToString()
        {
            string estado = Ativo ? "Ativo" : "Desativado";

            return Id + " - " + Nome + " - " + Especialidade + " - " + estado;
        }
    }
}
