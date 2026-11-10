using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dominio.Entidades
{
    public class Medico : Persona
    {
        public string Matricula { get; set; }

        public int TurnoTrabajoId { get; set; }
        public TurnoTrabajo TurnoTrabajo { get; set; }

        public List<Especialidad> Especialidades { get; set; } = new List<Especialidad>();
        public List<Turno> Turnos { get; set; } = new List<Turno>();
    }
}
