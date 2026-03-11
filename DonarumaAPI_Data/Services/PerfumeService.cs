using Dapper;
using Npgsql;
using DonarumaAPI_Data.Interfaces;
using DonarumaAPI_Model.PerfumesModel;

namespace DonarumaAPI_Data.Services
{
    public class PerfumeService : IPerfumeService
    {
        private readonly PostgreSQLConfiguration _connection;

        public PerfumeService(PostgreSQLConfiguration connection)
        {
            _connection = connection;
        }

        protected NpgsqlConnection dbConnection()
        {
            return new NpgsqlConnection(_connection.ConnectionString);
        }

        public async Task<IEnumerable<Perfume>> ObtenerPorMarcaAsync(string marca)
        {
            using var db = dbConnection();
            var sql = @"SELECT idperfume AS Id, nombreperfume AS Nombre, marca AS Marca, precio AS Precio, 
                               descripcion AS Descripcion, imagen_url AS Imagen_Url, ocasion AS Ocasion, 
                               genero AS Genero, stock AS Stock 
                        FROM perfumes WHERE marca = @Marca";
            return await db.QueryAsync<Perfume>(sql, new { Marca = marca });
        }

        public async Task<IEnumerable<Perfume>> BuscarPorNombreAsync(string nombre)
        {
            using var db = dbConnection();
            var sql = @"SELECT idperfume AS Id, nombreperfume AS Nombre, marca AS Marca, precio AS Precio, 
                               descripcion AS Descripcion, imagen_url AS Imagen_Url, ocasion AS Ocasion, 
                               genero AS Genero, stock AS Stock 
                        FROM perfumes WHERE nombreperfume ILIKE @NombreBuscado";
            return await db.QueryAsync<Perfume>(sql, new { NombreBuscado = $"%{nombre}%" });
        }

        public async Task<IEnumerable<Perfume>> ObtenerPorPrecioMayorAsync()
        {
            using var db = dbConnection();
            var sql = @"SELECT idperfume AS Id, nombreperfume AS Nombre, marca AS Marca, precio AS Precio, 
                               descripcion AS Descripcion, imagen_url AS Imagen_Url, ocasion AS Ocasion, 
                               genero AS Genero, stock AS Stock 
                        FROM perfumes ORDER BY precio DESC";
            return await db.QueryAsync<Perfume>(sql);
        }

        public async Task<IEnumerable<Perfume>> ObtenerPorPrecioMenorAsync()
        {
            using var db = dbConnection();
            var sql = @"SELECT idperfume AS Id, nombreperfume AS Nombre, marca AS Marca, precio AS Precio, 
                               descripcion AS Descripcion, imagen_url AS Imagen_Url, ocasion AS Ocasion, 
                               genero AS Genero, stock AS Stock 
                        FROM perfumes ORDER BY precio ASC";
            return await db.QueryAsync<Perfume>(sql);
        }
    }
}