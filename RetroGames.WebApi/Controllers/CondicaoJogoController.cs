using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CondicaoJogoController : ControllerBase
{
    private readonly ICondicaoJogoService _condicaoJogoService;

    public CondicaoJogoController(
        ICondicaoJogoService condicaoJogoService)
    {
        _condicaoJogoService = condicaoJogoService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var condicoes = await _condicaoJogoService.ListarAsync();

        var data = condicoes
            .Select(condicao => condicao.ParaResponse())
            .ToList();

        return Ok(new RetroGamesResponseApiModel<List<ListarCondicaoJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = data,
            ErrorMessage = null
        });
    }
}
