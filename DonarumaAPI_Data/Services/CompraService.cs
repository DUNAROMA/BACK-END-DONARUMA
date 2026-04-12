// DonarumaAPI_Data/Services/CompraService.cs
using Dapper;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_DTOs.Compras;
using DonarumaAPI_DTOs.Compras.DonarumaAPI_DTOs.Compras;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonarumaAPI_Data.Services
{
    public class CompraService : ICompraService
    {
        private readonly string _connectionString;

        public CompraService(PostgreSQLConfiguration config)
        {
            _connectionString = config.ConnectionString;
        }

        private IDbConnection Connection => new NpgsqlConnection(_connectionString);

        public async Task<CompraResultadoDto> ProcesarCompraAsync(ProcesarCompraDto request)
        {
            using var db = Connection;

            var resultado = await db.ExecuteScalarAsync<string>(
                @"SELECT procesar_compra_con_stock(
                    @p_idusuario,
                    @p_direccion,
                    @p_idmetodo,
                    @p_perfumes_ids,
                    @p_cantidades
                  )",
                new
                {
                    p_idusuario = request.IdUsuario,
                    p_direccion = request.Direccion,
                    p_idmetodo = request.IdMetodo,
                    p_perfumes_ids = request.PerfumesIds,
                    p_cantidades = request.Cantidades
                }
            );

            return new CompraResultadoDto { Mensaje = resultado ?? "Compra procesada." };
        }
    }
}