using GeoSense.API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GeoSense.API.Infrastructure.Mappings
{
    /// <summary>
    /// Configuração de mapeamento da entidade Vaga para o banco de dados.
    /// Relaciona vaga com seu pátio e define restrição de unicidade composta.
    /// </summary>
    public class VagaMapping : IEntityTypeConfiguration<Vaga>
    {
        public void Configure(EntityTypeBuilder<Vaga> builder)
        {
            builder.ToTable("VAGA");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Id).HasColumnName("ID").HasColumnType("bigint").ValueGeneratedOnAdd();
            builder.Property(v => v.Numero).HasColumnName("NUMERO").HasColumnType("int");
            builder.Property(v => v.Tipo).HasColumnName("TIPO").HasColumnType("int");
            builder.Property(v => v.Status).HasColumnName("STATUS").HasColumnType("int");
            builder.Property(v => v.PatioId).HasColumnName("PATIO_ID").HasColumnType("bigint");

            builder.HasOne(v => v.Patio)
                   .WithMany(p => p.Vagas)
                   .HasForeignKey(v => v.PatioId);

            builder.HasIndex(v => new { v.Numero, v.PatioId }).IsUnique();
        }
    }
}