namespace ManejoPresupuesto.Models
{
    public class TransaccionActualizacionDTO: TransaccionCreacionDTO
    {
        public int CuentaAnteriorId { get; set; }
        public decimal MontoAnterior { get; set; }
        public string UrlRetorno { get; set; }
    }
}
