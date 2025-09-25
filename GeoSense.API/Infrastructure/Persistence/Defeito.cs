namespace GeoSense.API.Infrastructure.Persistence
{
    public class Defeito
    {
        public long Id { get; set; }
        public required string Descricao { get; set; }
        public required string TiposDefeitos { get; set; }
        public long MotoId { get; set; }
        public Moto? Moto { get; set; }
    }
}