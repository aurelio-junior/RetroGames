using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class JogoMapper
{
    public static (
        string? Busca,
        int? IdPlataforma,
        int? IdGenero,
        int Pagina,
        int TamanhoPagina)
        ParaFiltro(this ListarJogoRequestApiModel request)
    {
        return (
            request.Busca,
            request.IdPlataforma,
            request.IdGenero,
            request.Pagina,
            request.TamanhoPagina);
    }

    public static ListarJogoResponseApiModel ParaListarResponse(
        this JogoDetalheServiceModel detalhe)
    {
        var jogo = detalhe.Jogo;

        return new ListarJogoResponseApiModel
        {
            Id = jogo.Id,
            Nome = jogo.Nome,
            AnoLancamento = jogo.AnoLancamento,
            Preco = jogo.Preco,
            Estoque = jogo.Estoque,
            Ativo = jogo.Ativo,
            NomePlataforma = detalhe.NomePlataforma,
            DescricaoCondicao = detalhe.DescricaoCondicao,
            Generos = detalhe.Generos
                .Select(genero => genero.Nome)
                .ToList()
        };
    }

    public static ListarPorIdJogoResponseApiModel ParaDetalheResponse(
        this JogoDetalheServiceModel detalhe)
    {
        var jogo = detalhe.Jogo;

        return new ListarPorIdJogoResponseApiModel
        {
            Id = jogo.Id,
            Nome = jogo.Nome,
            Descricao = jogo.Descricao,
            AnoLancamento = jogo.AnoLancamento,
            Preco = jogo.Preco,
            Estoque = jogo.Estoque,
            Ativo = jogo.Ativo,
            IdPlataforma = jogo.IdPlataforma,
            IdCondicaoJogo = jogo.IdCondicaoJogo,
            NomePlataforma = detalhe.NomePlataforma,
            DescricaoCondicao = detalhe.DescricaoCondicao,
            Generos = detalhe.Generos
                .Select(genero => genero.ParaResponse())
                .ToList()
        };
    }
}
