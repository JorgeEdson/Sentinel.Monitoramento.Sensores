namespace Sentinel.Monitoramento.Sensores.Modelos
{
    public class DadosSensor
    {
        public long Id { get; set; }
        public long IdMaquina { get; set; }
        public long IdSensor { get; set; }
        public TipoSensor TipoSensor { get; set; }
        public double Valor { get; set; }
        public DateTime DataHora { get; set; }
        public bool Enviado { get; set; }
    }

    public enum TipoSensor
    {
        Temperatura,
        FluidoArrefecimento,
        RotacaoMotor
    }
}
