using AutoMapper;
using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Servicios
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Cuenta, CuentaCreacionDTO>();
            CreateMap<TransaccionActualizacionDTO, Transaccion>().ReverseMap();
        }
    }
}
