namespace DonarumaAPI_DTOs.OfertasDTOs
{
    
    public class AgregarOfertaDTO
    {
        public int IdPerfume { get; set; }
        public int Descuento { get; set; }
        public decimal PrecioOferta { get; set; }
    }

    public class RelojDTO
    {
        public DateTime FechaFinOferta { get; set; }
    }

    
    public class OfertaResponseDTO
    {
        public int IdOferta { get; set; }
        public int IdPerfume { get; set; }
        public int Descuento { get; set; }
        public decimal PrecioOferta { get; set; }
        public bool Activo { get; set; }
        public string NombrePerfume { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public decimal PrecioOriginal { get; set; }
        public string Imagen_Url { get; set; } = string.Empty;
        public int Stock { get; set; }
    }
}