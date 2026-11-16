using System;
using Dominio.Enums;

namespace Dominio.Entidades
{
    public class Turno
    {
        public int Id { get; set; }
        public Paciente Paciente { get; set; }
        public Medico Medico { get; set; }
        public Especialidad Especialidad { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string Observaciones { get; set; }
        public string Diagnostico { get; set; }
        public EstadoTurno Estado { get; set; }
        public DateTime FechaAlta { get; set; }
    }
}
