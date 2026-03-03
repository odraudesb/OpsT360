-----------------------------
----- upgrade database app
------------------------------
use N4Middleware
go
set nocount on;
go

IF NOT EXISTS (SELECT TOP 1 1 FROM [N4Middleware].[brbk].[Options] WHERE Code='VHS1' and [Group] ='VHS')
BEGIN 
	insert into [brbk].[Options] (Code,[Name],Form,[Order],Create_user,Create_date,[Status],Module,[Group])
	values('VHS1','Orden deTrabajo','VHSOrdenTrabajoPage',1,'admin',GETDATE(), 1,'APP','VHS')
	insert into [brbk].[RoleOptions](RoleName,OptionName,Create_user,Create_date,Status,RoleId,OptionId)
	values('Administrador','Orden de Trabajo', 'admin', GETDATE(),1,1 ,(Select top 1 Id FROM [brbk].[Options] where code ='VHS1' and [Group] ='VHS' ))
	PRINT 'opción VHS1 - del proceso VHS inserted!'
END
GO
IF NOT EXISTS (SELECT TOP 1 1 FROM [N4Middleware].[brbk].[Options] WHERE Code='VHS2' and [Group] ='VHS')
BEGIN 
	insert into [brbk].[Options] (Code,[Name],Form,[Order],Create_user,Create_date,[Status],Module,[Group])
	values('VHS2','Detalle Tarja','VHSTarjaDetallePage',1,'admin',GETDATE(), 1,'APP','VHS')
	insert into [brbk].[RoleOptions](RoleName,OptionName,Create_user,Create_date,Status,RoleId,OptionId)
	values('Administrador','Detalle Tarja', 'admin', GETDATE(),1,1 ,(Select top 1 Id FROM [brbk].[Options] where code ='VHS2' and [Group] ='VHS' ))
	PRINT 'opción VHS2 - detalle tarja VHS inserted!'
END
GO

-- Verifica si el esquema 'VHS' existe
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.schemas WHERE name = 'VHS')
BEGIN
    -- Si no existe, crea el esquema 'VHS'
    EXEC('CREATE SCHEMA VHS');
	print 'SCHEMA VHS CREATED!';
END
go

IF NOT EXISTS(select top 1 1 from sys.tables where [schema_id]= SCHEMA_ID('VHS') AND [name]='OrdenTrabajo')
BEGIN
	-- Tabla: Orden de Trabajo
	CREATE TABLE VHS.OrdenTrabajo (
		OrdenTrabajoID INT PRIMARY KEY IDENTITY(1,1),
		NumeroOrden VARCHAR(50) UNIQUE NOT NULL,
		FechaCreacion DATETIME DEFAULT GETDATE(),
		NumeroFactura VARCHAR(50),
		BL VARCHAR(100),
		Manifiesto VARCHAR(100),
		ClienteID INT,
		DescripcionProducto VARCHAR(200),
		UsuarioCreacion VARCHAR(100),
		FechaCreacionRegistro DATETIME DEFAULT GETDATE(),
		UsuarioModificacion VARCHAR(100),
		FechaModificacion DATETIME
	);
	PRINT 'TABLE [OrdenTrabajo] CREATED!';
END
IF NOT EXISTS(select top 1 1 from sys.tables where [schema_id]= SCHEMA_ID('VHS') AND [name]='Contenedor')
begin
	-- Tabla: Contenedor
	CREATE TABLE VHS.Contenedor (
		ContenedorID INT PRIMARY KEY IDENTITY(1,1),
		IdentificadorUnico VARCHAR(100) UNIQUE NOT NULL,
		NumeroContenedor VARCHAR(100),
		NombreNave VARCHAR(100),
		ClienteID INT,
		OrdenTrabajoID INT NOT NULL,
		Vehiculo VARCHAR(50),
		Estado VARCHAR(20),
		UsuarioCreacion VARCHAR(100),
		FechaCreacionRegistro DATETIME DEFAULT GETDATE(),
		UsuarioModificacion VARCHAR(100),
		FechaModificacion DATETIME,
		FOREIGN KEY (OrdenTrabajoID) REFERENCES VHS.OrdenTrabajo(OrdenTrabajoID)
	);
	PRINT 'TABLE [Contenedor] CREATED!';
end
go
IF NOT EXISTS(select top 1 1 from sys.tables where [schema_id]= SCHEMA_ID('VHS') AND [name]='Tarja')
begin
	-- Tabla: Tarja
	CREATE TABLE VHS.Tarja (
		TarjaID INT PRIMARY KEY IDENTITY(1,1),
		OrdenTrabajoID INT NOT NULL,
		Fecha DATETIME DEFAULT GETDATE(),
		Estado VARCHAR(20) DEFAULT 'Pendiente',
		Observacion VARCHAR(100),
		Contenido VARCHAR(100),
		UsuarioCreacion VARCHAR(100),
		FechaCreacionRegistro DATETIME DEFAULT GETDATE(),
		UsuarioModificacion VARCHAR(100),
		FechaModificacion DATETIME,
		FOREIGN KEY (OrdenTrabajoID) REFERENCES VHS.OrdenTrabajo(OrdenTrabajoID)
	);
	PRINT 'TABLE [Tarja] CREATED!';
end
go
IF NOT EXISTS(select top 1 1 from sys.tables where [schema_id]= SCHEMA_ID('VHS') AND [name]='TarjaFotos')
begin
	-- Tabla: TarjaFotos
	CREATE TABLE VHS.TarjaFotos (
		TarjaFotoID INT PRIMARY KEY IDENTITY(1,1),
		TarjaID INT NOT NULL,
		FotoContenedor VARCHAR(MAX),
		UsuarioCreacion VARCHAR(100),
		FechaCreacionRegistro DATETIME DEFAULT GETDATE(),
		UsuarioModificacion VARCHAR(100),
		FechaModificacion DATETIME,
		FOREIGN KEY (TarjaID) REFERENCES VHS.Tarja(TarjaID)
	);
	PRINT 'TABLE [TarjaFotos] CREATED!';
end
go
IF NOT EXISTS(select top 1 1 from sys.tables where [schema_id]= SCHEMA_ID('VHS') AND [name]='DetalleTarja')
begin
	-- Tabla: Detalle de Tarja
	CREATE TABLE VHS.DetalleTarja (
		DetalleTarjaID INT PRIMARY KEY IDENTITY(1,1),
		TarjaID INT NOT NULL,
		TipoCargaDescripcion VARCHAR(100),
		InformacionVehiculo TEXT,
		UbicacionBodega VARCHAR(100),
		FechaRetiro DATETIME,
		DocumentoTransporte VARCHAR(100),
		PackingList VARCHAR(100),
		VIN VARCHAR(50),
		NumeroMotor VARCHAR(50),
		Estado VARCHAR(20),
		UsuarioCreacion VARCHAR(100),
		FechaCreacionRegistro DATETIME DEFAULT GETDATE(),
		UsuarioModificacion VARCHAR(100),
		FechaModificacion DATETIME,
		FOREIGN KEY (TarjaID) REFERENCES VHS.Tarja(TarjaID)
	);
	PRINT 'TABLE [DetalleTarja] CREATED!';
end
go
IF NOT EXISTS(select top 1 1 from sys.tables where [schema_id]= SCHEMA_ID('VHS') AND [name]='DetalleTarjaFotos')
begin
	-- Tabla: Detalle de Tarja Fotos
	CREATE TABLE VHS.DetalleTarjaFotos (
		FotoID INT PRIMARY KEY IDENTITY(1,1),
		DetalleTarjaID INT,
		FotosVehiculo VARCHAR(MAX),
		UsuarioCreacion VARCHAR(100),
		FechaCreacionRegistro DATETIME DEFAULT GETDATE(),
		UsuarioModificacion VARCHAR(100),
		FechaModificacion DATETIME,
		FOREIGN KEY (DetalleTarjaID) REFERENCES VHS.DetalleTarja(DetalleTarjaID)
	);
	PRINT 'TABLE [DetalleTarjaFotos] CREATED!';
end
go
IF NOT EXISTS(select top 1 1 from sys.tables where [schema_id]= SCHEMA_ID('VHS') AND [name]='PasePuerta')
begin
	-- Tabla: Pase de Puerta
	CREATE TABLE VHS.PasePuerta (
		PasePuertaID INT PRIMARY KEY IDENTITY(1,1),
		NumeroPase VARCHAR(50) UNIQUE NOT NULL,
		FechaCreacion DATETIME DEFAULT GETDATE(),
		FechaRetiro DATETIME,
		PlacaCamion VARCHAR(50),
		ChoferID VARCHAR(20),
		EmpresaTransporteID VARCHAR(20),
		Estado VARCHAR(20),
		UsuarioCreacion VARCHAR(100),
		FechaCreacionRegistro DATETIME DEFAULT GETDATE(),
		UsuarioModificacion VARCHAR(100),
		FechaModificacion DATETIME
	);
	PRINT 'TABLE [PasePuerta] CREATED!';
end
go
IF NOT EXISTS(select top 1 1 from sys.tables where [schema_id]= SCHEMA_ID('VHS') AND [name]='VehiculosDespachados')
begin
	-- Tabla: Vehículos Despachados
	CREATE TABLE VHS.VehiculosDespachados (
		VehiculoDespachadoID INT PRIMARY KEY IDENTITY(1,1),
		PasePuertaID INT NOT NULL,
		DetalleTarjaID INT NOT NULL,
		ObservacionesDespacho VARCHAR(MAX),
		FotosDespacho VARCHAR(MAX),
		FechaDespacho DATETIME DEFAULT GETDATE(),
		UsuarioCreacion VARCHAR(100),
		FechaCreacionRegistro DATETIME DEFAULT GETDATE(),
		UsuarioModificacion VARCHAR(100),
		FechaModificacion DATETIME,
		FOREIGN KEY (PasePuertaID) REFERENCES VHS.PasePuerta(PasePuertaID),
		FOREIGN KEY (DetalleTarjaID) REFERENCES VHS.DetalleTarja(DetalleTarjaID)
	);
	PRINT 'TABLE [VehiculosDespachados] CREATED!';
end
go
IF NOT EXISTS(select top 1 1 from sys.tables where [schema_id]= SCHEMA_ID('VHS') AND [name]='EvidenciaEntrega')
begin
	-- Tabla: EvidenciaEntrega
	CREATE TABLE VHS.EvidenciaEntrega (
		EvidenciaEntregaID INT PRIMARY KEY IDENTITY(1,1),
		VehiculoDespachadoID INT NOT NULL,
		FotoEntrega VARCHAR(MAX) NOT NULL,
		UsuarioCreacion VARCHAR(100),
		FechaCreacionRegistro DATETIME DEFAULT GETDATE(),
		UsuarioModificacion VARCHAR(100),
		FechaModificacion DATETIME,
		FOREIGN KEY (VehiculoDespachadoID) REFERENCES VHS.VehiculosDespachados(VehiculoDespachadoID)
	);
	PRINT 'TABLE [EvidenciaEntrega] CREATED!';
end
go

IF NOT EXISTS(select top 1 1 from sys.tables where [schema_id]= SCHEMA_ID('VHS') AND [name]='RetiroContenedorVacio')
begin
	-- Tabla: RetiroContenedorVacio
	CREATE TABLE VHS.RetiroContenedorVacio (
		RetiroContenedorVacioID INT PRIMARY KEY IDENTITY(1,1),
		ContenedorID INT NOT NULL UNIQUE,
		FechaRetiro DATETIME DEFAULT GETDATE(),
		VehiculoRetiro VARCHAR(50),
		Estado VARCHAR(20),
		UsuarioCreacion VARCHAR(100),
		FechaCreacionRegistro DATETIME DEFAULT GETDATE(),
		UsuarioModificacion VARCHAR(100),
		FechaModificacion DATETIME,
		FOREIGN KEY (ContenedorID) REFERENCES VHS.Contenedor(ContenedorID)
	);
	PRINT 'TABLE [RetiroContenedorVacio] CREATED!';
end
go

IF NOT EXISTS(Select top 1 1  from sys.objects o 
				INNER JOIN sys.columns c ON o.object_id = c.object_id
				where  o.[schema_id] = SCHEMA_ID('VHS')  AND o.[name]= 'Contenedor' and lower(c.[name])='linea')
begin
	-- Tabla: Contenedor
	ALTER TABLE VHS.Contenedor ADD Linea varchar(50) null
	PRINT 'COLUMN [LINEA] ADDED TO VHS.Contenedor!';
end
go
IF NOT EXISTS(Select top 1 1  from sys.objects o 
				INNER JOIN sys.columns c ON o.object_id = c.object_id
				where  o.[schema_id] = SCHEMA_ID('VHS')  AND o.[name]= 'Contenedor' and lower(c.[name])='tamaño')
begin
	-- Tabla: Contenedor
	ALTER TABLE VHS.Contenedor ADD Tamaño varchar(20) null
	PRINT 'COLUMN [Tamaño] ADDED TO VHS.Contenedor!';
end
go
IF NOT EXISTS(Select top 1 1  from sys.objects o 
				INNER JOIN sys.columns c ON o.object_id = c.object_id
				where  o.[schema_id] = SCHEMA_ID('VHS')  AND o.[name]= 'Contenedor' and lower(c.[name])='ruc_importador')
begin
	-- Tabla: Contenedor
	ALTER TABLE VHS.Contenedor ADD ruc_importador varchar(50) null
	PRINT 'COLUMN [ruc_importador] ADDED TO VHS.Contenedor!';
end
go
IF NOT EXISTS(Select top 1 1  from sys.objects o 
				INNER JOIN sys.columns c ON o.object_id = c.object_id
				where  o.[schema_id] = SCHEMA_ID('VHS')  AND o.[name]= 'Contenedor' and lower(c.[name])='nombre_importador')
begin
	-- Tabla: Contenedor
	ALTER TABLE VHS.Contenedor ADD nombre_importador varchar(200) null
	PRINT 'COLUMN [nombre_importador] ADDED TO VHS.Contenedor!';
end
go
IF NOT EXISTS(Select top 1 1  from sys.objects o 
				INNER JOIN sys.columns c ON o.object_id = c.object_id
				where  o.[schema_id] = SCHEMA_ID('VHS')  AND o.[name]= 'Contenedor' and lower(c.[name])='fecha_cas')
begin
	-- Tabla: Contenedor
	ALTER TABLE VHS.Contenedor ADD fecha_cas datetime null
	PRINT 'COLUMN [fecha_cas] ADDED TO VHS.Contenedor!';
end
go
IF NOT EXISTS(Select top 1 1  from sys.objects o 
				INNER JOIN sys.columns c ON o.object_id = c.object_id
				where  o.[schema_id] = SCHEMA_ID('VHS')  AND o.[name]= 'Contenedor' and lower(c.[name])='tipo_contenedor')
begin
	-- Tabla: Contenedor
	ALTER TABLE VHS.Contenedor ADD tipo_contenedor varchar(20) null
	PRINT 'COLUMN [tipo_contenedor] ADDED TO VHS.Contenedor!';
end
go
IF NOT EXISTS(Select top 1 1  from sys.objects o 
				INNER JOIN sys.columns c ON o.object_id = c.object_id
				where  o.[schema_id] = SCHEMA_ID('VHS')  AND o.[name]= 'Contenedor' and lower(c.[name])='bultos')
begin
	-- Tabla: Contenedor
	ALTER TABLE VHS.Contenedor ADD bultos decimal(18,2) null
	PRINT 'COLUMN [bultos] ADDED TO VHS.Contenedor!';
end
go
IF NOT EXISTS(Select top 1 1  from sys.objects o 
				INNER JOIN sys.columns c ON o.object_id = c.object_id
				where  o.[schema_id] = SCHEMA_ID('VHS')  AND o.[name]= 'Contenedor' and lower(c.[name])='ridt')
begin
	-- Tabla: Contenedor
	ALTER TABLE VHS.Contenedor ADD ridt bit null
	PRINT 'COLUMN [ridt] ADDED TO VHS.Contenedor!';
end
go
IF NOT EXISTS(Select top 1 1  from sys.objects o 
				INNER JOIN sys.columns c ON o.object_id = c.object_id
				where  o.[schema_id] = SCHEMA_ID('VHS')  AND o.[name]= 'Contenedor' and lower(c.[name])='manifiesto')
begin
	-- Tabla: Contenedor
	ALTER TABLE VHS.Contenedor ADD manifiesto varchar(150) null
	PRINT 'COLUMN [manifiesto] ADDED TO VHS.Contenedor!';
end
go
IF NOT EXISTS(Select top 1 1  from sys.objects o 
				INNER JOIN sys.columns c ON o.object_id = c.object_id
				where  o.[schema_id] = SCHEMA_ID('VHS')  AND o.[name]= 'Contenedor' and lower(c.[name])='bl')
begin
	-- Tabla: Contenedor
	ALTER TABLE VHS.Contenedor ADD bl varchar(50) null
	PRINT 'COLUMN [bl] ADDED TO VHS.Contenedor!';
end
go
IF NOT EXISTS(Select top 1 1  from sys.objects o 
				INNER JOIN sys.columns c ON o.object_id = c.object_id
				where  o.[schema_id] = SCHEMA_ID('VHS')  AND o.[name]= 'Contenedor' and lower(c.[name])='buque')
begin
	-- Tabla: Contenedor
	ALTER TABLE VHS.Contenedor ADD buque varchar(50) null
	PRINT 'COLUMN [buque] ADDED TO VHS.Contenedor!';
end
go
---------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT TOP 1 1 FROM [N4Middleware].[brbk].[Options] WHERE Code='VHS3' and [Group] ='VHS')
BEGIN 
	insert into [brbk].[Options] (Code,[Name],Form,[Order],Create_user,Create_date,[Status],Module,[Group])
	values('VHS3','Contenedor','VHSContenedorPage',1,'admin',GETDATE(), 1,'APP','VHS')
	insert into [brbk].[RoleOptions](RoleName,OptionName,Create_user,Create_date,Status,RoleId,OptionId)
	values('Administrador','Contenedor VHS', 'admin', GETDATE(),1,1 ,(Select top 1 Id FROM [brbk].[Options] where code ='VHS3' and [Group] ='VHS' ))
	PRINT 'opción VHS3 - del proceso VHS inserted!'
END
GO
IF NOT EXISTS(Select top 1 1  from sys.objects o 
				INNER JOIN sys.columns c ON o.object_id = c.object_id
				where  o.[schema_id] = SCHEMA_ID('VHS')  AND o.[name]= 'OrdenTrabajo' and lower(c.[name])='estado')
begin
	-- Tabla: Contenedor
	ALTER TABLE VHS.OrdenTrabajo ADD Estado varchar(20) null
	PRINT 'COLUMN [Estado] ADDED TO VHS.OrdenTrabajo!';
end
go
------------------------------------------
IF NOT EXISTS (SELECT TOP 1 1 FROM [N4Middleware].[brbk].[configurationSet] WHERE App='BRBK' and Module ='VHS' and [Name]='RUTA_IMG_VHS')
BEGIN
	INSERT INTO [N4Middleware].[brbk].[configurationSet]([App],[Module],[Name],        [Value],
		[Data_type],   [Date],      [Username],[Status])
VALUES(                                                 'BRBK','VHS',   'RUTA_IMG_VHS','\\cgdes19\app_brbk_imagen\VHS\',
		'STRING',      GETDATE(),   'admin',   1);
	PRINT 'RUTA IMAGENES VHS - CREADA!'
END
GO
IF NOT EXISTS (SELECT TOP 1 1 FROM [N4Middleware].[brbk].[configurationSet] WHERE App='BRBK' and Module ='VHS' and [Name]='TIPO_IMG')
BEGIN
	INSERT INTO [N4Middleware].[brbk].[configurationSet]([App],[Module],[Name],        [Value],
		[Data_type],   [Date],      [Username],[Status])
VALUES(                                                 'BRBK','VHS',   'TIPO_IMG','.JPG',
		'STRING',      GETDATE(),   'admin',   1);
	PRINT 'TIPO IMAGENES PARA VHS - CREADA!'
END
GO
IF NOT EXISTS (Select top 1 1 From [brbk].[Options] Where Code='FVHS')
BEGIN
	INSERT INTO [N4Middleware].[brbk].[Options]([Code],[Name],							[Form],
		[Order],   [Create_date],   [Create_user],[Status],Module,[Group])
VALUES(                                         'FVHS','Permiso para exigir Foto VHS',   'VHSTarjaCrear',
		0,			GETDATE(),		'admin',		1,  'APP'	,'VHS');
	PRINT 'PERMISO FOTO PARA VHS - CREADA!'
END
GO
--------------------------------------------------------------------------------------------------------
-------- columna orden en tarjafotos ------------------
IF NOT EXISTS(Select top 1 1  from sys.objects o 
				INNER JOIN sys.columns c ON o.object_id = c.object_id
				where  o.[schema_id] = SCHEMA_ID('VHS')  AND o.[name]= 'TarjaFotos' and lower(c.[name])='Orden')
begin
	-- Tabla: Contenedor
	ALTER TABLE VHS.TarjaFotos ADD Orden int null
	PRINT 'COLUMN [Orden] ADDED TO VHS.TarjaFotos!';
end
go
