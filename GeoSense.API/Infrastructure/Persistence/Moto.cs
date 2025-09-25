namespace GeoSense.API.Infrastructure.Persistence
{
    public class Moto
    {
        public long Id { get; set; }
        public required string Modelo { get; set; }
        public required string Placa { get; set; }
        public required string Chassi { get; set; }
        public string? ProblemaIdentificado { get; set; }
        public long VagaId { get; set; }
        public virtual Vaga? Vaga { get; set; }

        public Moto() { }

        public Moto(long id, string modelo, string placa, string chassi, string? problema_identificado, long vaga_id)
        {
            Id = id;
            Modelo = modelo;
            Placa = placa;
            Chassi = chassi;
            ProblemaIdentificado = problema_identificado;
            VagaId = vaga_id;
        }

        public ICollection<Defeito> Defeitos { get; set; } = [];
        public ICollection<AlocacaoMoto> Alocacoes { get; set; } = [];
    }
}