using GeoSense.API.Domain;
using GeoSense.API.Domain.Enums;

namespace GeoSense.API.Infrastructure.Persistence
{
    public class Vaga
    {
        public long Id { get; private set; }
        public int Numero { get; private set; }
        public TipoVaga Tipo { get; private set; }
        public StatusVaga Status { get; private set; }
        public long PatioId { get; private set; }
        public virtual Patio? Patio { get; set; } // nullable para evitar warning

        protected Vaga() { }

        public Vaga(int numero, long patioId)
        {
            Numero = numero;
            Tipo = TipoVaga.Sem_Problema;
            Status = StatusVaga.LIVRE;
            PatioId = patioId;
        }

        public ICollection<Moto> Motos { get; set; } = [];
        public ICollection<AlocacaoMoto> Alocacoes { get; set; } = [];
    }
}