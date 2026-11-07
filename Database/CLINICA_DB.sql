/* =====================================================================
   CLINICA_DB - TPC Programación III - Equipo 1 - Centro de Salud
   ---------------------------------------------------------------------
   Crea la base DE CERO (si ya existe, la borra) con tablas y datos de
   prueba. Si cambian una tabla, actualicen este script en el mismo PR.

   Cómo correrlo:
   - SSMS / VS Code (mssql): abrir el archivo y ejecutar todo.
   - Consola:  sqlcmd -S .\SQLEXPRESS -E -C -I -f 65001 -i Database\CLINICA_DB.sql
     (-C confía en el certificado local, como TrustServerCertificate=True;
      -f 65001 lee el archivo como UTF-8, sin eso las tildes quedan mal)
   ===================================================================== */

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;   -- necesario para los índices filtrados de TURNOS
GO

USE master;
GO

IF DB_ID('CLINICA_DB') IS NOT NULL
BEGIN
    ALTER DATABASE CLINICA_DB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE CLINICA_DB;
END
GO

CREATE DATABASE CLINICA_DB;
GO

USE CLINICA_DB;
GO

/* ---------------------------------------------------------------------
   ENTIDADES ADMINISTRABLES (Etapa 2: ABM y listado de cada una)
   Las bajas son lógicas: Activo = 0. No se borran filas.
   --------------------------------------------------------------------- */

CREATE TABLE ESPECIALIDADES (
    Id          INT IDENTITY(1,1) NOT NULL,
    Descripcion VARCHAR(50)       NOT NULL,
    Activo      BIT               NOT NULL CONSTRAINT DF_ESPECIALIDADES_Activo DEFAULT 1,
    CONSTRAINT PK_ESPECIALIDADES PRIMARY KEY (Id),
    CONSTRAINT UQ_ESPECIALIDADES_Descripcion UNIQUE (Descripcion)
);
GO

-- Horario de trabajo de los médicos. Los turnos duran 1 hora,
-- por eso entrada y salida tienen que ser horas enteras (10:00, 11:00...).
CREATE TABLE TURNOS_TRABAJO (
    Id          INT IDENTITY(1,1) NOT NULL,
    Descripcion VARCHAR(50)       NOT NULL,
    HoraEntrada TIME(0)           NOT NULL,
    HoraSalida  TIME(0)           NOT NULL,
    Activo      BIT               NOT NULL CONSTRAINT DF_TURNOS_TRABAJO_Activo DEFAULT 1,
    CONSTRAINT PK_TURNOS_TRABAJO PRIMARY KEY (Id),
    CONSTRAINT CK_TURNOS_TRABAJO_Horario CHECK (HoraEntrada < HoraSalida),
    CONSTRAINT CK_TURNOS_TRABAJO_HorasEnteras CHECK (
        DATEPART(MINUTE, HoraEntrada) = 0 AND DATEPART(SECOND, HoraEntrada) = 0 AND
        DATEPART(MINUTE, HoraSalida)  = 0 AND DATEPART(SECOND, HoraSalida)  = 0)
);
GO

-- Cada médico tiene UN turno de trabajo (consigna).
CREATE TABLE MEDICOS (
    Id             INT IDENTITY(1,1) NOT NULL,
    Nombre         VARCHAR(50)       NOT NULL,
    Apellido       VARCHAR(50)       NOT NULL,
    Matricula      VARCHAR(20)       NOT NULL,
    Email          VARCHAR(100)      NULL,
    Telefono       VARCHAR(30)       NULL,
    IdTurnoTrabajo INT               NOT NULL,
    Activo         BIT               NOT NULL CONSTRAINT DF_MEDICOS_Activo DEFAULT 1,
    CONSTRAINT PK_MEDICOS PRIMARY KEY (Id),
    CONSTRAINT UQ_MEDICOS_Matricula UNIQUE (Matricula),
    CONSTRAINT FK_MEDICOS_TURNOS_TRABAJO FOREIGN KEY (IdTurnoTrabajo) REFERENCES TURNOS_TRABAJO (Id)
);
GO

-- Un médico tiene UNA O MÁS especialidades, y una especialidad tiene
-- varios médicos: relación N a N, se resuelve con esta tabla intermedia.
CREATE TABLE MEDICOS_ESPECIALIDADES (
    IdMedico       INT NOT NULL,
    IdEspecialidad INT NOT NULL,
    CONSTRAINT PK_MEDICOS_ESPECIALIDADES PRIMARY KEY (IdMedico, IdEspecialidad),
    CONSTRAINT FK_ME_MEDICOS        FOREIGN KEY (IdMedico)       REFERENCES MEDICOS (Id),
    CONSTRAINT FK_ME_ESPECIALIDADES FOREIGN KEY (IdEspecialidad) REFERENCES ESPECIALIDADES (Id)
);
GO

-- El email es obligatorio: la consigna manda la confirmación del turno por mail.
CREATE TABLE PACIENTES (
    Id              INT IDENTITY(1,1) NOT NULL,
    Nombre          VARCHAR(50)       NOT NULL,
    Apellido        VARCHAR(50)       NOT NULL,
    DNI             VARCHAR(10)       NOT NULL,
    FechaNacimiento DATE              NOT NULL,
    Email           VARCHAR(100)      NOT NULL,
    Telefono        VARCHAR(30)       NULL,
    Direccion       VARCHAR(100)      NULL,
    Activo          BIT               NOT NULL CONSTRAINT DF_PACIENTES_Activo DEFAULT 1,
    CONSTRAINT PK_PACIENTES PRIMARY KEY (Id),
    CONSTRAINT UQ_PACIENTES_DNI UNIQUE (DNI)
);
GO

/* ---------------------------------------------------------------------
   TURNOS (core, Etapa 3)
   --------------------------------------------------------------------- */

-- Ids fijos (sin IDENTITY) para poder usarlos en el código y en los índices.
CREATE TABLE ESTADOS_TURNO (
    Id          INT         NOT NULL,
    Descripcion VARCHAR(30) NOT NULL,
    CONSTRAINT PK_ESTADOS_TURNO PRIMARY KEY (Id)
);
GO

-- Id = número de turno que se le informa al paciente.
-- Los turnos NUNCA se borran: se reprograman (se cambia Fecha/Hora del mismo
-- registro y el estado pasa a Reprogramado) o se cancelan (estado Cancelado).
CREATE TABLE TURNOS (
    Id             INT IDENTITY(1,1) NOT NULL,
    IdPaciente     INT               NOT NULL,
    IdMedico       INT               NOT NULL,
    IdEspecialidad INT               NOT NULL,
    Fecha          DATE              NOT NULL,
    Hora           TIME(0)           NOT NULL,
    Observaciones  VARCHAR(500)      NOT NULL,   -- motivo de la consulta (lo carga recepción)
    Diagnostico    VARCHAR(1000)     NULL,       -- lo carga el médico después de atender
    IdEstado       INT               NOT NULL CONSTRAINT DF_TURNOS_IdEstado DEFAULT 1,
    FechaAlta      DATETIME          NOT NULL CONSTRAINT DF_TURNOS_FechaAlta DEFAULT GETDATE(),
    CONSTRAINT PK_TURNOS PRIMARY KEY (Id),
    CONSTRAINT FK_TURNOS_PACIENTES      FOREIGN KEY (IdPaciente)     REFERENCES PACIENTES (Id),
    CONSTRAINT FK_TURNOS_MEDICOS        FOREIGN KEY (IdMedico)       REFERENCES MEDICOS (Id),
    CONSTRAINT FK_TURNOS_ESPECIALIDADES FOREIGN KEY (IdEspecialidad) REFERENCES ESPECIALIDADES (Id),
    CONSTRAINT FK_TURNOS_ESTADOS_TURNO  FOREIGN KEY (IdEstado)       REFERENCES ESTADOS_TURNO (Id),
    CONSTRAINT CK_TURNOS_HoraEntera CHECK (DATEPART(MINUTE, Hora) = 0 AND DATEPART(SECOND, Hora) = 0)
);
GO

-- Red de seguridad para la regla de la consigna: el mismo médico (o el mismo
-- paciente) no puede tener dos turnos el mismo día a la misma hora.
-- Los cancelados (IdEstado = 3) no cuentan: liberan el horario.
-- La validación principal igual va en Negocio, con un mensaje claro al usuario.
CREATE UNIQUE INDEX UX_TURNOS_Medico_Horario
    ON TURNOS (IdMedico, Fecha, Hora) WHERE IdEstado <> 3;
CREATE UNIQUE INDEX UX_TURNOS_Paciente_Horario
    ON TURNOS (IdPaciente, Fecha, Hora) WHERE IdEstado <> 3;
GO

/* ---------------------------------------------------------------------
   SEGURIDAD (Etapa final)
   --------------------------------------------------------------------- */

CREATE TABLE PERFILES (
    Id          INT         NOT NULL,
    Descripcion VARCHAR(30) NOT NULL,
    CONSTRAINT PK_PERFILES PRIMARY KEY (Id)
);
GO

-- IdMedico solo se completa para el perfil Médico (así sabe qué turnos ver).
-- Pass en texto plano por simplicidad; hashearla es una mejora posible.
CREATE TABLE USUARIOS (
    Id            INT IDENTITY(1,1) NOT NULL,
    NombreUsuario VARCHAR(50)       NOT NULL,
    Pass          VARCHAR(100)      NOT NULL,
    IdPerfil      INT               NOT NULL,
    IdMedico      INT               NULL,
    Activo        BIT               NOT NULL CONSTRAINT DF_USUARIOS_Activo DEFAULT 1,
    CONSTRAINT PK_USUARIOS PRIMARY KEY (Id),
    CONSTRAINT UQ_USUARIOS_NombreUsuario UNIQUE (NombreUsuario),
    CONSTRAINT FK_USUARIOS_PERFILES FOREIGN KEY (IdPerfil) REFERENCES PERFILES (Id),
    CONSTRAINT FK_USUARIOS_MEDICOS  FOREIGN KEY (IdMedico) REFERENCES MEDICOS (Id),
    CONSTRAINT CK_USUARIOS_Medico CHECK (
        (IdPerfil = 3 AND IdMedico IS NOT NULL) OR (IdPerfil <> 3 AND IdMedico IS NULL))
);
GO

/* =====================================================================
   DATOS DE PRUEBA
   ===================================================================== */

INSERT INTO ESTADOS_TURNO (Id, Descripcion) VALUES
    (1, 'Nuevo'), (2, 'Reprogramado'), (3, 'Cancelado'), (4, 'No asistió'), (5, 'Cerrado');

INSERT INTO PERFILES (Id, Descripcion) VALUES
    (1, 'Administrador'), (2, 'Recepcionista'), (3, 'Médico');

INSERT INTO ESPECIALIDADES (Descripcion) VALUES
    ('Clínica médica'),   -- 1
    ('Pediatría'),        -- 2
    ('Odontología'),      -- 3
    ('Cardiología'),      -- 4
    ('Dermatología'),     -- 5
    ('Traumatología');    -- 6

INSERT INTO TURNOS_TRABAJO (Descripcion, HoraEntrada, HoraSalida) VALUES
    ('Mañana',     '08:00', '13:00'),   -- 1
    ('Tarde',      '13:00', '19:00'),   -- 2
    ('Intermedio', '10:00', '16:00');   -- 3

INSERT INTO MEDICOS (Nombre, Apellido, Matricula, Email, Telefono, IdTurnoTrabajo) VALUES
    ('Laura',    'Fernández', 'MN-10234', 'lfernandez@example.com', '11-4521-3344', 1),  -- 1
    ('Martín',   'Gómez',     'MN-11872', 'mgomez@example.com',     '11-4877-1290', 2),  -- 2
    ('Sofía',    'Ramírez',   'MN-12056', 'sramirez@example.com',   NULL,           3),  -- 3
    ('Diego',    'Sosa',      'MN-13490', 'dsosa@example.com',      '11-4002-8811', 1),  -- 4
    ('Valeria',  'Torres',    'MN-14211', NULL,                     '11-4390-7765', 2),  -- 5
    ('Gustavo',  'Pereyra',   'MN-15003', 'gpereyra@example.com',   NULL,           3);  -- 6

-- Hay especialidades con más de un médico (para probar las 3 sugerencias)
-- y médicos con más de una especialidad (relación N a N).
INSERT INTO MEDICOS_ESPECIALIDADES (IdMedico, IdEspecialidad) VALUES
    (1, 1), (1, 4),    -- Fernández: Clínica médica y Cardiología
    (2, 1),            -- Gómez: Clínica médica
    (3, 2),            -- Ramírez: Pediatría
    (4, 3),            -- Sosa: Odontología
    (5, 3), (5, 5),    -- Torres: Odontología y Dermatología
    (6, 6), (6, 1);    -- Pereyra: Traumatología y Clínica médica

-- Para probar el mail de confirmación, cambien el email de algún paciente por uno propio.
INSERT INTO PACIENTES (Nombre, Apellido, DNI, FechaNacimiento, Email, Telefono, Direccion) VALUES
    ('Juan',      'Pérez',     '30123456', '1983-04-12', 'juan.perez@example.com',     '11-5555-0101', 'Av. Maipú 1200, Vicente López'),
    ('María',     'López',     '32654987', '1987-09-30', 'maria.lopez@example.com',    '11-5555-0102', 'Belgrano 450, Munro'),
    ('Carlos',    'García',    '28987321', '1979-01-05', 'carlos.garcia@example.com',  NULL,           NULL),
    ('Ana',       'Martínez',  '40112233', '1997-06-18', 'ana.martinez@example.com',   '11-5555-0104', 'Mitre 980, Florida'),
    ('Lucía',     'Romero',    '45678123', '2004-11-02', 'lucia.romero@example.com',   '11-5555-0105', NULL),
    ('Pedro',     'Díaz',      '25333444', '1975-03-22', 'pedro.diaz@example.com',     NULL,           'San Martín 300, Olivos'),
    ('Camila',    'Álvarez',   '50111222', '2010-08-14', 'camila.alvarez@example.com', '11-5555-0107', 'Laprida 77, Munro'),
    ('Jorge',     'Benítez',   '22444555', '1970-12-01', 'jorge.benitez@example.com',  '11-5555-0108', NULL),
    ('Florencia', 'Medina',    '38999000', '1994-02-27', 'flor.medina@example.com',    NULL,           'Ugarte 1500, Olivos'),
    ('Tomás',     'Acosta',    '43555666', '2001-05-09', 'tomas.acosta@example.com',   '11-5555-0110', 'Rivadavia 2100, Munro');

-- Algunos turnos con fechas relativas a HOY, para que siempre haya turnos futuros.
DECLARE @hoy DATE = CAST(GETDATE() AS DATE);

INSERT INTO TURNOS (IdPaciente, IdMedico, IdEspecialidad, Fecha, Hora, Observaciones, Diagnostico, IdEstado) VALUES
    (1, 1, 4, DATEADD(DAY,  2, @hoy), '09:00', 'Control cardiológico anual.',            NULL, 1),
    (2, 4, 3, DATEADD(DAY,  3, @hoy), '10:00', 'Dolor de muela.',                        NULL, 1),
    -- Cancelado: el horario de Sosa a las 11 queda libre (no lo bloquea el índice).
    (3, 4, 3, DATEADD(DAY,  3, @hoy), '11:00', 'Limpieza dental.',                       NULL, 3),
    (4, 3, 2, DATEADD(DAY,  5, @hoy), '12:00', 'Control pediátrico de su hijo.',         NULL, 2),
    (5, 2, 1, DATEADD(DAY, -7, @hoy), '14:00', 'Fiebre y dolor de garganta hace 3 días.', 'Faringitis. Se indica reposo y antibiótico.', 5),
    (6, 6, 6, DATEADD(DAY, -2, @hoy), '10:00', 'Dolor de rodilla al correr.',            NULL, 4);

-- Usuarios de prueba (usuario / contraseña): admin/admin, recepcion/recepcion,
-- lfernandez/medico y dsosa/medico.
INSERT INTO USUARIOS (NombreUsuario, Pass, IdPerfil, IdMedico) VALUES
    ('admin',      'admin',     1, NULL),
    ('recepcion',  'recepcion', 2, NULL),
    ('lfernandez', 'medico',    3, 1),
    ('dsosa',      'medico',    3, 4);
GO
