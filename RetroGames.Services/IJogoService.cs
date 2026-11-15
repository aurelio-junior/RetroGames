using RetroGames.Model.Model;

namespace RetroGames.Services;

public interface IJogoService
{
    Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        string? busca,
        int? idPlataforma,
        int? idGenero,
        int pagina,
        int tamanhoPagina);

    Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id);

    Task<JogoDetalheServiceModel> InserirAsync(
        Jogo jogo,
        IReadOnlyCollection<int> idsGeneros);

    Task<JogoDetalheServiceModel> AlterarAsync(
        Jogo jogo,
        IReadOnlyCollection<int> idsGeneros);

    Task DesativarAsync(int id);
}
