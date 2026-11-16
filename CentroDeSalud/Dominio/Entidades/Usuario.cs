namespace Dominio.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Pass { get; set; }
        public Perfil Perfil { get; set; }
        public Medico Medico { get; set; }
        public bool Activo { get; set; } = true;
    }
}
