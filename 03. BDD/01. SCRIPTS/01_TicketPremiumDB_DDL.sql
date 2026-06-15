USE [master]
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'TicketPremiumDB')
BEGIN
    CREATE DATABASE [TicketPremiumDB];
END
GO

USE [TicketPremiumDB]
GO

-- ============================================================
-- 1. Paises
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Paises')
BEGIN
    CREATE TABLE [dbo].[Paises] (
        [Codigo]        NVARCHAR(10)    NOT NULL,
        [Nombre]        NVARCHAR(100)   NOT NULL,
        [Continente]    NVARCHAR(50)    NULL,
        CONSTRAINT [PK_Paises] PRIMARY KEY CLUSTERED ([Codigo])
    );
END
GO

-- ============================================================
-- 2. Estadios
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Estadios')
BEGIN
    CREATE TABLE [dbo].[Estadios] (
        [Codigo]            NVARCHAR(10)    NOT NULL,
        [Nombre]            NVARCHAR(100)   NOT NULL,
        [Ciudad]            NVARCHAR(100)   NULL,
        [CapacidadTotal]    INT             NOT NULL,
        [PaisCodigo]        NVARCHAR(10)    NOT NULL,
        CONSTRAINT [PK_Estadios] PRIMARY KEY CLUSTERED ([Codigo]),
        CONSTRAINT [FK_Estadios_Paises_PaisCodigo] FOREIGN KEY ([PaisCodigo])
            REFERENCES [dbo].[Paises] ([Codigo])
            ON DELETE CASCADE
    );
END
GO

-- ============================================================
-- 3. Localidades
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Localidades')
BEGIN
    CREATE TABLE [dbo].[Localidades] (
        [Codigo]            NVARCHAR(10)    NOT NULL,
        [Descripcion]       NVARCHAR(100)   NULL,
        [Capacidad]         INT             NOT NULL,
        [PrecioBase]        DECIMAL(18,2)   NOT NULL,
        [EstadioCodigo]     NVARCHAR(10)    NOT NULL,
        CONSTRAINT [PK_Localidades] PRIMARY KEY CLUSTERED ([Codigo]),
        CONSTRAINT [FK_Localidades_Estadios_EstadioCodigo] FOREIGN KEY ([EstadioCodigo])
            REFERENCES [dbo].[Estadios] ([Codigo])
            ON DELETE CASCADE
    );
END
GO

-- ============================================================
-- 4. Partidos
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Partidos')
BEGIN
    CREATE TABLE [dbo].[Partidos] (
        [Codigo]            NVARCHAR(10)    NOT NULL,
        [EquipoLocal]       NVARCHAR(100)   NOT NULL,
        [EquipoVisitante]   NVARCHAR(100)   NOT NULL,
        [FechaHora]         DATETIME2       NOT NULL,
        [EstadioCodigo]     NVARCHAR(10)    NOT NULL,
        CONSTRAINT [PK_Partidos] PRIMARY KEY CLUSTERED ([Codigo]),
        CONSTRAINT [FK_Partidos_Estadios_EstadioCodigo] FOREIGN KEY ([EstadioCodigo])
            REFERENCES [dbo].[Estadios] ([Codigo])
            ON DELETE CASCADE
    );
END
GO

-- ============================================================
-- 5. Clientes
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Clientes')
BEGIN
    CREATE TABLE [dbo].[Clientes] (
        [Cedula]            NVARCHAR(20)    NOT NULL,
        [Nombre]            NVARCHAR(100)   NOT NULL,
        [Apellido]          NVARCHAR(100)   NOT NULL,
        [FechaNacimiento]   DATETIME2       NOT NULL,
        [Genero]            NVARCHAR(20)    NULL,
        [Telefono]          NVARCHAR(20)    NULL,
        [Email]             NVARCHAR(200)   NULL,
        [PasswordHash]      NVARCHAR(256)   NOT NULL DEFAULT '',
        [Rol]               NVARCHAR(20)    NOT NULL DEFAULT 'CLIENTE',
        CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED ([Cedula])
    );
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clientes]') AND name = 'PasswordHash')
        ALTER TABLE [dbo].[Clientes] ADD [PasswordHash] NVARCHAR(256) NOT NULL DEFAULT '';
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clientes]') AND name = 'Rol')
        ALTER TABLE [dbo].[Clientes] ADD [Rol] NVARCHAR(20) NOT NULL DEFAULT 'CLIENTE';
END
GO

-- ============================================================
-- 6. Asientos
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Asientos')
BEGIN
    CREATE TABLE [dbo].[Asientos] (
        [Codigo]            NVARCHAR(20)    NOT NULL,
        [Fila]              NVARCHAR(5)     NULL,
        [Numero]            INT             NOT NULL,
        [Estado]            NVARCHAR(20)    NULL DEFAULT 'LIBRE',
        [TimestampReserva]  DATETIME2       NULL,
        [LocalidadCodigo]   NVARCHAR(10)    NOT NULL,
        [PartidoCodigo]     NVARCHAR(10)    NOT NULL,
        CONSTRAINT [PK_Asientos] PRIMARY KEY CLUSTERED ([Codigo]),
        CONSTRAINT [FK_Asientos_Localidades_LocalidadCodigo] FOREIGN KEY ([LocalidadCodigo])
            REFERENCES [dbo].[Localidades] ([Codigo])
            ON DELETE CASCADE,
        CONSTRAINT [FK_Asientos_Partidos_PartidoCodigo] FOREIGN KEY ([PartidoCodigo])
            REFERENCES [dbo].[Partidos] ([Codigo])
            ON DELETE NO ACTION
    );
END
GO

-- ============================================================
-- 7. Facturas
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Facturas')
BEGIN
    CREATE TABLE [dbo].[Facturas] (
        [Numero]            NVARCHAR(30)    NOT NULL,
        [Fecha]             DATETIME2       NOT NULL,
        [Subtotal]          DECIMAL(18,2)   NOT NULL,
        [Descuento]         DECIMAL(18,2)   NOT NULL,
        [Iva]               DECIMAL(18,2)   NOT NULL,
        [Total]             DECIMAL(18,2)   NOT NULL,
        [MetodoPago]        NVARCHAR(30)    NULL,
        [ClienteCedula]     NVARCHAR(20)    NOT NULL,
        CONSTRAINT [PK_Facturas] PRIMARY KEY CLUSTERED ([Numero]),
        CONSTRAINT [FK_Facturas_Clientes_ClienteCedula] FOREIGN KEY ([ClienteCedula])
            REFERENCES [dbo].[Clientes] ([Cedula])
            ON DELETE CASCADE
    );
END
GO

-- ============================================================
-- 8. DetallesFactura
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DetallesFactura')
BEGIN
    CREATE TABLE [dbo].[DetallesFactura] (
        [Id]                INT             IDENTITY(1,1) NOT NULL,
        [FacturaNumero]     NVARCHAR(30)    NOT NULL,
        [AsientoCodigo]     NVARCHAR(20)    NOT NULL,
        [PrecioUnitario]    DECIMAL(18,2)   NOT NULL,
        CONSTRAINT [PK_DetallesFactura] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_DetallesFactura_Facturas_FacturaNumero] FOREIGN KEY ([FacturaNumero])
            REFERENCES [dbo].[Facturas] ([Numero])
            ON DELETE CASCADE,
        CONSTRAINT [FK_DetallesFactura_Asientos_AsientoCodigo] FOREIGN KEY ([AsientoCodigo])
            REFERENCES [dbo].[Asientos] ([Codigo])
            ON DELETE CASCADE
    );
END
GO

PRINT 'DDL TicketPremiumDB ejecutado correctamente.';
GO
