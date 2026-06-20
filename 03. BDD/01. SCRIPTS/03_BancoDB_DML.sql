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

PRINT 'DML BancoDB ejecutado correctamente (Solo Clientes).';
GO
