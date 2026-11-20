
-- ============================================
-- ACADEMIA DO ZÉ
-- Inicialização do banco de dados - SQL Server
-- ============================================

-- ============================================
-- TABELA: LOGRADOURO
-- ============================================

IF OBJECT_ID(N'dbo.tb_logradouro', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tb_logradouro
    (
        id_logradouro INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        cep VARCHAR(8) NOT NULL,
        nome VARCHAR(150) NOT NULL,
        bairro VARCHAR(150) NOT NULL,
        cidade VARCHAR(150) NOT NULL,
        estado VARCHAR(2) NOT NULL,
        pais VARCHAR(100) NOT NULL
    );
END;

-- ============================================
-- TABELA: COLABORADOR
-- ============================================

IF OBJECT_ID(N'dbo.tb_colaborador', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tb_colaborador
    (
        id_colaborador INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        cpf VARCHAR(11) NOT NULL UNIQUE,
        nome VARCHAR(150) NOT NULL,
        nascimento DATE NOT NULL,
        telefone VARCHAR(11) NOT NULL,
        email VARCHAR(254) NOT NULL UNIQUE,
        logradouro_id INT NOT NULL
            REFERENCES dbo.tb_logradouro(id_logradouro),
        numero VARCHAR(20) NOT NULL,
        complemento VARCHAR(150) NULL,
        senha VARCHAR(255) NOT NULL,
        foto VARBINARY(MAX) NULL,
        admissao DATE NOT NULL,
        tipo INT NOT NULL,
        vinculo INT NOT NULL
    );
END;

-- ============================================
-- TABELA: ALUNO
-- ============================================

IF OBJECT_ID(N'dbo.tb_aluno', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tb_aluno
    (
        id_aluno INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        cpf VARCHAR(11) NOT NULL UNIQUE,
        nome VARCHAR(150) NOT NULL,
        nascimento DATE NOT NULL,
        telefone VARCHAR(11) NOT NULL,
        email VARCHAR(254) NOT NULL UNIQUE,
        logradouro_id INT NOT NULL
            REFERENCES dbo.tb_logradouro(id_logradouro),
        numero VARCHAR(20) NOT NULL,
        complemento VARCHAR(150) NULL,
        senha VARCHAR(255) NOT NULL,
        foto VARBINARY(MAX) NULL
    );
END;

-- ============================================
-- TABELA: REGISTRO DE ACESSO
-- ============================================

IF OBJECT_ID(N'dbo.tb_registro_acesso', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tb_registro_acesso
    (
        id_registro_acesso INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        pessoa_id INT NOT NULL,
        tipo_pessoa INT NOT NULL,
        entrada DATETIME2 NOT NULL,
        saida DATETIME2 NULL,

        CONSTRAINT CK_registro_tipo
            CHECK (tipo_pessoa IN (0, 1)),

        CONSTRAINT CK_registro_horario
            CHECK (saida IS NULL OR saida >= entrada)
    );
END;

-- ============================================
-- ÍNDICES: REGISTRO DE ACESSO
-- ============================================

IF OBJECT_ID(N'dbo.tb_registro_acesso', N'U') IS NOT NULL
BEGIN

    -- Impede duas entradas abertas para a mesma pessoa
    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.indexes
        WHERE name = N'UX_registro_entrada_aberta'
          AND object_id = OBJECT_ID(N'dbo.tb_registro_acesso')
    )
    BEGIN
        CREATE UNIQUE INDEX UX_registro_entrada_aberta
        ON dbo.tb_registro_acesso (pessoa_id, tipo_pessoa)
        WHERE saida IS NULL;
    END;

    -- Melhora consultas do histórico
    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.indexes
        WHERE name = N'IX_registro_data'
          AND object_id = OBJECT_ID(N'dbo.tb_registro_acesso')
    )
    BEGIN
        CREATE INDEX IX_registro_data
        ON dbo.tb_registro_acesso (entrada DESC);
    END;

END;
