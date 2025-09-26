IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926220225_InitalAzureSQLSchema'
)
BEGIN
    CREATE TABLE [PATIO] (
        [ID] bigint NOT NULL IDENTITY,
        [NOME] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_PATIO] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926220225_InitalAzureSQLSchema'
)
BEGIN
    CREATE TABLE [USUARIO] (
        [ID] bigint NOT NULL IDENTITY,
        [NOME] nvarchar(100) NOT NULL,
        [EMAIL] nvarchar(100) NOT NULL,
        [SENHA] nvarchar(255) NOT NULL,
        [TIPO] int NOT NULL,
        CONSTRAINT [PK_USUARIO] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926220225_InitalAzureSQLSchema'
)
BEGIN
    CREATE TABLE [VAGA] (
        [ID] bigint NOT NULL IDENTITY,
        [NUMERO] int NOT NULL,
        [TIPO] int NOT NULL,
        [STATUS] int NOT NULL,
        [PATIO_ID] bigint NOT NULL,
        CONSTRAINT [PK_VAGA] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_VAGA_PATIO_PATIO_ID] FOREIGN KEY ([PATIO_ID]) REFERENCES [PATIO] ([ID]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926220225_InitalAzureSQLSchema'
)
BEGIN
    CREATE TABLE [MOTO] (
        [ID] bigint NOT NULL IDENTITY,
        [MODELO] nvarchar(50) NOT NULL,
        [PLACA] nvarchar(10) NOT NULL,
        [CHASSI] nvarchar(50) NOT NULL,
        [PROBLEMA_IDENTIFICADO] nvarchar(255) NULL,
        [VAGA_ID] bigint NOT NULL,
        CONSTRAINT [PK_MOTO] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_MOTO_VAGA_VAGA_ID] FOREIGN KEY ([VAGA_ID]) REFERENCES [VAGA] ([ID]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926220225_InitalAzureSQLSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MOTO_CHASSI] ON [MOTO] ([CHASSI]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926220225_InitalAzureSQLSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MOTO_PLACA] ON [MOTO] ([PLACA]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926220225_InitalAzureSQLSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MOTO_VAGA_ID] ON [MOTO] ([VAGA_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926220225_InitalAzureSQLSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_USUARIO_EMAIL] ON [USUARIO] ([EMAIL]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926220225_InitalAzureSQLSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_VAGA_NUMERO_PATIO_ID] ON [VAGA] ([NUMERO], [PATIO_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926220225_InitalAzureSQLSchema'
)
BEGIN
    CREATE INDEX [IX_VAGA_PATIO_ID] ON [VAGA] ([PATIO_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926220225_InitalAzureSQLSchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250926220225_InitalAzureSQLSchema', N'8.0.13');
END;
GO

COMMIT;
GO

