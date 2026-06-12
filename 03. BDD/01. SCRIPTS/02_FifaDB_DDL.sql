USE [master]
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'FifaDB')
BEGIN
    CREATE DATABASE [FifaDB];
END
GO

USE [FifaDB]
GO

-- ============================================================
-- 1. PartidosFutbol
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PartidosFutbol')
BEGIN
    CREATE TABLE [dbo].[PartidosFutbol] (
        [Codigo]            NVARCHAR(50)    NOT NULL,
        [EquipoLocal]       NVARCHAR(100)   NOT NULL,
        [EquipoVisitante]   NVARCHAR(100)   NOT NULL,
        [FechaHora]         DATETIME2       NOT NULL,
        [EstadioCodigo]     NVARCHAR(50)    NOT NULL,
        CONSTRAINT [PK_PartidosFutbol] PRIMARY KEY CLUSTERED ([Codigo])
    );
END
GO

-- ============================================================
-- 2. LocalidadesPartido
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LocalidadesPartido')
BEGIN
    CREATE TABLE [dbo].[LocalidadesPartido] (
        [Codigo]            NVARCHAR(50)    NOT NULL,
        [Descripcion]       NVARCHAR(100)   NOT NULL,
        [PartidoCodigo]     NVARCHAR(50)    NOT NULL,
        [Capacidad]         INT             NOT NULL,
        [PrecioBase]        DECIMAL(18,2)   NOT NULL,
        CONSTRAINT [PK_LocalidadesPartido] PRIMARY KEY CLUSTERED ([Codigo]),
        CONSTRAINT [FK_LocalidadesPartido_PartidosFutbol_PartidoCodigo] FOREIGN KEY ([PartidoCodigo])
            REFERENCES [dbo].[PartidosFutbol] ([Codigo])
            ON DELETE CASCADE
    );
END
GO

-- ============================================================
-- 3. Asientos
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Asientos')
BEGIN
    CREATE TABLE [dbo].[Asientos] (
        [CodigoAsiento]             NVARCHAR(50)    NOT NULL,
        [Fila]                      NVARCHAR(10)    NOT NULL,
        [Numero]                    INT             NOT NULL,
        [Estado]                    NVARCHAR(20)    NOT NULL DEFAULT 'LIBRE',
        [TimestampReserva]          DATETIME2       NULL,
        [LocalidadPartidoCodigo]    NVARCHAR(50)    NOT NULL,
        CONSTRAINT [PK_Asientos] PRIMARY KEY CLUSTERED ([CodigoAsiento]),
        CONSTRAINT [FK_Asientos_LocalidadesPartido_LocalidadPartidoCodigo] FOREIGN KEY ([LocalidadPartidoCodigo])
            REFERENCES [dbo].[LocalidadesPartido] ([Codigo])
            ON DELETE CASCADE
    );
END
GO

PRINT 'DDL FifaDB ejecutado correctamente.';
GO
