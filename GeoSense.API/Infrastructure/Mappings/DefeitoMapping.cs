using GeoSense.API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GeoSense.API.Infrastructure.Mappings
{
    public class DefeitoMapping : IEntityTypeConfiguration<Defeito>
    {
        public void Configure(EntityTypeBuilder<Defeito> builder)
        {
            builder.ToTable("DEFEITO");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id).HasColumnName("ID").HasColumnType("bigint").ValueGeneratedOnAdd();
            builder.Property(d => d.TiposDefeitos).HasColumnName("TIPOS_DEFEITOS").HasColumnType("nvarchar(2000)");
            builder.Property(d => d.Descricao).HasColumnName("DESCRICAO").HasColumnType("nvarchar(2000)");
            builder.Property(d => d.MotoId).HasColumnName("MOTO_ID").HasColumnType("bigint");

            builder.HasOne(d => d.Moto)
                   .WithMany(m => m.Defeitos)
                   .HasForeignKey(d => d.MotoId);
        }
    }
}   