namespace DonarumaAPI_DTOs.Compras
{
    public class ProcesarCompraDto
    {
        public long IdUsuario { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public long IdTarjeta { get; set; }
        public int[] PerfumesIds { get; set; } = Array.Empty<int>();
        public int[] Cantidades { get; set; } = Array.Empty<int>();
    }
}
