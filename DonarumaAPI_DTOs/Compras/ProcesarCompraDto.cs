namespace DonarumaAPI_DTOs.Compras
{
    // DonarumaAPI_DTOs/Compras/ProcesarCompraDto.cs
    namespace DonarumaAPI_DTOs.Compras
    {
        public class ProcesarCompraDto
        {
            public long IdUsuario { get; set; }
            public string Direccion { get; set; }
            public int IdMetodo { get; set; } // antes era IdTarjeta
            public long[] PerfumesIds { get; set; }
            public int[] Cantidades { get; set; }
        }
        public class CompraResultadoDto
        {
            public string Mensaje { get; set; }
        }
    }
}
