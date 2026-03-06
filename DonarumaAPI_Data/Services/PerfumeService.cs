using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.PerfumesDTOs;

namespace DonarumaAPI_Data.Services
{
    public class PerfumeService : IPerfumeService
    {
        // Alan: Aquí va el código que se conecta a PostgreSQL 
        // y ejecuta el SELECT de cada función.

        public async Task<List<PerfumeDTO>> ObtenerTodos()
        {
            // Lógica de base de datos...
            throw new NotImplementedException();
        }

        // ... (implementar las demás funciones)
    }
}
