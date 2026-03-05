# OpsT360 (Prototipo Xamarin/.NET MAUI)

Prototipo móvil con login + lectura RFID/barcode de 4 sellos con hand-held scanner.

## Lo implementado

- **Login real** a `POST /api/auth/login` con `username`, `password`, `ip`, `device`.
- Se toma el **token** de `data.token` y se guarda en memoria.
- El token se inyecta como `Authorization: Bearer <token>` en llamadas posteriores.
- Pantalla de sellos con:
  - lectura secuencial de 4 sellos,
  - modo **Editar/Reemplazar**,
  - sugerencias de contenedor al escribir,
  - carga de 4 imágenes de sellos + imagen de contenedor,
  - validación de imágenes en Roboflow (si se configura API key),
  - envío final a `transactions/register-with-files` con multipart/form-data.
- Se arma `xmlDetails` (tipo simulador web) con datos de contenedor/sellos para registrar la transacción.

## Pendiente de configurar

- `RoboflowApiKey` en `Services/TransactionsService.cs`.
- Si cambian endpoints/base URL, ajustar constantes en servicios.

## Nota de entorno

En este contenedor no está instalado el SDK de .NET (`dotnet`), por lo que no se pudo compilar aquí.
