USE [FifaDB]
GO

-- ============================================================
-- PartidosFutbol (3 registros)
-- ============================================================
INSERT INTO [dbo].[PartidosFutbol] ([Codigo], [EquipoLocal], [EquipoVisitante], [FechaHora], [EstadioCodigo]) VALUES
(N'P001', N'USA', N'MEX', '2026-06-11T14:00:00', N'Estadio Azteca'),
(N'P002', N'ARG', N'BRA', '2026-06-13T16:00:00', N'MetLife Stadium'),
(N'P003', N'FRA', N'ENG', '2026-06-15T18:00:00', N'SoFi Stadium');
GO

-- ============================================================
-- LocalidadesPartido (9 registros)
-- ============================================================
INSERT INTO [dbo].[LocalidadesPartido] ([Codigo], [Descripcion], [PartidoCodigo], [Capacidad], [PrecioBase]) VALUES
(N'L001', N'PALCO',    N'P001', 10, 200.00),
(N'L002', N'TRIBUNA',  N'P001', 12, 100.00),
(N'L003', N'GENERAL',  N'P001', 15,  50.00),
(N'L004', N'PALCO',    N'P002', 10, 250.00),
(N'L005', N'TRIBUNA',  N'P002', 12, 120.00),
(N'L006', N'GENERAL',  N'P002', 15,  60.00),
(N'L007', N'PALCO',    N'P003', 10, 300.00),
(N'L008', N'TRIBUNA',  N'P003', 12, 150.00),
(N'L009', N'GENERAL',  N'P003', 15,  75.00);
GO

-- ============================================================
-- Asientos (109 registros)
-- Generados según: AS-{localidad}-{i:D2}
-- Filas: A (i 1-5), B (i 6-10), C (i 11-15)
-- ============================================================

-- L001 (10 asientos)
INSERT INTO [dbo].[Asientos] ([CodigoAsiento], [Fila], [Numero], [Estado], [LocalidadPartidoCodigo]) VALUES
(N'AS-L001-01', N'A', 1,  N'LIBRE', N'L001'),
(N'AS-L001-02', N'A', 2,  N'LIBRE', N'L001'),
(N'AS-L001-03', N'A', 3,  N'LIBRE', N'L001'),
(N'AS-L001-04', N'A', 4,  N'LIBRE', N'L001'),
(N'AS-L001-05', N'A', 5,  N'LIBRE', N'L001'),
(N'AS-L001-06', N'B', 6,  N'LIBRE', N'L001'),
(N'AS-L001-07', N'B', 7,  N'LIBRE', N'L001'),
(N'AS-L001-08', N'B', 8,  N'LIBRE', N'L001'),
(N'AS-L001-09', N'B', 9,  N'LIBRE', N'L001'),
(N'AS-L001-10', N'B', 10, N'LIBRE', N'L001');
GO

-- L002 (12 asientos)
INSERT INTO [dbo].[Asientos] ([CodigoAsiento], [Fila], [Numero], [Estado], [LocalidadPartidoCodigo]) VALUES
(N'AS-L002-01', N'A', 1,  N'LIBRE', N'L002'),
(N'AS-L002-02', N'A', 2,  N'LIBRE', N'L002'),
(N'AS-L002-03', N'A', 3,  N'LIBRE', N'L002'),
(N'AS-L002-04', N'A', 4,  N'LIBRE', N'L002'),
(N'AS-L002-05', N'A', 5,  N'LIBRE', N'L002'),
(N'AS-L002-06', N'B', 6,  N'LIBRE', N'L002'),
(N'AS-L002-07', N'B', 7,  N'LIBRE', N'L002'),
(N'AS-L002-08', N'B', 8,  N'LIBRE', N'L002'),
(N'AS-L002-09', N'B', 9,  N'LIBRE', N'L002'),
(N'AS-L002-10', N'B', 10, N'LIBRE', N'L002'),
(N'AS-L002-11', N'C', 11, N'LIBRE', N'L002'),
(N'AS-L002-12', N'C', 12, N'LIBRE', N'L002');
GO

-- L003 (15 asientos)
INSERT INTO [dbo].[Asientos] ([CodigoAsiento], [Fila], [Numero], [Estado], [LocalidadPartidoCodigo]) VALUES
(N'AS-L003-01', N'A', 1,  N'LIBRE', N'L003'),
(N'AS-L003-02', N'A', 2,  N'LIBRE', N'L003'),
(N'AS-L003-03', N'A', 3,  N'LIBRE', N'L003'),
(N'AS-L003-04', N'A', 4,  N'LIBRE', N'L003'),
(N'AS-L003-05', N'A', 5,  N'LIBRE', N'L003'),
(N'AS-L003-06', N'B', 6,  N'LIBRE', N'L003'),
(N'AS-L003-07', N'B', 7,  N'LIBRE', N'L003'),
(N'AS-L003-08', N'B', 8,  N'LIBRE', N'L003'),
(N'AS-L003-09', N'B', 9,  N'LIBRE', N'L003'),
(N'AS-L003-10', N'B', 10, N'LIBRE', N'L003'),
(N'AS-L003-11', N'C', 11, N'LIBRE', N'L003'),
(N'AS-L003-12', N'C', 12, N'LIBRE', N'L003'),
(N'AS-L003-13', N'C', 13, N'LIBRE', N'L003'),
(N'AS-L003-14', N'C', 14, N'LIBRE', N'L003'),
(N'AS-L003-15', N'C', 15, N'LIBRE', N'L003');
GO

-- L004 (10 asientos)
INSERT INTO [dbo].[Asientos] ([CodigoAsiento], [Fila], [Numero], [Estado], [LocalidadPartidoCodigo]) VALUES
(N'AS-L004-01', N'A', 1,  N'LIBRE', N'L004'),
(N'AS-L004-02', N'A', 2,  N'LIBRE', N'L004'),
(N'AS-L004-03', N'A', 3,  N'LIBRE', N'L004'),
(N'AS-L004-04', N'A', 4,  N'LIBRE', N'L004'),
(N'AS-L004-05', N'A', 5,  N'LIBRE', N'L004'),
(N'AS-L004-06', N'B', 6,  N'LIBRE', N'L004'),
(N'AS-L004-07', N'B', 7,  N'LIBRE', N'L004'),
(N'AS-L004-08', N'B', 8,  N'LIBRE', N'L004'),
(N'AS-L004-09', N'B', 9,  N'LIBRE', N'L004'),
(N'AS-L004-10', N'B', 10, N'LIBRE', N'L004');
GO

-- L005 (12 asientos)
INSERT INTO [dbo].[Asientos] ([CodigoAsiento], [Fila], [Numero], [Estado], [LocalidadPartidoCodigo]) VALUES
(N'AS-L005-01', N'A', 1,  N'LIBRE', N'L005'),
(N'AS-L005-02', N'A', 2,  N'LIBRE', N'L005'),
(N'AS-L005-03', N'A', 3,  N'LIBRE', N'L005'),
(N'AS-L005-04', N'A', 4,  N'LIBRE', N'L005'),
(N'AS-L005-05', N'A', 5,  N'LIBRE', N'L005'),
(N'AS-L005-06', N'B', 6,  N'LIBRE', N'L005'),
(N'AS-L005-07', N'B', 7,  N'LIBRE', N'L005'),
(N'AS-L005-08', N'B', 8,  N'LIBRE', N'L005'),
(N'AS-L005-09', N'B', 9,  N'LIBRE', N'L005'),
(N'AS-L005-10', N'B', 10, N'LIBRE', N'L005'),
(N'AS-L005-11', N'C', 11, N'LIBRE', N'L005'),
(N'AS-L005-12', N'C', 12, N'LIBRE', N'L005');
GO

-- L006 (15 asientos)
INSERT INTO [dbo].[Asientos] ([CodigoAsiento], [Fila], [Numero], [Estado], [LocalidadPartidoCodigo]) VALUES
(N'AS-L006-01', N'A', 1,  N'LIBRE', N'L006'),
(N'AS-L006-02', N'A', 2,  N'LIBRE', N'L006'),
(N'AS-L006-03', N'A', 3,  N'LIBRE', N'L006'),
(N'AS-L006-04', N'A', 4,  N'LIBRE', N'L006'),
(N'AS-L006-05', N'A', 5,  N'LIBRE', N'L006'),
(N'AS-L006-06', N'B', 6,  N'LIBRE', N'L006'),
(N'AS-L006-07', N'B', 7,  N'LIBRE', N'L006'),
(N'AS-L006-08', N'B', 8,  N'LIBRE', N'L006'),
(N'AS-L006-09', N'B', 9,  N'LIBRE', N'L006'),
(N'AS-L006-10', N'B', 10, N'LIBRE', N'L006'),
(N'AS-L006-11', N'C', 11, N'LIBRE', N'L006'),
(N'AS-L006-12', N'C', 12, N'LIBRE', N'L006'),
(N'AS-L006-13', N'C', 13, N'LIBRE', N'L006'),
(N'AS-L006-14', N'C', 14, N'LIBRE', N'L006'),
(N'AS-L006-15', N'C', 15, N'LIBRE', N'L006');
GO

-- L007 (10 asientos)
INSERT INTO [dbo].[Asientos] ([CodigoAsiento], [Fila], [Numero], [Estado], [LocalidadPartidoCodigo]) VALUES
(N'AS-L007-01', N'A', 1,  N'LIBRE', N'L007'),
(N'AS-L007-02', N'A', 2,  N'LIBRE', N'L007'),
(N'AS-L007-03', N'A', 3,  N'LIBRE', N'L007'),
(N'AS-L007-04', N'A', 4,  N'LIBRE', N'L007'),
(N'AS-L007-05', N'A', 5,  N'LIBRE', N'L007'),
(N'AS-L007-06', N'B', 6,  N'LIBRE', N'L007'),
(N'AS-L007-07', N'B', 7,  N'LIBRE', N'L007'),
(N'AS-L007-08', N'B', 8,  N'LIBRE', N'L007'),
(N'AS-L007-09', N'B', 9,  N'LIBRE', N'L007'),
(N'AS-L007-10', N'B', 10, N'LIBRE', N'L007');
GO

-- L008 (12 asientos)
INSERT INTO [dbo].[Asientos] ([CodigoAsiento], [Fila], [Numero], [Estado], [LocalidadPartidoCodigo]) VALUES
(N'AS-L008-01', N'A', 1,  N'LIBRE', N'L008'),
(N'AS-L008-02', N'A', 2,  N'LIBRE', N'L008'),
(N'AS-L008-03', N'A', 3,  N'LIBRE', N'L008'),
(N'AS-L008-04', N'A', 4,  N'LIBRE', N'L008'),
(N'AS-L008-05', N'A', 5,  N'LIBRE', N'L008'),
(N'AS-L008-06', N'B', 6,  N'LIBRE', N'L008'),
(N'AS-L008-07', N'B', 7,  N'LIBRE', N'L008'),
(N'AS-L008-08', N'B', 8,  N'LIBRE', N'L008'),
(N'AS-L008-09', N'B', 9,  N'LIBRE', N'L008'),
(N'AS-L008-10', N'B', 10, N'LIBRE', N'L008'),
(N'AS-L008-11', N'C', 11, N'LIBRE', N'L008'),
(N'AS-L008-12', N'C', 12, N'LIBRE', N'L008');
GO

-- L009 (15 asientos)
INSERT INTO [dbo].[Asientos] ([CodigoAsiento], [Fila], [Numero], [Estado], [LocalidadPartidoCodigo]) VALUES
(N'AS-L009-01', N'A', 1,  N'LIBRE', N'L009'),
(N'AS-L009-02', N'A', 2,  N'LIBRE', N'L009'),
(N'AS-L009-03', N'A', 3,  N'LIBRE', N'L009'),
(N'AS-L009-04', N'A', 4,  N'LIBRE', N'L009'),
(N'AS-L009-05', N'A', 5,  N'LIBRE', N'L009'),
(N'AS-L009-06', N'B', 6,  N'LIBRE', N'L009'),
(N'AS-L009-07', N'B', 7,  N'LIBRE', N'L009'),
(N'AS-L009-08', N'B', 8,  N'LIBRE', N'L009'),
(N'AS-L009-09', N'B', 9,  N'LIBRE', N'L009'),
(N'AS-L009-10', N'B', 10, N'LIBRE', N'L009'),
(N'AS-L009-11', N'C', 11, N'LIBRE', N'L009'),
(N'AS-L009-12', N'C', 12, N'LIBRE', N'L009'),
(N'AS-L009-13', N'C', 13, N'LIBRE', N'L009'),
(N'AS-L009-14', N'C', 14, N'LIBRE', N'L009'),
(N'AS-L009-15', N'C', 15, N'LIBRE', N'L009');
GO

PRINT 'DML FifaDB ejecutado correctamente.';
GO
