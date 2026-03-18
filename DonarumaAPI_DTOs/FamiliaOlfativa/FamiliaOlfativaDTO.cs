using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_DTOs.FamiliaOlfativa
{
    public class FamiliaOlfativaDTO
    {
        // "idPerfume" BIGINT -> Nunca es nulo, así que usamos long normal
        public long IdPerfume { get; set; }

        // nombre VARCHAR
        public string? Nombre { get; set; }

        // marca VARCHAR
        public string? Marca { get; set; }

        // genero VARCHAR
        public string? Genero { get; set; }

        // ocasion VARCHAR
        public string? Ocasion { get; set; }

        // precio NUMERIC(10,2) -> Puede ser nulo, usamos decimal?
        public decimal? Precio { get; set; }

        // descripcion TEXT -> ¡Este es el que te dio el último error! Usamos string?
        public string? Descripcion { get; set; }

        // "imagen_Url" TEXT
        public string? Imagen_Url { get; set; }

        // stock INTEGER -> ¡Este fue el primer error! Usamos int?
        public int? Stock { get; set; }

        // "familiaOlfativa" VARCHAR
        public string? FamiliaOlfativa { get; set; }
    }
}