using Microsoft.ML;
using Microsoft.ML.Data;

namespace GeoSense.API.Services
{
    /// <summary>
    /// Serviço ML.NET que faz classificação simples de risco de moto baseada nos dados da moto.
    /// </summary>
    public class MotoRiscoMlService
    {
        private readonly MLContext _mlContext;
        private readonly PredictionEngine<MotoRiscoInput, MotoRiscoOutput> _predictionEngine;

        public MotoRiscoMlService()
        {
            _mlContext = new MLContext();

            // Exemplo: risco ALTO se problema contém "motor" (dummy model para demonstração)
            var trainingData = new List<MotoRiscoInput>
            {
                new MotoRiscoInput { Modelo = "Honda CG", TipoVaga = 1, ProblemaIdentificado = "Motor com ruído excessivo", Risco = true },
                new MotoRiscoInput { Modelo = "Yamaha", TipoVaga = 0, ProblemaIdentificado = "", Risco = false },
                new MotoRiscoInput { Modelo = "Honda Biz", TipoVaga = 2, ProblemaIdentificado = "Sem placa", Risco = false },
                new MotoRiscoInput { Modelo = "Yamaha", TipoVaga = 1, ProblemaIdentificado = "Danos estruturais", Risco = true }
            };

            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);
            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(MotoRiscoInput.Risco))
                .Append(_mlContext.Transforms.Text.FeaturizeText("ModeloFeaturized", nameof(MotoRiscoInput.Modelo)))
                .Append(_mlContext.Transforms.Text.FeaturizeText("ProblemaFeaturized", nameof(MotoRiscoInput.ProblemaIdentificado)))
                .Append(_mlContext.Transforms.Concatenate("Features", "ModeloFeaturized", "ProblemaFeaturized", nameof(MotoRiscoInput.TipoVaga)))
                .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var model = pipeline.Fit(dataView);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<MotoRiscoInput, MotoRiscoOutput>(model);
        }

        public string ClassificarRisco(string modelo, int tipoVaga, string? problemaIdentificado)
        {
            var input = new MotoRiscoInput
            {
                Modelo = modelo,
                TipoVaga = tipoVaga,
                ProblemaIdentificado = problemaIdentificado ?? ""
            };
            var result = _predictionEngine.Predict(input);

            // Retorna "ALTO" se risco true (PredictedLabel = true), senão "BAIXO"
            return result.PredictedLabel ? "ALTO" : "BAIXO";
        }

        public class MotoRiscoInput
        {
            public string Modelo { get; set; } = "";
            public float TipoVaga { get; set; }
            public string ProblemaIdentificado { get; set; } = "";
            public bool Risco { get; set; }
        }

        public class MotoRiscoOutput
        {
            [ColumnName("PredictedLabel")]
            public bool PredictedLabel { get; set; }
        }
    }
}