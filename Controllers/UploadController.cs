using Microsoft.AspNetCore.Mvc;

namespace SignalR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        [HttpPost("arquivo")]
        public async Task<IActionResult> EnviarArquivo([FromForm] IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Nenhum arquivo enviado.");

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var nomeArquivo = Guid.NewGuid() + Path.GetExtension(arquivo.FileName);
            var caminhoCompleto = Path.Combine(uploads, nomeArquivo);

            using var stream = new FileStream(caminhoCompleto, FileMode.Create);
            await arquivo.CopyToAsync(stream);

            var url = $"/uploads/{nomeArquivo}";
            return Ok(new { url });
        }
    }
}
