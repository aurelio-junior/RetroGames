using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _jogoService;

    public JogoController(IJogoService jogoService)
    {
        _jogoService = jogoService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] ListarJogoRequestApiModel request)
    {
        var filtro = request.ParaFiltro();

        var (jogos, total) = await _jogoService.ListarAsync(
            filtro.Busca,
            filtro.IdPlataforma,
            filtro.IdGenero,
            filtro.Pagina,
            filtro.TamanhoPagina);

        var itens = jogos
            .Select(jogo => jogo.ParaListarResponse())
            .ToList();

        var totalPaginas = filtro.TamanhoPagina > 0
            ? (int)Math.Ceiling((double)total / filtro.TamanhoPagina)
            : 0;

        var pagina = new PaginaResponseApiModel<ListarJogoResponseApiModel>
        {
            Itens = itens,
            Pagina = filtro.Pagina,
            TamanhoPagina = filtro.TamanhoPagina,
            TotalItens = total,
            TotalPaginas = totalPaginas
        };

        return Ok(new RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = pagina,
            ErrorMessage = null
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ListarPorId(int id)
    {
        var jogo = await _jogoService.BuscarPorIdAsync(id);

        var data = jogo.ParaDetalheResponse();

        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = data,
            ErrorMessage = null
        });
    }
}
