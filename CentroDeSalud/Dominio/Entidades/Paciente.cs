using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades
{
    public class Paciente : Persona
    {
        public string ObraSocial { get; set; }
        public string NumeroAfiliado { get; set; }
        public List<Turno> Turnos { get; set; } = new List<Turno>();
    }
}
