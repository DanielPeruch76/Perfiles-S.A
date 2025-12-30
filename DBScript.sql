USE master;
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'PERFILES_SA')
BEGIN
    CREATE DATABASE PERFILES_SA;
    PRINT 'Base de datos PERFILES_SA creada.';
END
GO

USE PERFILES_SA;
GO

-- =============================================
-- TABLA: Departamentos
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Departamentos]') AND type in (N'U'))
BEGIN
    CREATE TABLE Departamentos (
        DepartamentoId INT PRIMARY KEY IDENTITY(1,1),
        Nombre NVARCHAR(100) NOT NULL,
        Descripcion NVARCHAR(500),
        Activo BIT DEFAULT 1 NOT NULL,
        FechaCreacion DATETIME DEFAULT GETDATE(),
        FechaActualizacion DATETIME
    );
    PRINT 'Tabla Departamentos creada.';
END
GO

-- =============================================
-- TABLA: Empleados
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Empleados]') AND type in (N'U'))
BEGIN
    CREATE TABLE Empleados (
        EmpleadoId INT PRIMARY KEY IDENTITY(1,1),
        DPI NVARCHAR(20) UNIQUE NOT NULL,
        Nombres NVARCHAR(100) NOT NULL,
        FechaNacimiento DATE NOT NULL,
        Sexo CHAR(1) CHECK (Sexo IN ('M', 'F')) NOT NULL,
        FechaIngreso DATE NOT NULL,
        Direccion NVARCHAR(500),
        NIT NVARCHAR(20),
        DepartamentoId INT NOT NULL,
        Activo BIT DEFAULT 1 NOT NULL,
        FechaCreacion DATETIME DEFAULT GETDATE(),
        FechaActualizacion DATETIME,
        
        CONSTRAINT FK_Empleados_Departamentos 
            FOREIGN KEY (DepartamentoId) 
            REFERENCES Departamentos(DepartamentoId)
            ON UPDATE CASCADE
    );
    PRINT 'Tabla Empleados creada.';
END
GO

-- =============================================
-- PROCEDIMIENTOS ALMACENADOS: Departamentos
-- =============================================

-- Insertar Departamento
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_InsertarDepartamento]') AND type in (N'P'))
    DROP PROCEDURE [dbo].[usp_InsertarDepartamento];
GO

CREATE PROCEDURE usp_InsertarDepartamento
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(500) = NULL,
    @NuevoId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Departamentos (Nombre, Descripcion)
    VALUES (@Nombre, @Descripcion);
    
    SET @NuevoId = SCOPE_IDENTITY();
    
    RETURN 0;
END
GO

-- Actualizar Departamento
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ActualizarDepartamento]') AND type in (N'P'))
    DROP PROCEDURE [dbo].[usp_ActualizarDepartamento];
GO

CREATE PROCEDURE usp_ActualizarDepartamento
    @DepartamentoId INT,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Departamentos 
    SET Nombre = @Nombre,
        Descripcion = @Descripcion,
        FechaActualizacion = GETDATE()
    WHERE DepartamentoId = @DepartamentoId;
    
    RETURN 0;
END
GO

-- Cambiar Estado Departamento
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_CambiarEstadoDepartamento]') AND type in (N'P'))
    DROP PROCEDURE [dbo].[usp_CambiarEstadoDepartamento];
GO

CREATE PROCEDURE usp_CambiarEstadoDepartamento
    @DepartamentoId INT,
    @Activo BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    
    UPDATE Departamentos 
    SET Activo = @Activo,
        FechaActualizacion = GETDATE()
    WHERE DepartamentoId = @DepartamentoId;
    
    IF @Activo = 0
    BEGIN
        UPDATE Empleados 
        SET Activo = 0,
            FechaActualizacion = GETDATE()
        WHERE DepartamentoId = @DepartamentoId;
    END
    
    COMMIT TRANSACTION;
    
    RETURN 0;
END
GO

-- Obtener Departamentos Activos
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ObtenerDepartamentosActivos]') AND type in (N'P'))
    DROP PROCEDURE [dbo].[usp_ObtenerDepartamentosActivos];
GO

CREATE PROCEDURE usp_ObtenerDepartamentosActivos
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT DepartamentoId, Nombre, Descripcion, Activo
    FROM Departamentos
    WHERE Activo = 1
    ORDER BY Nombre;
    
    RETURN 0;
END
GO

-- Obtener Todos Departamentos
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ObtenerTodosDepartamentos]') AND type in (N'P'))
    DROP PROCEDURE [dbo].[usp_ObtenerTodosDepartamentos];
GO

CREATE PROCEDURE usp_ObtenerTodosDepartamentos
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT DepartamentoId, Nombre, Descripcion, Activo,
           FechaCreacion, FechaActualizacion
    FROM Departamentos
    ORDER BY Nombre;
    
    RETURN 0;
END
GO

-- =============================================
-- PROCEDIMIENTOS ALMACENADOS: Empleados
-- =============================================

-- Insertar Empleado
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_InsertarEmpleado]') AND type in (N'P'))
    DROP PROCEDURE [dbo].[usp_InsertarEmpleado];
GO

CREATE PROCEDURE usp_InsertarEmpleado
    @DPI NVARCHAR(20),
    @Nombres NVARCHAR(100),
    @FechaNacimiento DATE,
    @Sexo CHAR(1),
    @FechaIngreso DATE,
    @Direccion NVARCHAR(500) = NULL,
    @NIT NVARCHAR(20) = NULL,
    @DepartamentoId INT,
    @NuevoId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM Departamentos WHERE DepartamentoId = @DepartamentoId AND Activo = 1)
    BEGIN
        RAISERROR('El departamento seleccionado no está activo.', 16, 1);
        RETURN -1;
    END
    
    INSERT INTO Empleados (
        DPI, Nombres, FechaNacimiento, Sexo, 
        FechaIngreso, Direccion, NIT, DepartamentoId
    )
    VALUES (
        @DPI, @Nombres, @FechaNacimiento, @Sexo,
        @FechaIngreso, @Direccion, @NIT, @DepartamentoId
    );
    
    SET @NuevoId = SCOPE_IDENTITY();
    
    RETURN 0;
END
GO

-- Actualizar Empleado
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ActualizarEmpleado]') AND type in (N'P'))
    DROP PROCEDURE [dbo].[usp_ActualizarEmpleado];
GO

CREATE PROCEDURE usp_ActualizarEmpleado
    @EmpleadoId INT,
    @DPI NVARCHAR(20),
    @Nombres NVARCHAR(100),
    @FechaNacimiento DATE,
    @Sexo CHAR(1),
    @FechaIngreso DATE,
    @Direccion NVARCHAR(500) = NULL,
    @NIT NVARCHAR(20) = NULL,
    @DepartamentoId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM Departamentos WHERE DepartamentoId = @DepartamentoId AND Activo = 1)
    BEGIN
        RAISERROR('El departamento seleccionado no está activo.', 16, 1);
        RETURN -1;
    END
    
    UPDATE Empleados 
    SET DPI = @DPI,
        Nombres = @Nombres,
        FechaNacimiento = @FechaNacimiento,
        Sexo = @Sexo,
        FechaIngreso = @FechaIngreso,
        Direccion = @Direccion,
        NIT = @NIT,
        DepartamentoId = @DepartamentoId,
        FechaActualizacion = GETDATE()
    WHERE EmpleadoId = @EmpleadoId;
    
    RETURN 0;
END
GO

-- Obtener Empleado por ID
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ObtenerEmpleadoPorId]') AND type in (N'P'))
    DROP PROCEDURE [dbo].[usp_ObtenerEmpleadoPorId];
GO

CREATE PROCEDURE usp_ObtenerEmpleadoPorId
    @EmpleadoId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.EmpleadoId,
        e.DPI,
        e.Nombres,
        e.FechaNacimiento,
        e.Sexo,
        e.FechaIngreso,
        e.Direccion,
        e.NIT,
        e.DepartamentoId,
        e.Activo,
        e.FechaCreacion,
        e.FechaActualizacion,
        d.Nombre AS NombreDepartamento,
        d.Activo AS DepartamentoActivo,
        DATEDIFF(YEAR, e.FechaNacimiento, GETDATE()) - 
        CASE 
            WHEN DATEADD(YEAR, DATEDIFF(YEAR, e.FechaNacimiento, GETDATE()), e.FechaNacimiento) > GETDATE() 
            THEN 1 
            ELSE 0 
        END AS Edad,
        DATEDIFF(YEAR, e.FechaIngreso, GETDATE()) AS AniosLaborando,
        DATEDIFF(MONTH, e.FechaIngreso, GETDATE()) % 12 AS MesesLaborando
    FROM Empleados e
    INNER JOIN Departamentos d ON e.DepartamentoId = d.DepartamentoId
    WHERE e.EmpleadoId = @EmpleadoId;
    
    RETURN 0;
END
GO

-- Obtener Todos los Empleados
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ObtenerTodosEmpleados]') AND type in (N'P'))
    DROP PROCEDURE [dbo].[usp_ObtenerTodosEmpleados];
GO

CREATE PROCEDURE usp_ObtenerTodosEmpleados
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.EmpleadoId,
        e.DPI,
        e.Nombres,
        e.FechaNacimiento,
        e.Sexo,
        e.FechaIngreso,
        e.Direccion,
        e.NIT,
        e.DepartamentoId,
        e.Activo,
        d.Nombre AS NombreDepartamento,
        DATEDIFF(YEAR, e.FechaNacimiento, GETDATE()) - 
        CASE 
            WHEN DATEADD(YEAR, DATEDIFF(YEAR, e.FechaNacimiento, GETDATE()), e.FechaNacimiento) > GETDATE() 
            THEN 1 
            ELSE 0 
        END AS Edad,
        DATEDIFF(YEAR, e.FechaIngreso, GETDATE()) AS AniosLaborando
    FROM Empleados e
    INNER JOIN Departamentos d ON e.DepartamentoId = d.DepartamentoId
    ORDER BY e.Nombres;
    RETURN 0;
END
GO

-- Reporte Empleados por Departamento
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ReporteEmpleadosPorDepartamento]') AND type in (N'P'))
    DROP PROCEDURE [dbo].[usp_ReporteEmpleadosPorDepartamento];
GO

CREATE PROCEDURE usp_ReporteEmpleadosPorDepartamento
    @DepartamentoId INT = NULL,
    @FechaInicio DATE = NULL,
    @FechaFin DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.EmpleadoId,
        e.DPI,
        e.Nombres,
        e.FechaNacimiento,
        e.Sexo,
        e.FechaIngreso,
        e.Direccion,
        e.NIT,
        e.Activo,
        d.DepartamentoId,
        d.Nombre AS NombreDepartamento,
        d.Activo AS DepartamentoActivo,
        DATEDIFF(YEAR, e.FechaNacimiento, GETDATE()) - 
        CASE 
            WHEN DATEADD(YEAR, DATEDIFF(YEAR, e.FechaNacimiento, GETDATE()), e.FechaNacimiento) > GETDATE() 
            THEN 1 
            ELSE 0 
        END AS Edad,
        DATEDIFF(YEAR, e.FechaIngreso, GETDATE()) AS AniosLaborando,
        DATEDIFF(MONTH, e.FechaIngreso, GETDATE()) % 12 AS MesesLaborando
    FROM Empleados e
    INNER JOIN Departamentos d ON e.DepartamentoId = d.DepartamentoId
    WHERE (@DepartamentoId IS NULL OR e.DepartamentoId = @DepartamentoId)
      AND (@FechaInicio IS NULL OR e.FechaIngreso >= @FechaInicio)
      AND (@FechaFin IS NULL OR e.FechaIngreso <= @FechaFin)
    ORDER BY d.Nombre, e.Nombres;
    
    RETURN 0;
END
GO

-- =============================================
-- DATOS INICIALES
-- =============================================

IF NOT EXISTS (SELECT 1 FROM Departamentos)
BEGIN
    INSERT INTO Departamentos (Nombre, Descripcion) VALUES
    ('Recursos Humanos', 'Gestión del personal y talento humano'),
    ('Tecnología', 'Desarrollo y soporte tecnológico'),
    ('Contabilidad', 'Gestión financiera y contable'),
    ('Ventas', 'Área comercial y atención al cliente'),
    ('Operaciones', 'Gestión de procesos operativos');
    
    PRINT 'Datos de departamentos insertados.';
END
GO

IF NOT EXISTS (SELECT 1 FROM Empleados)
BEGIN
    DECLARE @DeptId INT;
    
    SELECT @DeptId = DepartamentoId FROM Departamentos WHERE Nombre = 'Recursos Humanos';
    
    INSERT INTO Empleados (DPI, Nombres, FechaNacimiento, Sexo, FechaIngreso, Direccion, NIT, DepartamentoId) 
    VALUES ('1234567890101', 'Juan Pérez López', '1985-05-15', 'M', '2020-01-15', 'Ciudad, Zona 1', '123456-7', @DeptId);
    
    PRINT 'Datos de ejemplo insertados.';
END
GO