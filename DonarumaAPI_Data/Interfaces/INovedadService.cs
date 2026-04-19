using DonarumaAPI_DTOs.NovedadesDTOs;
using DonarumaAPI_Model.NovedadesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_Data.Interfaces
{
    public interface INovedadService
    {
        Task<List<NovedadesDTO>> ObtenerTodas();
        Task<int> CrearNovedad(NovedadesDTO novedad);
        Task<bool> ActualizarNovedad(NovedadesDTO novedad);
        Task<bool> EliminarNovedad(int idNovedad);
        Task<NovedadesDTO?> ObtenerPorId(int id);
        Task<bool> Eliminar(int id);
        Task<bool> Actualizar(int id, NovedadesDTO novedad);

    }
}
