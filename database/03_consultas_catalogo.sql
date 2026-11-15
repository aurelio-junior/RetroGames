
-- ============================================================
-- RETROGAMES
-- AULA 1 - ETAPA 5: CONSULTAS DO CATALOGO
-- ============================================================

USE retrogames;

-- 1. CATALOGO
-- Jogos ativos, plataforma e condicao, ordenados por nome.

SELECT
    j.co_seq_jogo,
    j.no_jogo,
    p.no_plataforma,
    c.ds_condicao_jogo,
    j.nu_ano_lancamento,
    j.vl_preco,
    j.qt_estoque
FROM tb_jogo j
INNER JOIN tb_plataforma p
    ON j.co_plataforma = p.co_seq_plataforma
INNER JOIN tb_condicao_jogo c
    ON j.co_condicao_jogo = c.co_condicao_jogo
WHERE j.st_ativo = 1
ORDER BY j.no_jogo;


-- 2. GENEROS DE UM JOGO INFORMADO
-- Exemplo: 1 = Super Mario Bros.

SET @id_jogo = 1;

SELECT
    j.no_jogo,
    g.no_genero
FROM tb_jogo j
INNER JOIN tb_jogo_genero jg
    ON j.co_seq_jogo = jg.co_jogo
INNER JOIN tb_genero g
    ON jg.co_genero = g.co_seq_genero
WHERE j.co_seq_jogo = @id_jogo
ORDER BY g.no_genero;


-- 3. JOGOS DE UM GENERO INFORMADO
-- Exemplo: 3 = Plataforma.

SET @id_genero = 3;

SELECT
    g.no_genero,
    j.no_jogo,
    p.no_plataforma
FROM tb_genero g
INNER JOIN tb_jogo_genero jg
    ON g.co_seq_genero = jg.co_genero
INNER JOIN tb_jogo j
    ON jg.co_jogo = j.co_seq_jogo
INNER JOIN tb_plataforma p
    ON j.co_plataforma = p.co_seq_plataforma
WHERE g.co_seq_genero = @id_genero
ORDER BY j.no_jogo;


-- 4. QUANTIDADE DE JOGOS POR PLATAFORMA
-- Inclui plataformas sem jogos.

SELECT
    p.no_plataforma,
    COUNT(j.co_seq_jogo) AS quantidade_jogos
FROM tb_plataforma p
LEFT JOIN tb_jogo j
    ON p.co_seq_plataforma = j.co_plataforma
GROUP BY p.co_seq_plataforma, p.no_plataforma
ORDER BY p.no_plataforma;


-- 5. BUSCA POR PARTE DO NOME
-- Cinco itens por pagina.
-- Primeira pagina: OFFSET 0.
-- Segunda pagina: OFFSET 5.

SET @termo_busca = 'a';

SELECT
    co_seq_jogo,
    no_jogo,
    nu_ano_lancamento
FROM tb_jogo
WHERE no_jogo LIKE CONCAT('%', @termo_busca, '%')
ORDER BY no_jogo
LIMIT 5 OFFSET 0;


SELECT
    co_seq_jogo,
    no_jogo,
    nu_ano_lancamento
FROM tb_jogo
WHERE no_jogo LIKE CONCAT('%', @termo_busca, '%')
ORDER BY no_jogo
LIMIT 5 OFFSET 5;
