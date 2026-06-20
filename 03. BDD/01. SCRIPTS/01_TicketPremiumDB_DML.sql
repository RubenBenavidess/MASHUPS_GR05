USE [TicketPremiumDB]
GO

-- ============================================================
-- Limpiar tablas
-- ============================================================
DELETE FROM [dbo].[DetallesFactura];
DELETE FROM [dbo].[Facturas];
DELETE FROM [dbo].[Asientos];
DELETE FROM [dbo].[Localidades];
DELETE FROM [dbo].[Partidos];
DELETE FROM [dbo].[Estadios];
DELETE FROM [dbo].[Paises];
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
-- Estadios (16 registros)
-- ============================================================
INSERT INTO [dbo].[Estadios] ([Codigo], [Nombre], [Ciudad], [CapacidadTotal], [PaisCodigo]) VALUES
(N'EST-001', N'MetLife Stadium', N'New York/New Jersey', 82500, N'USA'),
(N'EST-002', N'AT&T Stadium', N'Dallas', 80000, N'USA'),
(N'EST-003', N'Arrowhead Stadium', N'Kansas City', 76416, N'USA'),
(N'EST-004', N'NRG Stadium', N'Houston', 72220, N'USA'),
(N'EST-005', N'Mercedes-Benz Stadium', N'Atlanta', 71000, N'USA'),
(N'EST-006', N'SoFi Stadium', N'Los Ángeles', 70240, N'USA'),
(N'EST-007', N'Lincoln Financial Field', N'Philadelphia', 69796, N'USA'),
(N'EST-008', N'Lumen Field', N'Seattle', 69000, N'USA'),
(N'EST-009', N'Levi''s Stadium', N'San Francisco Bay Area', 68500, N'USA'),
(N'EST-010', N'Gillette Stadium', N'Boston', 65878, N'USA'),
(N'EST-011', N'Hard Rock Stadium', N'Miami', 64767, N'USA'),
(N'EST-012', N'Estadio Azteca', N'Ciudad de México', 83264, N'MEX'),
(N'EST-013', N'Estadio BBVA', N'Monterrey', 53500, N'MEX'),
(N'EST-014', N'Estadio Akron', N'Guadalajara', 49850, N'MEX'),
(N'EST-015', N'BC Place', N'Vancouver', 54500, N'CAN'),
(N'EST-016', N'BMO Field', N'Toronto', 45000, N'CAN');
GO

-- ============================================================
-- Partidos (104 registros)
-- ============================================================
INSERT INTO [dbo].[Partidos] ([Codigo], [EquipoLocal], [EquipoVisitante], [FechaHora], [EstadioCodigo]) VALUES
(N'PAR-001', N'México', N'Sudáfrica', '2026-06-11T18:00:00', N'EST-012'),
(N'PAR-002', N'Corea del Sur', N'Chequia', '2026-06-11T21:00:00', N'EST-014'),
(N'PAR-003', N'Canadáá', N'Bosnia y Herzegovina', '2026-06-12T18:00:00', N'EST-016'),
(N'PAR-004', N'EE. UU.', N'Paraguay', '2026-06-12T20:00:00', N'EST-006'),
(N'PAR-005', N'Qatar', N'Suiza', '2026-06-13T14:00:00', N'EST-009'),
(N'PAR-006', N'Brasil', N'Marruecos', '2026-06-13T17:00:00', N'EST-001'),
(N'PAR-007', N'Haitííí', N'Escocia', '2026-06-13T20:00:00', N'EST-010'),
(N'PAR-008', N'Australia', N'Türkiye', '2026-06-13T23:00:00', N'EST-015'),
(N'PAR-009', N'Alemania', N'Curazao', '2026-06-14T12:00:00', N'EST-004'),
(N'PAR-010', N'Países Bajos', N'Japón', '2026-06-14T15:00:00', N'EST-002'),
(N'PAR-011', N'Costa de Marfil', N'Ecuador', '2026-06-14T18:00:00', N'EST-007'),
(N'PAR-012', N'Suecia', N'Túnez', '2026-06-14T21:00:00', N'EST-013'),
(N'PAR-013', N'España', N'Cabo Verde', '2026-06-15T11:00:00', N'EST-005'),
(N'PAR-014', N'Bélgica', N'Egipto', '2026-06-15T14:00:00', N'EST-008'),
(N'PAR-015', N'Arabia Saudita', N'Uruguay', '2026-06-15T17:00:00', N'EST-011'),
(N'PAR-016', N'Irán', N'Nueva Zelanda', '2026-06-15T20:00:00', N'EST-006'),
(N'PAR-017', N'Francia', N'Senegal', '2026-06-16T14:00:00', N'EST-001'),
(N'PAR-018', N'Irak', N'Noruega', '2026-06-16T17:00:00', N'EST-010'),
(N'PAR-019', N'Argentina', N'Argelia', '2026-06-16T20:00:00', N'EST-003'),
(N'PAR-020', N'Austria', N'Jordania', '2026-06-16T23:00:00', N'EST-009'),
(N'PAR-021', N'Portugal', N'RD Congo', '2026-06-17T12:00:00', N'EST-004'),
(N'PAR-022', N'Inglaterra', N'Croacia', '2026-06-17T15:00:00', N'EST-002'),
(N'PAR-023', N'Ghana', N'Panamáá', '2026-06-17T18:00:00', N'EST-016'),
(N'PAR-024', N'Uzbekistán', N'Colombia', '2026-06-17T21:00:00', N'EST-012'),
(N'PAR-025', N'Chequia', N'Sudáfrica', '2026-06-18T11:00:00', N'EST-005'),
(N'PAR-026', N'Suiza', N'Bosnia y Herzegovina', '2026-06-18T14:00:00', N'EST-006'),
(N'PAR-027', N'Canadáá', N'Qatar', '2026-06-18T17:00:00', N'EST-015'),
(N'PAR-028', N'México', N'Corea del Sur', '2026-06-18T20:00:00', N'EST-014'),
(N'PAR-029', N'EE. UU.', N'Australia', '2026-06-19T14:00:00', N'EST-008'),
(N'PAR-030', N'Escocia', N'Marruecos', '2026-06-19T17:00:00', N'EST-010'),
(N'PAR-031', N'Brasil', N'Haitííí', '2026-06-19T20:00:00', N'EST-007'),
(N'PAR-032', N'Türkiye', N'Paraguay', '2026-06-19T23:00:00', N'EST-009'),
(N'PAR-033', N'Países Bajos', N'Suecia', '2026-06-20T12:00:00', N'EST-004'),
(N'PAR-034', N'Alemania', N'Costa de Marfil', '2026-06-20T15:00:00', N'EST-016'),
(N'PAR-035', N'Ecuador', N'Curazao', '2026-06-20T19:00:00', N'EST-003'),
(N'PAR-036', N'Túnez', N'Japón', '2026-06-20T23:00:00', N'EST-013'),
(N'PAR-037', N'España', N'Arabia Saudita', '2026-06-21T11:00:00', N'EST-005'),
(N'PAR-038', N'Bélgica', N'Irán', '2026-06-21T14:00:00', N'EST-006'),
(N'PAR-039', N'Uruguay', N'Cabo Verde', '2026-06-21T17:00:00', N'EST-011'),
(N'PAR-040', N'Nueva Zelanda', N'Egipto', '2026-06-21T20:00:00', N'EST-015'),
(N'PAR-041', N'Argentina', N'Austria', '2026-06-22T12:00:00', N'EST-002'),
(N'PAR-042', N'Francia', N'Irak', '2026-06-22T17:00:00', N'EST-007'),
(N'PAR-043', N'Noruega', N'Senegal', '2026-06-22T19:00:00', N'EST-001'),
(N'PAR-044', N'Jordania', N'Argelia', '2026-06-22T22:00:00', N'EST-009'),
(N'PAR-045', N'Portugal', N'Uzbekistán', '2026-06-23T12:00:00', N'EST-004'),
(N'PAR-046', N'Inglaterra', N'Ghana', '2026-06-23T16:00:00', N'EST-010'),
(N'PAR-047', N'Panamáá', N'Croacia', '2026-06-23T18:00:00', N'EST-016'),
(N'PAR-048', N'Colombia', N'RD Congo', '2026-06-23T21:00:00', N'EST-014'),
(N'PAR-049', N'Canadáá', N'Suiza', '2026-06-24T14:00:00', N'EST-015'),
(N'PAR-050', N'Bosnia y Herz.', N'Qatar', '2026-06-24T14:00:00', N'EST-008'),
(N'PAR-051', N'Escocia', N'Brasil', '2026-06-24T17:00:00', N'EST-011'),
(N'PAR-052', N'Marruecos', N'Haitííí', '2026-06-24T17:00:00', N'EST-005'),
(N'PAR-053', N'México', N'Chequia', '2026-06-24T20:00:00', N'EST-012'),
(N'PAR-054', N'Sudáfrica', N'Corea del Sur', '2026-06-24T20:00:00', N'EST-013'),
(N'PAR-055', N'Ecuador', N'Alemania', '2026-06-25T15:00:00', N'EST-001'),
(N'PAR-056', N'Curazao', N'Costa de Marfil', '2026-06-25T15:00:00', N'EST-007'),
(N'PAR-057', N'Japón', N'Suecia', '2026-06-25T18:00:00', N'EST-002'),
(N'PAR-058', N'Túnez', N'Países Bajos', '2026-06-25T18:00:00', N'EST-003'),
(N'PAR-059', N'EE. UU.', N'Türkiye', '2026-06-25T21:00:00', N'EST-006'),
(N'PAR-060', N'Paraguay', N'Australia', '2026-06-25T21:00:00', N'EST-009'),
(N'PAR-061', N'Noruega', N'Francia', '2026-06-26T14:00:00', N'EST-010'),
(N'PAR-062', N'Senegal', N'Irak', '2026-06-26T14:00:00', N'EST-016'),
(N'PAR-063', N'Cabo Verde', N'Arabia Saudita', '2026-06-26T19:00:00', N'EST-004'),
(N'PAR-064', N'Uruguay', N'España', '2026-06-26T19:00:00', N'EST-014'),
(N'PAR-065', N'Egipto', N'Irán', '2026-06-26T22:00:00', N'EST-008'),
(N'PAR-066', N'Nueva Zelanda', N'Bélgica', '2026-06-26T22:00:00', N'EST-015'),
(N'PAR-067', N'Panamáá', N'Inglaterra', '2026-06-27T16:00:00', N'EST-001'),
(N'PAR-068', N'Croacia', N'Ghana', '2026-06-27T16:00:00', N'EST-007'),
(N'PAR-069', N'Colombia', N'Portugal', '2026-06-27T18:30:00', N'EST-011'),
(N'PAR-070', N'RD Congo', N'Uzbekistán', '2026-06-27T18:30:00', N'EST-005'),
(N'PAR-071', N'Argelia', N'Austria', '2026-06-27T21:00:00', N'EST-003'),
(N'PAR-072', N'Jordania', N'Argentina', '2026-06-27T21:00:00', N'EST-002'),
(N'PAR-073', N'Ganador R32 A', N'Ganador R32 B', '2026-06-28T20:00:00', N'EST-010'),
(N'PAR-074', N'Ganador R32 A', N'Ganador R32 B', '2026-06-29T20:00:00', N'EST-001'),
(N'PAR-075', N'Ganador R32 A', N'Ganador R32 B', '2026-06-29T20:00:00', N'EST-012'),
(N'PAR-076', N'Ganador R32 A', N'Ganador R32 B', '2026-06-28T20:00:00', N'EST-010'),
(N'PAR-077', N'Ganador R32 A', N'Ganador R32 B', '2026-06-28T20:00:00', N'EST-006'),
(N'PAR-078', N'Ganador R32 A', N'Ganador R32 B', '2026-06-28T20:00:00', N'EST-016'),
(N'PAR-079', N'Ganador R32 A', N'Ganador R32 B', '2026-06-28T20:00:00', N'EST-009'),
(N'PAR-080', N'Ganador R32 A', N'Ganador R32 B', '2026-06-29T20:00:00', N'EST-013'),
(N'PAR-081', N'Ganador R32 A', N'Ganador R32 B', '2026-06-28T20:00:00', N'EST-009'),
(N'PAR-082', N'Ganador R32 A', N'Ganador R32 B', '2026-06-30T20:00:00', N'EST-008'),
(N'PAR-083', N'Ganador R32 A', N'Ganador R32 B', '2026-06-28T20:00:00', N'EST-003'),
(N'PAR-084', N'Ganador R32 A', N'Ganador R32 B', '2026-06-29T20:00:00', N'EST-002'),
(N'PAR-085', N'Ganador R32 A', N'Ganador R32 B', '2026-06-29T20:00:00', N'EST-012'),
(N'PAR-086', N'Ganador R32 A', N'Ganador R32 B', '2026-06-30T20:00:00', N'EST-001'),
(N'PAR-087', N'Ganador R32 A', N'Ganador R32 B', '2026-06-28T20:00:00', N'EST-001'),
(N'PAR-088', N'Ganador R32 A', N'Ganador R32 B', '2026-06-28T20:00:00', N'EST-010'),
(N'PAR-089', N'Ganador R16 A', N'Ganador R16 B', '2026-07-07T20:00:00', N'EST-013'),
(N'PAR-090', N'Ganador R16 A', N'Ganador R16 B', '2026-07-06T20:00:00', N'EST-003'),
(N'PAR-091', N'Ganador R16 A', N'Ganador R16 B', '2026-07-06T20:00:00', N'EST-013'),
(N'PAR-092', N'Ganador R16 A', N'Ganador R16 B', '2026-07-05T20:00:00', N'EST-010'),
(N'PAR-093', N'Ganador R16 A', N'Ganador R16 B', '2026-07-04T20:00:00', N'EST-009'),
(N'PAR-094', N'Ganador R16 A', N'Ganador R16 B', '2026-07-06T20:00:00', N'EST-011'),
(N'PAR-095', N'Ganador R16 A', N'Ganador R16 B', '2026-07-07T20:00:00', N'EST-010'),
(N'PAR-096', N'Ganador R16 A', N'Ganador R16 B', '2026-07-06T20:00:00', N'EST-004'),
(N'PAR-097', N'Ganador QF A', N'Ganador QF B', '2026-07-10T20:00:00', N'EST-010'),
(N'PAR-098', N'Ganador QF A', N'Ganador QF B', '2026-07-09T20:00:00', N'EST-003'),
(N'PAR-099', N'Ganador QF A', N'Ganador QF B', '2026-07-09T20:00:00', N'EST-006'),
(N'PAR-100', N'Ganador QF A', N'Ganador QF B', '2026-07-11T20:00:00', N'EST-005'),
(N'PAR-101', N'Ganador SF A', N'Ganador SF B', '2026-07-14T20:00:00', N'EST-007'),
(N'PAR-102', N'Ganador SF A', N'Ganador SF B', '2026-07-14T20:00:00', N'EST-002'),
(N'PAR-103', N'Ganador 3RD A', N'Ganador 3RD B', '2026-07-18T20:00:00', N'EST-008'),
(N'PAR-104', N'Ganador FINAL A', N'Ganador FINAL B', '2026-07-19T20:00:00', N'EST-002');
GO

-- ============================================================
-- Localidades y Asientos (Generación Automática)
-- ============================================================
DECLARE @estId NVARCHAR(20);
DECLARE @capacidad INT;

DECLARE cursor_estadios CURSOR FOR SELECT [Codigo], [CapacidadTotal] FROM [dbo].[Estadios];
OPEN cursor_estadios;
FETCH NEXT FROM cursor_estadios INTO @estId, @capacidad;
WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO [dbo].[Localidades] ([Codigo], [Descripcion], [Capacidad], [PrecioBase], [EstadioCodigo]) VALUES
    (N'L-' + REPLACE(@estId, 'EST-', '') + N'-1', N'Palco VIP Norte', @capacidad * 0.05, 450.00, @estId),
    (N'L-' + REPLACE(@estId, 'EST-', '') + N'-2', N'Palco VIP Sur', @capacidad * 0.05, 450.00, @estId),
    (N'L-' + REPLACE(@estId, 'EST-', '') + N'-3', N'Tribuna Lateral', @capacidad * 0.30, 250.00, @estId),
    (N'L-' + REPLACE(@estId, 'EST-', '') + N'-4', N'General Cabecera Norte', @capacidad * 0.30, 100.00, @estId),
    (N'L-' + REPLACE(@estId, 'EST-', '') + N'-5', N'General Cabecera Sur', @capacidad * 0.30, 100.00, @estId);
    FETCH NEXT FROM cursor_estadios INTO @estId, @capacidad;
END
CLOSE cursor_estadios;
DEALLOCATE cursor_estadios;
GO

DECLARE @partidoId NVARCHAR(20);
DECLARE @estId NVARCHAR(20);
DECLARE @locFifaId NVARCHAR(50);
DECLARE @locTkId NVARCHAR(50);

DECLARE cursor_partidos CURSOR FOR 
SELECT p.[Codigo], p.[EstadioCodigo] FROM [dbo].[Partidos] p;
OPEN cursor_partidos;
FETCH NEXT FROM cursor_partidos INTO @partidoId, @estId;
WHILE @@FETCH_STATUS = 0
BEGIN
    -- 1. Palco VIP Norte (10)
    SET @locFifaId = N'L-' + @partidoId + N'-1';
    SET @locTkId = N'L-' + REPLACE(@estId, 'EST-', '') + N'-1';
    INSERT INTO [dbo].[Asientos] ([Codigo], [Fila], [Numero], [Estado], [LocalidadCodigo], [PartidoCodigo])
    SELECT N'AS-' + @locFifaId + N'-A' + CAST(N AS NVARCHAR), N'A', N, N'LIBRE', @locTkId, @partidoId
    FROM (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10)) T(N);

    -- 2. Palco VIP Sur (10)
    SET @locFifaId = N'L-' + @partidoId + N'-2';
    SET @locTkId = N'L-' + REPLACE(@estId, 'EST-', '') + N'-2';
    INSERT INTO [dbo].[Asientos] ([Codigo], [Fila], [Numero], [Estado], [LocalidadCodigo], [PartidoCodigo])
    SELECT N'AS-' + @locFifaId + N'-A' + CAST(N AS NVARCHAR), N'A', N, N'LIBRE', @locTkId, @partidoId
    FROM (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10)) T(N);

    -- 3. Tribuna Lateral (70)
    SET @locFifaId = N'L-' + @partidoId + N'-3';
    SET @locTkId = N'L-' + REPLACE(@estId, 'EST-', '') + N'-3';
    DECLARE @fila INT = 1;
    WHILE @fila <= 5
    BEGIN
        DECLARE @letraFila3 NVARCHAR(1) = CHAR(64 + @fila);
        INSERT INTO [dbo].[Asientos] ([Codigo], [Fila], [Numero], [Estado], [LocalidadCodigo], [PartidoCodigo])
        SELECT N'AS-' + @locFifaId + N'-' + @letraFila3 + CAST(N AS NVARCHAR), @letraFila3, N, N'LIBRE', @locTkId, @partidoId
        FROM (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12),(13),(14)) T(N);
        SET @fila = @fila + 1;
    END

    -- 4. General Cabecera Norte (70)
    SET @locFifaId = N'L-' + @partidoId + N'-4';
    SET @locTkId = N'L-' + REPLACE(@estId, 'EST-', '') + N'-4';
    SET @fila = 1;
    WHILE @fila <= 5
    BEGIN
        DECLARE @letraFila NVARCHAR(1) = CHAR(64 + @fila);
        INSERT INTO [dbo].[Asientos] ([Codigo], [Fila], [Numero], [Estado], [LocalidadCodigo], [PartidoCodigo])
        SELECT N'AS-' + @locFifaId + N'-' + @letraFila + CAST(N AS NVARCHAR), @letraFila, N, N'LIBRE', @locTkId, @partidoId
        FROM (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12),(13),(14)) T(N);
        SET @fila = @fila + 1;
    END

    -- 5. General Cabecera Sur (70)
    SET @locFifaId = N'L-' + @partidoId + N'-5';
    SET @locTkId = N'L-' + REPLACE(@estId, 'EST-', '') + N'-5';
    SET @fila = 1;
    WHILE @fila <= 5
    BEGIN
        DECLARE @letraFila2 NVARCHAR(1) = CHAR(64 + @fila);
        INSERT INTO [dbo].[Asientos] ([Codigo], [Fila], [Numero], [Estado], [LocalidadCodigo], [PartidoCodigo])
        SELECT N'AS-' + @locFifaId + N'-' + @letraFila2 + CAST(N AS NVARCHAR), @letraFila2, N, N'LIBRE', @locTkId, @partidoId
        FROM (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12),(13),(14)) T(N);
        SET @fila = @fila + 1;
    END

    FETCH NEXT FROM cursor_partidos INTO @partidoId, @estId;
END
CLOSE cursor_partidos;
DEALLOCATE cursor_partidos;
GO


INSERT INTO [dbo].[Clientes] ([Cedula], [Nombre], [Apellido], [FechaNacimiento], [Genero], [Telefono], [Email], [PasswordHash], [Rol]) VALUES
(N'1712345675', N'Monster', N'Admin', '1990-01-01', N'MASCULINO', N'0999999999', N'monster@ticketpremium.com', N'$2a$11$XJudBwF4RAtP0jW70kuh5OzheSBTu1M/kZ64v9fZsNa92bV4by.Sm', N'ADMIN'),
(N'1718765439', N'Maria', N'Garcia', '1985-10-20', N'FEMENINO',  N'0997654321', N'maria.garcia@email.com', N'$2a$11$qelASIpzU27bFaQ9x4z1D.yLsfqJT20zeDlzMYkXRbnnhygCSiOba', N'CLIENTE'),
(N'1709876542', N'Carlos', N'Lopez', '1995-03-08', N'MASCULINO', N'0981239876', N'carlos.lopez@email.com', N'$2a$11$qelASIpzU27bFaQ9x4z1D.yLsfqJT20zeDlzMYkXRbnnhygCSiOba', N'CLIENTE'),
(N'1712345683', N'Ana', N'Clienta', '1998-07-22', N'FEMENINO', N'0991111111', N'ana@email.com', N'$2a$11$qelASIpzU27bFaQ9x4z1D.yLsfqJT20zeDlzMYkXRbnnhygCSiOba', N'CLIENTE'),
(N'1712345691', N'Pedro', N'Usuario', '1992-03-15', N'MASCULINO', N'0982222222', N'pedro@email.com', N'$2a$11$qelASIpzU27bFaQ9x4z1D.yLsfqJT20zeDlzMYkXRbnnhygCSiOba', N'CLIENTE');
GO