
if not exists (SELECT top 1 1 FROM sys.procedures where schema_id = SCHEMA_ID('vhs')  and name='lista_order_trabajo_pendientes')
begin
	EXEC('CREATE procedure [VHS].[lista_order_trabajo_pendientes] as  ');
	PRINT 'SP [VHS].[lista_order_trabajo_pendientes] CREATED!'
end
go
alter PROCEDURE [VHS].[lista_order_trabajo_pendientes]      
AS            
begin   
  SELECT      OrdenTrabajoID, NumeroOrden,  
'Factura : '+ NumeroFactura +char(10)+  
'Bl : ' + BL +char(10)+  
'Manifiesto : ' +Manifiesto  as Mensaje  
FROM            vhs.OrdenTrabajo  
WHERE ESTADO='P'  
end  
