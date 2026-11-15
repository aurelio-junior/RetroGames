
-- ============================================================
-- RETROGAMES
-- AULA 1 - ETAPA 5: TESTES DE INTEGRIDADE
-- ============================================================

USE retrogames;

-- TESTE 1: PLATAFORMA INEXISTENTE
-- Comando executado manualmente.
-- Resultado esperado: ERRO 1452.
-- Resultado observado: ERRO 1452.
-- A chave estrangeira impediu a insercao do jogo.

-- TESTE 2: PLATAFORMA COM NOME DUPLICADO
-- Comando executado manualmente.
-- Resultado esperado: ERRO 1062.
-- Resultado observado: ERRO 1062.
-- A restricao UNIQUE impediu o nome duplicado.

-- TESTE 3: EXCLUSAO DE PLATAFORMA COM JOGOS
-- Comando executado manualmente.
-- Resultado esperado: ERRO 1451.
-- Resultado observado: ERRO 1451.
-- A chave estrangeira impediu a exclusao da plataforma.

-- TESTE 4: NOME DO JOGO NULO
-- Comando executado manualmente.
-- Resultado esperado: ERRO 1048.
-- Resultado observado: ERRO 1048.
-- A restricao NOT NULL impediu a insercao.

-- TESTE 5: PRECO IGUAL A ZERO
-- Comando executado manualmente.
-- Resultado esperado: ACEITO.
-- Resultado observado: ACEITO.
-- O registro foi inserido com vl_preco = 0.00.
-- A validacao do preco fica na camada Service (RN06).

-- ============================================================
-- COMANDOS UTILIZADOS NOS TESTES
-- ============================================================

-- Teste 1:
-- INSERT INTO tb_jogo
-- (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
--  qt_estoque, st_ativo, dt_cadastro,
--  co_plataforma, co_condicao_jogo)
-- VALUES
-- ('JOGO TESTE FK', 'Teste de chave estrangeira.',
--  1990, 10.00, 1, TRUE, NOW(), 9999, 1);

-- Teste 2:
-- INSERT INTO tb_plataforma
-- (no_plataforma, no_fabricante, nu_ano_lancamento)
-- VALUES ('Mega Drive', 'Sega', 1988);

-- Teste 3:
-- DELETE FROM tb_plataforma
-- WHERE co_seq_plataforma = 1;

-- Teste 4:
-- INSERT INTO tb_jogo
-- (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
--  qt_estoque, st_ativo, dt_cadastro,
--  co_plataforma, co_condicao_jogo)
-- VALUES
-- (NULL, 'Teste de NOT NULL.',
--  1990, 10.00, 1, TRUE, NOW(), 1, 1);

-- Teste 5:
-- INSERT INTO tb_jogo
-- (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
--  qt_estoque, st_ativo, dt_cadastro,
--  co_plataforma, co_condicao_jogo)
-- VALUES
-- ('JOGO_TESTE_PRECO_ZERO_ETAPA5',
--  'Teste de preco igual a zero.',
--  1990, 0.00, 1, TRUE, NOW(), 1, 1);

-- ============================================================
-- LIMPEZA
-- O registro de teste de preco zero deve ser removido.
-- ============================================================

DELETE FROM tb_jogo
WHERE no_jogo = 'JOGO_TESTE_PRECO_ZERO_ETAPA5';

-- ============================================================
-- FIM DOS TESTES
-- ============================================================
