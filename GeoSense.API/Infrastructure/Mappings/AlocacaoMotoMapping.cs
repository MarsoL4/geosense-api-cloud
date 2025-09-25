using GeoSense.API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GeoSense.API.Infrastructure.Mappings
{
    public class AlocacaoMotoMapping : IEntityTypeConfiguration<AlocacaoMoto>
    {
        public void Configure(EntityTypeBuilder<AlocacaoMoto> builder)
        {
            builder.ToTable("ALOCACAOMOTO");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).HasColumnName("ID").HasColumnType("bigint").ValueGeneratedOnAdd();
            builder.Property(a => a.DataHoraAlocacao).HasColumnName("DATA_HORA_ALOCACAO").HasColumnType("datetime2");
            builder.Property(a => a.MotoId).HasColumnName("MOTO_ID").HasColumnType("bigint");
            builder.Property(a => a.VagaId).HasColumnName("VAGA_ID").HasColumnType("bigint");
            builder.Property(a => a.MecanicoResponsavelId).HasColumnName("MECANICO_RESPONSAVEL_ID").HasColumnType("bigint");

            builder.HasOne(a => a.Moto)
                   .WithMany(m => m.Alocacoes)
                   .HasForeignKey(a => a.MotoId);

            builder.HasOne(a => a.Vaga)
                   .WithMany(v => v.Alocacoes)
                   .HasForeignKey(a => a.VagaId);
        }
    }
}