using Mindly.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class CartilhasController : ControllerBase
{
    [HttpGet]
    [AdminProtect]
    public IActionResult Get([FromQuery] string? emocao = null)
    {
        var cartilhas = new[]
        {
            new {
                Titulo = "Tristeza no trabalho",
                Conteudo = "Reconheça seus sentimentos e busque apoio. Pratique o autocuidado, converse com colegas ou um profissional, e tente identificar gatilhos. Exercícios de respiração e pausas durante o expediente podem ajudar. Se a tristeza persistir, procure acompanhamento psicológico.",
                Emo = "tristeza"
            },
            new {
                Titulo = "Felicidade no trabalho",
                Conteudo = "Valorize conquistas, celebre pequenas vitórias e compartilhe bons momentos com a equipe. Mantenha hábitos saudáveis e busque equilíbrio entre vida pessoal e profissional. A felicidade pode ser cultivada com gratidão e reconhecimento.",
                Emo = "felicidade"
            },
            new {
                Titulo = "Raiva no trabalho",
                Conteudo = "Identifique sinais de irritação e afaste-se da situação se necessário. Pratique técnicas de relaxamento, como respiração profunda. Evite tomar decisões impulsivas e, se possível, converse sobre o que incomoda de forma assertiva. Procure apoio se a raiva for frequente.",
                Emo = "raiva"
            },
            new {
                Titulo = "Ansiedade no trabalho",
                Conteudo = "Organize tarefas, estabeleça prioridades e faça pausas regulares. Pratique mindfulness ou meditação. Evite excesso de cafeína e busque apoio profissional se a ansiedade estiver prejudicando seu desempenho ou bem-estar.",
                Emo = "ansiedade"
            },
            new {
                Titulo = "Medo no trabalho",
                Conteudo = "Tente identificar a origem do medo e converse com alguém de confiança. Enfrente situações desafiadoras gradualmente e celebre avanços. Se o medo for intenso ou persistente, procure orientação psicológica.",
                Emo = "medo"
            },
            new {
                Titulo = "Burnout",
                Conteudo = "Fique atento a sinais como exaustão, desmotivação e queda de rendimento. Busque pausas, delegue tarefas quando possível e converse com a liderança sobre sobrecarga. O acompanhamento psicológico é fundamental para recuperação do burnout.",
                Emo = "burnout"
            }
        };
        var result = string.IsNullOrWhiteSpace(emocao)
            ? cartilhas
            : cartilhas.Where(c => c.Emo.Equals(emocao, StringComparison.OrdinalIgnoreCase));
        return Ok(result);
    }
}