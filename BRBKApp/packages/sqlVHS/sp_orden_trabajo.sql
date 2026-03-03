/*
*	sp app vhs
*	consulta data
*/
use N4Middleware
go
set ansi_nulls on;
set quoted_identifier on;
set nocount on;
go

----------------------------------------------------------------------------------------------------------------
if not exists (SELECT top 1 1 FROM sys.procedures where schema_id = SCHEMA_ID('vhs')  and name='sp_orden_trabajo')
begin
	EXEC('CREATE procedure sp_orden_trabajo as  ');
end
go
alter procedure sp_orden_trabajo
	@accion nvarchar(60),
	@id_transaccion int=-1,
	@
as
begin
	if @accion = 'C'
	begin
		select 
			OrdenTrabajoID, NumeroOrden, NumeroFactura, FechaCreacion, ClienteID, BL, Manifiesto, DescripcionProducto, UsuarioModificacion, 
			FechaCreacionRegistro, UsuarioModificacion,FechaModificacion
		from vhs.OrdenTrabajo where estado ='Creado'
	end
	if @accion='I'
	begin
		-- insert into 
		select 'i'
	end
end

-- select * from vhs.OrdenTrabajo
