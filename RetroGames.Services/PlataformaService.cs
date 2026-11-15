using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class PlataformaService : IPlataformaService
{
    private readonly IRepositorySet _repositorio;

    public PlataformaService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<Plataforma>> ListarAsync()
    {
        return await _repositorio.Plataforma.ListarAsync();
    }

    public async Task<Plataforma> BuscarPorIdAsync(int id)
    {
        var plataforma = await _repositorio.Plataforma.BuscarPorIdAsync(id);

        if (plataforma == null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
        }

        return plataforma;
    }

    public async Task<Plataforma> InserirAsync(Plataforma plataforma)
    {
        plataforma.Nome = plataforma.Nome.Trim();

        var existe = await _repositorio.Plataforma.ExisteComNomeAsync(
            plataforma.Nome);

        if (existe)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        }

        try
        {
            plataforma.Id = await _repositorio.Plataforma.InserirAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        }

        return plataforma;
    }

    public async Task AlterarAsync(Plataforma plataforma)
    {
        var existente = await _repositorio.Plataforma.BuscarPorIdAsync(
            plataforma.Id);

        if (existente == null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
        }

        plataforma.Nome = plataforma.Nome.Trim();

        var existe = await _repositorio.Plataforma.ExisteComNomeAsync(
            plataforma.Nome,
            plataforma.Id);

        if (existe)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        }

        try
        {
            await _repositorio.Plataforma.AlterarAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        }
    }

    public async Task ExcluirAsync(int id)
    {
        var plataforma = await _repositorio.Plataforma.BuscarPorIdAsync(id);

        if (plataforma == null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
        }

        var possuiJogos = await _repositorio.Plataforma.PossuiJogosAsync(id);

        if (possuiJogos)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_POSSUI_JOGOS);
        }

        await _repositorio.Plataforma.ExcluirAsync(id);
    }
}
