using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private readonly IRepositorySet _repositorio;

    public JogoService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        string? busca,
        int? idPlataforma,
        int? idGenero,
        int pagina,
        int tamanhoPagina)
    {
        IReadOnlyCollection<int>? idsJogos = null;

        if (idGenero.HasValue)
        {
            var vinculos = await _repositorio.JogoGenero.ListarTodosAsync();

            idsJogos = vinculos
                .Where(vinculo => vinculo.IdGenero == idGenero.Value)
                .Select(vinculo => vinculo.IdJogo)
                .Distinct()
                .ToList();
        }

        var resultado = await _repositorio.Jogo.ListarAsync(
            busca,
            idPlataforma,
            idsJogos,
            false,
            pagina,
            tamanhoPagina);

        var detalhes = await MontarDetalhesAsync(resultado.Jogos);

        return (detalhes, resultado.Total);
    }

    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);

        if (jogo == null || !jogo.Ativo)
        {
            throw new BusinessRuleException(
                BusinessErrors.JOGO_NAO_ENCONTRADO);
        }

        var detalhes = await MontarDetalhesAsync(new List<Jogo> { jogo });

        return detalhes[0];
    }

    public async Task<JogoDetalheServiceModel> InserirAsync(
        Jogo jogo,
        IReadOnlyCollection<int> idsGeneros)
    {
        var idsGenerosUnicos = idsGeneros.Distinct().ToList();

        using (var transacao = CriarTransacao())
        {
            await ValidarReferenciasAsync(
                jogo.IdPlataforma,
                jogo.IdCondicaoJogo,
                idsGenerosUnicos);

            jogo.Nome = jogo.Nome.Trim();
            jogo.Ativo = true;
            jogo.DataCadastro = DateTime.UtcNow;

            jogo.Id = await _repositorio.Jogo.InserirAsync(jogo);

            await _repositorio.JogoGenero.InserirAsync(
                jogo.Id,
                idsGenerosUnicos);

            transacao.Complete();
        }

        var jogoInserido = await _repositorio.Jogo.BuscarPorIdAsync(jogo.Id);

        if (jogoInserido == null)
        {
            throw new BusinessRuleException(
                BusinessErrors.JOGO_NAO_ENCONTRADO);
        }

        var detalhes = await MontarDetalhesAsync(
            new List<Jogo> { jogoInserido });

        return detalhes[0];
    }

    public async Task<JogoDetalheServiceModel> AlterarAsync(
        Jogo jogo,
        IReadOnlyCollection<int> idsGeneros)
    {
        var idsGenerosUnicos = idsGeneros.Distinct().ToList();

        using (var transacao = CriarTransacao())
        {
            var existente = await _repositorio.Jogo.BuscarPorIdAsync(jogo.Id);

            if (existente == null)
            {
                throw new BusinessRuleException(
                    BusinessErrors.JOGO_NAO_ENCONTRADO);
            }

            await ValidarReferenciasAsync(
                jogo.IdPlataforma,
                jogo.IdCondicaoJogo,
                idsGenerosUnicos);

            jogo.Nome = jogo.Nome.Trim();

            await _repositorio.Jogo.AlterarAsync(jogo);

            await _repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);

            await _repositorio.JogoGenero.InserirAsync(
                jogo.Id,
                idsGenerosUnicos);

            transacao.Complete();
        }

        var jogoAlterado = await _repositorio.Jogo.BuscarPorIdAsync(jogo.Id);

        if (jogoAlterado == null)
        {
            throw new BusinessRuleException(
                BusinessErrors.JOGO_NAO_ENCONTRADO);
        }

        var detalhes = await MontarDetalhesAsync(
            new List<Jogo> { jogoAlterado });

        return detalhes[0];
    }

    public async Task DesativarAsync(int id)
    {
        var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);

        if (jogo == null)
        {
            throw new BusinessRuleException(
                BusinessErrors.JOGO_NAO_ENCONTRADO);
        }

        await _repositorio.Jogo.DesativarAsync(id);
    }

    private async Task ValidarReferenciasAsync(
        int idPlataforma,
        int idCondicaoJogo,
        IReadOnlyCollection<int> idsGeneros)
    {
        var plataforma = await _repositorio.Plataforma.BuscarPorIdAsync(
            idPlataforma);

        if (plataforma == null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
        }

        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();

        if (!condicoes.Any(condicao => condicao.Id == idCondicaoJogo))
        {
            throw new BusinessRuleException(
                BusinessErrors.CONDICAO_NAO_ENCONTRADA);
        }

        var generos = await _repositorio.Genero.ListarAsync();

        if (idsGeneros.Count == 0)
        {
            throw new BusinessRuleException(
                BusinessErrors.GENERO_NAO_ENCONTRADO);
        }

        var idsGenerosExistentes = generos
            .Select(genero => genero.Id)
            .ToHashSet();

        if (idsGeneros.Any(idGenero => !idsGenerosExistentes.Contains(idGenero)))
        {
            throw new BusinessRuleException(
                BusinessErrors.GENERO_NAO_ENCONTRADO);
        }
    }

    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(
        List<Jogo> jogos)
    {
        if (jogos.Count == 0)
        {
            return new List<JogoDetalheServiceModel>();
        }

        var idsJogos = jogos
            .Select(jogo => jogo.Id)
            .ToList();

        var vinculos = await _repositorio.JogoGenero.ListarPorJogosAsync(
            idsJogos);

        var plataformas = await _repositorio.Plataforma.ListarAsync();

        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();

        var generos = await _repositorio.Genero.ListarAsync();

        var plataformasPorId = plataformas.ToDictionary(
            plataforma => plataforma.Id);

        var condicoesPorId = condicoes.ToDictionary(
            condicao => condicao.Id);

        var generosPorId = generos.ToDictionary(
            genero => genero.Id);

        var detalhes = jogos.Select(jogo =>
        {
            var generosDoJogo = vinculos
                .Where(vinculo => vinculo.IdJogo == jogo.Id)
                .Select(vinculo => generosPorId[vinculo.IdGenero])
                .ToList();

            return new JogoDetalheServiceModel
            {
                Jogo = jogo,

                NomePlataforma = plataformasPorId[jogo.IdPlataforma].Nome,

                DescricaoCondicao = condicoesPorId[
                    jogo.IdCondicaoJogo].Descricao,

                Generos = generosDoJogo
            };
        }).ToList();

        return detalhes;
    }

    private static TransactionScope CriarTransacao()
    {
        var opcoes = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        return new TransactionScope(
            TransactionScopeOption.Required,
            opcoes,
            TransactionScopeAsyncFlowOption.Enabled);
    }
}
