using Microsoft.AspNetCore.Mvc;
using SignalR.Services;

namespace SignalR.Controllers
{
    [Route("api/ia")]
    [ApiController]
    public class IaController : ControllerBase
    {
        private readonly IaService _iaService;

        public IaController(IaService iaService)
        {
            _iaService = iaService;
        }

        [HttpPost("comando")]
        public async Task<IActionResult> Comando([FromBody] ComandoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Comando))
                return BadRequest(new { erro = "O comando não pode ser vazio." });

            try
            {
                var resposta = await _iaService.ConsultarIa(request.Comando);
                return Ok(new { resposta });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        [HttpPost("resumo")]
        public async Task<IActionResult> Resumo([FromBody] ResumoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Conversa))
                return BadRequest(new { erro = "A conversa não pode ser vazia." });

            try
            {
                var prompt = $"Faça um resumo detalhado desta conversa de chat:\n\n{request.Conversa}";
                var resumo = await _iaService.ConsultarIa(prompt);
                return Ok(new { resumo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }
    }

    public class ComandoRequest
    {
        public string Comando { get; set; }
    }

    public class ResumoRequest
    {
        public string Conversa { get; set; }
    }
}
