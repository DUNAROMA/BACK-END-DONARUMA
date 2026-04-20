using Microsoft.AspNetCore.Mvc;
using DonarumaAPI_Data.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityLogsController : ControllerBase
    {
        private readonly ISecurityLogService _securityLogService;

        public SecurityLogsController(ISecurityLogService securityLogService)
        {
            _securityLogService = securityLogService;
        }

        [HttpGet]
        
        public async Task<IActionResult> ObtenerLogs([FromQuery] string? buscar = null, [FromQuery] int limite = 200)
        {
            
            var logs = await _securityLogService.ObtenerLogsFiltrados(buscar, limite);
            return Ok(logs);
        }
    }
}