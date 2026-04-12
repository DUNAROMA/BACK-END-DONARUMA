using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonarumaAPI_DTOs.PerfumesDTOs;
<<<<<<< HEAD
﻿using DonarumaAPI_Model.PerfumesModel;
=======
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619

namespace DonarumaAPI_Data.Interfaces
{
    public interface IPerfumeService
    {
        Task<List<PerfumeDTO>> ObtenerTodos();
        Task<List<PerfumeDTO>> ObtenerDeNoche();
        Task<List<PerfumeDTO>> ObtenerPorOcasion(string ocasion); // Puedes usar parámetros para no hacer 20 funciones
        Task<List<PerfumeDTO>> ObtenerPorGenero(string genero);
        Task<int> CrearPerfume(PerfumeDTO perfume);
<<<<<<< HEAD

        Task<IEnumerable<PerfumeDTO>> ObtenerPorMarcaAsync(string marca);
        Task<IEnumerable<PerfumeDTO>> BuscarPorNombreAsync(string nombre);
        Task<IEnumerable<PerfumeDTO>> ObtenerPorPrecioMayorAsync();
        Task<IEnumerable<PerfumeDTO>> ObtenerPorPrecioMenorAsync();
        Task<bool> ActualizarPerfume(PerfumeDTO perfume);
        Task<bool> EliminarPerfume(int idPerfume);

    }
}
       
=======
    }
}
>>>>>>> 2fa79a9c8a6d4349ca87bfe5778a812a9f19a619
