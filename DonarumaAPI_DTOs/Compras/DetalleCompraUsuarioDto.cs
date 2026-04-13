using System;

namespace DonarumaAPI_DTOs.Compras
{
    public class DetalleCompraUsuarioDto
    {
        public string NombrePerfume { get; set; }
        public string Direccion { get; set; }
        public long Cantidad { get; set; }
        public DateTime Fecha_Compra { get; set; }
    }
}
