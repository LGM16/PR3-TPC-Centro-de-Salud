using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades
{
    public class Paciente : Persona
    {
        public DateTime FechaNacimiento { get; set; }
        public string Direccion { get; set; }
    }
}
