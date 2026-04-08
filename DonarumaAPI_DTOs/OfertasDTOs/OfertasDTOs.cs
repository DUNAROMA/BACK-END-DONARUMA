namespace DonarumaAPI_DTOs.OfertasDTOs
{
    // DTO para recibir los datos desde Angular al crear/modificar
    public class AgregarOfertaDTO
    {
        public int IdPerfume { get; set; }
        public int Descuento { get; set; }
        public decimal PrecioOferta { get; set; }
    }

    // DTO para recibir la fecha del reloj desde Angular
    public class RelojDTO
    {
        public DateTime FechaFinOferta { get; set; }
    }

    // DTO para enviar el catálogo completo a la página principal
    public class OfertaResponseDTO
    {
        public int IdOferta { get; set; }
        public int IdPerfume { get; set; }
        public int Descuento { get; set; }
        public decimal PrecioOferta { get; set; }
        public bool Activo { get; set; }
        public string NombrePerfume { get; set; }
        public string Marca { get; set; }
        public decimal PrecioOriginal { get; set; }
        public string Imagen_Url { get; set; }
        public int Stock { get; set; }
    }
}