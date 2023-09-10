namespace ManejoPresupuesto.Models
{
    public class ReporteTransaccionesDetalladas
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set;}
        public IEnumerable<TransaccionesPorFecha> TransaccionesAgrupadas { get; set; }
        public decimal BalanceDepositos => TransaccionesAgrupadas.Sum(t => t.BalanceDepositos);
        public decimal BalanceRetiros => TransaccionesAgrupadas.Sum(_ => _.BalanceRetiros);
        public decimal Total => BalanceDepositos - BalanceRetiros;

        public class TransaccionesPorFecha
        {
            public DateTime FechaTransaccion { get; set; }
            public IEnumerable<Transaccion> Transacciones { get; set; }
            public Decimal BalanceDepositos => Transacciones.Where(t => t.TipoOperacionId == TipoOperacion.Ingreso).Sum(t => t.Monto);
            public Decimal BalanceRetiros => Transacciones.Where(t => t.TipoOperacionId == TipoOperacion.Gasto).Sum(t => t.Monto);
        }
    }
}
