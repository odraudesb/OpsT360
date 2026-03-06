# OpsT360 (Prototipo Xamarin/.NET MAUI)

Prototipo móvil con login + lectura RFID/barcode de 4 sellos con hand-held scanner.

## Lo implementado

- **Login real** a `POST /api/auth/login` con `username`, `password`, `ip`, `device` (`device = web`).
- Login request serializa claves en minúscula de forma explícita (`username`, `password`, `ip`, `device`).
- Login intenta primero `http://38.242.225.119:3000/api/auth/login` y, si la red bloquea el puerto 3000, prueba fallback `http://38.242.225.119/api/auth/login`.
- Botón `Sign in` se habilita en azul solo cuando Email y Password tienen datos.
- Campo Password incluye ojito para mostrar/ocultar contraseña.
- Login navega a pantalla de sellos únicamente cuando API devuelve token válido.
- Se toma el **token** de `data.token` y se guarda en memoria.
- El token se inyecta como `Authorization: Bearer <token>` en llamadas posteriores.
- Pantalla de sellos con:
  - diseño tipo operativo (similar al mock),
  - `Activation Point` seleccionable,
  - lectura y bloqueo por cada sello (`Read Seal #n`),
  - carga de `Seal Image #n` por cada sello,
  - evidencia de imagen de contenedor,
  - modo **Editar/Reemplazar**,
  - sugerencias de contenedor al escribir,
  - validación de imágenes en Roboflow (si se configura API key),
  - envío final a `transactions/register-with-files` con multipart/form-data.
- Se arma `xmlDetails` (tipo simulador web) con datos de contenedor/sellos para registrar la transacción.

- Android habilita `cleartextTraffic` para permitir login por `http://38.242.225.119:3000` (sin TLS) durante el prototipo.

- Si sale `Failed to connect to /38.242.225.119:3000`, el problema es de conectividad/red/puerto y **no** del header `application/json`.

## Pendiente de configurar

- `RoboflowApiKey` en `Services/TransactionsService.cs`.
- Si cambian endpoints/base URL, ajustar constantes en servicios.

- Si copiaste carpetas web (`src/`, `.ts`, `.html`, `en.json`) dentro del proyecto MAUI, quedan excluidas del build para evitar errores de compilación.

## Nota de entorno

En este contenedor no está instalado el SDK de .NET (`dotnet`), por lo que no se pudo compilar aquí.


- Login usa `Source="logo-360.svg"` en XAML; coloca tu archivo `logo-360.svg` en `Resources/Images/` localmente antes de compilar.


## Uso de hand-held (pistolear)

- El hand-held funciona como **teclado externo**: escribe el código en el campo del sello que esté enfocado.
- No tiene que “salir un infrarrojo” en pantalla; eso depende del hardware del scanner, no de la app.
- En cada sello: toca `Read Seal #n` para enfocar ese sello, luego dispara con el gatillo; al recibir Enter se confirma automático.
- Si el EPC ya está escrito en el campo, `Read Seal #n` también confirma la lectura de inmediato.
