using DonarumaAPI_Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Backend_Donaruma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class SecurityLogsController : ControllerBase
    {
        private readonly ISecurityLogService _logService;

        public SecurityLogsController(ISecurityLogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerLogs()
        {
            var logs = await _logService.ObtenerTodosLosLogs();
            return Ok(logs);
        }
    }
}