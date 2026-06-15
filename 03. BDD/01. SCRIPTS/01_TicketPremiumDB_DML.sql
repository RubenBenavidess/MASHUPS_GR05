USE [TicketPremiumDB]
GO

-- ============================================================
-- Paises (3 registros)
-- ============================================================
INSERT INTO [dbo].[Paises] ([Codigo], [Nombre], [Continente]) VALUES
(N'USA', N'Estados Unidos', N'América'),
(N'MEX', N'México', N'América'),
(N'CAN', N'Canadá', N'América');
GO

-- ============================================================
-- Estadios (3 registros)
-- ============================================================
INSERT INTO [dbo].[Estadios] ([Codigo], [Nombre], [Ciudad], [CapacidadTotal], [PaisCodigo]) VALUES
(N'EST-001', N'Estadio Azteca', N'Ciudad de México', 87523, N'MEX'),
(N'EST-002', N'MetLife Stadium', N'East Rutherford', 82500, N'USA'),
(N'EST-003', N'SoFi Stadium', N'Los Ángeles', 70240, N'USA');
GO

-- ============================================================
-- Localidades (9 registros)
-- ============================================================
INSERT INTO [dbo].[Localidades] ([Codigo], [Descripcion], [Capacidad], [PrecioBase], [EstadioCodigo]) VALUES
(N'LOC-A1', N'PALCO',   20, 500.00, N'EST-001'),
(N'LOC-A2', N'TRIBUNA', 30, 250.00, N'EST-001'),
(N'LOC-A3', N'GENERAL', 50, 100.00, N'EST-001'),
(N'LOC-B1', N'PALCO',   20, 500.00, N'EST-002'),
(N'LOC-B2', N'TRIBUNA', 30, 250.00, N'EST-002'),
(N'LOC-B3', N'GENERAL', 50, 100.00, N'EST-002'),
(N'LOC-C1', N'PALCO',   20, 500.00, N'EST-003'),
(N'LOC-C2', N'TRIBUNA', 30, 250.00, N'EST-003'),
(N'LOC-C3', N'GENERAL', 50, 100.00, N'EST-003');
GO

-- ============================================================
-- Partidos (3 registros)
-- ============================================================
INSERT INTO [dbo].[Partidos] ([Codigo], [EquipoLocal], [EquipoVisitante], [FechaHora], [EstadioCodigo]) VALUES
(N'PAR-001', N'México', N'Estados Unidos', '2026-06-11T20:00:00', N'EST-001'),
(N'PAR-002', N'Argentina', N'Brasil', '2026-06-18T20:00:00', N'EST-002'),
(N'PAR-003', N'Francia', N'Inglaterra', '2026-07-05T20:00:00', N'EST-003');
GO

-- ============================================================
-- Clientes (5 registros)
-- PasswordHash usa BCrypt con costo 11.
-- Admin unico: cedula 1712345678, password "Monster9"
-- Clientes: password "cliente123"
-- Cedulas validas segun algoritmo ecuatoriano (digito verificador correcto)
-- ============================================================
INSERT INTO [dbo].[Clientes] ([Cedula], [Nombre], [Apellido], [FechaNacimiento], [Genero], [Telefono], [Email], [PasswordHash], [Rol]) VALUES
(N'1712345675', N'Monster', N'Admin', '1990-01-01', N'MASCULINO', N'0999999999', N'monster@ticketpremium.com', N'$2a$11$XJudBwF4RAtP0jW70kuh5OzheSBTu1M/kZ64v9fZsNa92bV4by.Sm', N'ADMIN'),
(N'1718765439', N'Maria', N'Garcia', '1985-10-20', N'FEMENINO',  N'0997654321', N'maria.garcia@email.com', N'$2a$11$qelASIpzU27bFaQ9x4z1D.yLsfqJT20zeDlzMYkXRbnnhygCSiOba', N'CLIENTE'),
(N'1709876542', N'Carlos', N'Lopez', '1995-03-08', N'MASCULINO', N'0981239876', N'carlos.lopez@email.com', N'$2a$11$qelASIpzU27bFaQ9x4z1D.yLsfqJT20zeDlzMYkXRbnnhygCSiOba', N'CLIENTE'),
(N'1712345683', N'Ana', N'Clienta', '1998-07-22', N'FEMENINO', N'0991111111', N'ana@email.com', N'$2a$11$qelASIpzU27bFaQ9x4z1D.yLsfqJT20zeDlzMYkXRbnnhygCSiOba', N'CLIENTE'),
(N'1712345691', N'Pedro', N'Usuario', '1992-03-15', N'MASCULINO', N'0982222222', N'pedro@email.com', N'$2a$11$qelASIpzU27bFaQ9x4z1D.yLsfqJT20zeDlzMYkXRbnnhygCSiOba', N'CLIENTE');
GO

-- ============================================================
-- Asientos (109 registros)
-- Generados según: AS-{AppFifaLocalidad}-{i:D2}
-- Filas: A (i 1-5), B (i 6-10), C (i 11-15)
-- ============================================================

-- LOC-A1 / PAR-001 / L001 (10 asientos)
INSERT INTO [dbo].[Asientos] ([Codigo], [Fila], [Numero], [Estado], [LocalidadCodigo], [PartidoCodigo]) VALUES
(N'AS-L001-01', N'A', 1,  N'LIBRE', N'LOC-A1', N'PAR-001'),
(N'AS-L001-02', N'A', 2,  N'LIBRE', N'LOC-A1', N'PAR-001'),
(N'AS-L001-03', N'A', 3,  N'LIBRE', N'LOC-A1', N'PAR-001'),
(N'AS-L001-04', N'A', 4,  N'LIBRE', N'LOC-A1', N'PAR-001'),
(N'AS-L001-05', N'A', 5,  N'LIBRE', N'LOC-A1', N'PAR-001'),
(N'AS-L001-06', N'B', 6,  N'LIBRE', N'LOC-A1', N'PAR-001'),
(N'AS-L001-07', N'B', 7,  N'LIBRE', N'LOC-A1', N'PAR-001'),
(N'AS-L001-08', N'B', 8,  N'LIBRE', N'LOC-A1', N'PAR-001'),
(N'AS-L001-09', N'B', 9,  N'LIBRE', N'LOC-A1', N'PAR-001'),
(N'AS-L001-10', N'B', 10, N'LIBRE', N'LOC-A1', N'PAR-001');
GO

-- LOC-A2 / PAR-001 / L002 (12 asientos)
INSERT INTO [dbo].[Asientos] ([Codigo], [Fila], [Numero], [Estado], [LocalidadCodigo], [PartidoCodigo]) VALUES
(N'AS-L002-01', N'A', 1,  N'LIBRE', N'LOC-A2', N'PAR-001'),
(N'AS-L002-02', N'A', 2,  N'LIBRE', N'LOC-A2', N'PAR-001'),
(N'AS-L002-03', N'A', 3,  N'LIBRE', N'LOC-A2', N'PAR-001'),
(N'AS-L002-04', N'A', 4,  N'LIBRE', N'LOC-A2', N'PAR-001'),
(N'AS-L002-05', N'A', 5,  N'LIBRE', N'LOC-A2', N'PAR-001'),
(N'AS-L002-06', N'B', 6,  N'LIBRE', N'LOC-A2', N'PAR-001'),
(N'AS-L002-07', N'B', 7,  N'LIBRE', N'LOC-A2', N'PAR-001'),
(N'AS-L002-08', N'B', 8,  N'LIBRE', N'LOC-A2', N'PAR-001'),
(N'AS-L002-09', N'B', 9,  N'LIBRE', N'LOC-A2', N'PAR-001'),
(N'AS-L002-10', N'B', 10, N'LIBRE', N'LOC-A2', N'PAR-001'),
(N'AS-L002-11', N'C', 11, N'LIBRE', N'LOC-A2', N'PAR-001'),
(N'AS-L002-12', N'C', 12, N'LIBRE', N'LOC-A2', N'PAR-001');
GO

-- LOC-A3 / PAR-001 / L003 (15 asientos)
INSERT INTO [dbo].[Asientos] ([Codigo], [Fila], [Numero], [Estado], [LocalidadCodigo], [PartidoCodigo]) VALUES
(N'AS-L003-01', N'A', 1,  N'LIBRE', N'LOC-A3', N'PAR-001'),
(N'AS-L003-02', N'A', 2,  N'LIBRE', N'LOC-A3', N'PAR-001'),
(N'AS-L003-03', N'A', 3,  N'LIBRE', N'LOC-A3', N'PAR-001'),
(N'AS-L003-04', N'A', 4,  N'LIBRE', N'LOC-A3', N'PAR-001'),
(N'AS-L003-05', N'A', 5,  N'LIBRE', N'LOC-A3', N'PAR-001'),
(N'AS-L003-06', N'B', 6,  N'LIBRE', N'LOC-A3', N'PAR-001'),
(N'AS-L003-07', N'B', 7,  N'LIBRE', N'LOC-A3', N'PAR-001'),
(N'AS-L003-08', N'B', 8,  N'LIBRE', N'LOC-A3', N'PAR-001'),
(N'AS-L003-09', N'B', 9,  N'LIBRE', N'LOC-A3', N'PAR-001'),
(N'AS-L003-10', N'B', 10, N'LIBRE', N'LOC-A3', N'PAR-001'),
(N'AS-L003-11', N'C', 11, N'LIBRE', N'LOC-A3', N'PAR-001'),
(N'AS-L003-12', N'C', 12, N'LIBRE', N'LOC-A3', N'PAR-001'),
(N'AS-L003-13', N'C', 13, N'LIBRE', N'LOC-A3', N'PAR-001'),
(N'AS-L003-14', N'C', 14, N'LIBRE', N'LOC-A3', N'PAR-001'),
(N'AS-L003-15', N'C', 15, N'LIBRE', N'LOC-A3', N'PAR-001');
GO

-- LOC-B1 / PAR-002 / L004 (10 asientos)
INSERT INTO [dbo].[Asientos] ([Codigo], [Fila], [Numero], [Estado], [LocalidadCodigo], [PartidoCodigo]) VALUES
(N'AS-L004-01', N'A', 1,  N'LIBRE', N'LOC-B1', N'PAR-002'),
(N'AS-L004-02', N'A', 2,  N'LIBRE', N'LOC-B1', N'PAR-002'),
(N'AS-L004-03', N'A', 3,  N'LIBRE', N'LOC-B1', N'PAR-002'),
(N'AS-L004-04', N'A', 4,  N'LIBRE', N'LOC-B1', N'PAR-002'),
(N'AS-L004-05', N'A', 5,  N'LIBRE', N'LOC-B1', N'PAR-002'),
(N'AS-L004-06', N'B', 6,  N'LIBRE', N'LOC-B1', N'PAR-002'),
(N'AS-L004-07', N'B', 7,  N'LIBRE', N'LOC-B1', N'PAR-002'),
(N'AS-L004-08', N'B', 8,  N'LIBRE', N'LOC-B1', N'PAR-002'),
(N'AS-L004-09', N'B', 9,  N'LIBRE', N'LOC-B1', N'PAR-002'),
(N'AS-L004-10', N'B', 10, N'LIBRE', N'LOC-B1', N'PAR-002');
GO

-- LOC-B2 / PAR-002 / L005 (12 asientos)
INSERT INTO [dbo].[Asientos] ([Codigo], [Fila], [Numero], [Estado], [LocalidadCodigo], [PartidoCodigo]) VALUES
(N'AS-L005-01', N'A', 1,  N'LIBRE', N'LOC-B2', N'PAR-002'),
(N'AS-L005-02', N'A', 2,  N'LIBRE', N'LOC-B2', N'PAR-002'),
(N'AS-L005-03', N'A', 3,  N'LIBRE', N'LOC-B2', N'PAR-002'),
(N'AS-L005-04', N'A', 4,  N'LIBRE', N'LOC-B2', N'PAR-002'),
(N'AS-L005-05', N'A', 5,  N'LIBRE', N'LOC-B2', N'PAR-002'),
(N'AS-L005-06', N'B', 6,  N'LIBRE', N'LOC-B2', N'PAR-002'),
(N'AS-L005-07', N'B', 7,  N'LIBRE', N'LOC-B2', N'PAR-002'),
(N'AS-L005-08', N'B', 8,  N'LIBRE', N'LOC-B2', N'PAR-002'),
(N'AS-L005-09', N'B', 9,  N'LIBRE', N'LOC-B2', N'PAR-002'),
(N'AS-L005-10', N'B', 10, N'LIBRE', N'LOC-B2', N'PAR-002'),
(N'AS-L005-11', N'C', 11, N'LIBRE', N'LOC-B2', N'PAR-002'),
(N'AS-L005-12', N'C', 12, N'LIBRE', N'LOC-B2', N'PAR-002');
GO

-- LOC-B3 / PAR-002 / L006 (15 asientos)
INSERT INTO [dbo].[Asientos] ([Codigo], [Fila], [Numero], [Estado], [LocalidadCodigo], [PartidoCodigo]) VALUES
(N'AS-L006-01', N'A', 1,  N'LIBRE', N'LOC-B3', N'PAR-002'),
(N'AS-L006-02', N'A', 2,  N'LIBRE', N'LOC-B3', N'PAR-002'),
(N'AS-L006-03', N'A', 3,  N'LIBRE', N'LOC-B3', N'PAR-002'),
(N'AS-L006-04', N'A', 4,  N'LIBRE', N'LOC-B3', N'PAR-002'),
(N'AS-L006-05', N'A', 5,  N'LIBRE', N'LOC-B3', N'PAR-002'),
(N'AS-L006-06', N'B', 6,  N'LIBRE', N'LOC-B3', N'PAR-002'),
(N'AS-L006-07', N'B', 7,  N'LIBRE', N'LOC-B3', N'PAR-002'),
(N'AS-L006-08', N'B', 8,  N'LIBRE', N'LOC-B3', N'PAR-002'),
(N'AS-L006-09', N'B', 9,  N'LIBRE', N'LOC-B3', N'PAR-002'),
(N'AS-L006-10', N'B', 10, N'LIBRE', N'LOC-B3', N'PAR-002'),
(N'AS-L006-11', N'C', 11, N'LIBRE', N'LOC-B3', N'PAR-002'),
(N'AS-L006-12', N'C', 12, N'LIBRE', N'LOC-B3', N'PAR-002'),
(N'AS-L006-13', N'C', 13, N'LIBRE', N'LOC-B3', N'PAR-002'),
(N'AS-L006-14', N'C', 14, N'LIBRE', N'LOC-B3', N'PAR-002'),
(N'AS-L006-15', N'C', 15, N'LIBRE', N'LOC-B3', N'PAR-002');
GO

-- LOC-C1 / PAR-003 / L007 (10 asientos)
INSERT INTO [dbo].[Asientos] ([Codigo], [Fila], [Numero], [Estado], [LocalidadCodigo], [PartidoCodigo]) VALUES
(N'AS-L007-01', N'A', 1,  N'LIBRE', N'LOC-C1', N'PAR-003'),
(N'AS-L007-02', N'A', 2,  N'LIBRE', N'LOC-C1', N'PAR-003'),
(N'AS-L007-03', N'A', 3,  N'LIBRE', N'LOC-C1', N'PAR-003'),
(N'AS-L007-04', N'A', 4,  N'LIBRE', N'LOC-C1', N'PAR-003'),
(N'AS-L007-05', N'A', 5,  N'LIBRE', N'LOC-C1', N'PAR-003'),
(N'AS-L007-06', N'B', 6,  N'LIBRE', N'LOC-C1', N'PAR-003'),
(N'AS-L007-07', N'B', 7,  N'LIBRE', N'LOC-C1', N'PAR-003'),
(N'AS-L007-08', N'B', 8,  N'LIBRE', N'LOC-C1', N'PAR-003'),
(N'AS-L007-09', N'B', 9,  N'LIBRE', N'LOC-C1', N'PAR-003'),
(N'AS-L007-10', N'B', 10, N'LIBRE', N'LOC-C1', N'PAR-003');
GO

-- LOC-C2 / PAR-003 / L008 (12 asientos)
INSERT INTO [dbo].[Asientos] ([Codigo], [Fila], [Numero], [Estado], [LocalidadCodigo], [PartidoCodigo]) VALUES
(N'AS-L008-01', N'A', 1,  N'LIBRE', N'LOC-C2', N'PAR-003'),
(N'AS-L008-02', N'A', 2,  N'LIBRE', N'LOC-C2', N'PAR-003'),
(N'AS-L008-03', N'A', 3,  N'LIBRE', N'LOC-C2', N'PAR-003'),
(N'AS-L008-04', N'A', 4,  N'LIBRE', N'LOC-C2', N'PAR-003'),
(N'AS-L008-05', N'A', 5,  N'LIBRE', N'LOC-C2', N'PAR-003'),
(N'AS-L008-06', N'B', 6,  N'LIBRE', N'LOC-C2', N'PAR-003'),
(N'AS-L008-07', N'B', 7,  N'LIBRE', N'LOC-C2', N'PAR-003'),
(N'AS-L008-08', N'B', 8,  N'LIBRE', N'LOC-C2', N'PAR-003'),
(N'AS-L008-09', N'B', 9,  N'LIBRE', N'LOC-C2', N'PAR-003'),
(N'AS-L008-10', N'B', 10, N'LIBRE', N'LOC-C2', N'PAR-003'),
(N'AS-L008-11', N'C', 11, N'LIBRE', N'LOC-C2', N'PAR-003'),
(N'AS-L008-12', N'C', 12, N'LIBRE', N'LOC-C2', N'PAR-003');
GO

-- LOC-C3 / PAR-003 / L009 (15 asientos)
INSERT INTO [dbo].[Asientos] ([Codigo], [Fila], [Numero], [Estado], [LocalidadCodigo], [PartidoCodigo]) VALUES
(N'AS-L009-01', N'A', 1,  N'LIBRE', N'LOC-C3', N'PAR-003'),
(N'AS-L009-02', N'A', 2,  N'LIBRE', N'LOC-C3', N'PAR-003'),
(N'AS-L009-03', N'A', 3,  N'LIBRE', N'LOC-C3', N'PAR-003'),
(N'AS-L009-04', N'A', 4,  N'LIBRE', N'LOC-C3', N'PAR-003'),
(N'AS-L009-05', N'A', 5,  N'LIBRE', N'LOC-C3', N'PAR-003'),
(N'AS-L009-06', N'B', 6,  N'LIBRE', N'LOC-C3', N'PAR-003'),
(N'AS-L009-07', N'B', 7,  N'LIBRE', N'LOC-C3', N'PAR-003'),
(N'AS-L009-08', N'B', 8,  N'LIBRE', N'LOC-C3', N'PAR-003'),
(N'AS-L009-09', N'B', 9,  N'LIBRE', N'LOC-C3', N'PAR-003'),
(N'AS-L009-10', N'B', 10, N'LIBRE', N'LOC-C3', N'PAR-003'),
(N'AS-L009-11', N'C', 11, N'LIBRE', N'LOC-C3', N'PAR-003'),
(N'AS-L009-12', N'C', 12, N'LIBRE', N'LOC-C3', N'PAR-003'),
(N'AS-L009-13', N'C', 13, N'LIBRE', N'LOC-C3', N'PAR-003'),
(N'AS-L009-14', N'C', 14, N'LIBRE', N'LOC-C3', N'PAR-003'),
(N'AS-L009-15', N'C', 15, N'LIBRE', N'LOC-C3', N'PAR-003');
GO

PRINT 'DML TicketPremiumDB ejecutado correctamente.';
GO
