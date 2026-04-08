namespace DonarumaAPI_Model.OfertasModel
{
    // Modelo que representa la tabla ConfiguracionVenta
    public class ConfiguracionReloj
    {
        public int IdConfiguracion { get; set; }
        public DateTime FechaFinOferta { get; set; }
        public bool Activo { get; set; }
        public DateTime UltimaActualizacion { get; set; }
    }
}