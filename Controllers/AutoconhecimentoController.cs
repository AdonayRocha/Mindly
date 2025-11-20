using Mindly.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class AutoconhecimentoController : ControllerBase
{
    public class AutoconhecimentoRequest
    {
        public bool SenteCansaco { get; set; }
        public bool TemDificuldadeSono { get; set; }
        public bool SenteAnsiedade { get; set; }
        public bool TemDificuldadeConcentracao { get; set; }
        public bool SenteTristeza { get; set; }
        public bool TemPerdaInteresse { get; set; }
        public bool SenteIrritacao { get; set; }
        public bool TemPensamentosNegativos { get; set; }
    }

        
    public class AutoconhecimentoResponse
    {
        public string? Resultado { get; set; }
        public string[]? Dicas { get; set; }
    }

    /// <summary>
    /// Avalia sinais de bem-estar emocional com base em 8 indicadores.
    /// </summary>
    /// <remarks>
    /// A avaliação soma 1 ponto para cada item marcado como verdadeiro:
    /// cansaço, dificuldade de sono, ansiedade, dificuldade de concentração, tristeza,
    /// perda de interesse, irritação e pensamentos negativos.
    ///
    /// Faixas de resultado:
    /// - 0 a 2: equilíbrio emocional, sugestões leves de autocuidado.
    /// - 3 a 5: atenção a sinais de alerta, recomendações de rotina e relaxamento.
    /// - 6 a 8: alerta importante, recomendação de procurar acompanhamento profissional.
    /// </remarks>
    /// <response code="200">Retorna o resultado textual e uma lista de dicas</response>
    [HttpPost]
    [AdminProtect]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AutoconhecimentoResponse), StatusCodes.Status200OK)]
    public IActionResult Post([FromBody] AutoconhecimentoRequest req)
    {
        int score = 0;
        if (req.SenteCansaco) score++;
        if (req.TemDificuldadeSono) score++;
        if (req.SenteAnsiedade) score++;
        if (req.TemDificuldadeConcentracao) score++;
        if (req.SenteTristeza) score++;
        if (req.TemPerdaInteresse) score++;
        if (req.SenteIrritacao) score++;
        if (req.TemPensamentosNegativos) score++;

        string resultado;
        string[] dicas;
        if (score <= 2)
        {
            resultado = "Seu resultado indica bom equilíbrio emocional. Continue cuidando da sua saúde mental!";
            dicas = new[] { "Mantenha hábitos saudáveis", "Busque momentos de lazer", "Converse com amigos e familiares" };
        }
        else if (score <= 5)
        {
            resultado = "Atenção: alguns sinais de alerta emocional. Reflita sobre sua rotina e busque apoio se necessário.";
            dicas = new[] { "Pratique exercícios físicos", "Estabeleça uma rotina de sono", "Procure relaxar e meditar" };
        }
        else
        {
            resultado = "Alerta: sinais importantes de sofrimento emocional. Considere procurar um profissional de saúde mental.";
            dicas = new[] { "Procure um psicólogo", "Converse sobre seus sentimentos", "Não ignore sinais persistentes" };
        }
        return Ok(new AutoconhecimentoResponse { Resultado = resultado, Dicas = dicas });
    }
}
