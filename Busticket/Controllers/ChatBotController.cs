using GenerativeAI;
using Microsoft.AspNetCore.Mvc;

namespace Busticket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatBotController : ControllerBase
    {
        private readonly string _apiKey;

        public ChatBotController(IConfiguration configuration)
        {
            _apiKey = configuration["Gemini:ApiKey"] ?? "";
        }

        [HttpPost("preguntar")]
        public async Task<IActionResult> Preguntar([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Mensaje))
            {
                return BadRequest(new { respuesta = "El mensaje está vacío." });
            }

            try
            {
                // MODELO ACTUAL DE GEMINI
                var client = new GenerativeModel(_apiKey, "gemini-2.5-flash");

                string prompt = $"Eres el asistente virtual de Busticket, una plataforma para comprar tiquetes de bus. " +
                                $"Responde de forma amable y corta. Usuario: {request.Mensaje}";

                var response = await client.GenerateContentAsync(prompt);

                return Ok(new
                {
                    respuesta = response.Text
                });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("429"))
                {
                    return Ok(new
                    {
                        respuesta = "Estoy iniciando. Espera unos segundos y vuelve a intentarlo 🚌"
                    });
                }

                return StatusCode(500, new
                {
                    respuesta = "Error técnico: " + ex.Message
                });
            }
        }
    }

    public class ChatRequest
    {
        public string Mensaje { get; set; } = "";
    }
}