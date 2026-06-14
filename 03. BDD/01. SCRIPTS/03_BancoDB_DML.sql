USE [BancoDB]
GO

-- ============================================================
-- Clientes
-- ============================================================

INSERT INTO [dbo].[Clientes]
([Cedula], [Nombre], [Apellido], [FechaNacimiento], [Genero], [Estado])
VALUES
(N'1712345678', N'Juan',   N'Perez',   '1981-03-15', N'MASCULINO', N'ACTIVO'),
(N'1712345679', N'Maria',  N'Gomez',   '1996-05-20', N'FEMENINO',  N'ACTIVO'),
(N'1712345680', N'Pedro',  N'Ramirez', '2004-01-10', N'MASCULINO', N'ACTIVO'),
(N'1712345681', N'Ana',    N'Lopez',   '1998-07-22', N'FEMENINO',  N'INACTIVO'),
(N'1712345682', N'Carlos', N'Ruiz',    '1991-08-05', N'MASCULINO', N'ACTIVO');
GO

-- ============================================================
-- Obtener IDs generados
-- ============================================================

DECLARE @JuanId UNIQUEIDENTIFIER;
DECLARE @MariaId UNIQUEIDENTIFIER;
DECLARE @PedroId UNIQUEIDENTIFIER;
DECLARE @AnaId UNIQUEIDENTIFIER;
DECLARE @CarlosId UNIQUEIDENTIFIER;

SELECT @JuanId   = Id FROM Clientes WHERE Cedula = '1712345678';
SELECT @MariaId  = Id FROM Clientes WHERE Cedula = '1712345679';
SELECT @PedroId  = Id FROM Clientes WHERE Cedula = '1712345680';
SELECT @AnaId    = Id FROM Clientes WHERE Cedula = '1712345681';
SELECT @CarlosId = Id FROM Clientes WHERE Cedula = '1712345682';

-- ============================================================
-- Cuentas
-- ============================================================

INSERT INTO [dbo].[Cuentas]
([Numero], [ClienteId], [Tipo], [Saldo])
VALUES
(N'1001-001', @JuanId,   N'AHORROS',   2800.00),
(N'1002-001', @MariaId,  N'CORRIENTE', 4500.00),
(N'1003-001', @PedroId,  N'AHORROS',    600.00),
(N'1004-001', @AnaId,    N'AHORROS',   1200.00),
(N'1005-001', @CarlosId, N'CORRIENTE', 3500.00);
GO

-- ============================================================
-- Movimientos
-- ============================================================

SET IDENTITY_INSERT [dbo].[Movimientos] ON;

INSERT INTO [dbo].[Movimientos]
([Codigo], [CuentaNumero], [Tipo], [Monto], [Fecha])
VALUES
(1,  N'1001-001', N'DEPOSITO', 1000.00, '2026-06-05'),
(2,  N'1001-001', N'DEPOSITO', 2000.00, '2026-04-21'),
(3,  N'1001-001', N'DEPOSITO', 1500.00, '2026-02-10'),
(4,  N'1001-001', N'RETIRO',    500.00, '2026-05-31'),
(5,  N'1001-001', N'RETIRO',    300.00, '2026-03-07'),
(6,  N'1002-001', N'DEPOSITO', 1200.00, '2026-06-07'),
(7,  N'1002-001', N'DEPOSITO', 2500.00, '2026-04-26'),
(8,  N'1002-001', N'DEPOSITO', 1800.00, '2026-01-11'),
(9,  N'1002-001', N'RETIRO',    400.00, '2026-06-02'),
(10, N'1002-001', N'RETIRO',    600.00, '2026-04-11'),
(11, N'1003-001', N'DEPOSITO',  800.00, '2026-06-03'),
(12, N'1003-001', N'RETIRO',    200.00, '2026-05-21'),
(13, N'1004-001', N'DEPOSITO',  600.00, '2026-05-26'),
(14, N'1004-001', N'RETIRO',    300.00, '2026-05-01'),
(15, N'1005-001', N'DEPOSITO', 1500.00, '2026-06-08'),
(16, N'1005-001', N'DEPOSITO', 3000.00, '2026-04-16'),
(17, N'1005-001', N'RETIRO',    700.00, '2026-05-29');

SET IDENTITY_INSERT [dbo].[Movimientos] OFF;
GO

-- ============================================================
-- Creditos
-- ============================================================

DECLARE @CarlosCreditoId UNIQUEIDENTIFIER;

SELECT @CarlosCreditoId = Id
FROM Clientes
WHERE Cedula = '1712345682';

SET IDENTITY_INSERT [dbo].[Creditos] ON;

INSERT INTO [dbo].[Creditos]
([Codigo], [ClienteId], [Monto], [PlazoMeses], [TasaAnual], [FechaAprobacion], [Estado])
VALUES
(1, @CarlosCreditoId, 10000.00, 12, 16.50, '2026-03-10', N'ACTIVO');

SET IDENTITY_INSERT [dbo].[Creditos] OFF;
GO

-- ============================================================
-- Amortizaciones
-- ============================================================

SET IDENTITY_INSERT [dbo].[Amortizaciones] ON;

INSERT INTO [dbo].[Amortizaciones]
([Codigo], [CreditoCodigo], [NumeroCuota], [ValorCuota], [FechaPago])
VALUES
(1, 1, 1, 912.85, '2026-04-10'),
(2, 1, 2, 912.85, '2026-05-10'),
(3, 1, 3, 912.85, '2026-06-10'),
(4, 1, 4, 912.85, '2026-07-10');

SET IDENTITY_INSERT [dbo].[Amortizaciones] OFF;
GO

PRINT 'DML BancoDB ejecutado correctamente.';
GO
