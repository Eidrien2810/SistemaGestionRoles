/* ============================================================
   PROYECTO: Sistema de Gestión Empresarial con Control de Roles
   TECNOLOGÍA: SQL Server
   AUTOR: Jesus Guerrero | A00118565
   FECHA DE ENTREGA: 11-06-2026
   ============================================================ */

USE master;
GO

IF DB_ID('SistemaGestionRolesDB') IS NOT NULL
BEGIN
    ALTER DATABASE SistemaGestionRolesDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SistemaGestionRolesDB;
END
GO

CREATE DATABASE SistemaGestionRolesDB;
GO

USE SistemaGestionRolesDB;
GO

CREATE TABLE Roles
(
    IdRol INT IDENTITY(1,1) PRIMARY KEY,
    NombreRol VARCHAR(50) NOT NULL UNIQUE,
);
GO


CREATE TABLE Usuarios
(
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario VARCHAR(50) NOT NULL UNIQUE,
    Clave VARCHAR(100) NOT NULL,
    IdRol INT NOT NULL,
    IntentosFallidos INT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    BloqueadoHasta DATETIME NULL,
    UltimoAcceso DATETIME NULL,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Usuarios_Roles 
    FOREIGN KEY (IdRol) REFERENCES Roles(IdRol)
);
GO

CREATE TABLE Clientes
(
    IdCliente INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Telefono VARCHAR(20) NOT NULL,
    Correo VARCHAR(100) NOT NULL,
    Direccion VARCHAR(200) NOT NULL,
    Estado BIT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
);
GO


INSERT INTO Roles (NombreRol)
VALUES
('Administrador'),
('Supervisor'),
('Ejecutor'),
('Invitado');
GO


INSERT INTO Usuarios 
(
    NombreUsuario, 
    Clave, 
    IdRol, 
    IntentosFallidos, 
    Activo, 
    BloqueadoHasta, 
    UltimoAcceso
)
VALUES
('admin', 'admin123', 1, 0, 1, NULL, NULL),
('supervisor', 'supervisor123', 2, 0, 1, NULL, NULL),
('ejecutor', 'user78910', 3, 0, 1, NULL, NULL);
GO

INSERT INTO Clientes (Nombre, Telefono, Correo, Direccion, Estado)
VALUES
('Juan Pérez', '809-555-1001', 'juan.perez@email.com', 'Santo Domingo Este', 1),
('María Rodríguez', '809-555-1002', 'maria.rodriguez@email.com', 'Distrito Nacional', 1),
('Carlos Gómez', '809-555-1003', 'carlos.gomez@email.com', 'Santiago de los Caballeros', 1),
('Ana Martínez', '809-555-1004', 'ana.martinez@email.com', 'San Cristóbal', 1),
('Luis Fernández', '809-555-1005', 'luis.fernandez@email.com', 'La Vega', 1),
('Patricia Sánchez', '809-555-1006', 'patricia.sanchez@email.com', 'Puerto Plata', 1),
('José Ramírez', '809-555-1007', 'jose.ramirez@email.com', 'San Pedro de Macorís', 1),
('Laura Castillo', '809-555-1008', 'laura.castillo@email.com', 'Moca', 1),
('Miguel Herrera', '809-555-1009', 'miguel.herrera@email.com', 'Bonao', 1),
('Carmen Núñez', '809-555-1010', 'carmen.nunez@email.com', 'Higüey', 1),
('Pedro Vargas', '809-555-1011', 'pedro.vargas@email.com', 'Baní', 1),
('Rosa Méndez', '809-555-1012', 'rosa.mendez@email.com', 'Azua', 1),
('Francisco Díaz', '809-555-1013', 'francisco.diaz@email.com', 'Barahona', 1),
('Isabel Torres', '809-555-1014', 'isabel.torres@email.com', 'San Francisco de Macorís', 1),
('Ricardo Peña', '809-555-1015', 'ricardo.pena@email.com', 'Nagua', 1),
('Gabriela Jiménez', '809-555-1016', 'gabriela.jimenez@email.com', 'Jarabacoa', 1),
('Andrés Molina', '809-555-1017', 'andres.molina@email.com', 'Monte Plata', 1),
('Lucía Cabrera', '809-555-1018', 'lucia.cabrera@email.com', 'Constanza', 1),
('Emilio Castro', '809-555-1019', 'emilio.castro@email.com', 'Dajabón', 0),
('Valentina Ortiz', '809-555-1020', 'valentina.ortiz@email.com', 'Samaná', 1);
GO



SELECT 
    IdRol,
    NombreRol,
    Descripcion
FROM Roles;
GO

SELECT 
    U.IdUsuario,
    U.NombreUsuario,
    U.Clave,
    R.NombreRol,
    U.IntentosFallidos,
    U.Activo,
    U.BloqueadoHasta,
    U.UltimoAcceso,
    U.FechaCreacion
FROM Usuarios U
INNER JOIN Roles R ON U.IdRol = R.IdRol;
GO

SELECT 
    IdCliente,
    Nombre,
    Telefono,
    Correo,
    Direccion,
    Estado,
    FechaRegistro
FROM Clientes;
GO


/*
Esta es la consulta lógica que usa el sistema para validar credenciales:

SELECT 
    U.IdUsuario,
    U.NombreUsuario,
    U.Clave,
    U.IdRol,
    R.NombreRol,
    U.IntentosFallidos,
    U.Activo,
    U.BloqueadoHasta,
    U.UltimoAcceso
FROM Usuarios U
INNER JOIN Roles R ON U.IdRol = R.IdRol
WHERE U.NombreUsuario = @NombreUsuario
AND U.Clave = @Clave;
*/



/*
SELECT 
    IdCliente,
    Nombre,
    Telefono,
    Correo,
    Direccion,
    Estado
FROM Clientes;
*/


/*
SELECT 
    IdCliente,
    Nombre,
    Telefono,
    Correo,
    Direccion,
    Estado
FROM Clientes
WHERE 
    Nombre LIKE @Filtro
    OR Telefono LIKE @Filtro
    OR Correo LIKE @Filtro
    OR CAST(IdCliente AS VARCHAR) LIKE @Filtro;
*/

-- Esta linea de aca hace case sensitive estos campos
ALTER TABLE Usuarios 
ALTER COLUMN Clave VARCHAR(100) 
COLLATE SQL_Latin1_General_CP1_CS_AS;

ALTER TABLE Usuarios 
ALTER COLUMN NombreUsuario VARCHAR(50) 
COLLATE SQL_Latin1_General_CP1_CS_AS;

-- Esta linea arregla el error de que en la tabla clientes los ids salten a 1000
ALTER DATABASE SCOPED CONFIGURATION SET IDENTITY_CACHE = OFF