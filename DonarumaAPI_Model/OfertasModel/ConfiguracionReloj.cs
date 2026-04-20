namespace DonarumaAPI_Model.OfertasModel
{
    
    public class ConfiguracionReloj
    {
        public int IdConfiguracion { get; set; }
        public DateTime FechaFinOferta { get; set; }
        public bool Activo { get; set; }
        public DateTime UltimaActualizacion { get; set; }
    }
}