
-- ============================================================
-- RETROGAMES
-- AULA 1 - ETAPA 4: DADOS DE EXEMPLO
-- ============================================================

USE retrogames;

SET NAMES utf8mb4;

START TRANSACTION;

-- ============================================================
-- 1. CONDIÇÕES DOS JOGOS
-- Chaves fixas definidas na especificação.
-- ============================================================

INSERT INTO tb_condicao_jogo
    (co_condicao_jogo, ds_condicao_jogo)
VALUES
    (1, 'Novo'),
    (2, 'Usado'),
    (3, 'Recondicionado');


-- ============================================================
-- 2. PERFIS
-- Chaves fixas definidas na especificação.
-- ============================================================

INSERT INTO tb_perfil
    (co_perfil, ds_perfil)
VALUES
    (1, 'Administrador'),
    (2, 'Cliente');


-- ============================================================
-- 3. PLATAFORMAS
-- IDs explícitos para facilitar a associação dos jogos.
-- ============================================================

INSERT INTO tb_plataforma
    (co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES
    (1, 'Nintendo Entertainment System', 'Nintendo', 1983),
    (2, 'Super Nintendo Entertainment System', 'Nintendo', 1990),
    (3, 'Mega Drive', 'Sega', 1988),
    (4, 'PlayStation', 'Sony', 1994),
    (5, 'Atari 2600', 'Atari', 1977),
    (6, 'Game Boy', 'Nintendo', 1989);


-- ============================================================
-- 4. GÊNEROS
-- ============================================================

INSERT INTO tb_genero
    (co_seq_genero, no_genero)
VALUES
    (1, 'Ação'),
    (2, 'Aventura'),
    (3, 'Plataforma'),
    (4, 'RPG'),
    (5, 'Corrida'),
    (6, 'Luta'),
    (7, 'Esporte'),
    (8, 'Puzzle');


-- ============================================================
-- 5. JOGOS
-- Os preços são valores fictícios para demonstração.
-- Cada jogo utiliza uma plataforma e uma condição cadastradas.
-- ============================================================

INSERT INTO tb_jogo
    (
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
    )
VALUES

-- NINTENDO ENTERTAINMENT SYSTEM (NES)

(1, 'Super Mario Bros.',
 'Jogo clássico de plataforma protagonizado por Mario.',
 1985, 49.90, 10, TRUE, NOW(), 1, 2),

(2, 'The Legend of Zelda',
 'Aventura em que Link explora Hyrule para resgatar a princesa Zelda.',
 1986, 69.90, 5, TRUE, NOW(), 1, 2),

(3, 'Metroid',
 'Aventura de ação protagonizada pela exploradora Samus Aran.',
 1986, 59.90, 4, TRUE, NOW(), 1, 3),

(4, 'Mega Man 2',
 'Jogo de ação e plataforma da série Mega Man.',
 1988, 54.90, 7, TRUE, NOW(), 1, 2),


-- SUPER NINTENDO ENTERTAINMENT SYSTEM (SNES)

(5, 'Super Mario World',
 'Aventura de plataforma protagonizada por Mario e Luigi.',
 1990, 89.90, 8, TRUE, NOW(), 2, 2),

(6, 'The Legend of Zelda: A Link to the Past',
 'Aventura de ação ambientada no reino de Hyrule.',
 1991, 99.90, 3, TRUE, NOW(), 2, 3),

(7, 'Chrono Trigger',
 'RPG com viagens no tempo e diferentes períodos históricos.',
 1995, 129.90, 2, TRUE, NOW(), 2, 2),

(8, 'Street Fighter II Turbo',
 'Jogo de luta com diversos personagens e combates.',
 1993, 79.90, 6, TRUE, NOW(), 2, 2),


-- MEGA DRIVE

(9, 'Sonic the Hedgehog 2',
 'Jogo de plataforma protagonizado por Sonic e Tails.',
 1992, 69.90, 9, TRUE, NOW(), 3, 2),

(10, 'Streets of Rage 2',
 'Jogo de ação e combate contra grupos de inimigos.',
 1992, 74.90, 4, TRUE, NOW(), 3, 3),

(11, 'Mortal Kombat',
 'Jogo de luta conhecido por seus personagens e combates.',
 1993, 64.90, 3, FALSE, NOW(), 3, 2),


-- PLAYSTATION

(12, 'Final Fantasy VII',
 'RPG que acompanha Cloud Strife em sua jornada.',
 1997, 119.90, 5, TRUE, NOW(), 4, 2),

(13, 'Gran Turismo',
 'Jogo de corrida com diversos veículos e pistas.',
 1997, 89.90, 7, TRUE, NOW(), 4, 2),

(14, 'Tekken 3',
 'Jogo de luta com diversos personagens jogáveis.',
 1998, 99.90, 4, TRUE, NOW(), 4, 3),

(15, 'Crash Bandicoot',
 'Jogo de plataforma protagonizado por Crash.',
 1996, 84.90, 6, TRUE, NOW(), 4, 2),


-- ATARI 2600

(16, 'Pac-Man',
 'Jogo clássico em que o jogador percorre labirintos.',
 1982, 39.90, 5, TRUE, NOW(), 5, 2),

(17, 'Pitfall!',
 'Jogo de aventura e plataforma em ambientes selvagens.',
 1982, 44.90, 2, TRUE, NOW(), 5, 3),


-- GAME BOY

(18, 'Tetris',
 'Jogo de quebra-cabeça baseado no encaixe de peças.',
 1989, 49.90, 0, TRUE, NOW(), 6, 2),

(19, 'Pokémon Red Version',
 'RPG em que o jogador captura e treina criaturas.',
 1996, 109.90, 3, TRUE, NOW(), 6, 2),

(20, 'Super Mario Land',
 'Jogo de plataforma protagonizado por Mario.',
 1989, 59.90, 8, TRUE, NOW(), 6, 3);


-- ============================================================
-- 6. RELACIONAMENTO ENTRE JOGOS E GÊNEROS
-- Cada jogo possui entre 1 e 3 gêneros.
-- ============================================================

INSERT INTO tb_jogo_genero
    (co_jogo, co_genero)
VALUES

-- NES
(1, 1), (1, 3),
(2, 2), (2, 4),
(3, 1), (3, 2),
(4, 1), (4, 3),

-- SNES
(5, 1), (5, 3),
(6, 1), (6, 2),
(7, 2), (7, 4),
(8, 6),

-- MEGA DRIVE
(9, 1), (9, 3),
(10, 1), (10, 6),
(11, 1), (11, 6),

-- PLAYSTATION
(12, 2), (12, 4),
(13, 5), (13, 7),
(14, 6),
(15, 2), (15, 3),

-- ATARI 2600
(16, 1), (16, 8),
(17, 2), (17, 3),

-- GAME BOY
(18, 8),
(19, 2), (19, 4),
(20, 1), (20, 3);


-- ============================================================
-- 7. FINALIZAÇÃO
-- Nenhum usuário é inserido nesta etapa.
-- ============================================================

COMMIT;
