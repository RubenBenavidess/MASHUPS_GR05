USE [BancoDB]
GO

-- ============================================================
-- Clientes (5 registros)
-- ============================================================
INSERT INTO [dbo].[Clientes] ([Cedula], [Nombre], [Apellido], [FechaNacimiento], [Genero], [Estado]) VALUES
(N'1712345678', N'Juan',  N'Perez',   '1981-03-15', N'MASCULINO', N'ACTIVO'),
(N'1712345679', N'Maria', N'Gomez',   '1996-05-20', N'FEMENINO',  N'ACTIVO'),
(N'1712345680', N'Pedro', N'Ramirez', '2004-01-10', N'MASCULINO', N'ACTIVO'),
(N'1712345681', N'Ana',   N'Lopez',   '1998-07-22', N'FEMENINO',  N'INACTIVO'),
(N'1712345682', N'Carlos', N'Ruiz',   '1991-08-05', N'MASCULINO', N'ACTIVO');
GO

-- ============================================================
-- Cuentas (5 registros)
-- ============================================================
INSERT INTO [dbo].[Cuentas] ([Numero], [ClienteCedula], [Tipo], [Saldo]) VALUES
(N'1001-001', N'1712345678', N'AHORROS',   2800.00),
(N'1002-001', N'1712345679', N'CORRIENTE', 4500.00),
(N'1003-001', N'1712345680', N'AHORROS',    600.00),
(N'1004-001', N'1712345681', N'AHORROS',   1200.00),
(N'1005-001', N'1712345682', N'CORRIENTE', 3500.00);
GO

-- ============================================================
-- Movimientos (17 registros)
-- Fechas relativas a 2026-06-10
-- ============================================================

SET IDENTITY_INSERT [dbo].[Movimientos] ON;

-- Juan (1712345678) - cuenta 1001-001 (5 movimientos)
INSERT INTO [dbo].[Movimientos] ([Codigo], [CuentaNumero], [Tipo], [Monto], [Fecha]) VALUES
(1,  N'1001-001', N'DEPOSITO', 1000.00, '2026-06-05'),
(2,  N'1001-001', N'DEPOSITO', 2000.00, '2026-04-21'),
(3,  N'1001-001', N'DEPOSITO', 1500.00, '2026-02-10'),
(4,  N'1001-001', N'RETIRO',    500.00, '2026-05-31'),
(5,  N'1001-001', N'RETIRO',    300.00, '2026-03-07');

-- Maria (1712345679) - cuenta 1002-001 (5 movimientos)
INSERT INTO [dbo].[Movimientos] ([Codigo], [CuentaNumero], [Tipo], [Monto], [Fecha]) VALUES
(6,  N'1002-001', N'DEPOSITO', 1200.00, '2026-06-07'),
(7,  N'1002-001', N'DEPOSITO', 2500.00, '2026-04-26'),
(8,  N'1002-001', N'DEPOSITO', 1800.00, '2026-01-11'),
(9,  N'1002-001', N'RETIRO',    400.00, '2026-06-02'),
(10, N'1002-001', N'RETIRO',    600.00, '2026-04-11');

-- Pedro (1712345680) - cuenta 1003-001 (2 movimientos)
INSERT INTO [dbo].[Movimientos] ([Codigo], [CuentaNumero], [Tipo], [Monto], [Fecha]) VALUES
(11, N'1003-001', N'DEPOSITO', 800.00, '2026-06-03'),
(12, N'1003-001', N'RETIRO',   200.00, '2026-05-21');

-- Ana (1712345681) - cuenta 1004-001 (2 movimientos)
INSERT INTO [dbo].[Movimientos] ([Codigo], [CuentaNumero], [Tipo], [Monto], [Fecha]) VALUES
(13, N'1004-001', N'DEPOSITO', 600.00, '2026-05-26'),
(14, N'1004-001', N'RETIRO',   300.00, '2026-05-01');

-- Carlos (1712345682) - cuenta 1005-001 (3 movimientos)
INSERT INTO [dbo].[Movimientos] ([Codigo], [CuentaNumero], [Tipo], [Monto], [Fecha]) VALUES
(15, N'1005-001', N'DEPOSITO', 1500.00, '2026-06-08'),
(16, N'1005-001', N'DEPOSITO', 3000.00, '2026-04-16'),
(17, N'1005-001', N'RETIRO',    700.00, '2026-05-29');
SET IDENTITY_INSERT [dbo].[Movimientos] OFF;
GO

-- ============================================================
-- Creditos (1 registro) - Carlos tiene crédito activo
-- ============================================================
SET IDENTITY_INSERT [dbo].[Creditos] ON;
INSERT INTO [dbo].[Creditos] ([Codigo], [ClienteCedula], [Monto], [PlazoMeses], [TasaAnual], [FechaAprobacion], [Estado]) VALUES
(1, N'1712345682', 10000.00, 12, 16.50, '2026-03-10', N'ACTIVO');
SET IDENTITY_INSERT [dbo].[Creditos] OFF;
GO

-- ============================================================
-- Amortizaciones (4 registros) - cuotas del crédito de Carlos
-- ============================================================
SET IDENTITY_INSERT [dbo].[Amortizaciones] ON;
INSERT INTO [dbo].[Amortizaciones] ([Codigo], [CreditoCodigo], [NumeroCuota], [ValorCuota], [FechaPago]) VALUES
(1, 1, 1, 912.85, '2026-04-10'),
(2, 1, 2, 912.85, '2026-05-10'),
(3, 1, 3, 912.85, '2026-06-10'),
(4, 1, 4, 912.85, '2026-07-10');
SET IDENTITY_INSERT [dbo].[Amortizaciones] OFF;
GO

PRINT 'DML BancoDB ejecutado correctamente.';
GO
