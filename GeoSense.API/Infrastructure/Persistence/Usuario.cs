using GeoSense.API.Domain.Enums;

namespace GeoSense.API.Infrastructure.Persistence
{
    public class Usuario
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public TipoUsuario Tipo { get; set; }

        public Usuario() { }

        public Usuario(long id, string nome, string email, string senha, TipoUsuario tipo)
        {
            Id = id;
            Nome = nome;
            Email = email;
            Senha = senha;
            Tipo = tipo;
        }
    }
}