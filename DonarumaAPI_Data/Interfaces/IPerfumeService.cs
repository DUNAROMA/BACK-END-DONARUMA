using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonarumaAPI_DTOs.PerfumesDTOs;

namespace DonarumaAPI_Data.Interfaces
{
    public interface IPerfumeService
    {
        Task<List<PerfumeDTO>> ObtenerTodos();
        Task<List<PerfumeDTO>> ObtenerDeNoche();
        Task<List<PerfumeDTO>> ObtenerPorOcasion(string ocasion); // Puedes usar parámetros para no hacer 20 funciones
        Task<List<PerfumeDTO>> ObtenerPorGenero(string genero);
        Task<int> CrearPerfume(PerfumeDTO perfume);


        Task<IEnumerable<PerfumeDTO>> ObtenerPorMarcaAsync(string marca);
        Task<IEnumerable<PerfumeDTO>> BuscarPorNombreAsync(string nombre);
        Task<IEnumerable<PerfumeDTO>> ObtenerPorPrecioMayorAsync();
        Task<IEnumerable<PerfumeDTO>> ObtenerPorPrecioMenorAsync();

    }
}
