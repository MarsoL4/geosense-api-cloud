using GeoSense.API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GeoSense.API.Infrastructure.Mappings
{
    public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("USUARIO");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id).HasColumnName("ID").HasColumnType("bigint").ValueGeneratedOnAdd();
            builder.Property(u => u.Nome).HasColumnName("NOME").HasColumnType("nvarchar(100)").IsRequired();
            builder.Property(u => u.Email).HasColumnName("EMAIL").HasColumnType("nvarchar(100)").IsRequired();
            builder.Property(u => u.Senha).HasColumnName("SENHA").HasColumnType("nvarchar(255)").IsRequired();
            builder.Property(u => u.Tipo).HasColumnName("TIPO").HasColumnType("int").HasConversion<int>().IsRequired();

            builder.HasIndex(u => u.Email).IsUnique();
        }
    }
}