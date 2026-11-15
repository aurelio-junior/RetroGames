using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<List<JogoGenero>> ListarTodosAsync()
    {
        const string sql = """
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero
            ORDER BY co_jogo, co_genero;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        var vinculos = new List<JogoGenero>();

        while (await leitor.ReadAsync())
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = leitor.GetInt32(leitor.GetOrdinal("co_jogo")),
                IdGenero = leitor.GetInt32(leitor.GetOrdinal("co_genero"))
            });
        }

        return vinculos;
    }

    public async Task<List<JogoGenero>> ListarPorJogosAsync(
        IReadOnlyCollection<int> idsJogos)
    {
        if (idsJogos.Count == 0)
        {
            return new List<JogoGenero>();
        }

        var nomesParametros = idsJogos
            .Select((id, indice) => $"@idJogo{indice}")
            .ToList();

        var sql = $"""
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero
            WHERE co_jogo IN ({string.Join(", ", nomesParametros)})
            ORDER BY co_jogo, co_genero;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        var indice = 0;

        foreach (var id in idsJogos)
        {
            AdicionarParametro(comando, $"@idJogo{indice++}", id);
        }

        await using var leitor = await comando.ExecuteReaderAsync();

        var vinculos = new List<JogoGenero>();

        while (await leitor.ReadAsync())
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = leitor.GetInt32(leitor.GetOrdinal("co_jogo")),
                IdGenero = leitor.GetInt32(leitor.GetOrdinal("co_genero"))
            });
        }

        return vinculos;
    }

    public async Task InserirAsync(
        int idJogo,
        IReadOnlyCollection<int> idsGeneros)
    {
        if (idsGeneros.Count == 0)
        {
            return;
        }

        var valores = new List<string>();

        var parametros = new List<(string Nome, int Valor)>();

        var indice = 0;

        foreach (var idGenero in idsGeneros)
        {
            var parametroJogo = $"@idJogo{indice}";
            var parametroGenero = $"@idGenero{indice}";

            valores.Add($"({parametroJogo}, {parametroGenero})");

            parametros.Add((parametroJogo, idJogo));
            parametros.Add((parametroGenero, idGenero));

            indice++;
        }

        var sql = $"""
            INSERT INTO tb_jogo_genero (co_jogo, co_genero)
            VALUES {string.Join(", ", valores)};
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        foreach (var parametro in parametros)
        {
            AdicionarParametro(
                comando,
                parametro.Nome,
                parametro.Valor);
        }

        await ExecutarGravacaoAsync(comando);
    }

    public async Task RemoverPorJogoAsync(int idJogo)
    {
        const string sql = """
            DELETE FROM tb_jogo_genero
            WHERE co_jogo = @idJogo;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idJogo", idJogo);

        await ExecutarGravacaoAsync(comando);
    }
}
