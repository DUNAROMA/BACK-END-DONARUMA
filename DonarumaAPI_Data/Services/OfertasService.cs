using Dapper;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.OfertasDTOs;
using DonarumaAPI_Model.OfertasModel;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DonarumaAPI_Data.Services
{
    public class OfertasService : IOfertasService
    {
        private readonly PostgreSQLConfiguration _connectionConfig;

        public OfertasService(PostgreSQLConfiguration connectionConfig)
        {
            _connectionConfig = connectionConfig;
        }

        protected NpgsqlConnection dbConnection() => new NpgsqlConnection(_connectionConfig.ConnectionString);

        public async Task<IEnumerable<VistaGestionOferta>> ObtenerOfertasDetalladasAsync()
        {
            using var db = dbConnection();
            // Consultamos la vista directamente usando Dapper
            var sql = "SELECT * FROM vista_gestion_ofertas;";
            return await db.QueryAsync<VistaGestionOferta>(sql);
        }

        public async Task<ConfiguracionReloj> ObtenerRelojActivoAsync()
        {
            using var db = dbConnection();
            var sql = "SELECT * FROM ConfiguracionVenta LIMIT 1;";
            return await db.QueryFirstOrDefaultAsync<ConfiguracionReloj>(sql);
        }

        public async Task ActualizarRelojAsync(DateTime fechaFin)
        {
            using var db = dbConnection();
            var sql = "SELECT sp_actualizar_reloj_oferta(@Fecha);";
            await db.ExecuteScalarAsync(sql, new { Fecha = fechaFin });
        }

        public async Task<IEnumerable<OfertaResponseDTO>> ObtenerOfertasActivasAsync()
        {
            using var db = dbConnection();
            var sql = "SELECT * FROM vw_ofertas_privadas_activas;";
            return await db.QueryAsync<OfertaResponseDTO>(sql);
        }

        public async Task<int> AgregarOModificarOfertaAsync(AgregarOfertaDTO ofertaDto)
        {
            using var db = dbConnection();
            var sql = "SELECT sp_agregar_o_modificar_oferta(@IdPerfume, @Descuento, @PrecioOferta);";

            return await db.ExecuteScalarAsync<int>(sql, new
            {
                IdPerfume = ofertaDto.IdPerfume,
                Descuento = ofertaDto.Descuento,
                PrecioOferta = ofertaDto.PrecioOferta
            });
        }

        public async Task CambiarEstadoOfertaAsync(int idOferta, bool activo)
        {
            using var db = dbConnection();
            var sql = "SELECT sp_cambiar_estado_oferta(@IdOferta, @Activo);";
            await db.ExecuteScalarAsync(sql, new { IdOferta = idOferta, Activo = activo });
        }
    }
}