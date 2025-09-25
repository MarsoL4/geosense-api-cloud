namespace GeoSense.API.Infrastructure.Persistence
{
    public class Patio
    {
        public long Id { get; private set; }
        public string Nome { get; set; } = string.Empty;

        public ICollection<Vaga> Vagas { get; set; } = [];
    }
}