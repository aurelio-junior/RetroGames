using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlataformaController : ControllerBase
{
    private readonly IPlataformaService _plataformaService;

    public PlataformaController(IPlataformaService plataformaService)
    {
        _plataformaService = plataformaService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var plataformas = await _plataformaService.ListarAsync();

        var data = plataformas
            .Select(plataforma => plataforma.ParaResponse())
            .ToList();

        return Ok(new RetroGamesResponseApiModel<List<ListarPlataformaResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = data,
            ErrorMessage = null
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ListarPorId(int id)
    {
        var plataforma = await _plataformaService.BuscarPorIdAsync(id);

        var data = plataforma.ParaResponse();

        return Ok(new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = data,
            ErrorMessage = null
        });
    }
}
