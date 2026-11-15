using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    public JogoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    private static Jogo MapearJogo(MySqlDataReader leitor)
    {
        var ordinalDescricao = leitor.GetOrdinal("ds_jogo");

        return new Jogo
        {
            Id = leitor.GetInt32(leitor.GetOrdinal("co_seq_jogo")),
            Nome = leitor.GetString(leitor.GetOrdinal("no_jogo")),
            Descricao = leitor.IsDBNull(ordinalDescricao)
                ? null
                : leitor.GetString(ordinalDescricao),
            AnoLancamento = leitor.GetInt32(leitor.GetOrdinal("nu_ano_lancamento")),
            Preco = leitor.GetDecimal(leitor.GetOrdinal("vl_preco")),
            Estoque = leitor.GetInt32(leitor.GetOrdinal("qt_estoque")),
            Ativo = leitor.GetBoolean(leitor.GetOrdinal("st_ativo")),
            DataCadastro = leitor.GetDateTime(leitor.GetOrdinal("dt_cadastro")),
            IdPlataforma = leitor.GetInt32(leitor.GetOrdinal("co_plataforma")),
            IdCondicaoJogo = leitor.GetInt32(leitor.GetOrdinal("co_condicao_jogo"))
        };
    }

    public async Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        string? busca,
        int? idPlataforma,
        IReadOnlyCollection<int>? idsJogos,
        bool incluirInativos,
        int pagina,
        int tamanhoPagina)
    {
        if (idsJogos is not null && idsJogos.Count == 0)
        {
            return (new List<Jogo>(), 0);
        }

        var filtros = new List<string>();
        var parametros = new List<(string Nome, object Valor)>();

        if (!incluirInativos)
        {
            filtros.Add("st_ativo = 1");
        }

        if (!string.IsNullOrWhiteSpace(busca))
        {
            filtros.Add("no_jogo LIKE @busca");
            parametros.Add(("@busca", $"%{busca}%"));
        }

        if (idPlataforma.HasValue)
        {
            filtros.Add("co_plataforma = @idPlataforma");
            parametros.Add(("@idPlataforma", idPlataforma.Value));
        }

        if (idsJogos is not null)
        {
            var nomesParametros = new List<string>();

            var indice = 0;

            foreach (var idJogo in idsJogos)
            {
                var nomeParametro = $"@idJogo{indice++}";
                nomesParametros.Add(nomeParametro);
                parametros.Add((nomeParametro, idJogo));
            }

            filtros.Add($"co_seq_jogo IN ({string.Join(", ", nomesParametros)})");
        }

        var whereSql = filtros.Count > 0
            ? "WHERE " + string.Join(" AND ", filtros)
            : string.Empty;

        const string campos = """
            co_seq_jogo,
            no_jogo,
            ds_jogo,
            nu_ano_lancamento,
            vl_preco,
            qt_estoque,
            st_ativo,
            dt_cadastro,
            co_plataforma,
            co_condicao_jogo
            """;

        var sqlContagem = $"""
            SELECT COUNT(*)
            FROM tb_jogo
            {whereSql};
            """;

        var sqlListagem = $"""
            SELECT {campos}
            FROM tb_jogo
            {whereSql}
            ORDER BY no_jogo
            LIMIT @limite OFFSET @offset;
            """;

        var offset = (pagina - 1) * tamanhoPagina;

        await using var conexao = await AbrirConexaoAsync();

        await using var comandoContagem = CriarComando(conexao, sqlContagem);

        foreach (var parametro in parametros)
        {
            AdicionarParametro(
                comandoContagem,
                parametro.Nome,
                parametro.Valor);
        }

        var total = Convert.ToInt32(
            await comandoContagem.ExecuteScalarAsync());

        await using var comandoListagem = CriarComando(conexao, sqlListagem);

        foreach (var parametro in parametros)
        {
            AdicionarParametro(
                comandoListagem,
                parametro.Nome,
                parametro.Valor);
        }

        AdicionarParametro(comandoListagem, "@limite", tamanhoPagina);
        AdicionarParametro(comandoListagem, "@offset", offset);

        await using var leitor = await comandoListagem.ExecuteReaderAsync();

        var jogos = new List<Jogo>();

        while (await leitor.ReadAsync())
        {
            jogos.Add(MapearJogo(leitor));
        }

        return (jogos, total);
    }

    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        const string sql = """
            SELECT
                co_seq_jogo,
                no_jogo,
                ds_jogo,
                nu_ano_lancamento,
                vl_preco,
                qt_estoque,
                st_ativo,
                dt_cadastro,
                co_plataforma,
                co_condicao_jogo
            FROM tb_jogo
            WHERE co_seq_jogo = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await using var leitor = await comando.ExecuteReaderAsync();

        if (!await leitor.ReadAsync())
        {
            return null;
        }

        return MapearJogo(leitor);
    }

    public async Task<int> InserirAsync(Jogo jogo)
    {
        const string sql = """
            INSERT INTO tb_jogo
            (
                no_jogo,
                ds_jogo,
                nu_ano_lancamento,
                vl_preco,
                qt_estoque,
                st_ativo,
                dt_cadastro,
                co_plataforma,
                co_condicao_jogo
            )
            VALUES
            (
                @nome,
                @descricao,
                @anoLancamento,
                @preco,
                @estoque,
                @ativo,
                @dataCadastro,
                @idPlataforma,
                @idCondicaoJogo
            );
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@dataCadastro", jogo.DataCadastro);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);

        await ExecutarGravacaoAsync(comando);

        return checked((int)comando.LastInsertedId);
    }

    public async Task AlterarAsync(Jogo jogo)
    {
        const string sql = """
            UPDATE tb_jogo
            SET
                no_jogo = @nome,
                ds_jogo = @descricao,
                nu_ano_lancamento = @anoLancamento,
                vl_preco = @preco,
                qt_estoque = @estoque,
                co_plataforma = @idPlataforma,
                co_condicao_jogo = @idCondicaoJogo
            WHERE co_seq_jogo = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", jogo.Id);
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task DesativarAsync(int id)
    {
        const string sql = """
            UPDATE tb_jogo
            SET st_ativo = 0
            WHERE co_seq_jogo = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await ExecutarGravacaoAsync(comando);
    }
}
