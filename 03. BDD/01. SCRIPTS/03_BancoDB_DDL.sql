USE [master]
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'BancoDB')
BEGIN
    CREATE DATABASE [BancoDB];
END
GO

USE [BancoDB]
GO

-- ============================================================
-- 1. Clientes
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Clientes')
BEGIN
    CREATE TABLE [dbo].[Clientes] (
        [Cedula]            NVARCHAR(50)    NOT NULL,
        [Nombre]            NVARCHAR(200)   NOT NULL,
        [Apellido]          NVARCHAR(200)   NOT NULL,
        [FechaNacimiento]   DATETIME2       NOT NULL,
        [Genero]            NVARCHAR(50)    NOT NULL,
        [Estado]            NVARCHAR(50)    NOT NULL,
        CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED ([Cedula])
    );
END
GO

-- ============================================================
-- 2. Cuentas
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Cuentas')
BEGIN
    CREATE TABLE [dbo].[Cuentas] (
        [Numero]            NVARCHAR(50)    NOT NULL,
        [ClienteCedula]     NVARCHAR(50)    NOT NULL,
        [Tipo]              NVARCHAR(50)    NOT NULL,
        [Saldo]             DECIMAL(18,2)   NOT NULL,
        CONSTRAINT [PK_Cuentas] PRIMARY KEY CLUSTERED ([Numero]),
        CONSTRAINT [FK_Cuentas_Clientes_ClienteCedula] FOREIGN KEY ([ClienteCedula])
            REFERENCES [dbo].[Clientes] ([Cedula])
            ON DELETE CASCADE
    );
END
GO

-- ============================================================
-- 3. Movimientos
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Movimientos')
BEGIN
    CREATE TABLE [dbo].[Movimientos] (
        [Codigo]            INT             IDENTITY(1,1) NOT NULL,
        [CuentaNumero]      NVARCHAR(50)    NOT NULL,
        [Tipo]              NVARCHAR(50)    NOT NULL,
        [Monto]             DECIMAL(18,2)   NOT NULL,
        [Fecha]             DATETIME2       NOT NULL,
        CONSTRAINT [PK_Movimientos] PRIMARY KEY CLUSTERED ([Codigo]),
        CONSTRAINT [FK_Movimientos_Cuentas_CuentaNumero] FOREIGN KEY ([CuentaNumero])
            REFERENCES [dbo].[Cuentas] ([Numero])
            ON DELETE CASCADE
    );
END
GO

-- ============================================================
-- 4. Creditos
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Creditos')
BEGIN
    CREATE TABLE [dbo].[Creditos] (
        [Codigo]            INT             IDENTITY(1,1) NOT NULL,
        [ClienteCedula]     NVARCHAR(50)    NOT NULL,
        [Monto]             DECIMAL(18,2)   NOT NULL,
        [PlazoMeses]        INT             NOT NULL,
        [TasaAnual]         DECIMAL(18,2)   NOT NULL,
        [FechaAprobacion]   DATETIME2       NOT NULL,
        [Estado]            NVARCHAR(50)    NOT NULL,
        CONSTRAINT [PK_Creditos] PRIMARY KEY CLUSTERED ([Codigo]),
        CONSTRAINT [FK_Creditos_Clientes_ClienteCedula] FOREIGN KEY ([ClienteCedula])
            REFERENCES [dbo].[Clientes] ([Cedula])
            ON DELETE CASCADE
    );
END
GO

-- ============================================================
-- 5. Amortizaciones
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Amortizaciones')
BEGIN
    CREATE TABLE [dbo].[Amortizaciones] (
        [Codigo]            INT             IDENTITY(1,1) NOT NULL,
        [CreditoCodigo]     INT             NOT NULL,
        [NumeroCuota]       INT             NOT NULL,
        [ValorCuota]        DECIMAL(18,2)   NOT NULL,
        [FechaPago]         DATETIME2       NOT NULL,
        CONSTRAINT [PK_Amortizaciones] PRIMARY KEY CLUSTERED ([Codigo]),
        CONSTRAINT [FK_Amortizaciones_Creditos_CreditoCodigo] FOREIGN KEY ([CreditoCodigo])
            REFERENCES [dbo].[Creditos] ([Codigo])
            ON DELETE CASCADE
    );
END
GO

PRINT 'DDL BancoDB ejecutado correctamente.';
GO
