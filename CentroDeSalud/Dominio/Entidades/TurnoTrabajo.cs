using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dominio.Entidades
{
    public class TurnoTrabajo
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public bool Activo { get; set; } = true;

        public List<Medico> Medicos { get; set; } = new List<Medico>();
    }
}
