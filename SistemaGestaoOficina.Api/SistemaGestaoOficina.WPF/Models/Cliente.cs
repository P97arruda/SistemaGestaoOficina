using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestaoOficina.WPF.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Apelido { get; set; }

        public  string Contacto { get; set; }

        public string NIF { get; set; }

        public string Email { get; set; }

        public string NomeCompleto
        {
            get
            {
                return Nome + " " + Apelido;
            }
        }

    }
}
