using GeoSense.API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GeoSense.API.Infrastructure.Mappings
{
    public class MotoMapping : IEntityTypeConfiguration<Moto>
    {
        public void Configure(EntityTypeBuilder<Moto> builder)
        {
            builder.ToTable("MOTO");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id).HasColumnName("ID").HasColumnType("bigint").ValueGeneratedOnAdd();
            builder.Property(m => m.Modelo).HasColumnName("MODELO").HasColumnType("nvarchar(50)").IsRequired();
            builder.Property(m => m.Placa).HasColumnName("PLACA").HasColumnType("nvarchar(10)").IsRequired();
            builder.Property(m => m.Chassi).HasColumnName("CHASSI").HasColumnType("nvarchar(50)").IsRequired();
            builder.Property(m => m.ProblemaIdentificado).HasColumnName("PROBLEMA_IDENTIFICADO").HasColumnType("nvarchar(255)"); // Permite nulo

            builder.Property(m => m.VagaId).HasColumnName("VAGA_ID").HasColumnType("bigint").IsRequired();

            builder.HasOne(m => m.Vaga)
                .WithMany(v => v.Motos)
                .HasForeignKey(m => m.VagaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Restrições de unicidade
            builder.HasIndex(m => m.Placa).IsUnique();
            builder.HasIndex(m => m.Chassi).IsUnique();
            builder.HasIndex(m => m.VagaId).IsUnique();
        }
    }
}